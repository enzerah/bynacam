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
using BynaCam.Classes;

namespace BynaCam
{
    public class PacketReader
    {
        Client client;
        public FileHandler fileHandler;
        public TibiaNetwork Network;
        string movieFile;
        public double speed = 1.0;
        public bool readingDone = false;
        Tibia.Util.Timer movieTimer;

        string TibiaVer = string.Empty;
        public TimeSpan movieTime = TimeSpan.Zero;
        public TimeSpan actualTime = TimeSpan.Zero;
        
        Stopwatch addWatch = new Stopwatch();

        #region Constructor

        public PacketReader(Client c, string moviePath)
        {
            client = c;
            if (client == null)
                throw new NullReferenceException();
            if (!File.Exists(moviePath))
                throw new FileNotFoundException();

            Network = new TibiaNetwork(client);
            movieFile = moviePath;
            try
            {
                fileHandler = new FileHandler();
                fileHandler.Open(moviePath);
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


        #region Send
        private void sendPacket(byte[] packet, TimeSpan delay)
        {
            Thread.Sleep((int)(delay.TotalMilliseconds / speed));
            try
            {
                Network.gameServer.Send(packet);
            }
            catch { readingDone = true; }
        }
        #endregion

        #region Update CLIENT title
        private void UpdateClientTitle()
        {
            fileHandler.ReadHeader();
            movieTime = fileHandler.playTime;

            movieTimer = new Tibia.Util.Timer(100, false);
            movieTimer.Execute += new Tibia.Util.Timer.TimerExecution(delegate
            {
                try
                {
                    TimeSpan time = TimeSpan.FromMilliseconds(addWatch.ElapsedMilliseconds) + fileHandler.packetTime;
                    if (actualTime < time)
                      actualTime = time;
                }
                catch { }
            });
            movieTimer.Start();
        }
        #endregion

        public void ReadAllPackets()
        {
            new Thread(new ThreadStart(delegate()
            {
                UpdateClientTitle();

                if (fileHandler.tibiaVersion != client.Version)
                {
                    Messages.Error("Tibia Version does not match to this BynaCam Version!");
                    try { client.Process.Kill(); }
                    catch { }
                    Process.GetCurrentProcess().Kill(); 
                }

                while (fileHandler.ReadPacket())
                {
                        addWatch = Stopwatch.StartNew();
                        sendPacket(fileHandler.packet, fileHandler.packetDelay);
                }

                try
                {
                    actualTime = movieTime;
                    movieTimer.Stop();
                    TibiaClient.updateTitle(client, speed, actualTime, movieTime);
                    Thread.Sleep(1000);
                    readingDone = true;
                }
                catch { Process.GetCurrentProcess().Kill(); }
                })).Start();
        } 
    }
}
