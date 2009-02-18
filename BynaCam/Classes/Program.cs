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

        [STAThreadAttribute]
        static void Main(string[] args)
        {
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;
            client = ConfigClient.getIniClient();
            
            if (client != null)
            {
                if (!KeyboardHook.Enable())
                    Messages.Error("Could not read DI keyboard!");

                client.Exited += new EventHandler(client_Exited);
                string camfilepath = FileChooser.getCamFilePath(client);
                
                TibiaNetwork network = new TibiaNetwork(client);
                PacketReader reader = new PacketReader(client, network, camfilepath);
                new KeyHook().setUpKeyboardHook(client, reader);
                Thread.Sleep(500);
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
                    
                    TibiaClient.updateTitle(client, reader.speed, reader.actualTime, reader.movieTime);
                    Thread.Sleep(100);
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
