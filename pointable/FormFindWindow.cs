#region Author
/// <author>
/// Written by Jörg Bausch
/// www.modulesoft.de
/// </author>
#endregion

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

using System.Runtime.InteropServices;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;
using Microsoft.Win32;

namespace LeapProject
{
	/// <summary>
	/// class MainForm
	/// </summary>
	public class FormFindWindow : System.Windows.Forms.Form
	{
        [DllImport("gdi32.dll")]
        static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

        private const int HORZSIZE = 4;
        private const int VERTSIZE = 6;
        private const double MM_TO_INCH_CONVERSION_FACTOR = 25.4;

        private System.Windows.Forms.PictureBox pictureBox;
        private System.ComponentModel.IContainer components;
		private System.Windows.Forms.Label labelTest;
		private System.Windows.Forms.GroupBox groupBoxWindow;
        private System.Windows.Forms.TextBox textBoxCaption;
		private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.TextBox textBoxClassName;
        private Button buttonOK;
        private Button buttonCancel;
		private System.Windows.Forms.ImageList imageList;

		public enum GetSystem_Metrics : int
		{
			SM_CXBORDER     = 5,
			SM_CXFULLSCREEN = 16,
			SM_CYFULLSCREEN = 17
		}
		[DllImport("user32.dll")]
		public static extern int GetSystemMetrics(int smIndex);

		private IntPtr LastWindow = IntPtr.Zero;

		/// <summary>
		/// Paint a rect into the given window
		/// </summary>
		/// <param name="window"></param>
		static void ShowInvertRectTracker(IntPtr window)
		{
			if(window != IntPtr.Zero)
			{
				// get the coordinates from the window on the screen
				Rectangle WindowRect = ApiWrapper.Window.GetWindowRect(window);
				// get the window's device context
				IntPtr dc = ApiWrapper.Window.GetWindowDC(window);

				// Create an inverse pen that is the size of the window border
				ApiWrapper.Gdi.SetROP2(dc, (int)ApiWrapper.Gdi.RopMode.R2_NOT);

				Color color = Color.FromArgb(0, 255, 0);
				IntPtr Pen = ApiWrapper.Gdi.CreatePen((int)ApiWrapper.Gdi.PenStyles.PS_INSIDEFRAME, 3 * GetSystemMetrics((int)GetSystem_Metrics.SM_CXBORDER), (uint)color.ToArgb());

				// Draw the rectangle around the window
				IntPtr OldPen = ApiWrapper.Gdi.SelectObject(dc, Pen);
				IntPtr OldBrush = ApiWrapper.Gdi.SelectObject(dc, ApiWrapper.Gdi.GetStockObject((int)ApiWrapper.Gdi.StockObjects.NULL_BRUSH));
				ApiWrapper.Gdi.Rectangle(dc, 0, 0, WindowRect.Width, WindowRect.Height);

				ApiWrapper.Gdi.SelectObject(dc, OldBrush);
				ApiWrapper.Gdi.SelectObject(dc, OldPen);

				//release the device context, and destroy the pen
				ApiWrapper.Window.ReleaseDC(window, dc);
				ApiWrapper.Gdi.DeleteObject(Pen);
			}
		}

        static IntPtr ParentWindowFromPoint(Point point)
        {
            IntPtr WindowPoint = ApiWrapper.Window.WindowFromPoint(point);
            if (WindowPoint != IntPtr.Zero)
            {
                if (checkWindowType(WindowPoint) == WindowType.DragTestWindow)
                    return WindowPoint;
            }
            IntPtr WindowRoot = ApiWrapper.Window.GetAncestor(WindowPoint, 2);

            return WindowRoot;
        }
        ///// <summary>
        ///// return the window from the given point
        ///// </summary>
        ///// <param name="point"></param>
        ///// <returns>if return == IntPtr.Zero no window was found</returns>
        //static IntPtr ChildWindowFromPoint(Point point)
        //{
        //    IntPtr WindowPoint = ApiWrapper.Window.WindowFromPoint(point);
        //    if (WindowPoint == IntPtr.Zero)
        //        return IntPtr.Zero;

