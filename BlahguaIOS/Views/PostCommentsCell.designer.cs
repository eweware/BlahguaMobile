// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace Eweware
{
	[Register ("PostCommentsCell")]
	partial class PostCommentsCell
	{
		[Outlet]
		MonoTouch.UIKit.UIImageView commentImageView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextView commentTextView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel commentTimeLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel commentUserNameLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (commentTextView != null) {
				commentTextView.Dispose ();
				commentTextView = null;
			}

			if (commentTimeLabel != null) {
				commentTimeLabel.Dispose ();
				commentTimeLabel = null;
			}

			if (commentUserNameLabel != null) {
				commentUserNameLabel.Dispose ();
				commentUserNameLabel = null;
			}

			if (commentImageView != null) {
				commentImageView.Dispose ();
				commentImageView = null;
			}
		}
	}
}
