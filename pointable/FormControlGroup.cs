using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CoreAudioApi;
using System.Drawing.Drawing2D;
using System.Timers;
using PointableUI;
using System.Windows.Media.Imaging;

namespace LeapProject
{
    partial class FormControlGroup :PerPixelAlphaForm
    {

        private Bitmap bitmapBall;
        private Bitmap bitmapBallCurrentSettings;
        private Bitmap bitmapBallRealTime;
        private Bitmap bitmapBallSpeaker;

        private static Bitmap bitmapBallStatic;
        private static Bitmap bitmapBallSmallStatic;
        private static Bitmap bitmapBallNoEdgeStatic;
        private static Bitmap bitmapBallEdgeStatic;

        private static Bitmap bitmapVolumeBlankStatic;
        private static Bitmap bitmapVolumeFullStatic;

        public FormControlGroup()
        {
            InitializeComponent();

            try
            {
                bitmapBallStatic = new Bitmap(Form1.executable.GetManifestResourceStream("Pointable.Resources.Pointables_ball.png"));
                bitmapBallStatic.MakeTransparent();

                bitmapBallEdgeStatic = new Bitmap(Form1.executable.GetManifestResourceStream("Pointable.Resources.Pointables_ball_edge.png"));
                bitmapBallEdgeStatic.MakeTransparent();

                bitmapBallSmallStatic = new Bitmap(bitmapBallStatic.Width / 1, bitmapBallStatic.Height / 1);
                Graphics g = Graphics.FromImage(bitmapBallSmallStatic);
                g.CompositingMode = CompositingMode.SourceOver;
                g.DrawImage(bitmapBallStatic, resizeRectangle(new Rectangle(0, 0, bitmapBallStatic.Width, bitmapBallStatic.Height)));
                g.Dispose();

                bitmapBallNoEdgeStatic = new Bitmap(Form1.executable.GetManifestResourceStream("Pointable.Resources.Pointables_ball_no_edge.png"));
                bitmapBallNoEdgeStatic.MakeTransparent();


                bitmapVolumeBlankStatic = new Bitmap(Form1.executable.GetManifestResourceStream("Pointable.Resources.sound_volume_blank.png"));
                bitmapVolumeBlankStatic.MakeTransparent();

                bitmapVolumeFullStatic = new Bitmap(Form1.executable.GetManifestResourceStream("Pointable.Resources.sound_volume_full.png"));
                bitmapVolumeFullStatic.MakeTransparent();

            }
            catch { }

            resetUI();

        }

        internal void updatePosition()
        {
            try
            {
                this.Left = -this.Width / 2 + System.Windows.Forms.Screen.AllScreens[Tools.ScreenNumber].Bounds.X + System.Windows.Forms.Screen.AllScreens[Tools.ScreenNumber].Bounds.Width / 2;
                this.Top = -this.Height / 2 + System.Windows.Forms.Screen.AllScreens[Tools.ScreenNumber].Bounds.Y + System.Windows.Forms.Screen.AllScreens[Tools.ScreenNumber].Bounds.Height / 2;
            }
            catch { }
        }
        internal void updatePosition(Point centerPoint)
        {
            try
            {
                this.Left = -this.Width / 2 + centerPoint.X;
                this.Top = -this.Height / 2 + centerPoint.Y;
            }
            catch { }
        }

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
        //// Let Windows drag this form for us
        //protected override void WndProc(ref Message m)
        //{
        //    if (m.Msg == 0x0084 /*WM_NCHITTEST*/)
        //    {
        //        m.Result = (IntPtr)2;	// HTCLIENT
        //        return;
        //    }
        //    base.WndProc(ref m);
        //}

 

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            //Graphics g = e.Graphics;
            //Pen pen = new Pen(Color.FromArgb(255, 0, 0, 0));
            //g.DrawLine(pen, 0, 0, 150, 150);
            //g.DrawLine(pen, 450, 0, 300, 150);
            //g.DrawLine(pen, 0, 450, 150, 300);
            //g.DrawLine(pen, 450, 450, 300, 300);
        }

        private void FormControlGroup_Load(object sender, EventArgs e)
        {

        }

        internal void setUI2(string center, string top, string right, string bottom, string left)
        {
            try
            {
                pictureBoxCenter.Image = Image.FromFile(Form1.iconFolderPath + center);
                pictureBoxTop.Image = Image.FromFile(Form1.iconFolderPath + top);
                pictureBoxRight.Image = Image.FromFile(Form1.iconFolderPath + right);
                pictureBoxBottom.Image = Image.FromFile(Form1.iconFolderPath + bottom);
                pictureBoxLeft.Image = Image.FromFile(Form1.iconFolderPath + left);
            }
            catch { }
        }

