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
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using System.Threading;

namespace Player
{
    public class Client
    {
        #region Vars
        private Process hProcess;
        private Memory hMem;
        private Input hInput;
        public event EventHandler Exited;
        #endregion

        #region Constructor
        public Client(Process process)
        {
            hProcess = process;
            hProcess.EnableRaisingEvents = true;
            hProcess.Exited += new EventHandler(this.Client_Exited);
            hMem = new Memory(hProcess);
            hInput = new Input(this);
        }
        #endregion

        public static Client OpenMC(string path, string arguments)
        {
            WinApi.PROCESS_INFORMATION pi = new WinApi.PROCESS_INFORMATION();
            WinApi.STARTUPINFO si = new WinApi.STARTUPINFO();

            if (arguments == null)
                arguments = "";

            WinApi.CreateProcess(path, " " + arguments, IntPtr.Zero, IntPtr.Zero,
                false, WinApi.CREATE_SUSPENDED, IntPtr.Zero,
                System.IO.Path.GetDirectoryName(path), ref si, out pi);

            IntPtr handle = WinApi.OpenProcess(WinApi.PROCESS_ALL_ACCESS, 0, pi.dwProcessId);
            Memory memory = new Memory(handle);
            Process p = Process.GetProcessById(Convert.ToInt32(pi.dwProcessId));
            memory.WriteByte((long)Addresses.MultiClient, Addresses.MultiClientJMP);
            WinApi.ResumeThread(pi.hThread);
            p.WaitForInputIdle();
            WinApi.CloseHandle(handle);
            WinApi.CloseHandle(pi.hProcess);
            WinApi.CloseHandle(pi.hThread);

            return new Client(p);
        }

        public bool SetServer(string ip, short port)
        {
            bool result = true;
            long pointer = Addresses.LoginServerStart;

            ip += (char)0;

            for (int i = 0; i < Addresses.MaxLoginServers; i++)
            {
                result &= Memory.WriteString(pointer, ip);
                result &= Memory.WriteInt32(pointer + Addresses.DistancePort, port);
                pointer += Addresses.StepLoginServer;
            }
            return result;
        }

        public bool LoginCam(string login, string password)
        {
            //if the player is logged
            if (LoggedIn)
                return false;

            //sure the screen is clean, no dialog open
            Input.SendKey(Keys.Escape);
            Input.SendKey(Keys.Escape);
            
            //click the enter the game button
            Input.Click(120, (Rect.bottom - Rect.top) - 250);

            Thread.Sleep(2000);

            //now we have to send the login and the password
            Input.SendString(login);
            //press tab
            Input.SendKey(Keys.Tab);
            //put the pass
            Input.SendString(password);
            //press entrer..
            Input.SendKey(Keys.Enter);

            Thread.Sleep(2000);

            //we start at position 0
            Input.SendKey(Keys.Enter);

            //ok.
            return true;
        }

        private void Client_Exited(object sender, EventArgs e)
        {
            if (Exited != null)
                Exited.Invoke(sender, e);
        }

        #region Propeties
        public bool LoggedIn
        {
            get 
            {
                if (Memory.ReadByte(Addresses.Status) == 8)
                    return true;
                else
                    return false;
            }
        }
        public Memory Memory
        {
            get { return hMem; }
        }
        public Process Process
        {
            get { return hProcess; }
        }
        public bool HasExited
        {
            get { return hProcess.HasExited; }
        }
        public IntPtr WindowHandle
        {
            get
            {
                if (Process.MainWindowHandle == IntPtr.Zero)
                    Process.Refresh();

                return Process.MainWindowHandle;
            }
        }
        public WinApi.RECT Rect
        {
            get
            {
                WinApi.RECT r = new WinApi.RECT();
                WinApi.GetWindowRect(WindowHandle, ref r);
                return r;
            }
        }
        public Input Input
        {
            get { return hInput; }
        }
        public uint[] XteaKey
        {
            get
            {
                uint[] xtea = new uint[4];
                xtea[0] = BitConverter.ToUInt32(Memory.ReadBytes(Addresses.XTeaKey, 4), 0);
                xtea[1] = BitConverter.ToUInt32(Memory.ReadBytes(Addresses.XTeaKey + 4, 4), 0);
                xtea[2] = BitConverter.ToUInt32(Memory.ReadBytes(Addresses.XTeaKey + 8, 4), 0);
                xtea[3] = BitConverter.ToUInt32(Memory.ReadBytes(Addresses.XTeaKey + 12, 4), 0);
                return xtea;
            }
        }
        public string WindowTitle
        {
            get { return Process.MainWindowTitle; }
            set { WinApi.SetWindowText(WindowHandle, value); }
        }

        #endregion
    }
}
