// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace BlahguaMobile.IOS
{
	[Register ("BGAuthVewController")]
	partial class BGAuthVewController
	{
		[Outlet]
		MonoTouch.UIKit.UIButton aboutButton { get; set; }

		[Outlet]
		BlahguaMobile.IOS.BGTextField confirmPassword { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton helpButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton Mode { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel noLabel { get; set; }

		[Outlet]
		BlahguaMobile.IOS.BGTextField password { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel rememberMeLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton rememberMeNoButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton rememberMeYesButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton reportBugButton { get; set; }

		[Outlet]
		BlahguaMobile.IOS.BGTextField usernameOrEmail { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel yesLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (aboutButton != null) {
				aboutButton.Dispose ();
				aboutButton = null;
			}

			if (confirmPassword != null) {
				confirmPassword.Dispose ();
				confirmPassword = null;
			}

			if (helpButton != null) {
				helpButton.Dispose ();
				helpButton = null;
			}

			if (Mode != null) {
				Mode.Dispose ();
				Mode = null;
			}

			if (noLabel != null) {
				noLabel.Dispose ();
				noLabel = null;
			}

			if (password != null) {
				password.Dispose ();
				password = null;
			}

			if (rememberMeLabel != null) {
				rememberMeLabel.Dispose ();
				rememberMeLabel = null;
			}

			if (rememberMeNoButton != null) {
				rememberMeNoButton.Dispose ();
				rememberMeNoButton = null;
			}

			if (rememberMeYesButton != null) {
				rememberMeYesButton.Dispose ();
				rememberMeYesButton = null;
			}

			if (usernameOrEmail != null) {
				usernameOrEmail.Dispose ();
				usernameOrEmail = null;
			}

			if (yesLabel != null) {
				yesLabel.Dispose ();
				yesLabel = null;
			}

			if (reportBugButton != null) {
				reportBugButton.Dispose ();
				reportBugButton = null;
			}
		}
	}
}
