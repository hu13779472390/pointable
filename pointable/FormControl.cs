using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CoreAudioApi;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;
using System.Drawing.Drawing2D;
using PointableUI;
using Action = PointableUI.Action;
using System.Timers;

namespace LeapProject
{
    partial struct PointableObjectOld
    {
        public string iconCenter;
        public string iconTop;
        public string iconRight;
        public string iconBottom;
        public string iconLeft;
        

        public PointableObjectOld(string center, string top, string right, string bottom, string left)
        {
            iconCenter = center;
            iconTop = top;
            iconRight = right;
            iconBottom = bottom;
            iconLeft = left;
        }

        public string getIconName(FormControl.ControlLocations controlLocation)
        {
            switch (controlLocation)
            {
                case FormControl.ControlLocations.Top:
                    return iconTop;
                case FormControl.ControlLocations.Right:
                    return iconRight;
                case FormControl.ControlLocations.Bottom:
                    return iconBottom;
                case FormControl.ControlLocations.Left:
                    return iconLeft;
            }
            return null;
        }

    }

    partial class FormControl : PerPixelAlphaForm
    {
        //imports keybd_event function from user32.dll
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void keybd_event(byte bVk, byte bScan, long dwFlags, long dwExtraInfo);

        private MMDevice device;
        private Form1 mainForm;

        private static byte triggerKey;
        private static byte triggerKey2;
        private static byte[] triggerKeyArray;

        private static string triggerFilename;
        private static string triggerFileArgs;

        internal int masterVolume = 0;
        int initialVolume = 0;
       
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
        protected override bool ShowWithoutActivation
        {
            get { return true; }
        }

        private Bitmap bitmapSpotlight;
        private Bitmap bitmapSpotlightCurrent;

        private Bitmap bitmapShadow;
        private Bitmap bitmapWindowCursor;
        private Bitmap bitmapWindowCursorDropClick;
        private Bitmap bitmapWindowCursorDropClickYellow;

        private Bitmap bitmapWindowCursorMove;
        private Bitmap[] bitmapWindowCursorProgress;


        internal ShadowForm shadowForm;	// our test form
        internal WindowCursorForm windowCursorForm;

        internal WindowCursorForm windowCursorDropForm;

        internal WindowCursorForm windowCursorDropFormClick;

        internal static bool isSpeakers;
        internal static bool audioMute = false; 

        //int objectIndex;

       // PointableObjectOld speakerObject = new PointableObjectOld("sound.png", "sound_increase.png",
       //             "sound_next.png", "sound_decrease.png", "sound_mute.png");
       //// PointableObject screenObject = new PointableObject("screen.png", "screen_ppt.png",
       ////          "screen_word.png", "screen_excel.png", "screen_chrome.png");
       // PointableObjectOld monitorObject = new PointableObjectOld("spotify.png", "spotify_launch.png",
       //             "spotify_next.png", "spotify_pause.png", "spotify_previous.png");
       // PointableObjectOld screenObject = new PointableObjectOld("screen.png", "screen_pointable.png",
       //          "screen_settings.png", "screen_show_desktop.png", "screen_twitter.png");
       //// PointableObject pictureObject = new PointableObject("todo.png", "todo_amazon.png",
       // //            "todo_clock.png", "todo_calendar.png", "todo_pen.png");
       // PointableObjectOld pictureObject = new PointableObjectOld("stephanie.png", "stephanie_skype.png",
       //             "stephanie_gmail.png", "stephanie_facebook.png", "stephanie_twitter.png");
       // //
       // PointableObject pictureObject = new PointableObject("monitor.png", "screen_ppt.png",
        //            "", "picture_photoshop.png", "");

        //PointableObjectOld[] pointableObjects = new PointableObjectOld[4];


        public FormControl(Form1 _form)
        {
            //pointableObjects[0] = speakerObject;
            //pointableObjects[1] = monitorObject;
            //pointableObjects[2] = screenObject;
            //pointableObjects[3] = pictureObject;

            mainForm = _form;
            TopMost = true;
            ShowInTaskbar = false;
            InitializeComponent();

            try
            {

                MMDeviceEnumerator DevEnum = new MMDeviceEnumerator();
                device = DevEnum.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);
                tbMaster.Value = (int)(device.AudioEndpointVolume.MasterVolumeLevelScalar * 100);
                device.AudioEndpointVolume.OnVolumeNotification += new AudioEndpointVolumeNotificationDelegate(AudioEndpointVolume_OnVolumeNotification);
                audioMute = device.AudioEndpointVolume.Mute;
            }
            catch { }

            Bitmap newBitmap;

            try
            {
                newBitmap = new Bitmap(Form1.executable.GetManifestResourceStream("Pointable.Resources.Pointables_spotlight.png"));//Image.FromFile(file) as Bitmap; //Form1.iconFolderPath + "Pointables_spotlight.png"
                SetBitmap(newBitmap, (byte)220);
            }
            catch (ApplicationException e)
            {
                //MessageBox.Show(this, e.Message, "Error with bitmap.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            catch (Exception e)
            {
               // MessageBox.Show(this, e.Message, "Could not open image file.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                if (bitmapSpotlight != null)
                    bitmapSpotlight.Dispose();

                bitmapSpotlight = newBitmap;

                this.Height = bitmapSpotlight.Height;
                this.Width = bitmapSpotlight.Width;

                setShadowForm();
                setWindowCursorForm();
                setWindowCursorDropForm();
                setWindowCursorDropFormClick();
                loadCursorClickProgress();
            }
            catch { }
        }

