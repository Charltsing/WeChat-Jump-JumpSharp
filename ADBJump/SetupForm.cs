using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ShowAndroidModel
{
    public partial class SetupForm : Form
    {
        public SetupForm()
        {
            InitializeComponent();
        }

        private void SetupForm_Load(object sender, EventArgs e)
        {
            if (System.IO.File.Exists(string.Format(@"{0}\Android\android-sdk\platform-tools\adb.exe", System.Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles))))
            {
                textBox1.Text = string.Format(@"{0}\Android\android-sdk\platform-tools\adb.exe", System.Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles));
            }
            if (System.IO.File.Exists(string.Format(@"{0}\Android\android-sdk\platform-tools\adb.exe", System.Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86))))
            {
                textBox1.Text = string.Format(@"{0}\Android\android-sdk\platform-tools\adb.exe", System.Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86));
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.FileName = "adb.exe";
                dialog.Filter = "adb.exe|adb.exe";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    textBox1.Text = dialog.FileName;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (System.IO.File.Exists( textBox1.Text))
            {
                System.IO.File.WriteAllText("config.db", textBox1.Text);
                this.DialogResult = DialogResult.OK;

                this.Close();
            }
            else
            {
                MessageBox.Show(string.Format("{0}文件不存在", textBox1.Text));
            }
        }
    }
}
