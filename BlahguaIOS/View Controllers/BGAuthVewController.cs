using System;
using System.Drawing;
using System.CodeDom.Compiler;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.MessageUI;

using BlahguaMobile.BlahguaCore;

using BlahguaMobile.IOS;

namespace BlahguaMobile.IOS
{


	partial class BGAuthVewController : UIViewController
	{

		#region Fields


		private bool signUp = false;

		private MFMailComposeViewController mailComposer;

		#endregion

     
        public BGAuthVewController()
        { 
			NSNotificationCenter.DefaultCenter.AddObserver (UIKeyboard.WillHideNotification, OnKeyboardNotification);
			NSNotificationCenter.DefaultCenter.AddObserver (UIKeyboard.WillShowNotification, OnKeyboardNotification);
        }

		public BGAuthVewController (IntPtr handle) : base (handle)
		{
			NSNotificationCenter.DefaultCenter.AddObserver (UIKeyboard.WillHideNotification, OnKeyboardNotification);
			NSNotificationCenter.DefaultCenter.AddObserver (UIKeyboard.WillShowNotification, OnKeyboardNotification);
		}

		#region View Controller Overridden Methods

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

		}

		private void OnKeyboardNotification (NSNotification notification)
		{
			if (IsViewLoaded) {

				//Check if the keyboard is becoming visible
				bool visible = notification.Name == UIKeyboard.WillShowNotification;

				//Start an animation, using values from the keyboard
				UIView.BeginAnimations ("AnimateForKeyboard");
				UIView.SetAnimationBeginsFromCurrentState (true);
				UIView.SetAnimationDuration (UIKeyboard.AnimationDurationFromNotification (notification));
				UIView.SetAnimationCurve ((UIViewAnimationCurve)UIKeyboard.AnimationCurveFromNotification (notification));

				//Pass the notification, calculating keyboard height, etc.
				bool landscape = InterfaceOrientation == UIInterfaceOrientation.LandscapeLeft || InterfaceOrientation == UIInterfaceOrientation.LandscapeRight;
				if (visible) {
					var keyboardFrame = UIKeyboard.FrameEndFromNotification (notification);

					OnKeyboardChanged (visible, landscape ? keyboardFrame.Width : keyboardFrame.Height);
				} else {
					var keyboardFrame = UIKeyboard.FrameBeginFromNotification (notification);

					OnKeyboardChanged (visible, landscape ? keyboardFrame.Width : keyboardFrame.Height);
				}

				//Commit the animation
				UIView.CommitAnimations ();	
			}
		}

		protected  UIView KeyboardGetActiveView()
		{
			return FindFirstResponder(this.View);
		}

		private  UIView FindFirstResponder(UIView view)
		{
			if (view.IsFirstResponder)
			{
				return view;
			}
			foreach (UIView subView in view.Subviews)
			{
				var firstResponder = FindFirstResponder(subView);
				if (firstResponder != null)
					return firstResponder;
			}
			return null;
		}


		protected virtual void OnKeyboardChanged (bool visible, float keyboardHeight)
		{
			var activeView = KeyboardGetActiveView();
			if (activeView == null)
				return;



			if (!visible)
				RestoreScrollPosition(authScroller);
			else
				CenterViewInScroll(activeView, authScroller, keyboardHeight);
		}

		protected void CenterViewInScroll(UIView viewToCenter, UIScrollView scrollView, float keyboardHeight)
		{
			var contentInsets = new UIEdgeInsets(0.0f, 0.0f, keyboardHeight, 0.0f);
			scrollView.ContentInset = contentInsets;
			scrollView.ScrollIndicatorInsets = contentInsets;

			// Position of the active field relative isnside the scroll view
			RectangleF relativeFrame = viewToCenter.Superview.ConvertRectToView(viewToCenter.Frame, scrollView);

			bool landscape = InterfaceOrientation == UIInterfaceOrientation.LandscapeLeft || InterfaceOrientation == UIInterfaceOrientation.LandscapeRight;
			var spaceAboveKeyboard = (landscape ? scrollView.Frame.Width : scrollView.Frame.Height) - keyboardHeight;

			// Move the active field to the center of the available space
			var offset = relativeFrame.Y - (spaceAboveKeyboard - viewToCenter.Frame.Height) / 2;
			scrollView.ContentOffset = new PointF(0, offset);
		}

