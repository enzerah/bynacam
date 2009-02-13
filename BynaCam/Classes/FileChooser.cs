using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using Tibia.Objects;

namespace BynaCam
{
    public static class FileChooser
    {
        public static string getCamFilePath(Client client)
        {
            //Open File Dialog
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.CheckFileExists = true;
            dialog.Multiselect = false;
            dialog.Filter = "BynaCam Files|*.byn";
            dialog.Title = "Open BynaCam file.";
            if (dialog.ShowDialog(new WindowWrapper(client.MainWindowHandle)) == DialogResult.Cancel)
            {
                MessageBox.Show(new WindowWrapper(client.MainWindowHandle), "Your must select your BynaCam file!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                client.Process.Kill();
                Process.GetCurrentProcess().Kill();
            }

            return dialog.FileName;
        }
    }
}
