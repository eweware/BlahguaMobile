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
		UIScrollView authScroller { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		BGTextField confirmPassword { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UISwitch createBtn { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton helpButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		BGTextField password { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		BGTextField recoveryEmail { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton reportBugButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		BGTextField usernameOrEmail { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (aboutButton != null) {
				aboutButton.Dispose ();
				aboutButton = null;
			}
			if (authScroller != null) {
				authScroller.Dispose ();
				authScroller = null;
			}
			if (confirmPassword != null) {
				confirmPassword.Dispose ();
				confirmPassword = null;
			}
			if (createBtn != null) {
				createBtn.Dispose ();
				createBtn = null;
			}
			if (helpButton != null) {
				helpButton.Dispose ();
				helpButton = null;
			}
			if (password != null) {
				password.Dispose ();
				password = null;
			}
			if (recoveryEmail != null) {
				recoveryEmail.Dispose ();
				recoveryEmail = null;
			}
			if (reportBugButton != null) {
				reportBugButton.Dispose ();
				reportBugButton = null;
			}
			if (usernameOrEmail != null) {
				usernameOrEmail.Dispose ();
				usernameOrEmail = null;
			}
		}
	}
}
