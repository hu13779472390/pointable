using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;
using System.ComponentModel;
using System.Windows;
using System.IO;
using System.Diagnostics;

namespace LeapProject
{
    class Tools
    {
        public static class HWND
        {
            public static readonly IntPtr
            NOTOPMOST = new IntPtr(-2),
            BROADCAST = new IntPtr(0xffff),
            TOPMOST = new IntPtr(-1),
            TOP = new IntPtr(0),
            BOTTOM = new IntPtr(1);
        }


        [Flags()]
        internal enum SetWindowPosFlags : uint
        {
            /// <summary>If the calling thread and the thread that owns the window are attached to different input queues, 
            /// the system posts the request to the thread that owns the window. This prevents the calling thread from 
            /// blocking its execution while other threads process the request.</summary>
            /// <remarks>SWP_ASYNCWINDOWPOS</remarks>
            AsynchronousWindowPosition = 0x4000,
            /// <summary>Prevents generation of the WM_SYNCPAINT message.</summary>
            /// <remarks>SWP_DEFERERASE</remarks>
            DeferErase = 0x2000,
            /// <summary>Draws a frame (defined in the window's class description) around the window.</summary>
            /// <remarks>SWP_DRAWFRAME</remarks>
            DrawFrame = 0x0020,
            /// <summary>Applies new frame styles set using the SetWindowLong function. Sends a WM_NCCALCSIZE message to 
            /// the window, even if the window's size is not being changed. If this flag is not specified, WM_NCCALCSIZE 
            /// is sent only when the window's size is being changed.</summary>
            /// <remarks>SWP_FRAMECHANGED</remarks>
            FrameChanged = 0x0020,
            /// <summary>Hides the window.</summary>
            /// <remarks>SWP_HIDEWINDOW</remarks>
            HideWindow = 0x0080,
            /// <summary>Does not activate the window. If this flag is not set, the window is activated and moved to the 
            /// top of either the topmost or non-topmost group (depending on the setting of the hWndInsertAfter 
            /// parameter).</summary>
            /// <remarks>SWP_NOACTIVATE</remarks>
            DoNotActivate = 0x0010,
            /// <summary>Discards the entire contents of the client area. If this flag is not specified, the valid 
            /// contents of the client area are saved and copied back into the client area after the window is sized or 
            /// repositioned.</summary>
            /// <remarks>SWP_NOCOPYBITS</remarks>
            DoNotCopyBits = 0x0100,
            /// <summary>Retains the current position (ignores X and Y parameters).</summary>
            /// <remarks>SWP_NOMOVE</remarks>
            IgnoreMove = 0x0002,
            /// <summary>Does not change the owner window's position in the Z order.</summary>
            /// <remarks>SWP_NOOWNERZORDER</remarks>
            DoNotChangeOwnerZOrder = 0x0200,
            /// <summary>Does not redraw changes. If this flag is set, no repainting of any kind occurs. This applies to 
            /// the client area, the nonclient area (including the title bar and scroll bars), and any part of the parent 
            /// window uncovered as a result of the window being moved. When this flag is set, the application must 
            /// explicitly invalidate or redraw any parts of the window and parent window that need redrawing.</summary>
            /// <remarks>SWP_NOREDRAW</remarks>
            DoNotRedraw = 0x0008,
            /// <summary>Same as the SWP_NOOWNERZORDER flag.</summary>
            /// <remarks>SWP_NOREPOSITION</remarks>
            DoNotReposition = 0x0200,
            /// <summary>Prevents the window from receiving the WM_WINDOWPOSCHANGING message.</summary>
            /// <remarks>SWP_NOSENDCHANGING</remarks>
            DoNotSendChangingEvent = 0x0400,
            /// <summary>Retains the current size (ignores the cx and cy parameters).</summary>
            /// <remarks>SWP_NOSIZE</remarks>
            IgnoreResize = 0x0001,
            /// <summary>Retains the current Z order (ignores the hWndInsertAfter parameter).</summary>
            /// <remarks>SWP_NOZORDER</remarks>
            IgnoreZOrder = 0x0004,
            /// <summary>Displays the window.</summary>
            /// <remarks>SWP_SHOWWINDOW</remarks>
            ShowWindow = 0x0040,
        }

