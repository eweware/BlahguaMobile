using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using System.Drawing;

namespace TestApp
{
	[Register("StreamRollPostView")]
	public partial class StreamRollPostView : UIView
	{
		public StreamRollPostView(IntPtr h): base(h)
		{
		}

		public StreamRollPostView ()
		{
			var arr = NSBundle.MainBundle.LoadNib("StreamRollPostView", this, null);
			var v = Runtime.GetNSObject(arr.ValueAt(0)) as UIView;
			v.Frame = new RectangleF(0, 0, Frame.Width, Frame.Height);
			AddSubview(v);
		}

	}
}