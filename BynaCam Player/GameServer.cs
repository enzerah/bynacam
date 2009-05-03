using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tibia;
using Tibia.Packets;
using Tibia.Objects;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Connection
{
    public class GameServer
    {
        Client client;
        NetworkStream stream;
        public event EventHandler clientConnected;
        Thread serverthread;
        TcpListener server;

        public GameServer(Client c)
        {
            client = c;
        }

        public void SetServer(ushort port)
        {
            server = new TcpListener(IPAddress.Any, port);
            if (client != null && !client.LoggedIn)
            {
                serverthread = new Thread(new ThreadStart(delegate()
                {
                    while (true)
                    {
                        server.Start();
                        var socket = server.AcceptSocket();
                        stream = new NetworkStream(socket);

                        if (clientConnected != null)
                           clientConnected.Invoke(stream, new EventArgs());
                    }
                }));
                serverthread.Start();
            }
        }

        public void StopServer()
        {
            try
            {
                serverthread.Abort();
                server.Stop();
            }
            catch { }
        }

        public void Send(byte[] packet, bool raw)
        {
            if (!raw)
            {
                NetworkMessage msg = new NetworkMessage();
                msg.AddBytes(packet);
                msg.UpdateLogicalPacketHeader();
                msg.PrepareToSend(client.IO.XteaKey);

                try
                {
                    stream.Write(msg.Data, 0, msg.Length);
                }
                catch { }
            }
            else
            {
                try
                {
                    stream.Write(packet, 0, packet.Length);
                }
                catch { }
            }
        }

        public void Recv(byte[] buffer, int len)
        {
            try
            {
                stream.Read(buffer, 0, len);
            }
            catch { }
        }
    }
}
