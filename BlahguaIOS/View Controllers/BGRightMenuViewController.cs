
using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.Dialog.Utilities;
using BlahguaMobile.BlahguaCore;
using BlahguaMobile.IOS.View_Controllers;




namespace BlahguaMobile.IOS
{
	public partial class BGRightMenuViewController : UIViewController, IImageUpdated
	{
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

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			m_imgAvatar.Image = GetProfileImage ();

			m_lblUserName.Text = BlahguaAPIObject.Current.CurrentUser.UserName;


			m_btnProfile.TouchUpInside += (sender, e) => {

				((AppDelegate)UIApplication.SharedApplication.Delegate).SlideMenu.CloseRightMenuForNavigation();

				((AppDelegate)UIApplication.SharedApplication.Delegate).SlideMenu.NavigationController.PushViewController((BGProfileViewController)((AppDelegate)UIApplication.SharedApplication.Delegate).MainStoryboard.InstantiateViewController("BGProfileViewController"), true);
			};

			m_btnBadges.TouchUpInside += (sender, e) => {
	
				((AppDelegate)UIApplication.SharedApplication.Delegate).SlideMenu.CloseRightMenuForNavigation();

				((AppDelegate)UIApplication.SharedApplication.Delegate).SlideMenu.NavigationController.PushViewController((BGBadgeCollectionViewController)((AppDelegate)UIApplication.SharedApplication.Delegate).MainStoryboard.InstantiateViewController("BGBadgeCollectionViewController"), true);

			};

			m_btnHistory.TouchUpInside += (sender, e) => {
				((AppDelegate)UIApplication.SharedApplication.Delegate).SlideMenu.CloseRightMenuForNavigation();

				((AppDelegate)UIApplication.SharedApplication.Delegate).SlideMenu.NavigationController.PushViewController((BGHistoryViewController)((AppDelegate)UIApplication.SharedApplication.Delegate).MainStoryboard.InstantiateViewController("BGHistoryViewController"), true);
			};
				
			m_btnDemographics.TouchUpInside += (sender, e) => {
				BlahguaAPIObject.Current.GetUserProfile ((profile) => {
					InvokeOnMainThread (() => {
						((AppDelegate)UIApplication.SharedApplication.Delegate).SlideMenu.CloseRightMenuForNavigation();

						((AppDelegate)UIApplication.SharedApplication.Delegate).SlideMenu.NavigationController.PushViewController((BGDemographicsViewController)((AppDelegate)UIApplication.SharedApplication.Delegate).MainStoryboard.InstantiateViewController("BGDemographicsViewController"), true);

					});
				});
			};

            //m_btnStats.TouchUpInside += (sender, e) =>
            //{
            //    BlahguaAPIObject.Current.GetUserProfile((profile) =>
            //    {
            //        InvokeOnMainThread(() =>
            //        {
            //            ((AppDelegate)UIApplication.SharedApplication.Delegate).SlideMenu.CloseRightMenuForNavigation();

            //            ((AppDelegate)UIApplication.SharedApplication.Delegate).SlideMenu.NavigationController.PushViewController((BGStatsTableViewController)((AppDelegate)UIApplication.SharedApplication.Delegate).MainStoryboard.InstantiateViewController("BGStatsTableViewController"), true);

            //            //rollView.PerformSegue ("fromRollToStats", rollView);
            //        });
            //    });
            //};


            m_btnStats.TouchUpInside += (sender, e) =>
            {
                //PerformSegue("fromProfileToStats", null);
               BlahguaAPIObject.Current.GetUserProfile((profile) =>
                {
                    InvokeOnMainThread(() =>
                    {
                        ((AppDelegate)UIApplication.SharedApplication.Delegate).SlideMenu.CloseRightMenuForNavigation();

                        ((AppDelegate)UIApplication.SharedApplication.Delegate).SlideMenu.NavigationController.PushViewController((ProfileToStatsViewController)((AppDelegate)UIApplication.SharedApplication.Delegate).MainStoryboard.InstantiateViewController("ProfileToStatsViewController"), true);

                        //rollView.PerformSegue ("fromRollToStats", rollView);
                    });
                });
            };

			m_btnLogout.TouchUpInside += (sender, e) => {
				BlahguaAPIObject.Current.SignOut (null, null);
			};

			// Perform any additional setup after loading the view, typically from a nib.
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















