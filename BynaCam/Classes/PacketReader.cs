using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tibia;
using Tibia.Objects;
using Tibia.Util;
using Tibia.Packets;
using System.Threading;
using System.IO;
using System.Windows.Forms;
using System.IO.Compression;
using System.Diagnostics;

namespace BynaCam
{
    public class PacketReader
    {
        Client client;
        FileStream stream;
        DeflateStream defStream;
        TibiaNetwork Network;
        string movieFile;
        public double speed = 1.0;
        public bool readingDone = false;
        Tibia.Util.Timer movieTimer;

        TimeSpan packetDelay = TimeSpan.Zero;
        byte[] truePacket = new byte[0];
        public TimeSpan packetTime = TimeSpan.Zero;

        string TibiaVer = string.Empty;
        public TimeSpan movieTime = TimeSpan.Zero;
        public TimeSpan actualTime = TimeSpan.Zero;

        Stopwatch addWatch = new Stopwatch();

        #region Constructor

        public PacketReader(Client c, TibiaNetwork network, string moviePath)
        {
            client = c;
            if (client == null)
                throw new NullReferenceException();
            if (!File.Exists(moviePath))
                throw new FileNotFoundException();

            Network = network;//gameserver.dll
            movieFile = moviePath;
            try
            {
                stream = new FileStream(movieFile, FileMode.Open);
                defStream = new DeflateStream(stream, CompressionMode.Decompress);
            }
            catch 
            { 
                Messages.Error("Could not read file!");
                try { c.Process.Kill(); }
                catch { }
                Process.GetCurrentProcess().Kill(); 
            }
        }

        #endregion

        #region getHeader / getPacket
        private bool getHeader()
        {
            byte[] buffer = new byte[2];
            ushort len;
            
            try
            {
                //Tibia Version
                stream.Read(buffer, 0, 2);
                len = BitConverter.ToUInt16(buffer, 0);
                buffer = new byte[len];
                stream.Read(buffer, 0, len);
                TibiaVer = buffer.ToPrintableString(0, len);
                //Playing Time
                buffer = new byte[2];
                stream.Read(buffer, 0, 2);
                len = BitConverter.ToUInt16(buffer, 0);
                buffer = new byte[len];
                stream.Read(buffer, 0, len);
                movieTime = TimeSpan.Parse(buffer.ToPrintableString(0, len));
            }
            catch { return false; }
            return true;
        }

        private bool getPacket()
        {
            byte[] buffer = new byte[2];
            ushort len;
            try
            {
                defStream.Read(buffer, 0, 1);
                defStream.Read(buffer, 0, 2);
                len = BitConverter.ToUInt16(buffer, 0);
                buffer = new byte[len];
                defStream.Read(buffer, 0, len);
                packetDelay = TimeSpan.Parse(buffer.ToPrintableString(0, len));

                defStream.Read(buffer, 0, 1);
                defStream.Read(buffer, 0, 2);
                len = BitConverter.ToUInt16(buffer, 0);
                buffer = new byte[len];
                defStream.Read(buffer, 0, len);
                truePacket = buffer;
                return true;
            }
            catch { return false; }
        }
        #endregion

        private void getTime()
        {
            byte[] buffer = new byte[2];
            ushort len;

            //Time
            defStream.Read(buffer, 0, 1);
            defStream.Read(buffer, 0, 2); //len
            len = BitConverter.ToUInt16(buffer, 0);
            buffer = new byte[len];
            defStream.Read(buffer, 0, len);
            packetTime = TimeSpan.Parse(buffer.ToPrintableString(0, len));
        }

        #region sendPacket
        private void sendPacket(byte[] packet, TimeSpan delay)
        {
            Thread.Sleep((int)(packetDelay.TotalMilliseconds / speed));
            try
            {
                Network.uxGameServer.Send(truePacket);
            }
            catch { readingDone = true; }
        }
        #endregion

        public void ReadAllPackets()
        {
            movieTimer = new Tibia.Util.Timer(100, false);
            movieTimer.Execute += new Tibia.Util.Timer.TimerExecution(delegate
            {
                    try
                    {
                        actualTime = packetTime + TimeSpan.FromTicks(addWatch.Elapsed.Ticks / packetDelay.Ticks);
                    }
                    catch { }
            });

            new Thread(new ThreadStart(delegate()
            {
                getHeader();
                movieTimer.Start();

                if (TibiaVer != client.Version)
                {
                    Messages.Error("Tibia Version does not match to this BynaCam Version!");
                    try { client.Process.Kill(); }
                    catch { }
                    Process.GetCurrentProcess().Kill(); 
                }

                while (defStream.CanRead)
                {
                    try 
                    {
                        try { getTime(); }
                        catch { }
                        if (!getPacket())
                            break;

                        addWatch.Start();
                        sendPacket(truePacket, packetDelay);
                        addWatch.Stop();
                    }
                    catch { continue; }
                }

                try
                {
                    movieTimer.Stop();
                    actualTime = movieTime;
                    TibiaClient.updateTitle(client, speed, actualTime, movieTime);
                    Thread.Sleep(1000);
                    movieTimer.Stop();
                    readingDone = true;
                }
                catch { Process.GetCurrentProcess().Kill(); }
                })).Start();
        } 
    }
}
