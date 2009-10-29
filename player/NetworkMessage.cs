/*
Copyright (c) 2007 Ian Obermiller and Hugo Persson 

Permission is hereby granted, free of charge, to any person
obtaining a copy of this software and associated documentation
files (the "Software"), to deal in the Software without
restriction, including without limitation the rights to use,
copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the
Software is furnished to do so, subject to the following
conditions:

The above copyright notice and this permission notice shall be
included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
OTHER DEALINGS IN THE SOFTWARE.
*/


using System;
using System.Text;

namespace Player.Packets
{
    public class NetworkMessage
    {
        #region Instance Variables
        private byte[] buffer;
        private int position, length, bufferSize = 16394;
        private Client c;
        #endregion

        #region Contructors

        public NetworkMessage()
        {
            buffer = new byte[bufferSize];
            position = 8;
        }

        public NetworkMessage(byte[] data)
        {
            buffer = new byte[bufferSize];
            Array.Copy(data, buffer, data.Length);
            length = data.Length;
            position = 0;
        }

        public NetworkMessage(NetworkMessage msg)
            : this(msg.Data)
        {
            this.position = msg.position;
        }

        public NetworkMessage(int length)
        {
            bufferSize = length;
            buffer = new byte[bufferSize];
            position = 8;
        }

        public NetworkMessage(byte[] data, int length)
        {
            buffer = new byte[bufferSize];
            Array.Copy(data, buffer, length);
            this.length = length;
            position = 0;
        }

        public NetworkMessage(Client client)
        {
            buffer = new byte[bufferSize];
            c = client;
            position = 8;
        }

        public NetworkMessage(Client client, int size)
        {
            bufferSize = size;
            buffer = new byte[bufferSize];
            c = client;
            position = 8;
        }

        public NetworkMessage(Client client, byte[] data)
        {
            buffer = new byte[bufferSize];
            c = client;
            Array.Copy(data, buffer, data.Length);
            length = data.Length;
            position = 0;
        }

        public NetworkMessage(Client client, byte[] data, int length)
        {
            buffer = new byte[bufferSize];
            c = client;
            Array.Copy(data, buffer, length);
            this.length = length;
            position = 0;
        }

        public static NetworkMessage CreateUnencrypted(Client client)
        {
            NetworkMessage nm = new NetworkMessage(client);
            nm.Position = 0;
            return nm;
        }

        public static NetworkMessage CreateUnencrypted(Client client, int size)
        {
            NetworkMessage nm = new NetworkMessage(client, size);
            nm.Position = 0;
            return nm;
        }

        #endregion

        #region Properties

        public int Length
        {
            get { return length; }
            set { length = value; }
        }

        public int Position
        {
            get { return position; }
            set { position = value; }
        }

        public byte[] GetBuffer()
        {
            return buffer;
        }

        public byte[] Data
        {
            get
            {
                byte[] t = new byte[length];
                Array.Copy(buffer, t, length);
                return t;
            }
        }

        #endregion

        #region Packer Header

        public void InsertPacketHeader()
        {
            AddPacketHeader((ushort)(length - 2));
        }

        #endregion

        #region Get

        public byte GetByte()
        {
            if (position + 1 > length)
                throw new Exception("NetworkMessage try to get more bytes from a smaller buffer");

            return buffer[position++];
        }

        public byte[] GetBytes(int count)
        {
            if (position + count > length)
                throw new Exception("NetworkMessage try to get more bytes from a smaller buffer");

            byte[] t = new byte[count];
            Array.Copy(buffer, position, t, 0, count);
            position += count;
            return t;
        }

        public string GetString()
        {
            int len = (int)GetUInt16();
            string t = System.Text.ASCIIEncoding.Default.GetString(buffer, position, len);
            position += len;
            return t;
        }

        public ushort GetUInt16()
        {
            return BitConverter.ToUInt16(GetBytes(2), 0);
        }

        public uint GetUInt32()
        {
            return BitConverter.ToUInt32(GetBytes(4), 0);
        }

        private ushort GetPacketHeader()
        {
            return BitConverter.ToUInt16(buffer, 0);
        }

