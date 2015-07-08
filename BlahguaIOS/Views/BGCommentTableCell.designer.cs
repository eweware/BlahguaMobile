// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace BlahguaMobile.IOS
{
	[Register ("BGCommentTableCell")]
	partial class BGCommentTableCell
	{
		[Outlet]
		UIKit.UILabel author { get; set; }

		[Outlet]
		UIKit.UITableView badgeTable { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint badgeTableHeight { get; set; }

		[Outlet]
		UIKit.UIImageView commentImageView { get; set; }

		[Outlet]
		UIKit.UIView containerView { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint containerViewWidth { get; set; }

		[Outlet]
		UIKit.UIButton downVoteButton { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint imageViewHeight { get; set; }

		[Outlet]
		UIKit.UIImageView imgAvatar { get; set; }

		[Outlet]
		UIKit.UILabel lblUserType { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint rightPosition { get; set; }

		[Outlet]
		UIKit.UITextView text { get; set; }

		[Outlet]
		UIKit.UILabel timespan { get; set; }

		[Outlet]
		UIKit.UILabel upAndDownVotes { get; set; }

		[Outlet]
		UIKit.UIButton upVoteButton { get; set; }

		[Outlet]
		UIKit.UIView voteView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		NSLayoutConstraint LeftEdgeConstraint { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (LeftEdgeConstraint != null) {
				LeftEdgeConstraint.Dispose ();
				LeftEdgeConstraint = null;
			}
		}
	}
}
