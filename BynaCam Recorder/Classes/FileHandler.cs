using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using Tibia.Objects;
using Tibia;

namespace BynaCam_Recorder.Classes
{
    /*
     * File struct:
     * [header]
     * 1 - Tibia Version
     * 2 - Total playing time
     * 
     * [rest file]
     *   DELAY AND/OR TRUE PACKET (compressed by deflate)
     */

    public enum PacketType
    {
        PACKET = 3,
        DELAY = 4,
        TIME = 5
    }

    public class FileHandler
    {
        public FileStream fileStream;
        public DeflateStream deflateStream;
        string fileName = string.Empty;

        public FileHandler() {  }

        public void Open(string cFileName)
        {
            fileStream = new FileStream(cFileName, FileMode.Create);
            deflateStream = new DeflateStream(fileStream, CompressionMode.Compress);
        }

        public void WriteTruePacket(byte[] packet)
        {
            List<byte> temp = new List<byte>();
            temp.Add((byte)PacketType.PACKET);
            temp.AddRange(BitConverter.GetBytes((ushort)packet.Length));
            temp.AddRange(packet);
            deflateStream.Write(temp.ToArray(), 0, temp.Count);     
        }

        public void WriteCurrentTime(TimeSpan playTime)
        {
            List<byte> temp = new List<byte>();
            temp.Add((byte)PacketType.TIME);
            byte[] playtime = System.Text.ASCIIEncoding.ASCII.GetBytes(playTime.ToString());
            temp.AddRange(BitConverter.GetBytes((ushort)playtime.Length));
            temp.AddRange(playtime);
            deflateStream.Write(temp.ToArray(), 0, temp.Count);
        }

        public void WriteDelay(TimeSpan delay)
        {
            //playtime
            List<byte> temp = new List<byte>();
            byte[] delaytime = System.Text.ASCIIEncoding.ASCII.GetBytes(delay.ToString());
            temp.Add((byte)PacketType.DELAY);
            temp.AddRange(BitConverter.GetBytes((ushort)delaytime.Length));
            temp.AddRange(delaytime);
            deflateStream.Write(temp.ToArray(), 0, temp.Count);
        }

        public void WriteHeader(TimeSpan playTime)
        {
            //tibiaver
            List<byte> temp = new List<byte>();
            temp.AddRange(BitConverter.GetBytes((ushort)Version.TibiaVersion.ToByteArray().Length));
            temp.AddRange(Version.TibiaVersion.ToByteArray());

            //playtime
            byte[] playtime = System.Text.ASCIIEncoding.ASCII.GetBytes(playTime.ToString());
            temp.AddRange(BitConverter.GetBytes((ushort)playtime.Length));
            temp.AddRange(playtime);

            fileStream.Seek(0, SeekOrigin.Begin);
            fileStream.Write(temp.ToArray(), 0, temp.Count);
            fileStream.Seek(0, SeekOrigin.End);
        }
    }
}
