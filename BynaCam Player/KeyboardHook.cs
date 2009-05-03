using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tibia.Objects;

namespace BynaCam_Player
{
    public class KeyboardHook
    {
        Client client;
        public delegate void ForwardHandler();
        public event ForwardHandler FastForwardPressed;
        public event ForwardHandler FastBackPressed;

        public KeyboardHook(Client c)
        {
            client = c;

            Tibia.KeyboardHook.Enable();
            Tibia.KeyboardHook.KeyDown +=new Tibia.KeyboardHook.KeyboardHookHandler(delegate(Keys key)
                {
                    if (client.Window.IsActive)
                    {

                        if (key == Keys.Right)
                        {
                            if (Kernel.Speed == 50)
                                return false;
                            Kernel.Speed++;
                        }
                        if (key == Keys.Left)
                        {
                            if (Kernel.Speed == 1)
                                return false;
                            Kernel.Speed--;
                        }
                        if (key == Keys.Up)
                        {
                            Kernel.Speed = 50;
                        }
                        if (key == Keys.Down)
                        {
                            Kernel.Speed = 1;
                        }
                        if (key == Keys.Add)
                        {
                            if (FastForwardPressed != null)
                                FastForwardPressed.Invoke();
                        }
                        if (key == Keys.Subtract)
                        {
                            if (FastBackPressed != null)
                                FastBackPressed.Invoke();
                        }
                        if (key == Keys.Left || key == Keys.Right
                            || key == Keys.Down || key == Keys.Up
                            || key == Keys.Add || key == Keys.Subtract)
                        {
                            return false;
                        }
                    }
                    
                  return true;
                });
        }
    }
}
