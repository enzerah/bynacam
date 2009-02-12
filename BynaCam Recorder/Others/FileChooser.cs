using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tibia.Objects;
using System.Diagnostics;

namespace BynaCam_Recorder
{
    public static class FileChooser
    {
        public static string getBynaCamFile(Client c)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "BynaCam files|*.byn";

            if (dialog.ShowDialog(new WindowWrapper(c.MainWindowHandle)) == DialogResult.Cancel)
            {
                MessageBox.Show(null, "You must choose file to save!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Process.GetCurrentProcess().Kill();
                try { c.Process.Kill(); }
                catch { }
            }

            return dialog.FileName;
        }
    }
}
