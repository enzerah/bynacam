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

namespace BynaCam
{
    public class PacketReader
    {
        Client client;
        StreamReader stream;
        TibiaNetwork Network;
        string movieFile;
        public int speed = 1;
        public string movieVer;
        public bool readingDone = false;

        public PacketReader(Client c, TibiaNetwork network, string moviePath)
        {
            client = c;
            if (client == null)
                throw new NullReferenceException();
            if (!File.Exists(moviePath))
                throw new FileNotFoundException();

            Network = network;//gameserver.dll
            movieFile = moviePath;
            stream = new StreamReader(new DeflateStream(File.Open(movieFile, FileMode.Open), CompressionMode.Decompress));
        }

        public void ReadAllPackets()
        {
            new Thread(new ThreadStart(delegate()
            {
                TimeSpan delay = TimeSpan.Zero;
                byte[] packet = new byte[0];

                stream.ReadLine();

                while (!stream.EndOfStream)
                {
                    try
                    {
                        delay = TimeSpan.Parse(stream.ReadLine());
                        packet = stream.ReadLine().ToBytesAsHex();
                    }
                    catch 
                    {
                        Thread.Sleep(3000);
                        readingDone = true;
                        Thread.CurrentThread.Abort(); 
                    }

                    if (packet == null)
                        continue;

                    if (packet[0] == 0xc8 //setoufit block
                        || packet[0] == (byte)IncomingPacketType.ChannelList ////channellist block
                        || packet[0] == 0x96 //textwindow block
                        || packet[0] == 0x14 //disconnectclient
                        || packet[0] == (byte)IncomingPacketType.HouseTextWindow
                        || packet[0] == (byte)IncomingPacketType.ItemTextWindow
                        || packet[0] == (byte)IncomingPacketType.RuleViolationOpen
                        || packet[0] == (byte)IncomingPacketType.ShopWindowOpen
                        || packet[0] == (byte)IncomingPacketType.ShowTutorial)
                        continue;

                    if (packet[0] == 0x65 || packet[0] == 0x66 || packet[0] == 0x67 || packet[0] == 0x68
                         || packet[0] == (byte)IncomingPacketType.MapDescription
                         || packet[0] == (byte)IncomingPacketType.SelfAppear
                         || packet[0] == (byte)IncomingPacketType.WorldLight)
                    {
                        try
                        {
                            Network.uxGameServer.Send(packet);
                        }
                        catch { readingDone = true; return; }
                        continue;
                    }
                    else
                    {
                        Thread.Sleep(delay.Milliseconds / speed);
                    }

                    try
                    {
                        Network.uxGameServer.Send(packet);
                    }
                    catch { readingDone = true; return; }
                }
                Thread.Sleep(3000);
                readingDone = true;
            })).Start();
        }

        public void updateClientTitle(Client client)
        {
            try
            {
                client.Title = "BynaCam -> speed: x" + speed;
            }
            catch { }
        }

        public void setUpKeyboardHook(Client client)
        {
            KeyboardHook.Enable();
            KeyboardHook.KeyDown = null;
            KeyboardHook.KeyDown += new KeyboardHook.KeyboardHookHandler(delegate(Keys key)
            {
                if (client.IsActive)
                {
                    if (key == Keys.Right)
                    {
                        if (speed == 50)
                            return false;
                        speed++;
                        updateClientTitle(client);
                    }
                    if (key == Keys.Left)
                    {
                        if (speed == 1)
                            return false;
                        speed--;
                        updateClientTitle(client);
                    }
                    if (key == Keys.Up)
                    {
                        speed = 50;
                        updateClientTitle(client);
                    }
                    if (key == Keys.Down)
                    {
                        speed = 1;
                        updateClientTitle(client);
                    }

                    if (key == Keys.Left || key == Keys.Right
                        || key == Keys.Down || key == Keys.Up)
                        return false;
                }
                return true;
            });
            KeyboardHook.KeyDown += null;
        }
    }
}
