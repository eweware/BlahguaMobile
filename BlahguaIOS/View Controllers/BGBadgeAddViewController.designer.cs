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
		UILabel labelPrivacy { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel labelRequestBadge { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel labelVerifyCodeText { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton RequestBadgeBtn { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton verifyCodeBtn { get; set; }

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
			if (labelPrivacy != null) {
				labelPrivacy.Dispose ();
				labelPrivacy = null;
			}
			if (labelRequestBadge != null) {
				labelRequestBadge.Dispose ();
				labelRequestBadge = null;
			}
			if (labelVerifyCodeText != null) {
				labelVerifyCodeText.Dispose ();
				labelVerifyCodeText = null;
			}
			if (RequestBadgeBtn != null) {
				RequestBadgeBtn.Dispose ();
				RequestBadgeBtn = null;
			}
			if (verifyCodeBtn != null) {
				verifyCodeBtn.Dispose ();
				verifyCodeBtn = null;
			}
			if (verifyCodeTextField != null) {
				verifyCodeTextField.Dispose ();
				verifyCodeTextField = null;
			}
		}
	}
}
