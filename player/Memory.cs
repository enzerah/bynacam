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
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Player
{
    public class Memory
    {
        private IntPtr pHandle;

        public Memory(Process process)
        {
            pHandle = process.Handle;
        }
        public Memory(IntPtr handle)
        {
            pHandle = handle;
        }

        public byte[] ReadBytes(long address, uint bytesToRead)
        {
            IntPtr ptrBytesRead;
            byte[] buffer = new byte[bytesToRead];

            WinApi.ReadProcessMemory(pHandle, new IntPtr(address), buffer, bytesToRead, out ptrBytesRead);

            return buffer;
        }

        public byte ReadByte(long address)
        {
            return ReadBytes(address, 1)[0];
        }

        public short ReadInt16(long address)
        {
            return BitConverter.ToInt16(ReadBytes(address, 2), 0);
        }

        public ushort ReadUInt16(long address)
        {
            return BitConverter.ToUInt16(ReadBytes(address, 2), 0);
        }

        public int ReadInt32(long address)
        {
            return BitConverter.ToInt32(ReadBytes(address, 4), 0);
        }

        public uint ReadUInt32(long address)
        {
            return BitConverter.ToUInt32(ReadBytes(address, 4), 0);
        }

        public double ReadDouble(long address)
        {
            return BitConverter.ToDouble(ReadBytes(address, 8), 0);
        }

        public string ReadString(long address)
        {
            return ReadString(address, 0);
        }

        public string ReadString(long address, uint length)
        {
            if (length > 0)
            {
                byte[] buffer;
                buffer = ReadBytes(address, length);
                return System.Text.ASCIIEncoding.Default.GetString(buffer).Split(new Char())[0];
            }
            else
            {
                string s = "";
                byte temp = ReadByte(address++);
                while (temp != 0)
                {
                    s += (char)temp;
                    temp = ReadByte(address++);
                }
                return s;
            }
        }

        public bool WriteBytes(long address, byte[] bytes, uint length)
        {
            IntPtr bytesWritten;

            // Write to memory
            int result = WinApi.WriteProcessMemory(pHandle, new IntPtr(address), bytes, length, out bytesWritten);

            return result != 0;
        }

        public bool WriteInt32(long address, int value)
        {
            return WriteBytes(address, BitConverter.GetBytes(value), 4);
        }

        public bool WriteUInt32(long address, uint value)
        {
            return WriteBytes(address, BitConverter.GetBytes(value), 4);
        }

        public bool WriteInt16(long address, short value)
        {
            return WriteBytes(address, BitConverter.GetBytes(value), 2);
        }

        public bool WriteUInt16(long address, ushort value)
        {
            return WriteBytes(address, BitConverter.GetBytes(value), 2);
        }

        public bool WriteDouble(long address, double value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            return WriteBytes(address, bytes, 8);
        }

        public bool WriteByte(long address, byte value)
        {
            return WriteBytes(address, new byte[] { value }, 1);
        }

        public bool WriteString(long address, string str)
        {
            str += '\0';
            byte[] bytes = System.Text.ASCIIEncoding.Default.GetBytes(str);
            return WriteBytes(address, bytes, (uint)bytes.Length);
        }

        public bool WriteRSA(long address, string newKey)
        {
            IntPtr bytesWritten;
            int result;
            uint oldProtection = 0;

            System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
            byte[] bytes = enc.GetBytes(newKey);

            // Make it so we can write to the memory block
            WinApi.VirtualProtectEx(pHandle, new IntPtr(address), new IntPtr(bytes.Length), WinApi.PAGE_EXECUTE_READWRITE, ref oldProtection);

            // Write to memory
            result = WinApi.WriteProcessMemory(pHandle, new IntPtr(address), bytes, (uint)bytes.Length, out bytesWritten);

            // Put the protection back on the memory block
            WinApi.VirtualProtectEx(pHandle, new IntPtr(address), new IntPtr(bytes.Length), oldProtection, ref oldProtection);

            return (result != 0);
        }
    }
}
