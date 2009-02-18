using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using Tibia;

namespace BynaCam.Classes
{
    public class FileHandler
    {
        public FileStream fileStream;
        public DeflateStream deflateStream;
        string fileName = string.Empty;

        public byte[] packet;
        public TimeSpan packetTime;
        public TimeSpan packetDelay;

        public TimeSpan playTime;
        public string tibiaVersion;

        public FileHandler() { }

        public void Open(string cFileName)
        {
            fileStream = new FileStream(cFileName, FileMode.Open);
            deflateStream = new DeflateStream(fileStream, CompressionMode.Decompress);
        }

        public bool ReadPacket()
        {
                packetTime = ReadTime(deflateStream);
                packetDelay = ReadTime(deflateStream);
                packet = ReadPacketBytes(deflateStream);

                if (packetTime == TimeSpan.Zero)
                    return false;
                else
                    return true;
        }

        public void ReadHeader()
        {
            if (ReadByte(fileStream) != 0x01) //always must be 0x01
                throw new Exception("header corrupted");
            //tibiaver
            tibiaVersion = ReadString(fileStream);
            //playtime
            playTime = ReadTime(fileStream);
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

        public byte[] ReadPacketBytes(Stream stream)
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
