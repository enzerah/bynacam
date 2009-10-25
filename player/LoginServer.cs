using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace Player.Server
{
    public class LoginServer
    {
        Client client;
        event EventHandler connected;
        TcpListener server;
        Thread serverthread;

        ushort lport;
        ushort sport;

        public LoginServer(Client c, ushort loginport)
        {
            client = c;
            lport = loginport;
            connected += new EventHandler(LoginServer_connected);
        }

        private void LoginServer_connected(object sock, EventArgs e)
        {
            Socket socket = (Socket)sock;
            var stream = new NetworkStream(socket);
            byte[] buffer = new byte[8192];
            int readbytes = 0;

            readbytes = stream.Read(buffer, 0, buffer.Length);
            Packets.NetworkMessage msg = new Packets.NetworkMessage(client, buffer, readbytes);
            msg.Position = 6;

            if (msg.GetByte() == 0x01)
            {
                msg = new Packets.NetworkMessage(client);

                msg.AddByte(0x64);
                msg.AddByte(1);
                msg.AddString("Byna");
                msg.AddString("BynaCam");
                msg.AddBytes(new byte[] { 127, 0, 0, 1 });
                msg.AddUInt16(sport);
                msg.AddUInt16(90);

                msg.InsetLogicalPacketHeader();
                msg.XteaEncrypt();
                msg.AddAdler32();
                msg.InsertPacketHeader();

                stream.Write(msg.Data, 0, msg.Length);
            }
            
            Stop();
        }

        public void Start(ushort worldport)
        {
            sport = worldport;
            server = new TcpListener(IPAddress.Any, lport);
            if (client != null)
            {
                client.SetServer("127.0.0.1", (short)lport);
                server.Start();

                serverthread = new Thread(new ThreadStart(delegate()
                {
                    while (true)
                    {
                        var socket = server.AcceptSocket();
                        connected.Invoke(socket, new EventArgs());
                    }
                }));
                serverthread.Start();
            }
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
    }
}
