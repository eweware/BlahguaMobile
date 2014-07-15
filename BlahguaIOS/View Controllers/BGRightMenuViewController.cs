
using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.Dialog.Utilities;
using BlahguaMobile.BlahguaCore;


namespace BlahguaMobile.IOS
{
	public partial class BGRightMenuViewController : UIViewController, IImageUpdated
	{
		UIViewController contentView = null;

		public BGRightMenuViewController (IntPtr handle) : base (handle)
		{
		}

		public BGRightMenuViewController () : base ("BGRightMenuViewController", null)
		{
		}

		public void SetContentView(UIViewController contentViewController)
		{
			contentView = contentViewController;
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
				if(contentView != null)
					((BGRollViewController)((BGMainNavigationController)contentView).ViewControllers [0]).PerformSegue ("fromRollToProfile", ((BGRollViewController)((BGMainNavigationController)contentView).ViewControllers [0]));

			};

			m_btnBadges.TouchUpInside += (sender, e) => {
				if(contentView != null)
					((BGRollViewController)((BGMainNavigationController)contentView).ViewControllers [0]).PerformSegue ("fromRollToBadges", ((BGRollViewController)((BGMainNavigationController)contentView).ViewControllers [0]));

			};

			m_btnHistory.TouchUpInside += (sender, e) => {
				if(contentView != null)
					((BGRollViewController)((BGMainNavigationController)contentView).ViewControllers [0]).PerformSegue ("fromRollToHistory", ((BGRollViewController)((BGMainNavigationController)contentView).ViewControllers [0]));

			};
				
			m_btnDemographics.TouchUpInside += (sender, e) => {
				BlahguaAPIObject.Current.GetUserProfile ((profile) => {
					InvokeOnMainThread (() => { if(contentView != null)
						((BGRollViewController)((BGMainNavigationController)contentView).ViewControllers [0]).PerformSegue ("fromRollToDemographics", ((BGRollViewController)((BGMainNavigationController)contentView).ViewControllers [0]));
					});
				});
			};

			m_btnStats.TouchUpInside += (sender, e) => {
				BlahguaAPIObject.Current.GetUserProfile ((profile) => {
					InvokeOnMainThread (() => { if(contentView != null)
						((BGRollViewController)((BGMainNavigationController)contentView).ViewControllers [0]).PerformSegue ("fromRollToStats", ((BGRollViewController)((BGMainNavigationController)contentView).ViewControllers [0]));
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

