using System;
using System.Collections.Generic;
using System.Linq;
using CoreGraphics;
using BlahguaMobile.BlahguaCore;

using Foundation;
using UIKit;
using MonoTouch.SlideMenu;
using System.IO.IsolatedStorage;
using HockeyApp;

namespace BlahguaMobile.IOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : UIApplicationDelegate
    {
		#region Fields

		private BGLeftMenuTableViewController menu;
		private BGRightMenuViewController rightMenu;
        public static SlideMenuController slideMenu;
        public static UINavigationController navController;
		public SwipeViewController swipeView;
        private bool appIsDead = false;

		#endregion

		#region Properties

		public override UIWindow Window { get; set; }

        public static GoogleAnalytics   analytics = null;

		public SlideMenuController SlideMenu
		{
			get
			{
				if (slideMenu == null)
				{
					slideMenu = new SlideMenuController(Menu,RightMenu, new UIViewController());
					navController = new UINavigationController (slideMenu);
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

        NetworkStatus remoteHostStatus, internetStatus, localWifiStatus;



        void UpdateStatus ()
        {
            remoteHostStatus = Reachability.RemoteHostStatus ();
            internetStatus = Reachability.InternetConnectionStatus ();
            localWifiStatus = Reachability.LocalWifiConnectionStatus ();
        }

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
			
            HockeyApp.Setup.EnableCustomCrashReporting (() => {

                //Get the shared instance
                var manager = BITHockeyManager.SharedHockeyManager;

                //Configure it to use our APP_ID
                manager.Configure ("a045bb829d42942b5659242046565b9e");

                //Start the manager
                manager.StartManager ();

                //Authenticate (there are other authentication options)
                manager.Authenticator.AuthenticateInstallation ();

                //Rethrow any unhandled .NET exceptions as native iOS 
                // exceptions so the stack traces appear nicely in HockeyApp
                AppDomain.CurrentDomain.UnhandledException += (sender, e) => 
                    Setup.ThrowExceptionAsNative(e.ExceptionObject);

                System.Threading.Tasks.TaskScheduler.UnobservedTaskException += (sender, e) => 
                    Setup.ThrowExceptionAsNative(e.Exception);
            });



            UpdateStatus ();
			if ((remoteHostStatus == NetworkStatus.NotReachable) && (!BlahguaCore.BlahguaRESTservice.usingQA))
            {
                if ((internetStatus == NetworkStatus.NotReachable) && (localWifiStatus == NetworkStatus.NotReachable))
                {
                    ShowTerminalDialog("Unable to connect to the Internet.  Please check your connection or try again later.");
                }
                else
                {
                    ShowTerminalDialog("Unable to connect to the Heard.  Please try again later.");
                }
            }
            else
            {
                Reachability.ReachabilityChanged += (object sender, EventArgs e) =>
                {
                    UpdateStatus();
                };

                UIApplication.SharedApplication.SetStatusBarHidden(true, UIStatusBarAnimation.Slide);
                //UIImage theImg = UIImage.FromBundle("SplashScreen");
				Window.RootViewController.View.BackgroundColor = BGAppearanceConstants.HeardRed;

                InitAnalytics();
                BlahguaCore.BlahguaAPIObject.Current.Initialize(null, InitCallback);
                this.Window.TintColor = BGAppearanceConstants.TealGreen;

                // set the sizes
                SetBlahSizesForScreen();
            }

            return true;
        }

        private void InitAnalytics()
        {
            string uniqueId;

            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            if (settings.Contains("uniqueId"))
                uniqueId = settings["uniqueId"].ToString();
            else
            {
                uniqueId = Guid.NewGuid().ToString();
                settings.Add("uniqueId", uniqueId);
                settings.Save();

            }

            string maker = "Apple";
            string model = UIDevice.CurrentDevice.Model;
            string version = NSBundle.MainBundle.InfoDictionary["CFBundleVersion"].ToString();
            string platform = "IOS";
            string userAgent = "Mozilla/5.0 (IOS; Apple; Mobile) ";

            analytics = new GoogleAnalytics(userAgent, maker, model, version, platform, uniqueId);
            analytics.StartSession();
        }

		public override void OnActivated(UIApplication application)
		{
			if (BlahguaAPIObject.Current.CurrentUser != null)
				BlahguaAPIObject.Current.EnsureSignin ();
			BlahguaAPIObject.Current.ForceCurrentChannel ();

		}
            
        public override void DidEnterBackground(UIApplication application)
        {
            BlahguaAPIObject.Current.FlushImpressionList();
            if (appIsDead)
                throw new Exception("App hit terminal error");
        }


        private void ShowTerminalDialog(string errMsg)
        {
            InvokeOnMainThread(() =>
                {
                    UIAlertView theView = new UIAlertView("Error", errMsg + Environment.NewLine + 
                        Environment.NewLine + "You should exit the app now", null, null);
                    theView.Clicked += (object sender, UIButtonEventArgs e) => 
                        {

                        };
                    theView.Show();
                    appIsDead = true;
                });
        }


        public void SetBlahSizesForScreen()
        {
            CGRect screenRect = UIScreen.MainScreen.Bounds;
            float screenWidth = (float)screenRect.Width;
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
            BGBlahCellSizesConstants.TinyCellSize = new CGSize(smallSize, smallSize);
            BGBlahCellSizesConstants.SmallCellSize = new CGSize(mediumSize, smallSize);
            BGBlahCellSizesConstants.MediumCellSize = new CGSize(mediumSize, mediumSize);
            BGBlahCellSizesConstants.LargeCellSize = new CGSize(largeSize, mediumSize);
			BlahguaAPIObject.largeTileSize = (int)largeSize;
			BlahguaAPIObject.mediumTileSize = (int)mediumSize;
			BlahguaAPIObject.smallTileSize = (int)smallSize;
        }

		#region Methods

        static int retryCount = 0;

		private void InitCallback(bool isOk)
		{
			if(!isOk)
			{
                retryCount++;
                if (retryCount < 3)
				    BlahguaCore.BlahguaAPIObject.Current.Initialize (null, InitCallback);
                else
                {
                    analytics.PostCrash("startupfail");
                    ShowTerminalDialog("Unable to connect to the Heard.  Please check your internet connection or try again later.");
                }
			}
			else
			{
                if (BlahguaAPIObject.Current.CurrentUser != null)
                    AppDelegate.analytics.PostAutoLogin();
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

						bool isSecond = NSUserDefaults.StandardUserDefaults.BoolForKey("isSecond");
                        if (!isSecond)
						{
                            UIStoryboard signUpSB = UIStoryboard.FromName("SignOnStoryBoard", null);
                            BGSignOnPageViewController signUpVC = (BGSignOnPageViewController)signUpSB.InstantiateViewController("SignOnViewController");
                            Window.RootViewController = signUpVC;
						}
						else
						{
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
									Window.RootViewController = SlideMenu.NavigationController;
									}); 
								}
							}
						}
					});
			}
		}

		#endregion
    }
}

