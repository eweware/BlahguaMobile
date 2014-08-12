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
		UIButton aboutButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton btnCreateAccount { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		BGTextField confirmPassword { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton helpButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton Mode { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel noLabel { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		BGTextField password { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		BGTextField recoveryEmail { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel rememberMeLabel { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton rememberMeNoButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton rememberMeYesButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton reportBugButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		BGTextField usernameOrEmail { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel yesLabel { get; set; }

		[Action ("RememberMeAction:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void RememberMeAction (UIButton sender);

		void ReleaseDesignerOutlets ()
		{
			if (aboutButton != null) {
				aboutButton.Dispose ();
				aboutButton = null;
			}
			if (btnCreateAccount != null) {
				btnCreateAccount.Dispose ();
				btnCreateAccount = null;
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
			if (recoveryEmail != null) {
				recoveryEmail.Dispose ();
				recoveryEmail = null;
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
			if (reportBugButton != null) {
				reportBugButton.Dispose ();
				reportBugButton = null;
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
