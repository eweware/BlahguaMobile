using System;
using MonoTouch.UIKit;

namespace BlahguaMobile.IOS
{
	public static class AppearanceHelper
	{
		private static string iphone4Postfix = "-4";
		private static string iphone5Postfix = "-5";

		public static UIImage GetBackgroundImage(string backName)
		{
			string[] nameParts = backName.Split (new char [] { '.' });
			string postfix;
			if(((AppDelegate)UIApplication.SharedApplication.Delegate).DeviceType == DeviceType.iPhone5 && nameParts.Length > 1)
			{
				postfix = iphone5Postfix;
			}
			else
			{
				postfix = iphone4Postfix;
			}
			string fileName = String.Format ("{0}{1}.{2}", nameParts [0], postfix, nameParts [nameParts.Length - 1]);
			return UIImage.FromFile(fileName);
		}

		public static UIColor GetColorForBackground(string backName)
		{
			return UIColor.FromPatternImage (GetBackgroundImage (backName));
		}
	}
}

