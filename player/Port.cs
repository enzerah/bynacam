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
using System.Net.Sockets;
using System.Net; 

namespace Player.Server
{
    public static class Port
    {
        #region Port Checking
        /// <summary>
        /// Check if a port is open on localhost
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        public static bool CheckPort(ushort port)
        {
            try
            {
                TcpListener tcpScan = new TcpListener(IPAddress.Any, port);
                tcpScan.Start();
                tcpScan.Stop();

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Get the first free port on localhost starting at the default 7171
        /// </summary>
        /// <returns></returns>
        public static ushort GetFreePort()
        {
            return GetFreePort(7171);
        }

        /// <summary>
        /// Get the first free port on localhost beginning at start
        /// </summary>
        /// <param name="start"></param>
        /// <returns></returns>
        public static ushort GetFreePort(ushort start)
        {
            while (!CheckPort(start))
            {
                start++;
            }

            return start;
        }
        #endregion
    }
}