        internal Bitmap[] bitmapObjects;

        private void resetBitmapObject()
        {
            try
            {
                if (bitmapObjects != null)
                {
                    foreach (Bitmap bmp in bitmapObjects)
                    {
                        if (bmp != null)
                        {
                            bmp.Dispose();
                            //bmp = null;
                        }
                    }
                }
                bitmapObjects = new Bitmap[5];
            }
            catch { }
        }

        internal Rectangle resizeRectangle(Rectangle rectangle)
        {
            return rectangle;
            return new Rectangle(rectangle.X / 2, rectangle.Y / 2, rectangle.Width / 2, rectangle.Height / 2);
        }
        internal void setUI(bool setWindow, PointableObject pointable)
        {
            setUI(setWindow, pointable.iconPath, pointable.actions[0].iconPath, pointable.actions[1].iconPath, pointable.actions[2].iconPath, pointable.actions[3].iconPath,
                pointable.iconFromResource, pointable.actions[0].iconFromResource, pointable.actions[1].iconFromResource, pointable.actions[2].iconFromResource, pointable.actions[3].iconFromResource, null);
        }

        internal void setUI(bool setWindow, string center, string top, string right, string bottom, string left, 
            bool centerFromResource, bool topFromResource, bool rightFromResource, bool bottomFromResource, bool leftFromResource, Bitmap iconCenter)
        {
            resetBitmapObject();

            bitmapBallCurrentSettings = new Bitmap(bitmapBall.Width, bitmapBall.Height);
            Graphics g = Graphics.FromImage(bitmapBallCurrentSettings);

            g.CompositingMode = CompositingMode.SourceOver;

            //Bitmap smallBmp;

            try
            {
                //smallBmp = new Bitmap(Form1.executable.GetManifestResourceStream("Pointable.Resources.Pointables_ball.png"));//Image.FromFile(file) as Bitmap; //Form1.iconFolderPath + "Pointables_spotlight.png"
                // smallBmp = Image.FromFile(Form1.iconFolderPath + "Pointables_ball.png") as Bitmap; 
                //smallBmp.MakeTransparent();
                //g.DrawImage(bitmapBallStatic, 0, 0, bitmapBall.Width, bitmapBall.Height);
                g.DrawImage(bitmapBallStatic, resizeRectangle(new Rectangle(0, 0, bitmapBall.Width, bitmapBall.Height)));
                //smallBmp.Dispose();
            }
            catch { }
            if (iconCenter != null)
            {
                bitmapObjects[4] = iconCenter;
                bitmapObjects[4].MakeTransparent();
                g.DrawImage(bitmapObjects[4], resizeRectangle(pictureBoxCenter.Bounds));
            }
            else
            {
                if (center != null && center != "")
                {
                    try
                    {
                        // smallBmp =  bitmapObjects[4];
                        if (centerFromResource)
                            bitmapObjects[4] = new Bitmap(Form1.executable.GetManifestResourceStream("Pointable.Resources." + center));
                        else
                            bitmapObjects[4] = Image.FromFile(Form1.iconFolderPath + center) as Bitmap;
                        bitmapObjects[4].MakeTransparent();
                        g.DrawImage(bitmapObjects[4], resizeRectangle(pictureBoxCenter.Bounds));
                    }
                    catch { }
                    //  smallBmp.Dispose();
                }
            }
            if (top != null && top != "")
            {
                try
                {
                    if (centerFromResource)
                        bitmapObjects[0] = new Bitmap(Form1.executable.GetManifestResourceStream("Pointable.Resources." + top));
                    else
                        bitmapObjects[0] = Image.FromFile(Form1.iconFolderPath + top) as Bitmap;
                    bitmapObjects[0].MakeTransparent();
                    g.DrawImage(bitmapObjects[0], resizeRectangle(pictureBoxTop.Bounds));
                }
                catch { }
              //  bitmapObjects[0].Dispose();
            }
            if (right != null && right != "")
            {
                try
                {
                    if (centerFromResource)
                        bitmapObjects[1] = new Bitmap(Form1.executable.GetManifestResourceStream("Pointable.Resources." + right));
                    else
                        bitmapObjects[1] = Image.FromFile(Form1.iconFolderPath + right) as Bitmap;
                    bitmapObjects[1].MakeTransparent();
                    g.DrawImage(bitmapObjects[1], resizeRectangle(pictureBoxRight.Bounds));
                }
                catch { }
               // smallBmp.Dispose();
            }
            if (bottom != null && bottom != "")
            {
                try
                {
                    if (centerFromResource)
                        bitmapObjects[2] = new Bitmap(Form1.executable.GetManifestResourceStream("Pointable.Resources." + bottom));
                    else
                        bitmapObjects[2] = Image.FromFile(Form1.iconFolderPath + bottom) as Bitmap;
                    bitmapObjects[2].MakeTransparent();
                    g.DrawImage(bitmapObjects[2], resizeRectangle(pictureBoxBottom.Bounds));
                }
                catch { }
               // smallBmp.Dispose();
             }
            if (left != null && left != "")
            {
                try
                {
                    if (centerFromResource)
                        bitmapObjects[3] = new Bitmap(Form1.executable.GetManifestResourceStream("Pointable.Resources." + left));
                    else
                        bitmapObjects[3] = Image.FromFile(Form1.iconFolderPath + left) as Bitmap;
                    bitmapObjects[3].MakeTransparent();
                    g.DrawImage(bitmapObjects[3], resizeRectangle(pictureBoxLeft.Bounds));
                }
                catch { }
               // smallBmp.Dispose();
            }

            if (setWindow)
                SetBitmap(bitmapBallCurrentSettings, (byte)255);

            g.Dispose();
        }


