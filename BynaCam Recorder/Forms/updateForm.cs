using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using Tibia.Packets;

namespace BynaCam_Recorder
{
    public partial class updateForm : Form
    {
        string updateFilePath = "http://www.bynacam.com/update.log";
        public event Action FileDownloaded;
        public event Action FileDownloadError;

        public updateForm()
        {
            InitializeComponent();
        }

        private void AddDebug(string msg)
        {
            statusListBox.Items.Add(msg);
        }

        private LogFileStruct GetLogFile()
        {
            LogFileStruct temp = new LogFileStruct();
            WebClient client = new WebClient();
            byte[] data = client.DownloadData(updateFilePath);
            NetworkMessage msg = new NetworkMessage(data);
            temp.Version = msg.GetUInt32();
            temp.LogString = msg.GetString();
            temp.downString = msg.GetString();
            return temp;
        }

        private void downloadFile(string addr)
        {
            WebClient client = new WebClient();
            client.DownloadFileAsync(new Uri(addr), "update.exe");
            client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
            client.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadFileCompleted);
        }

        private void client_DownloadProgressChanged(object sender, EventArgs e)
        {
            if (progressBar.Value < 99)
                progressBar.Value++;
        }

        private void client_DownloadFileCompleted(object sender, EventArgs e)
        {
            progressBar.Value = 100;

            Invoke(new MethodInvoker(delegate()
            {
                AddDebug("Completed!");
            }));

            if (FileDownloaded != null)
                FileDownloaded.Invoke();
        }

        public bool UpdateSoftware()
        {
            try
            {

                AddDebug("Getting Log File:");
                AddDebug(updateFilePath);
                LogFileStruct logfile = GetLogFile();
                if (logfile.Version > 0 && Constants.UintVer != logfile.Version)
                {
                    AddDebug("OK!");
                    AddDebug("Version: " + logfile.Version);
                    statusTextBox.Text = logfile.LogString;
                    AddDebug(logfile.downString);
                    AddDebug("Downloading..");
                    downloadFile(logfile.downString);
                    return true;
                }
                else
                {
                    AddDebug("Update not needed..");
                    if (FileDownloadError != null)
                        FileDownloadError.BeginInvoke(null, null);
                    return false;
                }
            }
            catch(Exception)
            {
                if (FileDownloadError != null)
                    FileDownloadError.BeginInvoke(null, null);
                return false;
            }
        }
    }

    public struct LogFileStruct
    {
        public uint Version;
        public string LogString;
        public string downString;
    };
}
