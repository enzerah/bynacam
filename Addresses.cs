using System;
using System.Collections.Generic;
using System.Text;

namespace Player
{
    public class Addresses
    {
        /// <summary>
        /// Address to activate multiclient.
        /// </summary>
        public static uint MultiClient = 0x5067E4; //8.52

        /// <summary>
        /// Value to be written to the multiclient address(JMP).
        /// </summary>
        public static byte MultiClientJMP = 0xEB;

        // Login Server addresses
        public static uint LoginServerStart = 0x787E30; //8.52
        public static uint StepLoginServer = 112;
        public static uint DistancePort = 100;
        public static uint MaxLoginServers = 10;

        /// <summary>
        /// 8 = Connected | 0 = Disconnected
        /// </summary>
        public static uint Status = 0x790558; //8.52

        /// <summary>
        /// Address to the XTea encryption key.
        /// </summary>
        public static uint XTeaKey = 0x78CEF4; //8.52

        /// <summary>
        /// Distance between creatures.
        /// </summary>
        public static uint StepCreatures = 0xA0;

        /// <summary>
        /// Maximum number of creatures.
        /// </summary>
        public static uint MaxCreatures = 250;

        /// <summary>
        /// Start of the battle list.
        /// </summary>
        public static uint Start = 0x633EF0; //8.52

        /// <summary>
        /// End of the battle list.
        /// </summary>
        public static uint End = Start + (StepCreatures * MaxCreatures);


    }
}
