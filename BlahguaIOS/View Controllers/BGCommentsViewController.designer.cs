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
	[Register ("BGCommentsViewController")]
	partial class BGCommentsViewController
	{
		[Outlet]
		MonoTouch.UIKit.UIToolbar bottomToolbar { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem commentsView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITableView contentView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem downVote { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem statsView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem summaryView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem upVote { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel viewTitle { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (downVote != null) {
				downVote.Dispose ();
				downVote = null;
			}

			if (statsView != null) {
				statsView.Dispose ();
				statsView = null;
			}

			if (summaryView != null) {
				summaryView.Dispose ();
				summaryView = null;
			}

			if (upVote != null) {
				upVote.Dispose ();
				upVote = null;
			}

			if (bottomToolbar != null) {
				bottomToolbar.Dispose ();
				bottomToolbar = null;
			}

			if (commentsView != null) {
				commentsView.Dispose ();
				commentsView = null;
			}

			if (viewTitle != null) {
				viewTitle.Dispose ();
				viewTitle = null;
			}

			if (contentView != null) {
				contentView.Dispose ();
				contentView = null;
			}
		}
	}
}