        //    if (ApiWrapper.Window.ScreenToClient(WindowPoint, ref point) == false)
        //        throw new Exception("ScreenToClient failed");

        //    IntPtr Window = ApiWrapper.Window.ChildWindowFromPointEx(WindowPoint, point, 0);
        //    if (Window == IntPtr.Zero)
        //        return WindowPoint;

        //    if(ApiWrapper.Window.ClientToScreen(WindowPoint, ref point) == false)
        //        throw new Exception("ClientToScreen failed");

        //    if(ApiWrapper.Window.IsChild(ApiWrapper.Window.GetParent(Window),Window) == false) 
        //        return Window;

        //    // create a list to hold all childs under the point
        //    ArrayList WindowList = new ArrayList();
        //    while (Window != IntPtr.Zero)
        //    {
        //        Rectangle rect = ApiWrapper.Window.GetWindowRect(Window);
        //        if(rect.Contains(point))
        //            WindowList.Add(Window);
        //        Window = ApiWrapper.Window.GetWindow(Window, (uint)ApiWrapper.Window.GetWindow_Cmd.GW_HWNDNEXT);
        //    }
			
        //    // search for the smallest window in the list
        //    int MinPixel = GetSystemMetrics((int)GetSystem_Metrics.SM_CXFULLSCREEN) * GetSystemMetrics((int)GetSystem_Metrics.SM_CYFULLSCREEN);
        //    for (int i = 0; i < WindowList.Count; ++i)
        //    {
        //        Rectangle rect = ApiWrapper.Window.GetWindowRect( (IntPtr)WindowList[i] );
        //        int ChildPixel = rect.Width * rect.Height;
        //        if (ChildPixel < MinPixel)
        //        {
        //            MinPixel = ChildPixel;
        //            Window = (IntPtr)WindowList[i];
        //        }
        //    }
        //    return Window;
        //}

        Form1 mainForm;
        string windowTitle;
        string windowClass;
        private IntPtr windowHandle = IntPtr.Zero;

		public FormFindWindow(Form1 mform1)
		{
            mainForm = mform1;
			InitializeComponent();
		}

		private void buttonClose_Click(object sender, System.EventArgs e)
		{
			this.Close();	// close this window
		}

		/// <summary>
		/// clear used resources.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		/// <summary>
		/// Show informations about the given window
		/// </summary>
		/// <param name="window"></param>
		private void DisplayWindowInfo(IntPtr window)
		{
            try
            {
                if (window == IntPtr.Zero)
                {
                    // reset all edit box
                    textBoxCaption.Text = "";
                    textBoxClassName.Text = "";
                }
                else
                {

                    // Class
                    StringBuilder ClassName = new StringBuilder(256);
                    int ret = ApiWrapper.Window.GetClassName(window, ClassName, ClassName.Capacity);
                    windowClass = ClassName.ToString();
                    textBoxClassName.Text = windowClass;

                    windowTitle = ApiWrapper.Window.GetWindowText(window);
                    textBoxCaption.Text = windowTitle;

                    windowHandle = window;
                }
            }
            catch { }
		}

