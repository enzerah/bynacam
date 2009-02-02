using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tibia;
using Tibia.Objects;
using Tibia.Util;
using System.Diagnostics;
using Tibia.Packets;
using System.IO;
using System.Threading;

namespace BynaCam
{
    public partial class Form1 : Form
    {
        Client client;
       // static string[] args;
        StreamReader stream;

        public Form1(string[] arguments)
        {
            InitializeComponent();
            //arguments = args;
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.CheckFileExists = true;
            dialog.Multiselect = false;
            dialog.Filter = "BynaCam Files|*.byn";
            dialog.Title = "Open BynaCam file.";
            if (dialog.ShowDialog() == DialogResult.Cancel)
            {
                MessageBox.Show("You must choose your file", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Process.GetCurrentProcess().Kill();
            }
            stream = new StreamReader(dialog.OpenFile());

            WinApi.PROCESS_INFORMATION pi = new Tibia.Util.WinApi.PROCESS_INFORMATION();
            WinApi.STARTUPINFO si = new Tibia.Util.WinApi.STARTUPINFO();

            WinApi.CreateProcess(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), @"Tibia\tibia.exe"), " ", IntPtr.Zero, IntPtr.Zero,
                false, WinApi.CREATE_SUSPENDED, IntPtr.Zero,
                System.IO.Path.GetDirectoryName(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), @"Tibia\tibia.exe")), ref si, out pi);

            IntPtr handle = WinApi.OpenProcess(WinApi.PROCESS_ALL_ACCESS, 0, pi.dwProcessId);
            Process p = Process.GetProcessById(Convert.ToInt32(pi.dwProcessId));
            Memory.WriteByte(handle, (long)Tibia.Addresses.Client.DMultiClient, Tibia.Addresses.Client.DMultiClientJMP);
            WinApi.ResumeThread(pi.hThread);
            p.WaitForInputIdle();
            Memory.WriteByte(handle, (long)Tibia.Addresses.Client.DMultiClient, Tibia.Addresses.Client.DMultiClientJNZ);
            WinApi.CloseHandle(handle);
            WinApi.CloseHandle(pi.hProcess);
            WinApi.CloseHandle(pi.hThread);

            client = new Client(p);

            if (client != null)
            {
                client.Exited += new EventHandler(client_Exited);
                client.Title = "BynaCam";
                GameServer g = new GameServer(client);

                Thread t = new Thread(new ThreadStart(delegate()
                    {
                        LoginServer.Login l = new LoginServer.Login();
                        l.StartServer(client, 7171, "Byna", "BynaCam", new byte[] { 127, 0, 0, 1 }, 7172);
                        g.SetServer(7172);
                    }));
                t.Start();

                client.AutoLogin("111", "111", "Byna", "BynaCam");

                Thread.Sleep(2000);

                if (!g.Accepted)
                {
                    client.Process.Kill();
                    Process.GetCurrentProcess().Kill();
                }
                else
                {
                    TimeSpan time;
                    byte[] packet;

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
                            g.sendpacket(packet);
                            continue;
                        }
                        else
                        {
                            Thread.Sleep(time);       
                        }

                        if (packet[0] == 0xc8)//setoufit block
                            return;
                        if (packet[0] == (byte)IncomingPacketType.ChannelList)
                            return; //channellist block

                        g.sendpacket(packet);
                    }
                   Thread.Sleep(3000);
                   Process.GetCurrentProcess().Kill();
                }
            }
            else
            {
                Process.GetCurrentProcess().Kill();
            }

            while (!client.LoggedIn)
            {
                Thread.Sleep(100);
            }
        }

        private void client_Exited(object sender, EventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }
    }
}
