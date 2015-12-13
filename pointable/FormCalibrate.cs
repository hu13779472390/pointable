using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PointableUI;

namespace LeapProject
{
    public partial class FormCalibrate : Form
    {
        Form1 mainForm;
        PointableObject currentObject;
        CalibrationStates currentCalibrationState;

        Vector3 line1A, line1B;
        Vector3 line2A, line2B;
        Vector3 objectCoordinate;

        public enum CalibrationStates { None, First, Second, Calibrated };
        public enum CalibrationEvents { Start, Next, Cancel };


        public FormCalibrate(Form1 mForm, PointableObject calibratePointable)
        {
            currentObject = calibratePointable;
            mainForm = mForm;
            InitializeComponent();

            try
            {
                this.Text = "Calibrate Pointable - " + currentObject.description;
                calibrateFirst();
            }
            catch { }

            try
            {
                if (calibratePointable.iconFromResource)
                    pictureBoxIcon.Image = new Bitmap(Form1.executable.GetManifestResourceStream("Pointable.Resources." + calibratePointable.iconPath));
                else
                    pictureBoxIcon.Image = Image.FromFile(Form1.iconFolderPath + calibratePointable.iconPath) as Bitmap;
            }
            catch { }

        }

        internal void calibrationData(Vector3 fingertip, Vector3 fingertipOut)
        {
            try
            {
                if (currentCalibrationState == CalibrationStates.First)
                {
                    line1A = new Vector3(fingertip);
                    line1B = new Vector3(fingertipOut);


                    calibrateDone();
                   // calibrateSecond();
                }
                else if (currentCalibrationState == CalibrationStates.Second)
                {
                    if (Vector3.computeDistance(fingertip, line1A) > 80) //8cm away
                    {
                        line2A = new Vector3(fingertip);
                        line2B = new Vector3(fingertipOut);
                        calibrateDone();
                    }
                }
            }
            catch { }
        }

        internal void calibrateFirst()
        {
            currentCalibrationState = CalibrationStates.First;

            try
            {
                mainForm.state.ProcessEvent(Form1.FiniteStateMachine.Events.EndGesture);
                mainForm.state.ProcessEvent(Form1.FiniteStateMachine.Events.StartCalibration);
            }
            catch { }

            try
            {
                timerAlternate.Enabled = false;
                buttonRestart.Enabled = false;
                buttonOK.Enabled = false;
                timerEndCalibration.Stop();

                this.label1.Text = "Point at the center of " + currentObject.description + " (real object or area outside your screen) with only one finger";
                this.label1.Text = this.label1.Text + System.Environment.NewLine + System.Environment.NewLine + "Info: Do not set area within screen as a Pointable";
                //Point at the center of Speakers (real object or area outside your screen) with only one finger


                pictureBoxIcon.Visible = true;
                pictureBox1.Visible = true;
                pictureBox2.Visible = false;
                pictureBox3.Visible = false;
                pictureBoxDone.Visible = false;
            }
            catch { }
        }

        internal void calibrateSecond()
        {
            currentCalibrationState = CalibrationStates.Second;

            try
            {
                buttonRestart.Enabled = true;
                buttonOK.Enabled = false;
                timerEndCalibration.Stop();

                this.label1.Text = "Move your hand to more than 10cm away, and point at the same object or area again";

                pictureBoxIcon.Visible = true;
                pictureBox1.Visible = false;
                pictureBox2.Visible = true;
                pictureBox3.Visible = false;
                pictureBoxDone.Visible = false;

                timerAlternate.Start();
            }

            catch { }
        }
        
        internal void calibrateDone()
        {
            currentCalibrationState = CalibrationStates.Calibrated;
            //mainForm.state.ProcessEvent(Form1.FiniteStateMachine.Events.EndCalibration);

            //objectCoordinate = Vector3.computeIntersect(line1A, line1B, line2A, line2B);      //TODO recalibrate if lines far apart

            objectCoordinate = Vector3.intersectLineScreen(line1A, line1B, mainForm.screenNormal, mainForm.screenCorner[0]);

            if (objectCoordinate.Y < 0)
            {
                objectCoordinate = Vector3.intersectLineScreen(line1A, line1B, new Vector3(0, 1, 0), new Vector3(0, 0, 0));
            }


            try
            {
                timerAlternate.Enabled = false;
                timerEndCalibration.Start();

                buttonRestart.Enabled = true;
                buttonOK.Enabled = true;

                this.label1.Text = currentObject.description + " calibrated";

                //pictureBoxIcon.Visible = false;
                pictureBox1.Visible = false;
                pictureBox2.Visible = false;
                pictureBox3.Visible = false;
                pictureBoxDone.Visible = true;

                buttonOK.Focus();
            }
            catch { }
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
            try
            {
                currentObject.calibrated = true;
                currentObject.position = objectCoordinate;
                mainForm.saveCalibration(currentObject);
                //save calibration;
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
            calibrateFirst();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            saveCalibration();
        }

        private void FormCalibrate_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                mainForm.endCalibration();
                mainForm.state.ProcessEvent(Form1.FiniteStateMachine.Events.EndCalibration);
            }
            catch { }
        }

        int i = 0;
        private void timerAlternate_Tick(object sender, EventArgs e)
        {
            try
            {
                if (i == 3)
                {
                    this.SuspendLayout();
                    pictureBox2.Visible = true;
                    pictureBox3.Visible = false;
                    this.ResumeLayout();

                    i = 0;
                }
                else if (i == 1)
                {
                    this.SuspendLayout();
                    pictureBox3.Visible = true;
                    pictureBox2.Visible = false;
                    this.ResumeLayout();
                }
                i++;
            }
            catch { }
        }

        private void timerEndCalibration_Tick(object sender, EventArgs e)
        {
            try
            {
                timerEndCalibration.Stop();
                mainForm.state.ProcessEvent(Form1.FiniteStateMachine.Events.EndCalibration);
            }
            catch { }
        }
    }
}
