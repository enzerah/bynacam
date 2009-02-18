using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tibia;
using Tibia.Objects;
using System.Windows.Forms;

namespace BynaCam
{
    public class KeyHook
    {
        public void setUpKeyboardHook(Client client, PacketReader reader)
        {
            KeyboardHook.KeyDown = null;
            KeyboardHook.KeyDown += new KeyboardHook.KeyboardHookHandler(delegate(Keys key)
            {
                if (client.IsActive)
                {
                    if (key == Keys.Add)
                    {
                        reader.GotoPacketTime(reader.actualTime + new TimeSpan(0,3,0));
                    }
                    if (key == Keys.Subtract)
                    {
                        reader.GotoPacketTime(reader.actualTime - new TimeSpan(0,3,0));
                    }
                    if (key == Keys.Right)
                    {
                        if (reader.speed == 50)
                            return false;
                        reader.speed++;
                    }
                    if (key == Keys.Left)
                    {
                        if (reader.speed == 1)
                            return false;
                        reader.speed--;
                    }
                    if (key == Keys.Up)
                    {
                        reader.speed = 50;
                    }
                    if (key == Keys.Down)
                    {
                        reader.speed = 1;
                    }

                    if (key == Keys.Left || key == Keys.Right
                        || key == Keys.Down || key == Keys.Up
                        || key == Keys.Subtract || key == Keys.Add)
                    {
                        TibiaClient.updateTitle(client, reader.speed, reader.actualTime, reader.movieTime);
                        return false;
                    }
                }
                return true;
            });
            KeyboardHook.KeyDown += null;
        }
    }
}
