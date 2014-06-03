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
	[Register ("BGProfileViewController")]
	partial class BGProfileViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextField nicknameTextField { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIView profileView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton selectImage { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (nicknameTextField != null) {
				nicknameTextField.Dispose ();
				nicknameTextField = null;
			}
			if (profileView != null) {
				profileView.Dispose ();
				profileView = null;
			}
			if (selectImage != null) {
				selectImage.Dispose ();
				selectImage = null;
			}
		}
	}
}
