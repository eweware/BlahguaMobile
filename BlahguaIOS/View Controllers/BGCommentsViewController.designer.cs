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
	[Register ("BGCommentsViewController")]
	partial class BGCommentsViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		MonoTouch.UIKit.UIToolbar bottomToolbar { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		MonoTouch.UIKit.UIBarButtonItem commentsView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		MonoTouch.UIKit.UITableView contentView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		MonoTouch.UIKit.UIBarButtonItem downVote { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		MonoTouch.UIKit.UIBarButtonItem statsView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		MonoTouch.UIKit.UIBarButtonItem summaryView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		MonoTouch.UIKit.UIBarButtonItem upVote { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		MonoTouch.UIKit.UILabel viewTitle { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (bottomToolbar != null) {
				bottomToolbar.Dispose ();
				bottomToolbar = null;
			}
			if (commentsView != null) {
				commentsView.Dispose ();
				commentsView = null;
			}
			if (contentView != null) {
				contentView.Dispose ();
				contentView = null;
			}
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
			if (viewTitle != null) {
				viewTitle.Dispose ();
				viewTitle = null;
			}
		}
	}
}
