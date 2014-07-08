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
	[Register ("BGCommentTableCell")]
	partial class BGCommentTableCell
	{
		[Outlet]
		MonoTouch.UIKit.UILabel author { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView commentImageView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView containerView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton downVoteButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.NSLayoutConstraint rightPosition { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextView text { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel timespan { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel upAndDownVotes { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton upVoteButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView voteView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (author != null) {
				author.Dispose ();
				author = null;
			}

			if (commentImageView != null) {
				commentImageView.Dispose ();
				commentImageView = null;
			}

			if (containerView != null) {
				containerView.Dispose ();
				containerView = null;
			}

			if (downVoteButton != null) {
				downVoteButton.Dispose ();
				downVoteButton = null;
			}

			if (rightPosition != null) {
				rightPosition.Dispose ();
				rightPosition = null;
			}

			if (text != null) {
				text.Dispose ();
				text = null;
			}

			if (timespan != null) {
				timespan.Dispose ();
				timespan = null;
			}

			if (upAndDownVotes != null) {
				upAndDownVotes.Dispose ();
				upAndDownVotes = null;
			}

			if (upVoteButton != null) {
				upVoteButton.Dispose ();
				upVoteButton = null;
			}

			if (voteView != null) {
				voteView.Dispose ();
				voteView = null;
			}
		}
	}
}
