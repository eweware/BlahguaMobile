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
	[Register ("BGBadgeAddViewController")]
	partial class BGBadgeAddViewController
	{
		[Outlet]
		MonoTouch.UIKit.UIButton doneButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextField emailTextField { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel infoTitle { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (infoTitle != null) {
				infoTitle.Dispose ();
				infoTitle = null;
			}

			if (emailTextField != null) {
				emailTextField.Dispose ();
				emailTextField = null;
			}

			if (doneButton != null) {
				doneButton.Dispose ();
				doneButton = null;
			}
		}
	}
}
