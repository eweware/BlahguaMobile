using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace BlahguaMobile.IOS
{
	public static class BGAppearanceHelper
	{
		#region Fields

		private static string iphone4Postfix = "-4";
		private static string iphone5Postfix = "-5";
		private static DeviceType type;

		#endregion

		#region Properties

        public static void SetButtonFont(UIButton theBtn, string theFontName, float fontSize = 0f )
        {
            if (fontSize == 0f)
                fontSize = theBtn.Font.PointSize;
            var buttonsTextAttributes = new UIStringAttributes {
                Font = UIFont.FromName (theFontName, fontSize),
                ForegroundColor = theBtn.CurrentTitleColor
            };

            theBtn.SetAttributedTitle (new NSAttributedString (theBtn.CurrentTitle, buttonsTextAttributes), UIControlState.Normal);

        }

		public static DeviceType DeviceType 
		{
			get
			{
				if(type == DeviceType.Undefined)
				{
					GetDeviceType();
				}

				return type;
			}
		}

		#endregion

		#region Methods

		public static UIImage GetBackgroundImage(string backName)
		{
			string[] nameParts = backName.Split (new char [] { '.' });
			string postfix;
			if(BGAppearanceHelper.DeviceType == DeviceType.iPhone5 && nameParts.Length > 1)
			{
				postfix = iphone5Postfix;
			}
			else
			{
				postfix = iphone4Postfix;
			}
			//string fileName = String.Format ("{0}{1}.{2}", nameParts [0], postfix, nameParts [nameParts.Length - 1]);
            string fileName = String.Format ("{0}{1}", nameParts [0], postfix);
			return UIImage.FromBundle(fileName);
		}

		public static UIColor GetColorForBackground(string backName)
		{
			return UIColor.FromPatternImage (GetBackgroundImage (backName));
		}

		private static void GetDeviceType ()
		{
			float height = UIScreen.MainScreen.Bounds.Height;

			switch((int)height)
			{
			case 480:
				{
					type = DeviceType.iPhone4;
					break;
				}
			case 568:
				{
					type = DeviceType.iPhone5;
					break;
				}
			case 1024:
				{
					type = DeviceType.iPad;
					break;
				}
			case 2048:
				{
					type = DeviceType.iPadRetina;
					break;
				}
			default:
				{
					type = DeviceType.iPhone4;
					break;
				}
			}
		}

		#endregion
	}
}

