
using System;
using CoreGraphics;
using Foundation;
using UIKit;
using MonoTouch.Dialog.Utilities;
using BlahguaMobile.BlahguaCore;


namespace BlahguaMobile.IOS
{
	public partial class BGRightMenuViewController : UIViewController, IImageUpdated
	{
		private bool _isProcessing = false;

		public BGRightMenuViewController (IntPtr handle) : base (handle)
		{
		}

		public BGRightMenuViewController () : base ("BGRightMenuViewController", null)
		{
		}
		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();

			// Release any cached data, images, etc that aren't in use.
		}

        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
			if (!_isProcessing) {
				_isProcessing = true;

				base.TouchesBegan(touches, evt);
				// Get the current touch
				UITouch touch = touches.AnyObject as UITouch;
				if (touch != null)
				{
					// Check to see if any of the images have been touched
					if (m_imgAvatar.Frame.Contains(touch.LocationInView(this.View)))
					{
						((AppDelegate)UIApplication.SharedApplication.Delegate).SlideMenu.CloseRightMenuForNavigation();
						BGProfileViewController vc = (BGProfileViewController)((AppDelegate)UIApplication.SharedApplication.Delegate).MainStoryboard.InstantiateViewController("BGProfileViewController");
						vc.IsEditMode = true;
						((AppDelegate)UIApplication.SharedApplication.Delegate).SlideMenu.NavigationController.PushViewController(vc, true);
					}
				}
			}

            
        }

		public override void ViewDidAppear (bool animated)
		{
			_isProcessing = false;
			base.ViewDidAppear (animated);
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

            m_imgAvatar.Image = GetProfileImage ();
          
			m_lblUserName.Text = BlahguaAPIObject.Current.CurrentUser.UserName;


            BGAppearanceHelper.SetButtonFont(m_btnProfile, BGAppearanceConstants.MediumFontName); 


			m_btnProfile.TouchUpInside += (sender, e) => {

				((AppDelegate)UIApplication.SharedApplication.Delegate).SlideMenu.CloseRightMenuForNavigation();
				BGProfileViewController vc = (BGProfileViewController)((AppDelegate)UIApplication.SharedApplication.Delegate).MainStoryboard.InstantiateViewController("BGProfileViewController");
				vc.IsEditMode = true;
				((AppDelegate)UIApplication.SharedApplication.Delegate).SlideMenu.NavigationController.PushViewController(vc, true);
			};

            BGAppearanceHelper.SetButtonFont(m_btnBadges, BGAppearanceConstants.MediumFontName); 
			m_btnBadges.TouchUpInside += (sender, e) => {
	
				((AppDelegate)UIApplication.SharedApplication.Delegate).SlideMenu.CloseRightMenuForNavigation();

				((AppDelegate)UIApplication.SharedApplication.Delegate).SlideMenu.NavigationController.PushViewController((BGBadgeCollectionViewController)((AppDelegate)UIApplication.SharedApplication.Delegate).MainStoryboard.InstantiateViewController("BGBadgeCollectionViewController"), true);

			};

            BGAppearanceHelper.SetButtonFont(m_btnHistory, BGAppearanceConstants.MediumFontName); 
			m_btnHistory.TouchUpInside += (sender, e) => {
				((AppDelegate)UIApplication.SharedApplication.Delegate).SlideMenu.CloseRightMenuForNavigation();

				((AppDelegate)UIApplication.SharedApplication.Delegate).SlideMenu.NavigationController.PushViewController((BGHistoryViewController)((AppDelegate)UIApplication.SharedApplication.Delegate).MainStoryboard.InstantiateViewController("BGHistoryViewController"), true);
			};
			
            BGAppearanceHelper.SetButtonFont(m_btnDemographics, BGAppearanceConstants.MediumFontName); 
			m_btnDemographics.TouchUpInside += (sender, e) => {
				BlahguaAPIObject.Current.GetUserProfile ((profile) => {
					InvokeOnMainThread (() => {
						((AppDelegate)UIApplication.SharedApplication.Delegate).SlideMenu.CloseRightMenuForNavigation();

						((AppDelegate)UIApplication.SharedApplication.Delegate).SlideMenu.NavigationController.PushViewController((BGDemographicsViewController)((AppDelegate)UIApplication.SharedApplication.Delegate).MainStoryboard.InstantiateViewController("BGDemographicsViewController"), true);

					});
				});
			};

            BGAppearanceHelper.SetButtonFont(m_btnStats, BGAppearanceConstants.MediumFontName); 
            m_btnStats.Hidden = true; // TO DO:  renable when this page is fixed
			m_btnStats.TouchUpInside += (sender, e) => {
				BlahguaAPIObject.Current.GetUserProfile ((profile) => {
					InvokeOnMainThread (() => {
						((AppDelegate)UIApplication.SharedApplication.Delegate).SlideMenu.CloseRightMenuForNavigation();

						((AppDelegate)UIApplication.SharedApplication.Delegate).SlideMenu.NavigationController.PushViewController((BGStatsTableViewController)((AppDelegate)UIApplication.SharedApplication.Delegate).MainStoryboard.InstantiateViewController("BGStatsTableViewController"), true);

						//rollView.PerformSegue ("fromRollToStats", rollView);
					});
				});
			};

            BGAppearanceHelper.SetButtonFont(m_btnLogout, BGAppearanceConstants.MediumFontName); 
			m_btnLogout.TouchUpInside += (sender, e) => {
				((AppDelegate)UIApplication.SharedApplication.Delegate).SlideMenu.ToggleRightMenuAnimated();
				BlahguaAPIObject.Current.SignOut (null, (theStr) =>
					{
						if (theStr)
						{
							InvokeOnMainThread(()=>{
								((BGRollViewController)	((BGMainNavigationController)((AppDelegate)UIApplication.SharedApplication.Delegate).SlideMenu.ContentViewController).ViewControllers[0]).NaturalScrollInProgress = true;
								((BGRollViewController)	((BGMainNavigationController)((AppDelegate)UIApplication.SharedApplication.Delegate).SlideMenu.ContentViewController).ViewControllers[0]).ClearRightBarButton();
							});
						}

						//NavigationService.GoBack();
					}
				);


			};

			// Perform any additional setup after loading the view, typically from a nib.
		}

		public void UpdateForUser()
		{
            if (BlahguaAPIObject.Current.CurrentUser != null)
            {
                if (m_imgAvatar != null)
                    m_imgAvatar.Image = GetProfileImage();
                if (m_lblUserName != null)
                    m_lblUserName.Text = BlahguaAPIObject.Current.CurrentUser.UserName;
            }
		}

		private UIImage GetProfileImage ()
		{
			return ImageLoader.DefaultRequestImage (new Uri (BlahguaCore.BlahguaAPIObject.Current.CurrentUser.UserImage), this);

		}


		#region IImageUpdated implementation

		public void UpdatedImage (Uri uri)
		{
			var image = ImageLoader.DefaultRequestImage (new Uri (BlahguaCore.BlahguaAPIObject.Current.CurrentUser.UserImage), this);
			//profile.SetImage (image, UIControlState.Normal);
			m_imgAvatar.Image = image;
		}

		#endregion
	}
}















