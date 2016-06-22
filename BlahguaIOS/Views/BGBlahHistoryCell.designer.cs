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
    [Register ("BGBlahHistoryCell")]
    partial class BGBlahHistoryCell
    {
        [Outlet]
        UIKit.UILabel BlahCommentsLabel { get; set; }


        [Outlet]
        UIKit.UILabel BlahConversionLabel { get; set; }


        [Outlet]
        UIKit.UILabel BlahDownVotesLabel { get; set; }


        [Outlet]
        UIKit.UIImageView BlahImageView { get; set; }


        [Outlet]
        UIKit.UILabel BlahTimeLabel { get; set; }


        [Outlet]
        UIKit.UILabel BlahTitleLabel { get; set; }


        [Outlet]
        UIKit.UIImageView BlahTypeImage { get; set; }


        [Outlet]
        UIKit.UILabel BlahUpVotesLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (BlahCommentsLabel != null) {
                BlahCommentsLabel.Dispose ();
                BlahCommentsLabel = null;
            }

            if (BlahConversionLabel != null) {
                BlahConversionLabel.Dispose ();
                BlahConversionLabel = null;
            }

            if (BlahDownVotesLabel != null) {
                BlahDownVotesLabel.Dispose ();
                BlahDownVotesLabel = null;
            }

            if (BlahTimeLabel != null) {
                BlahTimeLabel.Dispose ();
                BlahTimeLabel = null;
            }

            if (BlahTitleLabel != null) {
                BlahTitleLabel.Dispose ();
                BlahTitleLabel = null;
            }

            if (BlahTypeImage != null) {
                BlahTypeImage.Dispose ();
                BlahTypeImage = null;
            }

            if (BlahUpVotesLabel != null) {
                BlahUpVotesLabel.Dispose ();
                BlahUpVotesLabel = null;
            }
        }
    }
}