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
		UIButton entertainmentBtn { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton publicBtn { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIView SignUpPage02View { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton techBtn { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (entertainmentBtn != null) {
				entertainmentBtn.Dispose ();
				entertainmentBtn = null;
			}
			if (publicBtn != null) {
				publicBtn.Dispose ();
				publicBtn = null;
			}
			if (SignUpPage02View != null) {
				SignUpPage02View.Dispose ();
				SignUpPage02View = null;
			}
			if (techBtn != null) {
				techBtn.Dispose ();
				techBtn = null;
			}
		}
	}
}
