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

namespace BynaCam_Recorder
{
    public partial class mainFrm : Form
    {
        StreamWriter file;
        Client c;
        Stopwatch w = new Stopwatch();
        Queue<CapturedPacket> PacketQueue = new Queue<CapturedPacket>();
        TimeSpan time = TimeSpan.Zero;

        public mainFrm()
        {
            InitializeComponent();
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            c = IniClient.getIniClient();
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "BynaCam files|*.byn";

            if (dialog.ShowDialog(new WindowWrapper(c.MainWindowHandle))  == DialogResult.Cancel)
            {
                this.Activate();
                    MessageBox.Show(null, "You must choose file to save!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    notifyIcon1.Visible = false;
                    Process.GetCurrentProcess().Kill();
                    try { c.Process.Kill(); }
                    catch { }
            }

            file = new StreamWriter(File.Create(dialog.FileName));
            
            if (c != null)
            {
                notifyIcon1.ShowBalloonTip(5000, "BynaCam", "BynaCam is waiting for login...", ToolTipIcon.Info);
                c.Exited += new EventHandler(c_Exited);
                c.StartProxy();
                c.Proxy.IncomingSplitPacket += new SocketBase.SplitPacket(Proxy_IncomingSplitPacket);
                c.Proxy.ReceivedPlayerSpeechOutgoingPacket += new SocketBase.OutgoingPacketListener(Proxy_ReceivedPlayerSpeechOutgoingPacket);
            }
            else
            {
                MessageBox.Show("Choose your client first!!");
                notifyIcon1.Visible = false;
                Process.GetCurrentProcess().Kill();
            }
         }

        private bool Proxy_ReceivedPlayerSpeechOutgoingPacket(OutgoingPacket packet)
        {
            Tibia.Packets.Outgoing.PlayerSpeechPacket p = (Tibia.Packets.Outgoing.PlayerSpeechPacket)packet;

            if (p.SpeechType == SpeechType.Private)
            {
                BeginInvoke(new Action(delegate()
                {
                    //(c, p.Receiver, 0, ">> " + p.Message, SpeechType.Private, p.ChannelId);

                    Tibia.Packets.Incoming.CreatureSpeechPacket pack = new Tibia.Packets.Incoming.CreatureSpeechPacket(c);
                    pack.SenderName = p.Receiver;
                    pack.SenderLevel = 0;
                    pack.Message = ">> " + p.Message;
                    pack.SpeechType = p.SpeechType;
                    pack.ChannelId = p.ChannelId;
                    pack.Position = Tibia.Objects.Location.Invalid;
                    pack.Time = 0;
                    
                    LogPacket(pack.ToByteArray());
                }));
            }

            return true;
        }

        private void Proxy_IncomingSplitPacket(byte type, byte[] data)
        {           
            if (!w.IsRunning)
            {
                BeginInvoke(new Action(delegate() { this.Hide(); }));
                notifyIcon1.ShowBalloonTip(5000, "BynaCam", "BynaCam is recording...", ToolTipIcon.Info);
                w.Start();
            }

            LogPacket(data);
        }

        private void LogPacket(byte[] data)
        {
            PacketQueue.Enqueue(new CapturedPacket(w.Elapsed - time, data));
            time = w.Elapsed;
            ProcessWritePackets();
        }

        private void ProcessWritePackets()
        {
            CapturedPacket packet = PacketQueue.Dequeue();
                   BeginInvoke(new Action(delegate()
                        {  
                            file.WriteLine(packet.Time);
                            file.WriteLine(packet.Packet.ToHexString());
                            file.Flush();
                        }));
        }

        private void saveAndExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            notifyIcon1.Visible = false;
            c.Process.Kill();
            Process.GetCurrentProcess().Kill();
        }

        private void c_Exited(object sender, EventArgs e)
        {
            notifyIcon1.Visible = false;
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

    public struct CapturedPacket
    {
        public TimeSpan Time;
        public byte[] Packet;

        public CapturedPacket(TimeSpan time, byte[] data)
        {
            Time = time;
            Packet = data;
        }
    }
}
