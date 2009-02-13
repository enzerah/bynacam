using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tibia;
using Tibia.Packets;
using Tibia.Objects;
using System.Net;
using System.Net.Sockets;

namespace GameServer
{
    public class GameServer
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

        public void Send(byte[] packet)
        {
            NetworkMessage msg = new NetworkMessage(client);
            //msg.InsetLogicalPacketHeader();
            //msg.PrepareToSend();
            //msg.XteaEncrypt();
            msg.AddBytes(new byte[] { 0, 0 });
            msg.AddBytes(packet);
            msg.UpdateLogicalPacketHeader();
            msg.PrepareToSend();
            stream.BeginWrite(msg.Packet, 0, msg.Packet.Length, null, null);
        }
    }
}
