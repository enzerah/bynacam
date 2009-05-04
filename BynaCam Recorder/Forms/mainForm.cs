using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using Tibia;
using Tibia.Objects;
using Tibia.Packets;

namespace BynaCam_Recorder
{
    public partial class Form1 : Form
    {
        #region Variables
        Process currentProcess = Process.GetCurrentProcess();
        PacketHandler packetHandler;
        Client client;

        string forceLoginServer = string.Empty;
        ushort forceLoginPort = 0;
        bool loginforced = false;
        #endregion

        #region Constructor
        public Form1(string[] argv)
        {
            InitializeComponent();
            if (argv.Length == 2)
            {
                forceLoginServer = argv[0];
                forceLoginPort = Convert.ToUInt16(argv[1]);
            }
        }
        #endregion


        #region Main Form closed / Client exited
        private void Form1_FormClosed(object sender, EventArgs e)
        {
            Exit();
        }
        private void client_Exited(object sender, EventArgs e)
        {
            Exit();
        }
        private void Exit()
        {
            Invoke(new MethodInvoker(delegate()
            {
                notifyIcon.Visible = false;
                notifyIcon.Dispose();
            }));
            try { packetHandler.Close(); }
            catch { }
            try { client.Close(); }
            catch { }
            try { currentProcess.Kill(); }
            catch { }
        }
        #endregion

        #region ErrMsg
        private void ErrMsg(string msg)
        {
            Invoke(new MethodInvoker(delegate()
                {
                    MessageBox.Show(msg, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    startBtn.Text = "Exit";
                }));
        }
        #endregion

        private bool Proxy_ReceivedPlayerSpeechOutgoingPacket(OutgoingPacket packet)
        {
            Tibia.Packets.Outgoing.PlayerSpeechPacket p = (Tibia.Packets.Outgoing.PlayerSpeechPacket)packet;

            if (p.SpeechType == SpeechType.Private)
            {
                Tibia.Packets.Incoming.CreatureSpeechPacket pack = new Tibia.Packets.Incoming.CreatureSpeechPacket(client);
                pack.SenderName = p.Receiver;
                pack.SenderLevel = 0;
                pack.Message = ">> " + p.Message;
                pack.SpeechType = p.SpeechType;
                pack.ChannelId = p.ChannelId;
                pack.Position = Tibia.Objects.Location.Invalid;
                pack.Time = 0;
                NetworkMessage msg = NetworkMessage.CreateUnencrypted(client);
                pack.ToNetworkMessage(ref msg);
                packetHandler.LogMsg(msg.Data);
                return true;
            }
            if (p.SpeechType == SpeechType.PrivatePlayerToNPC)
            {
                Tibia.Packets.Incoming.CreatureSpeechPacket pack = new Tibia.Packets.Incoming.CreatureSpeechPacket(client);
                pack.Message = ">> " + p.Message;
                pack.SenderName = "";
                pack.SpeechType = SpeechType.PrivateNPCToPlayer;
                pack.ChannelId = p.ChannelId;
                pack.Position = Tibia.Objects.Location.Invalid;
                NetworkMessage msg = NetworkMessage.CreateUnencrypted(client);
                pack.ToNetworkMessage(ref msg);
                packetHandler.LogMsg(msg.Data);
                return true;
            }
            return true;
        }

        private void Proxy_ReceivedDataFromServer(byte[] data)
        {
            NetworkMessage msg = new NetworkMessage(client, data);
            msg.PrepareToRead();
            msg.Position = 6;

            if (msg.Data[8] == (byte)IncomingPacketType.ErrorMessage
                || msg.Data[8] == (byte)IncomingPacketType.WaitingList)
                return;

            packetHandler.LogPacket(msg.GetBytes(msg.Length - msg.Position));
        }


        private void startBtn_Click(object sender, EventArgs e)
        {
            if (startBtn.Text == "Exit")
            {
                Exit();
            }
            if (startBtn.Text == "Start")
            {
                //EXECUTING CLIENT!
                client = new TibiaClient().getClient();
                if (client == null)
                {
                    ErrMsg("ERROR: Cannot open Tibia client! Wrong version or path doesn't exists!");
                    return;
                }

                client.Exited += new EventHandler(client_Exited);
                client.Process.PriorityClass = ProcessPriorityClass.Idle;

                //FORCING LOGIN SERVER | OT?
                if (forceLoginServer != string.Empty
                    && forceLoginPort != 0)
                {
                    loginforced = true;
                    client.Login.SetOT(forceLoginServer, (short)forceLoginPort);
                }

                //START PROXY!
                if (!client.IO.UsingProxy)
                {
                    if (!client.IO.StartProxy())
                    {
                        ErrMsg("ERROR: Proxy error!");
                        return;
                    }
                }
                if (!System.IO.Directory.Exists(Kernel.OptionsForm.tb_path.Text))
                    packetHandler = new PacketHandler(Application.StartupPath, client);
                else
                    packetHandler = new PacketHandler(Kernel.OptionsForm.tb_path.Text, client);

                client.IO.Proxy.ReceivedDataFromServer += new Tibia.Util.ProxyBase.DataListener(Proxy_ReceivedDataFromServer);
                client.IO.Proxy.ReceivedPlayerSpeechOutgoingPacket += new Tibia.Util.ProxyBase.OutgoingPacketListener(Proxy_ReceivedPlayerSpeechOutgoingPacket);
                if (loginforced)
                    notifyIcon.ShowBalloonTip(5000, "BynaCam Recorder", "BynaCam loaded successfully!\r\nRecording started!\r\nForce Login Server: " + forceLoginServer + ":" + forceLoginPort + "\r\n\r\nMovie:\r\n" + packetHandler.fileName, ToolTipIcon.Info);
                else
                    notifyIcon.ShowBalloonTip(5000, "BynaCam Recorder", "BynaCam loaded successfully!\r\nRecording started!\r\n\r\nMovie:\r\n" + packetHandler.fileName, ToolTipIcon.Info);

                this.WindowState = FormWindowState.Minimized;
                this.ShowInTaskbar = false;
                this.Visible = false;
            }
        }

        #region About button / Options Button
        private void aboutBtn_Click(object sender, EventArgs e)
        {
            Kernel.AboutForm.ShowInTaskbar = false;
            Kernel.AboutForm.ShowIcon = false;
            Kernel.AboutForm.ShowDialog(new WindowWrapper(this.Handle));
        }

        private void optionsBtn_Click(object sender, EventArgs e)
        {
            Kernel.OptionsForm.ShowInTaskbar = false;
            Kernel.OptionsForm.ShowIcon = false;
            Kernel.OptionsForm.ShowDialog(new WindowWrapper(this.Handle));
        }
        #endregion

        #region Form1_Shown (auto update)

        private void Form1_Shown(object sender, EventArgs e)
        {
            updateForm uForm = new updateForm();
            uForm.FileDownloaded += new Action(delegate()
            {
                MessageBox.Show("BynaCam will run installation file...");
                Process.Start(Application.StartupPath + "\\update.exe");
                Exit();
            });
            uForm.FileDownloadError += new Action(delegate()
                {
                    Thread.Sleep(1000);
                    uForm.Invoke(new MethodInvoker(delegate()
                        {
                            uForm.Close();
                        }));
                });

            uForm.Show(this);
            uForm.UpdateSoftware();
        }
        #endregion

    }
}
