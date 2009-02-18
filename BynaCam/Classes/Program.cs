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
            client = Client.OpenMC(ConfigClient.getClientPath(), "");
            
            if (client != null)
            {
                if (!KeyboardHook.Enable())
                    Messages.Error("Could not read DI keyboard!");

                client.Exited += new EventHandler(client_Exited);
                string camfilepath = FileChooser.getCamFilePath(client);
                
                PacketReader packetReader = new PacketReader(client, camfilepath);
                new KeyHook().setUpKeyboardHook(client, packetReader); //KeyDown
                Thread.Sleep(500);
                client.AutoLogin("1", "1", "Byna", "BynaCam");

                while (!packetReader.Network.gameServer.Accepted)
                    Thread.Sleep(100);

                packetReader.ReadAllPackets();

                while (!packetReader.readingDone)
                {
                    TibiaClient.updateTitle(client, packetReader.speed, packetReader.actualTime, packetReader.movieTime);
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
