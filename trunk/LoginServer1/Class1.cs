using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace LoginServer
{
    public class Login
    {
        public void StartServer(Tibia.Objects.Client client, short port, string charname, string worldname, byte[] worldip, ushort worldport)
        {
            TcpListener server = new TcpListener(IPAddress.Any, port);
            byte[] buffer = new byte[8192];
            int readbytes = 0;

            if (client != null && !client.LoggedIn)
            {
                client.SetOT("127.0.0.1", port);
                server.Start();
                var socket = server.AcceptSocket();
                var stream = new NetworkStream(socket);


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
                    msg.AddString(charname);
                    msg.AddString(worldname);
                    msg.AddBytes(worldip);
                    msg.AddUInt16(worldport);
                    msg.AddUInt16(90);

                    msg.InsetLogicalPacketHeader();
                    msg.XteaEncrypt(key);
                    msg.InsertAdler32();
                    msg.InsertPacketHeader();

                    stream.Write(msg.Packet, 0, msg.Length);

                    server.Stop();
                }
            }
        }
    }
}
