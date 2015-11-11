using System;
using CoreGraphics;

using UIKit;
using Foundation;

namespace BlahguaMobile.IOS
{
	public static class BGAppearanceConstants
	{
        public static readonly string FontName = "GothamRounded-Book";
        public static readonly string MediumFontName = "GothamRounded-Book";
        public static readonly string MediumItalicFontName = "GothamRounded-BookItalic";
        public static readonly string BoldFontName = "GothamRounded-Bold";


        public static readonly CGRect InitialRightViewContainerFrame = new CGRect(UIScreen.MainScreen.Bounds.Width, 0, UIScreen.MainScreen.Bounds.Width, UIScreen.MainScreen.Bounds.Height);
        public static readonly CGRect OpenedRightViewContainerFrame = new CGRect(0, 0, UIScreen.MainScreen.Bounds.Width, UIScreen.MainScreen.Bounds.Height);

		public static readonly CGRect RightViewFrame = new CGRect(163, 44, 157, UIScreen.MainScreen.Bounds.Height - 44);

        public static readonly CGRect PollCellRect = new CGRect(0, 0, UIScreen.MainScreen.Bounds.Width, 64);


		public static readonly UIColor buttonTitleInactiveColor = UIColor.FromRGB(248, 248, 248);
		public static readonly UIColor buttonBackInactiveColor = UIColor.FromRGB(188, 188, 188);
        public static readonly UIColor DarkBrown = UIColor.FromRGB(63, 43, 47);
        public static readonly UIColor TealGreen = UIColor.FromRGB(96, 191, 164);
		public static readonly UIColor HeaderBlue = UIColor.FromRGB(31, 187, 209);
		public static readonly UIColor HeardRed = UIColor.FromRGB(231,62,82);
	}
}

