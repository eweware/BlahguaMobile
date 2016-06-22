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
    [Register ("BGSignUpPage01")]
    partial class BGSignUpPage01
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField confirmPassword { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton createAccountBtn { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField emailField { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField passwordField { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton PrepSignIn { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel recoveryLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIScrollView Scroller { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton signInBtn { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView SignupPage01View { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton skipButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField usernameField { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (confirmPassword != null) {
                confirmPassword.Dispose ();
                confirmPassword = null;
            }

            if (createAccountBtn != null) {
                createAccountBtn.Dispose ();
                createAccountBtn = null;
            }

            if (emailField != null) {
                emailField.Dispose ();
                emailField = null;
            }

            if (passwordField != null) {
                passwordField.Dispose ();
                passwordField = null;
            }

            if (PrepSignIn != null) {
                PrepSignIn.Dispose ();
                PrepSignIn = null;
            }

            if (recoveryLabel != null) {
                recoveryLabel.Dispose ();
                recoveryLabel = null;
            }

            if (Scroller != null) {
                Scroller.Dispose ();
                Scroller = null;
            }

            if (signInBtn != null) {
                signInBtn.Dispose ();
                signInBtn = null;
            }

            if (SignupPage01View != null) {
                SignupPage01View.Dispose ();
                SignupPage01View = null;
            }

            if (skipButton != null) {
                skipButton.Dispose ();
                skipButton = null;
            }

            if (usernameField != null) {
                usernameField.Dispose ();
                usernameField = null;
            }
        }
    }
}