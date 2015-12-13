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
    public partial class FormCalibrateScreen : Form
    {
        Form1 mainForm;
        PointableObject currentObject;
        CalibrationStates currentCalibrationState;

        Vector3 line1A, line1B;
        Vector3 line2A, line2B;
        Vector3 objectCoordinate;

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


        public FormCalibrateScreen(Form1 mForm, PointableObject calibratePointable)
        {
            currentObject = calibratePointable;
            mainForm = mForm;
            InitializeComponent();
            //this.Text = "Point at the center";// +currentObject.description;
            //pictureBoxIcon.Left = this.Width / 2 - pictureBoxIcon.Width / 2;
            //pictureBoxIcon.Top = this.Height / 2 - pictureBoxIcon.Height / 2;

            try
            {
                if (Screen.AllScreens.Length > 1)
                    buttonSwitchScreen.Enabled = true;
                else
                    buttonSwitchScreen.Enabled = false;

                calibrateFirst();
            }
            catch { }
            //try
            //{
            //    if (calibratePointable.iconFromResource)
            //        pictureBoxIcon.Image = new Bitmap(Form1.executable.GetManifestResourceStream("Pointable.Resources." + calibratePointable.iconPath));
            //    else
            //        pictureBoxIcon.Image = Image.FromFile(Form1.iconFolderPath + calibratePointable.iconPath) as Bitmap;
            //}
            //catch { }
        }



        //internal void calibrationData(Vector3 fingertip, Vector3 fingertipOut)
        //{
        //    if (currentCalibrationState == CalibrationStates.First)
        //    {
        //        line1A = new Vector3(fingertip);
        //        line1B = new Vector3(fingertipOut);
        
        //        calibrateSecond();
        //    }
        //    else if(currentCalibrationState == CalibrationStates.Second)  
        //    {
        //        if (Vector3.computeDistance(fingertip, line1A) > 80) //8cm away
        //        {
        //            line2A = new Vector3(fingertip);
        //            line2B = new Vector3(fingertipOut);
        //            calibrateDone();
        //        }
        //    }
        //}
        internal void calibrateFirst()
        {
            currentCalibrationState = CalibrationStates.First;

            try
            {
                //reposition form
                if (this.WindowState == FormWindowState.Maximized)
                    this.WindowState = FormWindowState.Normal;

                this.Left = -this.Width / 2 + System.Windows.Forms.Screen.AllScreens[Tools.ScreenNumber].Bounds.X + System.Windows.Forms.Screen.AllScreens[Tools.ScreenNumber].Bounds.Width / 2;
                this.Top = -this.Height / 2 + System.Windows.Forms.Screen.AllScreens[Tools.ScreenNumber].Bounds.Y + System.Windows.Forms.Screen.AllScreens[Tools.ScreenNumber].Bounds.Height / 2;
            }
            catch { }

            try
            {
                timerAlternate.Enabled = false;
                buttonRestart.Enabled = false;
                buttonRestart.Visible = false;

                buttonOK.Enabled = true;
                buttonOK.Visible = true;
                buttonCancel.Visible = true;
                buttonCancel.Enabled = true;
                buttonInteractionZone.Visible = false;


                this.label1.Text = "Select screen size" + System.Environment.NewLine;// +System.Environment.NewLine;
                label1.Visible = false;
                pictureBoxScreenSize.Visible = true;
                pictureBoxScreenPoint.Visible = false;
                pictureBoxScreenDone.Visible = false;


                pictureBox1.Visible = false;
                pictureBoxDone.Visible = false;
                pictureBoxScreenDesktop.Visible = true;
                pictureBoxScreenLaptop.Visible = true;
                pictureBoxScreenTV.Visible = true;

                macTrackBar1.Value = Tools.ScreenSize;
                macTrackBar1.Visible = true;
                labelValues.Visible = true;
                labelValueShow();
            }
            catch { }

            try
            {
                Form1.detectionActivated = false;
                mainForm.BeginInvoke((MethodInvoker)delegate()
                {
                    try
                    {                        
                        mainForm.state.ProcessEvent(Form1.FiniteStateMachine.Events.EndGesture);
                        Tools.changeCursorWindowsNormal();
                    }
                    catch { }
                });
            }
            catch { }
        }

        internal void calibrateSecond()
        {
            currentCalibrationState = CalibrationStates.Second;

            try
            {
                //reposition form
                if (this.WindowState == FormWindowState.Maximized)
                    this.WindowState = FormWindowState.Normal;

                this.Left = -this.Width / 2 + System.Windows.Forms.Screen.AllScreens[Tools.ScreenNumber].Bounds.X + System.Windows.Forms.Screen.AllScreens[Tools.ScreenNumber].Bounds.Width / 2;
                this.Top = -this.Height / 2 + System.Windows.Forms.Screen.AllScreens[Tools.ScreenNumber].Bounds.Y + System.Windows.Forms.Screen.AllScreens[Tools.ScreenNumber].Bounds.Height / 2;
            }
            catch { }

            try
            {
                timerAlternate.Enabled = false;
                buttonRestart.Enabled = false;
                buttonRestart.Visible = false;

                buttonOK.Enabled = false;
                buttonOK.Visible = false;
                buttonCancel.Visible = true;
                buttonCancel.Enabled = true;
                buttonInteractionZone.Visible = true;


                this.label1.Text = "Point here" + System.Environment.NewLine + "and" + System.Environment.NewLine + "stay still";
                label1.Visible = false;
                pictureBoxScreenSize.Visible = false;
                pictureBoxScreenPoint.Visible = true;
                pictureBoxScreenDone.Visible = false;

                pictureBox1.Visible = false;
                pictureBoxDone.Visible = false;
                
                pictureBoxScreenDesktop.Visible = false;
                pictureBoxScreenLaptop.Visible = false;
                pictureBoxScreenTV.Visible = false;
                macTrackBar1.Visible = false;
                labelValues.Visible = false;
                labelValueHide();
            }
            catch { }

           // Thread th = new Thread
            //startCalibrationDelay();

            Form1.detectionActivated = true;
            try
            {
                mainForm.state.ProcessEvent(Form1.FiniteStateMachine.Events.EndGesture);
                mainForm.state.ProcessEvent(Form1.FiniteStateMachine.Events.StartCalibration);
            }
            catch { }
            //Thread th = new Thread(new ThreadStart(startCalibrationDelay));
            //th.IsBackground = true;
            //th.Start();
        }

        internal void calibrateNextState()
        {            
            mainForm.state.ProcessEvent(Form1.FiniteStateMachine.Events.EndCalibration);
           // if (currentCalibrationState == CalibrationStates.Calibrated)
            {
                calibrateDone();
            }
        }

        //internal void calibrateSecond()
        //{
        //    currentCalibrationState = CalibrationStates.Second;

        //    try
        //    {
        //        buttonRestart.Enabled = true;
        //        buttonOK.Enabled = false;

        //        this.label1.Text = "Move your hand to more than 10cm away, and point at the same object again";

        //        pictureBoxIcon.Visible = true;
        //        pictureBox1.Visible = false;
        //        pictureBoxDone.Visible = false;

        //        timerAlternate.Start();
        //    }

        //    catch { }
        //}

        internal void calibrateDone()
        {
            currentCalibrationState = CalibrationStates.Calibrated;
            //mainForm.state.ProcessEvent(Form1.FiniteStateMachine.Events.EndCalibration);

            //objectCoordinate = Vector3.computeIntersect(line1A, line1B, line2A, line2B);      //TODO recalibrate if lines far apart

            try
            {
                timerAlternate.Enabled = false;

                buttonRestart.Enabled = true;
                buttonRestart.Visible = true;
                buttonOK.Visible = true;
                buttonOK.Enabled = true;
                buttonCancel.Visible = false;
                buttonCancel.Enabled = false;
                buttonInteractionZone.Visible = false;

                this.label1.Text = "If the cursor does not appear where you are pointing, click on Restart Calibration" ;
                label1.Visible = false;
                pictureBoxScreenSize.Visible = false;
                pictureBoxScreenPoint.Visible = false;
                pictureBoxScreenDone.Visible = true;

                //pictureBoxIcon.Visible = false;
                pictureBox1.Visible = false;
                pictureBoxDone.Visible = true;
                
                pictureBoxScreenDesktop.Visible = false;
                pictureBoxScreenLaptop.Visible = false;
                pictureBoxScreenTV.Visible = false;
                macTrackBar1.Visible = false;
                labelValues.Visible = false;
                labelValueHide();

                buttonOK.Focus();
            }
            catch { }
        }
        public void labelValueHide()
        {
            labelValue11.Visible = false;
            labelValue14.Visible = false;
            labelValue17.Visible = false;
            labelValue20.Visible = false;
            labelValue23.Visible = false;
            labelValue26.Visible = false;
            labelValue29.Visible = false;
            labelValue32.Visible = false;
            labelValue35.Visible = false;
            labelValue38.Visible = false;
            labelValue41.Visible = false;
            labelValue44.Visible = false;
            
        }

        public void labelValueShow()
        {
            labelValue11.Visible = true;
            labelValue14.Visible = true;
            labelValue17.Visible = true;
            labelValue20.Visible = true;
            labelValue23.Visible = true;
            labelValue26.Visible = true;
            labelValue29.Visible = true;
            labelValue32.Visible = true;
            labelValue35.Visible = true;
            labelValue38.Visible = true;
            labelValue41.Visible = true;
            labelValue44.Visible = true;
        }

        private void FormCalibrate_Load(object sender, EventArgs e)
        {
        }

        private void FormCalibrate_Click(object sender, EventArgs e)
        {

        }

        internal void cancelCalibration()
        {
            try
            {
                this.Close();
            }
            catch { }
        }
        internal void saveCalibration()
        {
            //currentObject.calibrated = true;
            //currentObject.position = objectCoordinate;
            //mainForm.saveCalibration(currentObject);
            //save calibration;
            try
            {
                this.Close();
            }
            catch { }
        }
        private void buttonOKCancel_Click(object sender, EventArgs e)
        {
            cancelCalibration();
        }

        private void buttonRestart_Click(object sender, EventArgs e)
        {
            //calibrateFirst();
            timerStartFirst.Enabled = true;
        }



        private void buttonOK_Click(object sender, EventArgs e)
        {
            try
            {
                Tools.ScreenSize = macTrackBar1.Value;
                //saveCalibration();
                if (currentCalibrationState == CalibrationStates.First)
                {
                    if (mainForm.tutorialInProgress && mainForm.firstRun)
                        openCalibrateScreenForm();
                    else
                        calibrateSecond();
                }

                if (currentCalibrationState == CalibrationStates.Calibrated)
                    this.Close();
            }
            catch { }
        }


        internal void infoClose()
        {
            if (currentCalibrationState == CalibrationStates.First)
            {
               calibrateSecond();
               this.Show();
            }
            else if (currentCalibrationState == CalibrationStates.Second)
            {
                this.Show();
            }
        }

        FormCalibrateScreenInfo formCalibrateScreenInfo;
        private void openCalibrateScreenForm()
        {
            try
            {
                if (formCalibrateScreenInfo != null && !formCalibrateScreenInfo.IsDisposed)
                {
                    formCalibrateScreenInfo.Close();
                }

                formCalibrateScreenInfo = new FormCalibrateScreenInfo(this);
                formCalibrateScreenInfo.Left = this.Left;
                formCalibrateScreenInfo.Top = this.Top;
                formCalibrateScreenInfo.Show();
                formCalibrateScreenInfo.TopMost = true;
                this.Hide();
            }
            catch { }
        }

        private void FormCalibrate_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                Form1.detectionActivated = true;
                //mainForm.calibrationScreenClosing();
                mainForm.endCalibration();
                mainForm.state.ProcessEvent(Form1.FiniteStateMachine.Events.EndCalibration);
            }
            catch { }
        }

        int i = 0;

        private void buttonSwitchScreen_Click(object sender, EventArgs e)
        {
            try
            {
                mainForm.switchCalibratedScreen();
            }
            catch { }
        }
        //private void timerAlternate_Tick(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (i == 3)
        //        {
        //            this.SuspendLayout();
        //            pictureBox2.Visible = true;
        //            pictureBox3.Visible = false;
        //            this.ResumeLayout();

        //            i = 0;
        //        }
        //        else if (i == 1)
        //        {
        //            this.SuspendLayout();
        //            pictureBox3.Visible = true;
        //            pictureBox2.Visible = false;
        //            this.ResumeLayout();
        //        }
        //        i++;
        //    }
        //    catch { }
        //}


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

        private void pictureBoxScreenLaptop_Click(object sender, EventArgs e)
        {
            try
            {
                macTrackBar1.Value = 14;
            }
            catch { }
        }

        private void pictureBoxScreenDesktop_Click(object sender, EventArgs e)
        {
            try
            {
                macTrackBar1.Value = 23;
            }
            catch { }

        }

        private void pictureBoxScreenTV_Click(object sender, EventArgs e)
        {
            try
            {
                macTrackBar1.Value = 41;
            }
            catch { }
        }

        private void macTrackBar1_ValueChanged(object sender, EventArgs e)//, decimal value)
        {
            try
            {
                labelValues.Text = macTrackBar1.Value.ToString() + " inch";
                labelValues.Left = (macTrackBar1.Value - macTrackBar1.Minimum) *
                    (macTrackBar1.Width - 10) / (macTrackBar1.Maximum - macTrackBar1.Minimum) +
                    macTrackBar1.Left + 10 - labelValues.Width / 2;
            }
            catch { }
        }

        private void macTrackBar1_MouseDown(object sender, MouseEventArgs e)
        {
            macTrackBar1.Value = (int)Math.Round(e.X*(macTrackBar1.Maximum - macTrackBar1.Minimum)/(double)macTrackBar1.Width + macTrackBar1.Minimum);
        }

        private void labelValue11_Click(object sender, EventArgs e)
        {
            macTrackBar1.Value = 11;
        }

        private void labelValue14_Click(object sender, EventArgs e)
        {
            macTrackBar1.Value = 14;

        }

        private void labelValue17_Click(object sender, EventArgs e)
        {
            macTrackBar1.Value = 17;

        }

        private void labelValue20_Click(object sender, EventArgs e)
        {
            macTrackBar1.Value = 20;

        }

        private void labelValue23_Click(object sender, EventArgs e)
        {
            macTrackBar1.Value = 23;

        }

        private void labelValue26_Click(object sender, EventArgs e)
        {
            macTrackBar1.Value = 26;
        }

        private void labelValue29_Click(object sender, EventArgs e)
        {
            macTrackBar1.Value = 29;
        }

        private void labelValue32_Click(object sender, EventArgs e)
        {
            macTrackBar1.Value = 32;
        }

        private void labelValue35_Click(object sender, EventArgs e)
        {
            macTrackBar1.Value = 35;
        }

        private void labelValue38_Click(object sender, EventArgs e)
        {
            macTrackBar1.Value = 38;
        }

        private void labelValue41_Click(object sender, EventArgs e)
        {
            macTrackBar1.Value = 41;
        }

        private void labelValue44_Click(object sender, EventArgs e)
        {
            macTrackBar1.Value = 44;
        }

        private void buttonInteractionZone_Click(object sender, EventArgs e)
        {
            openCalibrateScreenForm();
        }

        private void timerStartFirst_Tick(object sender, EventArgs e)
        {

            Form1.detectionActivated = false;
            timerStartFirst.Enabled = false;
            calibrateFirst();
        }

    }
}
