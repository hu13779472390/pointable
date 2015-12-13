using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
//using Vector3;
using Leap;
using System.Windows;
using System.IO;
using System.Threading;
using System.Runtime.InteropServices;
using System.Timers;
using PointableUI;
using Action = PointableUI.Action;
using System.Xml.Serialization;
using Microsoft.Win32;
using System.Diagnostics;
using Point = System.Drawing.Point;
using Screen = System.Windows.Forms.Screen;
using System.Windows.Forms.Design;
using System.Drawing.IconLib;
using Pointable.Properties;
using System.Reflection;
using Utilities;
using Pointable;



namespace LeapProject
{
    public partial class Form1 : Form, LeapEventListener
    {
        #region key hook
        globalKeyboardHook gkh = new globalKeyboardHook();

        #endregion


        int dropletMinDuration = 300;
        int dropletMaxVelocity = 100;
        int dropletDurationBetween = 400;
        internal void setCursorDropletData()
        {//TODO
            try
            {
                switch (Tools.CursorDroplet)
                {
                    case 0:
                        dropletMinDuration = 400;
                        dropletMaxVelocity = 120;
                        dropletDurationBetween = 500;
                        break;
                    case 1:
                        dropletMinDuration = 300;
                        dropletMaxVelocity = 100;
                        dropletDurationBetween = 400;
                        break;
                    case 2:
                        dropletMinDuration = 200;
                        dropletMaxVelocity = 70;
                        dropletDurationBetween = 350;
                        break;
                    case 3:
                        dropletMinDuration = 150;
                        dropletMaxVelocity = 50;
                        dropletDurationBetween = 200;
                        break;
                }
            }
            catch { }
        }


        #region smoothing

        const int MAX_SMOOTHING = 40;
        //Smoothing
        double[] smoothingDirectionX = new double[MAX_SMOOTHING]; //store the past coordinates for smoothing
        double[] smoothingDirectionY = new double[MAX_SMOOTHING];
        double[] smoothingDirectionZ = new double[MAX_SMOOTHING];
        double[] smoothingDirectionVelocity = new double[MAX_SMOOTHING];
        Vector3[] smoothingDirectionPosition = new Vector3[MAX_SMOOTHING];
        //double[] smoothingVelocity = new double[MAX_SMOOTHING];


        int smoothingDirectionCount = 0; //track the number of points stored
        private int smoothingDirectionPoints = 13;//1,2 is 13
        private bool smoothingMode = true; //set the smoothing mode

        internal void setSmoothingData()
        {
            try
            {
                switch (Tools.CursorStabilization)
                {
                    case 0:
                        smoothingDirectionPoints = 6;
                        smoothingPositionPoints = 6;
                        break;
                    case 1:
                        smoothingDirectionPoints = 13;
                        smoothingPositionPoints = 12;
                        break;
                    case 2:
                        smoothingDirectionPoints = 18;
                        smoothingPositionPoints = 14;
                        break;
                    case 3:
                        smoothingDirectionPoints = 17;
                        smoothingPositionPoints = 16;
                        break;
                    case 5:
                        smoothingDirectionPoints = 22;
                        smoothingPositionPoints = 18;
                        break;
                    case 6:
                        smoothingDirectionPoints = 30;
                        smoothingPositionPoints = 20;
                        break;
                }

                resetSmoothingDirection();
                resetSmoothingPosition();
            }
            catch { }
        }



        private void resetSmoothingDirection() //reset smoothing data
        {
            try
            {
                smoothingDirectionCount = 0;
                for (int i = 0; i < MAX_SMOOTHING; i++)
                {
                    smoothingDirectionX[i] = 0;
                    smoothingDirectionY[i] = 0;
                    smoothingDirectionZ[i] = 0;
                    smoothingDirectionVelocity[i] = 0;
                    
                    //smoothingDirectionPosition[i].X = 0;
                    //smoothingDirectionPosition[i].Y = 0;
                    //smoothingDirectionPosition[i].Z = 0 ;

                   // Console.WriteLine("reset");
                }
                smoothingDirectionLockSet = false;
            }
            catch { }
        }

        Vector3 smoothingDirectionLock;
        Vector3 smoothingPositionLock;
        bool smoothingDirectionLockSet = false;

        private Vector3 smoothingDirection(double rawX, double rawY, double rawZ, Vector3 rawVelocityData)
        {
            return smoothingDirection(rawX, rawY, rawZ, rawVelocityData, new Vector3());
        }

        private Vector3 smoothingDirection(double rawX, double rawY, double rawZ, Vector3 rawVelocityData, Vector3 fingerPosition)
        {
            try
            {

                double rawVelocity = rawVelocityData.Magnitude * rawVelocityData.Magnitude * rawVelocityData.Magnitude;
                //double sumX = rawX * rawVelocity, sumY = rawY * rawVelocity, sumZ = rawZ * rawVelocity, sumVelocity = rawVelocity; //initialize the sum
                double sumX = 0, sumY = 0, sumZ = 0, sumVelocity = 0; //initialize the sum
                double smoothX, smoothY, smoothZ; //store the averaged values
                smoothingDirectionCount += 1; //track number of stored values

                if (smoothingDirectionCount > smoothingDirectionPoints)
                {
                    smoothingDirectionCount = smoothingDirectionPoints; //max stored points determine by user
                }


                for (int i = 0; i < smoothingDirectionPoints - 1; i++)
                {
                    smoothingDirectionX[i] = smoothingDirectionX[i + 1]; //shift stored points
                    smoothingDirectionY[i] = smoothingDirectionY[i + 1];
                    smoothingDirectionZ[i] = smoothingDirectionZ[i + 1];
                    smoothingDirectionVelocity[i] = smoothingDirectionVelocity[i + 1];

                    //smoothingDirectionPosition[i].X = smoothingDirectionPosition[i + 1].X;
                    //smoothingDirectionPosition[i].Y = smoothingDirectionPosition[i + 1].Y;
                    //smoothingDirectionPosition[i].Z = smoothingDirectionPosition[i + 1].Z;
                }
                
                smoothingDirectionX[smoothingDirectionPoints - 1] = rawX; //add new points to last position
                smoothingDirectionY[smoothingDirectionPoints - 1] = rawY;
                smoothingDirectionZ[smoothingDirectionPoints - 1] = rawZ;
                smoothingDirectionVelocity[smoothingDirectionPoints - 1] = rawVelocity;

                //smoothingDirectionPosition[smoothingDirectionPoints - 1].X = fingerPosition.X;
                //smoothingDirectionPosition[smoothingDirectionPoints - 1].Y = fingerPosition.Y;
                //smoothingDirectionPosition[smoothingDirectionPoints - 1].Z = fingerPosition.Z;

                    //if velocity more take less points
                   // if (i < 1000 / rawVelocity)        

                int startSum = 0; //average all
               // if (rawVelocityData > 0)
                {
                    for (int i = startSum; i < smoothingDirectionPoints; i++)
                    {
                        sumX += smoothingDirectionX[i] * smoothingDirectionVelocity[i] * (i + 1); //adding sum
                        sumY += smoothingDirectionY[i] * smoothingDirectionVelocity[i] * (i + 1);
                        sumZ += smoothingDirectionZ[i] * smoothingDirectionVelocity[i] * (i + 1);
                        sumVelocity += smoothingDirectionVelocity[i] * (i + 1);
                    }
                    smoothX = sumX / sumVelocity;// (smoothingCount); //calculate average
                    smoothY = sumY / sumVelocity;//(smoothingCount);
                    smoothZ = sumZ / sumVelocity;//(smoothingCount);
                }
                //else
                //{ // very slow
                //    startSum = 0;
                //    for (int i = startSum; i < smoothingDirectionPoints; i++)
                //    {
                //        if (Vector3.computeDistance(fingerPosition, smoothingDirectionPosition[i]) < 5)
                //        {
                //            sumX += smoothingDirectionX[i];// *smoothingDirectionVelocity[i] * (i + 1); //adding sum
                //            sumY += smoothingDirectionY[i];// *smoothingDirectionVelocity[i] * (i + 1);
                //            sumZ += smoothingDirectionZ[i];// *smoothingDirectionVelocity[i] * (i + 1);
                //            sumVelocity += 1;// smoothingDirectionVelocity[i] * (i + 1);
                //        }
                //    }
                //    smoothX = sumX / sumVelocity;// (smoothingCount); //calculate average
                //    smoothY = sumY / sumVelocity;//(smoothingCount);
                //    smoothZ = sumZ / sumVelocity;//(smoothingCount);
                //}


               // Console.WriteLine(rawVelocityData.ToString());

                if (fingerPosition.Magnitude > 150 && smoothingDirectionLockSet && state.State == FiniteStateMachine.States.WindowAware)//Math.Abs(rawVelocityData.Y) < 150 && smoothingDirectionLockSet) //150
                {
                    double multiplier = 0.0035;// 0.0041

                    //Vector3 withoutYComponent = new Vector3(smoothX, 0, smoothZ);
                    //withoutYComponent.Normalize();

                    //double yValue = (fingerPosition.Y - smoothingPositionLock.Y) * multiplier + smoothingDirectionLock.Y;;
                    //Vector3 yComponentOnly = new Vecftor3(0, yValue,0);
                    //yComponentOnly.Normalize();

                    //Vector3 preciseDirection = withoutYComponent + yComponentOnly;
                    //preciseDirection.Normalize();
                    double ratioDivider = 100;
                    double ratioRelative = (fingerPosition.Magnitude-150)/150*100;
                    if (ratioRelative > 100) ratioRelative = 100;
                    if (ratioRelative < 0) ratioRelative = 0;
                    double ratioAbsolute = ratioDivider - ratioRelative;



                    Vector3 preciseDirection = new Vector3();
                    if (fingerPosition.Magnitude < 250)
                    {
                        preciseDirection.X = smoothX;//(ratioAbsolute * smoothX + ratioRelative * ((fingerPosition.X - smoothingPositionLock.X) * multiplier + smoothingDirectionLock.X)) / ratioDivider; //smoothX
                        smoothingDirectionLock.X = smoothX;
                        smoothingPositionLock.X = fingerPosition.X;
                    }
                    else
                    {
                        double ratioRelativeX = (fingerPosition.Magnitude - 250) / 100 * 100;
                        if (ratioRelativeX > 100) ratioRelativeX = 100;
                        if (ratioRelativeX < 0) ratioRelativeX = 0;
                        double ratioAbsoluteX = ratioDivider - ratioRelativeX;

                        preciseDirection.X = (ratioAbsoluteX * smoothX + ratioRelativeX * ((fingerPosition.X - smoothingPositionLock.X) * multiplier*2/3 + smoothingDirectionLock.X)) / ratioDivider; //smoothX

                    }
                    preciseDirection.Y = (ratioAbsolute  *smoothY + ratioRelative * ((fingerPosition.Y - smoothingPositionLock.Y) * multiplier + smoothingDirectionLock.Y)) / ratioDivider;
                    preciseDirection.Z = (ratioAbsolute * smoothZ + ratioRelative * ((fingerPosition.Z - smoothingPositionLock.Z) * multiplier + smoothingDirectionLock.Z)) / ratioDivider;

                   // preciseDirection.Normalize();

                    //smoothingDirectionX[smoothingDirectionPoints - 1] = preciseDirection.X; //add new points to last position
                    //smoothingDirectionY[smoothingDirectionPoints - 1] = preciseDirection.Y;
                    //smoothingDirectionZ[smoothingDirectionPoints - 1] = preciseDirection.Z;

                    //preciseDirection.
                    return preciseDirection;
                }
                else if (fingerPosition.Z < 0 && smoothingDirectionLockSet)
                {
                    double multiplier = 0.0035;// 0.0041

                    Vector3 preciseDirection = new Vector3();
                    preciseDirection.X = smoothX;// (10 * ((fingerPosition.X - smoothingPositionLock.X) * multiplier + smoothingDirectionLock.X)) / 10; //smoothX
                    preciseDirection.Y = (0*smoothY + 20 * ((fingerPosition.Y - smoothingPositionLock.Y) * multiplier + smoothingDirectionLock.Y)) / 20;
                    preciseDirection.Z = (3 * ((fingerPosition.Z - smoothingPositionLock.Z) * multiplier + smoothingDirectionLock.Z)) / 3;

                   // preciseDirection.Normalize();

                   // smoothingDirectionX[smoothingDirectionPoints - 1] = preciseDirection.X; //add new points to last position
                   // smoothingDirectionY[smoothingDirectionPoints - 1] = preciseDirection.Y;
                   // smoothingDirectionZ[smoothingDirectionPoints - 1] = preciseDirection.Z;
                    return preciseDirection;

                }


                smoothingDirectionLockSet = true;
                smoothingDirectionLock.X = smoothX; //must keep updating depending on stable direction data
                smoothingDirectionLock.Y = smoothY;
                smoothingDirectionLock.Z = smoothZ;

                smoothingPositionLock.X = fingerPosition.X;
                smoothingPositionLock.Y = fingerPosition.Y;
                smoothingPositionLock.Z = fingerPosition.Z;

                return new Vector3(smoothX, smoothY, smoothZ);
            }
            catch { }
            return new Vector3();
        }


        //smoothing tip position
        double[] smoothingPositionX = new double[MAX_SMOOTHING]; //store the past coordinates for smoothing
        double[] smoothingPositionY = new double[MAX_SMOOTHING];
        double[] smoothingPositionZ = new double[MAX_SMOOTHING];
        double[] smoothingPositionVelocity = new double[MAX_SMOOTHING];
        //double[] smoothingVelocity = new double[MAX_SMOOTHING];


        int smoothingPositionCount = 0; //track the number of points stored
        private int smoothingPositionPoints = 12;//
        private void resetSmoothingPosition() //reset smoothing data
        {
            try
            {
                smoothingPositionCount = 0;
                for (int i = 0; i < MAX_SMOOTHING; i++)
                {
                    smoothingPositionX[i] = 0;
                    smoothingPositionY[i] = 0;
                    smoothingPositionZ[i] = 0;
                    smoothingPositionVelocity[i] = 0;
                }
            }
            catch { }
        }

        private Vector3 smoothingPosition(double rawX, double rawY, double rawZ, double rawVelocity)
        {
            try
            {
                rawVelocity = rawVelocity * rawVelocity;
                //double sumX = rawX * rawVelocity, sumY = rawY * rawVelocity, sumZ = rawZ * rawVelocity, sumVelocity = rawVelocity; //initialize the sum
                double sumX = 0, sumY = 0, sumZ = 0, sumVelocity = 0; //initialize the sum
                double smoothX, smoothY, smoothZ; //store the averaged values
                smoothingPositionCount += 1; //track number of stored values

                if (smoothingPositionCount > smoothingPositionPoints)
                {
                    smoothingPositionCount = smoothingPositionPoints; //max stored points determine by user
                }

                for (int i = 0; i < smoothingPositionPoints - 1; i++)
                {
                    smoothingPositionX[i] = smoothingPositionX[i + 1]; //shift stored points
                    smoothingPositionY[i] = smoothingPositionY[i + 1];
                    smoothingPositionZ[i] = smoothingPositionZ[i + 1];
                    smoothingPositionVelocity[i] = smoothingPositionVelocity[i + 1];
                }

                smoothingPositionX[smoothingPositionPoints - 1] = rawX; //add new points to last position
                smoothingPositionY[smoothingPositionPoints - 1] = rawY;
                smoothingPositionZ[smoothingPositionPoints - 1] = rawZ;
                smoothingPositionVelocity[smoothingPositionPoints - 1] = rawVelocity;

                //if velocity more take less points
                // if (i < 1000 / rawVelocity)                
                for (int i = 0; i < smoothingPositionPoints; i++)
                {
                    sumX += smoothingPositionX[i] * smoothingPositionVelocity[i] * (i + 1); //adding sum
                    sumY += smoothingPositionY[i] * smoothingPositionVelocity[i] * (i + 1);
                    sumZ += smoothingPositionZ[i] * smoothingPositionVelocity[i] * (i + 1);
                    sumVelocity += smoothingPositionVelocity[i] * (i + 1);
                }

                smoothX = sumX / sumVelocity;// (smoothingCount); //calculate average
                smoothY = sumY / sumVelocity;//(smoothingCount);
                smoothZ = sumZ / sumVelocity;//(smoothingCount);

                return new Vector3(smoothX, smoothY, smoothZ);
            }
            catch { }
            return new Vector3();
        }
        //smoothing tip position



        double[] smoothingScroll = new double[MAX_SMOOTHING];

        int smoothingScrollCount = 0; //track the number of points stored
        private int smoothScrollPoints = 7; //store the number of points to average out

        private void resetScrollSmoothing() //reset smoothing data
        {
            try
            {
                smoothingScrollCount = 0;
                for (int i = 0; i < MAX_SMOOTHING; i++)
                {
                    smoothingScroll[i] = 0;
                }
            }
            catch { }
        }
        private int smoothingScrollData(int rawX)
        {
            try
            {
                double sumX = rawX; //initialize the sum
                double smoothX; //store the averaged values
                smoothingScrollCount += 1; //track number of stored values

                if (smoothingScrollCount > smoothScrollPoints)
                {
                    smoothingScrollCount = smoothScrollPoints; //max stored points determine by user
                }

                for (int i = 0; i < smoothScrollPoints - 1; i++)
                {
                    smoothingScroll[i] = smoothingScroll[i + 1]; //shift stored points

                    sumX += smoothingScroll[i]; //adding sum
                }

                smoothingScroll[smoothScrollPoints - 1] = rawX; 

                smoothX = sumX / (smoothingScrollCount);

                return (int)smoothX;
            }
            catch { }
            return 1;
        }

        #endregion



        private static WINDOWPLACEMENT GetPlacement(IntPtr hwnd)
        {
            WINDOWPLACEMENT placement = new WINDOWPLACEMENT();
            placement.length = Marshal.SizeOf(placement);
            GetWindowPlacement(hwnd, ref placement);
            return placement;
        }

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetWindowPlacement(
            IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        internal struct WINDOWPLACEMENT
        {
            public int length;
            public int flags;
            public ShowWindowCommands showCmd;
            public System.Drawing.Point ptMinPosition;
            public System.Drawing.Point ptMaxPosition;
            public System.Drawing.Rectangle rcNormalPosition;
        }       

        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        internal static extern IntPtr SetFocus(IntPtr hwnd);
        internal enum ShowWindowCommands : int
        {
            Hide = 0,
            Normal = 1,
            Minimized = 2,
            Maximized = 3,
        }
        public static class HWND
        {
            public static readonly IntPtr
            NOTOPMOST = new IntPtr(-2),
            BROADCAST = new IntPtr(0xffff),
            TOPMOST = new IntPtr(-1),
            TOP = new IntPtr(0),
            BOTTOM = new IntPtr(1);
        }

        #region desktop
        private const int SW_HIDE = 0;
        private const int SW_SHOWNORMAL = 1;
        private const int SW_SHOWMINIMIZED = 2;
        private const int SW_SHOWMAXIMIZED = 3;
        private const int SW_SHOWNOACTIVATE = 4;
        private const int SW_RESTORE = 9;
        private const int SW_SHOWDEFAULT = 10;  

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, SetWindowPosFlags uFlags);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetWindowRect(IntPtr hWnd, ref RECT Rect);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int Width, int Height, bool Repaint);


        [Flags()]
        private enum SetWindowPosFlags : uint
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

        #endregion

        // The path to the key where Windows looks for startup applications
        RegistryKey rkApp;// = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);


        // define the key code
        public const int MOUSEEVENTF_WHEEL = 0x0800;
        public const int MOUSEEVENTF_HWHEEL = 0x1000;

