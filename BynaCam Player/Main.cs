using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Tibia.Objects;
using Tibia;
using Tibia.Packets;

namespace BynaCam_Player
{
    public partial class Main
    {
        Connection.LoginServer login;
        Connection.GameServer game;
        KeyboardHook keyhook;
        HeaderStruct header;
        FileHandler handler = new FileHandler();
        TibiaClient tibiaclient = new TibiaClient();
        Client client;
        TibiaWindowTitle windowTitle;
        bool forwarding = false;


        public Main(string[] args)
        {
            if (args.Length > 0)
            {
                try
                {
                    handler.Open(args[0]);
                    header = handler.ReadHeader();
                }
                catch(Exception e)
                {
                    MessageBox.Show(e.Message);
                    Exit(null, null);
                }
            }
            else
                Exit(null, null);

            client = tibiaclient.getClient();
            if (header.TibiaVersion != client.Version)
            {
                MessageBox.Show("Wrong movie version!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                Exit(null, null);
            }
            keyhook = new KeyboardHook(client);
            windowTitle = new TibiaWindowTitle(client);
            login = new Connection.LoginServer(client);
            game = new Connection.GameServer(client);
            ushort loginport = Tibia.Util.ProxyBase.GetFreePort(8080);
            ushort gameport = Tibia.Util.ProxyBase.GetFreePort((ushort)(loginport + 1));
            login.StartServer(loginport, gameport);
            game.clientConnected += new EventHandler(game_clientConnected);
            game.SetServer(gameport);

            client.Process.PriorityClass = System.Diagnostics.ProcessPriorityClass.Idle;
            client.Exited += new EventHandler(Exit);
            client.Login.Login("", "", "Byna");
            keyhook.FastBackPressed += new KeyboardHook.ForwardHandler(keyhook_FastBackPressed);
            keyhook.FastForwardPressed += new KeyboardHook.ForwardHandler(keyhook_FastForwardPressed);
        }

        private void Exit(object sender, EventArgs e)
        {
            try { client.Close(); }
            catch { }
            try
            {
                login.StopServer();
                game.StopServer();
            }
            catch { }

            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }

        private void keyhook_FastBackPressed()
        {
            //gotoPacket(TimeSpan.FromMilliseconds(Kernel.packetTime.TotalMilliseconds - 30));
        }

        private void keyhook_FastForwardPressed()
        {
            //gotoPacket(TimeSpan.FromMilliseconds(Kernel.packetTime.TotalMilliseconds + 30));
        }

        private void game_clientConnected(object socketstream, EventArgs e)
        {
            NetworkStream stream = (NetworkStream)socketstream;
            game.Send("0C 00 32 02 86 07 06 00 1F 95 BC 00 00 BB".ToBytesAsHex(), true);
            game.Recv(new byte[8096], 8096);

            Kernel.tibiaClientVer = header.TibiaVersion;
            Kernel.movieTime = header.MovieTime;

            while (true)
            {
                PacketStruct packet = handler.ReadPacket();
                if (packet.packet == null || packet.packet.Length == 0)
                    break;

                Kernel.packetTime = packet.packetTime;

                byte type = packet.packet[0];

                if (type == (byte)IncomingPacketType.OutfitWindow
                || type == (byte)IncomingPacketType.ChannelList
                || type == (byte)IncomingPacketType.ItemTextWindow
                || type == (byte)IncomingPacketType.ErrorMessage
                || type == (byte)IncomingPacketType.HouseTextWindow
                || type == (byte)IncomingPacketType.ItemTextWindow
                || type == (byte)IncomingPacketType.RuleViolationOpen
                || type == (byte)IncomingPacketType.ShowTutorial
                || type == (byte)IncomingPacketType.WaitingList)
                    continue;

                while (forwarding)
                    continue;

                sendPacket(packet.packetDelay, packet.packet);
            }
            Thread.Sleep(3000);
            Exit(null, null);
        }

        private void sendPacket(TimeSpan delay, byte[] packet)
        {
            Thread.Sleep(TimeSpan.FromMilliseconds(delay.TotalMilliseconds / (1 * Kernel.Speed)));
            game.Send(packet, false);
        }

        private void gotoPacket(TimeSpan packettime)
        {
            forwarding = true;
            while (true)
            {
                PacketStruct packet = handler.ReadPacket();
                if (packet.packet == null || packet.packet.Length == 0)
                    break;
                if (packet.packetTime >= packettime)
                    break;

                Kernel.packetTime = packet.packetTime;

                sendPacket(packet.packetDelay, packet.packet);
            }
            forwarding = false;

        }
    }
}