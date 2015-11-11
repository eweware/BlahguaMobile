using System;
using CoreGraphics;

using UIKit;
using Foundation;

namespace BlahguaMobile.IOS
{
	public static class BGUIColorExtension
	{
		public static UIImage GetImage(this UIColor color, CGSize size)
		{
			var rect = new CGRect (new CGPoint (0, 0), size);

			UIGraphics.BeginImageContext (size);
			var context = UIGraphics.GetCurrentContext ();
            context.SetFillColor(color.CGColor);

			context.FillRect (rect);

			var image = UIGraphics.GetImageFromCurrentImageContext ();
			UIGraphics.EndImageContext ();

			return image;
		}
	}
}

