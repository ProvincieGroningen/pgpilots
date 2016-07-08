using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace GdbCodaInventarisatie
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var folderName = textBox1.Text;
            dataGridView1.DataSource = null;
            if (new DirectoryInfo(folderName).Exists)
            {
                var result = InspectCodaFields.Rename(folderName, fileName =>
                {
                    label2.Text = fileName;
                    Refresh();
                }).ToArray();
                dataGridView1.DataSource = result;
                label2.Text = "";
            }
            else
            {
                MessageBox.Show($"Folder {folderName} bestaat niet.");
            }
        }
    }
}