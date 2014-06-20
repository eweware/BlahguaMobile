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
	[Register ("BGBlahViewController")]
	partial class BGBlahViewController
	{
		[Outlet]
		MonoTouch.UIKit.UILabel author { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView badgeImage { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIToolbar bottomToolbar { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem commentsView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIScrollView contentView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem downVote { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem statsView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem summaryView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem upVote { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView userImage { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (author != null) {
				author.Dispose ();
				author = null;
			}

			if (badgeImage != null) {
				badgeImage.Dispose ();
				badgeImage = null;
			}

			if (bottomToolbar != null) {
				bottomToolbar.Dispose ();
				bottomToolbar = null;
			}

			if (contentView != null) {
				contentView.Dispose ();
				contentView = null;
			}

			if (userImage != null) {
				userImage.Dispose ();
				userImage = null;
			}

			if (upVote != null) {
				upVote.Dispose ();
				upVote = null;
			}

			if (downVote != null) {
				downVote.Dispose ();
				downVote = null;
			}

			if (summaryView != null) {
				summaryView.Dispose ();
				summaryView = null;
			}

			if (commentsView != null) {
				commentsView.Dispose ();
				commentsView = null;
			}

			if (statsView != null) {
				statsView.Dispose ();
				statsView = null;
			}
		}
	}
}
