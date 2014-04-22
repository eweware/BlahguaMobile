using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using System.Drawing;

namespace TestApp
{
	[Register("BadgeProfileView")]
	public partial class BadgeProfileView : UIView
	{
		public BadgeProfileView(IntPtr h): base(h)
		{
		}

		public BadgeProfileView ()
		{
			var arr = NSBundle.MainBundle.LoadNib("BadgeProfileView", this, null);
			var v = Runtime.GetNSObject(arr.ValueAt(0)) as UIView;
			v.Frame = new RectangleF(0, 0, Frame.Width, Frame.Height);
			AddSubview(v);
		}

	}
}