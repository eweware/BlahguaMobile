using System;
using System.Drawing;
using System.CodeDom.Compiler;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

using BlahguaMobile.BlahguaCore;

namespace BlahguaMobile.IOS
{
	partial class BGAuthVewController : UIViewController
	{

		#region Fields

		private bool rememberMe = true;
		private bool signUp = false;

		#endregion

		public BGAuthVewController (IntPtr handle) : base (handle)
		{
		}

		#region View Controller Overridden Methods

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			View.BackgroundColor = BGAppearanceHelper.GetColorForBackground ("signinBckg.png");

			NavigationItem.RightBarButtonItem = new UIBarButtonItem ("Done", UIBarButtonItemStyle.Plain, DoneHandler);
			NavigationItem.LeftBarButtonItem = new UIBarButtonItem ("Cancel", UIBarButtonItemStyle.Plain, CancelHandler);
			NavigationItem.RightBarButtonItem.Enabled = true;

			NavigationItem.LeftBarButtonItem.Clicked += (object sender, EventArgs e) => {
				NavigationController.PopViewControllerAnimated(true);
			};

			usernameOrEmail.ShouldReturn = delegate {
				password.BecomeFirstResponder();
				return false;
			};
			password.ShouldReturn = delegate {
				SignIn();
				password.ResignFirstResponder();
				return true;
			};

			SetMode (signUp);

			Mode.TouchUpInside += (sender, e) => {
				signUp = !signUp;
				Mode.SetImage(GetModeBackgroundImage(), UIControlState.Normal);
				if(signUp)
				{
					password.AllEditingEvents += PasswordEditingHandler;
					password.ReturnKeyType = UIReturnKeyType.Next;
					password.ShouldReturn = delegate {
						confirmPassword.BecomeFirstResponder();
						return false;
					};

					confirmPassword.AllEditingEvents += PasswordEditingHandler;
					confirmPassword.ShouldReturn = delegate {
						if(password.Text == confirmPassword.Text)
						{
							SignUp();
						}
						return true;
					};

				}
				else
				{
					NavigationItem.RightBarButtonItem.Enabled = true;
					password.ReturnKeyType = UIReturnKeyType.Done;
					password.AllEditingEvents -= PasswordEditingHandler;
					confirmPassword.AllEditingEvents -= PasswordEditingHandler;

					password.ShouldReturn = delegate {
						SignIn();
						return true;
					};
				}
				SetMode(signUp);
			};

			usernameOrEmail.ReturnKeyType = UIReturnKeyType.Next;
			password.ReturnKeyType = UIReturnKeyType.Done;

			password.SecureTextEntry = true;
			confirmPassword.SecureTextEntry = true;

			RectangleF viewRectangleF = BGAppearanceHelper.DeviceType == DeviceType.iPhone4 ?
				new RectangleF (0, 517, 320, 51) : new RectangleF (0, 429, 320, 51);
			UIView bottomView = new UIView (viewRectangleF);
			bottomView.BackgroundColor = UIColor.White;



		}

		public override bool PrefersStatusBarHidden()
		{
			return false;
		}
			
		#endregion

		#region Methods

		private void DoneHandler(object sender, EventArgs args)
		{
			if(signUp)
			{
				SignUp ();
			}
			else
			{
				SignIn ();
			}
		}

		private void CancelHandler(object sender, EventArgs args)
		{

		}

		private void SignIn()
		{
			BlahguaAPIObject.Current.SignIn (usernameOrEmail.Text, password.Text, rememberMe, AuthenticationResultCallback);
		}

		private void SignUp ()
		{
			BlahguaAPIObject.Current.Register (usernameOrEmail.Text, password.Text, rememberMe, AuthenticationResultCallback);
		}

		private void PasswordEditingHandler(object sender, EventArgs e) {
			if(password.Text != confirmPassword.Text || password.Text == String.Empty || password.Text == String.Empty)
			{
				NavigationItem.RightBarButtonItem.Enabled = false;
				confirmPassword.ReturnKeyType = UIReturnKeyType.Default;
			}
			else
			{
				NavigationItem.RightBarButtonItem.Enabled = true;
				confirmPassword.ReturnKeyType = UIReturnKeyType.Done;
			}
		}

		private void SetMode(bool isSignUpMode)
		{
			confirmPassword.Hidden = !isSignUpMode;
			rememberMeLabel.Hidden = !isSignUpMode;
			rememberMeNoButton.Hidden = !isSignUpMode;
			rememberMeYesButton.Hidden = !isSignUpMode;
			yesLabel.Hidden = !isSignUpMode;
			noLabel.Hidden = !isSignUpMode;
		}

		private void AuthenticationResultCallback(string result)
		{
			if (result == null)
			{
				InvokeOnMainThread (() => PerformSegue ("fromLoginToProfile", this));
			}
		}

		private UIImage GetModeBackgroundImage()
		{
			return UIImage.FromFile (signUp ? "signupRadioButton.png" : "signupRadioButtonUn.png");
		}
			

		partial void RememberMeAction (UIButton sender)
		{
			if((sender == rememberMeNoButton && !rememberMe) || (sender == rememberMeYesButton && rememberMe))
			{
				return;
			}

			rememberMe = !rememberMe;

			if(rememberMe)
			{
				rememberMeYesButton.SetImage(UIImage.FromFile("rememberMeRadioButton.png"), UIControlState.Normal);
				rememberMeNoButton.SetImage(UIImage.FromFile("rememberMeRadioButton_un.png"), UIControlState.Normal);
			}
			else
			{
				rememberMeYesButton.SetImage(UIImage.FromFile("rememberMeRadioButton_un.png"), UIControlState.Normal);
				rememberMeNoButton.SetImage(UIImage.FromFile("rememberMeRadioButton.png"), UIControlState.Normal);
			}
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