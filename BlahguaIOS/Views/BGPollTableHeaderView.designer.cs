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
    [Register ("BGPollTableHeaderView")]
    partial class BGPollTableHeaderView
    {
        [Outlet]
        UIKit.UILabel headerText { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (headerText != null) {
                headerText.Dispose ();
                headerText = null;
            }
        }
    }
}