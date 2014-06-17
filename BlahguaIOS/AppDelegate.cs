using System;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.SlideMenu;

namespace BlahguaMobile.IOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : UIApplicationDelegate
    {
		#region Fields

		private BGLeftMenuTableViewController menu;
		private SlideMenuController slideMenu;


		#endregion

		#region Properties

		public override UIWindow Window { get; set; }

		public SlideMenuController SlideMenu
		{
			get
			{
				if (slideMenu == null)
				{
					slideMenu = new SlideMenuController(Menu, new UIViewController());
				}
				return slideMenu;
			}
		}

		public BGLeftMenuTableViewController Menu
		{
			get
			{
				if(menu == null)
				{
					menu = (BGLeftMenuTableViewController)MainStoryboard.InstantiateViewController("BGLeftMenuTableViewController");
				}
				return menu;
			}
		}


		public UIStoryboard MainStoryboard
		{
			get
			{
				return UIStoryboard.FromName ("Blahgua_iPhone", null);
			}
		}

		#endregion


        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {

            foreach (string curString in UIFont.FamilyNames)
            {
                    System.Console.WriteLine("Family: " + curString);
                foreach (string curFont in UIFont.FontNamesForFamilyName(curString))
                {
                            System.Console.WriteLine("   Font: " + curFont);
                }
            }

			UIApplication.SharedApplication.SetStatusBarHidden (true, UIStatusBarAnimation.Slide);
			//UIApplication.SharedApplication.SetStatusBarStyle(UIStatusBarStyle.LightContent, true);
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
			else
			{
				InvokeOnMainThread (() => {
					UINavigationBar.Appearance.SetTitleTextAttributes(new UITextAttributes { TextColor = UIColor.White, 
						TextShadowColor = UIColor.Clear, 
						Font = UIFont.FromName(BGAppearanceConstants.BoldFontName, 18) });
					UINavigationBar.Appearance.TintColor = UIColor.White;
					UINavigationBar.Appearance.SetBackgroundImage (UIImage.FromFile ("navigationBar.png"), UIBarMetrics.Default);
					UINavigationBar.Appearance.ShadowImage = new UIImage();

					var c = MainStoryboard.InstantiateViewController ("BGMainNavigationController");

					if (SlideMenu.ContentViewController != null && c.GetType() == SlideMenu.ContentViewController.GetType())
					{
						SlideMenu.ShowContentViewControllerAnimated(true, null, false);
					} 
					else
					{
						SlideMenu.SetContentViewControllerAnimated(c as UIViewController, true);
						if (Window.RootViewController != SlideMenu) {
							UIView.Transition(this.Window.RootViewController.View, this.SlideMenu.View, 0.5, UIViewAnimationOptions.TransitionFlipFromRight, delegate {
								Window.RootViewController = SlideMenu;
							}); 
						}
					}
				});
			}
		}

		#endregion
    }
}

