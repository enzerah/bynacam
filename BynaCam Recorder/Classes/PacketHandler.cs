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

            if (p.SpeechType == SpeechType.Private && !mainFrm.hidePm)
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
            if (mainFrm.hideVips)
            {
                if (type == (byte)IncomingPacketType.VipState
                    || type == (byte)IncomingPacketType.VipLogout
                    || type == (byte)IncomingPacketType.VipLogin)
                    return;
            }
            if (mainFrm.hideSkills)
            {
                if (type == (byte)IncomingPacketType.PlayerSkillsUpdate)
                    return;
            }
            if (mainFrm.hideMsg)
            {
                if (type == (byte)IncomingPacketType.CreatureSpeech)
                {
                    NetworkMessage msg = new NetworkMessage(data);
                    msg.GetByte();
                    msg.GetUInt32();
                    msg.GetString();
                    msg.GetUInt16();
                    byte speechtype = msg.GetByte();
                    if (speechtype == (byte)Tibia.Packets.SpeechType.Say ||
                        speechtype == (byte)Tibia.Packets.SpeechType.Whisper ||
                        speechtype == (byte)Tibia.Packets.SpeechType.Yell)
                        return;
                }
            }
            if (mainFrm.hidePm)
            {
                NetworkMessage msg = new NetworkMessage(data);
                    msg.GetByte();
                    msg.GetUInt32();
                    msg.GetString();
                    msg.GetUInt16();
                    byte speechtype = msg.GetByte();
                if (speechtype == (byte)Tibia.Packets.SpeechType.Private)
                    return;
            }

            PacketQueue.LogPacket(data);
        }
    }
}