        //import dll method
        [DllImport("user32.dll")]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, uint dwExtraInfo);

        //use dll method for vertical scroll and works fine
        public static void VScrollWheel(int steps)
        { mouse_event(MOUSEEVENTF_WHEEL, 0, 0, (uint)steps, 0); }

        //use dll method for horizontal scroll and no response
        public static void HScrollWheel(int steps)
        { mouse_event(MOUSEEVENTF_HWHEEL, 0, 0, (uint)steps, 0); }

        //for firing mouse and keyboard event
        [DllImport("user32.dll", SetLastError = true)]
        static extern uint SendInput(uint nInputs, Tools.INPUT[] pInputs, int cbSize);

        private double THRESHOLD = 200; //30cm
        private const int NUMBEROBJECTS = 10;
        
       
        Vector3[] objectCoordinate = new Vector3[NUMBEROBJECTS];
        String[] objectNames = new String[NUMBEROBJECTS];
       // Vector3[] pointedObjectCoordinate;

        Vector3[] objectCoordA1 = new Vector3[NUMBEROBJECTS]; //1st line
        Vector3[] objectCoordA2 = new Vector3[NUMBEROBJECTS];

        Vector3[] objectCoordB1 = new Vector3[NUMBEROBJECTS]; //2nd line
        Vector3[] objectCoordB2 = new Vector3[NUMBEROBJECTS];

        //Vector3[] fingertipCoordinate = new Vector3[2];
        //Vector3[] fingertipCoordinateOut = new Vector3[2]; //2 users
        int objectPointedIndex;

        internal Vector3 screenNormal = new Vector3(0.0422684382341971, 0.152597225103874, 0.987384153214867); //pointing outwards
        Vector3 horizontalAxis = new Vector3(1,0,0);
        Vector3 verticalAxis = new Vector3(0,1,0);

        internal Vector3[] screenCorner = new Vector3[4];
        Vector3[] screenLineA = new Vector3[8];
        Vector3[] screenLineB = new Vector3[8];
        Vector3 screenCenter = new Vector3();
        double screenPhysicalWidth = 51;
        double screenPhysicalHeight = 29;
        public int screenHeight = 1080;
        public int screenWidth = 1920;

        int[] objectStatus = new int[NUMBEROBJECTS];

        int[] clientReferenceObject = new int[10];

        const int STATUS_OFF = 0;
        const int STATUS_ON = 1;
        const int STATUS_TARGET = 2;

        Vector3[] fingertipCoordinate = new Vector3[2];
        Vector3[] fingertipCoordinateOut = new Vector3[2];

        Vector3[] initialFingertipCoordinate = new Vector3[2];
        Vector3[] initialFingertipCoordinateOut = new Vector3[2];

        bool palmFound = false;

        Vector3 palmCoordinates;
        Vector3 palmCoordinatesOut;
        //Vector3 palmDirection;

        Vector3 initialPalmCoordinates;
        Vector3 initialPalmDirection;

        Vector3 fingertipVelocity;
        Vector3 vScreen;
        PointableList fingers;
        Hand hand;

        double currentX, currentY; //current position screen pointed

        FormControl formControl;
        internal FormControlGroup formControlGroup;
        static Form1 thisForm;
        LeapListener listener ;
        Controller controller;

        internal FiniteStateMachine state;

        FormSplash formSplash;

        bool isWindows8 = false;
        internal static bool isWindows81 = false;

        private void showSplashScreen()
        {
            formSplash = new FormSplash();
            formSplash.Show();
        }

        internal static bool showSplash = true;

        public Form1()
        {
            if (showSplash)
                showSplashScreen();

            InitializeComponent();
            //this.WindowState = FormWindowState.Minimized;
            //this.Visible = false;
            this.Height = 0;
            this.Width = 0;
            this.Hide();

            thisForm = this;

            try
            {
                executable = System.Reflection.Assembly.GetExecutingAssembly();

                string executableName = System.Windows.Forms.Application.ExecutablePath;
                FileInfo executableFileInfo = new FileInfo(executableName);
                exeDirectory = executableFileInfo.DirectoryName;

                iconFolderPathDefault = exeDirectory + @"\DefaultIcons\";
                pointableFolderPathDefault = exeDirectory + @"\DefaultPointables\";

                string windir = Environment.SystemDirectory; // C:\windows\system32
                string windrive = Path.GetPathRoot(Environment.SystemDirectory);
                iconFolderPath = windrive + iconFolderPath;
                pointableFolderPath = windrive + pointableFolderPath;
                settingsFolderPath = windrive + settingsFolderPath;
            }
            catch { }
            //TODO check folders

            try
            {
                int numberOfScreens = System.Windows.Forms.Screen.AllScreens.Length;

                if (numberOfScreens == 1)
                {
                    screen2ToolStripMenuItem.Visible = false;
                    screen3ToolStripMenuItem.Visible = false;
                }
                else if (numberOfScreens == 2)
                {
                    screen3ToolStripMenuItem.Visible = false;
                }
            }
            catch { }


            try
            {
                rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
        
            }
            catch { }
            checkFirstRun();


            Microsoft.Win32.SystemEvents.DisplaySettingsChanged += new EventHandler(ScreenHandler);
        }

        private void ScreenHandler(object sender, EventArgs e)
        {
            BeginInvoke((MethodInvoker)delegate()
            {
                try
                {
                    restartApp();
                    //this.TopMost = true;
                    //DialogResult dr = System.Windows.Forms.MessageBox.Show("Display changed detected. Restart Pointable now?",
                    //  "Pointable", MessageBoxButtons.YesNo);
                    //switch (dr)
                    //{
                    //    case DialogResult.Yes:
                    //        restartApp();
                    //        break;
                    //    case DialogResult.No: 
                    //        break;
                    //}


                    // System.Windows.Forms.Application.Exit();

                }
                catch { }
            });
        }

        public void restartApp()
        {
            try
            {
                //saveSettingsData(exeDirectory + settingsFolder + "\\" + fileSettings);
                //appRestart = true;

                // Restart the app
               // System.Diagnostics.Process.Start(System.Windows.Forms.Application.ExecutablePath, "-multiple");


                ProcessStartInfo startInfo = new ProcessStartInfo();
              //  string triggerFilenameExpanded = Environment.ExpandEnvironmentVariables(triggerFilename);
                startInfo.FileName = System.Windows.Forms.Application.ExecutablePath;// triggerFilename;//"C:\\Program Files (x86)\\EventGhost\\EventGhost.exe";
                startInfo.Arguments = "-multiple";// "-event NextChannel";
                Process.Start(startInfo);


                System.Windows.Forms.Application.Exit();
            }
            catch { }
        }

        private void checkLeapConnection()
        {
            Thread.Sleep(1500);

            try
            {
                if (controller != null && controller.IsConnected)
                {
                    BeginInvoke((MethodInvoker)delegate()
                    {
                        try
                        {
                            notifyIconSysTray.ShowBalloonTip(3000);
                        }
                        catch { }
                    });
                }
                else
                {
                    leapNotConnected();
                }

                setControllerConfiguration();

                if (controller.PolicyFlags != Controller.PolicyFlag.POLICYBACKGROUNDFRAMES)
                {
                 //   this.Focus();
                    BeginInvoke((MethodInvoker)delegate()
                    {
                        try
                        {
                            this.TopMost = true;
                            System.Windows.MessageBox.Show("Set Allow Background Apps from the Leap Motion Control Panel before starting Pointable.", "Pointable");
                            try
                            {
                                //start help
                                ProcessStartInfo startInfo = new ProcessStartInfo();
                                startInfo.FileName = "http://www.pointable.net/allow-background-apps"; //C:\\Program Files (x86)\\EventGhost\\EventGhost.exe";
                                startInfo.Arguments = "";// "-event NextChannel";
                                Process.Start(startInfo);

                            }
                            catch { }

                           // System.Windows.Forms.Application.Exit();

                        }
                        catch { }
                    });
                }

                if (firstRun)

                    BeginInvoke((MethodInvoker)delegate()
                    {
                        try
                        {
                            startTutorial();
                        }
                        catch { }
                    });
            }
            catch { }
        }

        internal void resetControllerConfiguration()
        {
            try
            {
                float keyTapMinDownVelocity, keyTapHistorySeconds, keyTapMinDistance;
                
                keyTapMinDownVelocity = 50.0f;
                keyTapHistorySeconds = 0.1f;
                keyTapMinDistance = 5.0f;

                if (controller.Config.SetFloat("Gesture.KeyTap.MinDownVelocity", keyTapMinDownVelocity) &&
        controller.Config.SetFloat("Gesture.KeyTap.HistorySeconds", keyTapHistorySeconds) &&
        controller.Config.SetFloat("Gesture.KeyTap.MinDistance", keyTapMinDistance))
                    controller.Config.Save();


               //  System.Windows.Forms.MessageBox.Show("test");
            }
            catch { }
        }

        internal void setControllerConfiguration()
        {
            //return;
            //https://developer.leapmotion.com/documentation/Languages/Java/API/classcom_1_1leapmotion_1_1leap_1_1_key_tap_gesture.html
            //if (controller.Config.SetFloat("Gesture.KeyTap.MinDownVelocity", 20.0f) &&
            //    controller.Config.SetFloat("Gesture.KeyTap.HistorySeconds", .2f) &&
            //    controller.Config.SetFloat("Gesture.KeyTap.MinDistance", 10.0f))
            //    controller.Config.Save();

            try
            {
                float keyTapMinDownVelocity, keyTapHistorySeconds, keyTapMinDistance;
                float screenTapMinDownVelocity, screenTapHistorySeconds, screenTapMinDistance;

                keyTapMinDownVelocity = 50.0f;
                keyTapHistorySeconds = 0.1f;
                keyTapMinDistance = 3.0f;

                screenTapMinDownVelocity = 15.0f;
                screenTapHistorySeconds = .1f;
                screenTapMinDistance = 3.0f;

                //Console.WriteLine(Tools.TapSensitivity);

                switch (Tools.TapSensitivity)
                {
                    case 0:
                        keyTapMinDownVelocity = 50.0f;
                        keyTapHistorySeconds = 0.1f;
                        keyTapMinDistance = 10.0f;//3

                        screenTapMinDownVelocity = 15.0f;
                        screenTapHistorySeconds = .1f;
                        screenTapMinDistance = 3.0f;
                        break;
                    case 1:
                        keyTapMinDownVelocity = 40.0f;
                        keyTapHistorySeconds = 0.15f;
                        keyTapMinDistance = 10.0f; //5

                        screenTapMinDownVelocity = 10.0f;
                        screenTapHistorySeconds = .1f;
                        screenTapMinDistance = 3.0f;
                        break;
                    case 2:
                        keyTapMinDownVelocity = 30.0f;
                        keyTapHistorySeconds = 0.2f;
                        keyTapMinDistance = 10.0f; //3

                        screenTapMinDownVelocity = 7.0f;
                        screenTapHistorySeconds = .2f;
                        screenTapMinDistance = 3.0f;
                        break;
                    case 3: //half
                        keyTapMinDownVelocity = 20.0f;
                        keyTapHistorySeconds = 0.2f;
                        keyTapMinDistance = 15.0f; //5

                        screenTapMinDownVelocity = 5.0f;
                        screenTapHistorySeconds = .2f;
                        screenTapMinDistance = 3.0f;
                        break;
                    case 4:
                        keyTapMinDownVelocity = 10.0f;
                        keyTapHistorySeconds = 0.3f;
                        keyTapMinDistance = 15.0f; //10

                        screenTapMinDownVelocity = 3.0f;
                        screenTapHistorySeconds = .3f;
                        screenTapMinDistance = 3.0f;
                        break;
                    case 5:
                        keyTapMinDownVelocity = 5.0f;
                        keyTapHistorySeconds = 0.3f;
                        keyTapMinDistance = 15.0f; //10

                        screenTapMinDownVelocity = 2.0f;
                        screenTapHistorySeconds = .4f;
                        screenTapMinDistance = 3.0f;
                        break;
                }


                if (controller.Config.SetFloat("Gesture.KeyTap.MinDownVelocity", keyTapMinDownVelocity) &&
                    controller.Config.SetFloat("Gesture.KeyTap.HistorySeconds", keyTapHistorySeconds) &&
                    controller.Config.SetFloat("Gesture.KeyTap.MinDistance", keyTapMinDistance))
                    controller.Config.Save();

                //controller.Config.Save();
                if (controller.Config.SetFloat("Gesture.ScreenTap.MinForwardVelocity", screenTapMinDownVelocity) &&
                    controller.Config.SetFloat("Gesture.ScreenTap.HistorySeconds", screenTapHistorySeconds) &&
                    controller.Config.SetFloat("Gesture.ScreenTap.MinDistance", screenTapMinDistance))
                    controller.Config.Save();
            }
            catch { }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            checkDate();

            if (!firstRun)
            {
                loadSettingsData();
                checkVersion();
            }

            generatePointableData();
            
            try
            {
                initializeScreenCorners();

                formControl = new FormControl(this);
                formControlGroup = new FormControlGroup();

                state = new FiniteStateMachine(this);
            }
            catch { }

            try
            {
                listener = new LeapListener((LeapEventListener)this);
                controller = new Controller();
                controller.SetPolicyFlags(Controller.PolicyFlag.POLICYBACKGROUNDFRAMES);



                // Have the sample listener receive events from the controller
                controller.AddListener(listener);
            }
            catch
            {
                System.Windows.MessageBox.Show("Please ensure that the Leap Motion Controller is installed correctly.", "Pointable");
                System.Windows.Forms.Application.Exit();
            }

            try
            {
                if (formControl == null || formControl.IsDisposed)
                {
                    formControl = new FormControl(this);
                }


              //  if (firstRun)
              //      startCalibrateScreen(true);

               // if (firstRun)
               //     startTutorial();


                Thread th = new Thread(new ThreadStart(checkLeapConnection));
                th.IsBackground = true;
                th.Start();
            }
            catch { }

            try
            {
                gkh.HookedKeys.Add(Keys.LControlKey);
                gkh.HookedKeys.Add(Keys.RControlKey);

                gkh.HookedKeys.Add(Keys.RMenu);
                gkh.HookedKeys.Add(Keys.LMenu);
                
                gkh.HookedKeys.Add(Keys.LShiftKey);
                gkh.HookedKeys.Add(Keys.RShiftKey);

                gkh.KeyDown += new KeyEventHandler(gkh_KeyDown);
                gkh.KeyUp += new KeyEventHandler(gkh_KeyUp);
            }
            catch { }

            try
            {
                //isWindows8 = true;
                //isWindows81 = true;
                string OS = Tools.getOSInfo();
                if (OS == "7")
                    isWindows8 = false;
                else
                    isWindows8 = true;

                if (OS == "8.1")
                    isWindows81 = true;
               
            }
            catch { }

            //setControllerConfiguration();
        }

        bool controlKeyDown = false;
        bool altKeyDown = false;
        bool shiftKeyDown = false;

        bool actualKeyDownControl = false;
        bool actualKeyDownAlt = false;
        bool actualKeyDownShift = false;

        bool keyTriggeringTemp = false;
        long controlKeyDownPreviousTime = 0;

        Point previousClickPositionControlKey;

        void gkh_KeyDown(object sender, KeyEventArgs e)
        {
            if (keyTriggeringTemp) return;

            try
            {
                if (e.KeyCode == Keys.LControlKey || e.KeyCode == Keys.RControlKey) //cursor
                {
                    actualKeyDownControl = true;

                    if (formControl.windowCursorForm.Visible && Tools.keyModifierModeControl)
                    {
                        if (!controlKeyDown)
                        {
                            //check previous time
                            long duration = (DateTime.Now.Ticks - controlKeyDownPreviousTime) / 10000;

                           // Console.WriteLine(controlKeyDownPreviousTime.ToString());

                            if (duration < 300)
                            {
                                //Console.WriteLine("Double click");
                                _timerKeyDown.Enabled = false;
                                
                                BeginInvoke((MethodInvoker)delegate()
                                 {
                                     try
                                     {
                                         formControl.windowCursorForm.Visible = false;

                                         if (formControl.windowCursorDropForm.Visible)
                                             formControl.windowCursorDropForm.Hide();

                                         triggerMouseMoveAndClick(previousClickPositionControlKey, 0);
                                         formControl.drawWindowCursorDropClickAbsolute(previousClickPositionControlKey.X, previousClickPositionControlKey.Y, true);

                                         //leapTapGesture(false);
                                         formControl.windowCursorForm.Visible = true;
                                     }
                                     catch { }
                                 });

                                controlKeyDown = false;

                                controlKeyDownPreviousTime = DateTime.Now.Ticks;

                                e.Handled = true;
                                return;
                            }

                            controlKeyDown = true;


                            if (formControl.windowCursorDropForm.Visible)
                            {

                                BeginInvoke((MethodInvoker)delegate()
                                 {
                                     try
                                     {
                                         formControl.windowCursorDropForm.Hide();
                                     }
                                     catch { }
                                 });
                            }

                            try
                            {
                                if (_timerKeyDown == null)
                                {
                                    _timerKeyDown = new System.Timers.Timer(140); //300
                                    _timerKeyDown.Elapsed += new ElapsedEventHandler(_timerKeyDown_Elapsed);
                                    _timerKeyDown.Enabled = true;
                                }
                                else
                                {
                                    _timerKeyDown.Enabled = false;
                                    _timerKeyDown.Enabled = true;
                                }
                            }
                            catch { }
                        }
                        e.Handled = true;
                    }
                } else if (e.KeyCode == Keys.LMenu || e.KeyCode == Keys.RMenu) //cursor
                {
                    actualKeyDownAlt = true;

                    if (formControl.windowCursorForm.Visible && Tools.keyModifierModeAlt)
                    {
                        if (!altKeyDown)
                        {
                            //Console.WriteLine("right down");
                            altKeyDown = true;


                            BeginInvoke((MethodInvoker)delegate()
                             {
                                 try
                                 {
                                     if (formControl.windowCursorDropForm.Visible)
                                         formControl.windowCursorDropForm.Hide();

                                     triggerMouseMoveAndClick(pointedScreenPositionUniversal, 1);

                                     formControl.drawWindowCursorDropClickAbsolute(pointedScreenPositionUniversal.X, pointedScreenPositionUniversal.Y, true);
                                 }
                                 catch { }
                             });
                        }
                        e.Handled = true;
                    }
                }
                else if (e.KeyCode == Keys.LShiftKey || e.KeyCode == Keys.RShiftKey) //cursor
                {
                    actualKeyDownShift = true;

                    if (formControl.windowCursorForm.Visible && Tools.keyModifierModeShift)
                    {
                        if (!shiftKeyDown)
                        {
                            //Console.WriteLine("right down");
                            shiftKeyDown = true;


                            BeginInvoke((MethodInvoker)delegate()
                             {
                                 try
                                 {
                                     if (formControl.windowCursorDropForm.Visible)
                                         formControl.windowCursorDropForm.Hide();

                                     triggerMouseMoveAndClick(pointedScreenPositionUniversal, 2);

                                     formControl.drawWindowCursorDropClickAbsolute(pointedScreenPositionUniversal.X, pointedScreenPositionUniversal.Y, false);
                                 }
                                 catch { }
                             });
                        }
                        e.Handled = true;
                    }
                }
            }
            catch { }
        }


        void gkh_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {

                if (keyTriggeringTemp) return;

                if (e.KeyCode == Keys.LControlKey || e.KeyCode == Keys.RControlKey) //cursor
                {
                    actualKeyDownControl = false;

                    if (formControl.windowCursorForm.Visible && Tools.keyModifierModeControl)
                    {
                                       
                        if (controlKeyDown)
                        { 
                            BeginInvoke((MethodInvoker)delegate()
                             {
                                //Console.WriteLine("released");
                                 try
                                 {
                                     if (_timerKeyDown != null)
                                     {

                                         if (_timerKeyDown.Enabled)
                                         {
                                             formControl.windowCursorForm.Visible = false;
                                             previousClickPositionControlKey.X = pointedScreenPositionUniversal.X;
                                             previousClickPositionControlKey.Y = pointedScreenPositionUniversal.Y;
                                             triggerMouseMoveAndClick(pointedScreenPositionUniversal, 0);
                                             formControl.drawWindowCursorDropClickAbsolute(pointedScreenPositionUniversal.X, pointedScreenPositionUniversal.Y, true);
                                             //leapTapGesture(false);
                                             formControl.windowCursorForm.Visible = true;

                                             controlKeyDownPreviousTime = DateTime.Now.Ticks;


                                             e.Handled = true;
                                             //triggerMouseMoveAndClick(cursorPreviousClick, 0);
                                         }
                                         else
                                         {

                                             triggerMouseMoveAndClickUpOnly(pointedScreenPositionUniversal, 0);

                                             e.Handled = true;
                                         }

                                         _timerKeyDown.Enabled = false;
                                     }
                                 }
                                 catch { }
                             });
                         }
                    }
                        controlKeyDown = false;
                       // e.Handled = false;                    
                }
                else if (e.KeyCode == Keys.LMenu || e.KeyCode == Keys.RMenu) //cursor
                {
                    actualKeyDownAlt = false;

                    if (formControl.windowCursorForm.Visible && Tools.keyModifierModeAlt)
                    {
                        if (altKeyDown)
                        {
                            //Console.WriteLine("right up");
                            altKeyDown = false;                      

                        }
                        e.Handled = true;
                    }
                }
                else if (e.KeyCode == Keys.LShiftKey || e.KeyCode == Keys.RShiftKey) //cursor
                {
                    if (Tools.keyModifierModeEnableTracking)
                    {
                        if (actualKeyDownShift && actualKeyDownControl && actualKeyDownAlt && !formControl.windowCursorForm.Visible)
                        {
                            pauseResumeTracking();
                        }
                    }

                    actualKeyDownShift = false;

                    if (formControl.windowCursorForm.Visible && Tools.keyModifierModeShift)
                    {
                        if (shiftKeyDown)
                        {
                            //Console.WriteLine("right up");
                            shiftKeyDown = false;
                        }
                        e.Handled = true;
                    }
                }
            }
            catch { }
        }

        System.Timers.Timer _timerKeyDown;

        void _timerKeyDown_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                _timerKeyDown.Enabled = false;

                //start drag
                BeginInvoke((MethodInvoker)delegate()
                {
                   // formControl.windowCursorForm.Hide();
                    if (formControl.windowCursorDropForm != null && !formControl.windowCursorDropForm.IsDisposed && formControl.windowCursorDropForm.Visible)
                    {
                        formControl.windowCursorDropForm.Hide();
                        triggerMouseMoveAndClickDownOnly(cursorPreviousClick, 0);
                        formControl.drawWindowCursorDropClickAbsolute(cursorPreviousClick.X, cursorPreviousClick.Y, true);

                        //if (isWindowSpecial && touchedTop)
                        //    triggerMouseMove(cursorPreviousClick.X, cursorPreviousClick.Y + 10);
                    }
                    else
                    {
                        triggerMouseMoveAndClickDownOnly(pointedScreenPositionUniversal, 0);
                        formControl.drawWindowCursorDropClickAbsolute(pointedScreenPositionUniversal.X, pointedScreenPositionUniversal.Y, true);
                        //Console.WriteLine("No Drop");
                    }


                    //formControl.windowCursorForm.Show();
                    //formControl.drawWindowCursorDropClickAbsolute(pointedScreenPositionUniversal.X, pointedScreenPositionUniversal.Y, true);
                });
            }
            catch { }
        }

        internal void openGuide()
        {
            //if (firstRun)
            {
                try
                {
                    //start help
                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.FileName = "http://www.pointable.net/getstarted"; //C:\\Program Files (x86)\\EventGhost\\EventGhost.exe";
                    startInfo.Arguments = "";// "-event NextChannel";
                    Process.Start(startInfo);
                }
                catch { }

                firstRun = false;
            }
        }

        public void leapNotConnected()
        {
            disconnectedPreviously = true;

            BeginInvoke((MethodInvoker)delegate()
            {
                try
                {
                    disablePointableToolStripMenuItem.Enabled = false;

                    notifyIconSysTray.BalloonTipText = "Leap Motion Controller is not connected.";
                    notifyIconSysTray.ShowBalloonTip(5000);

                    notifyIconSysTray.Icon = new Icon(Form1.executable.GetManifestResourceStream("Pointable.Resources.Pointable_NotActivated.ico"));
                }
                catch { }
            });
        }

        public void leapDisconnected()
        {
            disconnectedPreviously = true;

            BeginInvoke((MethodInvoker)delegate()
            {
                try
                {
                    disablePointableToolStripMenuItem.Enabled = false;

                    notifyIconSysTray.BalloonTipText = "Leap Motion Controller has been disconnected.";
                    notifyIconSysTray.ShowBalloonTip(3000);

                    notifyIconSysTray.Icon = new Icon(Form1.executable.GetManifestResourceStream("Pointable.Resources.Pointable_NotActivated.ico"));


                }
                catch { }
            });

            BeginInvoke((MethodInvoker)delegate()
            {
                try
                {
                    state.ProcessEvent(FiniteStateMachine.Events.EndGesture);

                    doStateReset();
                }
                catch { }
            });
        }

        bool disconnectedPreviously = false;
        public void leapConnected()
        {
            if (disconnectedPreviously)
            {
                BeginInvoke((MethodInvoker)delegate()
                {
                    try
                    {
                        disablePointableToolStripMenuItem.Enabled = true;

                        if (detectionActivated)
                        {
                            notifyIconSysTray.BalloonTipText = "Leap Motion Controller is connected. Pointable tracking resumed.";
                            notifyIconSysTray.ShowBalloonTip(3000);
                            notifyIconSysTray.Icon = new Icon(Form1.executable.GetManifestResourceStream("Pointable.Resources.pointable2.ico"));
                        }
                        else
                        {
                            notifyIconSysTray.BalloonTipText = "Leap Motion Controller is connected.";
                            notifyIconSysTray.ShowBalloonTip(3000);
                        }
                    }
                    catch { }
                });
            }

            try
            {
                BeginInvoke((MethodInvoker)delegate()
                {
                    try
                    {
                        formControl.setCursorNormal();
                        formControl.setCursorDropletRefresh();
                        formControl.setCursorDropletClickRedRefresh();
                    }
                    catch { }
                });
            }
            catch { }
        }

        private void chkCursorControl_CheckedChanged(object sender, EventArgs e)
        {
            cursorControl = chkCursorControl.Checked;
        }


        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                notifyIconSysTray.Visible = false;
                saveSettingsData();
            }
            catch { }
        }
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                Tools.changeCursorWindowsNormal();
                notifyIconSysTray.Visible = false;
                controller.RemoveListener(listener);
            }
            catch { }
        }
        private void buttonProcessObjects_Click(object sender, EventArgs e)
        {
            try
            {
                //process intersect to get coordinates for object
                for (int i = 0; i < NUMBEROBJECTS; i++)
                {
                    objectCoordinate[i] = Vector3.computeIntersect(objectCoordA1[i], objectCoordA2[i], objectCoordB1[i], objectCoordB2[i]);
                }
                objectNames[0] = textBoxObject0.Text;
                objectNames[1] = textBoxObject1.Text;
                objectNames[2] = textBoxObject2.Text;
                objectNames[3] = textBoxObject3.Text;
                objectNames[4] = textBoxObject4.Text;
                objectNames[5] = textBoxObject5.Text;


                for (int i = 0; i < pointables.Count; i++)
                {
                    pointables[i].position = new Vector3(objectCoordinate[i]);
                }
            }
            catch { }
        }
        private void buttonObject0A_Click(object sender, EventArgs e)
        {
            calibrateCurrent(0, 0);
        }

        private void buttonObject0B_Click(object sender, EventArgs e)
        {
            calibrateCurrent(0, 1);
        }

        private void buttonObject1A_Click(object sender, EventArgs e)
        {
            calibrateCurrent(1, 0);
        }

        private void buttonObject1B_Click(object sender, EventArgs e)
        {
            calibrateCurrent(1, 1);
        }

        private void buttonObject2A_Click(object sender, EventArgs e)
        {
            calibrateCurrent(2, 0);
        }

        private void buttonObject2B_Click(object sender, EventArgs e)
        {
            calibrateCurrent(2, 1);
        }

        private void buttonObject3A_Click(object sender, EventArgs e)
        {
            calibrateCurrent(3, 0);
        }

        private void buttonObject3B_Click(object sender, EventArgs e)
        {
            calibrateCurrent(3, 1);
        }

        private void buttonObject4A_Click(object sender, EventArgs e)
        {
            calibrateCurrent(4, 0);
        }

        private void buttonObject4B_Click(object sender, EventArgs e)
        {
            calibrateCurrent(4, 1);
        }

        private void buttonObject5A_Click(object sender, EventArgs e)
        {
            calibrateCurrent(5, 0);
        }

        private void buttonObject5B_Click(object sender, EventArgs e)
        {
            calibrateCurrent(5, 1);
        }
        private void btnCalibrateProcess_Click(object sender, EventArgs e)
        {
            processScreenCalibration();
            saveSettingsData();
        }
        private void btnCalibrate1a_Click(object sender, EventArgs e)
        {
            screenLineA[0] = fingertipCoordinate[0];
            screenLineB[0] = fingertipCoordinateOut[0];
        }

        private void btnCalibrate1b_Click(object sender, EventArgs e)
        {
            screenLineA[4] = fingertipCoordinate[0];
            screenLineB[4] = fingertipCoordinateOut[0];
        }

        private void btnCalibrate2a_Click(object sender, EventArgs e)
        {
            screenLineA[1] = fingertipCoordinate[0];
            screenLineB[1] = fingertipCoordinateOut[0];
        }

        private void btnCalibrate2b_Click(object sender, EventArgs e)
        {
            screenLineA[5] = fingertipCoordinate[0];
            screenLineB[5] = fingertipCoordinateOut[0];
        }

        private void btnCalibrate3a_Click(object sender, EventArgs e)
        {
            screenLineA[2] = fingertipCoordinate[0];
            screenLineB[2] = fingertipCoordinateOut[0];
        }

        private void btnCalibrate3b_Click(object sender, EventArgs e)
        {
            screenLineA[6] = fingertipCoordinate[0];
            screenLineB[6] = fingertipCoordinateOut[0];
        }

        private void btnCalibrate4a_Click(object sender, EventArgs e)
        {
            screenLineA[3] = fingertipCoordinate[0];
            screenLineB[3] = fingertipCoordinateOut[0];
        }

        private void btnCalibrate4b_Click(object sender, EventArgs e)
        {
            screenLineA[7] = fingertipCoordinate[0];
            screenLineB[7] = fingertipCoordinateOut[0];
        }
        private void calibrateCurrent(int objectNumber, int calibrateNumber)
        {
            if (calibrateNumber == 0)
            {
                objectCoordA1[objectNumber].X = fingertipCoordinate[0].X;
                objectCoordA1[objectNumber].Y = fingertipCoordinate[0].Y;
                objectCoordA1[objectNumber].Z = fingertipCoordinate[0].Z;

                objectCoordA2[objectNumber].X = fingertipCoordinateOut[0].X;
                objectCoordA2[objectNumber].Y = fingertipCoordinateOut[0].Y;
                objectCoordA2[objectNumber].Z = fingertipCoordinateOut[0].Z;
            }
            else
            {
                objectCoordB1[objectNumber].X = fingertipCoordinate[0].X;
                objectCoordB1[objectNumber].Y = fingertipCoordinate[0].Y;
                objectCoordB1[objectNumber].Z = fingertipCoordinate[0].Z;

                objectCoordB2[objectNumber].X = fingertipCoordinateOut[0].X;
                objectCoordB2[objectNumber].Y = fingertipCoordinateOut[0].Y;
                objectCoordB2[objectNumber].Z = fingertipCoordinateOut[0].Z;
            }
        }

        public void processScreenData()
        {
            try
            {
                Vector3 lineOnScreen1 = screenCorner[0] - screenCorner[1];
                Vector3 lineOnScreen2 = screenCorner[2] - screenCorner[1];

                screenNormal = Vector3.crossProduct(lineOnScreen1, lineOnScreen2);
                try
                {
                    screenNormal.Normalize();
                }
                catch { }

                horizontalAxis = (screenCorner[1] - screenCorner[0]);
                try
                {
                    horizontalAxis.Normalize();
                }
                catch { }


                verticalAxis = (screenCorner[1] - screenCorner[2]);
                try
                {
                    verticalAxis.Normalize();
                }
                catch { }

                screenPhysicalWidth = (Vector3.computeDistance(screenCorner[0], screenCorner[1])); //+ computeDistance(screenCorner[2], screenCorner[3])) / 2;
                screenPhysicalHeight = (Vector3.computeDistance(screenCorner[1], screenCorner[2]));

                screenCenter = (screenCorner[0] + screenCorner[2]) / 2;
            }
            catch { }
           // System.Windows.Forms.MessageBox.Show(System.Environment.NewLine + System.Environment.NewLine + screenPhysicalWidth.ToString());
        }


        public void processScreenCalibration()
        {
            try
            {
                string calibrationStats = "";
                for (int i = 0; i < 4; i++)
                {
                    screenCorner[i] = Vector3.computeIntersect(screenLineA[i], screenLineB[i], screenLineA[i + 4], screenLineB[i + 4]);

                    calibrationStats = calibrationStats + screenCorner[i].X + System.Environment.NewLine + screenCorner[i].Y + System.Environment.NewLine + screenCorner[i].Z;
                    calibrationStats = calibrationStats + System.Environment.NewLine + System.Environment.NewLine;
                }

                Vector3 lineOnScreen1 = screenCorner[0] - screenCorner[1];
                Vector3 lineOnScreen2 = screenCorner[2] - screenCorner[1];

                screenNormal = Vector3.crossProduct(lineOnScreen1, lineOnScreen2);
                try
                {
                    screenNormal.Normalize();
                }
                catch { }

                horizontalAxis = (screenCorner[1] - screenCorner[0]);
                try
                {
                    horizontalAxis.Normalize();
                }
                catch { }


                verticalAxis = (screenCorner[1] - screenCorner[2]);
                try
                {
                    verticalAxis.Normalize();
                }
                catch { }

                screenPhysicalWidth = (Vector3.computeDistance(screenCorner[0], screenCorner[1])); //+ computeDistance(screenCorner[2], screenCorner[3])) / 2;
                screenPhysicalHeight = (Vector3.computeDistance(screenCorner[1], screenCorner[2]));

                screenCenter = (screenCorner[0] + screenCorner[2]) / 2;

                System.Windows.Forms.MessageBox.Show(calibrationStats = calibrationStats + System.Environment.NewLine + System.Environment.NewLine + screenPhysicalWidth.ToString());
            }
            catch { }
        }


        int fingerID;

        long fingerTrackStartTime;
        FingerTrackStates fingerTrackState = FingerTrackStates.NotFound;
        private enum FingerTrackStates { NotFound, Found, Accepted };

        List<Vector3> pastFingerPositions = new List<Vector3>();
        List<long> pastFingerPositionsTiming =new List<long>();
        const int MAX_STORE = 30;

        public void leapPointableAll(Leap.Frame frame)
        {
            try
            {
                Leap.PointableList pointables = frame.Pointables;
                fingers = pointables;

                Leap.Pointable finger = pointables.Frontmost;

                hand = finger.Hand;


                if (FormFindWindow.checkWindowType(Tools.GetForegroundWindow()) == FormFindWindow.WindowType.TaskSwitcher)
                {
                    if (taskSwitcher == false)
                    {
                        previousWindowAwareHandle = IntPtr.Zero;
                        handle = IntPtr.Zero;

                        taskSwitcherStartTime = DateTime.Now.Ticks;
                        taskSwitcher = true;


                        try
                        {
                            if (state.State != FiniteStateMachine.States.Wake)
                            {
                                BeginInvoke((MethodInvoker)delegate()
                                {
                                    state.ProcessEvent(FiniteStateMachine.Events.EndGesture);
                                });
                            }
                        }
                        catch { }
                    }

                    if (state.State != FiniteStateMachine.States.Wake)
                        return;
                }
                else
                {
                    if (taskSwitcher == true)
                    {
                        setControllerConfiguration();

                        previousWindowAwareHandle = IntPtr.Zero;
                        handle = IntPtr.Zero;

                        pastFingerPositions.Clear();
                        pastFingerPositionsTiming.Clear();
                    }
                    taskSwitcher = false;
                }

                if (state.State == FiniteStateMachine.States.Wake) //wait for pointing
                {
                    if (finger.TipVelocity.Magnitude < 30 && finger.Hand.IsValid && finger.Hand.Fingers.Count > 3 && finger.TipVelocity.Magnitude < 50)
                    {

                        double anglePalm = getAngleBetweenVectors(new Vector3(finger.Hand.Direction.x,finger.Hand.Direction.y , finger.Hand.Direction.z) , new Vector3 (0,0,-1));
                       // Console.WriteLine(anglePalm);
                        if (anglePalm > 55)
                        {
                            if (taskSwitcher == false)
                            {//application switcher
                                taskSwitcher = true;

                                long durationTaskSwitcher = (DateTime.Now.Ticks - taskSwitcherStartTime) / 10000;

                                if (durationTaskSwitcher < 1000)
                                {
                                    return;
                                }

                                taskSwitcherStartTime = DateTime.Now.Ticks;

                                BeginInvoke((MethodInvoker)delegate()
                                { //trigger CTRL -       
                                    try
                                    {
                                        state.ProcessEvent(FiniteStateMachine.Events.EndGesture);
                                    }
                                    catch { }
                                    try
                                    {
                                        byte[] key;
                                        if (isWindows8)
                                             key = new byte[] { (byte)0x11, (byte)0x12, (byte)0x09, (byte)0 }; //alt 0x12 //win 0x5B
                                        else
                                             key = new byte[] { (byte)0x11, (byte)0x5B, (byte)0x09, (byte)0 }; //alt 0x12 //win 0x5B

                                        keyTriggeringTemp = true;
                                        formControl.triggerKeyEventNoDelay(key);

                                        Thread.Sleep(30);
                                        keyTriggeringTemp = false;

                                        resetControllerConfiguration();
                                    }
                                    catch { }

                                });

                                //taskSwitcher = true;
                            }
                            return;
                        }
                        else if (!taskSwitcher)
                        {
                            handle = previousWindowAwareHandle;

                            if (handle != IntPtr.Zero && Tools.IsWindow(handle) && !isOwnApplicationFocus)
                            {
                                //state.ProcessEvent(FiniteStateMachine.Events.FoundWindow);

                                BeginInvoke((MethodInvoker)delegate()
                                {
                                    state.ProcessEvent(FiniteStateMachine.Events.FoundWindow);
                                    state.ProcessEvent(FiniteStateMachine.Events.StartGesture);
                                });
                                return;
                            }
                        }
                    }

                    if (fingerTrackState == FingerTrackStates.NotFound)
                    {//find finger

                        Tools.changeCursorWindowsNormal();
                        if (finger.Hand.IsValid)
                        {
                            Leap.Pointable pointer = finger;

                            fingertipCoordinate[0] = new Vector3((pointer.TipPosition.x + pointer.StabilizedTipPosition.x) / 2,
                                (pointer.TipPosition.y + pointer.StabilizedTipPosition.y) / 2, (pointer.TipPosition.z + pointer.StabilizedTipPosition.z) / 2);
                            Vector3 tipPositionOut = new Vector3(pointer.Direction.x, pointer.Direction.y, pointer.Direction.z);

                            fingertipCoordinateOut[0] = fingertipCoordinate[0] + tipPositionOut;
                            fingertipVelocity = new Vector3(finger.TipVelocity.x, finger.TipVelocity.y, finger.TipVelocity.z);

                            Vector3 palmPos = new Vector3(finger.Hand.PalmPosition.x, finger.Hand.PalmPosition.y, finger.Hand.PalmPosition.z);
                            double distancePalmFinger = Vector3.computeDistance(palmPos, fingertipCoordinate[0]);

                            if (distancePalmFinger > 85)//85
                            {
                                
                                resetSmoothingDirection();

                                fingerTrackStartTime = DateTime.Now.Ticks;
                                fingerID = finger.Id;
                                fingerTrackState = FingerTrackStates.Found;
                            }
                            //track if same finger, if yes give chance.
                        }
                        else
                        {
                            Leap.Pointable pointer = finger;

                            if (finger.StabilizedTipPosition.y < 150) //10cm
                            {
                                //fingertipCoordinate[0] = new Vector3((pointer.TipPosition.x + pointer.StabilizedTipPosition.x) / 2,
                                //     (pointer.TipPosition.y + pointer.StabilizedTipPosition.y) / 2, (pointer.TipPosition.z + pointer.StabilizedTipPosition.z) / 2);
                                //Vector3 tipPositionOut = new Vector3(pointer.Direction.x, pointer.Direction.y, pointer.Direction.z);

                                //fingertipCoordinateOut[0] = fingertipCoordinate[0] + tipPositionOut;
                                //fingertipVelocity = new Vector3(finger.TipVelocity.x, finger.TipVelocity.y, finger.TipVelocity.z);


                                resetSmoothingDirection();
                                fingerTrackStartTime = DateTime.Now.Ticks;
                                fingerID = finger.Id;
                                fingerTrackState = FingerTrackStates.Found;
                            }
                        }
                    }
                    else if (fingerTrackState == FingerTrackStates.Found)
                    {// previously found finger
                        //calculate finger direction velocity. TODO

                        Finger previousFinger = frame.Finger(fingerID);

                        if (previousFinger.IsValid && previousFinger.Hand.IsValid)
                        {
                            if (smoothingMode)
                            {
                                fingertipCoordinate[0] = new Vector3((previousFinger.TipPosition.x + previousFinger.StabilizedTipPosition.x) / 2,
                                     (previousFinger.TipPosition.y + previousFinger.StabilizedTipPosition.y) / 2, (previousFinger.TipPosition.z + previousFinger.StabilizedTipPosition.z) / 2);

                                Vector3 tipPositionOut = new Vector3(previousFinger.Direction.x, previousFinger.Direction.y, previousFinger.Direction.z);
                                fingertipVelocity = new Vector3(finger.TipVelocity.x, finger.TipVelocity.y, finger.TipVelocity.z);
                                Vector3 smoothTipPositionOut = smoothingDirection(tipPositionOut.X, tipPositionOut.Y, tipPositionOut.Z, fingertipVelocity, fingertipCoordinate[0]);
                            }

                            long duration = (DateTime.Now.Ticks - fingerTrackStartTime) / 10000;
                            if (duration > 250) //<1.00 is 150
                            {
                                fingerTrackState = FingerTrackStates.Accepted;
                            }
                        }
                        else if (previousFinger.IsValid && !previousFinger.Hand.IsValid && previousFinger.StabilizedTipPosition.y < 150)
                        {
                            //fingertipCoordinateOut[0] = fingertipCoordinate[0] + tipPositionOut;

                            if (smoothingMode)
                            {
                                fingertipCoordinate[0] = new Vector3((previousFinger.TipPosition.x + previousFinger.StabilizedTipPosition.x) / 2,
                                    (previousFinger.TipPosition.y + previousFinger.StabilizedTipPosition.y) / 2, (previousFinger.TipPosition.z + previousFinger.StabilizedTipPosition.z) / 2);

                                Vector3 tipPositionOut = new Vector3(previousFinger.Direction.x, previousFinger.Direction.y, previousFinger.Direction.z);
                                fingertipVelocity = new Vector3(finger.TipVelocity.x, finger.TipVelocity.y, finger.TipVelocity.z);
                                Vector3 smoothTipPositionOut = smoothingDirection(tipPositionOut.X, tipPositionOut.Y, tipPositionOut.Z, fingertipVelocity, fingertipCoordinate[0]);
                            }


                            long duration = (DateTime.Now.Ticks - fingerTrackStartTime) / 10000;
                            if (duration > 150)
                            {
                                fingerTrackState = FingerTrackStates.Accepted;
                            }
                        }
                        else
                        {
                            fingerTrackState = FingerTrackStates.NotFound;
                        }
                    }
                    else if (fingerTrackState == FingerTrackStates.Accepted)
                    {
                        Leap.Pointable pointer = frame.Finger(fingerID);
                                                
                        if (!pointer.IsValid)
                        {
                            fingerTrackState = FingerTrackStates.NotFound;
                            return;
                        }


                        if (taskSwitcher)
                        {//check vibration

                            if (!pointer.IsValid)

                                if (pastFingerPositions.Count == MAX_STORE)
                                {
                                    pastFingerPositions.RemoveAt(0);
                                    pastFingerPositionsTiming.RemoveAt(0);
                                }

                            pastFingerPositions.Add(new Vector3(pointer.TipPosition.x, pointer.TipPosition.y, pointer.TipPosition.z));
                            pastFingerPositionsTiming.Add(DateTime.Now.Ticks);
                        }

                        fingertipCoordinate[0] = new Vector3((pointer.TipPosition.x + pointer.StabilizedTipPosition.x) / 2,
                            (pointer.TipPosition.y + pointer.StabilizedTipPosition.y) / 2, (pointer.TipPosition.z + pointer.StabilizedTipPosition.z) / 2);
                        Vector3 tipPositionOut = new Vector3(pointer.Direction.x, pointer.Direction.y, pointer.Direction.z);

                        //fingertipCoordinateOut[0] = fingertipCoordinate[0] + tipPositionOut;
                        fingertipVelocity = new Vector3(finger.TipVelocity.x, finger.TipVelocity.y, finger.TipVelocity.z);

                        if (smoothingMode)
                        {
                            Vector3 smoothTipPositionOut = smoothingDirection(tipPositionOut.X, tipPositionOut.Y, tipPositionOut.Z, fingertipVelocity, fingertipCoordinate[0]);
                            fingertipCoordinateOut[0] = fingertipCoordinate[0] + smoothTipPositionOut;
                        }
                        else
                        {
                            fingertipCoordinateOut[0] = fingertipCoordinate[0] + tipPositionOut;
                        }

                        //check if same finger

                        //if (finger.Hand.IsValid)
                        //{
                        //    Vector3 palmPos = new Vector3(finger.Hand.PalmPosition.x, finger.Hand.PalmPosition.y, finger.Hand.PalmPosition.z);
                        //    double distancePalmFinger = Vector3.computeDistance(palmPos, fingertipCoordinate[0]);
                        //    Debug.WriteLine(distancePalmFinger);
                        //    if (distancePalmFinger < 85)//85
                        //        return;

                        //    //track if same finger, if yes give chance.
                        //}
                        //else //IMPORTANT TODO, give chance
                        //{
                        //    return;
                        //}
                        if (taskSwitcher) return;

                        if (fingertipVelocity.Magnitude > 1000) return;


                        checkGestureDown();
                        checkGestureRight();
                        checkGestureLeft();
                        BeginInvoke((MethodInvoker)delegate()
                        {
                            checkControl();
                        });
                        //resetSmoothingDirection();
                        resetSmoothingPosition();
                        // fingerID = finger.Id;
                    }
                }
                else if (state.State == FiniteStateMachine.States.PointableAware || state.State == FiniteStateMachine.States.WindowAware || state.State == FiniteStateMachine.States.WindowControl || state.State == FiniteStateMachine.States.WindowPointable)
                {


                    bool fingerFound = false;
                    foreach (Leap.Pointable pointer in pointables)
                    {
                        if (pointer.Id == fingerID)
                        {
                            fingerFound = true;

                            if (state.State == FiniteStateMachine.States.WindowAware)
                            {//check vibration
                                if (pastFingerPositions.Count == MAX_STORE)
                                {
                                    pastFingerPositions.RemoveAt(0);
                                    pastFingerPositionsTiming.RemoveAt(0);
                                }

                                pastFingerPositions.Add(new Vector3(pointer.TipPosition.x, pointer.TipPosition.y, pointer.TipPosition.z));
                                pastFingerPositionsTiming.Add(DateTime.Now.Ticks);
                            }

                            fingertipVelocity = new Vector3(pointer.TipVelocity.x, pointer.TipVelocity.y, pointer.TipVelocity.z);
                            Vector3 tipPositionOut = new Vector3(pointer.Direction.x, pointer.Direction.y, pointer.Direction.z);

                            if (smoothingMode)
                            {
                                Vector3 fingerTipPosition = new Vector3((pointer.TipPosition.x + pointer.StabilizedTipPosition.x) / 2,
                                    (pointer.TipPosition.y + pointer.StabilizedTipPosition.y) / 2, (pointer.TipPosition.z + pointer.StabilizedTipPosition.z) / 2);
                                fingertipCoordinate[0] = smoothingPosition(fingerTipPosition.X, fingerTipPosition.Y, fingerTipPosition.Z, fingertipVelocity.Magnitude);
                            }
                            else
                            {
                                fingertipCoordinate[0] = new Vector3((pointer.TipPosition.x + pointer.StabilizedTipPosition.x) / 2,
                                    (pointer.TipPosition.y + pointer.StabilizedTipPosition.y) / 2, (pointer.TipPosition.z + pointer.StabilizedTipPosition.z) / 2);
                            }


                            if (finger.Hand.IsValid)
                            {
                                Vector3 palmPos = new Vector3(finger.Hand.PalmPosition.x, finger.Hand.PalmPosition.y, finger.Hand.PalmPosition.z);
                                double distancePalmFinger = Vector3.computeDistance(palmPos, fingertipCoordinate[0]);

                              //  if (distancePalmFinger < pointer.Length) return;
                            }

                            if (smoothingMode)
                            {
                                Vector3 smoothTipPositionOut = smoothingDirection(tipPositionOut.X, tipPositionOut.Y, tipPositionOut.Z, fingertipVelocity, fingertipCoordinate[0]);
                                fingertipCoordinateOut[0] = fingertipCoordinate[0] + smoothTipPositionOut;
                            }
                            else
                            {
                                fingertipCoordinateOut[0] = fingertipCoordinate[0] + tipPositionOut;
                            }

                            if (finger.TipVelocity.Magnitude < 30 && finger.Hand.IsValid && finger.Hand.Fingers.Count > 3)
                            {
                                double anglePalm = getAngleBetweenVectors(new Vector3(pointer.Hand.Direction.x, pointer.Hand.Direction.y, pointer.Hand.Direction.z), new Vector3(0, 0, -1));
                                // Console.WriteLine(anglePalm);
                                if (anglePalm > 55)
                                {
                                    if (taskSwitcher == false)
                                    {//application switcher

                                        taskSwitcher = true;

                                        long durationTaskSwitcher = (DateTime.Now.Ticks - taskSwitcherStartTime) / 10000;

                                        if (durationTaskSwitcher < 1000)
                                        {
                                            return;
                                        }

                                        taskSwitcherStartTime = DateTime.Now.Ticks;

                                        BeginInvoke((MethodInvoker)delegate()
                                        { //trigger CTRL -       

                                            try
                                            {
                                                state.ProcessEvent(FiniteStateMachine.Events.EndGesture);
                                            }
                                            catch { }
                                            try
                                            {
                                                byte[] key;
                                                if (isWindows8)
                                                    key = new byte[] { (byte)0x11, (byte)0x12, (byte)0x09, (byte)0 }; //alt 0x12 //win 0x5B
                                                else
                                                    key = new byte[] { (byte)0x11, (byte)0x5B, (byte)0x09, (byte)0 }; //alt 0x12 //win 0x5B


                                                keyTriggeringTemp = true;
                                                formControl.triggerKeyEventNoDelay(key);
                                                Thread.Sleep(30);
                                                keyTriggeringTemp = false;

                                                resetControllerConfiguration();
                                            }
                                            catch { }
                                        });

                                        //taskSwitcher = true;                                        
                                    }
                                    return;
                                }
                            }

                            checkGestureDown();
                            checkGestureRight();
                            checkGestureLeft();
                            BeginInvoke((MethodInvoker)delegate()
                            {
                                checkControl();
                            });
                        }
                    }
                    if (fingerFound == false)
                    {
                        resetSmoothingDirection();
                        resetSmoothingPosition();

                        BeginInvoke((MethodInvoker)delegate()
                        {
                            state.ProcessEvent(FiniteStateMachine.Events.EndGesture);
                        });
                    }
                }
                else if (state.State == FiniteStateMachine.States.Standby) //for debug
                {
                    fingertipCoordinate[0] = new Vector3(finger.TipPosition.x, finger.TipPosition.y, finger.TipPosition.z);
                    Vector3 tipPositionOut = new Vector3(finger.Direction.x, finger.Direction.y, finger.Direction.z);

                    fingertipCoordinateOut[0] = fingertipCoordinate[0] + tipPositionOut;
                    fingertipVelocity = new Vector3(finger.TipVelocity.x, finger.TipVelocity.y, finger.TipVelocity.z);

                    fingerID = finger.Id;
                }
                else if (state.State == FiniteStateMachine.States.Calibrating) //wait for pointing
                {
                    Leap.Pointable pointer = finger;

                    fingertipCoordinate[0] = new Vector3((pointer.TipPosition.x + pointer.StabilizedTipPosition.x) / 2,
                        (pointer.TipPosition.y + pointer.StabilizedTipPosition.y) / 2, (pointer.TipPosition.z + pointer.StabilizedTipPosition.z) / 2);
                    Vector3 tipPositionOut = new Vector3(pointer.Direction.x, pointer.Direction.y, pointer.Direction.z);

                    fingertipCoordinateOut[0] = fingertipCoordinate[0] + tipPositionOut;
                    fingertipVelocity = new Vector3(finger.TipVelocity.x, finger.TipVelocity.y, finger.TipVelocity.z);

                    BeginInvoke((MethodInvoker)delegate()
                    {
                        if (calibrateScreen)
                        {
                            if ((fingers.Count == 1 || fingers.Count == 2) && fingertipVelocity.Magnitude < 15)
                            {//add delay

                                long durationSinceStart = (DateTime.Now.Ticks - calibrationStartTime)/10000;

                                if (durationSinceStart < 500) return;

                                if (calibrationFoundPointTime == 0)
                                {
                                    calibrationFoundPointTime = DateTime.Now.Ticks;
                                    return;
                                }

                                long durationSinceFoundPoint = (DateTime.Now.Ticks - calibrationFoundPointTime) / 10000;

                                if (durationSinceFoundPoint < 300) return;

                                double distance = Vector3.computeDistance(calibrationFoundPointPosition, fingertipCoordinate[0]);

                                if (distance > 20)
                                {
                                    calibrationFoundPointPosition.X = fingertipCoordinate[0].X;
                                    calibrationFoundPointPosition.Y = fingertipCoordinate[0].Y;
                                    calibrationFoundPointPosition.Z = fingertipCoordinate[0].Z;
                                    
                                    calibrationFoundPointTime = DateTime.Now.Ticks;
                                    return;
                                }

                                //formCalibrateScreen.calibrationData(fingertipCoordinate[0], fingertipCoordinateOut[0]);
                                resetSceenCorners();
                                Vector3 vScreen = Vector3.intersectLineScreen(fingertipCoordinate[0], fingertipCoordinateOut[0], screenNormal, screenCorner[0]);
                                double screenYOffset = vScreen.Y - (screenCorner[1].Y + screenCorner[2].Y )/2;
                                double screenXOffset = vScreen.X - (screenCorner[0].X + screenCorner[1].X )/2;
                                //Console.WriteLine(screenYOffset + "   " + screenXOffset);
                                                               


                                formCalibrateScreen.calibrateNextState();

                                screenVerticalOffset =  screenYOffset;
                                screenHorizontalOffset = screenXOffset;

                                initializeScreenCorners();
                            }
                        }
                        else
                        { 
                            if (fingers.Count == 1 && fingertipVelocity.Magnitude < 10)
                            {

                                bool pointWithinScreen = false;
                                Point pointedScreenPosition = checkPointedScreenPosition();
                                if (pointedScreenPosition.X == 0 && pointedScreenPosition.Y == 0)
                                    pointWithinScreen = false;
                                else
                                {
                                    int pixelAllowance = 10;
                                    foreach (Screen screen in Screen.AllScreens)
                                    {
                                        Rectangle screenBounds = screen.Bounds;
                                        screenBounds.Width += pixelAllowance * 2;
                                        screenBounds.Height += pixelAllowance * 2;
                                        screenBounds.X -= pixelAllowance;
                                        screenBounds.Y -= pixelAllowance;

                                        if (screenBounds.Contains(pointedScreenPosition))
                                        {
                                            pointWithinScreen = true;
                                            break;
                                        }
                                    }
                                }

                                if (!pointWithinScreen)
                                    formCalibrate.calibrationData(fingertipCoordinate[0], fingertipCoordinateOut[0]);
                            }
                        }

                    });
                    fingerID = finger.Id;
                }
            }
            catch { }
        }
        bool calibrateScreen = false;
        //public void leapFingerTipAll(Leap.Finger finger)
        //{
        //    //mut.ReleaseMutex();
        //    fingertipCoordinate[0] = new Vector3(finger.TipPosition.x, finger.TipPosition.y, finger.TipPosition.z);
        //    Vector3 tipPositionOut = new Vector3(finger.Direction.x, finger.Direction.y, finger.Direction.z);

        //    fingertipCoordinateOut[0] = fingertipCoordinate[0] + tipPositionOut;

        //}
        public void leapFingerTipData(Vector3 fingerTip, Vector3 fingerTipOut)
        {
            fingertipCoordinate[0] = fingerTip;
            fingertipCoordinateOut[0] = fingerTip + fingerTipOut;
        }

        public void leapPalmData(Vector3 _palmCoordinates, Vector3 _palmDirection, Vector3 _palmNormal)
        {
            palmFound = true;
            palmCoordinates = _palmCoordinates;
            palmCoordinatesOut = _palmCoordinates + _palmDirection + 0.3*_palmNormal;
            //palmDirection = _palmDirection;
            
        }
        public void leapPalmNotFound()
        {
            palmFound = false;
        }

        public void leapPalmClosed()
        {
            BeginInvoke((MethodInvoker)delegate()
            {
                try
                {
                    state.ProcessEvent(FiniteStateMachine.Events.PalmClosed);
                }
                catch { }
            });
        }

        public void leapStartGesture()
        {
            BeginInvoke((MethodInvoker)delegate()
            {
                try
                {
                    if (Tools.IsWindow(handle))
                        state.ProcessEvent(FiniteStateMachine.Events.StartGesture);
                }
                catch { }
            });
        }
        public void leapEndGesture()
        {
            BeginInvoke((MethodInvoker)delegate()
            {
                try
                {
                    state.ProcessEvent(FiniteStateMachine.Events.EndGesture);
                }
                catch { }
            });
        }

        public void leapGesture(Vector3 handCoord)
        {

        }


        internal bool circleGestureInProgress = false;
        internal bool taskSwitcher = false;
        long taskSwitcherStartTime = 0;

        public void leapCircleGestureStart(bool isClockwise, int fingerCount, bool isHand)
        {
            try
            {
                circleGestureInProgress = true;
                circleProgress = 0;
                if (state.State == FiniteStateMachine.States.WindowAware || state.State == FiniteStateMachine.States.Wake) //windowaware
                {
                    resetScrollSmoothing();
                }

                BeginInvoke((MethodInvoker)delegate()
                {
                    if (formControl.windowCursorDropForm.Visible)
                        formControl.windowCursorDropForm.Hide();
                });

                if (taskSwitcher)
                {
                    if (fingerCount > 3 && !isHand) return;

                    long duration = (DateTime.Now.Ticks - taskSwitcherStartTime)/10000;

                    if (duration > 300)
                    {
                        circleProgress = 2;
                        //if ((int)(circle.Progress * 4) > circleProgress - 1)
                        int stepsScroll = 30;

                        if (isClockwise)
                        {// clockwiseness = "clockwise";
                            stepsScroll = -stepsScroll;


                            byte[] key = new byte[] { (byte)0x27, (byte)0, (byte)0, (byte)0 };
                            formControl.triggerKeyEventNoDelay(key);
                        }
                        else
                        {//clockwiseness = "counterclockwise";

                            byte[] key = new byte[] { (byte)0x25, (byte)0, (byte)0, (byte)0 };
                            formControl.triggerKeyEventNoDelay(key);
                        }
                        // Console.WriteLine(stepsScroll);

                        //if (stepsScroll != 0)
                        //    VScrollWheel(stepsScroll);

                        
                        //Console.WriteLine(stepsScroll);
                    }
                }else if (state.State == FiniteStateMachine.States.WindowAware || state.State == FiniteStateMachine.States.Wake) //windowaware
                {                    
                    long duration = (DateTime.Now.Ticks - taskSwitcherStartTime)/10000;

                    if (duration < 300)
                    {
                        return;
                    }
                    //int stepsScroll = (int)(sweptAngle * fingertipVelocity.Magnitude / 200);

                    //if (stepsScroll < 1)
                    int  stepsScroll = 1;

                    stepsScroll = smoothingScrollData(stepsScroll);


                    if (isClockwise)
                    {// clockwiseness = "clockwise";
                        stepsScroll = -stepsScroll;
                    }
                    else
                    {//clockwiseness = "counterclockwise";
                    }

                    if (fingerCount < 3 && stepsScroll != 0)
                        VScrollWheel(stepsScroll);

                   // Console.WriteLine(stepsScroll);
                }

            }
            catch { }
        }

        public void leapCircleGestureEnd()
        {
            try
            {
                circleGestureInProgress = false;
                //circleProgress = 0;
                //if (state.State == FiniteStateMachine.States.WindowAware || state.State == FiniteStateMachine.States.Wake) //windowaware
                //{
                //    resetScrollSmoothing();
                //}
                //taskSwitcher = false;

                if (FormTutorial.isAudio)
                {
                    if (FormTutorial.tutorialPointableProgress == 4)
                    {
                        formTutorial.setTutorialPointableAnimation(5);
                    }
                    else if (FormTutorial.tutorialPointableProgress == 5)
                    {
                        startTimerTutorialCircle();
                    }
                }
            }
            catch { }
        }

        System.Timers.Timer _timerAfterTutorialCircle;

        private void startTimerTutorialCircle()
        {
            try
            {
                if (_timerAfterTutorialCircle == null)
                {
                    _timerAfterTutorialCircle = new System.Timers.Timer(2000); //300
                    _timerAfterTutorialCircle.Elapsed += new ElapsedEventHandler(_timerAfterTutorialCircle_Elapsed);
                    _timerAfterTutorialCircle.Enabled = true;
                }
                else
                {
                    if (_timerAfterTutorialCircle.Enabled == false)
                        _timerAfterTutorialCircle.Enabled = true;
                }
            }
            catch { }
        }
        void _timerAfterTutorialCircle_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                _timerAfterTutorialCircle.Enabled = false;
                formTutorial.setTutorialPointableAnimation(6);
            }
            catch { }
        }

        bool circleMouseTriggered = false;
        long circleMouseTriggeredPreviousTime = 0;

        int circleProgress = 0;

        public void leapCircleGesture(CircleGesture circle, double sweptAngle, int fingerCount,bool isHand)
        {
            try
            {
                long durationTaskSwitcher = (DateTime.Now.Ticks - taskSwitcherStartTime) / 10000;

                if (durationTaskSwitcher < 300)
                {
                    return;
                }
                //in

                if (state.State == FiniteStateMachine.States.PointableAware)
                {
                    if (pointableSpeakerRef == pointables[objectPointedIndex])
                    {//if pointable aware and speaker is pointed
                        if ((int)(circle.Progress*2) > circleProgress)
                        {
                            if (circle.Pointable.Direction.AngleTo(circle.Normal) <= Math.PI / 4)
                            {// clockwiseness = "clockwise";
                                formControl.increaseVolumeHalf();
                                BeginInvoke((MethodInvoker)delegate()
                                {
                                    formControlGroup.drawTriggeredVolume(formControl.masterVolume);
                                });
                            }
                            else
                            {//clockwiseness = "counterclockwise";

                                formControl.decreaseVolumeHalf();
                                BeginInvoke((MethodInvoker)delegate()
                                {
                                    formControlGroup.drawTriggeredVolume(formControl.masterVolume);
                                });
                            }
                            circleProgress = (int)(circle.Progress*2);
                        }
                    }
                }
                else if (state.State == FiniteStateMachine.States.WindowPointable)
                {
                    if (formControl.currentPointable.description.Contains("Speakers"))
                    {
                        if ((int)(circle.Progress * 2) > circleProgress)
                        {
                            if (circle.Pointable.Direction.AngleTo(circle.Normal) <= Math.PI / 4)
                            {// clockwiseness = "clockwise";
                                formControl.increaseVolumeHalf();
                                BeginInvoke((MethodInvoker)delegate()
                                {
                                    formControlGroup.drawTriggeredVolume(formControl.masterVolume);
                                });
                            }
                            else
                            {//clockwiseness = "counterclockwise";

                                formControl.decreaseVolumeHalf();
                                BeginInvoke((MethodInvoker)delegate()
                                {
                                    formControlGroup.drawTriggeredVolume(formControl.masterVolume);
                                });
                            }
                            circleProgress = (int)(circle.Progress * 2);
                        }
                    }
                    else if (formControl.currentPointable.description.Contains("Browser"))
                    {
                        if ((int)(circle.Progress) > circleProgress)
                        {
                            if (circle.Pointable.Direction.AngleTo(circle.Normal) <= Math.PI / 4)
                            {// clockwiseness = "clockwise";
                                BeginInvoke((MethodInvoker)delegate()
                                { //trigger CTRL -       
                                    byte[] key = new byte[] { (byte)0x11, (byte)0xbb, (byte)0, (byte)0 };
                                    formControl.triggerKeyEvent(key);
                                });
                            }
                            else
                            {//clockwiseness = "counterclockwise";
                                BeginInvoke((MethodInvoker)delegate()
                                { //CTRL +                             
                                    byte[] key = new byte[] { (byte)0x11, (byte)0xbd, (byte)0, (byte)0 };
                                    formControl.triggerKeyEvent(key);
                                });
                            }
                            circleProgress = (int)(circle.Progress);
                        }
                    }
                    else if (formControl.currentPointable.description.Contains("Spotify"))
                    {
                        if ((int)(circle.Progress) > circleProgress)
                        {
                            if (circle.Pointable.Direction.AngleTo(circle.Normal) <= Math.PI / 4)
                            {// clockwiseness = "clockwise";
                                BeginInvoke((MethodInvoker)delegate()
                                { //trigger CTRL -       
                                    byte[] key = new byte[] { (byte)0x11, (byte)Tools.VK_UP, (byte)0, (byte)0 };
                                    formControl.triggerKeyEvent(key);
                                });
                            }
                            else
                            {//clockwiseness = "counterclockwise";
                                BeginInvoke((MethodInvoker)delegate()
                                { //CTRL +                             
                                    byte[] key = new byte[] { (byte)0x11, (byte)Tools.VK_DOWN, (byte)0, (byte)0 };
                                    formControl.triggerKeyEvent(key);
                                });
                            }
                            circleProgress = (int)(circle.Progress);
                        }
                    }
                    else
                    {
                        if ((int)(circle.Progress * 8) > circleProgress)
                        {
                            if (circle.Pointable.Direction.AngleTo(circle.Normal) <= Math.PI / 4)
                            {// clockwiseness = "clockwise";
                                BeginInvoke((MethodInvoker)delegate()
                                { //trigger CTRL -      //scroll not working
                                    Tools.keybd_event((byte)0x11, 0x45, 0, 0);
                                    Thread.Sleep(20);
                                    VScrollWheel(50);
                                    Thread.Sleep(20);
                                    Tools.keybd_event((byte)0x11, 0x45, Tools.KEYEVENTF_KEYUP, 0);
                                });
                            }
                            else
                            {//clockwiseness = "counterclockwise";
                                BeginInvoke((MethodInvoker)delegate()
                                { //CTRL +                           
                                    Tools.keybd_event((byte)0x11, 0x45, 0, 0);
                                    Thread.Sleep(20);
                                    VScrollWheel(-50);  
                                    Thread.Sleep(20);
                                    Tools.keybd_event((byte)0x11, 0x45, Tools.KEYEVENTF_KEYUP, 0);
                                });
                            }
                            circleProgress = (int)(circle.Progress * 8);
                        }
                    }
                }
                else if (state.State == FiniteStateMachine.States.WindowAware || state.State == FiniteStateMachine.States.Wake) //windowaware
                {

                    if (newWindowScroll)
                    {
                        Point pointedScreenPosition = checkPointedScreenPosition(); 
                        bool withinScreen = false;
                        foreach (Screen screen in Screen.AllScreens)
                        {
                            if (screen.Bounds.Contains(pointedScreenPosition))
                            {
                                withinScreen = true;
                            }
                        }
                        Rectangle bottomCorner = new Rectangle(Screen.PrimaryScreen.Bounds.Right - 40, Screen.PrimaryScreen.Bounds.Bottom - 40, 41, 41);
                        if (!bottomCorner.Contains(pointedScreenPosition) && withinScreen)
                        {
                            //mouseControlAbsolute(pointedScreenPosition.X, pointedScreenPosition.Y);
                            circleMouseTriggeredPreviousTime = DateTime.Now.Ticks;
                        }
                        newWindowScroll = false;
                    }
                    // if (circle.Progress < 1) resetScrollSmoothing();
                    if (circle.Progress > 3 && !circleMouseTriggered)
                    {//simulate cursor on pointing position
                        circleMouseTriggered = true;
                        Point pointedScreenPosition = checkPointedScreenPosition();
                        long duration = (DateTime.Now.Ticks - circleMouseTriggeredPreviousTime) / 10000000; //seconds

                        if (duration > 3) //10 seconds
                        {//every 3 seconds
                            //if far away only trigger
                            Point cursorPosition = System.Windows.Forms.Control.MousePosition;//this.PointToScreen(System.Windows.Input.Mouse.GetPosition((this));
                           
                            if (Math.Sqrt(Math.Pow(cursorPosition.X - pointedScreenPosition.X, 2) + Math.Pow(cursorPosition.Y - pointedScreenPosition.Y, 2)) > 100)
                            {
                                bool withinScreen = false;
                                foreach (Screen screen in Screen.AllScreens)
                                {
                                    if (screen.Bounds.Contains(cursorPosition))
                                    {
                                        withinScreen = true;
                                    }
                                }
                                Rectangle bottomCorner = new Rectangle(Screen.PrimaryScreen.Bounds.Right - 40, Screen.PrimaryScreen.Bounds.Bottom - 40, 41, 41);
                                if (!bottomCorner.Contains(pointedScreenPosition) && withinScreen)
                                {
                                   // mouseControlAbsolute(pointedScreenPosition.X, pointedScreenPosition.Y);
                                    circleMouseTriggeredPreviousTime = DateTime.Now.Ticks;
                                }
                            }
                        }
                    }
                    else if (circle.Progress < 3)
                    {
                        circleMouseTriggered = false;
                    }

                    int stepsScroll = (int)(sweptAngle * fingertipVelocity.Magnitude / 200);


                    if (stepsScroll < 1)
                        stepsScroll = 1;
                    
                    stepsScroll = smoothingScrollData(stepsScroll);

                    
                    if (circle.Pointable.Direction.AngleTo(circle.Normal) <= Math.PI / 4)
                    {// clockwiseness = "clockwise";
                        stepsScroll = -stepsScroll;
                    }
                    else
                    {//clockwiseness = "counterclockwise";
                    }

                    if (state.State == FiniteStateMachine.States.WindowAware)
                    {
                        if (FormFindWindow.checkWindowType(handle) == FormFindWindow.WindowType.vlc)
                        {
                            if ((int)(circle.Progress*2) > circleProgress)
                            {
                                stepsScroll = 1;

                                if (circle.Pointable.Direction.AngleTo(circle.Normal) <= Math.PI / 4)
                                {// clockwiseness = "clockwise";
                                }
                                else
                                {//clockwiseness = "counterclockwise";
                                    stepsScroll = -stepsScroll;
                                }
                            }
                            else
                            {
                                stepsScroll = 0;
                            }
                            circleProgress = (int)(circle.Progress*2);
                        }else if (FormFindWindow.checkWindowType(handle) == FormFindWindow.WindowType.Powerpoint)
                        {
                            if ((int)(circle.Progress*1) > circleProgress)
                            {
                                stepsScroll = 1;

                                if (circle.Pointable.Direction.AngleTo(circle.Normal) <= Math.PI / 4)
                                {// clockwiseness = "clockwise";
                                    BeginInvoke((MethodInvoker)delegate()
                                    { //trigger CTRL -       
                                        byte[] key = new byte[] { (byte)Tools.VK.VK_NEXT, (byte)0, (byte)0, (byte)0 };
                                        formControl.triggerKeyEvent(key);
                                    });
                                }
                                else
                                {//clockwiseness = "counterclockwise";
                                    BeginInvoke((MethodInvoker)delegate()
                                    { //CTRL +                             
                                        byte[] key = new byte[] { (byte)Tools.VK.VK_PRIOR, (byte)0, (byte)0, (byte)0 };
                                        formControl.triggerKeyEvent(key);
                                    });
                                }
                            }
                            else
                            {
                                stepsScroll = 0;
                            }
                            circleProgress = (int)(circle.Progress*1);
                        } 
                    }
                    else if (taskSwitcher)
                    {
                        
                        if (fingerCount > 3 && !isHand) return;

                        if ((int)(circle.Progress * 2) > circleProgress)
                        {
                            if (fingertipVelocity.Magnitude > 1000) return;

                            stepsScroll = 30;

                            if (circle.Pointable.Direction.AngleTo(circle.Normal) <= Math.PI / 4)
                            {// clockwiseness = "clockwise";
                                byte[] key = new byte[] { (byte)0x27, (byte)0, (byte)0, (byte)0 };
                                formControl.triggerKeyEventNoDelay(key);

                            }
                            else
                            {//clockwiseness = "counterclockwise";

                                byte[] key = new byte[] { (byte)0x25, (byte)0, (byte)0, (byte)0 };
                                formControl.triggerKeyEventNoDelay(key);
                            }


                            circleProgress = (int)(circle.Progress * 2);
                        }
                        else
                        {
                            stepsScroll = 0;
                        }

                        //Console.WriteLine(fingerCount + "  " + circle.Progress + "  " + stepsScroll);

                        //if (stepsScroll != 0)
                        //    VScrollWheel(stepsScroll);



                        return;
                    }


                    if (fingerCount < 3 && stepsScroll != 0)
                        VScrollWheel(stepsScroll);


                    //Console.WriteLine(stepsScroll);
                }
            }
            catch { }
        }

        bool clickTriggerInProgress = false;
        bool filterTaps = false;

        public void leapTapGesture(bool checkDistance)
        {
            try
            {
               // if (!filterTaps) checkDistance = false;

                currentGestureDown = GestureDownStates.Initial;
                currentGestureRight = GestureRightStates.Initial;
                currentGestureLeft = GestureRightStates.Initial;

                bool triggerGesture;

                if (taskSwitcher && !circleGestureInProgress)
                {
                    triggerGesture = false;

                    double lowestX = pastFingerPositions[0].X;
                    double highestX = pastFingerPositions[0].X;

                    for (int i = 0; i < pastFingerPositions.Count -1; i++)
                    {
                        long durationDistance = (long)((DateTime.Now.Ticks - pastFingerPositionsTiming[i]) / 10000);

                        if (durationDistance < 200)
                        {
                            if (pastFingerPositions[i].X < lowestX)
                                lowestX = pastFingerPositions[i].X;

                            if (pastFingerPositions[i].X > highestX)
                                highestX = pastFingerPositions[i].X;

                            if (!triggerGesture && Math.Abs(pastFingerPositions[pastFingerPositions.Count - 1].Y - pastFingerPositions[i].Y) > 15) //1.0 25
                            {
                                triggerGesture = true;
                                break;
                            }
                        }
                    }

                    if (triggerGesture)// && (Math.Abs(highestX - lowestX) < 30))
                    {

                        long durationTaskSwitcher = (DateTime.Now.Ticks - taskSwitcherStartTime) / 10000;

                        if (durationTaskSwitcher < 300)
                        {
                            return;
                        }


                        taskSwitcherStartTime = DateTime.Now.Ticks;

                        byte[] key = new byte[] { (byte)0x0D, (byte)0, (byte)0, (byte)0 };
                        formControl.triggerKeyEvent(key);

                       // taskSwitcher = false;
                    }
                    else
                        triggerGesture = false;

                    // BeginInvoke((MethodInvoker)delegate()
                    // { //trigger enter      
                    //});
                    return;
                }

                long durationPrevious = (long)((DateTime.Now.Ticks - cursorPreviousClickTime) / 10000);

                triggerGesture = false;

                if (durationPrevious < 2200 && formControl.windowCursorDropForm.Visible && !circleGestureInProgress)
                {
                    triggerGesture = true;
                }

                if (checkDistance && triggerGesture)
                {
                    triggerGesture = false;

                    double lowestX = pastFingerPositions[0].X;
                    double highestX = pastFingerPositions[0].X;

                    for (int i = 0; i < pastFingerPositions.Count -1; i++)
                    {
                        long durationDistance = (long)((DateTime.Now.Ticks - pastFingerPositionsTiming[i]) / 10000);

                        if (durationDistance < 200)
                        {
                            if (pastFingerPositions[i].X < lowestX)
                                lowestX = pastFingerPositions[i].X;

                            if (pastFingerPositions[i].X > highestX)
                                highestX = pastFingerPositions[i].X;

                            if (!triggerGesture && Math.Abs(pastFingerPositions[pastFingerPositions.Count - 1].Y - pastFingerPositions[i].Y) > 25)
                            {
                                triggerGesture = true;
                                break;
                            }
                        }
                    }

                    if((Math.Abs(highestX-lowestX) < 30))
                        triggerGesture = true;
                    else
                        triggerGesture = false;

                    //if (triggerGesture && (Math.Abs(highestX-lowestX) < 30))
                    //    triggerGesture = true;
                    //else
                    //    triggerGesture = false;
                }

                if (!Tools.modeClickLeft)
                    triggerGesture = false;

                if (triggerGesture )
                {
                    //dropcursor again
                    cursorPreviousClickTime = DateTime.Now.Ticks;
                    //triggerMouseMoveAndClick(cursorPreviousClick, 0);

                    //triggerMouseMove(cursorPreviousClick.X - 1, cursorPreviousClick.Y - 1);

                    clickTriggerInProgress = true;

                    BeginInvoke((MethodInvoker)delegate()
                    {
                        try
                        {
                            //formControl.windowCursorForm.Hide();

                            //if (formControl.windowCursorDropForm.Visible)
                            //    formControl.windowCursorDropForm.Hide();

                            //if (formControl.windowCursorDropFormClick.Visible)
                            //    formControl.windowCursorDropFormClick.Hide();
                            formControl.drawWindowCursorDropAbsolute(cursorPreviousClick.X, cursorPreviousClick.Y);
                            formControl.drawWindowCursorDropClickAbsolute(cursorPreviousClick.X, cursorPreviousClick.Y, true);

                            triggerMouseMoveAndClick(cursorPreviousClick, 0);
                            
                         
                            //formControl.windowCursorForm.Visible = true;

                            clickTriggerInProgress = false;
                        }
                        catch { }
                    });
                }
            }
            catch { }
        }
        public void leapTapRightGesture()
        {
            try
            {
                currentGestureDown = GestureDownStates.Initial;
                currentGestureRight = GestureRightStates.Initial;
                currentGestureLeft = GestureRightStates.Initial;

                long durationPrevious = (long)((DateTime.Now.Ticks - cursorPreviousClickTime) / 10000);

                bool triggerGesture = false;
                if (durationPrevious < 2200 && formControl.windowCursorDropForm.Visible && !circleGestureInProgress)
                {
                    triggerGesture = true;
                }


                if (triggerGesture)
                {
                    triggerGesture = false;


                    double lowestY = pastFingerPositions[0].Y;
                    double highestY = pastFingerPositions[0].Y;

                    double lowestX = pastFingerPositions[0].X;
                    double highestX = pastFingerPositions[0].X;

                    int countLeft = 0;
                    int countRight = 0;

                    for (int i = 1; i < pastFingerPositions.Count; i++)
                    {
                        long durationDistance = (long)((DateTime.Now.Ticks - pastFingerPositionsTiming[i]) / 10000);

                        if (durationDistance < 200)
                        { //find the difference between lowest and highest y
                            if (pastFingerPositions[i].Y < lowestY)
                                lowestY = pastFingerPositions[i].Y;

                            if (pastFingerPositions[i].Y > highestY)
                                highestY = pastFingerPositions[i].Y;

                            if (pastFingerPositions[i].X < lowestX)
                                lowestX = pastFingerPositions[i].X;

                            if (pastFingerPositions[i].X > highestY)
                                highestX = pastFingerPositions[i].X;

                            if (pastFingerPositions[i].X > pastFingerPositions[pastFingerPositions.Count - 1].X)
                                countRight++;
                            else
                                countLeft++;

                            //if (Math.Abs(pastFingerPositions[pastFingerPositions.Count - 1].Y - pastFingerPositions[i].Y) > 25)
                            //{
                            //    triggerGesture = true;
                            //    break;
                            //}
                        }
                    }

                    if (Math.Abs(lowestY - highestY) < 20 && countRight > countLeft)// && Math.Abs(highestX- lowestX) > 10)
                        triggerGesture = true;
                }


                if (!Tools.modeClickRight)
                    triggerGesture = false;

                if (triggerGesture)
                {
                    //dropcursor again
                    cursorPreviousClickTime = DateTime.Now.Ticks;
                    triggerMouseMoveAndClick(cursorPreviousClick, 1);

                    BeginInvoke((MethodInvoker)delegate()
                    {
                        try
                        {
                            formControl.drawWindowCursorDropAbsolute(cursorPreviousClick.X, cursorPreviousClick.Y);
                            formControl.drawWindowCursorDropClickAbsolute(cursorPreviousClick.X, cursorPreviousClick.Y, true);
                            triggerMouseMove(cursorPreviousClick.X, cursorPreviousClick.Y);
                        }
                        catch { }
                    });
                }
            }
            catch { }
        }
        public void leapTapLeftGesture()
        {
            try
            {
                currentGestureDown = GestureDownStates.Initial;
                currentGestureRight = GestureRightStates.Initial;
                currentGestureLeft = GestureRightStates.Initial;

                long durationPrevious = (long)((DateTime.Now.Ticks - cursorPreviousClickTime) / 10000);

                bool triggerGesture = false;
                if (durationPrevious < 2200 && formControl.windowCursorDropForm.Visible && !circleGestureInProgress)
                {
                    triggerGesture = true;
                }


                if (triggerGesture)
                {
                    triggerGesture = false;

                    double lowestY = pastFingerPositions[0].Y;
                    double highestY = pastFingerPositions[0].Y;

                    double lowestX = pastFingerPositions[0].X;
                    double highestX = pastFingerPositions[0].X;

                    int countLeft = 0;
                    int countRight = 0;

                    for (int i = 1; i < pastFingerPositions.Count; i++)
                    {
                        long durationDistance = (long)((DateTime.Now.Ticks - pastFingerPositionsTiming[i]) / 10000);

                        if (durationDistance < 200)
                        { //find the difference between lowest and highest y
                            if (pastFingerPositions[i].Y < lowestY)
                                lowestY = pastFingerPositions[i].Y;

                            if (pastFingerPositions[i].Y > highestY)
                                highestY = pastFingerPositions[i].Y;

                            if (pastFingerPositions[i].X < lowestX)
                                lowestX = pastFingerPositions[i].X;

                            if (pastFingerPositions[i].X > highestY)
                                highestX = pastFingerPositions[i].X;

                            if (pastFingerPositions[i].X > pastFingerPositions[pastFingerPositions.Count - 1].X)
                                countRight++;
                            else
                                countLeft++;
                            //if (Math.Abs(pastFingerPositions[pastFingerPositions.Count - 1].Y - pastFingerPositions[i].Y) > 25)
                            //{
                            //    triggerGesture = true;
                            //    break;
                            //}
                        }
                    }


                    if (Math.Abs(lowestY - highestY) < 20 && countLeft > countRight && Math.Abs(highestX- lowestX) > 10)
                        triggerGesture = true;
                }



                if (!Tools.modeClickMiddle)
                    triggerGesture = false;

                if (triggerGesture)
                {
                    //dropcursor again
                    cursorPreviousClickTime = DateTime.Now.Ticks;
                    triggerMouseMoveAndClick(cursorPreviousClick, 2);

                    BeginInvoke((MethodInvoker)delegate()
                    {
                        try
                        {
                            formControl.drawWindowCursorDropAbsolute(cursorPreviousClick.X, cursorPreviousClick.Y);
                            formControl.drawWindowCursorDropClickAbsolute(cursorPreviousClick.X, cursorPreviousClick.Y, false);
                            triggerMouseMove(cursorPreviousClick.X, cursorPreviousClick.Y);
                        }
                        catch { }
                    });
                }
            }
            catch { }
        }
        //List<Vector3> fingerPositionHistory = new List<Vector3>();
        //List<Vector3> fingerPositionVelocityHistory = new List<Vector3>();
        //List<long> fingerPositionHistoryTime = new List<long>();
        //const int MAX_HISTORY = 100;


        enum GestureDownStates {Initial, DownFast, Switch, UpFast, SlowEnd };

        GestureDownStates currentGestureDown = GestureDownStates.Initial;
        Vector3 fingerPositionStartGesture;

        private void checkGestureDown()
        {
            try
            {
                if (currentGestureDown != GestureDownStates.Initial)
                {//check distance 
                    double distanceFromInitial = Vector3.computeDistance(fingerPositionStartGesture, fingertipCoordinate[0]);

                    if (distanceFromInitial > 80)
                        currentGestureDown = GestureDownStates.Initial;

                    if (Math.Abs(fingertipVelocity.X) > 500)//Math.Abs(fingertipVelocity.Y)*2/3)
                    {
                        currentGestureDown = GestureDownStates.Initial;
                    }
                }

                if (fingertipVelocity.Y < -300 && Math.Abs(fingertipVelocity.X) < 300 && (fingertipCoordinate[0].Y < fingerPositionStartGesture.Y)) //faster than 300mm/s downwards
                {
                    if (currentGestureDown == GestureDownStates.Initial)
                    {
                        currentGestureDown = GestureDownStates.DownFast;
                    }
                    else if (currentGestureDown != GestureDownStates.DownFast && currentGestureDown != GestureDownStates.Switch)
                    {
                        currentGestureDown = GestureDownStates.Initial;
                    }
                }

                if (fingertipVelocity.Y > 250 && Math.Abs(fingertipVelocity.X) < 300 && (fingertipCoordinate[0].Y < fingerPositionStartGesture.Y)) //faster than 300mm/s upwards
                {
                    if (currentGestureDown == GestureDownStates.DownFast || currentGestureDown == GestureDownStates.Switch)
                    {
                        currentGestureDown = GestureDownStates.UpFast;
                    }
                    else if (currentGestureDown != GestureDownStates.UpFast && currentGestureDown != GestureDownStates.SlowEnd)
                    {
                        currentGestureDown = GestureDownStates.Initial;
                    }
                }

                if (Math.Abs(fingertipVelocity.Y) < 100)
                {
                    if (currentGestureDown == GestureDownStates.DownFast)
                    {
                        currentGestureDown = GestureDownStates.Switch;
                    }
                    else if (currentGestureDown == GestureDownStates.UpFast)
                    {
                        double distanceFromInitial = Vector3.computeDistance(fingerPositionStartGesture, fingertipCoordinate[0]);
                        if (Math.Abs(fingertipVelocity.Y) < 50 && distanceFromInitial < 20) //3cm
                        {
                            currentGestureDown = GestureDownStates.Initial;
                            leapTapGesture(false);
                        }
                        else if (distanceFromInitial > 40) //10cm
                        {
                            currentGestureDown = GestureDownStates.Initial;
                        }
                    }
                    else if (currentGestureDown != GestureDownStates.Initial && currentGestureDown != GestureDownStates.Switch)
                    {
                        currentGestureDown = GestureDownStates.Initial;
                    }

                    if (currentGestureDown == GestureDownStates.Initial)
                    {
                        fingerPositionStartGesture.X = fingertipCoordinate[0].X;
                        fingerPositionStartGesture.Y = fingertipCoordinate[0].Y;
                        fingerPositionStartGesture.Z = fingertipCoordinate[0].Z;
                    }

                }
            }
        
             catch{}
        }


        enum GestureRightStates { Initial, RightFast, Switch, LeftFast, SlowEnd };

        GestureRightStates currentGestureRight = GestureRightStates.Initial;
        Vector3 fingerPositionStartGestureRight;

        private void checkGestureRight()
        {
            try
            {
                double distanceFromInitial = Vector3.computeDistance(fingerPositionStartGestureRight, fingertipCoordinate[0]);

                if (currentGestureRight != GestureRightStates.Initial)
                {//check distance 

                    if (distanceFromInitial > 80)
                        currentGestureRight = GestureRightStates.Initial;

                    if (Math.Abs(fingertipVelocity.Y) > 500)//Math.Abs(fingertipVelocity.Y)*2/3)
                    {
                        currentGestureRight = GestureRightStates.Initial;
                    }
                }

                double minVelocity = 150;
                switch (Tools.TapRightSensitivity)
                {
                    case 0:
                        minVelocity = 300;
                        break;
                    case 1:
                        minVelocity = 250;
                        break;
                    case 2:
                        minVelocity = 200;
                        break;
                    case 3:
                        minVelocity = 180;
                        break;
                    case 4:
                        minVelocity = 170;
                        break;
                    case 5:
                        minVelocity = 150;
                        break;
                    case 6:
                        minVelocity = 135;
                        break;
                }

                if (fingertipVelocity.X > minVelocity && Math.Abs(fingertipVelocity.Y) < 300 && distanceFromInitial >= 0 && (fingertipCoordinate[0].X > fingerPositionStartGestureRight.X)) //faster than 300mm/s downwards
                {
                    if (currentGestureRight == GestureRightStates.Initial)
                    {
                        currentGestureRight = GestureRightStates.RightFast;

                    }
                    else if (currentGestureRight != GestureRightStates.RightFast && currentGestureRight != GestureRightStates.Switch)
                    {
                        currentGestureRight = GestureRightStates.Initial;
                    }
                }

                if (fingertipVelocity.X < -minVelocity && Math.Abs(fingertipVelocity.Y) < 300 && (fingertipCoordinate[0].X > fingerPositionStartGestureRight.X)) //faster than 300mm/s upwards
                {
                    if (currentGestureRight == GestureRightStates.RightFast || currentGestureRight == GestureRightStates.Switch)
                    {
                        currentGestureRight = GestureRightStates.LeftFast;
                        currentGestureLeft = GestureRightStates.Initial;
                    }
                    else if (currentGestureRight != GestureRightStates.LeftFast && currentGestureRight != GestureRightStates.SlowEnd)
                    {
                        currentGestureRight = GestureRightStates.Initial;
                    }
                }

              //  if ((fingertipCoordinate[0].X > fingerPositionStartGestureRight.X)

                if (Math.Abs(fingertipVelocity.X) < 100)
                {
                    if (currentGestureRight == GestureRightStates.RightFast)
                    {
                        currentGestureRight = GestureRightStates.Switch;
                    }
                    else if (currentGestureRight == GestureRightStates.LeftFast)
                    {
                        //distanceFromInitial = Vector3.computeDistance(fingerPositionStartGestureRight, fingertipCoordinate[0]);
                        if (Math.Abs(fingertipVelocity.X) < 50 && distanceFromInitial < 20) //3cm
                        {
                            currentGestureRight = GestureRightStates.Initial;
                            
                            leapTapRightGesture();
                        }
                        else if (distanceFromInitial > 40) //10cm
                        {
                            currentGestureRight = GestureRightStates.Initial;
                        }
                    }
                    else if (currentGestureRight != GestureRightStates.Initial && currentGestureRight != GestureRightStates.Switch)
                    {
                        currentGestureRight = GestureRightStates.Initial;
                    }

                    if (currentGestureRight == GestureRightStates.Initial)
                    {
                        fingerPositionStartGestureRight.X = fingertipCoordinate[0].X;
                        fingerPositionStartGestureRight.Y = fingertipCoordinate[0].Y;
                        fingerPositionStartGestureRight.Z = fingertipCoordinate[0].Z;
                    }

                }
            }
            catch { }
        }

       // enum GestureRightStates { Initial, RightFast, Switch, LeftFast, SlowEnd };

        GestureRightStates currentGestureLeft = GestureRightStates.Initial;
        Vector3 fingerPositionStartGestureLeft;

        private void checkGestureLeft()
        {
            //return;// disabled
            try
            {
                double distanceFromInitial = Vector3.computeDistance(fingerPositionStartGestureLeft, fingertipCoordinate[0]);
                if (currentGestureLeft != GestureRightStates.Initial)
                {//check distance 

                    if (distanceFromInitial > 80)
                        currentGestureLeft = GestureRightStates.Initial;

                    if (Math.Abs(fingertipVelocity.Y) > 500)//Math.Abs(fingertipVelocity.Y)*2/3)
                    {
                        currentGestureLeft = GestureRightStates.Initial;
                    }
                }

                double minVelocity = 150;
                switch (Tools.TapLeftSensitivity)
                {
                    case 0:
                        minVelocity = 300;
                        break;
                    case 1:
                        minVelocity = 250;
                        break;
                    case 2:
                        minVelocity = 200;
                        break;
                    case 3:
                        minVelocity = 180;
                        break;
                    case 4:
                        minVelocity = 170;
                        break;
                    case 5:
                        minVelocity = 150;
                        break;
                    case 6:
                        minVelocity = 135;
                        break;
                }

                if (fingertipVelocity.X < -minVelocity && Math.Abs(fingertipVelocity.Y) < 300 && distanceFromInitial >= 0 && (fingertipCoordinate[0].X < fingerPositionStartGestureRight.X)) //faster than 300mm/s downwards
                {
                    if (currentGestureLeft == GestureRightStates.Initial)
                    {
                        currentGestureLeft = GestureRightStates.RightFast;

                    }
                    else if (currentGestureLeft != GestureRightStates.RightFast && currentGestureLeft != GestureRightStates.Switch)
                    {
                        currentGestureLeft = GestureRightStates.Initial;
                    }
                }

                if (fingertipVelocity.X > minVelocity && Math.Abs(fingertipVelocity.Y) < 300 && (fingertipCoordinate[0].X < fingerPositionStartGestureRight.X)) //faster than 300mm/s upwards
                {
                    if (currentGestureLeft == GestureRightStates.RightFast || currentGestureLeft == GestureRightStates.Switch)
                    {
                        currentGestureLeft = GestureRightStates.LeftFast;


                        currentGestureRight = GestureRightStates.Initial;
                    }
                    else if (currentGestureLeft != GestureRightStates.LeftFast && currentGestureLeft != GestureRightStates.SlowEnd)
                    {
                        currentGestureLeft = GestureRightStates.Initial;
                    }
                }

                if (Math.Abs(fingertipVelocity.X) < 100)
                {
                    if (currentGestureLeft == GestureRightStates.RightFast)
                    {
                        currentGestureLeft = GestureRightStates.Switch;
                    }
                    else if (currentGestureLeft == GestureRightStates.LeftFast)
                    {
                        //distanceFromInitial = Vector3.computeDistance(fingerPositionStartGestureLeft, fingertipCoordinate[0]);
                        if (Math.Abs(fingertipVelocity.X) < 50 && distanceFromInitial < 25) //3cm
                        {
                            currentGestureLeft = GestureRightStates.Initial;
                            leapTapLeftGesture();
                        }
                        else if (distanceFromInitial > 40) //10cm
                        {
                            currentGestureLeft = GestureRightStates.Initial;
                        }
                    }
                    else if (currentGestureLeft != GestureRightStates.Initial && currentGestureLeft != GestureRightStates.Switch)
                    {
                        currentGestureLeft = GestureRightStates.Initial;
                    }

                    if (currentGestureLeft == GestureRightStates.Initial)
                    {
                        fingerPositionStartGestureLeft.X = fingertipCoordinate[0].X;
                        fingerPositionStartGestureLeft.Y = fingertipCoordinate[0].Y;
                        fingerPositionStartGestureLeft.Z = fingertipCoordinate[0].Z;
                    }

                }
            }
            catch { }
        }

        Vector3 initialPointedObjectPosition;

        private int checkPointedObject()
        {//TODO
            //check for finger pointing on object
            try
            {
                double[] distances = new double[pointables.Count];

                Vector3 nearestPoint = new Vector3();

                for (int i = 0; i < pointables.Count; i++)                
                {
                    if (pointables[i].isEnabled && pointables[i].calibrated && pointables[i].type == PointableObject.PointableType.Physical)
                    {
                        distances[i] = getAngleBetweenVectors(fingertipCoordinateOut[0] - fingertipCoordinate[0], pointables[i].position - fingertipCoordinate[0]);//Vector3.distancePointToLine(pointables[i].position, fingertipCoordinate[0], fingertipCoordinateOut[0]);

                        nearestPoint = Vector3.nearestPointonLine(pointables[i].position, fingertipCoordinate[0], fingertipCoordinateOut[0]);
                        if (Vector3.computeDistance(nearestPoint, fingertipCoordinate[0]) > Vector3.computeDistance(nearestPoint, fingertipCoordinateOut[0]))
                        {
                        }
                        else
                        {
                            distances[i] = 100000;
                        }
                    }
                    else
                    {
                        distances[i] = 100000;
                    }
                }


                int smallest = 0;
                for (int i = 1; i < pointables.Count; i++)
                {
                    if (distances[i] < distances[smallest])
                    {
                        smallest = i;
                    }
                }

                //if (smallest != currentObjectHoverIndex && currentObjectHoverIndex>=0)
                //{ 
                //    if (distances[smallest] < distances[currentObjectHoverIndex] - 90) //30
                //    { //new direction closer than original object
                //        smallest = smallest;
                //    }
                //    else
                //    {
                //        smallest = currentObjectHoverIndex;
                //    }
                //}

                double smallestDistance = distances[smallest];


                if (smallestDistance < 30  || (smallest == currentObjectHoverIndex && smallestDistance < 90))//thresholdVariable)
                {
                    Debug.WriteLine(smallestDistance);
                }
                else
                {
                    smallest = -2;
                }

                if (pointedObjectIndexAfterWindowControl == smallest)
                    return -2;

                return smallest;
            }
            catch { }
            return -2;
        }
        
        //PointableObject previousObjectHover;
        PointableObject currentObjectHover;
        int currentObjectHoverIndex;
       
        
        private int checkHoveredObject()
        {//TODO
            //check for finger pointing on object
            try
            {
                if (fingertipVelocity.Magnitude > 200)////detect first area
                {
                    return -2;
                }

                double[] distances = new double[pointables.Count];

                Vector3 nearestPoint = new Vector3();

                for (int i = 0; i < pointables.Count; i++)
                {
                    if (pointables[i].isEnabled && pointables[i].calibrated)
                    {
                        distances[i] = getAngleBetweenVectors(palmCoordinatesOut - palmCoordinates, pointables[i].position - palmCoordinates);//Vector3.distancePointToLine(pointables[i].position, fingertipCoordinate[0], fingertipCoordinateOut[0]);

                        nearestPoint = Vector3.nearestPointonLine(pointables[i].position, palmCoordinates, palmCoordinatesOut);
                        if (Vector3.computeDistance(nearestPoint, palmCoordinates) > Vector3.computeDistance(nearestPoint, palmCoordinatesOut))
                        {
                        }
                        else
                        {
                            distances[i] = 100000;
                        }
                    }
                    else
                    {
                        distances[i] = 100000;
                    }
                }

                int smallest = 0;
                for (int i = 1; i < pointables.Count; i++)
                {
                    if (distances[i] < distances[smallest])
                    {
                        smallest = i;
                    }
                }
                
                double smallestDistance = distances[smallest];


                if (smallest != currentObjectHoverIndex && currentObjectHoverIndex >= 0)
                {
                    if (distances[smallest] < distances[currentObjectHoverIndex] - 7) //7
                    {
                        smallest = smallest;
                    }
                    else
                    {
                        smallest = currentObjectHoverIndex;
                    }
                }

                if (smallestDistance < 30)//thresholdVariable)
                {
                    currentObjectHover = pointables[smallest];
                    currentObjectHoverIndex = smallest;

                    formControl.setObjectHover(currentObjectHover);

                }
                else
                {
                    smallest = -2;

                    currentObjectHover = null;
                    currentObjectHoverIndex = -2;

                    formControl.setObjectHover(currentObjectHover);
                }
                return smallest;
            }
            catch { }
            return -2;
        }

        private double getAngleBetweenVectors(Vector3 vectorA, Vector3 vectorB)
        {
            double cosTheta, angle;

            try
            {
                cosTheta = Vector3.DotProduct(vectorA, vectorB) / (vectorA.Magnitude * vectorB.Magnitude);
                angle = Math.Acos(cosTheta);
            }
            catch { angle = 100000; }

            return angle * 57.3;
        }

        Bitmap handleBitmap = null;
        IntPtr handleBitmapHandle = IntPtr.Zero;

        IntPtr handle;
        IntPtr tempPointedWindow;

        long startTimePointedWindow;
        Point currentPointedWindowPosition;

        long windowControlBeforeLetGoTime;

        Rectangle windowResizeBounds;
        int windowResizeBoundsType; //0-no-resize 1-fixed size 2-maximized

        internal bool scrollMode = false;

        
        Rectangle screenRect = new Rectangle(int.MaxValue, int.MaxValue, int.MinValue, int.MinValue);

        private void checkControl()
        {
            try
            {
                if (state.State == FiniteStateMachine.States.Standby)
                {
                    //do nothing
                }
                else if (state.State == FiniteStateMachine.States.Wake)
                {
                    if (fingers.Count == 1 || fingers.Count == 2)// || (currentObjectHover !=null && fingers.Count == 2)) //thumb problem//update in future if know is finger or thumb TODO
                    {
                        if (fingertipVelocity.Magnitude < 1000)
                        {
                            bool pointWithinScreen = false;

                            Point currentPointedWindowPosition = checkPointedScreenPosition();
                            int pixelAllowance = 150;
                            if (tutorialInProgress)
                                pixelAllowance = 2000;

                            foreach (Screen screen in Screen.AllScreens)
                            {
                                Rectangle screenBounds = screen.Bounds;
                                screenBounds.Width += pixelAllowance * 2;
                                screenBounds.Height += pixelAllowance * 2 - 50;
                                screenBounds.X -= pixelAllowance;
                                screenBounds.Y -= pixelAllowance;

                                if (screenBounds.Contains(currentPointedWindowPosition))
                                {
                                    pointWithinScreen = true;
                                    break;
                                }
                            }

                            if (pointWithinScreen)
                            {
                                if (!formControl.windowCursorForm.Visible)
                                {
                                    formControl.windowCursorForm.Show();
                                    Tools.SetWindowPos(formControl.windowCursorForm.Handle, Tools.HWND.TOPMOST, 0, 0, 0, 0, Tools.SetWindowPosFlags.IgnoreMove | Tools.SetWindowPosFlags.IgnoreResize | Tools.SetWindowPosFlags.DoNotActivate);     
                                  //  formControl.windowCursorForm.TopMost = true;
                                }

                                if (currentObjectHover != null)
                                {
                                    currentObjectHover = null;
                                    currentObjectHoverIndex = -2;
                                    //formControl.setObjectHover(currentObjectHover);
                                }

                                if (formControlGroup.Visible)
                                    formControlGroup.Hide();

                                if (screenRect.X == int.MaxValue)
                                {
                                    foreach (Screen screen in Screen.AllScreens)
                                        screenRect = Rectangle.Union(screenRect, screen.Bounds);
                                }

                                int margin = 10;

                                if (currentPointedWindowPosition.X < screenRect.Left + margin)
                                    currentPointedWindowPosition.X = screenRect.Left + margin;
                                if (currentPointedWindowPosition.X > screenRect.Right - margin)
                                    currentPointedWindowPosition.X = screenRect.Right - margin;
                                if (currentPointedWindowPosition.Y < screenRect.Top + margin)
                                    currentPointedWindowPosition.Y = screenRect.Top + margin;
                                if (currentPointedWindowPosition.Y > screenRect.Bottom - margin)
                                    currentPointedWindowPosition.Y = screenRect.Bottom - margin;


                                IntPtr pointedWindow = IntPtr.Zero;// = checkPointedWindow(currentPointedWindowPosition);
                                if (pinchGestureState == PinchGestureStates.None)
                                {
                                    pointedWindow = checkPointedWindow(currentPointedWindowPosition);
                                   // checkMouseClick(currentPointedWindowPosition);
                                }

                                if (pointedWindow != IntPtr.Zero && pointedWindow != null)
                                {
                                    if (fingertipVelocity.Magnitude < 30 & !scrollMode) //093-15
                                    {
                                        if (handle != pointedWindow)
                                        {
                                            startTimePointedWindow = DateTime.Now.Ticks;
                                        }

                                        handle = pointedWindow;

                                        long duration = DateTime.Now.Ticks - startTimePointedWindow;
                                        duration = duration / 10000;

                                        if (duration > 250) //250. for testing 1000
                                        {//300ms before go to next state
                                            state.ProcessEvent(FiniteStateMachine.Events.FoundWindow);
                                        }
                                    }
                                    else //check scroll mode
                                    {

                                    }
                                }
                            }
                            else
                            {//point not within screen
                                if (formControl.windowCursorForm.Visible)
                                {
                                    Tools.changeCursorWindowsNormal();
                                    formControl.windowCursorForm.Hide();
                                }
                                if (formControl.windowCursorDropForm.Visible)
                                    formControl.windowCursorDropForm.Hide();

                                if (fingers.Count == 1 || (currentObjectHover != null && fingers.Count == 2)) //thumb problem//update in future if know is finger or thumb TODO
                                {
                                    objectPointedIndex = checkPointedObject();

                                    if (objectPointedIndex >= 0)// -1)
                                    {
                                        if (fingertipVelocity.Magnitude < 15) //less than 30mm per second
                                        {
                                            long durationSinceWake = (DateTime.Now.Ticks - fingerTrackStartTime) / 10000;
                                            if (durationSinceWake > 300)
                                            {
                                                state.ProcessEvent(FiniteStateMachine.Events.FoundPointable);
                                            }
                                        }
                                        else if (fingertipVelocity.Magnitude < 200)
                                        {
                                            long durationSinceWake = (DateTime.Now.Ticks - fingerTrackStartTime) / 10000;
                                            if (durationSinceWake > 500)
                                            {
                                                currentObjectHover = pointables[objectPointedIndex];
                                                currentObjectHoverIndex = objectPointedIndex;

                                                formControl.setObjectHover(currentObjectHover);
                                                if (!formControlGroup.Visible)
                                                    formControlGroup.Show();
                                            }
                                        }
                                    }
                                    else
                                    {
                                        currentObjectHover = null;
                                        currentObjectHoverIndex = -2;

                                        formControl.setObjectHover(currentObjectHover);
                                    }
                                }
                                else if (fingers.Count == 2 && currentObjectHover == null)
                                {//copy above
                                    long duration = DateTime.Now.Ticks - startTimeWake;
                                    duration = duration / 10000;

                                    if (duration > 500)
                                    {//allowance for finger detection. not thumb
                                        objectPointedIndex = checkPointedObject();

                                        if (objectPointedIndex >= 0)// -1)
                                        {
                                            if (fingertipVelocity.Magnitude < 15) //less than 30mm per second
                                            {
                                                state.ProcessEvent(FiniteStateMachine.Events.FoundPointable);

                                            }
                                            else if (fingertipVelocity.Magnitude < 200)
                                            {
                                                currentObjectHover = pointables[objectPointedIndex];
                                                currentObjectHoverIndex = objectPointedIndex;

                                                formControl.setObjectHover(currentObjectHover);
                                                if (!formControlGroup.Visible)
                                                    formControlGroup.Show();
                                            }
                                        }
                                    }
                                }
                            }
                        }

                    }
                    else if (fingers.Count == 5)// || (currentObjectHover !=null && fingers.Count == 2)) //thumb problem//update in future if know is finger or thumb TODO
                    {
                    }
                    else if (fingers.Count > 2)
                    {
                        long duration = DateTime.Now.Ticks - startTimeWake;
                        duration = duration / 10000;

                        if (duration > 1000)
                            checkHoveredObject();
                    }

                }
                else if (state.State == FiniteStateMachine.States.WindowControl)
                {
                    if ((fingers.Count == 1 || fingers.Count == 2 || fingers.Count == 3) && fingertipVelocity.Magnitude < 500)// || (currentObjectHover !=null && fingers.Count == 2)) //thumb problem//update in future if know is finger or thumb TODO
                    {
                        Point pointedScreenPositionAbsolute = checkPointedScreenPosition();
                        Point pointedScreenPosition = new Point(pointedScreenPositionAbsolute.X, pointedScreenPositionAbsolute.Y);
                        pointedScreenPosition.X = pointedScreenPosition.X - originalWindowOffset.X;
                        pointedScreenPosition.Y = pointedScreenPosition.Y - originalWindowOffset.Y;

                        //double angleBetweenFingers = 0;
                        double distanceBetweenTips = 0;
                        if (hand.Fingers.Count > 1)
                        {
                            Finger thumb;
                            if (hand.Fingers.Rightmost.Id != hand.Fingers.Frontmost.Id)
                                thumb = hand.Fingers.Rightmost;
                            else
                                thumb = hand.Fingers.Leftmost;


                            Vector3 finger1 = new Vector3(hand.Fingers.Frontmost.StabilizedTipPosition.x,
                                hand.Fingers.Frontmost.StabilizedTipPosition.y, hand.Fingers.Frontmost.StabilizedTipPosition.z);
                            Vector3 finger2 = new Vector3(thumb.StabilizedTipPosition.x,
                                thumb.StabilizedTipPosition.y, thumb.StabilizedTipPosition.z);

                            //angleBetweenFingers = getAngleBetweenVectors(new Vector3(fingers[0].Direction.x, fingers[0].Direction.y, fingers[0].Direction.z),
                            //    new Vector3(fingers[1].Direction.x, fingers[1].Direction.y, fingers[1].Direction.z));

                            distanceBetweenTips = Vector3.computeDistance(finger1, finger2);
                        }


                        if (fingers.Count > 1)// || distanceBetweenTips > fingers.Frontmost.Length + 20 && distanceBetweenTips < 200)//(angleBetweenFingers > 40 && angleBetweenFingers < 120)) //20 degrees
                        {
                            // int distance = (int)Vector3.computeDistance(finger1, finger2) * 10;
                            Rectangle screenBounds = Screen.AllScreens[Tools.ScreenNumber].WorkingArea;
                            bool screenBoundsFound = false;
                            foreach (Screen screen in Screen.AllScreens)
                            {
                                Rectangle screenBoundsFind = screen.WorkingArea;
                                if (screenBoundsFind.Contains(formControlGroup.Bounds.X + formControlGroup.Bounds.Width / 2, formControlGroup.Bounds.Y + formControlGroup.Bounds.Height / 2))
                                {
                                    screenBounds = screenBoundsFind;
                                    screenBoundsFound = true;
                                    break;
                                }
                            }
                            if (!screenBoundsFound)
                            {
                                foreach (Screen screen in Screen.AllScreens)
                                {
                                    Rectangle screenBoundsFind = screen.WorkingArea;
                                    if (screenBoundsFind.Contains(formControlGroup.Bounds.X + formControlGroup.Bounds.Width / 2, formControlGroup.Bounds.Y + formControlGroup.Bounds.Height))
                                    {
                                        screenBounds = screenBoundsFind;
                                        screenBoundsFound = true;
                                        break;
                                    }
                                }
                            }
                            if (!screenBoundsFound)
                            {
                                foreach (Screen screen in Screen.AllScreens)
                                {
                                    Rectangle screenBoundsFind = screen.WorkingArea;
                                    if (screenBoundsFind.Contains(formControlGroup.Bounds.X + formControlGroup.Bounds.Width / 2, formControlGroup.Bounds.Y - formControlGroup.Bounds.Height))
                                    {
                                        screenBounds = screenBoundsFind;
                                        screenBoundsFound = true;
                                        break;
                                    }
                                }
                            }

                            if (screenRect.X == int.MaxValue)
                            {
                                foreach (Screen screen in Screen.AllScreens)
                                    screenRect = Rectangle.Union(screenRect, screen.Bounds);
                            }

                            RECT Rect = new RECT();
                            if (GetWindowRect(handle, ref Rect))
                            {
                                if (pointedScreenPosition.X < screenRect.Left - (Rect.Right - Rect.Left) / 2)//formControlGroup.Bounds.Width / 2)
                                    pointedScreenPosition.X = screenRect.Left - (Rect.Right - Rect.Left) / 2;//formControlGroup.Bounds.Width / 2)

                                if (pointedScreenPosition.X > screenRect.Right - formControlGroup.Bounds.Width / 2)
                                    pointedScreenPosition.X = screenRect.Right - formControlGroup.Bounds.Width / 2;

                                if (pointedScreenPosition.Y < screenRect.Top - formControlGroup.Bounds.Height / 2)
                                    pointedScreenPosition.Y = screenRect.Top - formControlGroup.Bounds.Height / 2;

                                if (pointedScreenPosition.Y > screenRect.Bottom - formControlGroup.Bounds.Height / 2)
                                    pointedScreenPosition.Y = screenRect.Bottom - formControlGroup.Bounds.Height / 2;


                                //  MoveWindow(handle, pointedScreenPosition.X, pointedScreenPosition.Y, Rect.Right - Rect.Left, Rect.Bottom - Rect.Top, true);
                                SetWindowPos(handle, HWND.NOTOPMOST, pointedScreenPosition.X, pointedScreenPosition.Y, Rect.Right - Rect.Left, Rect.Bottom - Rect.Top, 0);

                                formControl.drawWindowCursorAbsolute((int)pointedScreenPositionAbsolute.X, (int)pointedScreenPositionAbsolute.Y, false, fingertipCoordinate[0]);
                            }
                            formControlGroup.updatePosition(new Point(pointedScreenPosition.X + (Rect.Right - Rect.Left) / 2, pointedScreenPosition.Y + (Rect.Bottom - Rect.Top) / 2));


                            if (formControlGroup.Bounds.Right > screenBounds.Right)
                            {
                                if (formControlGroup.Bounds.Top < screenBounds.Top)
                                {
                                    windowResizeBounds = new Rectangle(screenBounds.X + screenBounds.Width / 2, screenBounds.Y, screenBounds.Width / 2, screenBounds.Height / 2);
                                    formDraw.SetBoundsFixed(windowResizeBounds.X, windowResizeBounds.Y, windowResizeBounds.Width, windowResizeBounds.Height);
                                    windowResizeBoundsType = 1;
                                }
                                else if (formControlGroup.Bounds.Bottom > screenBounds.Bottom)
                                {
                                    windowResizeBounds = new Rectangle(screenBounds.X + screenBounds.Width / 2, screenBounds.Y + screenBounds.Height / 2, screenBounds.Width / 2, screenBounds.Height / 2);
                                    formDraw.SetBoundsFixed(windowResizeBounds.X, windowResizeBounds.Y, windowResizeBounds.Width, windowResizeBounds.Height);
                                    windowResizeBoundsType = 1;
                                }
                                else
                                {
                                    windowResizeBounds = new Rectangle(screenBounds.X + screenBounds.Width / 2, screenBounds.Y, screenBounds.Width / 2, screenBounds.Height);
                                    formDraw.SetBoundsFixed(windowResizeBounds.X, windowResizeBounds.Y, windowResizeBounds.Width, windowResizeBounds.Height);
                                    windowResizeBoundsType = 1;
                                }
                            }
                            else if (formControlGroup.Bounds.Left < screenBounds.Left)
                            {
                                if (formControlGroup.Bounds.Top < screenBounds.Top)
                                {
                                    windowResizeBounds = new Rectangle(screenBounds.X, screenBounds.Y, screenBounds.Width / 2, screenBounds.Height / 2);
                                    formDraw.SetBoundsFixed(windowResizeBounds.X, windowResizeBounds.Y, windowResizeBounds.Width, windowResizeBounds.Height);
                                    windowResizeBoundsType = 1;
                                }
                                else if (formControlGroup.Bounds.Bottom > screenBounds.Bottom)
                                {
                                    windowResizeBounds = new Rectangle(screenBounds.X, screenBounds.Y + screenBounds.Height / 2, screenBounds.Width / 2, screenBounds.Height / 2);
                                    formDraw.SetBoundsFixed(windowResizeBounds.X, windowResizeBounds.Y, windowResizeBounds.Width, windowResizeBounds.Height);
                                    windowResizeBoundsType = 1;
                                }
                                else
                                {
                                    windowResizeBounds = new Rectangle(screenBounds.X, screenBounds.Y, screenBounds.Width / 2, screenBounds.Height);
                                    formDraw.SetBoundsFixed(windowResizeBounds.X, windowResizeBounds.Y, windowResizeBounds.Width, windowResizeBounds.Height);
                                    windowResizeBoundsType = 1;
                                }
                            }
                            else if (formControlGroup.Bounds.Top < screenBounds.Top &&
                                formControlGroup.Bounds.Left > screenBounds.Left + 100 && formControlGroup.Bounds.Right < screenBounds.Right - 100)
                            {
                                windowResizeBounds = new Rectangle(screenBounds.X, screenBounds.Y, screenBounds.Width, screenBounds.Height);
                                formDraw.SetBoundsFixed(windowResizeBounds.X, windowResizeBounds.Y, windowResizeBounds.Width, windowResizeBounds.Height);
                                windowResizeBoundsType = 2;
                            }
                            else if (formControlGroup.Bounds.Bottom > screenBounds.Bottom + 200)
                            {
                                if (formControlGroup.Bounds.Left < screenBounds.Left + screenBounds.Width / 2 - 100)
                                {
                                    formDraw.SetBoundsFixed(screenBounds.X, screenBounds.Y + screenBounds.Height, screenBounds.Width, 40);
                                    windowResizeBoundsType = 3;
                                }
                                else
                                {
                                    formDraw.SetBoundsFixed(pointedScreenPosition.X, pointedScreenPosition.Y, Rect.Right - Rect.Left, Rect.Bottom - Rect.Top);
                                    formDraw.BackColor = Color.Red;
                                    formDraw.Opacity = 0.8;
                                    windowResizeBoundsType = 4;
                                }
                            }
                            else
                            {
                                formDraw.SetBoundsFixed(pointedScreenPosition.X, pointedScreenPosition.Y, Rect.Right - Rect.Left, Rect.Bottom - Rect.Top);
                                //windowResizeBounds = new Rectangle (0,0,0,0);
                                windowResizeBoundsType = 0;
                            }

                            Tools.SetWindowPos(formControl.windowCursorForm.Handle, Tools.HWND.TOPMOST, 0, 0, 0, 0, Tools.SetWindowPosFlags.IgnoreMove | Tools.SetWindowPosFlags.IgnoreResize | Tools.SetWindowPosFlags.DoNotActivate);     
 
                            windowControlBeforeLetGoTime = DateTime.Now.Ticks;
                        }
                        else  //equal to one finger/ let go
                        {//let go
                            long durationOneFinger = (DateTime.Now.Ticks - windowControlBeforeLetGoTime) / 10000;

                            if (durationOneFinger < 200)
                                return;

                            if (windowResizeBoundsType == 1)
                            { //fixed size
                                SetWindowPos(handle, HWND.NOTOPMOST, windowResizeBounds.X, windowResizeBounds.Y, windowResizeBounds.Width, windowResizeBounds.Height, 0);
                            }
                            else if (windowResizeBoundsType == 2)
                            { //maximize

                                try
                                {
                                    RECT Rect = new RECT();
                                    GetWindowRect(handle, ref Rect);

                                    Rectangle screenBounds = Screen.AllScreens[Tools.ScreenNumber].WorkingArea;
                                    bool screenBoundsFound = false;
                                    foreach (Screen screen in Screen.AllScreens)
                                    {
                                        Rectangle screenBoundsFind = screen.WorkingArea;
                                        if (screenBoundsFind.Contains(formControlGroup.Bounds.X + formControlGroup.Bounds.Width / 2, formControlGroup.Bounds.Y + formControlGroup.Bounds.Height / 2))
                                        {
                                            screenBounds = screenBoundsFind;
                                            screenBoundsFound = true;
                                            break;
                                        }
                                    }

                                    if (!screenBoundsFound)
                                    {
                                        foreach (Screen screen in Screen.AllScreens)
                                        {
                                            Rectangle screenBoundsFind = screen.WorkingArea;
                                            if (screenBoundsFind.Contains(formControlGroup.Bounds.X + formControlGroup.Bounds.Width / 2, formControlGroup.Bounds.Y + formControlGroup.Bounds.Height))
                                            {
                                                screenBounds = screenBoundsFind;
                                                screenBoundsFound = true;
                                                break;
                                            }
                                        }
                                    }
                                    if (!screenBoundsFound)
                                    {
                                        foreach (Screen screen in Screen.AllScreens)
                                        {
                                            Rectangle screenBoundsFind = screen.WorkingArea;
                                            if (screenBoundsFind.Contains(formControlGroup.Bounds.X + formControlGroup.Bounds.Width / 2, formControlGroup.Bounds.Y - formControlGroup.Bounds.Height))
                                            {
                                                screenBounds = screenBoundsFind;
                                                screenBoundsFound = true;
                                                break;
                                            }
                                        }
                                    }

                                    SetWindowPos(handle, HWND.NOTOPMOST, screenBounds.Left + screenBounds.Width / 2 - (Rect.Right - Rect.Left) / 2,
                                        screenBounds.Top + screenBounds.Height / 2 - (Rect.Bottom - Rect.Top) / 2, Rect.Right - Rect.Left, Rect.Bottom - Rect.Top, 0);
                                }
                                catch { }

                                ShowWindow(handle, SW_SHOWMAXIMIZED);
                            }
                            else if (windowResizeBoundsType == 3)
                            { //maximize
                                ShowWindow(handle, SW_SHOWMINIMIZED);
                            }
                            else if (windowResizeBoundsType == 4)
                            { //maximize
                                try
                                {
                                    handleWindowToClose = handle;
                                    handle = IntPtr.Zero;
                                    //handleBitmap = null;
                                    Thread th = new Thread(new ThreadStart(closeWindowApplication));
                                    th.IsBackground = true;
                                    th.Start();
                                }
                                catch { }
                            }

                            handle = IntPtr.Zero;
                            //handleBitmap = null;

                            pointedObjectIndexAfterWindowControl = checkPointedObject();
                            endWindowControlStartTimer();

                           // BeginInvoke((MethodInvoker)delegate()
                           {
                               state.ProcessEvent(FiniteStateMachine.Events.EndGesture);
                           }//);
                            // state.ProcessEvent(FiniteStateMachine.Events.FoundWindow);
                        }
                    }
                }
                else if (state.State == FiniteStateMachine.States.WindowPointable)
                {
                    if ((fingers.Count >= 3) && fingertipVelocity.Magnitude < 300)
                    {
                        //if (formControl.windowCursorForm.Visible)
                        //    formControl.windowCursorForm.Hide();
                        if (!formControl.Visible)
                            formControl.Show();
                        if (!formControl.shadowForm.Visible)
                            formControl.shadowForm.Show();

                       // formControl.TopMost = true;
                        Tools.SetWindowPos(this.formControl.Handle, Tools.HWND.TOPMOST, 0, 0, 0, 0, Tools.SetWindowPosFlags.IgnoreMove | Tools.SetWindowPosFlags.IgnoreResize | Tools.SetWindowPosFlags.DoNotActivate);     

                        Vector3 fingertipChangeVector = fingertipCoordinate[0] - initialFingertipCoordinate[0];

                        double distanceFingertipChange = fingertipChangeVector.Magnitude;
                        //Vector3.distancePointToLine(initialPointedObjectPosition, fingertipCoordinate[0], fingertipCoordinateOut[0]);
                        //check for gesture or control
                        if (fingertipVelocity.Magnitude < 500)// && distanceFingertipChange < 150) //velocity 150 for volume control
                        {
                            double distanceChangeVertical = Vector3.dotProduct(fingertipChangeVector, verticalAxis);
                            double distanceChangeHorizontal = Vector3.dotProduct(fingertipChangeVector, horizontalAxis);

                            double velocityHorizontal = Math.Abs(Vector3.dotProduct(fingertipVelocity, horizontalAxis));
                            double velocityVertical = Math.Abs(Vector3.dotProduct(fingertipVelocity, verticalAxis));

                            if (velocityHorizontal > 300 || velocityVertical > 300)
                            {
                                //   state.ProcessEvent(FiniteStateMachine.Events.EndGesture);
                            }

                            //if (velocityHorizontal > 100)

                            if (Math.Abs(distanceChangeVertical) < 80 && Math.Abs(distanceChangeHorizontal) < 80) //60 cm before 910
                            {
                                distanceChangeVertical = distanceChangeVertical / 45 * 100; // 0.9.2 35
                                distanceChangeHorizontal = distanceChangeHorizontal / 45 * 100; //35

                                formControl.setTracking(distanceChangeHorizontal, distanceChangeVertical);
                                //mouseControl2(distanceChangeHorizontal, distanceChangeVertical);
                            }
                            else
                            {
                                state.ProcessEvent(FiniteStateMachine.Events.EndGesture);
                            }
                        }
                        //if (distanceFingertipChange > 150)
                        //    state.ProcessEvent(FiniteStateMachine.Events.EndGesture);
                    }
                    else if (fingertipVelocity.Magnitude < 300)
                    {
                        state.ProcessEvent(FiniteStateMachine.Events.EndGesture);
                    }
                }
                else if (state.State == FiniteStateMachine.States.WindowAware)
                {
                 //   Cursor.Hide();
                    if (hand.Fingers.Count > 3)
                    {
                        //reset 
                        if (pinchGestureState == PinchGestureStates.Dragging)
                        {//reset pinchgesture
                            Tools.changeCursorWindowsNormal();
                            if (Tools.modeClickAndDrag)
                                triggerMouseMoveAndClickUpOnly(cursorPreviousClick, 0);
                            // formControl.drawWindowCursorAbsolute((int)cursorPreviousClick.X, (int)cursorPreviousClick.Y, true, fingertipCoordinate[0]);
                            pinchGestureState = PinchGestureStates.None;
                        }

                        state.ProcessEvent(FiniteStateMachine.Events.StartGesture);
                        return;
                    }

                    if (hand.IsValid && (hand.Fingers.Count == 2 || hand.Fingers.Count == 3))
                    {
                        Finger thumb;
                        if (hand.Fingers.Rightmost.Id != hand.Fingers.Frontmost.Id)
                            thumb = hand.Fingers.Rightmost;
                        else
                            thumb = hand.Fingers.Leftmost;

                        Vector3 finger1 = new Vector3(hand.Fingers.Frontmost.StabilizedTipPosition.x,
                            hand.Fingers.Frontmost.StabilizedTipPosition.y, hand.Fingers.Frontmost.StabilizedTipPosition.z);
                        Vector3 finger2 = new Vector3(thumb.StabilizedTipPosition.x,
                            thumb.StabilizedTipPosition.y, thumb.StabilizedTipPosition.z);

                        Vector3 palmPos = new Vector3(hand.PalmPosition.x, hand.PalmPosition.y, hand.PalmPosition.z);
                        double distanceBetweenFingerPalm = Vector3.computeDistance(finger2, palmPos);

                        Vector3 thumbDirection = new Vector3(thumb.Direction.x, thumb.Direction.y, thumb.Direction.z);
                        Vector3 fingerDirection = fingertipCoordinateOut[0] - fingertipCoordinate[0];

                        Vector3 crossVector = Vector3.CrossProduct(fingertipCoordinate[0] - palmPos, finger2 - palmPos);//fingerDirection, thumbDirection);

                        Vector3 verticalAxis = new Vector3(0, 1, 0);
                        double angleBetweenCrossAndVertical = getAngleBetweenVectors(crossVector, verticalAxis);
                        //double angleBetweenFingers = getAngleBetweenVectors(new Vector3(fingers[0].Direction.x, fingers[0].Direction.y, fingers[0].Direction.z),
                        //    new Vector3(fingers[1].Direction.x, fingers[1].Direction.y, fingers[1].Direction.z));

                        double distanceBetweenTips = Vector3.computeDistance(finger1, finger2);

                        if (((angleBetweenCrossAndVertical > 0 && angleBetweenCrossAndVertical < 40) || (angleBetweenCrossAndVertical > 110 && angleBetweenCrossAndVertical < 170))
                            &&
                            (distanceBetweenTips > 65 && distanceBetweenTips < 200))
                        {
                            if (pinchGestureState == PinchGestureStates.None && startPinchGestureTime==0 && !circleGestureInProgress)
                            {
                                //pinchGestureState = PinchGestureStates.CheckingTrigger;
                                startPinchGestureTime = DateTime.Now.Ticks;
                                //Console.WriteLine("Trigger Open");
                            }
                            else if (pinchGestureState == PinchGestureStates.None && startPinchGestureTime!=0)
                            {
                                long duration = (DateTime.Now.Ticks - startPinchGestureTime) / 10000;

                                if (duration > 250) //1.0.0 150ms
                                {
                                    pinchGestureState = PinchGestureStates.TriggerOpen;
                                }
                            }
                        }
                        else
                        {
                            startPinchGestureTime = 0;
                            //if (pinchGestureState == PinchGestureStates.CheckingTrigger)
                            //{
                            //    pinchGestureState = PinchGestureStates.None;
                            //    startPinchGestureTime = 0;
                            //}
                        }

                        startEndPinchGestureTime = 0;
                    }
                    else
                    {//1 finger only
                        startPinchGestureTime = 0;

                        if (pinchGestureState == PinchGestureStates.Dragging)
                        {
                            if (startEndPinchGestureTime == 0)
                            {
                                startEndPinchGestureTime = DateTime.Now.Ticks;
                            }
                            else
                            {
                                long duration = (DateTime.Now.Ticks - startEndPinchGestureTime) / 10000;

                                if (duration > 250)
                                {
                                    pinchGestureState = PinchGestureStates.TriggerClose;
                                }
                            }
                        }
                    }

                    //startPinchGestureTime = 0;

                    if (fingertipVelocity.Magnitude < 1000)
                    {
                        bool pointWithinScreen = false;

                        Point currentPointedWindowPosition = checkPointedScreenPosition();
                        int pixelAllowance = 300;// 350;

                        if (tutorialInProgress)
                            pixelAllowance = 2000;

                        foreach (Screen screen in Screen.AllScreens)
                        {
                            Rectangle screenBounds = screen.Bounds;
                            screenBounds.Width += pixelAllowance * 2;
                            screenBounds.Height += pixelAllowance * 2 - 50;
                            screenBounds.X -= pixelAllowance;
                            screenBounds.Y -= pixelAllowance;

                            if (screenBounds.Contains(currentPointedWindowPosition))
                            {
                                pointWithinScreen = true;
                                break;
                            }
                        }

                        if (pointWithinScreen || pinchGestureState != PinchGestureStates.None)
                        {

                            startTimePointedOutsideScreen = 0;
                            //if (fingertipVelocity.Magnitude < 13) 

                            if (!formControl.windowCursorForm.Visible && pinchGestureState == PinchGestureStates.None)
                            {
                                formControl.windowCursorForm.Show();
                                //formControl.windowCursorForm.TopMost = true;
                                Tools.SetWindowPos(formControl.windowCursorForm.Handle, Tools.HWND.TOPMOST, 0, 0, 0, 0, Tools.SetWindowPosFlags.IgnoreMove | Tools.SetWindowPosFlags.IgnoreResize | Tools.SetWindowPosFlags.DoNotActivate);     
                            }

                            if (currentObjectHover != null)
                            {
                                currentObjectHover = null;
                                currentObjectHoverIndex = -2;
                                //formControl.setObjectHover(currentObjectHover);
                            }

                            if (formControlGroup.Visible)
                                formControlGroup.Hide();

                            if (screenRect.X == int.MaxValue)
                            {
                                foreach (Screen screen in Screen.AllScreens)
                                    screenRect = Rectangle.Union(screenRect, screen.Bounds);
                            }

                            int margin = 10;

                            if (isWindowSpecial)
                                margin = 0;
                            bool touchedTop = false;

                            if (currentPointedWindowPosition.X < screenRect.Left + margin)
                                currentPointedWindowPosition.X = screenRect.Left + margin;
                            if (currentPointedWindowPosition.X > screenRect.Right - margin)
                                currentPointedWindowPosition.X = screenRect.Right - margin;
                            if (currentPointedWindowPosition.Y < screenRect.Top + margin)
                                currentPointedWindowPosition.Y = screenRect.Top + margin;
                            if (currentPointedWindowPosition.Y > screenRect.Bottom - margin)
                            {
                                currentPointedWindowPosition.Y = screenRect.Bottom - margin;
                                touchedTop = true;
                            }

                            //check the pointedposition is within any screen
                            bool pointModifiedWithinScreen = false;
                           // Rectangle screenBounds = Screen.AllScreens[Tools.ScreenNumber].Bounds;
                            foreach (Screen screen in Screen.AllScreens)
                            {
                                if (screen.Bounds.Contains(currentPointedWindowPosition.X, currentPointedWindowPosition.Y))
                                {
                                    pointModifiedWithinScreen = true;
                                }
                            }
                            if (!pointModifiedWithinScreen) //still not in screen
                            {
                                foreach (Screen screen in Screen.AllScreens)
                                {
                                    if (screen.Bounds.Contains(currentPointedWindowPosition.X, currentPointedWindowPosition.Y + 200))
                                    {
                                        pointModifiedWithinScreen = true;
                                        currentPointedWindowPosition.Y = screen.Bounds.Top + margin;

                                        touchedTop = true;
                                    }
                                }
                            }


                            if (pinchGestureState == PinchGestureStates.TriggerOpen)
                            {
                               // Console.WriteLine("Triggered2");
                                //Thread.Sleep(20);
                                if (Tools.modeClickAndDrag)
                                {
                                    formControl.windowCursorForm.Hide();

                                    if (formControl.windowCursorDropForm != null && !formControl.windowCursorDropForm.IsDisposed && formControl.windowCursorDropForm.Visible)
                                    {
                                        formControl.windowCursorDropForm.Hide();
                                        triggerMouseMoveAndClickDownOnly(cursorPreviousClick, 0);

                                        if (isWindowSpecial && touchedTop)
                                            triggerMouseMove(cursorPreviousClick.X, cursorPreviousClick.Y + 10);
                                    }
                                    else
                                    {
                                        triggerMouseMoveAndClickDownOnly(currentPointedWindowPosition, 0);
                                        //Console.WriteLine("No Drop");
                                    }


                                    //formControl.windowCursorForm.Show();
                                    formControl.drawWindowCursorDropClickAbsolute(currentPointedWindowPosition.X, currentPointedWindowPosition.Y, true);
                                }

                                pinchGestureState = PinchGestureStates.Dragging;

                                //startDraggingTime
                                startDraggingTime = 0;// DateTime.Now.Ticks;
                                return;
                            }
                            else if (pinchGestureState == PinchGestureStates.Dragging)
                            {
                              //if din move much
                                //double distancePoint = Math.Sqrt(Math.Pow(currentPointedWindowPosition.X - startDraggingPosition.X, 2) + Math.Pow(currentPointedWindowPosition.Y - startDraggingPosition.Y, 2));

                                long durationMS = (long)((DateTime.Now.Ticks - startDraggingTime) / 10000);

                                if (startDraggingTime == 0)
                                {
                                    startDraggingTime = DateTime.Now.Ticks;
                                    startDraggingPositionInitialized = false;
                                }
                                else if (durationMS > 100 && !startDraggingPositionInitialized)
                                {
                                    startDraggingPosition.X = fingertipCoordinate[0].X;
                                    startDraggingPosition.Y = fingertipCoordinate[0].Y;
                                    startDraggingPosition.Z = fingertipCoordinate[0].Z;
                                    startDraggingPositionInitialized = true;
                                }
                                else if (durationMS > 100 && startDraggingPositionInitialized)
                                {
                                    double distancePoint = Vector3.computeDistance(startDraggingPosition, fingertipCoordinate[0]);

                                    double distanceAllowance = 10;
                                    if (!Tools.modeClickAndDrag) distanceAllowance = 40;

                                    int durationAllowance = 800;
                                    if (!Tools.modeClickAndDrag) durationAllowance = 500;

                                    if (!Tools.modeWindowDrag) durationAllowance = 100000;
                                    if (fingertipVelocity.Magnitude < 100 && distancePoint < distanceAllowance) //45
                                    {
                                        if (durationMS > durationAllowance) //0.9.5 400
                                        {//
                                            if (!isWindowSpecial) //not desktop or taskbar
                                            {
                                                if (!tutorialInProgress || (tutorialInProgress && handle != formfullScreen.Handle))
                                                {
                                                    Tools.changeCursorWindowsNormal();
                                                    if (Tools.modeClickAndDrag)
                                                    {
                                                        triggerMouseMoveAndClickUpOnly(cursorPreviousClick, 0);
                                                    }
                                                    if (formControl.windowCursorDropForm.Visible)
                                                        formControl.windowCursorDropForm.Hide();
                                                    if (formControl.windowCursorDropFormClick.Visible)
                                                        formControl.windowCursorDropForm.Hide();

                                                    pinchGestureState = PinchGestureStates.None;
                                                    startDraggingTime = 0;
                                                    state.ProcessEvent(FiniteStateMachine.Events.PinchGesture);
                                                    return;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        startDraggingTime = DateTime.Now.Ticks + 10000 * 10000;
                                    }
                                }
                                //if (startDraggingTime > DateTime.Now.Ticks)
                                //{ //new calculation
                                //    if (fingertipVelocity.Magnitude < 30)
                                //    {
                                //        if (!isWindowSpecial) //not desktop or taskbar
                                //        {
                                //            Tools.changeCursorWindowsNormal();
                                //            triggerMouseMoveAndClickUpOnly(cursorPreviousClick, 0);
                                //            if (formControl.windowCursorDropForm.Visible)
                                //                formControl.windowCursorDropForm.Hide();
                                //            if (formControl.windowCursorDropFormClick.Visible)
                                //                formControl.windowCursorDropForm.Hide();

                                //            pinchGestureState = PinchGestureStates.None;
                                //            startDraggingTime = 0;
                                //            state.ProcessEvent(FiniteStateMachine.Events.PinchGesture);
                                //            return;
                                //        }
                                //    }
                                //}

                                if (Tools.modeClickAndDrag)
                                    triggerMouseMove(currentPointedWindowPosition);

                                cursorPreviousDrag.X = currentPointedWindowPosition.X;
                                cursorPreviousDrag.Y = currentPointedWindowPosition.Y;


                                if (Tools.modeClickAndDrag)
                                    formControl.drawWindowCursorDropClickAbsolute(currentPointedWindowPosition.X, currentPointedWindowPosition.Y, true);

                                //dra
                            }
                            else if (pinchGestureState == PinchGestureStates.TriggerClose)
                            {
                                //Thread.Sleep(20);
                                formControl.windowCursorDropFormClick.Hide();
                                formControl.windowCursorForm.Hide();
                                if (formControl.windowCursorDropForm.Visible)
                                {
                                    formControl.windowCursorDropForm.Hide();
                                   // triggerMouseMoveAndClickUpOnly(cursorPreviousClick, 0);
                                   // formControl.windowCursorDropForm.Show();
                                }


                                if (Tools.modeClickAndDrag)
                                {
                                    triggerMouseMoveAndClickUpOnly(cursorPreviousDrag, 0);

                                    // formControl.windowCursorForm.Show();
                                    formControl.drawWindowCursorAbsolute((int)cursorPreviousDrag.X, (int)cursorPreviousDrag.Y, true, fingertipCoordinate[0]);
                                }
                                //Thread.Sleep(20);
                                pinchGestureState = PinchGestureStates.None;
                                return;
                            }


                            IntPtr pointedWindow;


                            if (pinchGestureState == PinchGestureStates.None || !Tools.modeClickAndDrag)
                            {
                                pointedWindow = checkPointedWindow(currentPointedWindowPosition);
                                checkMouseClick(currentPointedWindowPosition);
                            }
                            else //if (pinchGestureState == PinchGestureStates.Dragging)
                            {
                                return;
                            }

                            if (pointedWindow != IntPtr.Zero && pointedWindow != null)
                            {
                                if (fingertipVelocity.Magnitude < 15 & !scrollMode)
                                {
                                    if (handle == pointedWindow)
                                    {
                                    }
                                    else
                                    {
                                        if (tempPointedWindow != pointedWindow)
                                        {//another window
                                            startTimePointedWindow = DateTime.Now.Ticks;
                                        }

                                        tempPointedWindow = pointedWindow;

                                        long duration = DateTime.Now.Ticks - startTimePointedWindow;
                                        duration = duration / 10000;

                                        if (duration > 250) //250. for testing 1000
                                        {//300ms before go to next state
                                            handle = pointedWindow;
                                            state.ProcessEvent(FiniteStateMachine.Events.FoundWindow);
                                        }
                                    }
                                }
                                else //check scroll mode
                                {

                                }
                            }
                        }
                        else
                        {//point not within screen
                            //check time first - 300ms
                            if (startTimePointedOutsideScreen == 0)
                            {
                                startTimePointedOutsideScreen = DateTime.Now.Ticks;
                            }
                            long durationOutsideScreen = DateTime.Now.Ticks - startTimePointedOutsideScreen;
                            durationOutsideScreen = durationOutsideScreen / 10000;

                            if (durationOutsideScreen < 400) //250. for testing 1000
                            {
                                return;
                            }

                            if (formControl.windowCursorForm.Visible)
                            {
                                Tools.changeCursorWindowsNormal();
                                formControl.windowCursorForm.Hide();
                            }

                            if (formControl.windowCursorDropForm.Visible)
                                formControl.windowCursorDropForm.Hide();

                            if (fingers.Count == 1 || (currentObjectHover != null && fingers.Count == 2)) //thumb problem//update in future if know is finger or thumb TODO
                            {
                                objectPointedIndex = checkPointedObject();

                                if (objectPointedIndex >= 0)// -1)
                                {
                                    if (fingertipVelocity.Magnitude < 15) //less than 30mm per second
                                    {
                                        if (formDraw.Visible)
                                        {
                                            formDraw.Hide();
                                        }
                                        state.ProcessEvent(FiniteStateMachine.Events.FoundPointable);
                                    }
                                    else if (fingertipVelocity.Magnitude < 50)
                                    {
                                        currentObjectHover = pointables[objectPointedIndex];
                                        currentObjectHoverIndex = objectPointedIndex;

                                        if (formDraw.Visible)
                                        {
                                            formDraw.Hide();
                                        }

                                        formControl.setObjectHover(currentObjectHover);
                                        if (!formControlGroup.Visible)
                                            formControlGroup.Show();
                                    }
                                }
                                //else
                                //{//in windowware, no hover mode
                                //    currentObjectHover = null;
                                //    currentObjectHoverIndex = -2;

                                //    formControl.setObjectHover(currentObjectHover);
                                //}
                            }
                            else if (fingers.Count == 2 && currentObjectHover == null)
                            {//copy above
                                long duration = DateTime.Now.Ticks - startTimeWake;
                                duration = duration / 10000;

                                if (duration > 500)
                                {//allowance for finger detection. not thumb
                                    objectPointedIndex = checkPointedObject();

                                    if (objectPointedIndex >= 0)// -1)
                                    {
                                        if (fingertipVelocity.Magnitude < 15) //less than 30mm per second
                                        {
                                            if (formDraw.Visible)
                                            {
                                                formDraw.Hide();
                                            }
                                            state.ProcessEvent(FiniteStateMachine.Events.FoundPointable);

                                        }
                                        else if (fingertipVelocity.Magnitude < 50)
                                        {
                                            currentObjectHover = pointables[objectPointedIndex];
                                            currentObjectHoverIndex = objectPointedIndex;

                                            formControl.setObjectHover(currentObjectHover);
                                            if (!formControlGroup.Visible)
                                                formControlGroup.Show();
                                        }
                                    }
                                }
                            }
                        }
                    }

                }
                else if (state.State == FiniteStateMachine.States.PointableAware)
                {
                    Vector3 fingertipChangeVector = fingertipCoordinate[0] - initialFingertipCoordinate[0];

                    double distanceFingertipChange = fingertipChangeVector.Magnitude;
                    //Vector3.distancePointToLine(initialPointedObjectPosition, fingertipCoordinate[0], fingertipCoordinateOut[0]);
                    //check for gesture or control
                    if (fingertipVelocity.Magnitude < 400)// && distanceFingertipChange < 150) //velocity 150 for volume control
                    {
                        double distanceChangeVertical = Vector3.dotProduct(fingertipChangeVector, verticalAxis);
                        double distanceChangeHorizontal = Vector3.dotProduct(fingertipChangeVector, horizontalAxis);

                        double velocityHorizontal = Math.Abs(Vector3.dotProduct(fingertipVelocity, horizontalAxis));
                        double velocityVertical = Math.Abs(Vector3.dotProduct(fingertipVelocity, verticalAxis));

                        if (velocityHorizontal > 300 || velocityVertical > 300)
                        {
                            //   state.ProcessEvent(FiniteStateMachine.Events.EndGesture);
                        }

                        if (Math.Abs(distanceChangeVertical) < 90 && Math.Abs(distanceChangeHorizontal) < 90) //10cm
                        {
                            distanceChangeVertical = distanceChangeVertical / 35 * 100; //conver to cm and 40 TODO
                            distanceChangeHorizontal = distanceChangeHorizontal / 35 * 100; //conver to cm and 40

                            formControl.setTracking(distanceChangeHorizontal, distanceChangeVertical);
                        }
                        else
                        {
                            state.ProcessEvent(FiniteStateMachine.Events.EndGesture);
                        }
                    }
                    if (distanceFingertipChange > 80) //distance 150mm to end gesture for item
                        state.ProcessEvent(FiniteStateMachine.Events.EndGesture);

                }
                else if (state.State == FiniteStateMachine.States.PointableControl)
                {
                    //control gesture
                }
                else if (state.State == FiniteStateMachine.States.PointableLaunch)
                {
                    //launch item
                }
            }
            catch { }
        }

        long startTimePointedOutsideScreen=0;

        enum PinchGestureStates { None, CheckingTrigger, TriggerOpen, Dragging, TriggerClose }
        PinchGestureStates pinchGestureState =PinchGestureStates.None;

        Point cursorPreviousDrag;
        bool startDraggingPositionInitialized = false;

        int pointedObjectIndexAfterWindowControl =-2;
        System.Timers.Timer _timerAfterWindowControl;

        private void endWindowControlStartTimer()
        {
            try
            {
                if (_timerAfterWindowControl == null)
                {
                    _timerAfterWindowControl = new System.Timers.Timer(1200); //300
                    _timerAfterWindowControl.Elapsed += new ElapsedEventHandler(_timerAfterWindowControl_Elapsed);
                    _timerAfterWindowControl.Enabled = true;
                }
                else
                {
                    _timerAfterWindowControl.Enabled = false;
                    _timerAfterWindowControl.Enabled = true;
                }
            }
            catch { }
        }
        void _timerAfterWindowControl_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                _timerAfterWindowControl.Enabled = false;
                pointedObjectIndexAfterWindowControl = -2;
            }
            catch { }
        }

        long startDraggingTime;
        Vector3 startDraggingPosition;
        long startPinchGestureTime;


        long startEndPinchGestureTime;


        Point originalWindowOffset;
        internal static Point pointedScreenPositionUniversal;
               
        private Point checkPointedScreenPosition()
        {
            Vector3 vScreen = Vector3.intersectLineScreen(fingertipCoordinate[0], fingertipCoordinateOut[0], screenNormal, screenCorner[0]);

            Vector3 pointY1 = Vector3.nearestPointonLine(vScreen, screenCorner[0], screenCorner[1]);
            Vector3 pointX1 = Vector3.nearestPointonLine(vScreen, screenCorner[1], screenCorner[2]);

            double y1 = Vector3.computeDistance(vScreen, pointY1);// distancePointToLine(vScreen, screenCorner[0], screenCorner[1]);
            double x1 = Vector3.computeDistance(vScreen, pointX1);// distancePointToLine(vScreen, screenCorner[1], screenCorner[2]);

            Vector3 pointY2 = Vector3.nearestPointonLine(vScreen, screenCorner[3], screenCorner[2]);
            Vector3 pointX2 = Vector3.nearestPointonLine(vScreen, screenCorner[0], screenCorner[3]);

            double y2 = Vector3.computeDistance(vScreen, pointY2);
            double x2 = Vector3.computeDistance(vScreen, pointX2);

            double x, y;

            Vector3 vectorDirection1 = Vector3.crossProduct(vScreen - screenCorner[1], screenCorner[0] - screenCorner[1]);

            try
            {
                vectorDirection1.Normalize();
            }
            catch { }

            if (Vector3.isVectorSame(vectorDirection1, screenNormal))
                y = y1;
            else
                y = -y1;

            Vector3 vectorDirection2 = Vector3.crossProduct(vScreen - screenCorner[1], screenCorner[2] - screenCorner[1]);

            try
            {
                vectorDirection2.Normalize();
            }
            catch { }
            if (Vector3.isVectorSame(vectorDirection2, screenNormal))
                x = -x1;
            else
                x = x1;

            currentX = x;// *100;
            currentY = y;// *100; in mm

            screenWidth = Screen.AllScreens[Tools.ScreenNumber].Bounds.Width;
            screenHeight = Screen.AllScreens[Tools.ScreenNumber].Bounds.Height;

            double screenRatioWidth = screenWidth / screenPhysicalWidth;
            double screenRatioHeight = screenHeight / screenPhysicalHeight;

            x = screenWidth + x * screenRatioWidth;
            y = -y * screenRatioHeight;

            x = x + System.Windows.Forms.Screen.AllScreens[Tools.ScreenNumber].Bounds.X;//1Screen2.AllScreens[1].Bounds.X;
            y = y + System.Windows.Forms.Screen.AllScreens[Tools.ScreenNumber].Bounds.Y;//Screen2.AllScreens[1].Bounds.Y;

            //pointedScreenPositionUniversal.X = (int)x;
            //pointedScreenPositionUniversal.Y = (int)y;

            return new Point((int)x , (int)y);
        }

        double screenVerticalOffset= 0;
        double screenHorizontalOffset = 0;
        double screenHeightDefault = 100;
        double screenSizeDefault = 23;
        double screenSizeHeightDefault = 11.28;
        double screenSizeWidthDefault = 20.05;
        //int screenHeightUser = 0;

        private void resetSceenCorners()
        {
            try
            { // x=Y/sqrt(a*a+1)
                double horizontalOverVerticalRatio = Screen.AllScreens[Tools.ScreenNumber].Bounds.Width / (double)Screen.AllScreens[Tools.ScreenNumber].Bounds.Height;
                double userScreenRealHeight = Tools.ScreenSize / (Math.Sqrt(horizontalOverVerticalRatio * horizontalOverVerticalRatio + 1));
                //verticalHeightValue = verticalHeightValue * screenSizeHeightDefault / 1080;
                double userScreenRealWidth = userScreenRealHeight * horizontalOverVerticalRatio;

                if (Screen.AllScreens[Tools.ScreenNumber].Bounds.Width > Screen.AllScreens[Tools.ScreenNumber].Bounds.Height)
                {//landscape
                    //depending on size
                    screenCorner[0] = new Vector3(-250, 400, -250);
                    screenCorner[1] = new Vector3(250, 400, -250);
                    screenCorner[2] = new Vector3(250, 100, -250);
                    screenCorner[3] = new Vector3(-250, 100, -250);
                }
                else
                {
                    screenCorner[0] = new Vector3(-150, 600, -250);
                    screenCorner[1] = new Vector3(150, 600, -250);
                    screenCorner[2] = new Vector3(150, 100, -250);
                    screenCorner[3] = new Vector3(-150, 100, -250);
                }

                for (int i = 0; i < 4; i++)
                {
                    screenCorner[i].X = screenCorner[i].X * userScreenRealWidth / screenSizeWidthDefault;//Tools.ScreenSize / screenSizeDefault;
                }

                double screenCenterY = (screenCorner[1].Y + screenCorner[2].Y) / 2;
                double screenRealHeight = (screenCorner[1].Y - screenCorner[2].Y);

                screenCorner[0].Y = screenCenterY + (screenRealHeight / 2) *  userScreenRealHeight / screenSizeHeightDefault;// Tools.ScreenSize / screenSizeDefault;
                screenCorner[1].Y = screenCorner[0].Y;

                screenCorner[2].Y = screenCenterY - (screenRealHeight / 2) * userScreenRealHeight / screenSizeHeightDefault;//Tools.ScreenSize / screenSizeDefault;
                screenCorner[3].Y = screenCorner[2].Y;
            }
            catch { }
        }

        private void initializeScreenCorners()
        {
            try
            {
                resetSceenCorners();

                for (int i = 0; i < 4; i++)
                {
                    screenCorner[i].Y = screenCorner[i].Y + screenVerticalOffset;
                }
                for (int i = 0; i < 4; i++)
                {
                    screenCorner[i].X = screenCorner[i].X + screenHorizontalOffset;
                }

                processScreenData();
            }
            catch { }
        }

        private IntPtr checkPointedWindow(Point pointedWindowPosition)
        {
            try
            {
                long startTime = DateTime.Now.Ticks;
                
                Tools.changeCursorWindowsNone();

                formControl.drawWindowCursorAbsolute((int)pointedWindowPosition.X, (int)pointedWindowPosition.Y, true, fingertipCoordinate[0]);

               IntPtr handleLocal = FormFindWindow.findPointedWindow(new System.Drawing.Point((int)pointedWindowPosition.X, (int)pointedWindowPosition.Y)); //todo all the time call

               try
               {
                   if (!formControl.windowCursorDropForm.Visible)
                       triggerMouseMove(pointedWindowPosition.X, pointedWindowPosition.Y);
               }
               catch { }

               return handleLocal;
            }
            catch { }

            return IntPtr.Zero;
        }
        




        #region SettingsData

        private void saveSettingsData()
        {
            //savePointableDataAll();
            try
            {
                TextWriter tw = new StreamWriter(settingsFolderPath + settingsFile);
                tw.WriteLine("ScreenNumber=" + Tools.ScreenNumber.ToString()); //screenHeightUser
                tw.WriteLine("ScreenSize=" + Tools.ScreenSize.ToString()); //screenHeightUser

                tw.WriteLine("screenHorizontalOffset=" + ((int)screenHorizontalOffset).ToString());
                tw.WriteLine("screenVerticalOffset=" + ((int)screenVerticalOffset).ToString());

                tw.WriteLine("TapSensitivity=" + Tools.TapSensitivity.ToString()); //screenHeightUser
                tw.WriteLine("TapLeftSensitivity=" + Tools.TapLeftSensitivity.ToString()); //screenHeightUser
                tw.WriteLine("TapRightSensitivity=" + Tools.TapRightSensitivity.ToString()); //screenHeightUser
                tw.WriteLine("CursorStabilization=" + Tools.CursorStabilization.ToString()); //screenHeightUser
                tw.WriteLine("CursorDroplet=" + Tools.CursorDroplet.ToString()); //screenHeightUser

                tw.WriteLine("versionMajor=" + (currentVersionMajor.ToString())); 
                tw.WriteLine("versionMinor=" + (currentVersionMinor.ToString())); 
                tw.WriteLine("versionBuild=" + (currentVersionBuild.ToString()));


                if (Tools.modeClickAndDrag) 
                {
                    tw.WriteLine("modeClickAndDrag=1");
                }
                else
                {
                    tw.WriteLine("modeClickAndDrag=-1");
                }
                if (Tools.modeClickLeft)
                {
                    tw.WriteLine("modeClickLeft=1");
                }
                else
                {
                    tw.WriteLine("modeClickLeft=-1");
                } 
                
                if (Tools.modeClickRight)
                {
                    tw.WriteLine("modeClickRight=1");
                }
                else
                {
                    tw.WriteLine("modeClickRight=-1");
                } 
                
                if (Tools.modeClickMiddle)
                {
                    tw.WriteLine("modeClickMiddle=1");
                }
                else
                {
                    tw.WriteLine("modeClickMiddle=-1");
                }

                if (Tools.modeWindowDrag)
                {
                    tw.WriteLine("modeWindowDrag=1");
                }
                else
                {
                    tw.WriteLine("modeWindowDrag=-1");
                }


                if (Tools.keyModifierModeEnableTracking)
                {
                    tw.WriteLine("keyModifierModeEnableTracking=1");
                }
                else
                {
                    tw.WriteLine("keyModifierModeEnableTracking=-1");
                }


                if (Tools.keyModifierModeControl)
                {
                    tw.WriteLine("keyModifierModeControl=1");
                }
                else
                {
                    tw.WriteLine("keyModifierModeControl=-1");
                }


                if (Tools.keyModifierModeAlt)
                {
                    tw.WriteLine("keyModifierModeAlt=1");
                }
                else
                {
                    tw.WriteLine("keyModifierModeAlt=-1");
                }


                if (Tools.keyModifierModeShift)
                {
                    tw.WriteLine("keyModifierModeShift=1");
                }
                else
                {
                    tw.WriteLine("keyModifierModeShift=-1");
                }


                tw.WriteLine("modeClickS=" + Tools.modeClickS.ToString()); //screenHeightUser
            
                //tw.WriteLine("screenHeightUser=" + screenHeightUser.ToString());

            //tw.WriteLine("THRESHOLD=" + THRESHOLD.ToString());

            //for (int i = 0; i < NUMBEROBJECTS; i++)
            //{
            //    tw.WriteLine("objectNames[" + i + "]=" + objectNames[i]);

            //    Tools.writeVectorValues(tw, "objectCoordinate[" + i + "]", objectCoordinate[i]);

            //    Tools.writeVectorValues(tw, "objectCoordA1[" + i + "]", objectCoordA1[i]);
            //    Tools.writeVectorValues(tw, "objectCoordA2[" + i + "]", objectCoordA2[i]);
            //    Tools.writeVectorValues(tw, "objectCoordB1[" + i + "]", objectCoordB1[i]);
            //    Tools.writeVectorValues(tw, "objectCoordB2[" + i + "]", objectCoordB2[i]);
            //}


                //for (int i = 0; i < 4; i++)
                //{
                //    Tools.writeVectorValues(tw, "screenCorner[" + i + "]", screenCorner[i]);
                //}

                //if (cursorControl) //save settings for smoothing
                //{
                //    tw.WriteLine("cursorControl=1");
                //}
                //else
                //{
                //    tw.WriteLine("cursorControl=-1");
                //}

                //for (int i = 0; i < 8; i++)
                //{
                //    Tools.writeVectorValues(tw, "screenLineA[" + i + "]", screenLineA[i]);
                //    Tools.writeVectorValues(tw, "screenLineB[" + i + "]", screenLineB[i]);
                //}

                //Tools.writeVectorValues(tw, "screenNormal", screenNormal);
                //Tools.writeVectorValues(tw, "verticalAxis", verticalAxis);
                //Tools.writeVectorValues(tw, "horizontalAxis", horizontalAxis);
                //Tools.writeVectorValues(tw, "screenCenter", screenCenter);
                //tw.WriteLine("screenPhysicalWidth=" + screenPhysicalWidth.ToString());
                //tw.WriteLine("screenPhysicalHeight=" + screenPhysicalHeight.ToString());
                //tw.WriteLine("screenHeight=" + screenHeight.ToString());
                //tw.WriteLine("screenWidth=" + screenWidth.ToString());
                //save calibration data for screen

                tw.Close();
            }
            catch { }
        }
        private void loadSettingsData()
        {
            try
            {
                TextReader tr = new StreamReader(settingsFolderPath + settingsFile);


                string strSettingsFile = tr.ReadToEnd();
                tr.Close();

                strSettingsFile = strSettingsFile.Replace("\r", "");
                string[] strLineArray = strSettingsFile.Split(new char[] { '\n' });

                string[] strValuesArray = new String[strLineArray.Length];
                string[] strKeywordsArray = new String[strLineArray.Length];

                for (int i = 0; i < strLineArray.Length; i++)
                {
                    if (strLineArray[i].Length >= 2)
                    {
                        strValuesArray[i] = strLineArray[i].Split(new char[] { '=' })[1];
                        strKeywordsArray[i] = strLineArray[i].Split(new char[] { '=' })[0];
                    }
                }
                int position;


                position = Tools.obtainValues("ScreenNumber", strKeywordsArray);
                if (position != -1)
                {
                    Tools.ScreenNumber = int.Parse(strValuesArray[position]);
                    if (Tools.ScreenNumber > System.Windows.Forms.Screen.AllScreens.Length -1)
                        Tools.ScreenNumber = 0;
                    if (Tools.ScreenNumber == 0)
                    {
                        screen1ToolStripMenuItem.Checked = true;
                        screen2ToolStripMenuItem.Checked = false;
                        screen3ToolStripMenuItem.Checked = false;
                    }
                    if (Tools.ScreenNumber == 1)
                    {
                        screen3ToolStripMenuItem.Checked = false;
                        screen2ToolStripMenuItem.Checked = true;
                        screen1ToolStripMenuItem.Checked = false;
                    }
                    if (Tools.ScreenNumber == 2)
                    {
                        screen3ToolStripMenuItem.Checked = true;
                        screen2ToolStripMenuItem.Checked = false;
                        screen1ToolStripMenuItem.Checked = false;
                    }

                    //sliderDistance.Value = (int)(THRESHOLD * 100);
                }

                position = Tools.obtainValues("ScreenSize", strKeywordsArray);
                if (position != -1)
                {
                    Tools.ScreenSize = int.Parse(strValuesArray[position]);

                    if (Tools.ScreenSize < 11)
                        Tools.ScreenSize = 11;
                    if (Tools.ScreenSize > 44)
                        Tools.ScreenSize = 44;
                }

                position = Tools.obtainValues("TapSensitivity", strKeywordsArray);
                if (position != -1)
                {
                    Tools.TapSensitivity = int.Parse(strValuesArray[position]);

                    if (Tools.TapSensitivity < 0)
                        Tools.TapSensitivity = 0;
                    if (Tools.TapSensitivity > 5)
                        Tools.TapSensitivity = 5;
                }
                else
                    Tools.TapSensitivity = 3;

                position = Tools.obtainValues("TapRightSensitivity", strKeywordsArray);
                if (position != -1)
                {
                    Tools.TapRightSensitivity = int.Parse(strValuesArray[position]);

                    if (Tools.TapRightSensitivity < 0)
                        Tools.TapRightSensitivity = 0;
                    if (Tools.TapRightSensitivity > 6)
                        Tools.TapRightSensitivity = 6;
                }
                else
                    Tools.TapSensitivity = 3;

                position = Tools.obtainValues("TapLeftSensitivity", strKeywordsArray);
                if (position != -1)
                {
                    Tools.TapLeftSensitivity = int.Parse(strValuesArray[position]);

                    if (Tools.TapLeftSensitivity < 0)
                        Tools.TapLeftSensitivity = 0;
                    if (Tools.TapLeftSensitivity > 6)
                        Tools.TapLeftSensitivity = 6;
                }
                else
                    Tools.TapLeftSensitivity = 3;


                position = Tools.obtainValues("CursorStabilization", strKeywordsArray);
                if (position != -1)
                {
                    Tools.CursorStabilization = int.Parse(strValuesArray[position]);

                    if (Tools.CursorStabilization < 0)
                        Tools.CursorStabilization = 0;
                    if (Tools.CursorStabilization > 5)
                        Tools.CursorStabilization = 5;
                }
                else
                    Tools.CursorStabilization = 1;

                setSmoothingData();


                position = Tools.obtainValues("CursorDroplet", strKeywordsArray);
                if (position != -1)
                {
                    Tools.CursorDroplet = int.Parse(strValuesArray[position]);

                    if (Tools.CursorDroplet < 0)
                        Tools.CursorDroplet = 0;
                    if (Tools.CursorDroplet > 3)
                        Tools.CursorDroplet = 3;
                }
                else
                    Tools.CursorDroplet = 1;

                setCursorDropletData();


                position = Tools.obtainValues("screenVerticalOffset", strKeywordsArray);
                if (position != -1)
                {
                    screenVerticalOffset = int.Parse(strValuesArray[position]);
                }
                position = Tools.obtainValues("screenHorizontalOffset", strKeywordsArray);
                if (position != -1)
                {
                    screenHorizontalOffset = int.Parse(strValuesArray[position]);
                }

                position = Tools.obtainValues("versionMajor", strKeywordsArray);
                if (position != -1)
                {
                    previousVersionMajor= int.Parse(strValuesArray[position]);
                }
                position = Tools.obtainValues("versionMinor", strKeywordsArray);
                if (position != -1)
                {
                    previousVersionMinor = int.Parse(strValuesArray[position]);
                }
                position = Tools.obtainValues("versionBuild", strKeywordsArray);
                if (position != -1)
                {
                    previousVersionBuild = int.Parse(strValuesArray[position]);
                }


                position = Tools.obtainValues("modeClickAndDrag", strKeywordsArray);
                if (position != -1)
                {
                    if (strValuesArray[position] == "1")
                    {
                        Tools.modeClickAndDrag = true;
                    }
                    else
                    {
                        Tools.modeClickAndDrag = false;
                    }
                }
                else
                {
                    Tools.modeClickAndDrag = false;
                }
                clickanddragToolStripMenuItem.Checked = Tools.modeClickAndDrag;



                position = Tools.obtainValues("modeClickLeft", strKeywordsArray);
                if (position != -1)
                {
                    if (strValuesArray[position] == "1")
                    {
                        Tools.modeClickLeft = true;
                    }
                    else
                    {
                        Tools.modeClickLeft = false;
                    }
                }
                else
                {
                    Tools.modeClickLeft = true;
                }



                position = Tools.obtainValues("modeClickRight", strKeywordsArray);
                if (position != -1)
                {
                    if (strValuesArray[position] == "1")
                    {
                        Tools.modeClickRight = true;
                    }
                    else
                    {
                        Tools.modeClickRight = false;
                    }
                }
                else
                {
                    Tools.modeClickRight = true;
                }


                position = Tools.obtainValues("modeClickMiddle", strKeywordsArray);
                if (position != -1)
                {
                    if (strValuesArray[position] == "1")
                    {
                        Tools.modeClickMiddle = true;
                    }
                    else
                    {
                        Tools.modeClickMiddle = false;
                    }
                }
                else
                {
                    Tools.modeClickMiddle = true;
                }


                position = Tools.obtainValues("modeWindowDrag", strKeywordsArray);
                if (position != -1)
                {
                    if (strValuesArray[position] == "1")
                    {
                        Tools.modeWindowDrag = true;
                    }
                    else
                    {
                        Tools.modeWindowDrag = false;
                    }
                }
                else
                {
                    Tools.modeWindowDrag = true;
                }

                position = Tools.obtainValues("keyModifierModeControl", strKeywordsArray);
                if (position != -1)
                {
                    if (strValuesArray[position] == "1")
                    {
                        Tools.keyModifierModeControl = true;
                    }
                    else
                    {
                        Tools.keyModifierModeControl = false;
                    }
                }
                else
                {
                    Tools.keyModifierModeControl = true;
                }
                //controlLeftClickToolStripMenuItem.Checked = Tools.keyModifierModeControl;


                position = Tools.obtainValues("keyModifierModeAlt", strKeywordsArray);
                if (position != -1)
                {
                    if (strValuesArray[position] == "1")
                    {
                        Tools.keyModifierModeAlt = true;
                    }
                    else
                    {
                        Tools.keyModifierModeAlt = false;
                    }
                }
                else
                {
                    Tools.keyModifierModeAlt = true;
                }
                //altRightClickToolStripMenuItem.Checked = Tools.keyModifierModeAlt;


                position = Tools.obtainValues("keyModifierModeShift", strKeywordsArray);
                if (position != -1)
                {
                    if (strValuesArray[position] == "1")
                    {
                        Tools.keyModifierModeShift = true;
                    }
                    else
                    {
                        Tools.keyModifierModeShift = false;
                    }
                }
                else
                {
                    Tools.keyModifierModeShift = true;
                }
                //shiftMiddleClickToolStripMenuItem.Checked = Tools.keyModifierModeShift;


                position = Tools.obtainValues("keyModifierModeEnableTracking", strKeywordsArray);
                if (position != -1)
                {
                    if (strValuesArray[position] == "1")
                    {
                        Tools.keyModifierModeEnableTracking = true;
                    }
                    else
                    {
                        Tools.keyModifierModeEnableTracking = false;
                    }
                }
                else
                {
                    Tools.keyModifierModeEnableTracking = true;
                }

                position = Tools.obtainValues("modeClickS", strKeywordsArray);
                if (position != -1)
                {
                    Tools.modeClickS = int.Parse(strValuesArray[position]);

                    if (Tools.modeClickS < 0)
                        Tools.ScreenSize = 0;
                }



                //controlAltShiftEnableDisablePointableTrackingToolStripMenuItem.Checked = Tools.keyModifierModeEnableTracking;

                ////screen calibration
                //for (int i = 0; i < 4; i++)
                //{
                //    screenCorner[i] = Tools.getValuesVector("screenCorner[" + i + "]", strKeywordsArray, strValuesArray);
                //}
                //for (int i = 0; i < 8; i++)
                //{
                //    screenLineA[i] = Tools.getValuesVector("screenLineA[" + i + "]", strKeywordsArray, strValuesArray);
                //    screenLineB[i] = Tools.getValuesVector("screenLineB[" + i + "]", strKeywordsArray, strValuesArray);
                //}

                //screenNormal = Tools.getValuesVector("screenNormal", strKeywordsArray, strValuesArray);
                //verticalAxis = Tools.getValuesVector("verticalAxis", strKeywordsArray, strValuesArray);
                //horizontalAxis = Tools.getValuesVector("horizontalAxis", strKeywordsArray, strValuesArray);

                //position = Tools.obtainValues("screenPhysicalWidth", strKeywordsArray);
                //if (position != -1)
                //{
                //    screenPhysicalWidth = double.Parse(strValuesArray[position]);
                //}
                //position = Tools.obtainValues("screenPhysicalHeight", strKeywordsArray);
                //if (position != -1)
                //{
                //    screenPhysicalHeight = double.Parse(strValuesArray[position]);
                //}
                //position = Tools.obtainValues("screenHeight", strKeywordsArray);
                //if (position != -1)
                //{
                //    screenHeight = int.Parse(strValuesArray[position]);
                //}
                //position = Tools.obtainValues("screenWidth", strKeywordsArray);
                //if (position != -1)
                //{
                //    screenWidth = int.Parse(strValuesArray[position]);
                //}
                //screenCenter = Tools.getValuesVector("screenCenter", strKeywordsArray, strValuesArray);

                
            }
            catch { }
        }
#endregion

        internal void saveCalibration(PointableObject currentObject)
        {
            savePointableData(currentObject);
            //update mainwindow ui

            try
            {
                if (mainWindow != null)
                    mainWindow.calibrationUpdate(currentObject);
            }
            catch { }
        }


        #region set state conditions
        internal void doStateResetFromStandby()
        {
            BeginInvoke((MethodInvoker)delegate()
            {
                try
                {
                    if (formControlGroup != null && !formControlGroup.IsDisposed)
                    {
                        if (formControlGroup.Visible)
                            formControlGroup.Hide();

                        formControlGroup.resetUI();
                    }
                }
                catch { }
                try 
                {
                    if (formControl != null && !formControl.IsDisposed)
                    {
                        if (formControl.Visible)
                            formControl.Hide();

                        if (formControl.shadowForm.Visible)
                            formControl.hideShadow();

                        if (formControl.windowCursorForm.Visible)
                        {
                            formControl.windowCursorForm.Hide(); 
                            Tools.changeCursorWindowsNormal();
                        }

                        if (formControl.windowCursorDropForm.Visible)
                        {
                            formControl.windowCursorDropForm.Hide();
                        }

                        if (formControl.windowCursorDropFormClick.Visible)
                        {
                            formControl.windowCursorDropFormClick.Hide();
                        }

                        if (pinchGestureState == PinchGestureStates.Dragging)
                        {
                            if (Tools.modeClickAndDrag)
                                triggerMouseMoveAndClickUpOnly(cursorPreviousClick, 0);
                            pinchGestureState = PinchGestureStates.None;
                        }
                        else
                        {
                            pinchGestureState = PinchGestureStates.None;
                        }

                        formControl.resetControls();

                    }
                }
                catch { }

                try
                {
                    if (formDraw != null && !formDraw.IsDisposed)
                    {
                        if (formDraw.Visible)
                            formDraw.Hide();
                    }
                }
                catch { }
            });
        }
        internal void doStateReset()
        {
            BeginInvoke((MethodInvoker)delegate()
            {
                try
                {
                    if (formControlGroup != null && !formControlGroup.IsDisposed)
                    {
                        formControlGroup.Hide();
                        formControlGroup.resetUI();
                    }
                }
                catch { }
                try 
                {
                    if (formControl != null && !formControl.IsDisposed)
                    {
                        if (formControl.Visible)
                            formControl.Hide();

                        if (formControl.shadowForm.Visible)
                            formControl.hideShadow();

                        if (formControl.windowCursorForm.Visible)
                        {
                            formControl.windowCursorForm.Hide();
                            Tools.changeCursorWindowsNormal();
                        }

                        if (formControl.windowCursorDropForm.Visible)
                        {
                            formControl.windowCursorDropForm.Hide();
                        }

                        if (formControl.windowCursorDropFormClick.Visible)
                        {
                            formControl.windowCursorDropFormClick.Hide();
                        }

                        if (pinchGestureState == PinchGestureStates.Dragging)
                        {
                            if (Tools.modeClickAndDrag)
                                triggerMouseMoveAndClickUpOnly(cursorPreviousClick, 0);
                            pinchGestureState = PinchGestureStates.None;
                        }
                        else
                        {
                            pinchGestureState = PinchGestureStates.None;
                        }

                        formControl.resetControls();
                    }
                }                
                catch { }
            });
        }

        long startTimeWake;

        internal void doStateWake()
        {
            BeginInvoke((MethodInvoker)delegate()
            {
                try
                {
                    //TODO
                    if (formControlGroup == null || formControlGroup.IsDisposed)
                    {
                        formControlGroup = new FormControlGroup();

                    }
                   //if(  formControlGroup.Visible

                    //if (!formControl.Visible)
                    //    formControl.Show();
                    scrollMode = false;

                    startTimeWake = DateTime.Now.Ticks;
                    currentObjectHoverIndex = -2;
                    currentObjectHover = null;
                    formControl.currentObjectHover = null;
                    handle = IntPtr.Zero;
                    //handleBitmap = null;
                    tempPointedWindow = IntPtr.Zero;


                    Tools.changeCursorWindowsNormal();

                }
                catch { }
            });
        }


        internal void checkWake()
        {
            if (formControlGroup != null && !formControlGroup.IsDisposed)
            {
                BeginInvoke((MethodInvoker)delegate()
                {
                    if (formControlGroup.Visible)
                        formControlGroup.Hide();
                });
            }
            if (formControl != null && !formControl.IsDisposed)
            {
                BeginInvoke((MethodInvoker)delegate()
                {
                    if (formControl.Visible)
                        formControl.Hide();
                    if (formControl.shadowForm.Visible)
                        formControl.shadowForm.Hide();
                    if (formControl.windowCursorForm.Visible)
                    {
                        Tools.changeCursorWindowsNormal();
                        formControl.windowCursorForm.Hide();
                    }
                    if (formControl.windowCursorDropForm.Visible)
                        formControl.windowCursorDropForm.Hide();
                });
            }
        }

        internal void doStatePointableAware()
        {
            try
            {
                initialPointedObjectPosition =
                    Vector3.nearestPointonLine(pointables[objectPointedIndex].position, fingertipCoordinate[0], fingertipCoordinateOut[0]);

                initialFingertipCoordinate[0] = new Vector3(fingertipCoordinate[0].X, fingertipCoordinate[0].Y, fingertipCoordinate[0].Z);
                initialFingertipCoordinateOut[0] = new Vector3(fingertipCoordinateOut[0].X, fingertipCoordinateOut[0].Y, fingertipCoordinateOut[0].Z);


                if (formControlGroup != null && !formControlGroup.IsDisposed)
                {
                    if (!formControlGroup.Visible)
                        formControlGroup.Show();
                }
                if (formControl == null || formControl.IsDisposed)
                {
                    formControl = new FormControl(this);
                }
                 
                formControl.setObjectControl(pointables[objectPointedIndex]);//, objectPointedIndex);

                formControl.Show();
                formControl.shadowForm.Show(); //todo

                Tools.SetWindowPos(this.formControl.Handle, Tools.HWND.TOPMOST, 0, 0, 0, 0,
Tools.SetWindowPosFlags.IgnoreMove | Tools.SetWindowPosFlags.IgnoreResize | Tools.SetWindowPosFlags.DoNotActivate);   
                //formControl.TopMost = true;
            }
            catch { }
        }

        internal void doStatePointableControl()
        {

        }


        internal void doStatePointableLaunch()
        {

        }

        private MultiIcon mMultiIcon = new MultiIcon();

        internal void doStateWindowPointable()
        {
            try
            {
                if (isOwnApplicationFocus)
                {
                    //string windowClass = FormFindWindow.getWindowClass(handle);
                    //if (FormFindWindow.getWindowTitle(handle) == "Pointable")
                    {
                        state.ProcessEvent(FiniteStateMachine.Events.EndGesture);
                        return;
                    }
                }
                if (formControl == null || formControl.IsDisposed)
                {
                    formControl = new FormControl(this);
                }


                if (!Tools.IsWindow(handle))
                {
                    state.ProcessEvent(FiniteStateMachine.Events.EndGesture);
                    return;
                }

                Bitmap windowIcon = null ;

                string windowClass = FormFindWindow.getWindowClass(handle);
                bool foundWindowPointable = false;
                PointableObject windowPointable = null;

                if (windowClass != null && windowClass != "" && FormFindWindow.checkWindowType(handle) != FormFindWindow.WindowType.Browser)
                {
                    if (windowClass == "Windows.UI.Core.CoreWindow")
                    {
                        string windowTitle = FormFindWindow.getWindowTitle(handle);
                        foreach (PointableObject pointableLocal in pointables)
                        {
                            if (pointableLocal.type == PointableObject.PointableType.Application && pointableLocal.isEnabled
                                    && pointableLocal.windowClass == windowClass && pointableLocal.windowTitle == windowTitle)
                            {
                                foundWindowPointable = true;
                                windowPointable = pointableLocal;
                                break;
                            }
                        }

                        if (!foundWindowPointable)
                        {//find default full screen pointable
                            foreach (PointableObject pointableLocal in pointables)
                            {
                                if (pointableLocal.type == PointableObject.PointableType.Application && pointableLocal.isEnabled
                                        && pointableLocal.windowClass == windowClass && pointableLocal.description == "Start Screen App")
                                {
                                    foundWindowPointable = true;
                                    windowPointable = pointableLocal;
                                    break;
                                }
                            }
                        }

                    }
                    else if (FormFindWindow.checkWindowType(handle) == FormFindWindow.WindowType.Tutorial)
                    {
                        string description;
                        if (FormTutorial.isAudio)
                        {
                            description = "Speakers";

                            windowPointable = pointableTutorialSpeakerRef;
                            formTutorial.startMusicIfNotPlaying();

                            foundWindowPointable = true;

                            try
                            {
                                if (FormTutorial.tutorialPointableProgress == 0)
                                {
                                    formTutorial.setTutorialPointableAnimation(1);
                                }
                            }
                            catch { }
                        }
                        else if (!FormTutorial.isLast)
                        {
                            description = "Tutorial Pointable";
                            
                            windowPointable = pointableTutorialNormalRef;
                            foundWindowPointable = true;
                        }else
                        {
                            description = "Tutorial Pointable Last";
                            
                            windowPointable = pointableTutorialLastRef;
                            foundWindowPointable = true;
                        }
                    }
                    else
                    {
                        foreach (PointableObject pointableLocal in pointables)
                        {
                            if (pointableLocal.type == PointableObject.PointableType.Application && pointableLocal.isEnabled
                                    && pointableLocal.windowClass == windowClass)
                            {
                                foundWindowPointable = true;
                                windowPointable = pointableLocal;
                                break;
                            }
                        }
                    }
                }

                if (!foundWindowPointable)
                {
                    if (FormFindWindow.checkWindowType(handle) == FormFindWindow.WindowType.Browser)
                    {
                        foreach (PointableObject pointableLocal in pointables)
                        {
                            if (pointableLocal.type == PointableObject.PointableType.Application && pointableLocal.isEnabled
                                && pointableLocal.description == "Browser")
                            {
                                foundWindowPointable = true;
                                windowPointable = pointableLocal;
                                windowIcon = getHandleIcon(handle);

                                break;
                            }
                        }
                    }
                    else if (FormFindWindow.checkWindowType(handle) == FormFindWindow.WindowType.BrowserVideo)
                    {
                        foreach (PointableObject pointableLocal in pointables)
                        {
                            if (pointableLocal.type == PointableObject.PointableType.Application && pointableLocal.isEnabled 
                                && pointableLocal.description == "Browser Video")
                            {
                                foundWindowPointable = true;
                                windowPointable = pointableLocal;
                                windowIcon = getHandleIcon(handle);
                                break;
                            }
                        }
                    }
                    else if (FormFindWindow.checkWindowType(handle) == FormFindWindow.WindowType.BrowserFullScreen)
                    {
                        foreach (PointableObject pointableLocal in pointables)
                        {
                            if (pointableLocal.type == PointableObject.PointableType.Application && pointableLocal.isEnabled
                                && pointableLocal.description == "Browser Full Screen")
                            {
                                foundWindowPointable = true;
                                windowPointable = pointableLocal;
                                windowIcon = getHandleIcon(handle);
                                break;
                            }
                        }
                    }
                    else if (FormFindWindow.isDesktop(handle))
                    {
                        foreach (PointableObject pointableLocal in pointables)
                        {
                            if (pointableLocal.type == PointableObject.PointableType.Application && pointableLocal.isEnabled
                                && pointableLocal.description == "Desktop")
                            {
                                foundWindowPointable = true;
                                windowPointable = pointableLocal;
                                break;
                            }
                        }
                    }
                }
                if (!foundWindowPointable)
                {
                    foreach (PointableObject pointableLocal in pointables)
                    {
                        if (pointableLocal.description == "Default")
                        {
                            foundWindowPointable = true;
                            windowPointable = pointableLocal;
                            windowIcon = getHandleIcon(handle);
                            break;
                        }
                    }
                    // formControl.setObjectControl(pointableEmpty, windowIcon);//, objectPointedIndex); //window pointable todo
                }

                if (isWindows81)
                {
                    if (windowPointable.type == PointableObject.PointableType.Application &&
                        windowPointable.description.Contains("Start Screen App"))
                    {
                        foreach (Action action in windowPointable.actions)
                        {
                            if (action.actionType == Action.ActionType.Keystroke)
                            {
                                if (action.description == "Start Screen Switch")
                                {
                                    action.keys[1] = 0x27; //right key                                    
                                }
                            }
                        }
                    }

                }

                if (!foundWindowPointable || windowPointable == null)
                {//no pointable for default found
                    state.ProcessEvent(FiniteStateMachine.Events.EndGesture);
                    return;
                }
                else
                {//if found
                    //if (windowIcon !=null)
                    //handleBitmap = windowIcon;

                    if (FormTutorial.isAudio)
                        formControl.setObjectControl(windowPointable);
                    else
                        formControl.setObjectControl(windowPointable, windowIcon);
                }



                initialFingertipCoordinate[0] = new Vector3(fingertipCoordinate[0].X, fingertipCoordinate[0].Y, fingertipCoordinate[0].Z);
                
                RECT Rect = new RECT();
                Point pointedScreenPosition = checkPointedScreenPosition();
                if (GetWindowRect(handle, ref Rect))
                {
                    originalWindowOffset.X = pointedScreenPosition.X - Rect.Left;
                    originalWindowOffset.Y = pointedScreenPosition.Y - Rect.Top;
                }

                //SetForegroundWindow(handle);
               // SetWindowPos(handle, HWND.TOPMOST, 0, 0, 0, 0, SetWindowPosFlags.IgnoreMove | SetWindowPosFlags.IgnoreResize);
                //SetWindowPos(handle, HWND.NOTOPMOST, 0, 0, 0, 0, SetWindowPosFlags.IgnoreMove | SetWindowPosFlags.IgnoreResize);
                

                if (formControlGroup != null && !formControlGroup.IsDisposed)
                {

                    if (Screen.AllScreens.Length > 1 && (Rect.Right - Rect.Left == screenRect.Width))
                    {
                        Rectangle screenBounds = Screen.AllScreens[Tools.ScreenNumber].Bounds;
                        foreach (Screen screen in Screen.AllScreens)
                        {
                            if (screen.Bounds.Contains(pointedScreenPosition))
                            {
                                screenBounds = screen.Bounds;
                                break;
                            }
                        }

                        formControlGroup.updatePosition(new Point((screenBounds.Left + screenBounds.Right) / 2,(screenBounds.Bottom + screenBounds.Top) / 2));
                        
                        if (!formControlGroup.Visible)
                            formControlGroup.Show();
                    }
                    else
                    {
                        formControlGroup.updatePosition(new Point((Rect.Left + Rect.Right) / 2, (Rect.Top + Rect.Bottom) / 2));

                        if ((Rect.Left + Rect.Right) / 2 != 0)
                        {
                            if (!formControlGroup.Visible)
                                formControlGroup.Show();
                        }
                    }

                    //scroll mode todo

                    // formControl.TopMost = true;
                }
                //if (formDraw == null || formDraw.IsDisposed)
                //{
                //    formDraw = new FormDraw();
                //}
                //formDraw.Opacity = 0;
                //formDraw.Visible = true;
                //formDraw.SetBounds(Rect.Left, Rect.Top, Rect.Right - Rect.Left, Rect.Bottom - Rect.Top);
                //formDraw.Opacity = 100;

                //formControl.drawWindowCursorAbsolute((int)currentPointedWindowPosition.X, (int)currentPointedWindowPosition.Y, true);
                if (formControl.windowCursorForm.Visible)
                {
                    Tools.changeCursorWindowsNormal();
                    formControl.windowCursorForm.Hide();
                }

                if (formControl.windowCursorDropForm.Visible)
                    formControl.windowCursorDropForm.Hide();

                formControl.drawSpotlight(50, 50, true);
                
                formControl.Show();
                //scroll mode todo
                formControl.shadowForm.Show();

                //formControl.TopMost = true;
                Tools.SetWindowPos(this.formControl.Handle, Tools.HWND.TOPMOST, 0, 0, 0, 0,
Tools.SetWindowPosFlags.IgnoreMove | Tools.SetWindowPosFlags.IgnoreResize | Tools.SetWindowPosFlags.DoNotActivate);

                if (formTutorial != null && !formTutorial.IsDisposed && formTutorial.Visible)
                {
                    formfullScreen.Focus();
                    formTutorial.Focus();
                }
            }
            catch (Exception ex) {  }
        }

        bool isWindowSpecial = false; //desktop and startmenu

        System.Timers.Timer _timerWindowCheckExist;
        void _timerWindowCheckExist_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                //_timerProgressClick.Enabled = false;
                if (!Tools.IsWindow(handle))
                {
                    _timerWindowCheckExist.Enabled = false;
                    if (formDraw != null && !formDraw.IsDisposed && formDraw.Visible)
                    {
                        this.BeginInvoke((MethodInvoker)delegate()
                        {
                            if (formDraw != null && formDraw.Visible)
                                formDraw.Hide();
                            state.ProcessEvent(FiniteStateMachine.Events.EndGesture);
                            //change mode
                        });
                    }
                }
                else
                {//if exist //check position
                    if (state.State == FiniteStateMachine.States.WindowAware)
                    {
                        if (formDraw != null && !formDraw.IsDisposed && formDraw.Visible)
                        {
                            RECT Rect = new RECT();
                            WINDOWPLACEMENT placement = new WINDOWPLACEMENT();
                            GetWindowPlacement(handle, ref placement);
                            if (placement.showCmd == ShowWindowCommands.Maximized)
                            {
                                GetWindowRect(handle, ref Rect);
                                Rectangle screenBounds = Screen.AllScreens[Tools.ScreenNumber].WorkingArea;
                                foreach (Screen screen in Screen.AllScreens)
                                {
                                    if (screen.Bounds.Contains((Rect.Left + Rect.Right) / 2, (Rect.Top + Rect.Bottom) / 2))
                                    {
                                        screenBounds = screen.Bounds;
                                        Rect.Left = screenBounds.Left;
                                        Rect.Right = screenBounds.Right;
                                        Rect.Top = screenBounds.Top;
                                        Rect.Bottom = screenBounds.Bottom;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                GetWindowRect(handle, ref Rect);
                            }
                            formDraw.SetBounds(Rect.Left, Rect.Top, Rect.Right - Rect.Left, Rect.Bottom - Rect.Top);
                        }

                    }
                }
            }
            catch { }
        }

        IntPtr previousWindowAwareHandle;

        internal void doStateWindowAware()
        {
            try
            {
                //Bitmap windowIcon = Tools.GetBitmapIcon(handle);
                //formControl.setObjectControl(pointableEmpty, windowIcon);//, objectPointedIndex); //window pointable todo
                previousWindowAwareHandle = handle;


                initialFingertipCoordinate[0] = new Vector3(fingertipCoordinate[0].X, fingertipCoordinate[0].Y, fingertipCoordinate[0].Z);

                isWindowSpecial = (FormFindWindow.checkWindowType(handle) == FormFindWindow.WindowType.SpecialWindow);

                RECT Rect = new RECT();
                WINDOWPLACEMENT placement = new WINDOWPLACEMENT();
                GetWindowPlacement(handle, ref placement);
                if (placement.showCmd == ShowWindowCommands.Maximized)
                {
                    GetWindowRect(handle, ref Rect);
                    Rectangle screenBounds = Screen.AllScreens[Tools.ScreenNumber].WorkingArea;
                    foreach (Screen screen in Screen.AllScreens)
                    {
                        if (screen.Bounds.Contains((Rect.Left + Rect.Right) / 2, (Rect.Top + Rect.Bottom) / 2))
                        {
                            screenBounds = screen.Bounds;
                            Rect.Left = screenBounds.Left;
                            Rect.Right = screenBounds.Right;
                            Rect.Top = screenBounds.Top;
                            Rect.Bottom = screenBounds.Bottom;
                            break;
                        }
                    }
                }
                else
                {
                    GetWindowRect(handle, ref Rect);
                }
                //RECT Rect = new RECT();
               // if (GetWindowRect(handle, ref Rect))
                {
                    Point pointedScreenPosition = checkPointedScreenPosition();
                    originalWindowOffset.X = pointedScreenPosition.X - Rect.Left;
                    originalWindowOffset.Y = pointedScreenPosition.Y - Rect.Top;
                }

                if (!isWindowSpecial)
                {
                    //SetWindowPos(handle, HWND.TOPMOST, 0, 0, 0, 0, SetWindowPosFlags.IgnoreMove | SetWindowPosFlags.IgnoreResize);
                    //SetWindowPos(handle, HWND.NOTOPMOST, 0, 0, 0, 0, SetWindowPosFlags.IgnoreMove | SetWindowPosFlags.IgnoreResize);
                    SetForegroundWindow(handle);
                }
                //SetFocus(handle);

                if (formDraw == null || formDraw.IsDisposed)
                {
                    formDraw = new FormDraw();
                }

                if (!isWindowSpecial)
                {
                    formDraw.Opacity = 0;
                    formDraw.Visible = true;
                    formDraw.SetBoundsFade(Rect.Left, Rect.Top, Rect.Right - Rect.Left, Rect.Bottom - Rect.Top);

                    Tools.SetWindowPos(this.formDraw.Handle, Tools.HWND.TOPMOST, 0, 0, 0, 0,
Tools.SetWindowPosFlags.IgnoreMove | Tools.SetWindowPosFlags.IgnoreResize | Tools.SetWindowPosFlags.DoNotActivate);     
                    //formDraw.TopMost = true;
                }
                else
                    formDraw.Visible = false;
                //formDraw.Opacity = 100;

                formControl.drawWindowCursorAbsolute((int)currentPointedWindowPosition.X, (int)currentPointedWindowPosition.Y, true, fingertipCoordinate[0]);

                if (!formControl.windowCursorForm.Visible)
                    formControl.windowCursorForm.Show();
                //scroll mode todo
                //formControl.shadowForm.Show();

                //formControl.windowCursorForm.TopMost = true;
                //if(false)//if (!(tutorialInProgress && FormFindWindow.checkWindowType(handle) == FormFindWindow.WindowType.DragTestWindow))
                {
                    Tools.SetWindowPos(formControl.windowCursorForm.Handle, Tools.HWND.TOPMOST, 0, 0, 0, 0,
    Tools.SetWindowPosFlags.IgnoreMove | Tools.SetWindowPosFlags.IgnoreResize | Tools.SetWindowPosFlags.DoNotActivate); 
                }    

                newWindowScroll = true;

                var procId = Process.GetCurrentProcess().Id;
                int activeProcId;
                Tools.GetWindowThreadProcessId(handle, out activeProcId);
                
                if (activeProcId == procId)
                    isOwnApplicationFocus = true;
                else
                    isOwnApplicationFocus = false;

                if (isOwnApplicationFocus)
                {
                    if (FormFindWindow.checkWindowType(handle) == FormFindWindow.WindowType.Tutorial)
                    {
                        isOwnApplicationFocus = false;
                    }
                }


                if (_timerWindowCheckExist != null)
                {
                    _timerWindowCheckExist.Enabled = false;
                    _timerWindowCheckExist = null;
                }

                if (_timerWindowCheckExist == null)
                {
                    _timerWindowCheckExist = new System.Timers.Timer(150); //300
                    _timerWindowCheckExist.Elapsed += new ElapsedEventHandler(_timerWindowCheckExist_Elapsed);
                    _timerWindowCheckExist.Enabled = true;
                }
            }
            catch (Exception ex){ Debug.WriteLine(ex.ToString()); }
        }
        bool newWindowScroll = false;

        internal void doStateWindowControl()
        {
            try
            {
                if (formControlGroup != null && !formControlGroup.IsDisposed)
                {
                    //   formControlGroup.Hide();
                }
                if (formControl == null || formControl.IsDisposed)
                {
                    formControl = new FormControl(this);
                }

               // formControl.setObjectControl(pointableEmpty);//, objectPointedIndex); //window pointable todo
                
                initialFingertipCoordinate[0] = new Vector3(fingertipCoordinate[0].X, fingertipCoordinate[0].Y, fingertipCoordinate[0].Z);


                RECT Rect = new RECT();
                WINDOWPLACEMENT placement = new WINDOWPLACEMENT();
                GetWindowPlacement(handle, ref placement);
                switch (placement.showCmd)
                {
                    case ShowWindowCommands.Normal:
                        break;
                    case ShowWindowCommands.Minimized:
                        break;
                    case ShowWindowCommands.Maximized:
                        GetWindowRect(handle, ref Rect);
                        Rectangle screenBounds = Screen.AllScreens[Tools.ScreenNumber].Bounds;
                        foreach (Screen screen in Screen.AllScreens)
                        {
                            if (screen.Bounds.Contains((Rect.Left + Rect.Right)/2, (Rect.Top + Rect.Bottom)/2))
                            {
                                screenBounds = screen.Bounds;
                            }
                        }
                        ShowWindow(handle, SW_RESTORE);
                        GetWindowRect(handle, ref Rect);

                        SetWindowPos(handle, HWND.NOTOPMOST, screenBounds.Left + screenBounds.Width/2 - (Rect.Right - Rect.Left) / 2,
                            screenBounds.Top + screenBounds.Height / 2 -(Rect.Bottom - Rect.Top) / 2, Rect.Right - Rect.Left, Rect.Bottom - Rect.Top, 0);
                     
                        break;

                }


                if (GetWindowRect(handle, ref Rect))
                {
                    Point pointedScreenPosition = checkPointedScreenPosition();
                    originalWindowOffset.X = pointedScreenPosition.X - Rect.Left;
                    originalWindowOffset.Y = pointedScreenPosition.Y - Rect.Top;
                }

                formControlGroup.updatePosition(new Point((Rect.Left + Rect.Right) / 2, (Rect.Top + Rect.Bottom) / 2));
                if (formDraw == null || formDraw.IsDisposed)
                {
                    formDraw = new FormDraw();
                }
                formDraw.SetBoundsFixed(Rect.Left, Rect.Top, Rect.Right - Rect.Left, Rect.Bottom - Rect.Top);
                formDraw.Visible = true;

                if (formControl.Visible)
                    formControl.Hide();
                if (formControl.shadowForm.Visible)
                    formControl.shadowForm.Hide();

                formControl.setCursorMove();
                if (!formControl.windowCursorForm.Visible)
                    formControl.windowCursorForm.Show();
                //formControl.Show();
                //formControl.shadowForm.Show();
               // formControl.TopMost = true;
            }
            catch { }
        }
    #endregion

        #region state machine
        internal class FiniteStateMachine
        {
            public enum States { Standby, Wake, PointableAware, PointableControl, PointableLaunch, Calibrating, WindowAware, WindowControl, WindowPointable };
            public States State { get; set; }

            public enum Events { StartGesture, EndGesture, FoundPointable, DetectedGesture, DetectedSelection, TimeOut, PalmClosed, StartCalibration, EndCalibration, FoundWindow, PinchGesture};

            private System.Action[,] fsm;

            Form1 mForm;

            public FiniteStateMachine(Form1 _mform)
            {//http://stackoverflow.com/questions/5923767/simple-state-machine-example-in-c
                this.fsm = new System.Action[9, 11] {
                  //StartGesture	EndGesture	FoundPointable	DetectedGesture	DetectedSelection	Timeout	    PalmClosed	, StartCalibration  ,EndCalibration,  FoundWindow, PinchGesture
                {   Ignore	, Ignore  ,   Ignore   	,      Ignore        ,     Ignore   , Ignore    , Ignore    ,   GotoCalibrating,    Ignore     ,    Ignore   ,    Ignore },    //Standby
                {	Ignore      ,  GotoCheckWake, GotoPointableAware,   Ignore        ,    Ignore	,GotoStandby, StartTimer ,GotoCalibrating   ,  Ignore   , GotoWindowAware,    Ignore },  //Wake
                {	Ignore	,  GotoStandby ,   Ignore       ,GotoPointableControl,  Ignore  ,   Ignore   ,GotoStandby   ,   Ignore         ,  Ignore      ,    Ignore    ,   Ignore  },      //PointableAware
                {	GotoWake    ,	GotoStandby	 ,   Ignore   	,      Ignore	,GotoPointableLaunch, Ignore, GotoStandby   ,    Ignore         ,  Ignore     ,     Ignore    ,    Ignore   },      //PointableControl
                {	GotoWake    ,	GotoStandby	 ,   Ignore   	,      Ignore     ,     Ignore     ,Ignore ,GotoStandby	    ,    Ignore         ,  Ignore      ,    Ignore    ,    Ignore },	//PointableLaunch
                {	Ignore        ,	Ignore	 ,       Ignore   	,      Ignore     ,     Ignore     ,Ignore ,   Ignore	    ,    Ignore     , GotoStandbyFromCalibration,  Ignore  ,  Ignore  },	//Calibrating
           {GotoWindowPointable, GotoStandby,GotoPointableAware,     Ignore     ,      Ignore     ,Ignore     ,GotoStandby , GotoCalibrating   ,  Ignore     ,GotoWindowAware,GotoWindowControl  },	//WindowAware
                {	GotoWake    ,	GotoStandby	 ,   Ignore   	,      Ignore     ,     Ignore     ,Ignore ,GotoStandby	    ,    Ignore         ,  Ignore     ,    Ignore,    Ignore  },	//WindowControl
                {	Ignore     ,	GotoStandby	 ,   Ignore   	,      Ignore     ,     Ignore     ,GotoStandby ,StartTimer	 ,    Ignore         ,  Ignore     ,    Ignore,    Ignore  },	//WindowPointable

                //windowaware should have GOTOSTANDBYWITHDELAY, GotoStandbyWithDelay

                };   //on

                mForm = _mform;
                GotoStandby();

                //   //StartGesture	EndGesture	FoundPointable	DetectedGesture	DetectedSelection	Timeout	    PalmClosed	, StartCalibration  ,EndCalibration,  FoundWindow, PinchGesture
                //{GotoWakeFromStandby	, Ignore  ,   Ignore   	,      Ignore        ,     Ignore   , Ignore    , Ignore    ,   GotoCalibrating,    Ignore     ,    Ignore   ,    Ignore },    //Standby
                //{	Ignore      ,  GotoCheckWake, GotoPointableAware,   Ignore        ,    Ignore	,GotoStandby, StartTimer ,	   GotoCalibrating   ,  Ignore       , GotoWindowAware,    Ignore },  //Wake
                //{	GotoWake	,  GotoStandby ,   Ignore       ,GotoPointableControl,  Ignore  ,   Ignore   ,GotoStandby   ,   Ignore         ,  Ignore      ,    Ignore    ,   Ignore  },      //PointableAware
                //{	GotoWake    ,	GotoStandby	 ,   Ignore   	,      Ignore	,GotoPointableLaunch, Ignore, GotoStandby   ,    Ignore         ,  Ignore     ,     Ignore    ,    Ignore   },      //PointableControl
                //{	GotoWake    ,	GotoStandby	 ,   Ignore   	,      Ignore     ,     Ignore     ,Ignore ,GotoStandby	    ,    Ignore         ,  Ignore      ,    Ignore    ,    Ignore },	//PointableLaunch
                //{	Ignore        ,	Ignore	 ,       Ignore   	,      Ignore     ,     Ignore     ,Ignore ,   Ignore	    ,    Ignore     , GotoStandbyFromCalibration,  Ignore  ,  Ignore  },	//Calibrating
                //{GotoWindowPointable,GotoStandbyWithDelay,GotoPointableAware ,  Ignore     ,     Ignore     ,Ignore ,GotoStandby	    ,    Ignore         ,  Ignore     ,GotoWindowAware,GotoWindowControl  },	//WindowAware
                //{	GotoWake    ,	GotoStandby	 ,   Ignore   	,      Ignore     ,     Ignore     ,Ignore ,GotoStandby	    ,    Ignore         ,  Ignore     ,    Ignore,    Ignore  },	//WindowControl
                //{	Ignore     ,	GotoStandby	 ,   Ignore   	,      Ignore     ,     Ignore     ,GotoStandby ,StartTimer	 ,    Ignore         ,  Ignore     ,    Ignore,    Ignore  },	//WindowPointable
            }
            
            public void ProcessEvent(Events theEvent)
            {
                int i = (int)this.State;
                int j = (int)theEvent;

                this.fsm[i, j].Invoke();
            }

            //do nothing
            public void Ignore()
            {
            }

            //reset all previous states
            //gotostandby
            //wait for pointed object
            private void GotoWakeFromStandby() 
            {
                this.State = States.Wake;
                Debug.WriteLine(this.State);
                //reset states
                mForm.doStateResetFromStandby();
                mForm.doStateWake();
            }

            private void GotoCheckWake()
            {
                mForm.checkWake();
            }
            private void GotoWake()
            {
                this.State = States.Wake;
                Debug.WriteLine(this.State);
                //reset states
                mForm.doStateReset();
                mForm.doStateWake();
            }

            //reset all previous states
            private void GotoStandby() 
            {
                this.State = States.Standby;
                Debug.WriteLine(this.State);

                //reset states
               //todo// mForm.doStateResetFromStandby();
                GotoWakeFromStandby();
            }

            bool newStandbyDelay = true;
            long standbyDelayStartTime;

            private void GotoStandbyWithDelay()
            {
                try
                {
                    //this.State = States.Standby;
                    Debug.WriteLine(this.State);

                    if (newStandbyDelay)
                    {
                        newStandbyDelay = false;
                        standbyDelayStartTime = DateTime.Now.Ticks;
                    }
                    long durationPrevious = (long)((DateTime.Now.Ticks - standbyDelayStartTime) / 10000);
                    if (durationPrevious < 500)
                        return;

                    newStandbyDelay = true;
                    //reset states
                    //todo// mForm.doStateResetFromStandby();
                    GotoWakeFromStandby();
                }
                catch { }
            }

            //detected pointed object
            //show context of object
            //wait to determine gesture or select
            private void GotoPointableAware()
            {
                try
                {
                    if (_timer != null)
                        _timer.Stop();
                }
                catch { }
                this.State = States.PointableAware;
                Debug.WriteLine(this.State);

                mForm.doStatePointableAware();
            }

            //detected gesture
            //detect and trigger change
            private void GotoPointableControl() 
            {
                this.State = States.PointableControl;
                Debug.WriteLine(this.State);
                mForm.doStatePointableControl();
            }


            //detected selection
            //trigger action
            private void GotoPointableLaunch()
            {
                this.State = States.PointableLaunch;
                Debug.WriteLine(this.State);
                mForm.doStatePointableLaunch();
            }
            //detected pointed window
            //show context of window
            //wait to determine gesture or select
            private void GotoWindowAware()
            {
                try
                {
                    if (_timer != null)
                        _timer.Stop();
                }
                catch { }
               // return;
               // return; //todo scroll
                this.State = States.WindowAware;
                Debug.WriteLine(this.State);

                mForm.doStateWindowAware();
            }
            private void GotoWindowControl()
            {
                this.State = States.WindowControl;
                Debug.WriteLine(this.State);
                mForm.doStateWindowControl();
            }

            private void GotoWindowPointable()
            {
                //delay 1second
                Thread.Sleep(200);
                this.State = States.WindowPointable;
                Debug.WriteLine(this.State);
                mForm.doStateWindowPointable();
            }

            private void GotoCalibrating()
            {
                this.State = States.Calibrating;

                Debug.WriteLine(this.State);
                //reset states
                mForm.doStateResetFromStandby();

                mForm.doStateCalibrateScreen();
                //call calibrating
            }

            private void GotoStandbyFromCalibration()
            {
                mForm.doStateEndCalibration();
                GotoStandby();
            }

            System.Timers.Timer _timer;
            private void StartTimer()
            {

               // this.ProcessEvent(FiniteStateMachine.Events.TimeOut);
                try
                {
                    if (_timer == null || !_timer.Enabled)
                    {
                        _timer = new System.Timers.Timer(700); //300
                        _timer.Elapsed += new ElapsedEventHandler(_timer_Elapsed);
                        _timer.Enabled = true;
                    }
                }
                catch { }
            }

            void _timer_Elapsed(object sender, ElapsedEventArgs e)
            {
                try
                {
                    _timer.Stop();
                }
                catch { }
                try
                {
                    this.ProcessEvent(FiniteStateMachine.Events.TimeOut);
                }
                catch { }
            }
        }
        #endregion //statremachine

        FormShutdown formShutdown;

        internal void systemRestart()
        {
            BeginInvoke((MethodInvoker)delegate()
            {
                try
                {
                    if (formShutdown != null && !formShutdown.IsDisposed)
                    {
                        this.formShutdown.Close();
                    }
                    else
                    {
                        this.formShutdown = new FormShutdown(this, false);
                        this.formShutdown.TopMost = true;
                        this.formShutdown.Show();
                    }
                }
                catch { }
            });
        }

        internal void systemShutdown()
        {
            BeginInvoke((MethodInvoker)delegate()
            {
                try
                {
                    if (formShutdown != null && !formShutdown.IsDisposed)
                    {
                        this.formShutdown.Close();
                    }
                    else
                    {
                        this.formShutdown = new FormShutdown(this, true);
                        this.formShutdown.TopMost = true;
                        this.formShutdown.Show();
                    }
                }
                catch { }
            });
        }

        MainWindow mainWindow;

        private void buttonConfiguration_Click(object sender, EventArgs e)        
        {
            launchConfiguration();
        }
        
        internal void launchConfiguration()
        {
            thisForm.BeginInvoke((MethodInvoker)delegate()
            {
                try
                {
                    if (mainWindow == null)
                    {
                        this.mainWindow = new MainWindow(this);
                        this.mainWindow.Closed += mainWindowClosed;//(sender, args) => this.mainWindow = null;
                        //this.mainWindow.Left = this.Left + this.Width - this.mainWindow.Width;
                        //this.mainWindow.Top = this.Top + this.Height / 2 - this.mainWindow.Height / 2;
                        this.mainWindow.comboBoxApplicationRefreshing = true;
                        this.mainWindow.Topmost = true;
                        this.mainWindow.Show();

                        this.mainWindow.Topmost = false;
                        this.mainWindow.comboBoxApplicationRefreshing = false;
                    }
                    else
                    {
                        this.mainWindow.Close();
                    }
                }
                catch { }
            });
        }

        public void mainWindowClosed(object sender, System.EventArgs e)
        {
            mainWindow = null;
        }

        private void notifyIconSysTray_MouseDoubleClick(object sender, MouseEventArgs e)
        {

        }

        private void notifyIconSysTray_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {

                if (e.Button == MouseButtons.Left)
                {
                    launchConfiguration();
                    //if (WindowState != FormWindowState.Normal)
                    //{
                    //    Show();
                    //    WindowState = FormWindowState.Normal;
                    //    this.TopMost = true;
                    //    this.Focus();

                    //    this.TopMost = false;
                    //}
                    //else
                    //{
                    //    WindowState = FormWindowState.Minimized;
                    //}
                }
            }
            catch { }
        }

        private void toolStripMenuItemConfiguration_Click(object sender, EventArgs e)
        {
            launchConfiguration();
        }

        private void screen1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                screen1ToolStripMenuItem.Checked = true;
                screen2ToolStripMenuItem.Checked = false;
                screen3ToolStripMenuItem.Checked = false;

                Tools.ScreenNumber = 0;
                formControlGroup.updatePosition();
            }
            catch { }
        }

        private void screen2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                screen1ToolStripMenuItem.Checked = false;
                screen2ToolStripMenuItem.Checked = true;
                screen3ToolStripMenuItem.Checked = false;
                Tools.ScreenNumber = 1;
                formControlGroup.updatePosition();
            }
            catch { }
        }

        private void screen3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                screen1ToolStripMenuItem.Checked = false;
                screen2ToolStripMenuItem.Checked = false;
                screen3ToolStripMenuItem.Checked = true;
                Tools.ScreenNumber = 2;
                formControlGroup.updatePosition();
            }
            catch { }
        }

        internal static bool detectionActivated = true;

        private void disablePointableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pauseResumeTracking();
        }

        internal void pauseResumeTracking()
        {
            try
            {
                if (tutorialInProgress)
                {
                    BeginInvoke((MethodInvoker)delegate()
                    {
                        try
                        {
                            notifyIconSysTray.BalloonTipText = "Tutorial in Progress!";
                            notifyIconSysTray.ShowBalloonTip(3000);
                        }
                        catch { }
                    });
                    return;
                }


                if (detectionActivated)
                {
                    detectionActivated = false;
                    BeginInvoke((MethodInvoker)delegate()
                    {
                        try
                        {
                            disablePointableToolStripMenuItem.Text = "Resume Pointable Tracking";
                            notifyIconSysTray.Icon = new Icon(Form1.executable.GetManifestResourceStream("Pointable.Resources.Pointable_NotActivated.ico"));

                            notifyIconSysTray.BalloonTipText = "Pointable tracking paused";
                            notifyIconSysTray.ShowBalloonTip(3000);

                            state.ProcessEvent(FiniteStateMachine.Events.EndGesture);
                        }
                        catch { }
                    });
                }
                else
                {
                    detectionActivated = true;
                    BeginInvoke((MethodInvoker)delegate()
                    {
                        try
                        {
                            disablePointableToolStripMenuItem.Text = "Pause Pointable Tracking";
                            notifyIconSysTray.Icon = new Icon(Form1.executable.GetManifestResourceStream("Pointable.Resources.pointable2.ico"));


                            notifyIconSysTray.BalloonTipText = "Pointable tracking resumed";
                            notifyIconSysTray.ShowBalloonTip(3000);
                        }
                        catch { }
                    });
                }
            }
            catch { }
        }

        private void exitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                System.Windows.Forms.Application.Exit();
            }
            catch { }
        }

        #region pointable default

        ////Speakers
        //Action actionSpecialSoundIncrease = new Action("Increase Volume", "sound_increase.png", "", "", "", "", 
        //    (byte)0x004, (byte)0x00, (byte)0x00, (byte)0x00);
        //Action actionSpecialSoundNext = new Action("Next Track", "sound_next.png", "", "", "", "", 
        //    (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00);
        //Action actionSpecialSoundDecrease = new Action("Decrease Volume", "sound_decrease.png", "", "", "", "", 
        //    (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00);
        //Action actionSpecialSoundMute = new Action("Mute", "sound_mute.png", "F5", "", "", "", 
        //    (byte)0x00, (byte)0x00, (byte)0x00, (byte)0x00);

        ////Screen
        //Action actionLaunchPointableSite = new Action("Launch Pointable.net", "screen_pointable.png", "http://www.pointable.net", "");
        //Action actionSpecialSettings = new Action("Open Pointable Configuration", "screen_settings.png", "special", "");
        //Action actionKeyShowDesktop = new Action("Show Desktop", "screen_show_desktop.png", "Win", "D", "", "", (byte)0x5b, (byte)0x44, (byte)0x00, (byte)0x00);
        //Action actionLaunchTwitterShare = new Action("Launch Twitter Share", "screen_twitter.png", "http://www.twitter.com", "");


        //Action actionLaunchPowerPoint = new Action("Launch PowerPoint", "screen_ppt.png","powerpnt","" );
        //Action actionLaunchWord = new Action("Launch Word", "screen_word.png", "winword", "");
        //Action actionLaunchExcel = new Action("Launch Excel", "screen_excel.png", "excel", "");
        //Action actionLaunchChrome = new Action("Launch Chrome", "screen_chrome.png", "chrome", "");
        
        //Action actionLaunchFacebook = new Action("Facebook", "monitor_facebook.png", "http://www.facebook.com", "");
        //Action actionLaunchYahoo = new Action("Yahoo", "monitor_yahoo.png", "http://www.yahoo.com", "");
        //Action actionLaunchCNN = new Action("CNN", "monitor_cnn.png", "http://www.cnn.com", "");
        //Action actionLaunchEngadget = new Action("Engadget", "monitor_engadget.png", "http://www.engadget.com", "");

        //Action actionLaunchPhotoshop = new Action("Photoshop", "picture_photoshop.png", @"C:\Program Files (x86)\Adobe\Adobe Photoshop CS4\Photoshop.exe", "");

        ////Action actionKeyShowDesktop = new Action("Show Desktop", "key_keyboard.png","Win", "D","","", (byte)0x5b, (byte)0x44, (byte)0x00, (byte)0x00);
        //Action actionKeyCopy = new Action("Copy", "key_keyboard.png", "Control", "C", "", "", (byte)0x11, (byte)0x43, (byte)0x00, (byte)0x00);
        //Action actionKeyPaste = new Action("Paste", "key_keyboard.png", "Control", "P", "", "", (byte)0x11, (byte)0x50, (byte)0x00, (byte)0x00);

        //Action actionKeyRefresh = new Action("Browser Refresh", "key_keyboard.png", "F5", "", "", "", (byte)0x74, (byte)0x00, (byte)0x00, (byte)0x00);
        //Action actionKeyBack = new Action("Browser Back", "key_keyboard.png", "Alt", "Left", "", "", (byte)0x12, (byte)0x25, (byte)0x00, (byte)0x00);
        //Action actionKeyForward = new Action("Browser Forward", "key_keyboard.png", "Alt", "Right", "", "", (byte)0x12, (byte)0x27, (byte)0x00, (byte)0x00);
        //Action actionKeyNewTab = new Action("Browser New Tab", "key_keyboard.png", "Control", "T", "", "", (byte)0x11, (byte)0x54, (byte)0x00, (byte)0x00);


        Action actionKeyLaunchMyComputer = new Action("Launch My Computer", "keystroke_my_computer.png", "Win", "E", "", "", (byte)0x5b, (byte)0x45, (byte)0x00, (byte)0x00);
        Action actionKeyCycleThroughPrograms = new Action("Cycle Through Programs", "keystroke_windows.png", "Alt", "Escape", "", "", (byte)0x12, (byte)0x1B, (byte)0x00, (byte)0x00);
       // Action actionKeyLockComputer = new Action("Lock Computer", "keystroke_lock.png", "Win", "L", "", "", (byte)0x5b, (byte)0x4C, (byte)0x00, (byte)0x00);
        Action actionKeyPrint = new Action("Print", "keystroke_print.png", "Control", "P", "", "", (byte)0x11, (byte)0x50, (byte)0x00, (byte)0x00);

        Action actionKeyMaximize = new Action("Window - Maximize", "keystroke_up.png", "Win", "Up", "", "", (byte)0x5b, (byte)0x26, (byte)0x00, (byte)0x00);
        Action actionKeyMinimize = new Action("Window - Minimize", "keystroke_down.png", "Win", "Down", "", "", (byte)0x5b, (byte)0x28, (byte)0x00, (byte)0x00);
        Action actionKeyLeft = new Action("Window - Snap to the left", "keystroke_left.png", "Win", "Left", "", "", (byte)0x5b, (byte)0x25, (byte)0x00, (byte)0x00);
        Action actionKeyRight = new Action("Window - Snap to the right", "keystroke_right.png", "Win", "Right", "", "", (byte)0x5b, (byte)0x27, (byte)0x00, (byte)0x00);
  

        #endregion

        #region pointable data
        internal List<PointableObject> pointables = new List<PointableObject>();
        //store all actions
        internal List<Action> actionsApplication = new List<Action>();
        internal List<Action> actionsKeystrokes = new List<Action>();

        internal static string resourcePath = @"pack://application:,,/PointableUI;component/Resources/";

        //internal static string directoryPointableLab = @"\Pointable Lab";
        //internal static string directoryPointableFolder = @"\Pointable";
        internal static string iconFolderPath = @"Pointable Lab\Pointable\Icons\";
        internal static string pointableFolderPath = @"Pointable Lab\Pointable\Pointables\";
        internal static string settingsFolderPath = @"Pointable Lab\Pointable\";

        internal static string exeDirectory;
        internal static string iconFolderPathDefault;
        internal static string pointableFolderPathDefault;
        internal static string settingsFile = @"\settings.ini";
        internal static System.Reflection.Assembly executable;

        internal void savePointableData(PointableObject pointableToSave)
        {
            try
            {
                // if (pointable.description != "Screen" && pointable.description != "Speakers")
                {
                    pointableToSave.keysToKeyCode();
                    //System.Windows.Forms.MessageBox.Show(pointableToSave.position.X.ToString() + " " + pointableToSave.position.Y.ToString() + " " + pointableToSave.position.Z.ToString() + " ");
                    System.Xml.Serialization.XmlSerializer writer =
                        new System.Xml.Serialization.XmlSerializer(typeof(PointableObject));

                    System.IO.StreamWriter file = new System.IO.StreamWriter(
                        pointableFolderPath + pointableToSave.description + ".pnt");
                    writer.Serialize(file, pointableToSave);
                    file.Close();
                }
            }
            catch { }
        }

        internal void deletePointableFile(PointableObject pointableToDelete)
        {
            try
            {
                string fileToDelete = pointableFolderPath + pointableToDelete.description + ".pnt";
                File.Delete(fileToDelete);
            }
            catch { }
        }
        private void savePointableDataAll()
        {
            //remove pointable files existing
            //string[] filesInFolder = Directory.GetFiles(pointableFolderPath);

            string[] filesInFolder = new string[] { "" };
            try
            {
                filesInFolder = Directory.GetFiles(pointableFolderPath);
            }
            catch { }
            

            foreach (string file in filesInFolder)
            {
                if (Path.GetExtension(file) == ".pnt")
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch { }
                }
            }

            foreach (PointableObject pointable in pointables)
            {
                savePointableData(pointable);
            }
        }

        PointableObject pointableScreenRef;
        PointableObject pointableSpeakerRef;
        PointableObject pointableTutorialSpeakerRef;
        PointableObject pointableTutorialNormalRef;
        PointableObject pointableTutorialLastRef;

        private void loadPointableDataFromFile()
        {
            //return;

            //load speaker and screen from resource
            string[] strResource = new string[] { "Pointable_Screen.xml", "Pointable_Speakers.xml" };

            //new StreamReader(Form1.executable.GetManifestResourceStream("Pointable.Resources.Pointables_ball.png"));//Image.FromFile(file) as Bitmap; //Form1.iconFolderPath + "Pointables_spotlight.png"
            foreach (string file in strResource)
            {
                if (Path.GetExtension(file) == ".xml")
                {
                    try
                    {
                        string xml;
                        using (StreamReader sr = new StreamReader(Form1.executable.GetManifestResourceStream("Pointable.Resources." + file)))//Image.FromFile(file) as Bitmap; //Form1.iconFolderPath + "Pointables_spotlight.png")
                        {
                            xml = sr.ReadToEnd();
                        }
                        XmlSerializer serializer = new XmlSerializer(typeof(PointableObject));
                        PointableObject newPointable = (PointableObject)serializer.Deserialize(new StringReader(xml));
                        //newPointable.keycodeToKeys();

                        if (newPointable.description == "Configuration")
                        {
                            //if (firstRun)
                            //{
                            //    newPointable.position = new Vector3(0, 150, -200);
                            //    newPointable.calibrated = true;
                            //}
                            pointableScreenRef = newPointable;
                        }
                        else if (newPointable.description == "Speakers")
                        {
                            pointableSpeakerRef = newPointable;
                        }

                        if (pointables.Count < 50) //8
                            pointables.Add(newPointable);

                       // System.Windows.Forms.MessageBox.Show(newPointable.position.X.ToString() + " " + newPointable.position.Y.ToString() + " " + newPointable.position.Z.ToString() + " ");
              
                    }
                    catch { }
                }
            }

            strResource = new string[] { "Pointable_Tutorial.pnt", "Pointable_Tutorial_Last.pnt", "Pointable_TutorialAudio.xml" };

            //new StreamReader(Form1.executable.GetManifestResourceStream("Pointable.Resources.Pointables_ball.png"));//Image.FromFile(file) as Bitmap; //Form1.iconFolderPath + "Pointables_spotlight.png"
            foreach (string file in strResource)
            {
                //if (Path.GetExtension(file) == ".xml")
                {
                    try
                    {
                        string xml;
                        using (StreamReader sr = new StreamReader(Form1.executable.GetManifestResourceStream("Pointable.Resources." + file)))//Image.FromFile(file) as Bitmap; //Form1.iconFolderPath + "Pointables_spotlight.png")
                        {
                            xml = sr.ReadToEnd();
                        }
                        XmlSerializer serializer = new XmlSerializer(typeof(PointableObject));
                        PointableObject newPointable = (PointableObject)serializer.Deserialize(new StringReader(xml));
                        //newPointable.keycodeToKeys();

                        if (newPointable.description == "Speakers")
                        {
                            pointableTutorialSpeakerRef = newPointable;
                        }
                        else if (newPointable.description == "Tutorial Pointable")
                        {
                            pointableTutorialNormalRef = newPointable;
                        }
                        else if (newPointable.description == "Tutorial Pointable Last")
                        {
                            pointableTutorialLastRef = newPointable;
                        }
                        // System.Windows.Forms.MessageBox.Show(newPointable.position.X.ToString() + " " + newPointable.position.Y.ToString() + " " + newPointable.position.Z.ToString() + " ");

                    }
                    catch { }
                }
            }


            string[] filesInFolder = new string[] { ""};
            try
            {
                filesInFolder = Directory.GetFiles(pointableFolderPath);
            }
            catch 
            {
                System.Windows.Forms.MessageBox.Show("Unable to load Pointables Folder");
                System.Windows.Forms.Application.Exit();
            }


            List<PointableObject> pointablesPhysical = new List<PointableObject>();
            List<PointableObject> pointablesApplication = new List<PointableObject>();

            foreach (string file in filesInFolder)
            {
                if (Path.GetExtension(file) == ".pnt")
                {
                    try
                    {
                        string xml;
                        using (StreamReader sr = new StreamReader(file))
                        {
                            xml = sr.ReadToEnd();
                        }
                        XmlSerializer serializer = new XmlSerializer(typeof(PointableObject));
                        PointableObject newPointable = (PointableObject)serializer.Deserialize(new StringReader(xml));

                        if (newPointable.description != "Configuration" && newPointable.description != "Speakers")
                        { 
                            //if screen speakers, copy only the calibration/calibrated/isenabled etc   
                            newPointable.keycodeToKeys();

                            //if (pointables.Count < 20) //8
                            if (newPointable.type == PointableObject.PointableType.Physical)
                                pointablesPhysical.Add(newPointable);
                            else
                                pointablesApplication.Add(newPointable);
                        }
                        if (newPointable.description == "Configuration")
                        {
                            pointableScreenRef.calibrated = newPointable.calibrated;
                            pointableScreenRef.isEnabled = newPointable.isEnabled;
                            pointableScreenRef.position.X = newPointable.position.X;
                            pointableScreenRef.position.Y = newPointable.position.Y;
                            pointableScreenRef.position.Z = newPointable.position.Z;
                        }
                        else if (newPointable.description == "Speakers")
                        {
                            pointableSpeakerRef.calibrated = newPointable.calibrated;
                            pointableSpeakerRef.isEnabled = newPointable.isEnabled;
                            pointableSpeakerRef.position.X = newPointable.position.X;
                            pointableSpeakerRef.position.Y = newPointable.position.Y;
                            pointableSpeakerRef.position.Z = newPointable.position.Z;
                        }
                    }
                    catch { }
                }
            }

            try
            {
                //copy to pointables
                foreach (PointableObject pointable in pointablesPhysical)
                {
                    if (pointables.Count < 50)
                        pointables.Add(pointable);
                }
                foreach (PointableObject pointable in pointablesApplication)
                {
                    if (pointables.Count < 50)
                        pointables.Add(pointable);
                }
            }
            catch { }

        }
        private void generatePointableData() //default values fixed here
        {
            loadPointableDataFromFile();

            try
            {
                actionsApplication.Add(MainWindow.actionEmptyApplication());
                actionsKeystrokes.Add(MainWindow.actionEmptyKeystroke());

                //actionsKeystrokes.Add(actionKeyMaximize);
                //actionsKeystrokes.Add(actionKeyMinimize);
                //actionsKeystrokes.Add(actionKeyLeft);
                //actionsKeystrokes.Add(actionKeyRight);

                //actionsKeystrokes.Add(actionKeyLaunchMyComputer);
                //actionsKeystrokes.Add(actionKeyCycleThroughPrograms);
                //// actionsKeystrokes.Add(actionKeyLockComputer);
                //actionsKeystrokes.Add(actionKeyPrint);

                foreach (PointableObject pointable in pointables)
                {
                    if (!pointable.locked)
                    {
                        foreach (Action action in pointable.actions)
                        {
                            if (action.actionType == Action.ActionType.LaunchApplication)
                            {
                                if (action.description != "")
                                    actionsApplication.Add(action);
                            }
                            else if (action.actionType == Action.ActionType.Keystroke)
                            {
                                if (action.description != "")
                                    actionsKeystrokes.Add(action);
                            }
                        }
                    }
                }
            }
            catch { }
        }

        internal PointableObject getPointableofAction(Action actionToFind)
        {
            foreach (PointableObject pointable in pointables)
            {
                foreach (Action action in pointable.actions)
                {
                    if (action == actionToFind)
                    {
                        return pointable;
                    }
                }
            }
            return null;
        }

        private Action actionEmpty()
        {
            return new Action("", "", "", "");
        }
        private Action actionEmptyApplication()
        {
            return new Action("", "", Action.ActionType.LaunchApplication);
        }
        private Action actionEmptyKeystroke()
        {
            return new Action("", "", Action.ActionType.Keystroke);
        }
        PointableObject pointableEmpty = new PointableObject(" ", "", new Action("", "", "", ""), new Action("", "", "", ""), new Action("", "", "", ""), new Action("", "", "", ""));
        //private PointableObject pointableEmpty()
        //{
        //    return new PointableObject(" ", "", actionEmpty(), actionEmpty(), actionEmpty(), actionEmpty());
        //}

        #endregion

        private void buttonSave_Click(object sender, EventArgs e)
        {
            //savePointableDataAll();
        }

        private void toolStripMenuItemAutostart_Click(object sender, EventArgs e)
        {
            try
            {
                if (toolStripMenuItemAutostart.Checked)
                {
                    // Add the value in the registry so that the application runs at startup
                    rkApp.SetValue("Pointable", System.Windows.Forms.Application.ExecutablePath.ToString());
                }
                else
                {
                    // Remove the value from the registry so that the application doesn't start
                    rkApp.DeleteValue("Pointable", false);
                }
            }
            catch { }
        }

        #region tutorial

        internal bool tutorialInProgress = false;
        internal FormTutorial formTutorial;
        FormFullScreen formfullScreen;


        internal bool startTutorial()
        {
            try
            {
                if (controller !=null && !controller.IsConnected)
                {
                    leapNotConnected();
                    return false;
                }else if (!detectionActivated && !tutorialInProgress)
                {
                    detectionActivated = true;
                    pauseResumeTracking();
                    return false;
                }

                if (formTutorial != null && !formTutorial.IsDisposed)
                {
                    try
                    {
                        formTutorial.Close();
                    }
                    catch { }
                }
                if (formfullScreen != null && !formfullScreen.IsDisposed)
                {
                    try
                    {
                        formfullScreen.Close();
                    }
                    catch { }
                }
                formfullScreen = new FormFullScreen(this);
               // formfullScreen.Size = System.Windows.Forms.Screen.AllScreens[Tools.ScreenNumber].Bounds.Size;
                //formfullScreen.Left = System.Windows.Forms.Screen.AllScreens[Tools.ScreenNumber].Bounds.Left;// +System.Windows.Forms.Screen.AllScreens[Tools.ScreenNumber].Bounds.Width / 2;
               // formfullScreen.Top = System.Windows.Forms.Screen.AllScreens[Tools.ScreenNumber].Bounds.Top;// +System.Windows.Forms.Screen.AllScreens[Tools.ScreenNumber].Bounds.Height / 2;
                formfullScreen.Bounds = System.Windows.Forms.Screen.AllScreens[Tools.ScreenNumber].Bounds;
                //formfullScreen.Show();
                formfullScreen.ShowSlow();

                tutorialInProgress = true;

                formTutorial = new FormTutorial(this);
                formTutorial.Left = -formTutorial.Width / 2 + formfullScreen.Width / 2;
                formTutorial.Top = -formTutorial.Height / 2 + formfullScreen.Height / 2;
     

                //formTutorial.Left = -formTutorial.Width / 2 + System.Windows.Forms.Screen.AllScreens[Tools.ScreenNumber].Bounds.X + System.Windows.Forms.Screen.AllScreens[Tools.ScreenNumber].Bounds.Width / 2;
                //formTutorial.Top = -formTutorial.Height / 2 + System.Windows.Forms.Screen.AllScreens[Tools.ScreenNumber].Bounds.Y + System.Windows.Forms.Screen.AllScreens[Tools.ScreenNumber].Bounds.Height / 2;
                //formTutorial.WindowState = FormWindowState.Maximized;
                formTutorial.MdiParent = formfullScreen;
                formTutorial.Show();

                detectionActivated = false;
                BeginInvoke((MethodInvoker)delegate()
                {
                    try
                    {
                        state.ProcessEvent(FiniteStateMachine.Events.EndGesture);
                    }
                    catch { }
                });

            }
            catch { }

            return true;
        }

        private void runClickGame()
        {
            if (startTutorial())
            {
                //formTutorial.Text = "Cursor Control - Click";
                detectionActivated = true;

                formTutorial.runClickGame();
            }
        }

        internal void closeTutorial()
        {
            tutorialInProgress = false;
            try
            {
                formfullScreen.Dispose();
            }
            catch { }

            firstRun = false;
        }
        #endregion 

        #region calibrate

        FormCalibrate formCalibrate;
        PointableObject currentPointableCalibrating;
        
        private void buttonStartCalibration_Click(object sender, EventArgs e)
        {
            startCalibration(pointables[0]);
        }

        internal void endCalibration()
        {
            try
            {
                if (!calibrateScreen)
                {
                    mainWindow.IsEnabled = true;
                }

                if (calibrateScreen)
                {
                    if (formDraw!= null && !formDraw.IsDisposed && formDraw.Visible)
                        formDraw.Hide();

                    saveSettingsData();
                }

                if (tutorialInProgress)
                {
                    formTutorial.Visible = true;
                    formTutorial.tutorialNext();
                }

            }
            catch { }
        }

        internal void startCalibration(PointableObject calibratePointable)
        {
            try
            {
                calibrateScreen = false;

                currentPointableCalibrating = calibratePointable;
                formCalibrate = new FormCalibrate(this, currentPointableCalibrating);
                formCalibrate.Left = -formCalibrate.Width / 2 + System.Windows.Forms.Screen.AllScreens[Tools.ScreenNumber].Bounds.X + System.Windows.Forms.Screen.AllScreens[Tools.ScreenNumber].Bounds.Width / 2;
                formCalibrate.Top = -formCalibrate.Height / 2 + System.Windows.Forms.Screen.AllScreens[Tools.ScreenNumber].Bounds.Y + System.Windows.Forms.Screen.AllScreens[Tools.ScreenNumber].Bounds.Height / 2;

                formCalibrate.Show();
                //disable
               // mainWindow
                mainWindow.IsEnabled = false;
            }
            catch { }
        }

        FormConfiguration formConfiguration;
        internal void openConfiguration()
        {
            try
            {
                if (formConfiguration != null && !formConfiguration.IsDisposed)
                {
                    formConfiguration.Close();
                }

                formConfiguration = new FormConfiguration(this);
                //formCalibrate.Left = -formCalibrate.Width / 2 + System.Windows.Forms.Screen.AllScreens[Tools.ScreenNumber].Bounds.X + System.Windows.Forms.Screen.AllScreens[Tools.ScreenNumber].Bounds.Width / 2;
                //formCalibrate.Top = -formCalibrate.Height / 2 + System.Windows.Forms.Screen.AllScreens[Tools.ScreenNumber].Bounds.Y + System.Windows.Forms.Screen.AllScreens[Tools.ScreenNumber].Bounds.Height / 2;

                formConfiguration.Show();
                formConfiguration.TopMost = true;
            }
            catch { }
        }

        FormCalibrateScreen formCalibrateScreen;
        internal void startCalibrateScreen()
        {
            BeginInvoke((MethodInvoker)delegate()
            {
                startCalibrateScreen(false);
            });
        }
        internal void startCalibrateScreen(bool slow)
        {
            try
            {
                calibrateScreen = true;

                if (formCalibrateScreen != null && !formCalibrateScreen.IsDisposed)
                {
                    formCalibrateScreen.Visible = false;
                    formCalibrateScreen.Left = -formCalibrateScreen.Width / 2 + System.Windows.Forms.Screen.AllScreens[Tools.ScreenNumber].Bounds.X + System.Windows.Forms.Screen.AllScreens[Tools.ScreenNumber].Bounds.Width / 2;
                    formCalibrateScreen.Top = -formCalibrateScreen.Height / 2 + System.Windows.Forms.Screen.AllScreens[Tools.ScreenNumber].Bounds.Y + System.Windows.Forms.Screen.AllScreens[Tools.ScreenNumber].Bounds.Height / 2;

                    formCalibrateScreen.calibrateFirst();
                    formCalibrateScreen.Visible = true;
                }
                else
                {

                    //currentPointableCalibrating = calibratePointable;
                    formCalibrateScreen = new FormCalibrateScreen(this, currentPointableCalibrating);
                    formCalibrateScreen.Left = -formCalibrateScreen.Width / 2 + System.Windows.Forms.Screen.AllScreens[Tools.ScreenNumber].Bounds.X + System.Windows.Forms.Screen.AllScreens[Tools.ScreenNumber].Bounds.Width / 2;
                    formCalibrateScreen.Top = -formCalibrateScreen.Height / 2 + System.Windows.Forms.Screen.AllScreens[Tools.ScreenNumber].Bounds.Y + System.Windows.Forms.Screen.AllScreens[Tools.ScreenNumber].Bounds.Height / 2;

                    if (slow)
                    {
                        //formCalibrateScreen.Opacity = 0;
                        formCalibrateScreen.ShowSlow();
                    }
                    else
                    {
                        formCalibrateScreen.Show();
                    }
                }
                //disable
                // mainWindow
               // mainWindow.IsEnabled = false;
            }
            catch { }
        }
        internal void switchCalibratedScreen()
        {
            try
            {
                if (Tools.ScreenNumber < Screen.AllScreens.Length - 1)
                {
                    Tools.ScreenNumber++;
                }
                else
                {
                    Tools.ScreenNumber = 0;
                }


                if (Tools.ScreenNumber == 0)
                {
                    screen1ToolStripMenuItem.Checked = true;
                    screen2ToolStripMenuItem.Checked = false;
                    screen3ToolStripMenuItem.Checked = false;
                }
                if (Tools.ScreenNumber == 1)
                {
                    screen3ToolStripMenuItem.Checked = false;
                    screen2ToolStripMenuItem.Checked = true;
                    screen1ToolStripMenuItem.Checked = false;
                }
                if (Tools.ScreenNumber == 2)
                {
                    screen3ToolStripMenuItem.Checked = true;
                    screen2ToolStripMenuItem.Checked = false;
                    screen1ToolStripMenuItem.Checked = false;
                }

                if (tutorialInProgress)
                {
                    try
                    {
                          formfullScreen.Bounds = System.Windows.Forms.Screen.AllScreens[Tools.ScreenNumber].Bounds;

                          formTutorial.Left = -formTutorial.Width / 2 + formfullScreen.Width / 2;
                          formTutorial.Top = -formTutorial.Height / 2 + formfullScreen.Height / 2;
                    }
                    catch { }
                    }
                startCalibrateScreen(false);
            }
            catch { }
        }
        internal void doStateEndCalibration()
        {
            try
            {
                if (calibrateScreen)
                {
                    resetSmoothingDirection();

                }
            }
            catch { }
        }

        long calibrationStartTime;
        long calibrationFoundPointTime;
        Vector3 calibrationFoundPointPosition;

        internal void doStateCalibrateScreen()
        {
            calibrationStartTime = DateTime.Now.Ticks;
            calibrationFoundPointTime = 0;
        }

        //attach to window

        internal void startAttachToWindow(PointableObject calibratePointable)
        {
            try
            {
               // currentPointableCalibrating = calibratePointable;
                findWindow = new FormFindWindow(this);//(this, currentPointableCalibrating);
                findWindow.Left = -findWindow.Width / 2 + System.Windows.Forms.Screen.AllScreens[Tools.ScreenNumber].Bounds.X + System.Windows.Forms.Screen.AllScreens[Tools.ScreenNumber].Bounds.Width / 2;
                findWindow.Top = -findWindow.Height / 2 + System.Windows.Forms.Screen.AllScreens[Tools.ScreenNumber].Bounds.Y + System.Windows.Forms.Screen.AllScreens[Tools.ScreenNumber].Bounds.Height / 2;

                mainWindow.Visibility = Visibility.Hidden;
                if (mainWindow.windowPointableEditor !=null)
                    mainWindow.windowPointableEditor.Visibility = Visibility.Hidden;
                findWindow.Show();

            }
            catch { }
        }
        internal void endAttachToWindow()
        {
            try
            {
                mainWindow.Visibility = Visibility.Visible;
                if (mainWindow.windowPointableEditor != null)
                    mainWindow.windowPointableEditor.Visibility = Visibility.Visible;
            }
            catch { }
        }
        internal void updateAttachToWindowData(string windowTitle, string windowClass, IntPtr windowHandle)
        {
            try
            {
                if (mainWindow.windowPointableEditor != null)
                    mainWindow.windowPointableEditor.updateAttachToWindowData(windowTitle, windowClass, windowHandle);

            }
            catch { }
        }
        #endregion


        #region first run


        private void checkDate()
        {
            try
            {
                long currentTime = DateTime.Now.Ticks;
                DateTime value = new DateTime(2016, 4, 1);
                long endTime = value.Ticks;

                if (currentTime > endTime)
                {
                    System.Windows.Forms.MessageBox.Show("New version is available.\nPlease update Pointable from Airspace Home.", "Pointable", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

                    //try
                    //{
                    //    //start help
                    //    ProcessStartInfo startInfo = new ProcessStartInfo();
                    //    startInfo.FileName = "http://www.pointable.net"; //C:\\Program Files (x86)\\EventGhost\\EventGhost.exe";
                    //    startInfo.Arguments = "";// "-event NextChannel";
                    //    Process.Start(startInfo);

                    //}
                    //catch { }
                    
                    System.Windows.Forms.Application.Exit();
                }
            }
            catch {}//(Exception ex) { Debug.WriteLine(ex.ToString()); }
        }

        
        internal bool firstRun = false;
        int currentVersionMajor;
        int currentVersionMinor;
        int currentVersionBuild;

        int previousVersionMajor;
        int previousVersionMinor;
        int previousVersionBuild;

        private void checkVersion()
        {
            if (previousVersionMajor == 1 && previousVersionMinor == 2 && previousVersionBuild == 3)
            { //before 1.3
                string[] filesInFolder = new string[] { "" };
                try
                {
                    filesInFolder = Directory.GetFiles(pointableFolderPathDefault);
                }
                catch
                {
                    System.Windows.Forms.MessageBox.Show("Unable to load Pointables Folder");
                    System.Windows.Forms.Application.Exit();
                }

                foreach (string file in filesInFolder)
                {
                    if (Path.GetExtension(file) == ".pnt")
                    {
                        string filename = Path.GetFileNameWithoutExtension(file);

                        if (filename == "Power Pointable")
                        {
                            try
                            {
                                string destinationFile = pointableFolderPath + Path.GetFileName(file);
                                //if (!File.Exists(destinationFile))
                                File.Copy(file, destinationFile, true);
                            }
                            catch { }
                        }
                    }
                }

                try
                {
                    filesInFolder = Directory.GetFiles(iconFolderPathDefault);
                }
                catch
                {
                    System.Windows.Forms.MessageBox.Show("Unable to load Icons Folder");
                    System.Windows.Forms.Application.Exit();
                }

                foreach (string file in filesInFolder)
                {
                    if (Path.GetExtension(file) == ".png")
                    {
                        try
                        {
                            string destinationFile = iconFolderPath + Path.GetFileName(file);
                            if (!File.Exists(destinationFile))
                                File.Copy(file, destinationFile, false);
                        }
                        catch { }
                    }
                }
            }
            else if (currentVersionMajor == previousVersionMajor && currentVersionMinor == previousVersionMinor && currentVersionBuild == previousVersionBuild)
            {//same version

            }
            else
            {//different version
                //copy fixed pointable data;
                //version 0.9.x to 0.9.11
                firstRun = true;

                string[] filesInFolder = new string[] { "" };
                try
                {
                    filesInFolder = Directory.GetFiles(pointableFolderPathDefault);
                }
                catch
                {
                    System.Windows.Forms.MessageBox.Show("Unable to load Pointables Folder");
                    System.Windows.Forms.Application.Exit();
                }

                foreach (string file in filesInFolder)
                {
                    if (Path.GetExtension(file) == ".pnt")
                    {
                        string filename = Path.GetFileNameWithoutExtension(file);

                        if (filename == "Default" || filename == "Desktop" || filename == "Start Screen" || filename == "Start Screen App" || filename == "Start Screen App - Store" || filename == "Browser" || filename == "Power Pointable")
                        {
                            try
                            {
                                string destinationFile = pointableFolderPath + Path.GetFileName(file);
                                //if (!File.Exists(destinationFile))
                                    File.Copy(file, destinationFile, true);
                            }
                            catch { }
                        }
                    }
                }

                try
                {
                    filesInFolder = Directory.GetFiles(iconFolderPathDefault);
                }
                catch
                {
                    System.Windows.Forms.MessageBox.Show("Unable to load Icons Folder");
                    System.Windows.Forms.Application.Exit();
                }

                foreach (string file in filesInFolder)
                {
                    if (Path.GetExtension(file) == ".png")
                    {
                        try
                        {
                            string destinationFile = iconFolderPath + Path.GetFileName(file);
                            if (!File.Exists(destinationFile))
                                File.Copy(file, destinationFile, false);
                        }
                        catch { }
                    }
                }

            }
        }

        private void checkFirstRun()
        {
            try
            {
                Version version = Assembly.GetEntryAssembly().GetName().Version;
                currentVersionMajor = version.Major;
                currentVersionMinor = version.Minor;
                currentVersionBuild = version.Build;
            }
            catch { }

            try
            {

                checkAndInitializePointableFolders();
                checkAndInitializeCursors();

                if (!File.Exists(settingsFolderPath + settingsFile))
                {
                    firstRun = true;
                    
                    //copyPointables
                    initializePointableFiles();
                    // The value exists, the application is set to run at startup   
                    //try
                    //{
                    //    rkApp.SetValue("Pointable", System.Windows.Forms.Application.ExecutablePath.ToString());
                    //    toolStripMenuItemAutostart.Checked = true;
                    //}
                    //catch { }
                }
                else
                {
                    try
                    {
                        // Check to see the current state (running at startup or not)
                        if (rkApp.GetValue("Pointable") == null)
                        {
                            // The value doesn't exist, the application is not set to run at startup
                            toolStripMenuItemAutostart.Checked = false;
                        }
                        else
                        {
                            // The value exists, the application is set to run at startup
                            toolStripMenuItemAutostart.Checked = true;
                        }
                    }
                    catch { }
                }
            }
            catch { }
        }

        private void checkAndInitializePointableFolders()
        {
            try
            {
                if (!System.IO.Directory.Exists(iconFolderPath))
                {
                    Directory.CreateDirectory(iconFolderPath);
                }
            }
            catch { }

            try
            {
                if (!System.IO.Directory.Exists(pointableFolderPath))
                {
                    Directory.CreateDirectory(pointableFolderPath);
                }
            }
            catch { }
        }

        private void checkAndInitializeCursors()
        {
            try
            {
                string myCursorFile = Form1.settingsFolderPath + @"cursor_windows.cur";
                string myCursorFile2 = Form1.settingsFolderPath + @"cursor_windows_point.cur";

                try
                {
                    if (!File.Exists(myCursorFile))
                    {
                        File.WriteAllBytes(myCursorFile, Resources.cursor_windows);
                    }
                }
                catch { }
                try
                {
                    if (!File.Exists(myCursorFile2))
                    {
                        File.WriteAllBytes(myCursorFile2, Resources.cursor_windows_point);
                    }
                }
                catch { }
            }
            catch { }
        }

        private void initializePointableFiles()
        {
            string[] filesInFolder = new string[] { "" };
            try
            {
                filesInFolder = Directory.GetFiles(pointableFolderPathDefault);
            }
            catch
            {
                System.Windows.Forms.MessageBox.Show("Unable to load Pointables Folder");
                System.Windows.Forms.Application.Exit();
            }

            foreach (string file in filesInFolder)
            {
                if (Path.GetExtension(file) == ".pnt")
                {
                    try
                    {
                        string destinationFile = pointableFolderPath + Path.GetFileName(file);
                        if (!File.Exists(destinationFile))
                            File.Copy(file, destinationFile, false);
                    }
                    catch { }
                }
            }

            try
            {
                filesInFolder = Directory.GetFiles(iconFolderPathDefault);
            }
            catch
            {
                System.Windows.Forms.MessageBox.Show("Unable to load Icons Folder");
                System.Windows.Forms.Application.Exit();
            }

            foreach (string file in filesInFolder)
            {
                if (Path.GetExtension(file) == ".png")
                {
                    try
                    {
                        string destinationFile = iconFolderPath + Path.GetFileName(file);
                        if (!File.Exists(destinationFile))
                            File.Copy(file, destinationFile, false);
                    }
                    catch { }
                }
            }
        }

        #endregion

        private void getStartedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                //start help
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = "http://www.pointable.net/getstarted"; //C:\\Program Files (x86)\\EventGhost\\EventGhost.exe";
                startInfo.Arguments = "";// "-event NextChannel";
                Process.Start(startInfo);
            }
            catch { }
        }

        FormFindWindow findWindow;
        private void buttonFindWindow_Click(object sender, EventArgs e)
        {
            findWindow = new FormFindWindow(this);
            findWindow.Show();
        }

        FormDraw formDraw;

        private void button1_Click(object sender, EventArgs e)
        {
            formDraw = new FormDraw();
            formDraw.Show();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            smoothingMode = checkBox1.Checked;
        }

        #region mouse click 

        
        Point cursorHoverFirstCoordinate;
        long cursorHoverStartTime;
        long cursorPreviousClickTime =0;

        bool cursorWithinArea = false;
        bool cursorControl = true;
        Point cursorPreviousClick;


        private void checkMouseClick(Point cursorPosition)
        {
            try
            {
                if (!cursorControl) return;

                if (controlKeyDown) return;

                long durationPrevious = (long)((DateTime.Now.Ticks - cursorPreviousClickTime) / 10000);
                if (durationPrevious < dropletDurationBetween) //1.2 400
                    return;

                //          if (Math.Sqrt(Math.Pow(cursorPosition.X - cursorPreviousClick.X, 2) + Math.Pow(cursorPosition.Y - cursorPreviousClick.Y, 2)) < 10)
                //              return;                     
                double distancePoint = Math.Sqrt(Math.Pow(cursorPosition.X - cursorHoverFirstCoordinate.X, 2) + Math.Pow(cursorPosition.Y - cursorHoverFirstCoordinate.Y, 2));


                if (fingertipVelocity.Magnitude < dropletMaxVelocity && distancePoint < 40) //1.2.1 100
                {
                    if (cursorHoverStartTime == 0)
                    {
                        cursorHoverStartTime = DateTime.Now.Ticks;
                        //formControl.startCursorProgress();
                        return;
                    }

                    long durationMS = (long)((DateTime.Now.Ticks - cursorHoverStartTime) / 10000);

                    if (durationMS > dropletMinDuration) //1.2.1 300
                    {//exceed 1.2 seconds //convert to right click
                        Rectangle bottomCorner = new Rectangle(Screen.PrimaryScreen.Bounds.Right - 40, Screen.PrimaryScreen.Bounds.Bottom - 40, 41, 41);

                        if (!bottomCorner.Contains(cursorPosition))
                        {
                            cursorHoverFirstCoordinate = new System.Drawing.Point((int)cursorPosition.X, (int)cursorPosition.Y);
                            cursorPreviousClickTime = DateTime.Now.Ticks;
                            cursorHoverStartTime = 0;



                            cursorPreviousClick = new Point(cursorPosition.X, cursorPosition.Y);
                            formControl.drawWindowCursorDropAbsolute(cursorPosition.X, cursorPosition.Y);
                            //triggerMouseMove(cursorPosition.X, cursorPosition.Y); //dunno what bug. youtube fullscreen cannot click if this activated
                           //if (!formControl.windowCursorDropFormClick.Visible)

                            triggerMouseMove(cursorPosition.X, cursorPosition.Y);
                        }
                        //clear draw                    
                    }
                    else if (durationMS > 300)
                    { //timer > certain value and null or exist but disposed
                        //draw rightclick
                    }

                }
                else
                {//outside of hover area
                    cursorWithinArea = false;

                    //cancel draw
                    //formControl.endCursorProgress();
                    //if (formControl.windowCursorDropForm.Visible)
                    //   formControl.windowCursorDropForm.Hide();


                    cursorHoverFirstCoordinate = new System.Drawing.Point((int)cursorPosition.X, (int)cursorPosition.Y);
                    cursorHoverStartTime = 0;// DateTime.Now.Ticks;
                }
            }
            catch { }
        }

        public void triggerMouseMove(double x, double y)
        {
            try
            {
                //if (clickTriggerInProgress) return;
                if (x < 0)
                    x = x - 1;
                else
                    x = x + 1;

                Tools.INPUT[] buffer = new Tools.INPUT[1];
                buffer[0].type = Tools.INPUT_MOUSE;
                buffer[0].mi.dx = (int)(x * 65535.0f / System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width);
                buffer[0].mi.dy = (int)(y * 65535.0f / System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height);
                buffer[0].mi.mouseData = 0;
                buffer[0].mi.dwFlags = Tools.MOUSEEVENTF_ABSOLUTE | Tools.MOUSEEVENTF_MOVE;
                buffer[0].mi.time = 0;
                buffer[0].mi.dwExtraInfo = (IntPtr)0;
                SendInput(1, buffer, Marshal.SizeOf(buffer[0]));
            }
            catch { }
        }
        private void triggerMouseMove(Point position)
        {
            try
            {
                //if (clickTriggerInProgress) return;

                if (position.X < 0)
                    position.X--;
                else
                    position.X++;

                Tools.INPUT[] buffer = new Tools.INPUT[1];
                buffer[0].type = Tools.INPUT_MOUSE;
                buffer[0].mi.dx = (int)(position.X * 65535.0f / System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width);
                buffer[0].mi.dy = (int)(position.Y * 65535.0f / System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height);
                buffer[0].mi.mouseData = 0;
                buffer[0].mi.dwFlags = Tools.MOUSEEVENTF_ABSOLUTE | Tools.MOUSEEVENTF_MOVE;
                buffer[0].mi.time = 0;
                buffer[0].mi.dwExtraInfo = (IntPtr)0;
                SendInput(1, buffer, Marshal.SizeOf(buffer[0]));

            }
            catch { }
        }

        private void triggerMouseMoveAndClick(Point position, int type)
        {
            try
            {
               // position.X = position.X + 1;
                if (position.X < 0)
                    position.X--;
                else
                    position.X++;

                if (SystemInformation.MouseButtonsSwapped)
                {
                    if (type == 0)
                        type = 1;
                    else if (type == 1)
                        type = 0;
                }

                if (type == 0)
                {
                    try
                    {
                        triggerMouseMove(position);
                        Thread.Sleep(25);
                    }
                    catch { }
                }


                int buttonDown = 0;
                int buttonUp = 0;
                if (type == 0)
                {
                    buttonDown = Tools.MOUSEEVENTF_LEFTDOWN;
                    buttonUp= Tools.MOUSEEVENTF_LEFTUP;
                }
                else if (type == 1)
                {
                    buttonDown = Tools.MOUSEEVENTF_RIGHTDOWN;
                    buttonUp = Tools.MOUSEEVENTF_RIGHTUP;
                }
                else if (type == 2)
                {
                    buttonDown = Tools.MOUSEEVENTF_MIDDLEDOWN;
                    buttonUp = Tools.MOUSEEVENTF_MIDDLEUP;
                }

                Tools.INPUT[] buffer = new Tools.INPUT[1];
                buffer[0].type = Tools.INPUT_MOUSE;
                buffer[0].mi.dx = (int)(position.X * 65535.0f / System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width); //AllScreens[0]
                buffer[0].mi.dy = (int)(position.Y * 65535.0f / System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height);
                buffer[0].mi.mouseData = 0;
                buffer[0].mi.dwFlags = Tools.MOUSEEVENTF_ABSOLUTE | Tools.MOUSEEVENTF_MOVE;
                buffer[0].mi.time = 0;
                buffer[0].mi.dwExtraInfo = (IntPtr)0;
                SendInput(1, buffer, Marshal.SizeOf(buffer[0]));

                buffer = new Tools.INPUT[1];
                buffer[0].type = Tools.INPUT_MOUSE;
                buffer[0].mi.dx = 0;
                buffer[0].mi.dy = 0;
                buffer[0].mi.mouseData = 0;
                buffer[0].mi.dwFlags = (uint)buttonDown;
                buffer[0].mi.time = 0;
                buffer[0].mi.dwExtraInfo = (IntPtr)0;
                SendInput(1, buffer, Marshal.SizeOf(buffer[0]));

                buffer = new Tools.INPUT[1];
                buffer[0].type = Tools.INPUT_MOUSE;
                buffer[0].mi.dx = 0;
                buffer[0].mi.dy = 0;
                buffer[0].mi.mouseData = 0;
                buffer[0].mi.dwFlags = (uint)buttonUp;
                buffer[0].mi.time = 0;
                buffer[0].mi.dwExtraInfo = (IntPtr)0;
                SendInput(1, buffer, Marshal.SizeOf(buffer[0]));

            }
            catch { }
        }

        private void triggerMouseMoveAndClickDownOnly(Point position, int type)
        {
            try
            {

                if (position.X < 0)
                    position.X--;
                else
                    position.X++;

                if (SystemInformation.MouseButtonsSwapped)
                {
                    if (type == 0)
                        type = 1;
                    else if (type == 1)
                        type = 0;
                }

                int buttonDown = 0;
                int buttonUp = 0;
                if (type == 0)
                {
                    buttonDown = Tools.MOUSEEVENTF_LEFTDOWN;
                    buttonUp = Tools.MOUSEEVENTF_LEFTUP;
                }
                else if (type == 1)
                {
                    buttonDown = Tools.MOUSEEVENTF_RIGHTDOWN;
                    buttonUp = Tools.MOUSEEVENTF_RIGHTUP;
                }
                else if (type == 2)
                {
                    buttonDown = Tools.MOUSEEVENTF_MIDDLEDOWN;
                    buttonUp = Tools.MOUSEEVENTF_MIDDLEUP;
                }

                Tools.INPUT[] buffer = new Tools.INPUT[1];
                buffer[0].type = Tools.INPUT_MOUSE;
                buffer[0].mi.dx = (int)(position.X * 65535.0f / System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width); //AllScreens[0]
                buffer[0].mi.dy = (int)(position.Y * 65535.0f / System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height);
                buffer[0].mi.mouseData = 0;
                buffer[0].mi.dwFlags = Tools.MOUSEEVENTF_ABSOLUTE | Tools.MOUSEEVENTF_MOVE;
                buffer[0].mi.time = 0;
                buffer[0].mi.dwExtraInfo = (IntPtr)0;
                SendInput(1, buffer, Marshal.SizeOf(buffer[0]));

                buffer = new Tools.INPUT[1];
                buffer[0].type = Tools.INPUT_MOUSE;
                buffer[0].mi.dx = 0;
                buffer[0].mi.dy = 0;
                buffer[0].mi.mouseData = 0;
                buffer[0].mi.dwFlags = (uint)buttonDown;
                buffer[0].mi.time = 0;
                buffer[0].mi.dwExtraInfo = (IntPtr)0;
                SendInput(1, buffer, Marshal.SizeOf(buffer[0]));

                //buffer = new Tools.INPUT[1];
                //buffer[0].type = Tools.INPUT_MOUSE;
                //buffer[0].mi.dx = 0;
                //buffer[0].mi.dy = 0;
                //buffer[0].mi.mouseData = 0;
                //buffer[0].mi.dwFlags = (uint)buttonDown;
                //buffer[0].mi.time = 100;
                //buffer[0].mi.dwExtraInfo = (IntPtr)0;
                //SendInput(1, buffer, Marshal.SizeOf(buffer[0]));



               // Console.WriteLine("mouse down :" + position.ToString());
                //Thread.Sleep(20);

                //buffer = new Tools.INPUT[1];
                //buffer[0].type = Tools.INPUT_MOUSE;
                //buffer[0].mi.dx = 0;
                //buffer[0].mi.dy = 0;
                //buffer[0].mi.mouseData = 0;
                //buffer[0].mi.dwFlags = (uint)buttonUp;
                //buffer[0].mi.time = 0;
                //buffer[0].mi.dwExtraInfo = (IntPtr)0;
                //SendInput(1, buffer, Marshal.SizeOf(buffer[0]));

            }
            catch { }
        }

        private void triggerMouseMoveAndClickUpOnly(Point position, int type)
        {
            try
            {

                if (position.X < 0)
                    position.X--;
                else
                    position.X++;

                if (SystemInformation.MouseButtonsSwapped)
                {
                    if (type == 0)
                        type = 1;
                    else if (type == 1)
                        type = 0;
                }

                int buttonDown = 0;
                int buttonUp = 0;
                if (type == 0)
                {
                    buttonDown = Tools.MOUSEEVENTF_LEFTDOWN;
                    buttonUp = Tools.MOUSEEVENTF_LEFTUP;
                }
                else if (type == 1)
                {
                    buttonDown = Tools.MOUSEEVENTF_RIGHTDOWN;
                    buttonUp = Tools.MOUSEEVENTF_RIGHTUP;
                }
                else if (type == 2)
                {
                    buttonDown = Tools.MOUSEEVENTF_MIDDLEDOWN;
                    buttonUp = Tools.MOUSEEVENTF_MIDDLEUP;
                }

                Tools.INPUT[] buffer = new Tools.INPUT[1];
                buffer[0].type = Tools.INPUT_MOUSE;
                buffer[0].mi.dx = (int)(position.X * 65535.0f / System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width); //AllScreens[0]
                buffer[0].mi.dy = (int)(position.Y * 65535.0f / System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height);
                buffer[0].mi.mouseData = 0;
                buffer[0].mi.dwFlags = Tools.MOUSEEVENTF_ABSOLUTE | Tools.MOUSEEVENTF_MOVE;
                buffer[0].mi.time = 0;
                buffer[0].mi.dwExtraInfo = (IntPtr)0;
                SendInput(1, buffer, Marshal.SizeOf(buffer[0]));

                //buffer = new Tools.INPUT[1];
                //buffer[0].type = Tools.INPUT_MOUSE;
                //buffer[0].mi.dx = 0;
                //buffer[0].mi.dy = 0;
                //buffer[0].mi.mouseData = 0;
                //buffer[0].mi.dwFlags = (uint)buttonDown;
                //buffer[0].mi.time = 0;
                //buffer[0].mi.dwExtraInfo = (IntPtr)0;
                //SendInput(1, buffer, Marshal.SizeOf(buffer[0]));

                buffer = new Tools.INPUT[1];
                buffer[0].type = Tools.INPUT_MOUSE;
                buffer[0].mi.dx = 0;
                buffer[0].mi.dy = 0;
                buffer[0].mi.mouseData = 100;
                buffer[0].mi.dwFlags = (uint)buttonUp;
                buffer[0].mi.time = 0;
                buffer[0].mi.dwExtraInfo = (IntPtr)0;
                SendInput(1, buffer, Marshal.SizeOf(buffer[0]));

                //buffer = new Tools.INPUT[1];
                //buffer[0].type = Tools.INPUT_MOUSE;
                //buffer[0].mi.dx = 0;
                //buffer[0].mi.dy = 0;
                //buffer[0].mi.mouseData = 100;
                //buffer[0].mi.dwFlags = (uint)buttonUp;
                //buffer[0].mi.time = 0;
                //buffer[0].mi.dwExtraInfo = (IntPtr)0;
                //SendInput(1, buffer, Marshal.SizeOf(buffer[0]));

              //  Console.WriteLine("mouse up: " + position.ToString());
            }
            catch { }
        }


        IntPtr handleWindowToClose;
        internal bool isOwnApplicationFocus = false;


        private void closeWindowApplication()
        {
            try
            {
                Tools.CloseWindow(handleWindowToClose);
            }
            catch { }
        }

        #endregion

        private void trackBar1_Scroll(object sender, EventArgs e)
        {

        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
        }

        private void toolStripMenuItemScreenHeight_Click(object sender, EventArgs e)
        {
            try
            {
                //TrackBar tb = (TrackBar)toolStripMenuItemScreenHeight.Control;
                //screenHeightValueToolStripMenuItem.Text = tb.Value + " cm";

                //screenHeightUser = tb.Value * 10;
                //screenVerticalOffset = screenHeightUser - screenHeightDefault;

                //initializeScreenCorners();
            }
            catch { }
        }

        private void toolStripMenuItemScreenHeight_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                //TrackBar tb = (TrackBar)toolStripMenuItemScreenHeight.Control;
                //screenHeightValueToolStripMenuItem.Text = tb.Value + " cm";

                //screenHeightUser = tb.Value * 10;
                //screenVerticalOffset = screenHeightUser - screenHeightDefault;

                //initializeScreenCorners();
            }
            catch { }
        }

        private void screenCalibrationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            startCalibrateScreen(false);
        }

        internal Bitmap getHandleIcon(IntPtr handle)
        {
            //if (handle != IntPtr.Zero && handle == handleBitmapHandle && handleBitmap != null)
            //{
            //    Bitmap windowIcon2 = new Bitmap(handleBitmap.Width, handleBitmap.Height);
            //    Graphics g = Graphics.FromImage(windowIcon2);

            //    //g.CompositingMode = CompositingMode.SourceOver;

            //    //g.DrawImage(bitmapSpotlight, 0, 0, bitmapSpotlight.Width, bitmapSpotlight.Height);
            //    g.DrawImage(handleBitmap,0,0);
            //    g.Dispose();

            //    return windowIcon2;
            //}

            Bitmap windowIcon = Tools.GetBitmapIcon(handle);

            if (windowIcon == null || windowIcon.Height < 64)
            {//get process and exe
                // int activeProcId;
                // Tools.GetWindowThreadProcessId(handle, out activeProcId);
                string executablePath = Tools.GetProcessPath(handle);

                try
                {
                    mMultiIcon.SelectedIndex = -1;
                    mMultiIcon.Load(executablePath);

                    int biggestIconWidth = 0;
                    IconImage biggestIcon = null;


                    for (int i = 0; i < mMultiIcon.Count; i++)
                    {
                        for (int j = 0; j < mMultiIcon[i].Count; j++)
                        {
                            if (mMultiIcon[i][j].Icon.Size.Width > biggestIconWidth)
                            {
                                biggestIconWidth = mMultiIcon[i][j].Icon.Size.Width;
                                biggestIcon = mMultiIcon[i][j];
                            }
                        }
                    }
                    if (biggestIcon != null)
                    {
                        windowIcon = biggestIcon.Icon.ToBitmap();
                    }
                }
                catch { }
            }

            //if (handleBitmap != null)
            //{
            //    handleBitmap.Dispose();
            //    handleBitmap = null;
            //}
            //if (windowIcon != null && windowIcon.Height >= 16)
            //{
            //    handleBitmap = windowIcon;
            //    handleBitmapHandle = handle;
            //}

            return windowIcon;
        }

       // bool modeClickAndDrag = false;
        private void clickanddragToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (clickanddragToolStripMenuItem.Checked)
            {
                Tools.modeClickAndDrag = true;
            }
            else
            {
                Tools.modeClickAndDrag = false;
            }
        }

        private void controlAltShiftEnableDisablePointableTrackingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (controlAltShiftEnableDisablePointableTrackingToolStripMenuItem.Checked)
            {
                Tools.keyModifierModeEnableTracking = true;
            }
            else
            {
                Tools.keyModifierModeEnableTracking = false;
            }
        }

        private void controlLeftClickToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (controlLeftClickToolStripMenuItem.Checked)
            {
                Tools.keyModifierModeControl = true;
            }
            else
            {
                Tools.keyModifierModeControl = false;
            }
        }

        private void altRightClickToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (altRightClickToolStripMenuItem.Checked)
            {
                Tools.keyModifierModeAlt = true;
            }
            else
            {
                Tools.keyModifierModeAlt = false;
            }
        }

        private void shiftMiddleClickToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (shiftMiddleClickToolStripMenuItem.Checked)
            {
                Tools.keyModifierModeShift = true;
            }
            else
            {
                Tools.keyModifierModeShift = false;
            }
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void tutorialToolStripMenuItem_Click(object sender, EventArgs e)
        {
            startTutorial();
        }

        private void clickGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            runClickGame();
        }

        private void interactionConfigurationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openConfiguration();
        }


    }

    [ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.MenuStrip |
                                   ToolStripItemDesignerAvailability.ContextMenuStrip)]
    public class TrackBarMenuItem : ToolStripControlHost
    {
        private TrackBar trackBar;

        public TrackBarMenuItem()
            : base(new TrackBar())
        {
            this.trackBar = this.Control as TrackBar;
        }
    }
}
