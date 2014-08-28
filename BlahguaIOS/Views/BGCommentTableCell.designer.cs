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
		MonoTouch.UIKit.UITableView badgeTable { get; set; }

		[Outlet]
		MonoTouch.UIKit.NSLayoutConstraint badgeTableHeight { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView commentImageView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView containerView { get; set; }

		[Outlet]
		MonoTouch.UIKit.NSLayoutConstraint containerViewWidth { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton downVoteButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.NSLayoutConstraint imageViewHeight { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView imgAvatar { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblUserType { get; set; }

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
			if (badgeTableHeight != null) {
				badgeTableHeight.Dispose ();
				badgeTableHeight = null;
			}

			if (author != null) {
				author.Dispose ();
				author = null;
			}

			if (badgeTable != null) {
				badgeTable.Dispose ();
				badgeTable = null;
			}

			if (commentImageView != null) {
				commentImageView.Dispose ();
				commentImageView = null;
			}

			if (containerView != null) {
				containerView.Dispose ();
				containerView = null;
			}

			if (containerViewWidth != null) {
				containerViewWidth.Dispose ();
				containerViewWidth = null;
			}

			if (downVoteButton != null) {
				downVoteButton.Dispose ();
				downVoteButton = null;
			}

			if (imageViewHeight != null) {
				imageViewHeight.Dispose ();
				imageViewHeight = null;
			}

			if (imgAvatar != null) {
				imgAvatar.Dispose ();
				imgAvatar = null;
			}

			if (lblUserType != null) {
				lblUserType.Dispose ();
				lblUserType = null;
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
