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

		private bool rememberMe = true;
		private bool signUp = false;

		private MFMailComposeViewController mailComposer;

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

			View.BackgroundColor = UIColor.FromPatternImage (UIImage.FromBundle("grayBack"));

			NavigationItem.RightBarButtonItem = new UIBarButtonItem ("Done", UIBarButtonItemStyle.Plain, DoneHandler);
			NavigationItem.RightBarButtonItem.Enabled = false;
            //Synsoft on 9 July 2014 for inactive color  #bcbcbc
            NavigationItem.RightBarButtonItem.TintColor = UIColor.FromRGB(188, 188, 188);
			NavigationItem.LeftBarButtonItem = new UIBarButtonItem ("Cancel", UIBarButtonItemStyle.Plain, CancelHandler);
            //Synsoft on 9 July 2014 for active color  #1FBBD1
            NavigationItem.LeftBarButtonItem.TintColor = UIColor.FromRGB(31, 187, 209);

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
			password.ShouldReturn = delegate {
				return true;
			};
			password.AllEditingEvents += InputsEditingHandler;

			confirmPassword.AttributedPlaceholder = new NSAttributedString (
				"Confirm Password", 
				placeholderAttrs
			);
			confirmPassword.AttributedText = new NSAttributedString ("", textAttrs);
			confirmPassword.Background = UIImage.FromBundle ("input_back");
			confirmPassword.AllEditingEvents += InputsEditingHandler;

			SetMode (signUp);

			Mode.TouchUpInside += (sender, e) => {
				signUp = !signUp;
				Mode.SetImage(GetModeBackgroundImage(), UIControlState.Normal);
				if(signUp)
				{

					password.ReturnKeyType = UIReturnKeyType.Next;
					password.ShouldReturn = delegate {
						confirmPassword.BecomeFirstResponder();
						return false;
					};

					confirmPassword.AllEditingEvents += InputsEditingHandler;

					confirmPassword.ShouldReturn = delegate {
						return true;
					};
					confirmPassword.ReturnKeyType = UIReturnKeyType.Default;
				}
				else
				{

					password.ReturnKeyType = UIReturnKeyType.Default;

					password.ShouldReturn = delegate {
						return true;
					};
				}
				SetMode(signUp);
			};

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
			aboutButton.Enabled = true;
			aboutButton.TouchUpInside += (object sender, EventArgs e) => {
				var url = NSUrl.FromString("http://www.goheard.com/"); 
				if(UIApplication.SharedApplication.CanOpenUrl(url))
				{
					UIApplication.SharedApplication.OpenUrl(url);
				}
			};
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
            NavigationController.PopViewControllerAnimated(true);
		}

		private void SignIn()
		{
			BlahguaAPIObject.Current.SignIn (usernameOrEmail.Text, password.Text, rememberMe, AuthenticationResultCallback);
		}

		private void SignUp ()
		{
			BlahguaAPIObject.Current.Register (usernameOrEmail.Text, password.Text, rememberMe, AuthenticationResultCallback);
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
                    NavigationItem.RightBarButtonItem.TintColor = UIColor.FromRGB(31, 187, 209);
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
                    NavigationItem.RightBarButtonItem.TintColor = UIColor.FromRGB(31, 187, 209);
				}
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
				InvokeOnMainThread (() => NavigationController.PopViewControllerAnimated(true));
			}
			else
			{
				InvokeOnMainThread (() => NavigationItem.RightBarButtonItem.Enabled = true );
			}
		}

		private UIImage GetModeBackgroundImage()
		{
			return UIImage.FromBundle (signUp ? "signupRadioButton" : "signupRadioButtonUn");
		}
			

		 void RememberMeAction (UIButton sender)
		{
			if((sender == rememberMeNoButton && !rememberMe) || (sender == rememberMeYesButton && rememberMe))
			{
				return;
			}

			rememberMe = !rememberMe;

			if(rememberMe)
			{
				rememberMeYesButton.SetImage(UIImage.FromBundle("rememberMeRadioButton"), UIControlState.Normal);
                rememberMeNoButton.SetImage(UIImage.FromBundle("rememberMeRadioButton_un"), UIControlState.Normal);
			}
			else
			{
                rememberMeYesButton.SetImage(UIImage.FromBundle("rememberMeRadioButton_un"), UIControlState.Normal);
                rememberMeNoButton.SetImage(UIImage.FromBundle("rememberMeRadioButton"), UIControlState.Normal);
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