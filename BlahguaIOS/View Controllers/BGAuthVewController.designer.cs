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
		MonoTouch.UIKit.UIScrollView authScroller { get; set; }

		[Outlet]
		BlahguaMobile.IOS.BGTextField confirmPassword { get; set; }

		[Outlet]
		MonoTouch.UIKit.UISwitch createBtn { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton helpButton { get; set; }

		[Outlet]
		BlahguaMobile.IOS.BGTextField password { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton recoverAccountBtn { get; set; }

		[Outlet]
		BlahguaMobile.IOS.BGTextField recoveryEmail { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton reportBugButton { get; set; }

		[Outlet]
		BlahguaMobile.IOS.BGTextField usernameOrEmail { get; set; }
		
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

			if (recoverAccountBtn != null) {
				recoverAccountBtn.Dispose ();
				recoverAccountBtn = null;
			}
		}
	}
}
