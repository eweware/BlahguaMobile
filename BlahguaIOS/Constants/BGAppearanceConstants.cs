using System;
using System.Drawing;

using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace BlahguaMobile.IOS
{
	public static class BGAppearanceConstants
	{
        public static readonly string FontName = "GothamRounded-Book";
        public static readonly string MediumFontName = "GothamRounded-Book";
        public static readonly string BoldFontName = "GothamRounded-Book";


		public static readonly RectangleF InitialRightViewContainerFrame = new RectangleF(320, 0, 320, UIScreen.MainScreen.Bounds.Height);
		public static readonly RectangleF OpenedRightViewContainerFrame = new RectangleF(0, 0, 320, UIScreen.MainScreen.Bounds.Height);

		public static readonly RectangleF RightViewFrame = new RectangleF(163, 64, 157, UIScreen.MainScreen.Bounds.Height - 64);
        public static readonly UIColor DarkBrown = UIColor.FromRGB(63, 43, 47);
        public static readonly UIColor TealGreen = UIColor.FromRGB(96, 191, 164);
	}
}

