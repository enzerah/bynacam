using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Tibia.Objects;
using Tibia;
using Tibia.Packets;
using Tibia.Util;
using System.Diagnostics;
using System.IO.Compression;
using BynaCam_Recorder.Classes;

namespace BynaCam_Recorder
{
    public partial class mainFrm : Form
    {
        Client c;
        FileHandler fileHandler;
        PacketHandler packetHandler;

        public mainFrm()
        {
            InitializeComponent();
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            c = ConfigClient.getIniClient();
            fileHandler = new FileHandler();
            fileHandler.Open(FileChooser.getBynaCamFile(c));
            
            if (c != null)
            {
                notifyIcon1.ShowBalloonTip(5000, "BynaCam", "BynaCam is waiting for login...", ToolTipIcon.Info);
                packetHandler = new PacketHandler(c);
                ProcessWritePackets();
                c.Exited += new EventHandler(c_Exited);
                c.Proxy.PlayerLogin += new EventHandler(Proxy_PlayerLogin);
            }
            else
            {
                MessageBox.Show("Choose your client first!!");
                notifyIcon1.Visible = false;
                Process.GetCurrentProcess().Kill();
            }
         }

        public void ProcessWritePackets()
        {
            Tibia.Util.Timer timer = new Tibia.Util.Timer(100, true);
            timer.Execute += new Tibia.Util.Timer.TimerExecution(new Action(delegate()
            {
                lock (timer)
                {
                    while (PacketQueue.PacketQ.Count > 0)
                    {
                        CapturedPacket packet = PacketQueue.PacketQ.Dequeue();
                        fileHandler.WriteHeader(PacketQueue.allTime.Elapsed);
                        fileHandler.WriteCurrentTime(PacketQueue.allTime.Elapsed);
                        fileHandler.WriteDelay(packet.Time);
                        fileHandler.WriteTruePacket(packet.Packet);
                        fileHandler.deflateStream.Flush();
                    }
                }
            }));
         
        }

        private void saveAndExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fileHandler.WriteHeader(PacketQueue.allTime.Elapsed);
            fileHandler.deflateStream.Close();
            fileHandler.fileStream.Close();
            notifyIcon1.Visible = false;
            c.Process.Kill();
            Process.GetCurrentProcess().Kill();
        }

        private void c_Exited(object sender, EventArgs e)
        {
            fileHandler.WriteHeader(PacketQueue.allTime.Elapsed);
            fileHandler.deflateStream.Close();
            fileHandler.fileStream.Close();
            notifyIcon1.Visible = false;
            Process.GetCurrentProcess().Kill();
        }

        private void Proxy_PlayerLogin(object sender, EventArgs e)
        {
            notifyIcon1.ShowBalloonTip(2000, "BynaCam", "BynaCam is recording!\r\nTo stop recording just exit your Tibia Client!", ToolTipIcon.Info);
            this.Hide();
        }
    }
}
