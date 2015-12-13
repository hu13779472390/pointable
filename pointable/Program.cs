using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;

namespace LeapProject
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
          //  Application.Run(new Form1());
           // new Form1();
            // get the name of our process
            string proc = Process.GetCurrentProcess().ProcessName;
            // get the list of all processes by that name
            Process[] processes = Process.GetProcessesByName(proc);
            // if there is more than one process...
//#if !DEBUG

            if (processes.Length > 1)//(false)//
            {
                // MessageBox.Show(args[0]);
                if (args != null && args.Length > 0 && args[0] == "-multiple")
                {//run command line with -multiple, smoothboard.exe -multiple to allow multiple instance, //choose wiimote
                    Form1.showSplash = false;
                    Application.Run(new Form1());
                }
                else
                {
                    MessageBox.Show("Application is already running", "Pointable");//, WiimoteWhiteboard.Properties.Resources.Smoothboard, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
            }
            else
//#endif
            {
                if (args != null && args.Length > 0 && args[0] == "-multiple")
                {
                    Form1.showSplash = false;
                    Application.Run(new Form1());
                }
                else
                {
                    Application.Run(new Form1());
                }
            }
            
        }
    }
}
