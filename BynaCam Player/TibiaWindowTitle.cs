using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tibia.Objects;
using Tibia.Util;

namespace BynaCam_Player
{
    public class TibiaWindowTitle
    {
        Client client;
        public TimeSpan ActualTime;

        public TibiaWindowTitle(Client c)
        {
            client = c;
            Timer updateTimer = new Timer(200, true);
            updateTimer.Execute += new Timer.TimerExecution(delegate()
                {
                    if (client != null)
                      setClientTitle(client);
                });
        }
        public void setClientTitle(Client c)
        {
            c.Window.Title = "BynaCam :: Speed x" + Kernel.Speed + " :: Time [" + Kernel.packetTime.Hours + ":" + Kernel.packetTime.Minutes + ":" + Kernel.packetTime.Seconds + " / " + Kernel.movieTime.Hours + ":" + Kernel.movieTime.Minutes + ":" + Kernel.movieTime.Seconds + "]";
        }
    }
}
