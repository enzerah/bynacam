using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;

namespace Player
{
    public class Files
    {
        public static string GetClientPath(byte[] version)
        {
            ClientPath cPath = new ClientPath(version);
            return cPath.GetClientPath();
        }
        public static string GetMoviePath()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.CheckFileExists = true;
            dialog.CheckPathExists = true;
            dialog.Multiselect = false;
            dialog.Title = "Select BynaCam movie.. ";
            dialog.Filter = "BynaCam movies|*.byn";
            dialog.ShowDialog(new WindowWrapper(WinApi.GetForegroundWindow()));
            return dialog.FileName;
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
    public class ClientPath
    {
        IniFile inifile;
        string Section = "Clients";
        string Key = "";

        public ClientPath(byte[] version)
        {
            inifile = new IniFile(System.Windows.Forms.Application.StartupPath + @"\clients.ini");

            //convert it to string
            Key += version[0].ToString();
            Key += ".";
            Key += version[1].ToString();
            Key += version[2].ToString();
        }

        private string GetClientPathFromFile()
        {
            string iniRet = inifile.IniReadValue(Section, Key);
            if (File.Exists(iniRet))
            {
                return iniRet;
            }
            else
            {
                return null;
            }
        }

        private string GetClientPathFromUser()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.CheckFileExists = true;
            dialog.CheckPathExists = true;
            dialog.Multiselect = false;
            dialog.InitialDirectory = "%programfiles%\\Tibia";
            dialog.Title = "Select Tibia " + Key;
            dialog.Filter = "Exe files|*.exe";
            dialog.ShowDialog(new WindowWrapper(WinApi.GetForegroundWindow()));
            return dialog.FileName;
        }

        private void SetClientPath(string path)
        {
            inifile.IniWriteValue(Section, Key, path);
        }

        public string GetClientPath()
        {
            string ClientFile = string.Empty;
            ClientFile = GetClientPathFromFile();
            if (ClientFile == null || ClientFile == "")
            {
                string UserPath = GetClientPathFromUser();
                if (UserPath == null || UserPath == "")
                {
                    return null;
                }
                else
                {
                    SetClientPath(UserPath);
                    ClientFile = UserPath;
                }
            }
            if (FileVersionInfo.GetVersionInfo(ClientFile).FileVersion != Key)
            {
                inifile.IniWriteValue(Section, Key, "");
                MessageBox.Show(new WindowWrapper(WinApi.GetForegroundWindow()), "Wrong Tibia Version!!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            return ClientFile;
        }
    }

}
