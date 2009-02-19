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
        public GameServer.GameServer gameServer;
        public TibiaNetwork(Client client)
        {
            new Thread(new ThreadStart(delegate()
            {
                ushort loginPort = Tibia.Util.Proxy.GetFreePort(7171);
                ushort gamePort = Tibia.Util.Proxy.GetFreePort(8181);

                LoginServer.Login login = new LoginServer.Login();
                login.StartServer(client, (short)loginPort, "Byna", "BynaCam", new byte[] { 127, 0, 0, 1 }, gamePort);
                gameServer = new GameServer.GameServer(client);
                gameServer.SetServer((short)gamePort);
            })).Start();
        }
    }
}
