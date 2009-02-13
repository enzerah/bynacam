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
        public static Client getIniClient()
        {
            IniFile inifile = new IniFile(System.Windows.Forms.Application.StartupPath + @"\config.ini");
            if (File.Exists(inifile.IniReadValue("CLIENT", "Path")))
            {
                WinApi.PROCESS_INFORMATION pi = new Tibia.Util.WinApi.PROCESS_INFORMATION();
                WinApi.STARTUPINFO si = new Tibia.Util.WinApi.STARTUPINFO();

                WinApi.CreateProcess(inifile.IniReadValue("CLIENT", "Path"), "", IntPtr.Zero, IntPtr.Zero,
                    false, WinApi.CREATE_SUSPENDED, IntPtr.Zero,
                    System.IO.Path.GetDirectoryName(inifile.IniReadValue("CLIENT", "Path")), ref si, out pi);

                IntPtr handle = WinApi.OpenProcess(WinApi.PROCESS_ALL_ACCESS, 0, pi.dwProcessId);
                Process p = Process.GetProcessById(Convert.ToInt32(pi.dwProcessId));
                Memory.WriteByte(handle, (long)Tibia.Addresses.Client.DMultiClient, Tibia.Addresses.Client.DMultiClientJMP);
                WinApi.ResumeThread(pi.hThread);
                p.WaitForInputIdle();
                Memory.WriteByte(handle, (long)Tibia.Addresses.Client.DMultiClient, Tibia.Addresses.Client.DMultiClientJNZ);
                WinApi.CloseHandle(handle);
                WinApi.CloseHandle(pi.hProcess);
                WinApi.CloseHandle(pi.hThread);
                return new Client(p);
            }
            else
            {
                DialogResult res = Messages.OKCancel("Tibia Client not found! Choose new one..", "Information!", MessageBoxButtons.OKCancel);
                if (res == DialogResult.Cancel) Process.GetCurrentProcess().Kill();
                OpenFileDialog tibiaDialog = new OpenFileDialog();
                tibiaDialog.CheckFileExists = true;
                tibiaDialog.CheckPathExists = true;
                tibiaDialog.InitialDirectory = "C:\\Program Files\\Tibia\\";
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

            return getIniClient();
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
