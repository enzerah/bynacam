using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Tibia.Packets;
using Tibia.Objects;
using System.IO;
using Tibia;
using Tibia.Packets.Incoming;

namespace BynaCam_Recorder
{
    public class PacketHandler : FileHandler
    {
        Stopwatch delayWatch = new Stopwatch();
        bool firstPacket = false;
        TimeSpan delayTime = TimeSpan.Zero;
        TimeSpan allTime = TimeSpan.Zero;
        Client client;
        Queue<CapturedPacket> queue = new Queue<CapturedPacket>();

        public PacketHandler(string file, Client c)
        {
            client = c;

            for (uint i = 1; i < uint.MaxValue; i++)
            {
                if (!File.Exists(file + "\\movie" + i + ".byn"))
                {
                    Open(file + "\\movie" + i + ".byn");
                    break;
                }
            }
        }

        public void LogPacket(byte[] packetdata)
        {
            if (!firstPacket)
            {
                firstPacket = true;
                delayWatch.Start();
            }

            TimeSpan delay = delayWatch.Elapsed - delayTime;
            queue.Enqueue(new CapturedPacket(delay, packetdata));
            allTime = allTime + delay;
            delayTime = delayWatch.Elapsed;
            ProcessWritePackets();
        }

        public void LogMsg(byte[] msgpacket)
        {
            NetworkMessage msg = new NetworkMessage(client);
            msg.AddBytes(msgpacket);
            msg.UpdateLogicalPacketHeader();
            msg.Position = 6;
            LogPacket(msg.GetBytes(msg.Length - msg.Position));
        }

        public void ProcessWritePackets()
        {
            while (queue.Count > 0)
            {
                CapturedPacket packet = queue.Dequeue();
                if (packet.Packet == null)
                    return;

                WriteHeader(allTime);
                WritePacket(packet.Packet, allTime, packet.Delay);
            }
        }

        public struct CapturedPacket
        {
            public TimeSpan Delay;
            public byte[] Packet;

            public CapturedPacket(TimeSpan delay, byte[] data)
            {
                Delay = delay;
                Packet = data;
            }
        }

    }
}
