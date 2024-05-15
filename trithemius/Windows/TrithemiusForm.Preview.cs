// =====
//
// Copyright (c) 2013-2020 Timothy Baxendale
//
// =====
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;

namespace Trithemius.Windows
{
    partial class TrithemiusForm
    {
        private string ImagePath { get; set; }
        
        private Image CurrentImage => pictureBox.Image;
        private int ImageArea => CurrentImage == null ? 0 : CurrentImage.Width * CurrentImage.Height;
        public int AvailableBytes => CurrentImage == null ? 0 : (Image.GetPixelFormatSize(CurrentImage.PixelFormat) / 8) * ImageArea;
        public int AvailableBits => AvailableBytes * (int)numericUpDownLsb.Value;

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            if ("Browse" == ((Button)sender).Text) {
                string path = Browse();
                if (path == null) {
                    return;
                }
                else {
                    ImagePath = path;
                    pathTextBox.Text = openFileDialog.FileName;
                }
            }
            ImagePath = pathTextBox.Text;
            if (!string.IsNullOrEmpty(ImagePath)) {
                try {
                    ReloadPreview(ImagePath);
                    RefreshOptions();
                }
                catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException) {
                    ShowError(ex.Message);
                    pathTextBox.SelectAll();
                    pathTextBox.Focus();
                }
            }
        }

        private string Browse()
        {
            if (!string.IsNullOrWhiteSpace(pathTextBox.Text) && pathTextBox.Text != openFileDialog.FileName) {
                try {
                    openFileDialog.FileName = Path.GetFileName(pathTextBox.Text);
                    openFileDialog.InitialDirectory = Path.GetDirectoryName(pathTextBox.Text);
                }
                catch (Exception ex) when (ex is PathTooLongException || ex is ArgumentException) {
                    // ignore
                }
            }

            if (openFileDialog.ShowDialog(this) == DialogResult.OK) {
                return openFileDialog.FileName;
            }
            else {
                return null;
            }
        }

        private void pathTextBox_TextChanged(object sender, EventArgs e)
        {
            buttonBrowse.Text = pathTextBox.Text == ImagePath || pathTextBox.Text == "" ? "Browse" : "Refresh";
            RefreshOptions();
        }

        private void ReloadPreview(string path)
        {
            Image newimage = LoadImage(path);
            if (pictureBox.Image != null) {
                Image oldimage = pictureBox.Image;
                pictureBox.Image = null;
                oldimage.Dispose();
            }
            pictureBox.Image = newimage;
            ReloadDetails();
            RefreshOptions();
        }

        private void ReloadDetails()
        {
            string format = CurrentImage.PixelFormat.ToString();
            if (format.StartsWith("Format")) format = format.Substring("Format".Length);
            textBoxFormat.Text = format;
            textBoxWidth.Text = $"{CurrentImage.Width} px";
            textBoxHeight.Text = $"{CurrentImage.Height} px";
            textBoxMaxSize.Text = AvailableBits.ToString("#,##0");
            numericUpDownOffset.Value = Math.Min(numericUpDownOffset.Value, ImageArea);
            numericUpDownOffset.Maximum = ImageArea;
        }
    }
}
