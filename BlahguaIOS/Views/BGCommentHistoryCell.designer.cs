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
    [Register ("BGCommentHistoryCell")]
    partial class BGCommentHistoryCell
    {
        [Outlet]
        UIKit.UILabel CommentDownVotesLabel { get; set; }


        [Outlet]
        UIKit.UILabel CommentElapsedTimeLabel { get; set; }


        [Outlet]
        UIKit.UILabel CommentText { get; set; }


        [Outlet]
        UIKit.UILabel CommentUpVotesLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (CommentDownVotesLabel != null) {
                CommentDownVotesLabel.Dispose ();
                CommentDownVotesLabel = null;
            }

            if (CommentElapsedTimeLabel != null) {
                CommentElapsedTimeLabel.Dispose ();
                CommentElapsedTimeLabel = null;
            }

            if (CommentText != null) {
                CommentText.Dispose ();
                CommentText = null;
            }

            if (CommentUpVotesLabel != null) {
                CommentUpVotesLabel.Dispose ();
                CommentUpVotesLabel = null;
            }
        }
    }
}