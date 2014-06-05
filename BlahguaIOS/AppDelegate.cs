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



		#endregion

		#region Properties

		public override UIWindow Window { get; set; }



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
																					 Font = UIFont.FromName(BGAppearanceConstants.BoldFontName, 18) });
			UINavigationBar.Appearance.TintColor = UIColor.White;
			UINavigationBar.Appearance.SetBackgroundImage (UIImage.FromFile ("navigationBar.png"), UIBarMetrics.Default);


			UIApplication.SharedApplication.SetStatusBarHidden (false, UIStatusBarAnimation.Slide);
			UIApplication.SharedApplication.SetStatusBarStyle(UIStatusBarStyle.LightContent, true);
			BlahguaCore.BlahguaAPIObject.Current.Initialize (null, InitCallback);

            return true;
        }

		#region Methods

		private void InitCallback(bool isOk)
		{
			if(!isOk)
			{
				BlahguaCore.BlahguaAPIObject.Current.Initialize (null, InitCallback);
			}
		}

		#endregion
    }
}

