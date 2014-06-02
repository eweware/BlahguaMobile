using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using System.Drawing;

namespace TestApp
{
	[Register("CreatePostView")]
	public partial class CreatePostView : UIView
	{
		public CreatePostView(IntPtr h): base(h)
		{
		}

		public CreatePostView ()
		{
			var arr = NSBundle.MainBundle.LoadNib("CreatePostView", this, null);
			var v = Runtime.GetNSObject(arr.ValueAt(0)) as UIView;
			v.Frame = new RectangleF(0, 0, Frame.Width, Frame.Height);
			AddSubview(v);
		}

	}
}
