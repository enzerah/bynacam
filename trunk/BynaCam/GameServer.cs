using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tibia;
using Tibia.Objects;
using Tibia.Util;
using System.Diagnostics;
using Tibia.Packets;
using System.Net.Sockets;
using System.Net;

namespace BynaCam
{
    class GameServer
    {
        Client client;
        Socket sock;
        public NetworkStream stream;

        public bool Accepted = false;

        public GameServer(Client c)
        {
            client = c;
        }

        public void SetServer(short port)
        {
            TcpListener server = new TcpListener(IPAddress.Any, port);
            server.Start();
            sock = server.AcceptSocket();
            stream = new NetworkStream(sock);

            if (sock.Connected)
            {
                Accepted = true;
            }
        }

        public void sendpacket(byte[] packet)
        {
            NetworkMessage msg = new NetworkMessage(client);
            //msg.InsetLogicalPacketHeader();
            //msg.PrepareToSend();
            //msg.XteaEncrypt();
            msg.AddBytes(new byte[] { 0, 0 });
            msg.AddBytes(packet);
            msg.UpdateLogicalPacketHeader();
            msg.PrepareToSend();
            stream.Write(msg.Packet, 0, msg.Packet.Length);
        }
    }
}