        #endregion

        #region Add

        public void AddByte(byte value)
        {
            if (1 + length > bufferSize)
                throw new Exception("NetworkMessage buffer is full.");

            AddBytes(new byte[] { value });
        }

        public void AddBytes(byte[] value)
        {
            if (value.Length + length > bufferSize)
                throw new Exception("NetworkMessage buffer is full.");

            Array.Copy(value, 0, buffer, position, value.Length);
            position += value.Length;

            if (position > length)
                length = position;
        }

        public void AddString(string value)
        {
            AddUInt16((ushort)value.Length);
            AddBytes(System.Text.ASCIIEncoding.Default.GetBytes(value));
        }

        public void AddUInt16(ushort value)
        {
            AddBytes(BitConverter.GetBytes(value));
        }

        public void AddUInt32(uint value)
        {
            AddBytes(BitConverter.GetBytes(value));
        }

        public void AddPaddingBytes(int count)
        {
            position += count;

            if (position > length)
                length = position;
        }

        private void AddPacketHeader(ushort value)
        {
            Array.Copy(BitConverter.GetBytes(value), 0, buffer, 0, 2);
        }

        #endregion

        #region Peek

        public byte PeekByte()
        {
            return buffer[position];
        }

        public byte[] PeekBytes(int count)
        {
            byte[] t = new byte[count];
            Array.Copy(buffer, position, t, 0, count);
            return t;
        }

        public ushort PeekUInt16()
        {
            return BitConverter.ToUInt16(PeekBytes(2), 0);
        }

        public uint PeekUInt32()
        {
            return BitConverter.ToUInt32(PeekBytes(4), 0);
        }

        public string PeekString()
        {
            int len = (int)PeekUInt16();
            return System.Text.ASCIIEncoding.ASCII.GetString(PeekBytes(len + 2), 2, len);
        }

        #endregion

        #region Replace

        public void ReplaceBytes(int index, byte[] value)
        {
            if (length - index >= value.Length)
                Array.Copy(value, 0, buffer, index, value.Length);
        }

        #endregion

        #region Other Functions

        public void Reset()
        {
            position = 8;
            length = 8;
        }

        public bool PrepareToSend()
        {
            return PrepareToSend(c.XteaKey);
        }

        public bool PrepareToSend(uint[] XteaKey)
        {
            if (!XteaEncrypt(XteaKey))
                return false;

            AddAdler32();
            InsertPacketHeader();

            return true;
        }

        public bool PrepareToRead()
        {
            return PrepareToRead(c.XteaKey);
        }

        public bool PrepareToRead(uint[] XteaKey)
        {
            if (!XteaDecrypt(XteaKey))
                return false;

            position = 6;
            return true;
        }

        public void InsetLogicalPacketHeader()
        {
            Array.Copy(BitConverter.GetBytes((ushort)length - 8), 0, buffer, 6, 2);
        }

        public void UpdateLogicalPacketHeader()
        {
            Array.Copy(BitConverter.GetBytes((ushort)(length - 8)), 0, buffer, 6, 2);
        }

        #endregion

        #region Adler32 Wrappers

        public bool CheckAdler32()
        {
            if (AdlerChecksum.Generate(ref buffer, 6, length) != GetAdler32())
                return false;

            return true;
        }

        public void AddAdler32()
        {
            Array.Copy(BitConverter.GetBytes(AdlerChecksum.Generate(ref buffer, 6, length)), 0, buffer, 2, 4);
        }

        private uint GetAdler32()
        {
            return BitConverter.ToUInt32(buffer, 2);
        }


        #endregion

        #region Xtea Wrappers

        public bool XteaDecrypt()
        {
            return XteaDecrypt(c.XteaKey);
        }

        public bool XteaDecrypt(uint[] key)
        {
            return Xtea.XteaDecrypt(ref buffer, ref length, 6, key);
        }

        public bool XteaEncrypt()
        {
            return XteaEncrypt(c.XteaKey);
        }

        public bool XteaEncrypt(uint[] key)
        {
            return Xtea.XteaEncrypt(ref buffer, ref length, 6, key);
        }

        #endregion
    }
}
