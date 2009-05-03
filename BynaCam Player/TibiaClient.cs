using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Tibia.Objects;
using System.Diagnostics;

namespace BynaCam_Player
{
    public class TibiaClient
    {
        private IniFile inifile;
        private static string Section = "BYNACAM";
        private static string Key = "ClientPath";

        public TibiaClient()
        {
            inifile = new IniFile(System.Windows.Forms.Application.StartupPath + @"\config.ini");
        }

        private string getClientPathFromFile()
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

        private string getClientPathFromUser()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.CheckFileExists = true;
            dialog.CheckPathExists = true;
            dialog.Multiselect = false;
            dialog.InitialDirectory = "%programfiles%\\Tibia";
            dialog.Title = "Select Tibia " + Constants.Version;
            dialog.Filter = "Exe files|*.exe";
            dialog.ShowDialog();
            return dialog.FileName;
        }

        private void setClientPath(string path)
        {
            inifile.IniWriteValue(Section, Key, path);
        }

        public Client getClient()
        {
            string ClientFile = string.Empty;
            ClientFile = getClientPathFromFile();
            if (ClientFile == null)
            {
                string path = getClientPathFromUser();
                if (path == null || path == "")
                {
                    return null;
                }
                else
                {
                    setClientPath(path);
                    ClientFile = getClientPathFromFile();
                }
            }
            if (FileVersionInfo.GetVersionInfo(ClientFile).FileVersion != Constants.Version)
            {
                return null;
            }
            return Client.OpenMC(ClientFile, "engine 0");
        }
    }
}
