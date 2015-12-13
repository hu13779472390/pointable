using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PointableUI;
using System.Threading;

namespace LeapProject
{
    public partial class FormCalibrateScreenInfo : Form
    {

        public enum CalibrationStates { None, First, Second, Calibrated };
        public enum CalibrationEvents { Start, Next, Cancel };

        protected override void WndProc(ref Message message)
        {
            const int WM_SYSCOMMAND = 0x0112;
            const int SC_MOVE = 0xF010;

            switch(message.Msg)
            {
                case WM_SYSCOMMAND:
                   int command = message.WParam.ToInt32() & 0xfff0;
                   if (command == SC_MOVE)
                      return;
                   break;
            }

            base.WndProc(ref message);
        }

        FormCalibrateScreen parentForm;
        public FormCalibrateScreenInfo(FormCalibrateScreen _parentForm)
        {
            parentForm = _parentForm;
            InitializeComponent();
        }
 
        private void FormCalibrate_Load(object sender, EventArgs e)
        {
        }

        private void FormCalibrate_Click(object sender, EventArgs e)
        {

        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            try
            {

                this.Close();
            }
            catch { }
        }

        private void FormCalibrate_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                parentForm.infoClose();
            }
            catch { }
        }



        internal void ShowSlow()
        {
            try
            {
                this.Opacity = 0;
                timerSlow.Start();
                this.Show();
            }
            catch { }
        }

        private void timerSlow_Tick(object sender, EventArgs e)
        {
            try
            {
                double currentOpacity = this.Opacity;
                currentOpacity += 0.05;

                if (currentOpacity > 1)
                {
                    currentOpacity = 1;
                    timerSlow.Stop();
                }
                this.Opacity = currentOpacity;
            }
            catch { }
        }


    }
}