        private void setShadowForm()
        {
            shadowForm = new ShadowForm();

            Bitmap newBitmap;

            try
            {
               // newBitmap = Image.FromFile(Form1.iconFolderPath+ "Pointables_shadow.png") as Bitmap;
                newBitmap = new Bitmap(Form1.executable.GetManifestResourceStream("Pointable.Resources.Pointables_shadow.png"));//Image.FromFile(file) as Bitmap; //Form1.iconFolderPath + "Pointables_spotlight.png"

                shadowForm.SetBitmap(newBitmap, (byte) 200);
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

            try
            {
                if (bitmapShadow != null)
                    bitmapShadow.Dispose();
                bitmapShadow = newBitmap;
            }
            catch { }
        }


        internal void setCursorMove()
        {
            try
            {
                windowCursorForm.SetBitmapWithOpacity(bitmapWindowCursorMove, (byte)255);
                currentWindowCursorNormal = false;
            }
            catch 
            {
                return;
            }
        }
        internal void setCursorNormal()
        {
            try
            {
                currentWindowCursorNormalOpacity = 255;
                windowCursorForm.SetBitmapWithOpacity(bitmapWindowCursor, (byte)255);
                currentWindowCursorNormal = true;
            }
            catch 
            {
                return;
            }
        }

        internal void setCursorNormal(byte opacity)
        {
            try
            {
                if (opacity != currentWindowCursorNormalOpacity)
                {
                    currentWindowCursorNormalOpacity = opacity;
                    windowCursorForm.SetBitmapWithOpacity(bitmapWindowCursor, (byte)opacity);
                }
                currentWindowCursorNormal = true;
            }
            catch
            {
                return;
            }
        }

        internal void setCursorDropletRefresh()
        {
            try
            {
                Bitmap newBitmap = new Bitmap(Form1.executable.GetManifestResourceStream("Pointable.Resources.cursor.Pointable_cursor_drop.png"));//Image.FromFile(file) as Bitmap; //Form1.iconFolderPath + "Pointables_spotlight.png"
                windowCursorDropForm.SetBitmapWithOpacity(newBitmap, (byte)255);
            }
            catch { }
        }
        internal void setCursorDropletClickRedRefresh()
        {
            try
            {
                windowCursorDropFormClick.SetBitmapWithOpacity(bitmapWindowCursorDropClick, (byte)255);
            }
            catch { }
        } 

        private void setWindowCursorForm()
        {
            windowCursorForm = new WindowCursorForm();

            Bitmap newBitmap;

            try
            {
                // newBitmap = Image.FromFile(Form1.iconFolderPath+ "Pointables_shadow.png") as Bitmap;
                newBitmap = new Bitmap(Form1.executable.GetManifestResourceStream("Pointable.Resources.Pointables_window_cursor.png"));//Image.FromFile(file) as Bitmap; //Form1.iconFolderPath + "Pointables_spotlight.png"
                bitmapWindowCursorMove = new Bitmap(Form1.executable.GetManifestResourceStream("Pointable.Resources.Pointables_window_cursor_move.png"));//Image.FromFile(file) as Bitmap; //Form1.iconFolderPath + "Pointables_spotlight.png"

                windowCursorForm.SetBitmapWithOpacity(newBitmap, (byte)255);

                windowCursorForm.Width = newBitmap.Width;
                windowCursorForm.Height = newBitmap.Height;
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

            try
            {
                if (bitmapWindowCursor != null)
                    bitmapWindowCursor.Dispose();
                bitmapWindowCursor = newBitmap;
            }
            catch { }
        }

        internal void drawWindowCursorDropAbsolute(int x, int y)
        {
            try
            {
                windowCursorDropForm.Left = x - windowCursorForm.Width / 2;
                windowCursorDropForm.Top = y - windowCursorForm.Height / 2;

                if (!windowCursorDropForm.Visible)
                {
                    windowCursorDropForm.Visible = true;
                }

                //windowCursorDropForm.TopMost = true;
                Tools.SetWindowPos(this.windowCursorDropForm.Handle, Tools.HWND.TOPMOST, 0, 0, 0, 0, Tools.SetWindowPosFlags.IgnoreMove | Tools.SetWindowPosFlags.IgnoreResize | Tools.SetWindowPosFlags.DoNotActivate);
      
               // windowCursorDropForm.ActiveControl = null;

                if (_timerProgressClick == null)
                {
                    _timerProgressClick = new System.Timers.Timer(2200); //300
                    _timerProgressClick.Elapsed += new ElapsedEventHandler(_timerProgressClick_Elapsed);
                    _timerProgressClick.Enabled = true;
                }
                else
                {
                    _timerProgressClick.Enabled = false;
                    _timerProgressClick.Enabled = true;
                }
            }
            catch { }
        }

        bool currentDropRed = true;

        internal void drawWindowCursorDropClickAbsolute(int x, int y, bool dropRed)
        {
            try
            {
                if (currentDropRed != dropRed)
                {
                    currentDropRed = dropRed;

                    if (dropRed)
                    {
                        try
                        {
                            windowCursorDropFormClick.SetBitmapWithOpacity(bitmapWindowCursorDropClick, (byte)255);
                        }
                        catch { }
                    }
                    else
                    {
                        try
                        {
                            windowCursorDropFormClick.SetBitmapWithOpacity(bitmapWindowCursorDropClickYellow, (byte)255);
                        }
                        catch { }
                    }
                }

                windowCursorDropFormClick.Left = x - windowCursorDropFormClick.Width / 2;
                windowCursorDropFormClick.Top = y - windowCursorDropFormClick.Height / 2;

                if (!windowCursorDropFormClick.Visible)
                    windowCursorDropFormClick.Show();


               // windowCursorDropFormClick.TopMost = true;
                Tools.SetWindowPos(this.windowCursorDropFormClick.Handle, Tools.HWND.TOPMOST, 0, 0, 0, 0, Tools.SetWindowPosFlags.IgnoreMove | Tools.SetWindowPosFlags.IgnoreResize | Tools.SetWindowPosFlags.DoNotActivate);
      

                if (_timerProgressClickAction == null)
                {
                    _timerProgressClickAction = new System.Timers.Timer(250); //300
                    _timerProgressClickAction.Elapsed += new ElapsedEventHandler(_timerProgressClickAction_Elapsed);
                    _timerProgressClickAction.Enabled = true;
                }
                else
                {
                    _timerProgressClickAction.Enabled = false;
                    _timerProgressClickAction.Enabled = true;
                }
            }
            catch { }
        }

        //internal void drawWindowCursorDropClickAbsoluteContinuous(int x, int y, bool dropRed)
        //{
        //    try
        //    {
        //        if (currentDropRed != dropRed)
        //        {
        //            currentDropRed = dropRed;

        //            if (dropRed)
        //            {
        //                try
        //                {
        //                    windowCursorDropFormClick.SetBitmapWithOpacity(bitmapWindowCursorDropClick, (byte)255);
        //                }
        //                catch { }
        //            }
        //            else
        //            {
        //                try
        //                {
        //                    windowCursorDropFormClick.SetBitmapWithOpacity(bitmapWindowCursorDropClickYellow, (byte)255);
        //                }
        //                catch { }
        //            }
        //        }

        //        windowCursorDropFormClick.Left = x - windowCursorDropFormClick.Width / 2;
        //        windowCursorDropFormClick.Top = y - windowCursorDropFormClick.Height / 2;

        //        if (!windowCursorDropFormClick.Visible)
        //            windowCursorDropFormClick.Show();

        //        windowCursorDropFormClick.TopMost = true;

        //    }
        //    catch { }
        //}

        private void setWindowCursorDropForm()
        {
            windowCursorDropForm = new WindowCursorForm();

            Bitmap newBitmap;

            try
            {
                // newBitmap = Image.FromFile(Form1.iconFolderPath+ "Pointables_shadow.png") as Bitmap;
                newBitmap = new Bitmap(Form1.executable.GetManifestResourceStream("Pointable.Resources.cursor.Pointable_cursor_drop.png"));//Image.FromFile(file) as Bitmap; //Form1.iconFolderPath + "Pointables_spotlight.png"
                windowCursorDropForm.SetBitmapWithOpacity(newBitmap, (byte)255);

                windowCursorDropForm.Width = newBitmap.Width;
                windowCursorDropForm.Height = newBitmap.Height;

                //windowCursorDropForm.TopMost = true;
                Tools.SetWindowPos(this.windowCursorDropForm.Handle, Tools.HWND.TOPMOST, 0, 0, 0, 0, Tools.SetWindowPosFlags.IgnoreMove | Tools.SetWindowPosFlags.IgnoreResize | Tools.SetWindowPosFlags.DoNotActivate);
      
            }
            catch (Exception e)
            {
                // MessageBox.Show(this, e.Message, "Could not open image file.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        private void setWindowCursorDropFormClick()
        {
            windowCursorDropFormClick = new WindowCursorForm();

            //Bitmap newBitmap;

            try
            {
                // newBitmap = Image.FromFile(Form1.iconFolderPath+ "Pointables_shadow.png") as Bitmap;
                bitmapWindowCursorDropClick = new Bitmap(Form1.executable.GetManifestResourceStream("Pointable.Resources.cursor.Pointable_cursor_drop_red.png"));//Image.FromFile(file) as Bitmap; //Form1.iconFolderPath + "Pointables_spotlight.png"
                bitmapWindowCursorDropClickYellow = new Bitmap(Form1.executable.GetManifestResourceStream("Pointable.Resources.cursor.Pointable_cursor_drop_yellow.png"));//Image.FromFile(file) as Bitmap; //Form1.iconFolderPath + "Pointables_spotlight.png"

                windowCursorDropFormClick.SetBitmapWithOpacity(bitmapWindowCursorDropClick, (byte)255);

                windowCursorDropFormClick.Width = bitmapWindowCursorDropClick.Width;
                windowCursorDropFormClick.Height = bitmapWindowCursorDropClick.Height;

                //windowCursorDropFormClick.TopMost = true;
                Tools.SetWindowPos(this.windowCursorDropFormClick.Handle, Tools.HWND.TOPMOST, 0, 0, 0, 0, Tools.SetWindowPosFlags.IgnoreMove | Tools.SetWindowPosFlags.IgnoreResize | Tools.SetWindowPosFlags.DoNotActivate);
      
            }
            catch (Exception e)
            {
                // MessageBox.Show(this, e.Message, "Could not open image file.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        void AudioEndpointVolume_OnVolumeNotification(AudioVolumeNotificationData data)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    object[] Params = new object[1];
                    Params[0] = data;
                    this.Invoke(new AudioEndpointVolumeNotificationDelegate(AudioEndpointVolume_OnVolumeNotification), Params);
                }
                else
                {
                    masterVolume = (int)(data.MasterVolume * 100);
                    tbMaster.Value = masterVolume;
                    audioMute = device.AudioEndpointVolume.Mute;
                    // mainForm.formControlGroup.setVolume(masterVolume);
                }
            }
            catch { }
        }

        private void tbMaster_Scroll(object sender, EventArgs e)
        {
           // device.AudioEndpointVolume.MasterVolumeLevelScalar = ((float)tbMaster.Value / 100.0f);
        }

        private void timerCheckVol_Tick(object sender, EventArgs e)
        {
          //  pkMaster.Value = (int)(device.AudioMeterInformation.MasterPeakValue * 100);
           // pkLeft.Value = (int)(device.AudioMeterInformation.PeakValues[0] * 100);
           // pkRight.Value = (int)(device.AudioMeterInformation.PeakValues[1] * 100);
        }

        internal void setMasterVolume(int x)
        {
            try
            {
                if (x > 100)
                    x = 100;
                if (x < 0)
                    x = 0;
                masterVolume = x;
                device.AudioEndpointVolume.MasterVolumeLevelScalar = ((float)x / 100.0f);
            }
            catch { }
        }
                
        internal void setInitialPosition()
        {
            try
            {
                initialVolume = 50;
                previousControlLocation = ControlLocations.None;
                currentControlLocation = ControlLocations.None;

                if (isSpeakers)
                {
                    masterVolume = (int)(device.AudioEndpointVolume.MasterVolumeLevelScalar * 100);
                    mainForm.formControlGroup.setVolume(true, masterVolume);
                }
            }
            catch { }
        }

        internal PointableObject currentPointable;

        internal void setObjectControl(PointableObject pointableObject)//, int index)
        {
            try
            {
                currentPointable = pointableObject;
                // labelObjectName.Text = strName;
                //objectIndex = index;

                if (pointableObject.description == "Speakers")
                {
                    isSpeakers = true;
                    mainForm.formControlGroup.setUISound(true, currentPointable);
                }
                else
                {
                    isSpeakers = false;
                    mainForm.formControlGroup.setUI(true, currentPointable);
                }

            }

            catch { }

            setInitialPosition();
        }
        internal void setObjectControl(PointableObject pointableObject, Bitmap icon)//, int index)
        {
            try
            {
                currentPointable = pointableObject;
                 isSpeakers = false;
                // mainForm.formControlGroup.setUI(true, currentPointable, icon);

                 mainForm.formControlGroup.setUI(true, currentPointable.iconPath, currentPointable.actions[0].iconPath, 
                     currentPointable.actions[1].iconPath, currentPointable.actions[2].iconPath, currentPointable.actions[3].iconPath,
                currentPointable.iconFromResource, currentPointable.actions[0].iconFromResource, currentPointable.actions[1].iconFromResource,
                currentPointable.actions[2].iconFromResource, currentPointable.actions[3].iconFromResource, icon);

            }

            catch{}// (Exception ex) { Console.WriteLine(ex.ToString()); }

            setInitialPosition();
        }


        internal PointableObject currentObjectHover;

        internal void setObjectHover(PointableObject pointableObject)//, int index)
        {
            try
            {
                if (currentObjectHover == pointableObject) return;

                if (pointableObject == null)
                {
                    mainForm.formControlGroup.resetUI();

                    currentObjectHover = pointableObject;
                    return;
                }

                currentObjectHover = pointableObject;
                // labelObjectName.Text = strName;
                //objectIndex = index;

                if (pointableObject.description == "Speakers")
                {
                    mainForm.formControlGroup.setUISound(false, pointableObject);
                    masterVolume = (int)(device.AudioEndpointVolume.MasterVolumeLevelScalar * 100);
                    mainForm.formControlGroup.masterVolume = masterVolume;
                    mainForm.formControlGroup.setBlurSound(false);
                }else
                {
                    mainForm.formControlGroup.setUI(false, currentObjectHover);
                    mainForm.formControlGroup.setBlur(5);
                }

            }

            catch { }
        }

        public void resetControls()
        {
            drawSpotlight((int)50, (int)50, false);
        }


        internal void increaseVolume()
        {
            try
            {
                Thread th = new Thread(new ThreadStart(increaseVolumeThread));
                th.IsBackground = true;
                th.Start();
            }
            catch { }
        }

        internal void decreaseVolume()
        {
            try
            {
                Thread th = new Thread(new ThreadStart(decreaseVolumeThread));
                th.IsBackground = true;
                th.Start();
            }
            catch { }
        }
        internal void increaseVolumeHalf()
        {
            try
            {
                Thread th = new Thread(new ThreadStart(increaseVolumeHalfThread));
                th.IsBackground = true;
                th.Start();
            }
            catch { }
        }

        internal void decreaseVolumeHalf()
        {
            try
            {
                Thread th = new Thread(new ThreadStart(decreaseVolumeHalfThread));
                th.IsBackground = true;
                th.Start();
            }
            catch { }
        }
        internal void increaseVolumeThread()
        {
            double volume = masterVolume + 13;
            setMasterVolume((int)volume);
        }
        internal void decreaseVolumeThread()
        {
            double volume = masterVolume - 13;
            setMasterVolume((int)volume);
        }

        internal void increaseVolumeHalfThread()
        {
            double volume = masterVolume + 6;
            setMasterVolume((int)volume);
        }
        internal void decreaseVolumeHalfThread()
        {
            double volume = masterVolume - 6;
            setMasterVolume((int)volume);
        }

        public enum ControlTriggerType { Discrete, Variable };
        public enum ControlState { Center, Inside, Outside };
        public enum ControlLocations { Top, Right, Bottom, Left,None };


        Enum[] controlTriggerType = new Enum []{ ControlTriggerType.Variable, ControlTriggerType.Discrete, ControlTriggerType.Variable, ControlTriggerType.Discrete };

        byte[] controlKeyStrokes = new byte[]{(byte)Tools.VK.VK_VOLUME_UP, (byte)Tools.VK.VK_MEDIA_PLAY_PAUSE, (byte)Tools.VK.VK_VOLUME_DOWN, (byte)Tools.VK.VK_VOLUME_MUTE};

        byte[] controlKeyStrokesTutorialAudio = new byte[] { (byte)Tools.VK.VK_VOLUME_UP, (byte)0x36, (byte)Tools.VK.VK_VOLUME_DOWN, (byte)Tools.VK.VK_VOLUME_MUTE };
 
        byte[] controlKeyStrokesSpotify = new byte[]{(byte)Tools.VK.VK_VOLUME_UP, (byte)Tools.VK.VK_MEDIA_NEXT_TRACK, (byte)Tools.VK.VK_MEDIA_PLAY_PAUSE, (byte)Tools.VK.VK_MEDIA_PREV_TRACK};


        ControlLocations previousControlLocation;
        ControlLocations controlLocationExit;
       // ControlLocations controlLocationEntrance;

        ControlLocations currentControlLocation;
        bool isSkippedLocation = false;
        
        
        ControlLocations previousExecutedControlLocation;// = currentControlLocation;
        long previousExecutedControlTime;// = DateTime.Now.Ticks;

        internal void setTracking(double x, double y)//, double coordObject)
        {
            try
            {
                double finalX = x + initialVolume;
                double finalY = y + initialVolume;

                if (finalX > 100) finalX = 100;
                if (finalX < 0) finalX = 0;

                if (finalY > 100) finalY = 100;
                if (finalY < 0) finalY = 0;

                drawSpotlight((int)finalX, (int)finalY, true);

                int LIMIT = 25;

                if (finalY == 100)
                {
                    currentControlLocation = ControlLocations.Top;
                }
                else if (finalY == 0)
                {
                    currentControlLocation = ControlLocations.Bottom;
                }
                else if (finalX == 100)
                {
                    currentControlLocation = ControlLocations.Right;
                }
                else if (finalX == 0)
                {
                    currentControlLocation = ControlLocations.Left;
                }
                else if (finalX < (100 - LIMIT) && finalX > LIMIT && finalY < (100 - LIMIT) && finalY > LIMIT) //TODO
                { //inside
                    currentControlLocation = ControlLocations.None;
                }

                if (previousControlLocation == ControlLocations.None && currentControlLocation != ControlLocations.None)
                {//just exited
                    long durationPreviousExecutedTime = (DateTime.Now.Ticks - previousExecutedControlTime)/10000;
                   
                    if (durationPreviousExecutedTime < 600 && previousExecutedControlLocation != currentControlLocation)
                        isSkippedLocation = true;
                    
                    if (durationPreviousExecutedTime < 230 && previousExecutedControlLocation == currentControlLocation)
                        isSkippedLocation = true;


                   // Console.WriteLine(durationPreviousExecutedTime);

                    controlLocationExit = currentControlLocation;

                    if (!isSkippedLocation)
                    {
                        drawIcon(getIconBitmap(controlLocationExit));

                        if (isSpeakers)
                        {
                            if ((controlLocationExit == ControlLocations.Top ||
                                controlLocationExit == ControlLocations.Bottom || controlLocationExit == ControlLocations.Left))
                                mainForm.formControlGroup.setBlurSound(true);
                            else
                                mainForm.formControlGroup.setBlurSound(false);
                        }
                        else
                        {
                            if (getIconBitmap(controlLocationExit) != null)
                                mainForm.formControlGroup.setBlur(5);
                        }
                    }
                }

                //out to out . already exited and now selectig another action -
                if (currentControlLocation != ControlLocations.None && controlLocationExit != currentControlLocation)
                {//just exited
                    controlLocationExit = currentControlLocation;

                    if (!isSkippedLocation)
                    {
                        drawIcon(getIconBitmap(controlLocationExit));

                        if (isSpeakers)
                        {
                            if ((controlLocationExit == ControlLocations.Top ||
                                controlLocationExit == ControlLocations.Bottom || controlLocationExit == ControlLocations.Left) && previousControlLocation == ControlLocations.Right)
                            {//right side to any other side
                                mainForm.formControlGroup.setBlurSound(true);
                            }
                            else if ((previousControlLocation == ControlLocations.Top ||
                                previousControlLocation == ControlLocations.Bottom || previousControlLocation == ControlLocations.Left) && currentControlLocation == ControlLocations.Right)
                            {//other side to right side
                                mainForm.formControlGroup.setBlurSound(false);
                            }
                        }
                        else
                        {
                            //if (getIconBitmap(controlLocationExit) != null)
                            //    mainForm.formControlGroup.setBlur();
                        }
                    }
                }


                //trigger - out to in from same side
                if (previousControlLocation == controlLocationExit && currentControlLocation == ControlLocations.None && previousControlLocation != ControlLocations.None)
                {
                    previousControlLocation = currentControlLocation;

                    
                    if (isSkippedLocation)
                    {
                        isSkippedLocation = false;
                    }
                    else
                    {
                        previousExecutedControlLocation = controlLocationExit;
                        previousExecutedControlTime = DateTime.Now.Ticks;
                        isSkippedLocation = false;

                        if (isSpeakers)
                        {
                            if (controlLocationExit == ControlLocations.Left ||
                                        controlLocationExit == ControlLocations.Right)
                            {
                                if (controlLocationExit == ControlLocations.Left)
                                    audioMute = !audioMute;

                                if (FormTutorial.isAudio)
                                    triggerKeyEvent((byte)controlKeyStrokesTutorialAudio[(int)controlLocationExit]);
                                else
                                     triggerKeyEvent((byte)controlKeyStrokes[(int)controlLocationExit]);
                            }

                            if (controlLocationExit == ControlLocations.Top)
                                increaseVolume();

                            if (controlLocationExit == ControlLocations.Bottom)
                                decreaseVolume();


                            if (FormTutorial.isAudio)
                            {
                                try
                                {
                                    if (controlLocationExit == ControlLocations.Left &&
                                        FormTutorial.tutorialPointableProgress == 1 && !audioMute)
                                    {
                                        mainForm.formTutorial.setTutorialPointableAnimation(2);
                                    }
                                    else if (controlLocationExit == ControlLocations.Top &&
                                        FormTutorial.tutorialPointableProgress == 2)
                                    {
                                        mainForm.formTutorial.setTutorialPointableAnimation(3);
                                    }
                                    else if (controlLocationExit == ControlLocations.Bottom &&
                                        FormTutorial.tutorialPointableProgress == 3)
                                    {
                                        mainForm.formTutorial.setTutorialPointableAnimation(4);
                                    }
                                    else if (controlLocationExit == ControlLocations.Right &&
                                        FormTutorial.tutorialPointableProgress == 6)
                                    {
                                        //mainForm.formTutorial.setTutorialPointableAnimation(0);
                                    }
                                }
                                catch { }
                            }
                        }
                        else
                        {
                            Action action = currentPointable.actions[(int)controlLocationExit];
                            switch (action.actionType)
                            {
                                case Action.ActionType.LaunchApplication:
                                    if (action.applicationPath == "pointable-configuration")
                                        launchConfigurationThread();
                                    else if (action.applicationPath == "pointable-pauseresume")
                                        launchPauseTrackingThread();
                                    else if (action.applicationPath == "pointable-screencalibration")
                                        launchScreenCalibrationThread();
                                    else if (action.applicationPath == "pointable-shutdown")
                                        launchSystemShutdownThread();
                                    else if (action.applicationPath == "pointable-restart")
                                        launchSystemRestartThread();
                                    else
                                        runApplication(action.applicationPath, action.applicationArgs); //powerpnt
                                    break;
                                case Action.ActionType.Keystroke:
                                    triggerKeyEvent(action.keys);
                                    break;
                            }
                        }

                        resetIcon();

                        if (isSpeakers)
                        {
                            if (controlLocationExit == ControlLocations.Top ||
                                    controlLocationExit == ControlLocations.Bottom)
                                mainForm.formControlGroup.drawTriggeredVolume(masterVolume);
                            else if (controlLocationExit == ControlLocations.Left)
                                mainForm.formControlGroup.drawTriggeredMute(getIconBitmap(controlLocationExit));
                            else
                                mainForm.formControlGroup.drawTriggeredIcon(getIconBitmap(controlLocationExit));
                        }
                        else
                            mainForm.formControlGroup.drawTriggeredIcon(getIconBitmap(controlLocationExit));

                    }
                }

                previousControlLocation = currentControlLocation;
            }
            catch { }
        }

        private void resetIcon()
        {
            try
            {
                bitmapSpotlightCurrent = new Bitmap(bitmapSpotlight.Width, bitmapSpotlight.Height);
                Graphics g = Graphics.FromImage(bitmapSpotlightCurrent);

                //g.CompositingMode = CompositingMode.SourceOver;

                //g.DrawImage(bitmapSpotlight, 0, 0, bitmapSpotlight.Width, bitmapSpotlight.Height);
                g.DrawImage(bitmapSpotlight, 50, 50, 128, 128);

                SetBitmap(bitmapSpotlightCurrent, (byte)255);

                g.Dispose();
            }
            catch { }
        }


        private string getIconFilename(ControlLocations controlLocation)
        {
            string imageFilename = "";
            try
            {
                imageFilename = currentPointable.actions[(int)controlLocation].iconPath;
            }
            catch { }
            return imageFilename;
        }

        private Bitmap getIconBitmap(ControlLocations controlLocation)
        {
            Bitmap bmp = null;
            try
            {
                bmp = mainForm.formControlGroup.bitmapObjects[(int)controlLocation];
            }
            catch { }

            return bmp;
        }

        private void drawIcon(Bitmap bitmap)
        {
            if (bitmap == null) return;

            try
            {
                bitmapSpotlightCurrent = new Bitmap(bitmapSpotlight.Width, bitmapSpotlight.Height);
                Graphics g = Graphics.FromImage(bitmapSpotlightCurrent);

                g.CompositingMode = CompositingMode.SourceOver;

                g.DrawImage(bitmapSpotlight, 0, 0, bitmapSpotlight.Width, bitmapSpotlight.Height);

                Bitmap smallBmp;
                smallBmp = bitmap;// Image.FromFile(imageFilename) as Bitmap;
                smallBmp.MakeTransparent();
                g.DrawImage(smallBmp, 50, 50, 128, 128);
                // smallBmp.Dispose();

                SetBitmap(bitmapSpotlightCurrent, (byte)255);

                g.Dispose();
            }
            catch { }
        }

        private void drawIcon(string imageFilename)
        {
            if (imageFilename == null || imageFilename == "") return;

            try
            {
                bitmapSpotlightCurrent = new Bitmap(bitmapSpotlight.Width, bitmapSpotlight.Height);
                Graphics g = Graphics.FromImage(bitmapSpotlightCurrent);

                g.CompositingMode = CompositingMode.SourceOver;

                g.DrawImage(bitmapSpotlight, 50, 50, 128, 128);

                Bitmap smallBmp;
                smallBmp = Image.FromFile(Form1.iconFolderPath + imageFilename) as Bitmap;
                smallBmp.MakeTransparent();
                g.DrawImage(smallBmp, 50, 50, 128, 128);
                smallBmp.Dispose();

                SetBitmap(bitmapSpotlightCurrent, (byte)255);

                g.Dispose();
            }
            catch { }
        }

        private void launchConfigurationThread()
        {
            try
            {
                Thread th = new Thread(new ThreadStart(mainForm.launchConfiguration));
                th.IsBackground = true;
                th.Start();
            }
            catch { }
        }

        private void launchScreenCalibrationThread()
        {
            try
            {
                Thread th = new Thread(new ThreadStart(mainForm.startCalibrateScreen));
                th.IsBackground = true;
                th.Start();
            }
            catch { }
        }

        private void launchPauseTrackingThread()
        {
            try
            {
                Thread th = new Thread(new ThreadStart(mainForm.pauseResumeTracking));
                th.IsBackground = true;
                th.Start();
            }
            catch { }
        }

        private void launchSystemRestartThread()
        {
            try
            {
                Thread th = new Thread(new ThreadStart(mainForm.systemRestart));
                th.IsBackground = true;
                th.Start();
            }
            catch { }
        }

        private void launchSystemShutdownThread()
        {
            try
            {
                Thread th = new Thread(new ThreadStart(mainForm.systemShutdown));
                th.IsBackground = true;
                th.Start();
            }
            catch { }
        }

        internal void triggerKeyEventNoDelay(byte[] key)
        {
            try
            {
                triggerKeyArray = key;
                
                triggerKeyArrayThreadNoDelay();

                //if (key[0] == (byte)0x12 && key[1] == (byte)0x73)
                //{
                //    mainForm.state.ProcessEvent(LeapProject.Form1.FiniteStateMachine.Events.EndGesture);
                //}
            }
            catch { }
        }

        internal void triggerKeyEvent(byte [] key)
        {
            try
            {
                triggerKeyArray = key;

                Thread th = new Thread(new ThreadStart(triggerKeyArrayThread));
                th.IsBackground = true;
                th.Start();

                if (key[0] == (byte)0x12 && key[1] == (byte)0x73)
                {
                    mainForm.state.ProcessEvent(LeapProject.Form1.FiniteStateMachine.Events.EndGesture);
                }
            }
            catch { }
        }

        private void triggerKeyEvent(byte p)
        {
            try
            {
                triggerKey = p;

                Thread th = new Thread(new ThreadStart(triggerKeyThread));
                th.IsBackground = true;
                th.Start();
            }
            catch { }
        }
        //private void triggerKeyEvent(byte p1, byte p2)
        //{
        //    triggerKey = p1;
        //    triggerKey2 = p2;

        //    Thread th = new Thread(new ThreadStart(triggerKeyThread2Keys));
        //    th.IsBackground = true;
        //    th.Start();
        //}
        //private void triggerKeyThread2Keys()
        //{
        //    keybd_event((byte)triggerKey, 0x45, 0, 0);
        //    keybd_event((byte)triggerKey2, 0x45, 0, 0);
        //    keybd_event((byte)triggerKey2, 0x45, Tools.KEYEVENTF_KEYUP, 0);
        //    keybd_event((byte)triggerKey, 0x45, Tools.KEYEVENTF_KEYUP, 0);
        //}
        private void triggerKeyThread()
        {
            try
            {
                keybd_event((byte)triggerKey, 0x45, 0, 0);
                Thread.Sleep(10);
                keybd_event((byte)triggerKey, 0x45, Tools.KEYEVENTF_KEYUP, 0);
            }
            catch { }
        }


        private void triggerKeyArrayThread()
        {
            try
            {
                this.BeginInvoke((MethodInvoker)delegate()
                {//Control -^  Alt - %  Shift+  Win

                    //SendKeys.Send("%+{NUMLOCK}");
                    //return;
                    if ((triggerKeyArray[0] == (byte)0xA0 || triggerKeyArray[0] == (byte)0xA1 || triggerKeyArray[0] == (byte)0x10) 
                        && triggerKeyArray[1] == (byte)0x23)
                    {
                        SendKeys.Send("+{END}");
                        return;
                    }
                    else if ((triggerKeyArray[0] == (byte)0x11 || triggerKeyArray[0] == (byte)0xA2 || triggerKeyArray[0] == (byte)0xA3) 
                        && (triggerKeyArray[1] == (byte)0xA0 || triggerKeyArray[1] == (byte)0xA1 || triggerKeyArray[1] == (byte)0x10)
                        && triggerKeyArray[2] == (byte)0x23)
                    {
                        SendKeys.Send("^+{END}");
                        return;
                    } 

                    for (int i = 0; i < triggerKeyArray.Length; i++)
                    {
                        if (triggerKeyArray[i] != 0)
                        {
                            byte keyTrigger = (byte)triggerKeyArray[i];
                            if (keyTrigger == (byte)0xA0)
                                keyTrigger = (byte)0x10;

                            keybd_event(keyTrigger, 0x45, 0, 0);
                            Thread.Sleep(10); //10
                        }
                    }

                    for (int i = triggerKeyArray.Length - 1; i >= 0; i--)
                    {
                        if (triggerKeyArray[i] != 0)
                        {
                            byte keyTrigger = (byte)triggerKeyArray[i];
                            if (keyTrigger == (byte)0xA0)
                                keyTrigger = (byte)0x10;
                            Thread.Sleep(10); ///10
                            keybd_event(keyTrigger, 0x45, Tools.KEYEVENTF_KEYUP, 0);
                        }
                    }

                   // SendKeys.Send("%{DOWN}");
                    //SendKeys.Send("+{END}");

                  //  SendKeys.Send("^%{DEL}");
                });
            }
            catch { }
        }

        private void triggerKeyArrayThreadNoDelay()
        {
            try
            {
               // this.BeginInvoke((MethodInvoker)delegate()
               // {//Control -^  Alt - %  Shift+  Win

                    for (int i = 0; i < triggerKeyArray.Length; i++)
                    {
                        if (triggerKeyArray[i] != 0)
                        {
                            byte keyTrigger = (byte)triggerKeyArray[i];
                            if (keyTrigger == (byte)0xA0)
                                keyTrigger = (byte)0x10;

                            keybd_event(keyTrigger, 0x45, 0, 0);
                            Thread.Sleep(1); //10
                        }
                    }

                    for (int i = triggerKeyArray.Length - 1; i >= 0; i--)
                    {
                        if (triggerKeyArray[i] != 0)
                        {
                            byte keyTrigger = (byte)triggerKeyArray[i];
                            if (keyTrigger == (byte)0xA0)
                                keyTrigger = (byte)0x10;
                            Thread.Sleep(1); ///10
                            keybd_event(keyTrigger, 0x45, Tools.KEYEVENTF_KEYUP, 0);
                        }
                    }

                    // SendKeys.Send("%{DOWN}");
                    //SendKeys.Send("+{END}");

                    //  SendKeys.Send("^%{DEL}");
               // });
            }
            catch { }
        }
        //public static void PressKey(char ch, bool press)
        //{
        //    byte vk = WindowsAPI.VkKeyScan(ch);
        //    ushort scanCode = (ushort)WindowsAPI.MapVirtualKey(vk, 0);

        //    if (press)
        //        KeyDown(scanCode);
        //    else
        //        KeyUp(scanCode);
        //}

        //public static void KeyDown(ushort scanCode)
        //{
        //    INPUT[] inputs = new INPUT[1];
        //    inputs[0].type = WindowsAPI.INPUT_KEYBOARD;
        //    inputs[0].ki.dwFlags = 0;
        //    inputs[0].ki.wScan = (ushort)(scanCode & 0xff);

        //    uint intReturn = WindowsAPI.SendInput(1, inputs, System.Runtime.InteropServices.Marshal.SizeOf(inputs[0]));
        //    if (intReturn != 1)
        //    {
        //        throw new Exception("Could not send key: " + scanCode);
        //    }
        //}

        //public static void KeyUp(ushort scanCode)
        //{
        //    INPUT[] inputs = new INPUT[1];
        //    inputs[0].type = WindowsAPI.INPUT_KEYBOARD;
        //    inputs[0].ki.wScan = scanCode;
        //    inputs[0].ki.dwFlags = WindowsAPI.KEYEVENTF_KEYUP;
        //    uint intReturn = WindowsAPI.SendInput(1, inputs, System.Runtime.InteropServices.Marshal.SizeOf(inputs[0]));
        //    if (intReturn != 1)
        //    {
        //        throw new Exception("Could not send key: " + scanCode);
        //    }
        //}

        private void runApplication(string filename, string arguments)
        {
            try
            {
                if (filename == null || filename == "")
                    return;

                triggerFilename = filename;
                triggerFileArgs = arguments;

                Thread th = new Thread(new ThreadStart(runApplicationThread));
                th.IsBackground = true;
                th.Start();
            }
            catch { }
        }

        private void runApplicationThread()
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                string triggerFilenameExpanded = Environment.ExpandEnvironmentVariables(triggerFilename);
                startInfo.FileName = triggerFilenameExpanded;// triggerFilename;//"C:\\Program Files (x86)\\EventGhost\\EventGhost.exe";
                startInfo.Arguments = triggerFileArgs;// "-event NextChannel";
                Process.Start(startInfo);
            }
            catch {};//(Exception error){Debug.WriteLine(error.ToString()); }
        }

        bool currentWindowCursorNormal = true;
        byte currentWindowCursorNormalOpacity = 255;
        internal  void drawWindowCursorAbsolute(int x, int y, bool normalCursor, Vector3 fingerPosition)
        {
            try
            {
                    Form1.pointedScreenPositionUniversal.X = (int)x;
                    Form1.pointedScreenPositionUniversal.Y = (int)y;

                if (currentWindowCursorNormal != normalCursor)
                {
                    currentWindowCursorNormal = normalCursor;
                    if (normalCursor)
                    {
                        setCursorNormal();
                    }
                    else
                    {
                        setCursorMove();
                    }
                }

                if (normalCursor)
                {
                    double YoverZ = (fingerPosition.Y / fingerPosition.Z);
                    
                    double fingerAngle = Math.Atan(YoverZ) * 57.3;

                    if (fingerAngle < 0) 
                        fingerAngle = 180 + fingerAngle;
                    //if (YoverZ < 0) 
                    //    YoverZ = -YoverZ;

                    double distanceRatio = 1;
                    double ratio =1;

                    distanceRatio = (fingerPosition.Magnitude - 250) / 200;
                    distanceRatio = 1 - distanceRatio;

                    if (distanceRatio < 0) distanceRatio = 0;
                    if (distanceRatio > 1) distanceRatio = 1;
                   // double opacity = 1;
                    //Console.WriteLine(fingerAngle);
                    if (fingerAngle > 80)
                    {
                        if (fingerAngle > 110)
                            return;

                        ratio = ((Math.Abs(180-fingerAngle) - 75) / 30.0); //30 is the range
                        if (ratio > 1) ratio = 1;
                        if (ratio < 0) ratio = 0;

                    }
                    else if (fingerAngle < 60)
                    {
                        if (fingerAngle < 35)
                            return;

                        ratio = ((fingerAngle - 35) / 30.0); //30 is the range
                        if (ratio > 1) ratio = 1;
                        if (ratio < 0) ratio = 0;

                    }
                    else
                    {

                    }

                    if (mainForm.circleGestureInProgress)
                        ratio = ratio * 0.3;

                    byte opacity = (byte)(ratio * distanceRatio * 255);
                    setCursorNormal(opacity);

                    //if (fingerPosition.Z < 0)
                    //{//nearer to monitor
                    //    if (fingerAngle < 70)
                    //        return;
                        
                    //    double ratio = ((fingerAngle - 75) / 30.0); //30 is the range

                    //    //if (fingerPosition.Magnitude > 250)
                    //    double distanceRatio = (fingerPosition.Magnitude - 250) / 200;

                    //    if (distanceRatio < 0) distanceRatio = 0;
                    //    if (distanceRatio > 1) distanceRatio = 1;

                    //    distanceRatio = 1 - distanceRatio;

                    //    if (ratio > 1) ratio = 1;
                    //    if (ratio < 0) ratio = 0;

                    //    byte opacity = (byte)(ratio * distanceRatio * 255);
                    //    setCursorNormal(opacity);
                    //}
                    //else
                    //{//further out
                    //    if (fingerAngle < 35)
                    //        return;

                    //    double ratio = ((fingerAngle - 35) / 45.0); //30 is the range
                    //    if (ratio > 1) ratio = 1;
                    //    if (ratio < 0) ratio = 0;

                    //    double distanceRatio = (fingerPosition.Magnitude - 250) / 200;
                    //    if (distanceRatio < 0) distanceRatio = 0;
                    //    if (distanceRatio > 1) distanceRatio = 1;

                    //    distanceRatio = 1 - distanceRatio;

                    //    byte opacity = (byte)(ratio * distanceRatio * 255);
                    //    setCursorNormal(opacity);
                    //}

                    //if (fingerPosition.Y < 5 * (-fingerPosition.Z))
                    //    return;
                    //if (fingerPosition.Y < 0.3 * fingerPosition.Z)
                    //    return;
                }

                windowCursorForm.Left = x - windowCursorForm.Width / 2;
                windowCursorForm.Top = y - windowCursorForm.Height / 2;

                Tools.SetWindowPos(this.windowCursorForm.Handle, Tools.HWND.TOPMOST, 0, 0, 0, 0, Tools.SetWindowPosFlags.IgnoreMove | Tools.SetWindowPosFlags.IgnoreResize | Tools.SetWindowPosFlags.DoNotActivate);
               // if (!mainForm.isOwnApplicationFocus)
               //     windowCursorForm.TopMost = true; //bug
           }
            catch { }
        }

        internal void drawSpotlight(int x, int y, bool show)
        {
            try
            {
                int height = 500; // this.Height;
                int width = 500; //this.Width;
                int changeX = (x - 50) * width / 100;
                int changeY = (50 - y) * height / 100;

                int distance = (int)(Math.Sqrt((x - 50) * (x - 50) + (50 - y) * (50 - y)) * (255 - 100) / 50 + 100);
                if (distance > 255) distance = 255;
                //this.Opacity = distance / 10000.0;
                distance = 255;

                if (!show)
                {
                    resetIcon();
                }
                if (bitmapSpotlightCurrent != null)
                    SetBitmap(bitmapSpotlightCurrent, (byte)distance);
                else
                    SetBitmap(bitmapSpotlight, (byte)distance);

                Point centerOfBall = new Point(mainForm.formControlGroup.Left + mainForm.formControlGroup.Width / 2, mainForm.formControlGroup.Top + mainForm.formControlGroup.Height / 2);
                this.Left = changeX - this.Width / 2 + centerOfBall.X;
                this.Top = changeY - this.Height / 2 + centerOfBall.Y;
                this.shadowForm.Left = -changeX - shadowForm.Width / 2 + centerOfBall.X ;//System.Windows.Forms.Screen.AllScreens[Tools.ScreenNumber].Bounds.X + System.Windows.Forms.Screen.AllScreens[Tools.ScreenNumber].Bounds.Width / 2;
                this.shadowForm.Top = 350 - changeY / 5 - shadowForm.Height / 2 + centerOfBall.Y;//System.Windows.Forms.Screen.AllScreens[Tools.ScreenNumber].Bounds.Y + System.Windows.Forms.Screen.AllScreens[Tools.ScreenNumber].Bounds.Height / 2;

                if (shadowForm.Visible == false && show)
                    shadowForm.Show();

            }
            catch { }
        }

        internal void hideShadow()
        {
            try
            {
                if (shadowForm != null && !shadowForm.IsDisposed)
                    shadowForm.Hide();
            }
            catch { }
        }



        System.Timers.Timer _timerProgressClick;

        System.Timers.Timer _timerProgressClickAction;

        int countProgressClick = 0;
        int countProgressIgnore = 0;

        internal void startCursorProgress()
        {

            countProgressClick = 0;
            countProgressIgnore = 0;
            //todo
            if (_timerProgressClick != null)
            {
                _timerProgressClick.Enabled = false;
                _timerProgressClick = null;
            }

            if (_timerProgressClick == null)
            {
                _timerProgressClick = new System.Timers.Timer(40); //300
                _timerProgressClick.Elapsed += new ElapsedEventHandler(_timerProgressClick_Elapsed);
                _timerProgressClick.Enabled = true;
            }
        }

        internal void endCursorProgress()
        {
            countProgressClick = 0;
            countProgressIgnore = 0;
            //todo
            if (_timerProgressClick != null)
            {
                _timerProgressClick.Enabled = false;
                _timerProgressClick = null;
            }

            setCursorNormal();
        }

        void _timerProgressClick_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                _timerProgressClick.Enabled = false;
                if (windowCursorDropForm.Visible)
                {
                    this.BeginInvoke((MethodInvoker)delegate()
                    {
                        windowCursorDropForm.Hide();
                    });
                }
            }
            catch { }
        }

