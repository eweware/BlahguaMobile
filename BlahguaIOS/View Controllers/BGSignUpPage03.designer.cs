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
    [Register ("BGSignUpPage03")]
    partial class BGSignUpPage03
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel badgeReceivedLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton checkBtn { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField emailAddrField { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton noCodeBtn { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel preBadgeLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIScrollView Scroller { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView SiginUpPage03View { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton skipBtn { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField verificationField { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton verifyBtn { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (badgeReceivedLabel != null) {
                badgeReceivedLabel.Dispose ();
                badgeReceivedLabel = null;
            }

            if (checkBtn != null) {
                checkBtn.Dispose ();
                checkBtn = null;
            }

            if (emailAddrField != null) {
                emailAddrField.Dispose ();
                emailAddrField = null;
            }

            if (noCodeBtn != null) {
                noCodeBtn.Dispose ();
                noCodeBtn = null;
            }

            if (preBadgeLabel != null) {
                preBadgeLabel.Dispose ();
                preBadgeLabel = null;
            }

            if (Scroller != null) {
                Scroller.Dispose ();
                Scroller = null;
            }

            if (SiginUpPage03View != null) {
                SiginUpPage03View.Dispose ();
                SiginUpPage03View = null;
            }

            if (skipBtn != null) {
                skipBtn.Dispose ();
                skipBtn = null;
            }

            if (verificationField != null) {
                verificationField.Dispose ();
                verificationField = null;
            }

            if (verifyBtn != null) {
                verifyBtn.Dispose ();
                verifyBtn = null;
            }
        }
    }
}