using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using BlahguaMobile.BlahguaCore;

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
		private BGRightMenuViewController rightMenu;
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
					slideMenu = new SlideMenuController(Menu,RightMenu, new UIViewController());
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

		public BGRightMenuViewController RightMenu
		{
			get
			{
				if(rightMenu == null)
				{
					rightMenu = (BGRightMenuViewController)MainStoryboard.InstantiateViewController("BGRightMenuViewController");
				}
				return rightMenu;
			}
		}


		public UIStoryboard MainStoryboard
		{
			get
			{
				return UIStoryboard.FromName ("Blahgua_iPhone", null);
			}
		}

		public Blah CurrentBlah 
		{
			get;
			set;
		}

		#endregion


        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
			UIApplication.SharedApplication.SetStatusBarHidden (true, UIStatusBarAnimation.Slide);
             Window.RootViewController.View.BackgroundColor = UIColor.FromPatternImage (
				UIImage.FromBundle (BGAppearanceHelper.DeviceType == DeviceType.iPhone4 ? 
					"Default" : "Default-568h"));     

			BlahguaCore.BlahguaAPIObject.Current.Initialize (null, InitCallback);
            this.Window.TintColor = BGAppearanceConstants.TealGreen;

            // set the sizes
            SetBlahSizesForScreen();

            return true;
        }

        public void SetBlahSizesForScreen()
        {
            RectangleF screenRect = UIScreen.MainScreen.Bounds;
            float screenWidth = screenRect.Width;
            if (screenWidth > 512)
            {
                BGBlahCellSizesConstants.BlahGutter = ((screenWidth - 512) / 2);
            }
            float smallSize = (screenWidth - ((BGBlahCellSizesConstants.BlahGutter * 2f) + (BGBlahCellSizesConstants.BlahSpacing * 2))) / 3;
            float mediumSize = (smallSize * 2) + BGBlahCellSizesConstants.BlahSpacing;
            float largeSize = screenWidth - (BGBlahCellSizesConstants.BlahGutter * 2);
            smallSize = (float)Math.Round(smallSize) ;
            mediumSize = (float)Math.Round(mediumSize) ;
            largeSize = (float)Math.Round(largeSize) ;
            BGBlahCellSizesConstants.TinyCellSize = new SizeF(smallSize, smallSize);
            BGBlahCellSizesConstants.SmallCellSize = new SizeF(mediumSize, smallSize);
            BGBlahCellSizesConstants.MediumCellSize = new SizeF(mediumSize, mediumSize);
            BGBlahCellSizesConstants.LargeCellSize = new SizeF(largeSize, mediumSize);
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
				InvokeOnMainThread (() => 
                        {
                            UINavigationBar.Appearance.SetTitleTextAttributes(new UITextAttributes 
                                { 
                                    TextColor = BGAppearanceConstants.TealGreen, 
        						    TextShadowColor = UIColor.Clear, 
        						    Font = UIFont.FromName(BGAppearanceConstants.BoldFontName, 18) 
                                });
                            UINavigationBar.Appearance.BarTintColor = BGAppearanceConstants.DarkBrown;
							UINavigationBar.Appearance.TintColor = BGAppearanceConstants.TealGreen;
                            UINavigationBar.Appearance.BackgroundColor = BGAppearanceConstants.DarkBrown;
                            //UINavigationBar.Appearance.SetBackgroundImage (UIImage.FromFile ("navigationBar.png"), UIBarMetrics.Default);
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

