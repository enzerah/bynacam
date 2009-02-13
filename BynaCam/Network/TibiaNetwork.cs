using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Tibia.Objects;

namespace BynaCam
{
    public class TibiaNetwork
    {
        public GameServer.GameServer uxGameServer;
        public TibiaNetwork(Client client)
        {
            new Thread(new ThreadStart(delegate()
            {
                LoginServer.Login login = new LoginServer.Login();
                login.StartServer(client, 7171, "Byna", "BynaCam", new byte[] { 127, 0, 0, 1 }, 7172);
                uxGameServer = new GameServer.GameServer(client);
                uxGameServer.SetServer(7172);
            })).Start();
        }
    }
}
