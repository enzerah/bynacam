using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace BynaCam_Player
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] argss)
        {
            string[] args = {"c:\\movie.byn" };
            System.Diagnostics.Process.GetCurrentProcess().PriorityClass = System.Diagnostics.ProcessPriorityClass.High;
            Main main = new Main(args);
            Application.Run();
        }
    }
}