        internal void setUISound(bool setWindow, PointableObject pointable)
        {
            setUISound(setWindow, pointable.iconPath, pointable.actions[0].iconPath, pointable.actions[1].iconPath, pointable.actions[2].iconPath, pointable.actions[3].iconPath,
                 pointable.iconFromResource, pointable.actions[0].iconFromResource, pointable.actions[1].iconFromResource, pointable.actions[2].iconFromResource, pointable.actions[3].iconFromResource);
        }
        internal void setUISound(bool setWindow, string center, string top, string right, string bottom, string left, bool centerFromResource, bool topFromResource, bool rightFromResource, bool bottomFromResource, bool leftFromResource)
        {
            try
            {
                resetBitmapObject();
                bitmapBallCurrentSettings = new Bitmap(bitmapBall.Width, bitmapBall.Height);
                Graphics g = Graphics.FromImage(bitmapBallCurrentSettings);

                g.CompositingMode = CompositingMode.SourceOver;

               // Bitmap smallBmp;

                //smallBmp = Image.FromFile(Form1.iconFolderPath+ "Pointables_ball.png") as Bitmap;
               // smallBmp = new Bitmap(Form1.executable.GetManifestResourceStream("Pointable.Resources.Pointables_ball.png"));
              //  smallBmp.MakeTransparent();
                g.DrawImage(bitmapBallStatic, resizeRectangle(new Rectangle(0, 0, bitmapBall.Width, bitmapBall.Height)));
               // smallBmp.Dispose();

                {
                    // center = "sound_volume_blank.png";
                    // smallBmp = Image.FromFile(Form1.iconFolderPath+center) as Bitmap;
                    //smallBmp = new Bitmap(Form1.executable.GetManifestResourceStream("Pointable.Resources.sound_volume_blank.png"));

                    //smallBmp.MakeTransparent();
                    g.DrawImage(bitmapVolumeBlankStatic, resizeRectangle(new Rectangle((bitmapBall.Width - bitmapVolumeBlankStatic.Width) / 2,
                        (bitmapBall.Height - bitmapVolumeBlankStatic.Height) / 2, bitmapVolumeBlankStatic.Width, bitmapVolumeBlankStatic.Height)));
                    //smallBmp.Dispose();
                }

                if (top != null && top != "")
                {
                    if (centerFromResource)
                        bitmapObjects[0] = new Bitmap(Form1.executable.GetManifestResourceStream("Pointable.Resources." + top));
                    else
                        bitmapObjects[0] = Image.FromFile(Form1.iconFolderPath + top) as Bitmap;
                    bitmapObjects[0].MakeTransparent();
                    g.DrawImage(bitmapObjects[0], resizeRectangle(pictureBoxTop.Bounds));
                    //  bitmapObjects[0].Dispose();
                }
                if (right != null && right != "")
                {
                    if (centerFromResource)
                        bitmapObjects[1] = new Bitmap(Form1.executable.GetManifestResourceStream("Pointable.Resources." + right));
                    else
                        bitmapObjects[1] = Image.FromFile(Form1.iconFolderPath + right) as Bitmap;
                    bitmapObjects[1].MakeTransparent();
                    g.DrawImage(bitmapObjects[1], resizeRectangle(pictureBoxRight.Bounds));
                    // smallBmp.Dispose();
                }
                if (bottom != null && bottom != "")
                {
                    if (centerFromResource)
                        bitmapObjects[2] = new Bitmap(Form1.executable.GetManifestResourceStream("Pointable.Resources." + bottom));
                    else
                        bitmapObjects[2] = Image.FromFile(Form1.iconFolderPath + bottom) as Bitmap;
                    bitmapObjects[2].MakeTransparent();
                    g.DrawImage(bitmapObjects[2], resizeRectangle(pictureBoxBottom.Bounds));
                    // smallBmp.Dispose();
                }
                if (left != null && left != "")
                {
                    if (centerFromResource)
                        bitmapObjects[3] = new Bitmap(Form1.executable.GetManifestResourceStream("Pointable.Resources." + left));
                    else
                        bitmapObjects[3] = Image.FromFile(Form1.iconFolderPath + left) as Bitmap;
                    bitmapObjects[3].MakeTransparent();
                    g.DrawImage(bitmapObjects[3], resizeRectangle(pictureBoxLeft.Bounds));
                    // smallBmp.Dispose();
                }



               // SetBitmap(bitmapBallCurrentSettings, (byte)255);
                             

                g.Dispose();


            }
            catch { }
        }