		#region Generated Form-Designer Code
		/// <summary>
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormFindWindow));
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.labelTest = new System.Windows.Forms.Label();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.groupBoxWindow = new System.Windows.Forms.GroupBox();
            this.textBoxClassName = new System.Windows.Forms.TextBox();
            this.textBoxCaption = new System.Windows.Forms.TextBox();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.groupBoxWindow.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox
            // 
            this.pictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox.Image")));
            this.pictureBox.Location = new System.Drawing.Point(150, 59);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(31, 28);
            this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox.TabIndex = 0;
            this.pictureBox.TabStop = false;
            this.pictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseMove);
            this.pictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseDown);
            this.pictureBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseUp);
            // 
            // labelTest
            // 
            this.labelTest.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelTest.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTest.Location = new System.Drawing.Point(8, 8);
            this.labelTest.Name = "labelTest";
            this.labelTest.Size = new System.Drawing.Size(320, 48);
            this.labelTest.TabIndex = 1;
            this.labelTest.Text = "Drag the icon below over the desired window, then release the mouse button.";
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "");
            this.imageList.Images.SetKeyName(1, "");
            // 
            // groupBoxWindow
            // 
            this.groupBoxWindow.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxWindow.Controls.Add(this.textBoxClassName);
            this.groupBoxWindow.Controls.Add(this.textBoxCaption);
            this.groupBoxWindow.Location = new System.Drawing.Point(10, 93);
            this.groupBoxWindow.Name = "groupBoxWindow";
            this.groupBoxWindow.Size = new System.Drawing.Size(312, 81);
            this.groupBoxWindow.TabIndex = 11;
            this.groupBoxWindow.TabStop = false;
            this.groupBoxWindow.Text = "Window";
            // 
            // textBoxClassName
            // 
            this.textBoxClassName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxClassName.Enabled = false;
            this.textBoxClassName.Location = new System.Drawing.Point(10, 53);
            this.textBoxClassName.Name = "textBoxClassName";
            this.textBoxClassName.ReadOnly = true;
            this.textBoxClassName.Size = new System.Drawing.Size(290, 20);
            this.textBoxClassName.TabIndex = 16;
            this.textBoxClassName.TabStop = false;
            // 
            // textBoxCaption
            // 
            this.textBoxCaption.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxCaption.Enabled = false;
            this.textBoxCaption.Location = new System.Drawing.Point(10, 24);
            this.textBoxCaption.Name = "textBoxCaption";
            this.textBoxCaption.ReadOnly = true;
            this.textBoxCaption.Size = new System.Drawing.Size(290, 20);
            this.textBoxCaption.TabIndex = 10;
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(67, 180);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(97, 29);
            this.buttonOK.TabIndex = 14;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(170, 180);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(97, 29);
            this.buttonCancel.TabIndex = 13;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // FormFindWindow
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(330, 218);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.groupBoxWindow);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.labelTest);
            this.Controls.Add(this.pictureBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormFindWindow";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Attach to Window";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.FormFindWindow_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormFindWindow_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.groupBoxWindow.ResumeLayout(false);
            this.groupBoxWindow.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private void pictureBox_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
            try
            {
                if (e.Button == MouseButtons.Left)
                {
                    System.IO.Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("Pointable.Resources.cursor.findcursor.cur");
                    // Cursor = new Cursor(GetType(), "findcursor.cur");
                    Cursor = new Cursor(stream);
                    pictureBox.Image = imageList.Images[0];
                }
            }
            catch { }
		}

		private void pictureBox_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
            try
            {
                if (Cursor != Cursors.Default)
                {
                    IntPtr FoundWindow = ParentWindowFromPoint(Cursor.Position);

                    // not this application
                    if (Control.FromHandle(FoundWindow) == null)
                    {
                        if (FoundWindow != LastWindow)
                        {
                            // clear old window
                            //ShowInvertRectTracker(LastWindow);
                            // set new window
                            LastWindow = FoundWindow;
                            // paint new window
                            //ShowInvertRectTracker(LastWindow);
                        }
                        DisplayWindowInfo(LastWindow);
                    }
                }
            }
            catch { }
		}

        internal static IntPtr findPointedWindow(Point point)
        {
            IntPtr FoundWindow = ParentWindowFromPoint(point);
            
            if (FoundWindow != IntPtr.Zero && FoundWindow != null)
            {
                StringBuilder ClassName = new StringBuilder(256);
                int ret = ApiWrapper.Window.GetClassName(FoundWindow, ClassName, ClassName.Capacity);
                string strClass = ClassName.ToString();
                //if (strClass == "Progman" || strClass == "Shell_TrayWnd" || strClass == "Button" || strClass == "Control")
                //    return IntPtr.Zero;
                if (strClass == "Button" || strClass == "Control" || strClass == "Draw")
                    return IntPtr.Zero;

                string windowText = ApiWrapper.Window.GetWindowText(FoundWindow);
                if (windowText == "Control" || windowText == "Cursor" || windowText =="Draw")
                {
                    return IntPtr.Zero;
                }


               // Console.WriteLine(strClass + " xx " + windowText);

                if (strClass == "" && windowText == "")
                    return IntPtr.Zero;

                //if (strClass == "CabinetWClass" || strClass == "") //explorer
                //{                    
                //    return IntPtr.Zero;
                //}

                if (strClass.Substring(0, 1) == "#" && windowText == "")
                {
                    return IntPtr.Zero;
                }

                try
                {
                    if (windowText == "" && !(strClass.Contains("Shell_") && strClass.Contains("TrayWnd")))
                    {
                        Rectangle WindowRect = ApiWrapper.Window.GetWindowRect(FoundWindow);

                        if (WindowRect.Height < 700)
                            return IntPtr.Zero;
                    }
                }
                catch { }

            }            
            // show global mouse cursor
           // labelCursor.Text = "Cursor: " + Cursor.Position.ToString();
            return FoundWindow;
        }

        internal enum WindowType { SpecialWindow, TaskSwitcher, Tutorial, DragTestWindow, Browser, BrowserVideo, BrowserFullScreen, Spotify, Word, Excel, Powerpoint, LightRoom, vlc, Wmplayer, Others, None }


        internal static string getWindowClass(IntPtr window)
        {
            if (window == IntPtr.Zero)
                return "";

            try
            {
                StringBuilder ClassName = new StringBuilder(256);
                int ret = ApiWrapper.Window.GetClassName(window, ClassName, ClassName.Capacity);
                string strClass = ClassName.ToString();
                if (strClass.Contains("VLC "))
                    strClass = "QWidget";

                if (strClass == "QWidget")
                {
                    // Rect
                    Rectangle WindowRect = ApiWrapper.Window.GetWindowRect(window);

                    foreach (Screen screen in Screen.AllScreens)
                    {
                        Rectangle screenBounds = screen.Bounds;

                        if (screenBounds.Equals(WindowRect))
                        {
                            return "QWidgetFullScreen";
                            //  break;
                        }
                    }
                }

                return strClass;
            }
            catch { }

            return "";
        }
        internal static string getWindowTitle(IntPtr window)
        {
            if (window == IntPtr.Zero)
                return "";
            

            string windowTitle = ApiWrapper.Window.GetWindowText(window);

            return windowTitle;
        }

        internal static bool isDesktop(IntPtr window)
        {
              StringBuilder ClassName = new StringBuilder(256);
                int ret = ApiWrapper.Window.GetClassName(window, ClassName, ClassName.Capacity);
                string strClass = ClassName.ToString();

            if (strClass == "Progman" || strClass == "WorkerW")
            {
                return true;
            }

            return false;
        }

        internal static WindowType checkWindowType(IntPtr window)
        {
            try
            {
                if (window == IntPtr.Zero)
                    return WindowType.None;

                StringBuilder ClassName = new StringBuilder(256);

                int ret = ApiWrapper.Window.GetClassName(window, ClassName, ClassName.Capacity);
                string strClass = ClassName.ToString();



                if (strClass == "Flip3D" || strClass.Length > 12 && strClass.Substring(0, 12) == "TaskSwitcher")// strClass == "TaskSwitcherWnd" || strClass == "TaskSwitcherOverlayWnd" ||
                    return WindowType.TaskSwitcher;



                string windowTitle = ApiWrapper.Window.GetWindowText(window);

                if (windowTitle == "Pointable Tutorial")
                    return WindowType.Tutorial;

               // Console.WriteLine(strClass + " xx " + windowTitle);

                if (strClass == "Windows.UI.Core.CoreWindow")
                {
                   // Console.WriteLine(windowTitle);
                    if (windowTitle.Length > 17 && windowTitle.Substring(windowTitle.Length - 17) == ("Internet Explorer"))
                    {
                        return WindowType.Browser;
                    }
                }

                if ((strClass.Contains("Shell_") && strClass.Contains("TrayWnd")) || strClass == "Progman" || strClass == "Windows.UI.Core.CoreWindow" || 
                    strClass == "ImmersiveLauncher" || strClass == "ImmersiveSwitchList" || strClass == "Shell_CharmWindow" || strClass =="WorkerW")
                    return WindowType.SpecialWindow;
                if (windowTitle == "Charm Bar")
                    return WindowType.SpecialWindow;
                //else if (windowTitle == "Drag Test Window")
                //    return WindowType.DragTestWindow;

                if (strClass == "")
                {
                    //string windowTitle = ApiWrapper.Window.GetWindowText(window);
                    if (windowTitle == "")
                        return WindowType.SpecialWindow;
                }

                if (strClass == "IEFrame" || strClass == "MozillaWindowClass" || strClass.Contains("Chrome_WidgetWin_") || strClass.Contains("OperaWindowClass"))
                {
                    //string windowTitle = ApiWrapper.Window.GetWindowText(window);
                    if (windowTitle.Contains("- YouTube") || windowTitle.Contains("Vimeo") || windowTitle.Contains("Netflix") ||
                        windowTitle.Contains("Hulu") || windowTitle.Contains("Dailymotion"))
                        return WindowType.BrowserVideo;
                    else
                        return WindowType.Browser;
                }


                if (strClass == "SpotifyMainWindow")
                    return WindowType.Spotify;
                if (strClass == "OpusApp")
                    return WindowType.Word;
                if (strClass == "XLMAIN")
                    return WindowType.Excel;
                if (strClass == "PPTFrameClass" || strClass == "screenClass")
                    return WindowType.Powerpoint;
                if (strClass == "AgWinMainFrame")
                    return WindowType.LightRoom;
                if (strClass == "QWidget" || strClass.Contains("VLC "))
                    return WindowType.vlc;
                if (strClass == "WMPlayerApp")
                    return WindowType.Wmplayer;

                if (strClass == "ShockwaveFlashFullScreen" || strClass == "Chrome_RenderWidgetHostHWND")
                    return WindowType.BrowserFullScreen;


                return WindowType.Others;
            }
            catch { }

            return WindowType.Others;
        }

		private void pictureBox_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
            try
            {
                if (Cursor != Cursors.Default)
                {
                    // reset all done things from mouse_down and mouse_move ...
                    //ShowInvertRectTracker(LastWindow);
                    LastWindow = IntPtr.Zero;

                    Cursor = Cursors.Default;
                    pictureBox.Image = imageList.Images[1];
                }
            }
            catch { }
		}


        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                var hDC = Graphics.FromHwnd(this.Handle).GetHdc();
                int horizontalSizeInMilliMeters = GetDeviceCaps(hDC, HORZSIZE);
                double horizontalSizeInInches = horizontalSizeInMilliMeters / MM_TO_INCH_CONVERSION_FACTOR;
                int vertivalSizeInMilliMeters = GetDeviceCaps(hDC, VERTSIZE);
                double verticalSizeInInches = vertivalSizeInMilliMeters / MM_TO_INCH_CONVERSION_FACTOR;

                Debug.WriteLine("Horizontal = " + horizontalSizeInMilliMeters);
                Debug.WriteLine("Vertical = " + vertivalSizeInMilliMeters);
            }
            catch { }
        }

        private void FormFindWindow_Load(object sender, EventArgs e)
        {

        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            try
            {
                //setbusyicon
                this.Cursor = Cursors.WaitCursor;
                mainForm.updateAttachToWindowData(windowTitle, windowClass, windowHandle);
                this.Cursor = Cursors.Default;

                this.Close();
            }
            catch { }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            try
            {
                this.Close();
            }
            catch { }
        }

        private void FormFindWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                mainForm.endAttachToWindow();
            }
            catch { }
        }

        //public static List<PointF> GetDesktopMonitors()
        //{
        //    List<PointF> screenSizeList = new List<PointF>();

        //    //////////////////////////////////////////////////////////////////////////

        //    try
        //    {
        //        ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\WMI", "SELECT * FROM WmiMonitorID");

        //        foreach (ManagementObject queryObj in searcher.Get())
        //        {
        //            Debug.WriteLine("-----------------------------------------------");
        //            Debug.WriteLine("WmiMonitorID instance");
        //            Debug.WriteLine("----------------");
        //            //   Debug.WriteLine("Active: {0}", queryObj["Active"]);
        //            //Debug.WriteLine("InstanceName: {0}", queryObj["InstanceName"]);
        //            //   dynamic snid = queryObj["SerialNumberID"];
        //            //   Debug.WriteLine("SerialNumberID: (length) {0}", snid.Length);
        //           // Debug.WriteLine("YearOfManufacture: {0}", queryObj["YearOfManufacture"]);

        //            /*
        //            foreach (PropertyData data in queryObj.Properties)
        //            {
        //                Debug.WriteLine(data.Value.ToString());
        //            }
        //            */

        //            dynamic code = queryObj["ProductCodeID"];
        //            string pcid = "";
        //            for (int i = 0; i < code.Length; i++)
        //            {
        //                pcid = pcid + Char.ConvertFromUtf32(code[i]);
        //                //pcid = pcid +code[i].ToString("X4");
        //            }
        //            Debug.WriteLine("ProductCodeID: " + pcid);


        //            int xSize = 0;
        //            int ySize = 0;
        //            string PNP = queryObj["InstanceName"].ToString();
        //            PNP = PNP.Substring(0, PNP.Length - 2);  // remove _0
        //            if (PNP != null && PNP.Length > 0)
        //            {
        //                string displayKey = "SYSTEM\\CurrentControlSet\\Enum\\";
        //                string strSubDevice = displayKey + PNP + "\\" + "Device Parameters\\";
        //                // e.g.
        //                // SYSTEM\CurrentControlSet\Enum\DISPLAY\LEN40A0\4&1144c54c&0&UID67568640\Device Parameters
        //                // SYSTEM\CurrentControlSet\Enum\DISPLAY\LGD0335\4&1144c54c&0&12345678&00&02\Device Parameters
        //                //
        //                Debug.WriteLine("Register Path: " + strSubDevice);

        //                RegistryKey regKey = Registry.LocalMachine.OpenSubKey(strSubDevice, false);
        //                if (regKey != null)
        //                {
        //                    if (regKey.GetValueKind("edid") == RegistryValueKind.Binary)
        //                    {
        //                        Debug.WriteLine("read edid");

        //                        byte[] edid = (byte[])regKey.GetValue("edid");

        //                        const int edid_x_size_in_mm = 21;
        //                        const int edid_y_size_in_mm = 22;
        //                        xSize = ((int)edid[edid_x_size_in_mm] * 10);
        //                        ySize = ((int)edid[edid_y_size_in_mm] * 10);
        //                        Debug.WriteLine("Screen size cx=" + xSize.ToString() + ", cy=" + ySize.ToString());
        //                    }
        //                    regKey.Close();
        //                }
        //            }

        //            Debug.WriteLine("-----------------------------------------------");

        //            PointF pt = new PointF();
        //            pt.X = (float)xSize;
        //            pt.Y = (float)ySize;

        //            screenSizeList.Add(pt);
        //        }
        //    }
        //    catch (ManagementException e)
        //    {
        //        Debug.WriteLine("An error occurred while querying for WMI data: " + e.Message);
        //    }

        //    return screenSizeList;
        //}
	}
}
