using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Tibia;

namespace BynaCam_Recorder.Classes
{
    public static class PacketQueue
    {
        static Stopwatch delayWatch = new Stopwatch();
        public static bool firstPacket = false;
        static TimeSpan delayTime = TimeSpan.Zero;
        public static Stopwatch allTime = new Stopwatch();

        public static Queue<CapturedPacket> PacketQ = new Queue<CapturedPacket>();

        public static void LogPacket(byte[] packet)
        {
            if (!firstPacket)
            {
                firstPacket = true;
                delayWatch.Start();
                allTime.Start();
            }

            PacketQ.Enqueue(new CapturedPacket(delayWatch.Elapsed - delayTime, packet));
            delayTime = delayWatch.Elapsed;
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
