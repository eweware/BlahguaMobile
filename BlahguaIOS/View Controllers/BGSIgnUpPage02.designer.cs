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
	[Register ("BGSignUpPage02")]
	partial class BGSignUpPage02
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITableView IndustryTable { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton publicBtn { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITableView PublishersTable { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIView SignUpPage02View { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (IndustryTable != null) {
				IndustryTable.Dispose ();
				IndustryTable = null;
			}
			if (publicBtn != null) {
				publicBtn.Dispose ();
				publicBtn = null;
			}
			if (PublishersTable != null) {
				PublishersTable.Dispose ();
				PublishersTable = null;
			}
			if (SignUpPage02View != null) {
				SignUpPage02View.Dispose ();
				SignUpPage02View = null;
			}
		}
	}
}