        internal const int SW_HIDE = 0;
        internal const int SW_SHOWNORMAL = 1;
        internal const int SW_SHOWMINIMIZED = 2;
        internal const int SW_SHOWMAXIMIZED = 3;
        internal const int SW_SHOWNOACTIVATE = 4;
        internal const int SW_RESTORE = 9;
        internal const int SW_SHOWDEFAULT = 10;

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, SetWindowPosFlags uFlags);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        internal static extern IntPtr GetActiveWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        internal static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        private const UInt32 WM_CLOSE = 0x0010;

        internal static void CloseWindow(IntPtr hwnd)
        {
            SendMessage(hwnd, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool IsWindow(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern int GetWindowThreadProcessId(IntPtr handle, out int processId);

        //imports keybd_event function from user32.dll
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void keybd_event(byte bVk, byte bScan, long dwFlags, long dwExtraInfo);

        internal static class NativeMethods
        {
            [DllImport("gdi32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool DeleteObject(IntPtr hObject);
        }

        public static int ScreenNumber = 0;

        public static int TapSensitivity = 3;
        public static int TapRightSensitivity = 3;
        public static int TapLeftSensitivity = 3;

        public static int CursorStabilization = 1;
        public static int CursorDroplet = 1;

        public static bool modeClickLeft = true;
        public static bool modeClickMiddle = true;
        public static bool modeClickRight = true;
        public static bool modeClickAndDrag = false;

        public static bool modeWindowDrag = true;

        public static bool keyModifierModeControl = true;
        public static bool keyModifierModeAlt = true;
        public static bool keyModifierModeShift = true;
        public static bool keyModifierModeEnableTracking = true;

        public static int modeClickS = 0;
       

        public static int ScreenSize = 23;

        public enum ProcessSpecificAccess : uint
        {
            PROCESS_VM_READ = 0x0010,
            PROCESS_VM_WRITE = 0x0020
        }


        [DllImport("psapi.dll")] //Supported under Windows Vista and Windows Server 2008. 
        static extern uint GetModuleFileNameEx(IntPtr hProcess, IntPtr hModule, [Out] StringBuilder lpBaseName,
        [In] [MarshalAs(UnmanagedType.U4)] int nSize);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowThreadProcessId(IntPtr handle, out uint processId);

        internal static string GetProcessPath(IntPtr hwnd)
        {
            try
            {
                uint pid = 0;
                GetWindowThreadProcessId(hwnd, out pid);
                Process proc = Process.GetProcessById((int)pid); //Gets the process by ID. 
                return proc.MainModule.FileName.ToString();    //Returns the path. 
            }
            catch (Exception ex)
            {
                return "";
               // return ex.Message.ToString();
            }
        }

        public static Vector3 getValuesVector(string strKeyword, string[] strKeywordsArray, string[] strValuesArray)
        {
            Vector3 vector = new Vector3();
            int position;

            position = obtainValues(strKeyword + "X", strKeywordsArray);
            if (position != -1)
            {
                vector.X = double.Parse(strValuesArray[position]);
            }
            position = obtainValues(strKeyword + "Y", strKeywordsArray);
            if (position != -1)
            {
                vector.Y = double.Parse(strValuesArray[position]);
            }
            position = obtainValues(strKeyword + "Z", strKeywordsArray);
            if (position != -1)
            {
                vector.Z = double.Parse(strValuesArray[position]);
            }

            return vector;
        }

        public static int obtainValues(string strKeyword, string[] strKeywordsArray)
        {
            try
            {
                for (int i = 0; i < strKeywordsArray.Length; i++)
                {
                    if (strKeywordsArray[i] != null)
                    {
                        if (strKeywordsArray[i].Trim() == strKeyword.Trim())
                        {
                            return i;
                        }
                    }
                }
                return -1;
            }
            catch { return -1; }
        }

        public static void writeVectorValues(System.IO.TextWriter tw, String strName, Vector3 vector)
        {
            tw.WriteLine(strName + "X=" + vector.X.ToString());
            tw.WriteLine(strName + "Y=" + vector.Y.ToString());
            tw.WriteLine(strName + "Z=" + vector.Z.ToString());
        }

        
        public static string DecimalPlaceNoRounding(double d, int decimalPlaces)
        {
            d = d * Math.Pow(10, decimalPlaces);
            d = Math.Truncate(d);
            d = d / Math.Pow(10, decimalPlaces);
            return string.Format("{0:N" + Math.Abs(decimalPlaces) + "}", d);
        }

        public static BitmapSource ToBitmapSource(System.Drawing.Bitmap source)
        {
            BitmapSource bitSrc = null;

            var hBitmap = source.GetHbitmap();

            try
            {
                bitSrc = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    hBitmap,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
            }
            catch (Win32Exception)
            {
                bitSrc = null;
            }
            finally
            {
                NativeMethods.DeleteObject(hBitmap);
            }

            return bitSrc;
        }


        internal static string getOSInfo()
        {
            //Get Operating system information.
            OperatingSystem os = Environment.OSVersion;
            //Get version information about the os.
            Version vs = os.Version;

            //Variable to hold our return value
            string operatingSystem = "";

            if (os.Platform == PlatformID.Win32Windows)
            {
                //This is a pre-NT version of Windows
                switch (vs.Minor)
                {
                    case 0:
                        operatingSystem = "95";
                        break;
                    case 10:
                        if (vs.Revision.ToString() == "2222A")
                            operatingSystem = "98SE";
                        else
                            operatingSystem = "98";
                        break;
                    case 90:
                        operatingSystem = "Me";
                        break;
                    default:
                        break;
                }
            }
            else if (os.Platform == PlatformID.Win32NT)
            {
                switch (vs.Major)
                {
                    case 3:
                        operatingSystem = "NT 3.51";
                        break;
                    case 4:
                        operatingSystem = "NT 4.0";
                        break;
                    case 5:
                        if (vs.Minor == 0)
                            operatingSystem = "2000";
                        else
                            operatingSystem = "XP";
                        break;
                    case 6:
                        if (vs.Minor == 0)
                            operatingSystem = "Vista";
                        else if (vs.Minor == 1)
                            operatingSystem = "7";
                        else if (vs.Minor == 2)
                            operatingSystem = "8";
                        else if (vs.Minor == 3)
                            operatingSystem = "8.1";
                        break;
                    default:
                        break;
                }
            }
            
          //  MessageBox.Show(vs.Major + " " + vs.Minor.ToString() + " " +  vs.Revision.ToString() );
            //Make sure we actually got something in our OS check
            //We don't want to just return " Service Pack 2" or " 32-bit"
            //That information is useless without the OS version.
            if (operatingSystem != "")
            {
                ////Got something.  Let's prepend "Windows" and get more info.
                //operatingSystem = "Windows " + operatingSystem;
                ////See if there's a service pack installed.
                //if (os.ServicePack != "")
                //{
                //    //Append it to the OS name.  i.e. "Windows XP Service Pack 3"
                //    operatingSystem += " " + os.ServicePack;
                //}
                //Append the OS architecture.  i.e. "Windows XP Service Pack 3 32-bit"
                //operatingSystem += " " + getOSArchitecture().ToString() + "-bit";
            }
            //Return the information we've gathered.
            return operatingSystem;
        }

        public const int GCL_HICONSM = -34;
        public const int GCL_HICON = -14;

        public const int ICON_SMALL = 0;
        public const int ICON_BIG = 1;
        public const int ICON_SMALL2 = 2;

        public const int WM_GETICON = 0x7F;

        public static IntPtr GetClassLongPtr(IntPtr hWnd, int nIndex)
        {
            if (IntPtr.Size > 4)
                return GetClassLongPtr64(hWnd, nIndex);
            else
                return new IntPtr(GetClassLongPtr32(hWnd, nIndex));
        }

        [DllImport("user32.dll", EntryPoint = "GetClassLong")]
        public static extern uint GetClassLongPtr32(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "GetClassLongPtr")]
        public static extern IntPtr GetClassLongPtr64(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        
        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = CharSet.Auto)]
        extern static bool DestroyIcon(IntPtr handle);

        public Icon GetAppIcon(IntPtr hwnd)
        {
            Icon icn = null;

            try
            {
                IntPtr iconHandle = SendMessage(hwnd, WM_GETICON, ICON_SMALL2, 0);
                if (iconHandle == IntPtr.Zero)
                    iconHandle = SendMessage(hwnd, WM_GETICON, ICON_SMALL, 0);
                if (iconHandle == IntPtr.Zero)
                    iconHandle = SendMessage(hwnd, WM_GETICON, ICON_BIG, 0);
                if (iconHandle == IntPtr.Zero)
                    iconHandle = GetClassLongPtr(hwnd, GCL_HICON);
                if (iconHandle == IntPtr.Zero)
                    iconHandle = GetClassLongPtr(hwnd, GCL_HICONSM);

                if (iconHandle == IntPtr.Zero)
                    return null;

                icn = Icon.FromHandle(iconHandle);
            }
            catch { }

            return icn;
        }

        public static Bitmap GetBitmapIcon(IntPtr hwnd)
        {
            try
            {
                IntPtr iconHandle = GetClassLongPtr(hwnd, GCL_HICONSM);
                if (iconHandle == IntPtr.Zero)
                    iconHandle = GetClassLongPtr(hwnd, GCL_HICON);
                if (iconHandle == IntPtr.Zero)
                    iconHandle = SendMessage(hwnd, WM_GETICON, ICON_BIG, 0);
                if (iconHandle == IntPtr.Zero)
                    iconHandle = SendMessage(hwnd, WM_GETICON, ICON_SMALL, 0);
                if (iconHandle == IntPtr.Zero)
                    iconHandle = SendMessage(hwnd, WM_GETICON, ICON_SMALL2, 0);

                if (iconHandle == IntPtr.Zero)
                    return null;

                Icon icn = Icon.FromHandle(iconHandle);
                Bitmap bmp = icn.ToBitmap();
                DestroyIcon(icn.Handle);
                return bmp;
            }
            catch { }
            return null;
        }



        #region mouse and keyboard

        [StructLayout(LayoutKind.Sequential)]
        public struct MOUSEINPUT
        {
            public int dx;//4
            public int dy;//4
            public uint mouseData;//4
            public uint dwFlags;//4
            public uint time;//4
            public IntPtr dwExtraInfo;//4
        }
        public struct INPUT
        {
            public int type;
            public MOUSEINPUT mi;
        }


        //declare consts for mouse messages
        public const int INPUT_MOUSE = 0;
        public const int INPUT_KEYBOARD = 1;
        public const int INPUT_HARDWARE = 2;

        public const int MOUSEEVENTF_MOVE = 0x01;
        public const int MOUSEEVENTF_LEFTDOWN = 0x02;
        public const int MOUSEEVENTF_LEFTUP = 0x04;
        public const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        public const int MOUSEEVENTF_RIGHTUP = 0x10;
        public const int MOUSEEVENTF_MIDDLEDOWN = 0x20;
        public const int MOUSEEVENTF_MIDDLEUP = 0x40;
        public const int MOUSEEVENTF_ABSOLUTE = 0x8000;
        //public const int MOUSEEVENTF_RELATIVE = 0x8000;

        public enum VK : ushort
        {               /*
        * Virtual Keys, Standard Set */
            VK_LBUTTON = 0x01,
            VK_RBUTTON = 0x02,
            VK_CANCEL = 0x03,
            VK_MBUTTON = 0x04,    /* NOT contiguous with L & RBUTTON */

            VK_XBUTTON1 = 0x05,    /* NOT contiguous with L & RBUTTON */
            VK_XBUTTON2 = 0x06,    /* NOT contiguous with L & RBUTTON */
            /*
    * 0x07 : unassigned */
            VK_BACK = 0x08,
            VK_TAB = 0x09,

            /*
    * 0x0A - 0x0B : reserved */
            VK_CLEAR = 0x0C,
            VK_RETURN = 0x0D,

            VK_SHIFT = 0x10,
            VK_CONTROL = 0x11,
            VK_MENU = 0x12,
            VK_PAUSE = 0x13,
            VK_CAPITAL = 0x14,

            VK_KANA = 0x15,
            VK_HANGEUL = 0x15,  /* old name - should be here for compatibility */
            VK_HANGUL = 0x15,
            VK_JUNJA = 0x17,
            VK_FINAL = 0x18,
            VK_HANJA = 0x19,
            VK_KANJI = 0x19,

            VK_ESCAPE = 0x1B,

            VK_CONVERT = 0x1C,
            VK_NONCONVERT = 0x1D,
            VK_ACCEPT = 0x1E,
            VK_MODECHANGE = 0x1F,

            VK_SPACE = 0x20,
            VK_PRIOR = 0x21,
            VK_NEXT = 0x22,
            VK_END = 0x23,
            VK_HOME = 0x24,
            VK_LEFT = 0x25,
            VK_UP = 0x26,
            VK_RIGHT = 0x27,
            VK_DOWN = 0x28,
            VK_SELECT = 0x29,
            VK_PRINT = 0x2A,
            VK_EXECUTE = 0x2B,
            VK_SNAPSHOT = 0x2C,
            VK_INSERT = 0x2D,
            VK_DELETE = 0x2E,
            VK_HELP = 0x2F,

            /*
    * VK_0 - VK_9 are the same as ASCII '0' - '9' (0x30 - 0x39) * 0x40 : unassigned * VK_A - VK_Z are the same as ASCII 'A' - 'Z' (0x41 - 0x5A) */
            VK_LWIN = 0x5B,
            VK_RWIN = 0x5C,
            VK_APPS = 0x5D,

            /*
    * 0x5E : reserved */
            VK_SLEEP = 0x5F,

            VK_NUMPAD0 = 0x60,
            VK_NUMPAD1 = 0x61,
            VK_NUMPAD2 = 0x62,
            VK_NUMPAD3 = 0x63,
            VK_NUMPAD4 = 0x64,
            VK_NUMPAD5 = 0x65,
            VK_NUMPAD6 = 0x66,
            VK_NUMPAD7 = 0x67,
            VK_NUMPAD8 = 0x68,
            VK_NUMPAD9 = 0x69,
            VK_MULTIPLY = 0x6A,
            VK_ADD = 0x6B,
            VK_SEPARATOR = 0x6C,
            VK_SUBTRACT = 0x6D,
            VK_DECIMAL = 0x6E,
            VK_DIVIDE = 0x6F,
            VK_F1 = 0x70,
            VK_F2 = 0x71,
            VK_F3 = 0x72,
            VK_F4 = 0x73,
            VK_F5 = 0x74,
            VK_F6 = 0x75,
            VK_F7 = 0x76,
            VK_F8 = 0x77,
            VK_F9 = 0x78,
            VK_F10 = 0x79,
            VK_F11 = 0x7A,
            VK_F12 = 0x7B,
            VK_F13 = 0x7C,
            VK_F14 = 0x7D,
            VK_F15 = 0x7E,
            VK_F16 = 0x7F,
            VK_F17 = 0x80,
            VK_F18 = 0x81,
            VK_F19 = 0x82,
            VK_F20 = 0x83,
            VK_F21 = 0x84,
            VK_F22 = 0x85,
            VK_F23 = 0x86,
            VK_F24 = 0x87,

            /*
    * 0x88 - 0x8F : unassigned */
            VK_NUMLOCK = 0x90,
            VK_SCROLL = 0x91,

            /*
    * VK_L* & VK_R* - left and right Alt, Ctrl and Shift virtual keys. * Used only as parameters to GetAsyncKeyState() and GetKeyState(). * No other API or message will distinguish left and right keys in this way. */
            VK_LSHIFT = 0xA0,
            VK_RSHIFT = 0xA1,
            VK_LCONTROL = 0xA2,
            VK_RCONTROL = 0xA3,
            VK_LMENU = 0xA4,
            VK_RMENU = 0xA5,

            VK_BROWSER_BACK = 0xA6,
            VK_BROWSER_FORWARD = 0xA7,
            VK_BROWSER_REFRESH = 0xA8,
            VK_BROWSER_STOP = 0xA9,
            VK_BROWSER_SEARCH = 0xAA,
            VK_BROWSER_FAVORITES = 0xAB,
            VK_BROWSER_HOME = 0xAC,

            VK_VOLUME_MUTE = 0xAD,
            VK_VOLUME_DOWN = 0xAE,
            VK_VOLUME_UP = 0xAF,
            VK_MEDIA_NEXT_TRACK = 0xB0,
            VK_MEDIA_PREV_TRACK = 0xB1,
            VK_MEDIA_STOP = 0xB2,
            VK_MEDIA_PLAY_PAUSE = 0xB3,
            VK_LAUNCH_MAIL = 0xB4,
            VK_LAUNCH_MEDIA_SELECT = 0xB5,
            VK_LAUNCH_APP1 = 0xB6,
            VK_LAUNCH_APP2 = 0xB7,

            /*
    * 0xB8 - 0xB9 : reserved */
            VK_OEM_1 = 0xBA,   // ';:' for US
            VK_OEM_PLUS = 0xBB,   // '+' any country
            VK_OEM_COMMA = 0xBC,   // ',' any country
            VK_OEM_MINUS = 0xBD,   // '-' any country
            VK_OEM_PERIOD = 0xBE,   // '.' any country
            VK_OEM_2 = 0xBF,   // '/?' for US
            VK_OEM_3 = 0xC0,   // '`~' for US

            /*
    * 0xC1 - 0xD7 : reserved */
            /*
    * 0xD8 - 0xDA : unassigned */
            VK_OEM_4 = 0xDB,  //  '[{' for US
            VK_OEM_5 = 0xDC,  //  '\|' for US
            VK_OEM_6 = 0xDD,  //  ']}' for US
            VK_OEM_7 = 0xDE,  //  ''"' for US
            VK_OEM_8 = 0xDF

            /*
    * 0xE0 : reserved */

        }

        //declare consts for key scan codes
        public const byte VK_TAB = 0x09;
        public const byte VK_MENU = 0x12;
        public const byte VK_SPACE = 0x20;
        public const byte VK_RETURN = 0x0D;
        public const byte VK_LEFT = 0x25;
        public const byte VK_UP = 0x26;
        public const byte VK_RIGHT = 0x27;
        public const byte VK_DOWN = 0x28;
        public const int KEYEVENTF_EXTENDEDKEY = 0x01;
        public const int KEYEVENTF_KEYUP = 0x02;



        //laser pointer mode
        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int CopyIcon(int hIcon);
        [DllImport("user32", EntryPoint = "LoadCursorFromFileA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int LoadCursorFromFile(string lpFileName);
        [DllImport("user32", EntryPoint = "LoadCursorA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int LoadCursor(int hInstance, int lpCursorName);
        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int SetCursor(int hCursor);
        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int SetSystemCursor(int hcur, int id);
        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int GetCursor();


        private const int OCR_NORMAL = 32512;
       // private static System.IO.MemoryStream cursorNormalMemoryStream = new System.IO.MemoryStream("Pointable.Resources.cursor.Cursor_Windows.cur");
       // private static System.IO.MemoryStream cursorNoneMemoryStream = new System.IO.MemoryStream("Pointable.Resources.cursor.Cursor_Windows_None.cur");

        private static bool cursorIsNormal = true;
        internal static void changeCursorWindowsNormal()
        {
            try
            {
                if (!cursorIsNormal)
                {
                    //Console.WriteLine("Change to Normal");
                    string myCursorFile = Form1.settingsFolderPath + @"cursor_windows.cur";
                    string myCursorFile2 = Form1.settingsFolderPath + @"cursor_windows_point.cur";

                    if (!File.Exists(myCursorFile) || !File.Exists(myCursorFile2))
                        return;

                    int newhcurs = LoadCursorFromFile(myCursorFile); //store reference
                    SetSystemCursor(newhcurs, OCR_NORMAL);

                    cursorIsNormal = true;
                }
            }
            catch { }
        }

        internal static void changeCursorWindowsNone()
        {
            try
            {
               
                if (cursorIsNormal)
                {
                    //Console.WriteLine("Change to None");

                    string myCursorFile = Form1.settingsFolderPath + @"cursor_windows_point.cur";
                    string myCursorFile2 = Form1.settingsFolderPath + @"cursor_windows.cur";
                    
                    if (!File.Exists(myCursorFile) || !File.Exists(myCursorFile2))
                        return;

                    int newhcurs = LoadCursorFromFile(myCursorFile);
                    SetSystemCursor(newhcurs, OCR_NORMAL);

                    cursorIsNormal = false;
                }
            }
            catch { }
        }

        #endregion
    }
    
    public class ImageTools
    {
        public static void FastBlur(Bitmap SourceImage, int radius)
        {
            try
            {
                var rct = new Rectangle(0, 0, SourceImage.Width, SourceImage.Height);
                var dest = new int[rct.Width * rct.Height];
                var source = new int[rct.Width * rct.Height];
                var bits = SourceImage.LockBits(rct, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                Marshal.Copy(bits.Scan0, source, 0, source.Length);
                SourceImage.UnlockBits(bits);

                if (radius < 1) return;

                int w = rct.Width;
                int h = rct.Height;
                int wm = w - 1;
                int hm = h - 1;
                int wh = w * h;
                int div = radius + radius + 1;
                var r = new int[wh];
                var g = new int[wh];
                var b = new int[wh];
                int rsum, gsum, bsum, x, y, i, p1, p2, yi;
                var vmin = new int[max(w, h)];
                var vmax = new int[max(w, h)];

                var dv = new int[256 * div];
                for (i = 0; i < 256 * div; i++)
                {
                    dv[i] = (i / div);
                }

                int yw = yi = 0;

                for (y = 0; y < h; y++)
                { // blur horizontal
                    rsum = gsum = bsum = 0;
                    for (i = -radius; i <= radius; i++)
                    {
                        int p = source[yi + min(wm, max(i, 0))];
                        rsum += (p & 0xff0000) >> 16;
                        gsum += (p & 0x00ff00) >> 8;
                        bsum += p & 0x0000ff;
                    }
                    for (x = 0; x < w; x++)
                    {

                        r[yi] = dv[rsum];
                        g[yi] = dv[gsum];
                        b[yi] = dv[bsum];

                        if (y == 0)
                        {
                            vmin[x] = min(x + radius + 1, wm);
                            vmax[x] = max(x - radius, 0);
                        }
                        p1 = source[yw + vmin[x]];
                        p2 = source[yw + vmax[x]];

                        rsum += ((p1 & 0xff0000) - (p2 & 0xff0000)) >> 16;
                        gsum += ((p1 & 0x00ff00) - (p2 & 0x00ff00)) >> 8;
                        bsum += (p1 & 0x0000ff) - (p2 & 0x0000ff);
                        yi++;
                    }
                    yw += w;
                }

                for (x = 0; x < w; x++)
                { // blur vertical
                    rsum = gsum = bsum = 0;
                    int yp = -radius * w;
                    for (i = -radius; i <= radius; i++)
                    {
                        yi = max(0, yp) + x;
                        rsum += r[yi];
                        gsum += g[yi];
                        bsum += b[yi];
                        yp += w;
                    }
                    yi = x;
                    for (y = 0; y < h; y++)
                    {
                        dest[yi] = (int)(0xff000000u | (uint)(dv[rsum] << 16) | (uint)(dv[gsum] << 8) | (uint)dv[bsum]);
                        if (x == 0)
                        {
                            vmin[y] = min(y + radius + 1, hm) * w;
                            vmax[y] = max(y - radius, 0) * w;
                        }
                        p1 = x + vmin[y];
                        p2 = x + vmax[y];

                        rsum += r[p1] - r[p2];
                        gsum += g[p1] - g[p2];
                        bsum += b[p1] - b[p2];

                        yi += w;
                    }
                }

                // copy back to image
                var bits2 = SourceImage.LockBits(rct, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                Marshal.Copy(dest, 0, bits2.Scan0, dest.Length);
                SourceImage.UnlockBits(bits);

            }
            catch { }
        }

        private static int min(int a, int b) { return Math.Min(a, b); }
        private static int max(int a, int b) { return Math.Max(a, b); }

        
    }
}