        void _timerProgressClickAction_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                _timerProgressClickAction.Enabled = false;
                if (windowCursorDropFormClick.Visible)
                {
                    this.BeginInvoke((MethodInvoker)delegate()
                    {
                        windowCursorDropFormClick.Hide();
                    });
                }
                //if (countProgressIgnore++ < 5)
                //    return;

                //countProgressClick++;

                //if (countProgressClick == 9)
                //{
                //    countProgressClick = 0;
                //    _timerProgressClick.Stop();
                //    return;
                //}
                //else
                //{
                //    try
                //    {
                //        this.BeginInvoke((MethodInvoker)delegate()
                //        {
                //            if (countProgressClick > 0 && countProgressClick <= 8)
                //                windowCursorForm.SetBitmapWithOpacity(bitmapWindowCursorProgress[countProgressClick - 1], (byte)255);
                //            currentWindowCursorNormal = true;
                //        });
                //    }
                //    catch (Exception ex) { Debug.WriteLine(ex.ToString()); }
                //}
            }
            catch { }
        }

        private void loadCursorClickProgress()
        {
            try
            {
                bitmapWindowCursorProgress = new Bitmap[8];
                for (int i = 0; i < 8; i++)
                {
                    bitmapWindowCursorProgress[i] = new Bitmap(Form1.executable.GetManifestResourceStream("Pointable.Resources.cursor.Pointable_cursor_progress" + (i + 1).ToString() + ".png"));
                }
            }
            catch { }
        }
    }

    class ShadowForm : PerPixelAlphaForm
    {
        public ShadowForm()
        {
            TopMost = true;
            ShowInTaskbar = false;
        }
        protected override bool ShowWithoutActivation
        {
            get { return true; }
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
    }

    class WindowCursorForm : PerPixelAlphaForm
    {
        public WindowCursorForm()
        {
            //TopMost = true;
            ShowInTaskbar = false;
            this.Text = "Cursor";
        }
        protected override bool ShowWithoutActivation
        {
            get { return true; }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                // turn on WS_EX_TOOLWINDOW style bit
               // cp.ExStyle |= 0x08000000; // 0x80;


                cp.ExStyle |= (int)(
                  ExtendedWindowStyles.WS_EX_NOACTIVATE |
                  ExtendedWindowStyles.WS_EX_TOOLWINDOW);
                return cp;
            }
        }
    }
}
