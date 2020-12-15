using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GoogleDriveSyncBlock
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        bool GDrive_isRunning = false;
        RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\GoogleDriveBlock");

        private void Form1_Load(object sender, EventArgs e)
        {
            KillGDrive();

            //Test if registry value exists
            if (key.GetValue("Path") == null)
            {
                //Set Standart Path
                key.SetValue("Path", @"C:\Program Files\Google\Drive\googledrivesync.exe");
            }

            PathTextBox.Text = (string)key.GetValue("Path");

            MainTimer.Start();

            notifyIcon1.Icon = this.Icon;
            notifyIcon1.Visible = true;
            notifyIcon1.Text = this.Text;
        }

        private void MainTimer_Tick(object sender, EventArgs e)
        {
            

            StatusLabel.Text = "Google Drive Sync is running:" + GDrive_isRunning.ToString();


            //Check if Visual Studio is running
            Process[] pname = Process.GetProcessesByName("devenv");
            if (pname.Length == 0)
            {
                if (GDrive_isRunning == false)
                {
                    GDrive_isRunning = true;
                    StartGDrive();
                }
            }
            else
            {
                GDrive_isRunning = false;
                KillGDrive();
            }
        }

        void KillGDrive()
        {
            try
            {
                foreach (Process proc in Process.GetProcessesByName("googledrivesync"))
                {
                    proc.Kill();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        void StartGDrive()
        {
            try
            {
                string filePath = @"C:\Program Files\Google\Drive\googledrivesync.exe";
                Process.Start(filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void SaveFilePathButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "(*.exe)|*.exe";
            ofd.ShowDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                PathTextBox.Text = ofd.FileName;
                //Save Path in Registry
                key.SetValue("Path", ofd.FileName);
            }
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            ShowInTaskbar = true;
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized && ShowInTaskbar == true)
            {
                ShowInTaskbar = false;
            }
        }
    }
}