		protected virtual void RestoreScrollPosition(UIScrollView scrollView)
		{
			scrollView.ContentInset = UIEdgeInsets.Zero;
			scrollView.ScrollIndicatorInsets = UIEdgeInsets.Zero;
		}

		public override void TouchesBegan (NSSet touches, UIEvent evt)
		{
			var activeView = KeyboardGetActiveView();
			if (activeView != null)
				activeView.ResignFirstResponder();
		}


		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
            AppDelegate.analytics.PostPageView("/signup");
			//View.BackgroundColor = UIColor.FromPatternImage (UIImage.FromBundle("grayBack"));

			if (BGAppearanceHelper.DeviceType == DeviceType.iPhone4) {
				this.View.BackgroundColor = UIColor.FromPatternImage (UIImage.FromBundle ("ios_signin_3x4.jpg"));

			} else if (BGAppearanceHelper.DeviceType == DeviceType.iPhone5) {

				this.View.BackgroundColor = UIColor.FromPatternImage (UIImage.FromBundle ("ios_signin_2x3.jpg"));

			} else {

				this.View.BackgroundColor = UIColor.FromPatternImage (UIImage.FromBundle ("ios_signin_3x4.jpg"));

			}

			authScroller.ContentSize = new SizeF (320, 640);

			NavigationItem.RightBarButtonItem = new UIBarButtonItem ("Done", UIBarButtonItemStyle.Plain, DoneHandler);
			NavigationItem.RightBarButtonItem.Enabled = false;
            //Synsoft on 9 July 2014 for inactive color  #bcbcbc
            NavigationItem.RightBarButtonItem.TintColor = UIColor.FromRGB(188, 188, 188);
			NavigationItem.LeftBarButtonItem = new UIBarButtonItem ("Cancel", UIBarButtonItemStyle.Plain, CancelHandler);
            //Synsoft on 9 July 2014 for active color  #1FBBD1
			NavigationItem.LeftBarButtonItem.TintColor =  UIColor.FromRGB(96, 191, 164);

			NavigationItem.LeftBarButtonItem.Clicked += (object sender, EventArgs e) => {
				NavigationController.PopViewControllerAnimated(true);
			};

			var placeholderAttrs = new UIStringAttributes {
				Font = UIFont.FromName (BGAppearanceConstants.FontName, 15),
				ForegroundColor = UIColor.LightGray
			};
			var textAttrs = new UIStringAttributes {
				Font = UIFont.FromName (BGAppearanceConstants.MediumFontName, 15),
				ForegroundColor = UIColor.Black
			};

			usernameOrEmail.AttributedPlaceholder = new NSAttributedString (
				"Username", 
				placeholderAttrs
			);
			usernameOrEmail.AttributedText = new NSAttributedString ("", textAttrs);
			usernameOrEmail.Background = UIImage.FromBundle ("input_back");
			usernameOrEmail.ShouldReturn = delegate {
				password.BecomeFirstResponder();
				return false;
			};
			usernameOrEmail.AllEditingEvents += InputsEditingHandler;
			usernameOrEmail.ReturnKeyType = UIReturnKeyType.Next;

			password.AttributedPlaceholder = new NSAttributedString (
				"Password", 
				placeholderAttrs
			);
			password.AttributedText = new NSAttributedString ("", textAttrs);
			password.Background = UIImage.FromBundle ("input_back");
			password.ReturnKeyType = UIReturnKeyType.Done;

			password.AllEditingEvents += InputsEditingHandler;

