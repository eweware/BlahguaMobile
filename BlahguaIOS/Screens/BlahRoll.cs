using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.CoreAnimation;

namespace BlahguaMobile.IOS
{
	public partial class BlahRoll : UIViewController
	{
		BlahDetails blahDetailsScreen;
		Profile profileScreen;
		SignIn signInScreen;

		public BlahRoll () : base ("BlahRoll", null)
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
			
			// Perform any additional setup after loading the view, typically from a nib.
			this.profileBtn.TouchUpInside += HandleShowProfile;
			this.detailsBtn.TouchUpInside += HandleShowBlahDetails;
			this.signInBtn.TouchUpInside +=  HandleShowSignIn; 

			AddBlahs ();
		}

		private void AddBlahs()
		{
			double curY = 0;
			double curX = 0;
			double gutter = 4;
			double margin = 8;
			double screenWidth = 320;
			double blahWidth = (screenWidth - ((gutter * 2) + (margin * 3))) / 4;
			double blahHeight = blahWidth;

			for (int curRow = 0; curRow < 20; curRow++) {
				curX = gutter;
				for (int curCol = 0; curCol < 3; curCol++) {

					RectangleF newRect = new RectangleF (curX, curY, blahWidth, blahHeight);
					UIButton newBtn = new UIButton (UIButtonType.RoundedRect);
					newBtn.Frame = newRect;
					newBtn.BackgroundColor = UIColor.Blue;
					newBtn.TintColor = UIColor.Purple;
					newBtn.SetTitle ("This is a blah!", UIControlState.Normal);
					curX += (blahWidth + margin);
				}

				curY += (blahHeight + margin);
			}

		}

		void  HandleShowProfile (object sender, EventArgs e)
		{
			if (this.profileScreen == null)
				this.profileScreen = new Profile ();

			UIView.BeginAnimations (null);
			UIView.SetAnimationCurve (UIViewAnimationCurve.EaseInOut);
			UIView.SetAnimationDuration (.75);
			this.NavigationController.PushViewController (this.profileScreen, false); 
			UIView.SetAnimationTransition (UIViewAnimationTransition.FlipFromLeft, this.NavigationController.View, false);
			UIView.CommitAnimations ();
			     

		}

		void  HandleShowBlahDetails (object sender, EventArgs e)
		{
			if (this.blahDetailsScreen == null)
				this.blahDetailsScreen = new BlahDetails ();
			this.NavigationController.PushViewController (this.blahDetailsScreen, true);      

		}

		void  HandleShowSignIn (object sender, EventArgs e)
		{
			if (this.signInScreen == null)
				this.signInScreen = new SignIn ();
			this.NavigationController.PushViewController (this.signInScreen, true);      

		}

		public override void ViewWillAppear (bool animated) {
			base.ViewWillAppear (animated);
			this.NavigationController.SetNavigationBarHidden (true, animated);
		}

		public override void ViewWillDisappear (bool animated) {
			base.ViewWillDisappear (animated);
			this.NavigationController.SetNavigationBarHidden (false, animated);
		}


	}
}

