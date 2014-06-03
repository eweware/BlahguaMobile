using System;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace BlahguaMobile.IOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : UIApplicationDelegate
    {
		#region Fields

		private DeviceType type;

		#endregion

		#region Properties

		public override UIWindow Window { get; set; }

		public DeviceType DeviceType 
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

		public UIStoryboard MainStoryboard
		{
			get
			{
				return UIStoryboard.FromName ("Balhgua_iPhone", null);
			}
		}

		#endregion


        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
			UINavigationBar.Appearance.SetTitleTextAttributes(new UITextAttributes { TextColor = UIColor.White, 
																					 TextShadowColor = UIColor.Clear, 
																					 Font = UIFont.FromName(AppearanceConstants.BoldFontName, 18) });
			UINavigationBar.Appearance.TintColor = UIColor.White;
			UINavigationBar.Appearance.SetBackgroundImage (UIImage.FromFile ("navigationBar.png"), UIBarMetrics.Default);


			UIApplication.SharedApplication.SetStatusBarStyle(UIStatusBarStyle.LightContent, true);

            return true;
        }

		#region Methods

		private void GetDeviceType ()
		{
			float height = Window.Bounds.Height;

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

