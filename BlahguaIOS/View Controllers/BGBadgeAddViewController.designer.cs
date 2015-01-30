// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System;
using System.CodeDom.Compiler;

namespace BlahguaMobile.IOS
{
	[Register ("BGBadgeAddViewController")]
	partial class BGBadgeAddViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIView BadgeRequestView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIView BadgeSubmitView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIView BadgeVerifyView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton doneButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		BGTextField emailTextField { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel infoTitle { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel labelPrivacyNotice { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel labelRequestText { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel labelVerifyText { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton requestButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton verifyButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextField verifyCodeTextField { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (BadgeRequestView != null) {
				BadgeRequestView.Dispose ();
				BadgeRequestView = null;
			}
			if (BadgeSubmitView != null) {
				BadgeSubmitView.Dispose ();
				BadgeSubmitView = null;
			}
			if (BadgeVerifyView != null) {
				BadgeVerifyView.Dispose ();
				BadgeVerifyView = null;
			}
			if (doneButton != null) {
				doneButton.Dispose ();
				doneButton = null;
			}
			if (emailTextField != null) {
				emailTextField.Dispose ();
				emailTextField = null;
			}
			if (infoTitle != null) {
				infoTitle.Dispose ();
				infoTitle = null;
			}
			if (labelPrivacyNotice != null) {
				labelPrivacyNotice.Dispose ();
				labelPrivacyNotice = null;
			}
			if (labelRequestText != null) {
				labelRequestText.Dispose ();
				labelRequestText = null;
			}
			if (labelVerifyText != null) {
				labelVerifyText.Dispose ();
				labelVerifyText = null;
			}
			if (requestButton != null) {
				requestButton.Dispose ();
				requestButton = null;
			}
			if (verifyButton != null) {
				verifyButton.Dispose ();
				verifyButton = null;
			}
			if (verifyCodeTextField != null) {
				verifyCodeTextField.Dispose ();
				verifyCodeTextField = null;
			}
		}
	}
}
