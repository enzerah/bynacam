using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace BynaCam_Recorder
{
    public static class Messages
    {
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        public static DialogResult Error(string msg)
        {
            return MessageBox.Show(new WindowWrapper(GetForegroundWindow()), msg, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        public static DialogResult Info(string msg)
        {
            return MessageBox.Show(new WindowWrapper(GetForegroundWindow()), msg, "Information!", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        public static DialogResult OKCancel(string msg, string caption, MessageBoxButtons btn)
        {
            return MessageBox.Show(new WindowWrapper(GetForegroundWindow()), msg, caption, btn);
        }
    }

    public class WindowWrapper : IWin32Window
    {
        public WindowWrapper(IntPtr handle)
        {
            _hwnd = handle;
        }

        public IntPtr Handle
        {
            get { return _hwnd; }
        }

        private IntPtr _hwnd;
    }
}
