using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;

namespace BlahguaMobile.IOS
{
	public class BGTutorialViewController:UIViewController
	{
		public RectangleF m_frame;
		public BGTutorialViewController ()
		{

		}

		public override void LoadView()
		{
			base.LoadView ();
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad ();
			m_frame = this.View.Bounds;
			TutorialSwipeViewController tv = new TutorialSwipeViewController (m_frame);
			TutorialSwipeViewDataSource dataSource = new BGTutorialDataSource(m_frame);//new TutorialSwipeViewDataSource ();

			tv.SetDataSource (dataSource);

			var btnRect = new RectangleF (m_frame.Width - 45, 5, 20, 20);
			var btnSkip = new UIButton (UIButtonType.Custom);
			btnSkip.Frame = btnRect;
			btnSkip.SetImage (UIImage.FromBundle ("tutorial_close"), UIControlState.Normal);

			btnSkip.TouchUpInside += (object sender, EventArgs e) => {
			
				AppDelegate appDelegate = (AppDelegate)UIApplication.SharedApplication.Delegate;

				var c =appDelegate.MainStoryboard.InstantiateViewController ("BGMainNavigationController");
				if (appDelegate.SlideMenu.ContentViewController != null && c.GetType() == appDelegate.SlideMenu.ContentViewController.GetType())
				{
					appDelegate.SlideMenu.ShowContentViewControllerAnimated(true, null, false);
				} 
				else
				{
					appDelegate.SlideMenu.SetContentViewControllerAnimated(c as UIViewController, true);
					if (appDelegate.Window.RootViewController != appDelegate.SlideMenu) {
						UIView.Transition(appDelegate.Window.RootViewController.View, appDelegate.SlideMenu.View, 0.5, UIViewAnimationOptions.TransitionFlipFromRight, delegate {
							appDelegate.Window.RootViewController = appDelegate.SlideMenu.NavigationController;
						}); 
					}
				}

				this.DismissViewController(true, null);
			};
			this.View.AddSubview (tv);
			this.View.AddSubview (btnSkip);


		}

		public class BGTutorialDataSource :TutorialSwipeViewDataSource{
			private RectangleF m_frame;
			public BGTutorialDataSource(RectangleF frame):base()
			{
				m_frame = frame;
			}
			public override int NumberOfPages()
			{
				return 5;
			}

			public override UIView PageAtIndex(int index)
			{
				UIImageView imv = new UIImageView(m_frame);
				String fileName = String.Format ("tutorial_screen_{0}.jpg", index + 1);
				UIImage img = UIImage.FromBundle (fileName);

				imv.Image = img;
				imv.ContentMode = UIViewContentMode.ScaleAspectFit;

				return imv;
			}
		}
	}

}

