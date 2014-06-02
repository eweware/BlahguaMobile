// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace Eweware
{
	[Register ("LoginViewController")]
	partial class LoginViewController
	{
		[Outlet]
		MonoTouch.UIKit.UIButton loginAboutButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextField loginConfirmPasswordTextfield { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton loginCreateAccountButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel loginCreateAccountLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextField loginEmailTextfield { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton loginHelpButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextField loginPasswordTextfield { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel loginRememberMeLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton loginRememberMeNoButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel loginRememberMeNoLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton loginRememberMeYesButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel loginRememberMeYesLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView view { get; set; }

		[Action ("loginAboutButtonPressed:")]
		partial void loginAboutButtonPressed (MonoTouch.Foundation.NSObject sender);

		[Action ("loginCreateAccountButtonPressed:")]
		partial void loginCreateAccountButtonPressed (MonoTouch.Foundation.NSObject sender);

		[Action ("loginHelpButtonPressed:")]
		partial void loginHelpButtonPressed (MonoTouch.Foundation.NSObject sender);

		[Action ("loginRememberMeNoButtonPressed:")]
		partial void loginRememberMeNoButtonPressed (MonoTouch.Foundation.NSObject sender);

		[Action ("loginRememberMeYesButtonPressed:")]
		partial void loginRememberMeYesButtonPressed (MonoTouch.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (view != null) {
				view.Dispose ();
				view = null;
			}

			if (loginCreateAccountButton != null) {
				loginCreateAccountButton.Dispose ();
				loginCreateAccountButton = null;
			}

			if (loginCreateAccountLabel != null) {
				loginCreateAccountLabel.Dispose ();
				loginCreateAccountLabel = null;
			}

			if (loginEmailTextfield != null) {
				loginEmailTextfield.Dispose ();
				loginEmailTextfield = null;
			}

			if (loginPasswordTextfield != null) {
				loginPasswordTextfield.Dispose ();
				loginPasswordTextfield = null;
			}

			if (loginConfirmPasswordTextfield != null) {
				loginConfirmPasswordTextfield.Dispose ();
				loginConfirmPasswordTextfield = null;
			}

			if (loginRememberMeLabel != null) {
				loginRememberMeLabel.Dispose ();
				loginRememberMeLabel = null;
			}

			if (loginRememberMeYesButton != null) {
				loginRememberMeYesButton.Dispose ();
				loginRememberMeYesButton = null;
			}

			if (loginRememberMeYesLabel != null) {
				loginRememberMeYesLabel.Dispose ();
				loginRememberMeYesLabel = null;
			}

			if (loginRememberMeNoButton != null) {
				loginRememberMeNoButton.Dispose ();
				loginRememberMeNoButton = null;
			}

			if (loginRememberMeNoLabel != null) {
				loginRememberMeNoLabel.Dispose ();
				loginRememberMeNoLabel = null;
			}

			if (loginHelpButton != null) {
				loginHelpButton.Dispose ();
				loginHelpButton = null;
			}

			if (loginAboutButton != null) {
				loginAboutButton.Dispose ();
				loginAboutButton = null;
			}
		}
	}
}
