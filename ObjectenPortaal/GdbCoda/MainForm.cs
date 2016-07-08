using System;
using System.IO;
using System.Windows.Forms;

namespace GdbCoda
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var directoryInfo = new DirectoryInfo(textBox1.Text);
            if (!directoryInfo.Exists)
            {
                MessageBox.Show($"Folder {textBox1.Text} niet gevonden");
                return;
            }
            if (MessageBox.Show(this, $"Start verwerking {directoryInfo.GetDirectories("*.gdb", SearchOption.TopDirectoryOnly).Length} geodatabases", "", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                textBox2.Text = "Gestart";
                RenameCodaFields.Rename(textBox1.Text, logText =>
                {
                    textBox2.Text = textBox2.Text + Environment.NewLine + logText;
                    Refresh();
                });
                textBox2.Text = "Klaar!";
            }
        }
    }
}