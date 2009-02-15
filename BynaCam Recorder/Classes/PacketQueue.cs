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
            delayTime = delayWatch.Elapsed;
        }

        public static void ProcessWritePackets()
        {
            new System.Threading.Thread(new System.Threading.ThreadStart(delegate()
                {
                    while (true)
                    {
                        while (PacketQueue.PacketQ.Count > 0)
                        {
                            CapturedPacket packet = PacketQueue.PacketQ.Dequeue();

                            if (packet.Packet[0] == 0xc8 //setoufit block
                            || packet.Packet[0] == (byte)IncomingPacketType.ChannelList ////channellist block
                            || packet.Packet[0] == 0x96 //textwindow block
                            || packet.Packet[0] == 0x14 //disconnectclient
                            || packet.Packet[0] == (byte)IncomingPacketType.HouseTextWindow
                            || packet.Packet[0] == (byte)IncomingPacketType.ItemTextWindow
                            || packet.Packet[0] == (byte)IncomingPacketType.RuleViolationOpen
                            || packet.Packet[0] == (byte)IncomingPacketType.ShowTutorial
                            || packet.Packet[0] == (byte)IncomingPacketType.WaitingList)
                                break;

                            mainFrm.fileHandler.WriteHeader(PacketQueue.allTime.Elapsed);
                            mainFrm.fileHandler.WriteCurrentTime(PacketQueue.allTime.Elapsed);

                            if (packet.Packet[0] == 0x65
                                || packet.Packet[0] == 0x66
                                || packet.Packet[0] == 0x67
                                || packet.Packet[0] == 0x68
                                || packet.Packet[0] == (byte)IncomingPacketType.MapDescription
                                || packet.Packet[0] == (byte)IncomingPacketType.SelfAppear
                                || packet.Packet[0] == (byte)IncomingPacketType.WorldLight)
                                mainFrm.fileHandler.WriteDelay(TimeSpan.Zero);
                            else
                                mainFrm.fileHandler.WriteDelay(packet.Time);

                            mainFrm.fileHandler.WriteTruePacket(packet.Packet);
                        }
                        System.Threading.Thread.Sleep(100);
                    }
                })).Start();
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
