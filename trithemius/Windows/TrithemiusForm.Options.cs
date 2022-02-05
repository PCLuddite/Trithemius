// =====
//
// Copyright (c) 2013-2020 Timothy Baxendale
//
// =====
using System;
using System.Drawing;
using System.Drawing.Imaging;

using Monk.Imaging;
using Monk.Memory.Bittwiddling;

namespace Trithemius.Windows
{
    partial class TrithemiusForm
    { 
        private void RefreshOptions()
        {
            Image image = pictureBox.Image;
            if (image == null || "Refresh" == buttonBrowse.Text) {
                groupBoxEncode.Enabled = false;
                buttonEncode.Enabled = false;
                buttonDecode.Enabled = false;
            }
            else {
                buttonEncode.Enabled = true;
                buttonDecode.Enabled = true;
                groupBoxEncode.Enabled = true;
                RefreshColorOptions(image.PixelFormat);
            }
        }

        private void numericUpDownLsb_ValueChanged(object sender, EventArgs e)
        {
            textBoxMaxSize.Text = AvailableBits.ToString("#,##0");
        }

        private void RefreshColorOptions(PixelFormat format)
        {
            switch (format) {
                case PixelFormat.Format32bppArgb: 
                    checkAlpha.Enabled = true;
                    checkRed.Enabled = true;
                    checkGreen.Enabled = true;
                    checkBlue.Enabled = true;
                    break;
                case PixelFormat.Format24bppRgb:
                    checkAlpha.Enabled = false;
                    checkRed.Enabled = true;
                    checkGreen.Enabled = true;
                    checkBlue.Enabled = true;
                    break;
                case PixelFormat.Format8bppIndexed:
                    checkAlpha.Enabled = false;
                    checkRed.Enabled = false;
                    checkGreen.Enabled = false;
                    checkBlue.Enabled = true;
                    break;
                default:
                    checkAlpha.Enabled = false;
                    checkRed.Enabled = false;
                    checkGreen.Enabled = false;
                    checkBlue.Enabled = false;
                    break;
            }
        }
        
        private void comboBoxVersions_SelectedIndexChanged(object sender, EventArgs e)
        {
            buttonEncode.Enabled = groupBoxStartPixel.Enabled = groupBoxPrefix.Enabled = 
                groupBoxInvert.Enabled = groupBoxEndian.Enabled = groupBoxLsb.Enabled = 
                checkAlpha.Enabled = !IsLegacyMode();

            if (IsLegacyMode()) {
                SetOptions(CreateTrithemius());
                checkAlpha.Checked = false;
                checkRed.Checked = true;
                checkGreen.Checked = false;
                checkBlue.Checked = false;
            }
            RefreshColorOptions(pictureBox.Image?.PixelFormat ?? PixelFormat.Format32bppArgb);
        }

        private void checkAlpha_CheckedChanged(object sender, EventArgs e)
        {
            if (IsLegacyMode() && checkAlpha.Checked) {
                checkRed.Checked = checkGreen.Checked = checkBlue.Checked = false;
            }
        }

        private void checkRed_CheckedChanged(object sender, EventArgs e)
        {
            if (IsLegacyMode() && checkRed.Checked) {
                checkAlpha.Checked = checkGreen.Checked = checkBlue.Checked = false;
            }
        }

        private void checkGreen_CheckedChanged(object sender, EventArgs e)
        {
            if (IsLegacyMode() && checkGreen.Checked) {
                checkAlpha.Checked = checkRed.Checked = checkBlue.Checked = false;
            }
        }

        private void checkBlue_CheckedChanged(object sender, EventArgs e)
        {
            if (IsLegacyMode() && checkBlue.Checked) {
                checkAlpha.Checked = checkRed.Checked = checkGreen.Checked = false;
            }
        }

        private void buttonReset_Click(object sender, EventArgs e)
        {
            comboBoxVersions.SelectedIndex = 0;
            comboBoxEndian.SelectedIndex = 0;
            numericUpDownOffset.Value = 1;
            numericUpDownLsb.Value = 1;
            checkBoxInvertData.Checked = false;
            checkBoxZeroBased.Checked = false;
            checkBoxInvertPrefix.Checked = false;
            checkBoxPrefixSize.Checked = true;
            checkAlpha.Checked = checkAlpha.Enabled;
            checkRed.Checked = checkRed.Enabled;
            checkGreen.Checked = checkGreen.Enabled;
            checkBlue.Checked = checkBlue.Enabled;
        }

        private void SetOptions(SteganographyInfo info)
        {
            comboBoxEndian.SelectedIndex = info.Endianness == EndianMode.BigEndian ? 1 : 0;
            numericUpDownOffset.Value = info.Offset + 1;
            numericUpDownLsb.Value = info.LeastSignificantBits;
            checkBoxZeroBased.Checked = info.ZeroBasedSize;
            checkBoxInvertData.Checked = info.InvertDataBits;
            checkBoxInvertPrefix.Checked = info.InvertPrefixBits;
            checkAlpha.Checked = info.Colors.Contains(PixelColor.Alpha);
            checkRed.Checked = info.Colors.Contains(PixelColor.Red);
            checkGreen.Checked = info.Colors.Contains(PixelColor.Green);
            checkBlue.Checked = info.Colors.Contains(PixelColor.Blue);
        }
    }
}
