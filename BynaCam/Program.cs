using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tibia;
using Tibia.Objects;
using Tibia.Util;
using System.Diagnostics;
using Tibia.Packets;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace BynaCam
{
    class MainClass
    {
        static Client client;
        static int speed = 1;
        static StreamReader stream;

        private static Client getIniClient()
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
                MessageBox.Show("Tibia Client not found! Choose new one..", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                OpenFileDialog tibiaDialog = new OpenFileDialog();
                tibiaDialog.CheckFileExists = true;
                tibiaDialog.CheckPathExists = true;
                tibiaDialog.InitialDirectory = "C:\\Program Files\\Tibia\\";
                tibiaDialog.Filter = "Exe files|*.exe";
                tibiaDialog.Multiselect = false;

                if (tibiaDialog.ShowDialog(new WindowWrapper(client.MainWindowHandle)) == DialogResult.OK)
                {
                    inifile.IniWriteValue("CLIENT", "Path", tibiaDialog.FileName);
                }
            }

            return getIniClient();
        }

        private static Stream getCamFileStream()
        {
            //Open File Dialog
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.CheckFileExists = true;
            dialog.Multiselect = false;
            dialog.Filter = "BynaCam Files|*.byn";
            dialog.Title = "Open BynaCam file.";
            if (dialog.ShowDialog(new WindowWrapper(client.MainWindowHandle)) == DialogResult.Cancel)
            {
                MessageBox.Show("Cannot open BynaCam file!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Process.GetCurrentProcess().Kill();
            }
            return dialog.OpenFile();
        }

        private static void updateClientTitle()
        {
            try
            {
                client.Title = "BynaCam -> speed: x" + speed;
            }
            catch { }
        }

        private static void setUpKeyboardHook(Client client)
        {
            KeyboardHook.Enable();
            KeyboardHook.KeyDown = null;
            KeyboardHook.KeyDown += new KeyboardHook.KeyboardHookHandler(delegate(Keys key)
            {
                if (client.IsActive)
                {
                    if (key == Keys.Right)
                    {
                        if (speed == 50)
                            return false;
                        speed++;
                        updateClientTitle();
                    }
                    if (key == Keys.Left)
                    {
                        if (speed == 1)
                            return false;
                        speed--;
                        updateClientTitle();
                    }
                    if (key == Keys.Up)
                    {
                        speed = 50;
                        updateClientTitle();
                    }
                    if (key == Keys.Down)
                    {
                        speed = 1;
                        updateClientTitle();
                    }

                    if (key == Keys.Left || key == Keys.Right
                        || key == Keys.Down || key == Keys.Up)
                        return false;
                }
                return true;
            });
            KeyboardHook.KeyDown += null;
        }

        [STAThreadAttribute]
        static void Main(string[] args)
        {
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;

            client = getIniClient();
            stream = new StreamReader(getCamFileStream());
            
            if (client != null)
            {
                string TibiaVersion = stream.ReadLine();
                if (TibiaVersion != client.Version)
                {
                    MessageBox.Show("This movie don't support your Tibia Client!");
                    client.Process.Kill();
                    Process.GetCurrentProcess().Kill();
                }

                TibiaNetwork Network = new TibiaNetwork(client);
                updateClientTitle();
                client.Exited += new EventHandler(client_Exited);
                client.AutoLogin("111", "111", "Byna", "BynaCam");

                Thread.Sleep(2000);
                if (!Network.uxGameServer.Accepted)
                {
                    client.Process.Kill();
                    Process.GetCurrentProcess().Kill();
                }
                else
                {
                    TimeSpan time;
                    byte[] packet;

                    new Thread(new ThreadStart(delegate()
                    {
                        while (!stream.EndOfStream)
                        {
                            time = TimeSpan.Parse(stream.ReadLine());
                            packet = stream.ReadLine().ToBytesAsHex();

                            if (packet == null)
                                continue;

                            if (packet[0] == 0x14) //disconnectclient 0x14
                                continue;

                            if (packet[0] == 0x65 || packet[0] == 0x66 || packet[0] == 0x67 || packet[0] == 0x68
                                 || packet[0] == (byte)IncomingPacketType.MapDescription
                                 || packet[0] == (byte)IncomingPacketType.SelfAppear
                                 || packet[0] == (byte)IncomingPacketType.WorldLight)
                            {
                                Network.uxGameServer.Send(packet);
                                continue;
                            }
                            else
                            {
                                Thread.Sleep(time.Milliseconds / speed);
                            }

                            if (packet[0] == 0xc8)//setoufit block
                                return;
                            if (packet[0] == (byte)IncomingPacketType.ChannelList)
                                return; //channellist block

                            Network.uxGameServer.Send(packet);
                        }
                        Thread.Sleep(3000);
                        Process.GetCurrentProcess().Kill();
                    })).Start();

                    while (true)
                    {
                        setUpKeyboardHook(client);
                        updateClientTitle();
                    }
                }
            }
            else
            {
                MessageBox.Show("Could not load Tibia Client!");
                Process.GetCurrentProcess().Kill();
            }
        }

        private static void client_Exited(object sender, EventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }
    }

    public class WindowWrapper : System.Windows.Forms.IWin32Window
    {
        public WindowWrapper(IntPtr handle)
        {
            _hwnd = handle;
        }

        public IntPtr Handle
        {
            get { return _hwnd; }
        }

        private IntPtr _hwnd;
    }
}
