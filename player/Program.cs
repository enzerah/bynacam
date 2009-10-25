using System;
using System.Collections.Generic;
using System.Text;
using Player.Server;
using System.Net.Sockets;
using System.Windows.Forms;

namespace Player
{
    class Program
    {
        public static Client client;
        public static BynFile file;

        static GameServer game;
        static LoginServer login;

        //starts login and game server
        static ushort StartServers(Client client)
        {
            ushort loginport = Server.Port.GetFreePort();
            ushort gameport = Server.Port.GetFreePort((ushort)(loginport + 1));
            game = new Server.GameServer(client);
            game.ClientConnected += new EventHandler(game_ClientConnected);
            game.Start(gameport);
            login = new Server.LoginServer(client, loginport);
            login.Start(gameport);
            return gameport;
        }

        private static void game_ClientConnected(object gameserver, EventArgs e)
        {
            GameServer game = (GameServer)gameserver;
            try
            {
                Player.ClientConnected(game);
            }
            catch (Exception ex)
            {
                if (!client.HasExited) client.Process.Kill();
                System.Windows.Forms.MessageBox.Show("Exception occured:\r\n\r\n" + ex.Message + "\r\nStack:\r\n" + ex.StackTrace, "BynaCam Error!", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error, System.Windows.Forms.MessageBoxDefaultButton.Button1, System.Windows.Forms.MessageBoxOptions.DefaultDesktopOnly);
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
        }

        private static void client_Exited(object sender, EventArgs e)
        {
            try
            {
                game.Stop();
                login.Stop();
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
            catch { }
        }

        static void UpdateTitle(object sender, EventArgs e)
        {
            try
            {
                TimeSpan length = TimeSpan.FromMilliseconds(file.MovieLength);
                if (!Player.paused)
                {
                    client.WindowTitle = "BynaCam - [" + Player.currentTime.Hours + ":" + Player.currentTime.Minutes + ":" + Player.currentTime.Seconds + " / " +
                        length.Hours + ":" + length.Minutes + ":" + length.Seconds + "] - Speed: x" + Player.speed;
                }
                else
                    client.WindowTitle = "BynaCam - [PAUSED]";
            }
            catch { }
        }

        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                string mpath = string.Empty;
                if (args.Length > 0)
                    mpath = args[0];
                else
                    mpath = Files.GetMoviePath();
                file = new BynFile(mpath);
                file.Buffer.Position = 0;
                client = Client.OpenMC(Files.GetClientPath(file.TibiaVersion), null);
            }
            catch { System.Diagnostics.Process.GetCurrentProcess().Kill(); }
            
            client.WindowTitle = "BynaCam";
            client.Exited += new EventHandler(client_Exited);
            StartServers(client);
            client.LoginCam("1", "1");

            Timer titleTimer = new Timer();
            titleTimer.Tick += new EventHandler(UpdateTitle);
            titleTimer.Interval = 500;
            titleTimer.Start();

            Utilities.globalKeyboardHook gkh = new Utilities.globalKeyboardHook();
            gkh.HookedKeys.Add(System.Windows.Forms.Keys.Up);
            gkh.HookedKeys.Add(System.Windows.Forms.Keys.Down);
            gkh.HookedKeys.Add(System.Windows.Forms.Keys.Left);
            gkh.HookedKeys.Add(System.Windows.Forms.Keys.Right);
            gkh.HookedKeys.Add(System.Windows.Forms.Keys.Pause);
            gkh.HookedKeys.Add(System.Windows.Forms.Keys.Back);
            gkh.KeyDown += new System.Windows.Forms.KeyEventHandler(delegate(object sender, KeyEventArgs e)
                {
                    if (client.WindowHandle == WinApi.GetForegroundWindow()) //isactive
                    {
                        if (e.KeyCode == Keys.Right)
                        {
                            if (Player.speed == 50)
                            {
                                e.Handled = false;
                                return;
                            }
                            Player.speed++;
                        }
                        if (e.KeyCode == Keys.Left)
                        {
                            if (Player.speed == 1)
                            {
                                e.Handled = false;
                                return;
                            }
                            Player.speed--;
                        }
                        if (e.KeyCode == Keys.Up)
                        {
                            Player.speed = 50;
                            e.Handled = false;
                        }
                        if (e.KeyCode == Keys.Down)
                        {
                            Player.speed = 1;
                            e.Handled = false;
                        }

                        if (e.KeyCode == Keys.Pause && !Player.paused)
                        {
                            Player.paused = true;
                        }
                        else if (e.KeyCode == Keys.Pause && Player.paused)
                        {
                            Player.paused = false;
                        }
                        if (e.KeyCode == Keys.Back)
                        {
                            int ms = (int)Player.currentTime.TotalMilliseconds - (30 * 1000);
                            if (ms <= 0)
                                ms = 0;

                            Player.GotoTime(TimeSpan.FromMilliseconds(ms));
                        }

                        if (e.KeyCode == Keys.Left || e.KeyCode == Keys.Right
                        || e.KeyCode == Keys.Down || e.KeyCode == Keys.Up || e.KeyCode == Keys.Pause)
                        {
                            UpdateTitle(null, null);
                        }
                    }
                    
                    e.Handled = true;
                });

            System.Windows.Forms.Application.Run();
        }
    }
}
