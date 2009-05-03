using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using Tibia;
using Tibia.Objects;

namespace Connection
{
    public class LoginServer
    {
        Client client;
        event EventHandler connected;
        ushort sport;
        ushort wport;
        TcpListener server;
        Thread serverthread;

        public LoginServer(Client c)
        {
            client = c;
            connected += new EventHandler(LoginServer_connected);
        }

        private void LoginServer_connected(object sock, EventArgs e)
        {
            Socket socket = (Socket)sock;
            var stream = new NetworkStream(socket);
            byte[] buffer = new byte[8192];
            int readbytes = 0;

            readbytes = stream.Read(buffer, 0, buffer.Length);
            Tibia.Packets.NetworkMessage msg = new Tibia.Packets.NetworkMessage(client, buffer, readbytes);
            msg.Position = 6;

            if (msg.GetByte() == 0x01)
            {
                ushort osVersion = msg.GetUInt16();
                ushort clientVersion = msg.GetUInt16();

                msg.GetUInt32();
                msg.GetUInt32();
                msg.GetUInt32();

                int pos = msg.Position;

                msg.RsaOTDecrypt();

                if (msg.GetByte() != 0)
                    return;

                uint[] key = new uint[4];
                key[0] = msg.GetUInt32();
                key[1] = msg.GetUInt32();
                key[2] = msg.GetUInt32();
                key[3] = msg.GetUInt32();

                msg = new Tibia.Packets.NetworkMessage(client);

                msg.AddByte(0x64);
                msg.AddByte(1);
                msg.AddString("Byna");
                msg.AddString("BynaCam");
                msg.AddBytes(new byte[] { 127, 0, 0, 1 });
                msg.AddUInt16(wport);
                msg.AddUInt16(90);

                msg.InsetLogicalPacketHeader();
                msg.XteaEncrypt(key);
                msg.AddAdler32();
                msg.InsertPacketHeader();

                stream.Write(msg.Data, 0, msg.Length);
            }
            server.Stop();
        }

        public void StartServer(ushort serverport, ushort worldport)
        {
            sport = serverport;
            wport = worldport;
            server = new TcpListener(IPAddress.Any, serverport);
            if (client != null && !client.LoggedIn)
            {
                client.Login.SetOT("127.0.0.1", (short)serverport);
                serverthread = new Thread(new ThreadStart(delegate()
                    {
                        while (true)
                        {
                            server.Start();
                            var socket = server.AcceptSocket();
                            connected.Invoke(socket, new EventArgs());
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
    }
}
