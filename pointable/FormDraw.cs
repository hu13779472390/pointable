using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace LeapProject
{
    partial class FormDraw : Form
    {

        protected override bool ShowWithoutActivation
        {
            get { return true; }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams baseParams = base.CreateParams;

                baseParams.ExStyle |= (int)(
                  ExtendedWindowStyles.WS_EX_NOACTIVATE |
                  ExtendedWindowStyles.WS_EX_TOOLWINDOW);

                return baseParams;
            }
        }

        public FormDraw()
        {
            InitializeComponent();
        }

        internal void SetBoundsFade(int left, int top, int width, int height)
        {
            try
            {
                this.Left = left;
                this.Top = top;
                this.Width = width;
                this.Height = height;


                this.BackColor = Color.Lime;
                this.TransparencyKey = System.Drawing.SystemColors.Control;
                this.Opacity = 1;

                Tools.SetWindowPos(this.Handle, Tools.HWND.TOPMOST, 0, 0, 0, 0, Tools.SetWindowPosFlags.IgnoreMove | Tools.SetWindowPosFlags.IgnoreResize | Tools.SetWindowPosFlags.DoNotActivate);

                timer1.Start();
            }
            catch { }
        }

        internal void SetBoundsFixed(int left, int top, int width, int height)
        {
            try
            {
                this.Left = left;
                this.Top = top;
                this.Width = width;
                this.Height = height;

                this.Opacity = 0.6;
                this.BackColor = Color.Lime;
                this.TransparencyKey = Color.Black;

                Tools.SetWindowPos(this.Handle, Tools.HWND.TOPMOST, 0, 0, 0, 0, Tools.SetWindowPosFlags.IgnoreMove | Tools.SetWindowPosFlags.IgnoreResize | Tools.SetWindowPosFlags.DoNotActivate);

                timer1.Stop();
            }
            catch { }
        }

        int count = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if (count++ < 10) return;

                double opacity = this.Opacity;
                opacity -= 0.05;
                if (opacity < 0.3)
                {
                    opacity = 0.3;
                    timer1.Stop();
                }
                this.Opacity = opacity;
            }
            catch { }
        }
    }

    public static class ExtendedWindowStyles
    {

        public static readonly Int32

        WS_EX_ACCEPTFILES = 0x00000010,

        WS_EX_APPWINDOW = 0x00040000,

        WS_EX_CLIENTEDGE = 0x00000200,

        WS_EX_COMPOSITED = 0x02000000,

        WS_EX_CONTEXTHELP = 0x00000400,

        WS_EX_CONTROLPARENT = 0x00010000,

        WS_EX_DLGMODALFRAME = 0x00000001,

        WS_EX_LAYERED = 0x00080000,

        WS_EX_LAYOUTRTL = 0x00400000,

        WS_EX_LEFT = 0x00000000,

        WS_EX_LEFTSCROLLBAR = 0x00004000,

        WS_EX_LTRREADING = 0x00000000,

        WS_EX_MDICHILD = 0x00000040,

        WS_EX_NOACTIVATE = 0x08000000,

        WS_EX_NOINHERITLAYOUT = 0x00100000,

        WS_EX_NOPARENTNOTIFY = 0x00000004,

        WS_EX_OVERLAPPEDWINDOW = WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE,

        WS_EX_PALETTEWINDOW = WS_EX_WINDOWEDGE | WS_EX_TOOLWINDOW | WS_EX_TOPMOST,

        WS_EX_RIGHT = 0x00001000,

        WS_EX_RIGHTSCROLLBAR = 0x00000000,

        WS_EX_RTLREADING = 0x00002000,

        WS_EX_STATICEDGE = 0x00020000,

        WS_EX_TOOLWINDOW = 0x00000080,

        WS_EX_TOPMOST = 0x00000008,

        WS_EX_TRANSPARENT = 0x00000020,

        WS_EX_WINDOWEDGE = 0x00000100;

    }
}
