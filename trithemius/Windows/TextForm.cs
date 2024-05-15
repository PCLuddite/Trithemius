// =====
//
// Copyright (c) 2013-2020 Timothy Baxendale
//
// =====
using System;
using System.Windows.Forms;

namespace Trithemius.Windows
{
    public partial class TextForm : Form
    {
        public string Result { get; private set; }
        public int CharacterLimit
        {
            get => textBox.MaxLength;
            set {
                textBox.MaxLength = value;
                lblCount.Text = $"{textBox.TextLength}/{value}";
            }
        }

        public TextForm()
        {
            InitializeComponent();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Result = textBox.Text;
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            lblCount.Text = $"{textBox.TextLength}/{CharacterLimit}";
        }
    }
}
