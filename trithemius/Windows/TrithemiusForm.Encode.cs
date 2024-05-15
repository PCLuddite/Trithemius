// =====
//
// Copyright (c) 2013-2020 Timothy Baxendale
//
// =====
using System;
using System.ComponentModel;
using System.IO;
using System.Security;
using System.Text;
using System.Windows.Forms;

using Monk.Encryption;
using Monk.Imaging;

namespace Trithemius.Windows
{
    partial class TrithemiusForm
    {
        private struct EncodeArgs
        {
            public SteganographyInfo SteganographyInfo { get; }
            public string Password { get; }
            public string Data { get; }
            public bool IsText { get; }
            public string OutputPath { get; }
            public string ImagePath { get; }
            public bool PrefixSize { get; }

            public EncodeArgs(SteganographyInfo info, string imagePath, string data, bool isText, string pass, string outputPath, bool prefixSize)
            {
                ImagePath = imagePath;
                SteganographyInfo = info;
                Password = pass;
                Data = data;
                IsText = isText;
                OutputPath = outputPath;
                PrefixSize = prefixSize;
            }
        }

        private struct EncodeResult
        {
            public bool Success { get; }
            public string Message { get; }

            public EncodeResult(bool success, string message)
            {
                Success = success;
                Message = message;
            }
        }

        private void buttonEncode_Click(object sender, EventArgs e)
        {
            string message = null;
            if (radioButtonFile.Checked) {
                if (msgOpenDialog.ShowDialog(this) == DialogResult.OK) {
                    message = msgOpenDialog.FileName;
                }
            }
            else {
                _inputTextForm.CharacterLimit = Encoding.UTF8.GetMaxCharCount(AvailableBits / 8);
                if (_inputTextForm.ShowDialog(this) == DialogResult.OK) {
                    message = _inputTextForm.Result;
                }
            }
            if (message != null && saveFileDialog.ShowDialog(this) == DialogResult.OK) {
                SteganographyInfo trithemius = CreateTrithemius();
                if (trithemius != null) {
                    SetEnabled(false);
                    encodeWorker.RunWorkerAsync(new EncodeArgs(trithemius, ImagePath, message, radioButtonText.Checked,
                        textBoxKey.Text, saveFileDialog.FileName, checkBoxPrefixSize.Checked || IsLegacyMode()));
                }
            }
        }

        private void encodeWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try {
                EncodeArgs args = (EncodeArgs)e.Argument;
                using (Steganographer trithemius = new Steganographer(ImagePath, args.SteganographyInfo)) {
                    byte[] data;
                    if (args.IsText) {
                        data = Encoding.UTF8.GetBytes(args.Data);
                    }
                    else {
                        data = File.ReadAllBytes(args.Data);
                    }
                    if (!string.IsNullOrEmpty(args.Password)) {
                        data = AESThenHMAC.SimpleEncryptWithPassword(data, args.Password);
                    }
                    trithemius.Encode(data, args.PrefixSize);
                    trithemius.SaveImage(args.OutputPath);
                    e.Result = new EncodeResult(true, args.OutputPath);
                }
            }
            catch(Exception ex) when (ex is EncoderFallbackException || ex is IOException || ex is SecurityException || ex is InvalidImageOptionException) {
                e.Result = new EncodeResult(false, ex.Message);
            }
        }

        private void encodeWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            SetEnabled(true);
            EncodeResult result = (EncodeResult)e.Result;
            if (result.Success) {
                ShowInfo("Encoding completed! You should spot no visible image differences.");
                if (result.Message == ImagePath) {
                    ReloadPreview(ImagePath);
                }
            }
            else { 
                ShowError(result.Message);
            }
        }
    }
}
