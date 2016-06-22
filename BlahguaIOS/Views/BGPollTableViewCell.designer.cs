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
    [Register ("BGPollTableViewCell")]
    partial class BGPollTableViewCell
    {
        [Outlet]
        UIKit.UILabel name { get; set; }


        [Outlet]
        UIKit.UILabel percentage { get; set; }


        [Outlet]
        UIKit.UIProgressView progressView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (name != null) {
                name.Dispose ();
                name = null;
            }

            if (percentage != null) {
                percentage.Dispose ();
                percentage = null;
            }

            if (progressView != null) {
                progressView.Dispose ();
                progressView = null;
            }
        }
    }
}