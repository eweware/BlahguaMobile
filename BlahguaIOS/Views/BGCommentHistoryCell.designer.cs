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
	[Register ("BGCommentHistoryCell")]
	partial class BGCommentHistoryCell
	{
		[Outlet]
		MonoTouch.UIKit.UILabel CommentDownVotesLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel CommentElapsedTimeLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel CommentText { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel CommentUpVotesLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (CommentText != null) {
				CommentText.Dispose ();
				CommentText = null;
			}

			if (CommentUpVotesLabel != null) {
				CommentUpVotesLabel.Dispose ();
				CommentUpVotesLabel = null;
			}

			if (CommentDownVotesLabel != null) {
				CommentDownVotesLabel.Dispose ();
				CommentDownVotesLabel = null;
			}

			if (CommentElapsedTimeLabel != null) {
				CommentElapsedTimeLabel.Dispose ();
				CommentElapsedTimeLabel = null;
			}
		}
	}
}
