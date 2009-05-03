using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace BynaCam_Recorder
{
    public static class Kernel
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 
        [STAThreadAttribute]
        static void Main(string[] args)
        {
            System.Diagnostics.Process.GetCurrentProcess().PriorityClass = System.Diagnostics.ProcessPriorityClass.High;
            Application.EnableVisualStyles();
            Application.Run(new Form1(args));
        }

        public static optionsForm OptionsForm = new optionsForm();
        public static aboutForm AboutForm = new aboutForm();
    }
}
