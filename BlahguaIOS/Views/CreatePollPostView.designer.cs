// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace TestApp
{
	partial class CreatePollPostView
	{
		[Outlet]
		MonoTouch.UIKit.UITextView postBodyTextView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton postDoneButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextField postHeadlineTextfield { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView postPollView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton postSelectImageButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton postSelectSignatureButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView postView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (postBodyTextView != null) {
				postBodyTextView.Dispose ();
				postBodyTextView = null;
			}

			if (postDoneButton != null) {
				postDoneButton.Dispose ();
				postDoneButton = null;
			}

			if (postHeadlineTextfield != null) {
				postHeadlineTextfield.Dispose ();
				postHeadlineTextfield = null;
			}

			if (postPollView != null) {
				postPollView.Dispose ();
				postPollView = null;
			}

			if (postSelectImageButton != null) {
				postSelectImageButton.Dispose ();
				postSelectImageButton = null;
			}

			if (postSelectSignatureButton != null) {
				postSelectSignatureButton.Dispose ();
				postSelectSignatureButton = null;
			}

			if (postView != null) {
				postView.Dispose ();
				postView = null;
			}
		}
	}
}
