
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

			//m_imgAvatar.Image = GetProfileImage ();

			//m_lblUserName.Text = BlahguaAPIObject.Current.CurrentUser.UserName;

			/*
			m_btnProfile.TouchUpInside += (sender, e) => {
				PerformSegue("menuToProfile", this);
			};
			*/
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
			//m_imgAvatar.Image = image;
		}

		#endregion
	}
}

