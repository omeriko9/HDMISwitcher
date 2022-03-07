using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;

namespace MonitorSwitcher
{
    public partial class Form1 : Form
    {

        MQTTListener Listener = null;
        const string AppName = "Monitor Switcher";

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == this.WindowState)
            {
                notifyIcon1.Visible = true;
                // notifyIcon1.ShowBalloonTip(500, "test", "test", ToolTipIcon.Info);
                this.ShowInTaskbar = false;
                this.Hide();
            }

            else if (FormWindowState.Normal == this.WindowState)
            {
                this.ShowInTaskbar = true;
               // notifyIcon1.Visible = false;
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            this.BringToFront();
            this.Focus();
            this.WindowState = FormWindowState.Normal;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
            chkStartUp.Checked = IsStartupSetInReg();

            Listener = new MQTTListener();
            int id = 0;     // The id of the hotkey. 
            PInvoke.RegisterHotKey(this.Handle);
            Listener.StartListening();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Listener.StopListening();
            PInvoke.UnregisterHotKey(this.Handle);
        }

        DateTime LastPressed = DateTime.Now;
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == 0x0312)
            {
                if (LastPressed.AddSeconds(1) > DateTime.Now)
                {
                    Listener.SwitchTo();
                }

                LastPressed = DateTime.Now;
            }
        }


        private bool IsStartupSetInReg()
        {
            var val = Registry.CurrentUser.OpenSubKey
                ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", false).GetValue(AppName);
            return val != null;
        }

        private void chkStartUp_CheckedChanged(object sender, EventArgs e)
        {


            RegistryKey rk = Registry.CurrentUser.OpenSubKey
                ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (chkStartUp.Checked)
                rk.SetValue(AppName, Application.ExecutablePath);
            else
                rk.DeleteValue(AppName, false);
        }

        private void ToolStripMenuItem1_Click(object sender, System.EventArgs e)
        {
           
            this.Close();
        }
    }
}