        bool triggeringVolume = false;
        internal void drawTriggeredVolume(int masterVolume)
        {
            triggeringVolume = true;

            setVolume(true, masterVolume);
            
            setBlurSound(true);
            //set timer 
            startTimer();
        }

        internal void drawTriggeredMute(Bitmap bitmap)
        {
            //triggeringVolume = true;

            //setVolume(masterVolume);

            setBlurSound(true);
            //drawTriggeredIcon(bitmap);
            //set timer 
            startTimer();
        }

        internal void drawTriggeredIcon(Bitmap bitmap)
        {
            try
            {
                if (bitmap == null) return;

                Bitmap bitmapBallTemp = new Bitmap(bitmapBall.Width, bitmapBall.Height);
                Graphics g = Graphics.FromImage(bitmapBallTemp);

                g.CompositingMode = CompositingMode.SourceOver;

                Bitmap smallBmp;

                smallBmp = bitmapBallRealTime;
                smallBmp.MakeTransparent();
                g.DrawImage(smallBmp, (new Rectangle(0, 0, bitmapBall.Width, bitmapBall.Height)));

                smallBmp = bitmap;
                smallBmp.MakeTransparent();
                g.DrawImage(smallBmp, resizeRectangle(new Rectangle(250, 250, bitmapBall.Width - 500, bitmapBall.Height - 500)));
                //smallBmp.Dispose();

                SetBitmap(bitmapBallTemp, (byte)255);
                g.Dispose();

                //set timer 
                startTimer();
            }
            catch { }
        }

        internal void drawTriggeredIcon(string imageFilename)
        {
            try
            {
                if (imageFilename == null || imageFilename == "") return;

                Bitmap bitmapBallTemp = new Bitmap(bitmapBall.Width, bitmapBall.Height);
                Graphics g = Graphics.FromImage(bitmapBallTemp);

                g.CompositingMode = CompositingMode.SourceOver;

                Bitmap smallBmp;

                smallBmp = bitmapBallRealTime;
                smallBmp.MakeTransparent();
                g.DrawImage(smallBmp, (new Rectangle(0, 0, bitmapBall.Width, bitmapBall.Height)));

                smallBmp = Image.FromFile(Form1.iconFolderPath + imageFilename) as Bitmap;
                smallBmp.MakeTransparent();
                g.DrawImage(smallBmp, resizeRectangle(new Rectangle(0, 0, bitmapBall.Width, bitmapBall.Height)));
                smallBmp.Dispose();

                SetBitmap(bitmapBallTemp, (byte)255);
                g.Dispose();

                //set timer 
                startTimer();
            }
            catch { }
        }

        System.Timers.Timer _timer;
        private void startTimer()
        {
            try
            {
                if (_timer != null) _timer.Stop();

                if (_timer == null || !_timer.Enabled)
                {
                    _timer = new System.Timers.Timer(750);
                    _timer.Elapsed += new ElapsedEventHandler(_timer_Elapsed);
                    _timer.Enabled = true;
                }
            }
            catch { }
        }
       // internal bool isSpeakers = false;