			confirmPassword.AttributedPlaceholder = new NSAttributedString (
				"Confirm Password", 
				placeholderAttrs
			);
			confirmPassword.AttributedText = new NSAttributedString ("", textAttrs);
			confirmPassword.Background = UIImage.FromBundle ("input_back");
			confirmPassword.AllEditingEvents += InputsEditingHandler;

			SetMode (signUp);

			createBtn.ValueChanged += SetModeAction;

			confirmPassword.ReturnKeyType = UIReturnKeyType.Next;
			confirmPassword.ShouldReturn = delegate {
				recoveryEmail.BecomeFirstResponder();
				return false;
			};

			recoveryEmail.ShouldReturn = delegate {
				recoveryEmail.ResignFirstResponder();
				return true;
			};

			recoveryEmail.ReturnKeyType = UIReturnKeyType.Done;



			usernameOrEmail.ReturnKeyType = UIReturnKeyType.Next;
			password.ReturnKeyType = UIReturnKeyType.Default;

			password.SecureTextEntry = true;
			confirmPassword.SecureTextEntry = true;

			helpButton.Enabled = true;
			helpButton.TouchUpInside += (object sender, EventArgs e) => {
				if(MFMailComposeViewController.CanSendMail)
				{
					mailComposer = new MFMailComposeViewController();
					mailComposer.SetToRecipients(new string[] { "info@goheard.com" });
					mailComposer.Finished += (s, ev) => {
						ev.Controller.DismissViewController(true, null);
					};
					PresentViewController(mailComposer, true, null);
				}
				else
				{
					var alert = new UIAlertView("Inforamtion", "There are not email accounts on this iPhone. Please add email account and try again.", null, "OK");
					alert.Show();
				}
			};

			reportBugButton.TouchUpInside +=(object sender, EventArgs e) => {
				if(MFMailComposeViewController.CanSendMail)
				{
					mailComposer = new MFMailComposeViewController();
					mailComposer.SetToRecipients(new string[] { "admin@goheard.com" });
					mailComposer.Finished += (s, ev) => {
						ev.Controller.DismissViewController(true, null);
					};
					PresentViewController(mailComposer, true, null);
				}
				else
				{
					var alert = new UIAlertView("Inforamtion", "There are not email accounts on this iPhone. Please add email account and try again.", null, "OK");
					alert.Show();
				}
			};

