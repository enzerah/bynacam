using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tibia.Objects;

namespace BynaCam
{
    public static class TibiaClient
    {
        public static void updateTitle(Client c, double speed, TimeSpan actualTime, TimeSpan movieTime)
        {
            c.Title = "BynaCam :: Speed x"+speed+" :: Time ["+actualTime.Hours+":"+actualTime.Minutes+":"+actualTime.Seconds+" / "+movieTime.Hours+":"+movieTime.Minutes+":"+movieTime.Seconds+"]";
        }
    }
}
