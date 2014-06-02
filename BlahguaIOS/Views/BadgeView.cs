using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using System.Drawing;

namespace TestApp
{
	[Register("BadgeView")]
	public partial class BadgeView : UIView
	{
		public BadgeView(IntPtr h): base(h)
		{
		}

		public BadgeView ()
		{
			var arr = NSBundle.MainBundle.LoadNib("BadgeView", this, null);
			var v = Runtime.GetNSObject(arr.ValueAt(0)) as UIView;
			v.Frame = new RectangleF(0, 0, Frame.Width, Frame.Height);
			AddSubview(v);
		}

	}
}