			aboutButton.Enabled = true;
			aboutButton.TouchUpInside += (object sender, EventArgs e) => {
				var url = NSUrl.FromString("http://www.goheard.com/"); 
				if(UIApplication.SharedApplication.CanOpenUrl(url))
				{
					UIApplication.SharedApplication.OpenUrl(url);
				}
			};
		}

		public void SetModeAction(object sender, EventArgs e)
		{
			signUp = createBtn.On;

			if(signUp)
			{

				password.ReturnKeyType = UIReturnKeyType.Next;
				password.ShouldReturn = delegate {
					confirmPassword.BecomeFirstResponder();
					return false;
				};




			}
			else
			{

				password.ReturnKeyType = UIReturnKeyType.Done;

				password.ShouldReturn = delegate {
					return true;
				};
			}

			SetMode(signUp);
		}

		public override bool PrefersStatusBarHidden()
		{
			return false;
		}
			
		#endregion

		#region Methods

		private void DoneHandler(object sender, EventArgs args)
		{
			usernameOrEmail.ResignFirstResponder ();
			password.ResignFirstResponder ();
			confirmPassword.ResignFirstResponder ();
			if(signUp)
			{
				SignUp ();
			}
			else
			{
				SignIn ();
			}
			NavigationItem.RightBarButtonItem.Enabled = false;
		}

		private void CancelHandler(object sender, EventArgs args)
		{
			UINavigationController nv = NavigationController;

			if (nv.ViewControllers.Length == 3) {
				DismissViewController (true, null);
				return;
			}

            NavigationController.PopViewControllerAnimated(true);
		}

		private void SignIn()
		{
			BlahguaAPIObject.Current.SignIn (usernameOrEmail.Text, password.Text, true, AuthenticationResultCallback);
		}

		private void SignUp ()
		{
			BlahguaAPIObject.Current.Register (usernameOrEmail.Text, password.Text, true, AuthenticationResultCallback);
		}

		private void InputsEditingHandler(object sender, EventArgs e) {
			if(signUp)
			{
				if(password.Text != confirmPassword.Text || 
				   String.IsNullOrEmpty(password.Text) || 
				   String.IsNullOrEmpty(confirmPassword.Text) || 
				   String.IsNullOrEmpty(usernameOrEmail.Text))
				{
					NavigationItem.RightBarButtonItem.Enabled = false;
                    //Synsoft on 9 July 2014 for inactive 
                    NavigationItem.RightBarButtonItem.TintColor = UIColor.FromRGB(188, 188, 188);
				}
				else
				{
					NavigationItem.RightBarButtonItem.Enabled = true;
                    //Synsoft on 9 July 2014
                    NavigationItem.RightBarButtonItem.TintColor = UIColor.FromRGB(96, 191, 164);
				}
			}
			else
			{
				if(String.IsNullOrEmpty(password.Text) || 
					String.IsNullOrEmpty(usernameOrEmail.Text))
				{
					NavigationItem.RightBarButtonItem.Enabled = false;
                    //Synsoft on 9 July 2014 for inactive 
                    NavigationItem.RightBarButtonItem.TintColor = UIColor.FromRGB(188, 188, 188);
				}
				else
				{
					NavigationItem.RightBarButtonItem.Enabled = true;
                    //Synsoft on 9 July 2014
					NavigationItem.RightBarButtonItem.TintColor =  UIColor.FromRGB(96, 191, 164);
				}
			}
		}

		private void SetMode(bool isSignUpMode)
		{
			confirmPassword.Hidden = !isSignUpMode;
			recoveryEmail.Hidden = !isSignUpMode;
		}

		private void AuthenticationResultCallback(string result)
		{
			if (result == null)
			{
                if (signUp)
                    AppDelegate.analytics.PostRegisterUser();
                else
                    AppDelegate.analytics.PostLogin();
				InvokeOnMainThread (() => NavigationController.PopViewControllerAnimated(true));
			}
			else
			{
                if (signUp)
                    AppDelegate.analytics.PostSessionError("registerfailed-" + result);
                else
                    AppDelegate.analytics.PostSessionError("signinfailed-" + result);

				InvokeOnMainThread (() => NavigationItem.RightBarButtonItem.Enabled = true );

				Console.WriteLine ("Authentication failed");

				InvokeOnMainThread (()=> displayAlert());
			
			}
		}
			
		public void displayAlert()
		{

			/*Console.WriteLine ("Authentication failed in display function");

			UIAlertView alert = new UIAlertView ();
							alert.Title = "Error";
							alert.AddButton ("OK");
							alert.Message = "This should be an error message";

			alert.Show ();*/

			int clicked = -1;
			var x = new UIAlertView ("Error", "Authentication failure",  null, "OK");
			x.Show ();
			//bool done = false;
			x.Clicked += (sender, buttonArgs) => {
				Console.WriteLine ("User clicked on {0}", buttonArgs.ButtonIndex);
				clicked = buttonArgs.ButtonIndex;
			};    
			while (clicked == -1){
				NSRunLoop.Current.RunUntil (NSDate.FromTimeIntervalSinceNow (0.5));
				Console.WriteLine ("Waiting for another 0.5 seconds");
			}
			Console.WriteLine ("The user clicked {0}", clicked);
			//return clicked;
			
		}

		private UIImage GetModeBackgroundImage()
		{
			return UIImage.FromBundle (signUp ? "signupRadioButton" : "signupRadioButtonUn");
		}


		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			if(segue.Identifier == "fromLoginToProfile")
			{
				((BGProfileViewController)segue.DestinationViewController).IsEditMode = true;
			}
			base.PrepareForSegue (segue, sender);
		}

		#endregion
	}
}
