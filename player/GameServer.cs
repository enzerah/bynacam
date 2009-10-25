using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Player.Server
{
    public class GameServer
    {
        Client client;
        NetworkStream stream;
        public event EventHandler ClientConnected;
        Thread serverthread;
        TcpListener server;

        public GameServer(Client c)
        {
            client = c;
        }

        public void Start(ushort port)
        {
            server = new TcpListener(IPAddress.Any, port);
            server.Start();
            serverthread = new Thread(new ThreadStart(delegate()
            {
                while (true)
                {

                    var socket = server.AcceptSocket();
                    stream = new NetworkStream(socket);

                    if (ClientConnected != null)
                    {
                        byte[] firstPacket = { 0x0C, 0x00, 0x32, 0x02, 0x86, 0x07, 0x06, 0x00, 0x1F, 0x95, 0xBC, 0x00, 0x00, 0xBB };
                        stream.Write(firstPacket, 0, firstPacket.Length);
                        stream.Flush();
                        stream.Read(new byte[0xFFFF], 0, 0xFFFF);
                        ClientConnected.Invoke(this, new EventArgs());
                        Stop();
                    }
                }
            }));
            serverthread.Start();
        }

        public void Stop()
        {
            try
            {
                serverthread.Abort();
                server.Stop();
            }
            catch { }
        }

        public void Send(byte[] packet)
        {
            Packets.NetworkMessage msg = new Packets.NetworkMessage();
            msg.AddBytes(packet);
            msg.UpdateLogicalPacketHeader();
            msg.PrepareToSend(client.XteaKey);
            stream.Write(msg.Data, 0, msg.Length);
        }

        public void Recv(byte[] buffer, int len)
        {
            stream.Read(buffer, 0, len);
        }
    }
}
