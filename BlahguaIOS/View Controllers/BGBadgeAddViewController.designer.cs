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
		UIButton doneButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		BGTextField emailTextField { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel infoTitle { get; set; }

		void ReleaseDesignerOutlets ()
		{
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
		}
	}
}
