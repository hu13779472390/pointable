using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LeapProject;

namespace Pointable
{
    public partial class FormFullScreen : Form
    {
        Form1 mainform;

        public FormFullScreen(Form1 form1)
        {
            InitializeComponent();

            mainform = form1;
            try
            {
                foreach (Control c in this.Controls)
                {
                    if (c is MdiClient)
                        c.BackColor = Color.White;
                }
            }
            catch { }
        }

        private void FormFullScreen_Activated(object sender, EventArgs e)
        {
            if (mainform.formTutorial != null && !mainform.IsDisposed)
            {
                mainform.formTutorial.checkTestWindowOnTop();
            }
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
                currentOpacity += 0.1;

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
