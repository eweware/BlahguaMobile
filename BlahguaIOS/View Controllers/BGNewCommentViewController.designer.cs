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
	[Register ("BGNewCommentViewController")]
	partial class BGNewCommentViewController
	{
		[Outlet]
		MonoTouch.UIKit.UIImageView back { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton done { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextField input { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton selectImageButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton selectSignature { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
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

			if (done != null) {
				done.Dispose ();
				done = null;
			}

			if (back != null) {
				back.Dispose ();
				back = null;
			}
		}
	}
}
