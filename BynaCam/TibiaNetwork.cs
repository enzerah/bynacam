using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Tibia.Objects;

namespace BynaCam
{
    class TibiaNetwork
    {
        public GameServer.GameServer uxGameServer;
        public TibiaNetwork(Client client)
        {
            uxGameServer = new GameServer.GameServer(client);
            LoginServer.Login login = new LoginServer.Login();

            Thread t = new Thread(new ThreadStart(delegate()
            {
                login.StartServer(client, 7171, "Byna", "BynaCam", new byte[] { 127, 0, 0, 1 }, 7172);
                uxGameServer.SetServer(7172);
            }));
            t.Start();
        }
    }
}
