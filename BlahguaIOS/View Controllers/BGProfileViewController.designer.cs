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
	[Register ("BGProfileViewController")]
	partial class BGProfileViewController
	{
		[Outlet]
		MonoTouch.UIKit.UITextField nicknameTextField { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView profileImageView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView profileView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton selectImage { get; set; }
		
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

			if (profileImageView != null) {
				profileImageView.Dispose ();
				profileImageView = null;
			}
		}
	}
}
