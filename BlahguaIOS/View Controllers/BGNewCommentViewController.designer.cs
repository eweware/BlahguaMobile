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
	[Register ("BGNewCommentViewController")]
	partial class BGNewCommentViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIImageView back { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton done { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextField input { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton selectImageButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton selectSignature { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (back != null) {
				back.Dispose ();
				back = null;
			}
			if (done != null) {
				done.Dispose ();
				done = null;
			}
			if (input != null) {
				input.Dispose ();
				input = null;
			}
			if (selectImageButton != null) {
				selectImageButton.Dispose ();
				selectImageButton = null;
			}
			if (selectSignature != null) {
				selectSignature.Dispose ();
				selectSignature = null;
			}
		}
	}
}
