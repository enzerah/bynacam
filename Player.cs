using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Windows.Forms;

namespace Player
{
    public class Player
    {
        public static uint speed = 1;
        public static bool paused = false;
        public static TimeSpan currentTime = new TimeSpan();

        static TimeSpan ffTime = new TimeSpan();
        static bool makeFf = false;
        static bool ffReseted = false;

        private static FileMessage GetFileMessage(Stream stream)
        {
            FileMessage msg = new FileMessage();
            byte[] temp = new byte[1];
            stream.Read(temp, 0, 1);
            msg.id = (MSG_ID)temp[0];

            switch (msg.id)
            {
                case MSG_ID.DELAY_ID:
                    {
                        temp = new byte[4];
                        stream.Read(temp, 0, 4);
                        msg.buffer = temp;
                        break;
                    }
                case MSG_ID.VERSION_ID:
                    {
                        temp = new byte[4];
                        stream.Read(temp, 0, 4);
                        msg.buffer = temp;
                        break;
                    }
                case MSG_ID.MOVIE_LEN_ID:
                    {
                        temp = new byte[4];
                        stream.Read(temp, 0, 4);
                        msg.buffer = temp;
                        break;
                    }
                case MSG_ID.PACKET_ID:
                    {
                        temp = new byte[4];
                        stream.Read(temp, 0, 4);
                        int plen = BitConverter.ToInt32(temp, 0);
                        temp = new byte[plen];
                        stream.Read(temp, 0, plen);
                        msg.buffer = temp;
                        break;
                    }
            }
            return msg;
        }

        public static void GotoTime(TimeSpan time)
        {
            ffTime = time;
            makeFf = true;
        }

        public static void ClientConnected(Server.GameServer stream)
        {
            BynFile file = Program.file;

            while (file.Buffer.Position < file.Buffer.Length)
            {
                if (makeFf && !ffReseted)
                {
                    ffReseted = true;
                    currentTime = TimeSpan.Zero;
                    file.Buffer.Position = 0;
                }
                FileMessage msg = GetFileMessage(file.Buffer);

                switch (msg.id)
                {
                    case MSG_ID.DELAY_ID:
                        {
                            int delay = BitConverter.ToInt32(msg.buffer, 0);

                            if (makeFf)
                            {
                                currentTime += TimeSpan.FromMilliseconds(delay);
                                if (ffTime > currentTime)
                                    break;
                                else
                                {
                                    makeFf = false;
                                    ffReseted = false;
                                }
                            }

                            int newdelay = delay / (int)speed;
                            int prevspeed = (int)speed;

                            if (newdelay <= 2)
                                newdelay = 0;

                            while (prevspeed == speed)
                            {
                                if ((newdelay - 100) > 0)
                                {
                                    newdelay -= 100;
                                    System.Threading.Thread.Sleep(100);
                                }
                                else
                                {
                                    System.Threading.Thread.Sleep(newdelay);
                                    break;
                                }
                            }

                            if (!makeFf)
                                currentTime += TimeSpan.FromMilliseconds(delay);

                            break;
                        }
                    case MSG_ID.PACKET_ID:
                        {
                            if (msg.buffer.Length == 0)
                                break;
                            while (paused)
                                System.Threading.Thread.Sleep(100);

                            try
                            {
                                stream.Send(msg.buffer);
                            }
                            catch { }
                            break;
                        }
                }
            }
        }
    }

    public enum MSG_ID
    {
        MOVIE_LEN_ID = 0x63,
        VERSION_ID = 0x64,
        DELAY_ID = 0x65,
        PACKET_ID = 0x66
    };

    public struct FileMessage
    {
        public MSG_ID id;
        public byte[] buffer;
    }
}