        void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                triggeringVolume = false;
                _timer.Stop();
                BeginInvoke((MethodInvoker)delegate()
                {
                    //SetBitmap(bitmapBallCurrentSettings, (byte)255);
                    if (FormControl.isSpeakers)
                        setVolume(true, masterVolume);
                    else
                        SetBitmap(bitmapBallCurrentSettings);
                });
            }
            catch { }
        }

        internal void setBlur(int blurPixels)
        {

            try
            {
                if (_timer != null)
                {
                    _timer.Stop();
                    triggeringVolume = false;
                }

                bitmapBallRealTime = new Bitmap(bitmapBall.Width, bitmapBall.Height);
                Graphics g = Graphics.FromImage(bitmapBallRealTime);

                g.CompositingMode = CompositingMode.SourceOver;

                drawScreenIcons(g);
                //Bitmap smallBmp;

                ImageTools.FastBlur(bitmapBallRealTime, blurPixels);
               // WriteableBitmap bitmap = new WriteableBitmap(bitmapBallRealTime);
                //bitmap.BoxBlur(13);
                //bitmapBallRealTime = bitmap.T

                bitmapBallRealTime.MakeTransparent();

                g = Graphics.FromImage(bitmapBallRealTime);

                //smallBmp = new Bitmap(Form1.executable.GetManifestResourceStream("Pointable.Resources.Pointables_ball_edge.png"));//Image.FromFile(file) as Bitmap; //Form1.iconFolderPath + "Pointables_spotlight.png"

                //smallBmp.MakeTransparent();
                g.DrawImage(bitmapBallEdgeStatic, resizeRectangle(new Rectangle(0, 0, bitmapBall.Width, bitmapBall.Height)));
                //smallBmp.Dispose();


                bitmapBallRealTime.MakeTransparent();

                SetBitmap(bitmapBallRealTime, (byte)255);
                g.Dispose();
            }
            catch { }
        }

        internal void setBlurSound(bool volumeChange)
        {
            if (_timer != null)
            {
                _timer.Stop();
                triggeringVolume = false;
            }

            try
            {
                //bitmapBallRealTime = 
                drawSoundIconsBitmap(); //updates bitmapballrealtime

               // bitmapBallRealTime = new Bitmap(bitmapBall.Width, bitmapBall.Height);
                Graphics g = Graphics.FromImage(bitmapBallRealTime);

                g.CompositingMode = CompositingMode.SourceOver;

                //drawSoundIcons(g);


                Bitmap smallBmp;

                if (!volumeChange)
                {
                    //smallBmp = new Bitmap(Form1.executable.GetManifestResourceStream("Pointable.Resources.sound_volume_blank.png"));//Image.FromFile(file) as Bitmap; //Form1.iconFolderPath + "Pointables_spotlight.png"
                    //smallBmp.MakeTransparent();
                    g.DrawImage(bitmapVolumeBlankStatic, resizeRectangle(new Rectangle((bitmapBall.Width - bitmapVolumeBlankStatic.Width) / 2, 
                        (bitmapBall.Height - bitmapVolumeBlankStatic.Height) / 2, bitmapVolumeBlankStatic.Width, bitmapVolumeBlankStatic.Height)));
                    //smallBmp.Dispose();

                    //smallBmp = new Bitmap(Form1.executable.GetManifestResourceStream("Pointable.Resources.sound_volume_full.png"));//Image.FromFile(file) as Bitmap; //Form1.iconFolderPath + "Pointables_spotlight.png"
                    //smallBmp.MakeTransparent();
                    smallBmp = bitmapVolumeFullStatic;
                    int height = masterVolume * smallBmp.Height / 100;

                    Rectangle srcRect = new Rectangle(0, smallBmp.Height - height, smallBmp.Width, height);
                    Rectangle destRect = new Rectangle((bitmapBall.Width - smallBmp.Width) / 2,
                        (bitmapBall.Height - smallBmp.Height) / 2 + smallBmp.Height - height, smallBmp.Width, height);

                    g.DrawImage(smallBmp, resizeRectangle(destRect), srcRect, GraphicsUnit.Pixel);
                    //smallBmp.Dispose();

                    if (FormControl.audioMute)
                    {
                        smallBmp = new Bitmap(Form1.executable.GetManifestResourceStream("Pointable.Resources.sound_volume_mute.png"));//Image.FromFile(file) as Bitmap; //Form1.iconFolderPath + "Pointables_spotlight.png"
                        smallBmp.MakeTransparent();
                        g.DrawImage(smallBmp,  resizeRectangle(new Rectangle( (bitmapBall.Width - smallBmp.Width) / 2, 
                            (bitmapBall.Height - smallBmp.Height) / 2, smallBmp.Width, smallBmp.Height)));
                        smallBmp.Dispose();
                    }
                }

                g.Dispose();
                ImageTools.FastBlur(bitmapBallRealTime, 5);

                bitmapBallRealTime.MakeTransparent();

                g = Graphics.FromImage(bitmapBallRealTime);
                // smallBmp = Image.FromFile(Form1.iconFolderPath + "Pointables_ball_edge.png") as Bitmap;
               // smallBmp = new Bitmap(Form1.executable.GetManifestResourceStream("Pointable.Resources.Pointables_ball_edge.png"));//Image.FromFile(file) as Bitmap; //Form1.iconFolderPath + "Pointables_spotlight.png"

               // smallBmp.MakeTransparent();
                g.DrawImage(bitmapBallEdgeStatic, resizeRectangle(new Rectangle(0, 0, bitmapBall.Width, bitmapBall.Height)));
               // smallBmp.Dispose();

                if (volumeChange)
                {
                    //smallBmp = new Bitmap(Form1.executable.GetManifestResourceStream("Pointable.Resources.sound_volume_blank.png"));
                    //smallBmp.MakeTransparent();
                    g.DrawImage(bitmapVolumeBlankStatic,  resizeRectangle(new Rectangle((bitmapBall.Width - bitmapVolumeBlankStatic.Width) / 2, 
                        (bitmapBall.Height - bitmapVolumeBlankStatic.Height) / 2, bitmapVolumeBlankStatic.Width, bitmapVolumeBlankStatic.Height)));
                    //smallBmp.Dispose();

                    //smallBmp = new Bitmap(Form1.executable.GetManifestResourceStream("Pointable.Resources.sound_volume_full.png"));
                   // smallBmp.MakeTransparent();
                    smallBmp = bitmapVolumeFullStatic;
                    int height = masterVolume * smallBmp.Height / 100;

                    Rectangle srcRect = new Rectangle(0, smallBmp.Height - height, smallBmp.Width, height);
                    Rectangle destRect = new Rectangle((bitmapBall.Width - smallBmp.Width) / 2,
                        (bitmapBall.Height - smallBmp.Height) / 2 + smallBmp.Height - height, smallBmp.Width, height);

                    g.DrawImage(smallBmp, resizeRectangle(destRect), srcRect, GraphicsUnit.Pixel);
                    //smallBmp.Dispose();

                    if (FormControl.audioMute)
                    {
                        smallBmp = new Bitmap(Form1.executable.GetManifestResourceStream("Pointable.Resources.sound_volume_mute.png"));//Image.FromFile(file) as Bitmap; //Form1.iconFolderPath + "Pointables_spotlight.png"
                        smallBmp.MakeTransparent();
                        g.DrawImage(smallBmp,  resizeRectangle(new Rectangle((bitmapBall.Width - smallBmp.Width) / 2, 
                            (bitmapBall.Height - smallBmp.Height) / 2, smallBmp.Width, smallBmp.Height)));
                        smallBmp.Dispose();
                    }
                }


                bitmapBallRealTime.MakeTransparent();

                SetBitmap(bitmapBallRealTime, (byte)255);
                g.Dispose();
            }

            catch { }
        }
        private void drawScreenIcons(Graphics g)
        {
            try
            {
                g.CompositingMode = CompositingMode.SourceOver;

                Bitmap smallBmp;

                // smallBmp = new Bitmap(Form1.executable.GetManifestResourceStream("Pointable.Resources.Pointables_ball_no_edge.png"));

                //smallBmp.MakeTransparent();
                g.DrawImage(bitmapBallNoEdgeStatic, resizeRectangle(new Rectangle(0, 0, bitmapBall.Width, bitmapBall.Height)));
                // smallBmp.Dispose();


                smallBmp = bitmapObjects[4];
                if (smallBmp != null)
                {
                    smallBmp.MakeTransparent();
                    g.DrawImage(smallBmp, resizeRectangle(pictureBoxCenter.Bounds));
                }

                smallBmp = bitmapObjects[0];
                if (smallBmp != null)
                {
                    smallBmp.MakeTransparent();
                    g.DrawImage(smallBmp, resizeRectangle(pictureBoxTop.Bounds));
                }

                smallBmp = bitmapObjects[1];
                if (smallBmp != null)
                {
                    smallBmp.MakeTransparent();
                    g.DrawImage(smallBmp, resizeRectangle(pictureBoxRight.Bounds));
                }

                smallBmp = bitmapObjects[2];
                if (smallBmp != null)
                {
                    smallBmp.MakeTransparent();
                    g.DrawImage(smallBmp, resizeRectangle(pictureBoxBottom.Bounds));
                }

                smallBmp = bitmapObjects[3];
                if (smallBmp != null)
                {
                    smallBmp.MakeTransparent();
                    g.DrawImage(smallBmp, resizeRectangle(pictureBoxLeft.Bounds));
                }
            }
            catch { }
        }

        private void resetBitmapBallSpeaker()
        {
            try
            {
                if (bitmapBallSpeaker != null)
                {
                    bitmapBallSpeaker.Dispose();
                    bitmapBallSpeaker = null;
                }
            }
            catch { }
        }

        private void drawSoundIconsBitmap()
        {
            try
            {
                if (FormTutorial.isAudio)
                    resetBitmapBallSpeaker();

                if (bitmapBallSpeaker == null)
                {
                    bitmapBallSpeaker = new Bitmap(bitmapBall.Width, bitmapBall.Height);
                    Graphics g = Graphics.FromImage(bitmapBallSpeaker);

                    g.CompositingMode = CompositingMode.SourceOver;

                    Bitmap smallBmp;

                    //smallBmp = new Bitmap(Form1.executable.GetManifestResourceStream("Pointable.Resources.Pointables_ball_no_edge.png"));

                   // smallBmp.MakeTransparent();
                    g.DrawImage(bitmapBallNoEdgeStatic, resizeRectangle(new Rectangle(0, 0, bitmapBall.Width, bitmapBall.Height)));
                   // smallBmp.Dispose();


                    smallBmp = bitmapObjects[0];
                    if (smallBmp != null)
                    {
                        smallBmp.MakeTransparent();
                        g.DrawImage(smallBmp, resizeRectangle(pictureBoxTop.Bounds));
                    }

                    smallBmp = bitmapObjects[1];
                    if (smallBmp != null)
                    {
                        smallBmp.MakeTransparent();
                        g.DrawImage(smallBmp, resizeRectangle(pictureBoxRight.Bounds));
                    }

                    smallBmp = bitmapObjects[2];
                    if (smallBmp != null)
                    {
                        smallBmp.MakeTransparent();
                        g.DrawImage(smallBmp, resizeRectangle(pictureBoxBottom.Bounds));
                    }

                    smallBmp = bitmapObjects[3];
                    if (smallBmp != null)
                    {
                        smallBmp.MakeTransparent();
                        g.DrawImage(smallBmp, resizeRectangle(pictureBoxLeft.Bounds));
                    }

                    g.Dispose();
                }
                else// (bitmapBallSpeaker != null)
                {

                }

                bitmapBallRealTime = new Bitmap(bitmapBall.Width, bitmapBall.Height);
                Graphics g2 = Graphics.FromImage(bitmapBallRealTime);
                g2.DrawImage(bitmapBallSpeaker, (new Rectangle(0, 0, bitmapBall.Width, bitmapBall.Height)));
                g2.Dispose();


                if (FormTutorial.isAudio)
                    resetBitmapBallSpeaker();
            }

            catch { }
        }

        private void drawSoundIcons(Graphics g)
        {
            try
            {
                g.CompositingMode = CompositingMode.SourceOver;

                Bitmap smallBmp;

                // smallBmp = new Bitmap(Form1.executable.GetManifestResourceStream("Pointable.Resources.Pointables_ball_no_edge.png"));

                //smallBmp.MakeTransparent();
                g.DrawImage(bitmapBallNoEdgeStatic, resizeRectangle(new Rectangle(0, 0, bitmapBall.Width, bitmapBall.Height)));
               // smallBmp.Dispose();


                smallBmp = bitmapObjects[0];
                if (smallBmp != null)
                {
                    smallBmp.MakeTransparent();
                    g.DrawImage(smallBmp, resizeRectangle(pictureBoxTop.Bounds));
                }

                smallBmp = bitmapObjects[1];
                if (smallBmp != null)
                {
                    smallBmp.MakeTransparent();
                    g.DrawImage(smallBmp, resizeRectangle(pictureBoxRight.Bounds));
                }

                smallBmp = bitmapObjects[2];
                if (smallBmp != null)
                {
                    smallBmp.MakeTransparent();
                    g.DrawImage(smallBmp, resizeRectangle(pictureBoxBottom.Bounds));
                }

                smallBmp = bitmapObjects[3];
                if (smallBmp != null)
                {
                    smallBmp.MakeTransparent();
                    g.DrawImage(smallBmp, resizeRectangle(pictureBoxLeft.Bounds));
                }
            }

            catch { }
        }
        internal void resetUI()
        {
            updatePosition();

            //Bitmap newBitmap;

            try
            {
               // newBitmap = Image.FromFile(Form1.iconFolderPath + "Pointables_ball.png") as Bitmap;
               // newBitmap = new Bitmap(Form1.executable.GetManifestResourceStream("Pointable.Resources.Pointables_ball.png"));

                SetBitmap(bitmapBallSmallStatic, (byte)255);
            }
            catch (ApplicationException e)
            {
                return;
            }
            catch (Exception e)
            {
                 return;
            }

            // if (bitmapBall != null)
            //     bitmapBall.Dispose();
            bitmapBall = bitmapBallStatic;

            try
            {
                if (_timer != null)
                    _timer.Stop();


                triggeringVolume = false;
            }
            catch { }
        }

        public Bitmap Superimpose(Bitmap largeBmp, Bitmap smallBmp)
        {
            Graphics g = Graphics.FromImage(largeBmp);
            try
            {
                g.CompositingMode = CompositingMode.SourceOver;
                smallBmp.MakeTransparent();
                int margin = 5;
                int x = largeBmp.Width - smallBmp.Width - margin;
                int y = largeBmp.Height - smallBmp.Height - margin;
                g.DrawImage(smallBmp, new Point(x, y));
            }
            catch { }
            return largeBmp;
        }

        internal int masterVolume;

        internal void setVolume(bool setWindow, int mVolume)
        {//0 to 100
            try
            {
                masterVolume = mVolume;

                Bitmap smallBmp;

                if (triggeringVolume)
                {//blur volume
                    Graphics g = Graphics.FromImage(bitmapBallRealTime);

                    //smallBmp = new Bitmap(Form1.executable.GetManifestResourceStream("Pointable.Resources.sound_volume_blank.png"));

                    //smallBmp.MakeTransparent();
                    g.DrawImage(bitmapVolumeBlankStatic, resizeRectangle(new Rectangle((bitmapBall.Width - bitmapVolumeBlankStatic.Width) / 2, 
                        (bitmapBall.Height - bitmapVolumeBlankStatic.Height) / 2, bitmapVolumeBlankStatic.Width, bitmapVolumeBlankStatic.Height)));
                   // smallBmp.Dispose();

                   // smallBmp = new Bitmap(Form1.executable.GetManifestResourceStream("Pointable.Resources.sound_volume_full.png"));

                   // smallBmp.MakeTransparent();
                    smallBmp = bitmapVolumeFullStatic;
                    int height = masterVolume * smallBmp.Height / 100;

                    Rectangle srcRect = new Rectangle(0, smallBmp.Height - height, smallBmp.Width, height);
                    Rectangle destRect = new Rectangle((bitmapBall.Width - smallBmp.Width) / 2,
                        (bitmapBall.Height - smallBmp.Height) / 2 + smallBmp.Height - height, smallBmp.Width, height);

                    g.DrawImage(smallBmp, resizeRectangle(destRect), srcRect, GraphicsUnit.Pixel);
                   // smallBmp.Dispose();

                    if (FormControl.audioMute)
                    {
                        smallBmp = new Bitmap(Form1.executable.GetManifestResourceStream("Pointable.Resources.sound_volume_mute.png"));//Image.FromFile(file) as Bitmap; //Form1.iconFolderPath + "Pointables_spotlight.png"
                        smallBmp.MakeTransparent();
                        g.DrawImage(smallBmp,  resizeRectangle(new Rectangle((bitmapBall.Width - smallBmp.Width) / 2, 
                            (bitmapBall.Height - smallBmp.Height) / 2, smallBmp.Width, smallBmp.Height)));
                        smallBmp.Dispose();
                    }

                    g.Dispose();
                }
                else
                {
                    bitmapBallRealTime = new Bitmap(bitmapBall.Width, bitmapBall.Height);
                    Graphics g = Graphics.FromImage(bitmapBallRealTime);

                    g.CompositingMode = CompositingMode.SourceOver;

                    smallBmp = bitmapBallCurrentSettings;
                    smallBmp.MakeTransparent();
                    g.DrawImage(smallBmp, (new Rectangle(0, 0, bitmapBall.Width, bitmapBall.Height)));

                    //smallBmp = new Bitmap(Form1.executable.GetManifestResourceStream("Pointable.Resources.sound_volume_full.png"));

                   // smallBmp.MakeTransparent();
                    smallBmp = bitmapVolumeFullStatic;

                    int height = masterVolume * smallBmp.Height / 100;

                    Rectangle srcRect = new Rectangle(0, smallBmp.Height - height, smallBmp.Width, height);
                    Rectangle destRect = new Rectangle((bitmapBall.Width - smallBmp.Width) / 2,
                        (bitmapBall.Height - smallBmp.Height) / 2 + smallBmp.Height - height, smallBmp.Width, height);

                    g.DrawImage(smallBmp,  resizeRectangle(destRect), srcRect, GraphicsUnit.Pixel);
                    //smallBmp.Dispose();

                    if (FormControl.audioMute)
                    {
                        smallBmp = new Bitmap(Form1.executable.GetManifestResourceStream("Pointable.Resources.sound_volume_mute.png"));
                        smallBmp.MakeTransparent();
                        g.DrawImage(smallBmp,  resizeRectangle(new Rectangle((bitmapBall.Width - smallBmp.Width) / 2, 
                            (bitmapBall.Height - smallBmp.Height) / 2, smallBmp.Width, smallBmp.Height)));
                        smallBmp.Dispose();
                    }
                    g.Dispose();
                }



                //bitmapBallRealTime.Save(@"C:\Test\ball2" + i++ + ".png");
                if (setWindow)
                    SetBitmap(bitmapBallRealTime, (byte)255);

            }
            catch { }
        }
    }
 }

