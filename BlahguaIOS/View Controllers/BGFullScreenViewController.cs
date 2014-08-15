
using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace BlahguaMobile.IOS
{
	public partial class BGFullScreenViewController : UIViewController
	{
		private UIImage m_image = null;
		public BGFullScreenViewController ()
		{
		}

		public BGFullScreenViewController(IntPtr handle) :base (handle)
		{
		}
		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}
		public UIImage FullImage {
			get {
				return m_image;
			}
			set{
				m_image = value;
			}

		}

		public override void ViewDidLoad ()
		{
            base.ViewDidLoad ();
            AppDelegate.analytics.PostPageView("/ImageViewer");

			this.NavigationController.SetNavigationBarHidden (true, false);

			if (m_image != null)
				m_imageView.Image = m_image;

			NSAction dismissView = () => {
				this.NavigationController.PopViewControllerAnimated(false);
			};

			UITapGestureRecognizer imageTapRecognizer = new UITapGestureRecognizer (dismissView);
			imageTapRecognizer.Delegate = new PanGestureRecognizerDelegate ();
			imageTapRecognizer.NumberOfTapsRequired = 1;
			m_imageView.AddGestureRecognizer (imageTapRecognizer);
			// Perform any additional setup after loading the view, typically from a nib.
		}
	}
}

