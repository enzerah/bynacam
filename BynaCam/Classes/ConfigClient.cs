using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Tibia;
using Tibia.Objects;
using Tibia.Util;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;

namespace BynaCam
{
    public static class ConfigClient
    {
        public static string getClientPath()
        {
            IniFile inifile = new IniFile(System.Windows.Forms.Application.StartupPath + @"\config.ini");
            if (File.Exists(inifile.IniReadValue("CLIENT", "Path")))
            {
                return inifile.IniReadValue("CLIENT", "Path");
            }
            else
            {
                DialogResult res = Messages.OKCancel("Tibia Client not found! Choose new one..", "Information!", MessageBoxButtons.OKCancel);
                if (res == DialogResult.Cancel) Process.GetCurrentProcess().Kill();
                OpenFileDialog tibiaDialog = new OpenFileDialog();
                tibiaDialog.CheckFileExists = true;
                tibiaDialog.CheckPathExists = true;
                tibiaDialog.InitialDirectory = "%programfiles%\\Tibia\\";
                tibiaDialog.Filter = "Exe files|*.exe";
                tibiaDialog.Multiselect = false;

                if (tibiaDialog.ShowDialog(new WindowWrapper(Messages.GetForegroundWindow())) == DialogResult.OK)
                {
                    if (FileVersionInfo.GetVersionInfo(tibiaDialog.FileName).FileVersion != "8.40")
                    {
                        Messages.Error("Only Tibia Client 8.4 allowed!!");
                        Process.GetCurrentProcess().Kill();
                    }
                    inifile.IniWriteValue("CLIENT", "Path", tibiaDialog.FileName);
                }
            }

            return getClientPath();
        }
    }
        public class IniFile
        {
            public string path;
            
            [DllImport("kernel32")]
            private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
            [DllImport("kernel32")]
            private static extern int GetPrivateProfileString(string section,
                string key, string def, StringBuilder retVal, int size, string filePath);

            public IniFile(string INIPath)
            {
                path = INIPath;
            }
            public void IniWriteValue(string Section, string Key, string Value)
            {
                WritePrivateProfileString(Section, Key, Value, this.path);
            }

            public string IniReadValue(string Section, string Key)
            {
                StringBuilder temp = new StringBuilder(255);
                int i = GetPrivateProfileString(Section, Key, "", temp, 255, this.path);
                return temp.ToString();

            }
        }
}
