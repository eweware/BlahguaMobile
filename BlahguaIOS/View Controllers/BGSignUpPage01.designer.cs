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
	[Register ("BGSignUpPage01")]
	partial class BGSignUpPage01
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextField confirmPassword { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton createAccountBtn { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextField emailField { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextField passwordField { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton signInBtn { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIView SignupPage01View { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton skipButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextField usernameField { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (confirmPassword != null) {
				confirmPassword.Dispose ();
				confirmPassword = null;
			}
			if (createAccountBtn != null) {
				createAccountBtn.Dispose ();
				createAccountBtn = null;
			}
			if (emailField != null) {
				emailField.Dispose ();
				emailField = null;
			}
			if (passwordField != null) {
				passwordField.Dispose ();
				passwordField = null;
			}
			if (signInBtn != null) {
				signInBtn.Dispose ();
				signInBtn = null;
			}
			if (SignupPage01View != null) {
				SignupPage01View.Dispose ();
				SignupPage01View = null;
			}
			if (skipButton != null) {
				skipButton.Dispose ();
				skipButton = null;
			}
			if (usernameField != null) {
				usernameField.Dispose ();
				usernameField = null;
			}
		}
	}
}
