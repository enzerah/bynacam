using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tibia.Objects;
using Tibia;
using Tibia.Packets;
using Tibia.Util;


namespace BynaCam_Recorder.Classes
{
    public class PacketHandler
    {
        Client client;

        public PacketHandler(Client c)
        {
            client = c;
            if (!client.StartProxy())
                throw new Exception("Could not start Proxy!");
            c.Proxy.IncomingSplitPacket += new SocketBase.SplitPacket(Proxy_IncomingSplitPacket);
            c.Proxy.ReceivedPlayerSpeechOutgoingPacket += new SocketBase.OutgoingPacketListener(Proxy_ReceivedPlayerSpeechOutgoingPacket);
        }

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
                    PacketQueue.LogPacket(pack.ToByteArray());
            }

            return true;
        }

        private void Proxy_IncomingSplitPacket(byte type, byte[] data)
        {
            PacketQueue.LogPacket(data);
        }
    }
}
