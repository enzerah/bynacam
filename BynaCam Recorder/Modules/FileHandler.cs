using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Diagnostics;

namespace BynaCam_Recorder
{
    public class FileHandler
    {
        FileStream fileStream;
        DeflateStream deflateStream;
        public string fileName = string.Empty;

        public FileHandler() { }

        public void Open(string cFileName)
        {
            fileStream = new FileStream(cFileName, FileMode.CreateNew, FileAccess.Write);
                deflateStream = new DeflateStream(fileStream, CompressionMode.Compress, false);
                fileName = cFileName;
        }

        public void Close()
        {
            try
            {
                deflateStream.Close();
            }
            catch { }

            if (new FileInfo(fileName).Length < 1)
                File.Delete(fileName);
        }

        public void WritePacket(byte[] packet, TimeSpan packetTime, TimeSpan packetDelay)
        {
            WriteTime(deflateStream, packetTime);
            WriteTime(deflateStream, packetDelay);
            WriteBytes(deflateStream, packet);
        }

        public void WriteHeader(TimeSpan playTime)
        {
            fileStream.Seek(0, SeekOrigin.Begin);
            WriteByte(fileStream, 0x01); //always 0x01
            //tibiaver
            WriteString(fileStream, Constants.Version);
            //playtime
            WriteTime(fileStream, playTime);
            fileStream.Seek(0, SeekOrigin.End);
        }

        private void WriteString(Stream stream, string value)
        {
            WriteBytes(stream, BitConverter.GetBytes((ushort)value.Length));
            WriteBytes(stream, Encoding.ASCII.GetBytes(value));
        }

        private void WriteTime(Stream stream, TimeSpan value)
        {
            WriteUInt32(stream, (uint)value.TotalMilliseconds);
        }

        private void WriteUInt32(Stream stream, UInt32 value)
        {
            WriteBytes(stream, BitConverter.GetBytes(value));
        }

        private void WriteBytes(Stream stream, byte[] data)
        {
            stream.Write(data, 0, data.Length);
        }

        private void WriteByte(Stream stream, byte value)
        {
            stream.WriteByte(value);
        }
    }

}
