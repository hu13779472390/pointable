using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LeapProject
{
    partial class FormSplash : PerPixelAlphaForm
    {
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                // turn on WS_EX_TOOLWINDOW style bit
                cp.ExStyle |= 0x08000000; // 0x80;
                return cp;
            }
        }

        Bitmap bmpSplash;
        static int count = 0;
        static double opacity = 255;

        public FormSplash()
        {
            InitializeComponent();

            try
            {
                this.TopMost = true;
                timer1.Tick += new EventHandler(timer1_Tick);
                timer1.Interval = 16;
                timer1.Enabled = true;
                timer1.Start();
            }
            catch { }
            Bitmap newBitmap;

            try
            {
                bmpSplash = new Bitmap(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("Pointable.Resources.pointable_logo_splash_screen.png"));//Image.FromFile(file) as Bitmap; //Form1.iconFolderPath + "Pointables_spotlight.png"


                if (bmpSplash != null)
                {
                    this.Left = Screen.AllScreens[0].Bounds.Left + Screen.AllScreens[0].Bounds.Width / 2 - bmpSplash.Width / 2;
                    this.Top = Screen.AllScreens[0].Bounds.Top + Screen.AllScreens[0].Bounds.Height / 2 - bmpSplash.Height / 2;
                    SetBitmapWithOpacity(bmpSplash, (byte)opacity);
                }
            }
            catch (ApplicationException e)
            {
               // MessageBox.Show(this, e.Message, "Error with bitmap.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            catch (Exception e)
            {
               // MessageBox.Show(this, e.Message, "Could not open image file.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                count += 1;
                if (count > 90)
                {
                    //this.Opacity -= 0.034; // Change to zero in 1 second
                    opacity -= 2.125;

                    if (opacity <= 0)
                    {
                        this.Close();
                        return;
                    }

                    if (bmpSplash != null)
                    {
                        SetBitmapWithOpacity(bmpSplash, (byte)opacity);
                    }

                }
            }
            catch { }
        }
    }
}
