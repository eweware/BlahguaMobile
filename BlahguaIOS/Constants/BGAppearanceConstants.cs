using System;
using System.Drawing;

using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace BlahguaMobile.IOS
{
	public static class BGAppearanceConstants
	{
		public static readonly string FontName = "HelveticaNeue-Light";
		public static readonly string MediumFontName = "HelveticaNeue-Medium";
		public static readonly string BoldFontName = "HelveticaNeue-CondensedBold";


		public static readonly RectangleF InitialRightViewContainerFrame = new RectangleF(320, 0, 320, UIScreen.MainScreen.Bounds.Height);
		public static readonly RectangleF OpenedRightViewContainerFrame = new RectangleF(0, 0, 320, UIScreen.MainScreen.Bounds.Height);

		public static readonly RectangleF RightViewFrame = new RectangleF(163, 64, 157, UIScreen.MainScreen.Bounds.Height - 64);

		public static readonly RectangleF PollCellRect = new RectangleF(0, 0, 320, 64);
	}
}

