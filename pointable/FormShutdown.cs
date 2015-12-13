using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using LeapProject;

namespace Pointable
{
    public partial class FormShutdown : Form
    {
        Form1 mainForm;
        bool shuttingDown = false;
        int counter = 10;

        public FormShutdown(Form1 mainForm1, bool isShutdown)
        {
            InitializeComponent();
            mainForm = mainForm1;
            shuttingDown = isShutdown;

            try
            {
                if (isShutdown)
                {
                    this.Text = "System Shutdown";
                    label1.Text = "Shutdown in";
                    buttonNow.Text ="Shutdown now";
                }
                else
                {
                    this.Text = "System Restart";
                    label1.Text = "Restart in";
                    buttonNow.Text = "Restart now";
                }
                counter = 10;
            }
            catch { }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            try
            {
                timer1.Enabled = false;
                this.Close();
            }
            catch { }
        }

        private void buttonNow_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            if (shuttingDown)
            {
                label1.Text = "System shutdown in progress";
                labelCounter.Text = "";
                runShutdown();
            }
            else
            {
                label1.Text = "System restart in progress";
                labelCounter.Text = "";
                runRestart();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                counter--;
                if (counter > 0)
                {
                    labelCounter.Text = counter.ToString() + " seconds";
                }
                else
                {
                    timer1.Enabled = false;
                    if (shuttingDown)
                    {
                        label1.Text = "System shutdown in progress";
                        labelCounter.Text = "";
                        runShutdown();
                    }
                    else
                    {
                        label1.Text = "System restart in progress";
                        labelCounter.Text = "";
                        runRestart();
                    }
                }
            }
            catch { }
        }

        private void runShutdown()
        {
            //MessageBox.Show("ShutDown"); return;
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = "shutdown.exe";// triggerFilename;//"C:\\Program Files (x86)\\EventGhost\\EventGhost.exe";
                startInfo.Arguments = "-s -t 00";// "-event NextChannel";
                Process.Start(startInfo);
            }
            catch { };//(Exception error){Debug.WriteLine(error.ToString()); }
        }
        private void runRestart()
        {
            //MessageBox.Show("restart"); return;
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = "shutdown.exe";// triggerFilename;//"C:\\Program Files (x86)\\EventGhost\\EventGhost.exe";
                startInfo.Arguments = "-r -t 00";// "-event NextChannel";
                Process.Start(startInfo);
            }
            catch { };//(Exception error){Debug.WriteLine(error.ToString()); }
        }
    }
}
