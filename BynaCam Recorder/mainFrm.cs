using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Tibia.Objects;
using Tibia;
using Tibia.Packets;
using Tibia.Util;
using System.Diagnostics;
using System.IO.Compression;
using BynaCam_Recorder.Classes;

namespace BynaCam_Recorder
{
    public partial class mainFrm : Form
    {
        public static Client c;
        public static FileHandler fileHandler;
        PacketHandler packetHandler;

        public mainFrm()
        {
            InitializeComponent();
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;
        }

        private void mainFrm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.WindowState = FormWindowState.Minimized;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            c = ConfigClient.getIniClient();
            fileHandler = new FileHandler();
            fileHandler.Open(FileChooser.getBynaCamFile(c));
            
            if (c != null)
            {
                notifyIcon1.ShowBalloonTip(5000, "BynaCam", "BynaCam is waiting for login...", ToolTipIcon.Info);
                this.TopMost = true;
                packetHandler = new PacketHandler(c);
                c.Exited += new EventHandler(c_Exited);
                c.Proxy.PlayerLogin += new EventHandler(Proxy_PlayerLogin);
            }
            else
            {
                MessageBox.Show("Choose your client first!!");
                notifyIcon1.Visible = false;
                Process.GetCurrentProcess().Kill();
            }

         }

        private void saveAndExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fileHandler.WriteHeader(PacketQueue.allTime.Elapsed);
            fileHandler.deflateStream.Close();
            fileHandler.fileStream.Close();
            notifyIcon1.Visible = false;
            c.Process.Kill();
            Process.GetCurrentProcess().Kill();
        }

        private void c_Exited(object sender, EventArgs e)
        {
            fileHandler.WriteHeader(PacketQueue.allTime.Elapsed);
            fileHandler.deflateStream.Close();
            fileHandler.fileStream.Close();
            notifyIcon1.Visible = false;
            Process.GetCurrentProcess().Kill();
        }

        private void Proxy_PlayerLogin(object sender, EventArgs e)
        {
            notifyIcon1.ShowBalloonTip(2000, "BynaCam", "BynaCam is recording!\r\nTo stop recording just exit your Tibia Client!", ToolTipIcon.Info);
            
            this.Hide();
        }

        #region Hiding skills, pms etc
        public static bool hidePm = false;
        public static bool hideMsg = false;
        public static bool hideSkills = false;
        public static bool hideVips = false;

        private void cb_vips_CheckedChanged(object sender, EventArgs e)
        {
            hideVips = cb_vips.Checked;
        }

        private void cb_messages_CheckedChanged(object sender, EventArgs e)
        {
            hideMsg = cb_messages.Checked;
        }

        private void cb_pms_CheckedChanged(object sender, EventArgs e)
        {
            hidePm = cb_pms.Checked;
        }

        private void cb_skills_CheckedChanged(object sender, EventArgs e)
        {
            hideSkills = cb_skills.Checked;
        }
        #endregion
    }
}
