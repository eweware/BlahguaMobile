using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using System.Drawing;

namespace TestApp
{
	[Register("CreatePredictPostView")]
	public partial class CreatePredictPostView : UIView
	{
		public CreatePredictPostView(IntPtr h): base(h)
		{
		}

		public CreatePredictPostView ()
		{
			var arr = NSBundle.MainBundle.LoadNib("CreatePredictPostView", this, null);
			var v = Runtime.GetNSObject(arr.ValueAt(0)) as UIView;
			v.Frame = new RectangleF(0, 0, Frame.Width, Frame.Height);
			AddSubview(v);
		}

	}
}

