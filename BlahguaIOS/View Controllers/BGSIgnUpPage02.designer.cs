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
    [Register ("BGSignUpPage02")]
    partial class BGSignUpPage02
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton CommunityChannelBtn { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton PublisherChannelBtn { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView SignUpPage02View { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (CommunityChannelBtn != null) {
                CommunityChannelBtn.Dispose ();
                CommunityChannelBtn = null;
            }

            if (PublisherChannelBtn != null) {
                PublisherChannelBtn.Dispose ();
                PublisherChannelBtn = null;
            }

            if (SignUpPage02View != null) {
                SignUpPage02View.Dispose ();
                SignUpPage02View = null;
            }
        }
    }
}