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
    public partial class FormConfiguration : Form
    {
        Form1 mainForm;
        public FormConfiguration(Form1 mainForm1)
        {
            InitializeComponent();
            mainForm = mainForm1;

            loadConfiguration();
        }

        private void loadConfiguration()
        {
            try
            {
                checkBoxLeftClick.Checked = Tools.modeClickLeft;
                checkBoxRightClick.Checked = Tools.modeClickRight;
                checkBoxMiddleClick.Checked = Tools.modeClickMiddle;
                checkBoxClickAndDrag.Checked = Tools.modeClickAndDrag;


                checkBoxModifierControl.Checked = Tools.keyModifierModeControl;
                checkBoxModifierAlt.Checked = Tools.keyModifierModeAlt;
                checkBoxModifierShift.Checked = Tools.keyModifierModeShift;

                checkBoxModifierPointableTracking.Checked = Tools.keyModifierModeEnableTracking;

                checkBoxWindowDrag.Checked = Tools.modeWindowDrag;

                trackBarClickSensitivity.Value = Tools.TapSensitivity;
                trackBarClickRightSensitivity.Value = Tools.TapRightSensitivity;
                trackBarClickMiddleSensitivity.Value = Tools.TapLeftSensitivity;

                trackBarStabilization.Value = Tools.CursorStabilization;
                trackBarCursorDroplet.Value = Tools.CursorDroplet;
            }
            catch { }
        }


        private void checkBoxLeftClick_CheckedChanged(object sender, EventArgs e)
        {
            Tools.modeClickLeft = checkBoxLeftClick.Checked;
        }

        private void checkBoxRightClick_CheckedChanged(object sender, EventArgs e)
        {
            Tools.modeClickRight = checkBoxRightClick.Checked;
        }

        private void checkBoxMiddleClick_CheckedChanged(object sender, EventArgs e)
        {
            Tools.modeClickMiddle = checkBoxMiddleClick.Checked;
        }

        private void checkBoxClickAndDrag_CheckedChanged(object sender, EventArgs e)
        {
            Tools.modeClickAndDrag = checkBoxClickAndDrag.Checked;
        }

        private void checkBoxModifierControl_CheckedChanged(object sender, EventArgs e)
        {
            Tools.keyModifierModeControl = checkBoxModifierControl.Checked;
        }

        private void checkBoxModifierAlt_CheckedChanged(object sender, EventArgs e)
        {
            Tools.keyModifierModeAlt = checkBoxModifierAlt.Checked;
        }

        private void checkBoxModifierShift_CheckedChanged(object sender, EventArgs e)
        {
            Tools.keyModifierModeShift = checkBoxModifierShift.Checked;
        }

        private void checkBoxModifierPointableTracking_CheckedChanged(object sender, EventArgs e)
        {
            Tools.keyModifierModeEnableTracking = checkBoxModifierPointableTracking.Checked;
        }

        private void checkBoxWindowDrag_CheckedChanged(object sender, EventArgs e)
        {
            Tools.modeWindowDrag = checkBoxWindowDrag.Checked;
        }

        private void label4_Click(object sender, EventArgs e)
        {

            try
            {
                //start help
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = @"http://www.pointable.net/guide"; //C:\\Program Files (x86)\\EventGhost\\EventGhost.exe";
                startInfo.Arguments = "";// "-event NextChannel";
                Process.Start(startInfo);

            }
            catch { }
                    
        }

        private void label5_Click(object sender, EventArgs e)
        {

            try
            {
                //start help
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = @"http://www.pointable.net/contact"; //C:\\Program Files (x86)\\EventGhost\\EventGhost.exe";
                startInfo.Arguments = "";// "-event NextChannel";
                Process.Start(startInfo);

            }
            catch { }
        }

        private void trackBarClickSensitivity_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                int value = (int)Math.Round(e.X * (trackBarClickSensitivity.Maximum - trackBarClickSensitivity.Minimum) / (double)trackBarClickSensitivity.Width + trackBarClickSensitivity.Minimum);
                if (value < trackBarClickSensitivity.Minimum)
                    value = trackBarClickSensitivity.Minimum;
                if (value > trackBarClickSensitivity.Maximum)
                    value = trackBarClickSensitivity.Maximum;

                trackBarClickSensitivity.Value = value;
            }
            catch { }
        }

        private void trackBarClickSensitivity_MouseUp(object sender, MouseEventArgs e)
        {

            try
            {
                int value = (int)Math.Round(e.X * (trackBarClickSensitivity.Maximum - trackBarClickSensitivity.Minimum) / (double)trackBarClickSensitivity.Width + trackBarClickSensitivity.Minimum);
                if (value < trackBarClickSensitivity.Minimum)
                    value = trackBarClickSensitivity.Minimum;
                if (value > trackBarClickSensitivity.Maximum)
                    value = trackBarClickSensitivity.Maximum;

                trackBarClickSensitivity.Value = value;
                Tools.TapSensitivity = trackBarClickSensitivity.Value;
                mainForm.setControllerConfiguration();
            }
            catch { }
        }

        private void buttonReset_Click(object sender, EventArgs e)
        {
            try
            {
                checkBoxLeftClick.Checked = true;
                checkBoxRightClick.Checked = true;
                checkBoxMiddleClick.Checked = true;
                checkBoxClickAndDrag.Checked = false;


                checkBoxModifierControl.Checked = true;
                checkBoxModifierAlt.Checked = true;
                checkBoxModifierShift.Checked = true;

                checkBoxModifierPointableTracking.Checked = true;

                checkBoxWindowDrag.Checked = true;


                trackBarClickSensitivity.Value = 3;
                Tools.TapSensitivity = trackBarClickSensitivity.Value;
                mainForm.setControllerConfiguration();


                trackBarClickRightSensitivity.Value = 3;
                Tools.TapRightSensitivity = trackBarClickRightSensitivity.Value;


                trackBarClickMiddleSensitivity.Value = 3;
                Tools.TapLeftSensitivity = trackBarClickMiddleSensitivity.Value;

                trackBarStabilization.Value = 1;
                Tools.CursorStabilization = trackBarStabilization.Value;


                trackBarCursorDroplet.Value = 1;
                Tools.CursorDroplet = trackBarCursorDroplet.Value;
            }
            catch { }
        }

        private void trackBarClickRightSensitivity_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                int value = (int)Math.Round(e.X * (trackBarClickRightSensitivity.Maximum - trackBarClickRightSensitivity.Minimum) /
                    (double)trackBarClickRightSensitivity.Width + trackBarClickRightSensitivity.Minimum);
                if (value < trackBarClickRightSensitivity.Minimum)
                    value = trackBarClickRightSensitivity.Minimum;
                if (value > trackBarClickRightSensitivity.Maximum)
                    value = trackBarClickRightSensitivity.Maximum;

                trackBarClickRightSensitivity.Value = value;
            }
            catch { }
        }

        private void trackBarClickRightSensitivity_MouseUp(object sender, MouseEventArgs e)
        {

            try
            {
                int value = (int)Math.Round(e.X * (trackBarClickRightSensitivity.Maximum - trackBarClickRightSensitivity.Minimum) /
                    (double)trackBarClickRightSensitivity.Width + trackBarClickRightSensitivity.Minimum);
                if (value < trackBarClickRightSensitivity.Minimum)
                    value = trackBarClickRightSensitivity.Minimum;
                if (value > trackBarClickRightSensitivity.Maximum)
                    value = trackBarClickRightSensitivity.Maximum;

                trackBarClickRightSensitivity.Value = value;
                Tools.TapRightSensitivity = trackBarClickRightSensitivity.Value;
            }
            catch { }
        }

        private void trackBarClickMiddleSensitivity_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                int value = (int)Math.Round(e.X * (trackBarClickMiddleSensitivity.Maximum - trackBarClickMiddleSensitivity.Minimum) /
                    (double)trackBarClickMiddleSensitivity.Width + trackBarClickMiddleSensitivity.Minimum);
                if (value < trackBarClickMiddleSensitivity.Minimum)
                    value = trackBarClickMiddleSensitivity.Minimum;
                if (value > trackBarClickMiddleSensitivity.Maximum)
                    value = trackBarClickMiddleSensitivity.Maximum;

                trackBarClickMiddleSensitivity.Value = value;
            }
            catch { }
        }

        private void trackBarClickMiddleSensitivity_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                int value = (int)Math.Round(e.X * (trackBarClickMiddleSensitivity.Maximum - trackBarClickMiddleSensitivity.Minimum) /
                    (double)trackBarClickMiddleSensitivity.Width + trackBarClickMiddleSensitivity.Minimum);
                if (value < trackBarClickMiddleSensitivity.Minimum)
                    value = trackBarClickMiddleSensitivity.Minimum;
                if (value > trackBarClickMiddleSensitivity.Maximum)
                    value = trackBarClickMiddleSensitivity.Maximum;

                trackBarClickMiddleSensitivity.Value = value;
                Tools.TapLeftSensitivity = trackBarClickMiddleSensitivity.Value;
               
            }
            catch { }
        }

        private void trackBarStabilization_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                int value = (int)Math.Round(e.X * (trackBarStabilization.Maximum - trackBarStabilization.Minimum) /
                    (double)trackBarStabilization.Width + trackBarStabilization.Minimum);
                if (value < trackBarStabilization.Minimum)
                    value = trackBarStabilization.Minimum;
                if (value > trackBarStabilization.Maximum)
                    value = trackBarStabilization.Maximum;

                trackBarStabilization.Value = value;
            }
            catch { }
        }

        private void trackBarStabilization_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                int value = (int)Math.Round(e.X * (trackBarStabilization.Maximum - trackBarStabilization.Minimum) /
                    (double)trackBarStabilization.Width + trackBarStabilization.Minimum);
                if (value < trackBarStabilization.Minimum)
                    value = trackBarStabilization.Minimum;
                if (value > trackBarStabilization.Maximum)
                    value = trackBarStabilization.Maximum;

                trackBarStabilization.Value = value;
                Tools.CursorStabilization = trackBarStabilization.Value;

                mainForm.setSmoothingData();
            }
            catch { }
        }

        private void trackBarCursorDroplet_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                int value = (int)Math.Round(e.X * (trackBarCursorDroplet.Maximum - trackBarCursorDroplet.Minimum) /
                    (double)trackBarCursorDroplet.Width + trackBarCursorDroplet.Minimum);
                if (value < trackBarCursorDroplet.Minimum)
                    value = trackBarCursorDroplet.Minimum;
                if (value > trackBarCursorDroplet.Maximum)
                    value = trackBarCursorDroplet.Maximum;

                trackBarCursorDroplet.Value = value;
            }
            catch { }
        }

        private void trackBarCursorDroplet_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                int value = (int)Math.Round(e.X * (trackBarCursorDroplet.Maximum - trackBarCursorDroplet.Minimum) /
                    (double)trackBarCursorDroplet.Width + trackBarCursorDroplet.Minimum);
                if (value < trackBarCursorDroplet.Minimum)
                    value = trackBarCursorDroplet.Minimum;
                if (value > trackBarCursorDroplet.Maximum)
                    value = trackBarCursorDroplet.Maximum;

                trackBarCursorDroplet.Value = value;
                Tools.CursorDroplet = trackBarCursorDroplet.Value;

                mainForm.setCursorDropletData();
            }
            catch { }
        }
    }
}
