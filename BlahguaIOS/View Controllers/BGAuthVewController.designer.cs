// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.CodeDom.Compiler;

namespace BlahguaMobile.IOS
{
	[Register ("BGAuthVewController")]
	partial class BGAuthVewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		MonoTouch.UIKit.UIButton aboutButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		BlahguaMobile.IOS.BGTextField confirmPassword { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		MonoTouch.UIKit.UIButton helpButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		MonoTouch.UIKit.UIButton Mode { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		MonoTouch.UIKit.UILabel noLabel { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		BlahguaMobile.IOS.BGTextField password { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		MonoTouch.UIKit.UILabel rememberMeLabel { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		MonoTouch.UIKit.UIButton rememberMeNoButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		MonoTouch.UIKit.UIButton rememberMeYesButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		BlahguaMobile.IOS.BGTextField usernameOrEmail { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		MonoTouch.UIKit.UILabel yesLabel { get; set; }

		[Action ("RememberMeAction:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void RememberMeAction (MonoTouch.UIKit.UIButton sender);

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
		}
	}
}
