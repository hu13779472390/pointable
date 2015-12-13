using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;


namespace ApiWrapper
{
	public class Gdi
	{
		public enum RopMode : int
		{
			R2_NOT = 6
		}
		[DllImport("gdi32.dll")]
		public static extern int SetROP2(IntPtr hdc, int fnDrawMode);

		public enum PenStyles : int
		{
			PS_INSIDEFRAME = 6
		}
		[DllImport("gdi32.dll")]
		public static extern IntPtr CreatePen(int fnPenStyle, int nWidth, uint crColor);

		[DllImport("gdi32.dll")]
		public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

		public enum StockObjects : int
		{
			NULL_BRUSH = 5
		}
		[DllImport("gdi32.dll")]
		public static extern IntPtr GetStockObject(int fnObject);

		[DllImport("gdi32.dll")]
		public static extern uint Rectangle(IntPtr hdc, int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);

		[DllImport("gdi32.dll")]
		public static extern bool DeleteObject(IntPtr hObject);

		public Gdi()
		{
		}
	}
}
