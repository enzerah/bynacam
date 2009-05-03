using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tibia;
using System.IO;
using System.IO.Compression;

namespace BynaCam_Player
{
    public struct HeaderStruct
    {
        public string TibiaVersion;
        public TimeSpan MovieTime;
    }

    public struct PacketStruct
    {
        public byte[] packet;
        public TimeSpan packetTime;
        public TimeSpan packetDelay;
    }

    public class FileHandler
    {
        public FileStream fileStream;
        public DeflateStream deflateStream;
        string fileName = string.Empty;

        public FileHandler() { }

        public void Open(string cFileName)
        {
            fileStream = new FileStream(cFileName, FileMode.Open, FileAccess.Read);
            deflateStream = new DeflateStream(fileStream, CompressionMode.Decompress);
        }

        public PacketStruct ReadPacket()
        {
            PacketStruct packet = new PacketStruct();
            packet.packetTime = ReadTime(deflateStream);
            packet.packetDelay = ReadTime(deflateStream);
            packet.packet = ReadPacketBytes(deflateStream);
            return packet;
        }

        public HeaderStruct ReadHeader()
        {
            HeaderStruct header = new HeaderStruct();
            if (ReadByte(fileStream) != 0x01) //always must be 0x01
                throw new Exception("header corrupted");
            //tibiaver
            header.TibiaVersion = ReadString(fileStream);
            //playtime
            header.MovieTime = ReadTime(fileStream);
            return header;
        }

        private string ReadString(Stream stream)
        {
            ushort len = 0;
            byte[] buffer;
            len = BitConverter.ToUInt16(ReadBytes(stream, 2), 0); //read len
            buffer = new byte[len];
            buffer = ReadBytes(stream, len);
            return buffer.ToPrintableString(0, len);
        }

        private TimeSpan ReadTime(Stream stream)
        {
            return TimeSpan.FromMilliseconds(ReadUInt32(stream));
        }

        private byte[] ReadPacketBytes(Stream stream)
        {
            ushort len = 0;
            byte[] buffer;
            len = BitConverter.ToUInt16(ReadBytes(stream, 2), 0); //read len
            buffer = new byte[len];
            buffer = ReadBytes(stream, len);
            return buffer;
        }

        private UInt32 ReadUInt32(Stream stream)
        {
            return BitConverter.ToUInt32(ReadBytes(stream, 4), 0);
        }

        private byte[] ReadBytes(Stream stream, int len)
        {
            byte[] buffer = new byte[len];
            stream.Read(buffer, 0, len);
            return buffer;
        }

        private byte ReadByte(Stream stream)
        {
            return (byte)stream.ReadByte();
        }
    }
}
