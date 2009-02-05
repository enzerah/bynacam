using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tibia;
using Tibia.Objects;
using Tibia.Util;
using System.Diagnostics;
using Tibia.Packets;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace BynaCam
{
    class MainClass
    {
        static Client client;

        private static string getCamFilePath()
        {
            //Open File Dialog
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.CheckFileExists = true;
            dialog.Multiselect = false;
            dialog.Filter = "BynaCam Files|*.byn";
            dialog.Title = "Open BynaCam file.";
            if (dialog.ShowDialog(new WindowWrapper(client.MainWindowHandle)) == DialogResult.Cancel)
            {
                MessageBox.Show(new WindowWrapper(client.MainWindowHandle), "Cannot open BynaCam file!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                client.Process.Kill();
                Process.GetCurrentProcess().Kill();
            }
            if (!File.Exists(dialog.FileName))
                return null;

            return dialog.FileName;
        }

        [STAThreadAttribute]
        static void Main(string[] args)
        {
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;
            client = IniClient.getIniClient();
            
            if (client != null)
            {
                string camfilepath = getCamFilePath();
                
                TibiaNetwork network = new TibiaNetwork(client);
                PacketReader reader = new PacketReader(client, network, camfilepath);
                client.AutoLogin("1", "1", "Byna", "BynaCam");
                
                try 
                { 
                    while (!network.uxGameServer.Accepted) 
                    {
                        Thread.Sleep(100);
                    } 
                }
                catch { Process.GetCurrentProcess().Kill(); }
                
                reader.ReadAllPackets();

                while (!reader.readingDone)
                {
                    reader.setUpKeyboardHook(client);
                    reader.updateClientTitle(client);
                    Thread.Sleep(10);
                }
            }
            else
            {
                MessageBox.Show("Could not load Tibia Client!");
                Process.GetCurrentProcess().Kill();
            }
        }

        private static void client_Exited(object sender, EventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }
    }
}
