using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BynaCam_Recorder
{
    public partial class optionsForm : Form
    {
        IniFile inifile;
        
        public optionsForm()
        {
            InitializeComponent();
            inifile = new IniFile(System.Windows.Forms.Application.StartupPath + @"\config.ini");
            tb_path.Text = inifile.IniReadValue("BYNACAM", "MoviesPath");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.ShowNewFolderButton = true;
            dialog.SelectedPath = Environment.CurrentDirectory;
            dialog.ShowDialog(new WindowWrapper(this.Handle));
            tb_path.Text = dialog.SelectedPath;
            inifile.IniWriteValue("BYNACAM", "MoviesPath", tb_path.Text);
        }
    }
}
