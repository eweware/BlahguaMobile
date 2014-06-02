using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using System.Drawing;

namespace TestApp
{
	[Register("CreatePollPostView")]
	public partial class CreatePollPostView : UIView
	{
		public CreatePollPostView(IntPtr h): base(h)
		{
		}

		public CreatePollPostView ()
		{
			var arr = NSBundle.MainBundle.LoadNib("CreatePollPostView", this, null);
			var v = Runtime.GetNSObject(arr.ValueAt(0)) as UIView;
			v.Frame = new RectangleF(0, 0, Frame.Width, Frame.Height);
			AddSubview(v);
		}

	}
}
