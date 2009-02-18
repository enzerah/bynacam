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
            c.Proxy.ReceivedPlayerSpeechOutgoingPacket += new SocketBase.OutgoingPacketListener(Proxy_ReceivedPlayerSpeechOutgoingPacket);
            c.Proxy.ReceivedMessageFromClient += new Proxy.MessageListener(Proxy_ReceivedMessageFromClient);
        }

        private void Proxy_ReceivedMessageFromClient(NetworkMessage server)
        {
            NetworkMessage msg = new NetworkMessage(client, server.Packet);
            msg.PrepareToRead();
            msg.GetUInt16(); //logical packet size
            parsePacket(msg.GetBytes(msg.Length - msg.Position));
        }

        private void parsePacket(byte[] data)
        {
            byte type = data[0];

            if (type == (byte)IncomingPacketType.OutfitWindow
            || type == (byte)IncomingPacketType.ChannelList
            || type == (byte)IncomingPacketType.ItemTextWindow
            || type == (byte)IncomingPacketType.ErrorMessage
            || type == (byte)IncomingPacketType.HouseTextWindow
            || type == (byte)IncomingPacketType.ItemTextWindow
            || type == (byte)IncomingPacketType.RuleViolationOpen
            || type == (byte)IncomingPacketType.ShowTutorial
            || type == (byte)IncomingPacketType.WaitingList)
                return;

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
                if (type == (byte)IncomingPacketType.CreatureSpeech)
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
            }

            PacketQueue.LogPacket(data);
        }

        private bool Proxy_ReceivedPlayerSpeechOutgoingPacket(OutgoingPacket packet)
        {
            Tibia.Packets.Outgoing.PlayerSpeechPacket p = (Tibia.Packets.Outgoing.PlayerSpeechPacket)packet;

            if (p.SpeechType == SpeechType.Private && !mainFrm.hideOutPm)
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
    }
}
