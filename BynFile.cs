using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Compression;
using System.IO;

namespace Player
{
    public class BynFile
    {
        GZipStream stream;

        public MemoryStream Buffer = new MemoryStream();
        public byte[] TibiaVersion;
        public uint MovieLength; //in ms

        public BynFile(string path)
        {
            stream = new GZipStream(new FileStream(path, FileMode.Open, FileAccess.Read), CompressionMode.Decompress);

            byte[] buf = new byte[10000];
            while (true)
            {
                int read = stream.Read(buf, 0, buf.Length);
                if (read <= 0)
                    break;
                else
                    Buffer.Write(buf, 0, read);
            }
            stream.Close();

            TibiaVersion = BitConverter.GetBytes(BitConverter.ToUInt32(Buffer.GetBuffer(), 1));
            MovieLength = BitConverter.ToUInt32(Buffer.GetBuffer(), (int)Buffer.Length - 4);
        }
    }
}
