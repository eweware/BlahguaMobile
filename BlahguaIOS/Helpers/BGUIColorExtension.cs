using System;
using System.Drawing;

using MonoTouch.UIKit;
using MonoTouch.Foundation;
using MonoTouch.CoreGraphics;

namespace BlahguaMobile.IOS
{
	public static class BGUIColorExtension
	{
		public static UIImage GetImage(this UIColor color, SizeF size)
		{
			var rect = new RectangleF (new PointF (0, 0), size);

			UIGraphics.BeginImageContext (size);
			var context = UIGraphics.GetCurrentContext ();

			context.SetFillColorWithColor (color.CGColor);
			context.FillRect (rect);

			var image = UIGraphics.GetImageFromCurrentImageContext ();
			UIGraphics.EndImageContext ();

			return image;
		}
	}
}

