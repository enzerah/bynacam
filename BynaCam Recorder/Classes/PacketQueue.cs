using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Tibia;
using Tibia.Packets;
using System.Windows.Forms;

namespace BynaCam_Recorder.Classes
{
    public static class PacketQueue
    {
        static Stopwatch delayWatch = new Stopwatch();
        public static bool firstPacket = false;
        static TimeSpan delayTime = TimeSpan.Zero;
        public static Stopwatch allTime = new Stopwatch();

        public static Queue<CapturedPacket> PacketQ = new Queue<CapturedPacket>();

        public static void LogPacket(byte[] packetdata)
        {
            if (!firstPacket && packetdata[0] == (byte)IncomingPacketType.SelfAppear)
            {
                firstPacket = true;
                delayWatch.Start();
                allTime.Start();
                ProcessWritePackets();
            }

            PacketQ.Enqueue(new CapturedPacket(delayWatch.Elapsed - delayTime, packetdata));
            ProcessWritePackets();
            delayTime = delayWatch.Elapsed;
        }

        public static void ProcessWritePackets()
        {
            lock ("ProcessWritePackets")
            {
                mainFrm.fileHandler.WriteHeader(allTime.Elapsed);

                while (PacketQueue.PacketQ.Count > 0)
                {
                    CapturedPacket packet = PacketQueue.PacketQ.Dequeue();

                    if (packet.Packet == null)
                        continue;

                    mainFrm.fileHandler.WritePacket(packet.Packet, allTime.Elapsed, packet.Time);
                }
            }
        }

        public struct CapturedPacket
        {
            public TimeSpan Time;
            public byte[] Packet;

            public CapturedPacket(TimeSpan time, byte[] data)
            {
                Time = time;
                Packet = data;
            }
        }
    }
}
