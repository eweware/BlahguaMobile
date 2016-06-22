using System;
using Foundation;
using UIKit;
using System.CodeDom.Compiler;
using CoreGraphics;

using BlahguaMobile.BlahguaCore;

namespace BlahguaMobile.IOS
{
	partial class BGSignUpPage01 : UIViewController
	{
        UITextField activeField = null;
        NSObject hideObserver;
        NSObject showObserver;
        UIActivityIndicatorView indicator = null;

		public BGSignUpPage01 (IntPtr handle) : base (handle)
		{

		}

        private void AddObservers()
        {
            hideObserver = NSNotificationCenter.DefaultCenter.AddObserver (UIKeyboard.WillHideNotification, OnKeyboardHideNotification);
            showObserver = NSNotificationCenter.DefaultCenter.AddObserver (UIKeyboard.WillShowNotification, OnKeyboardShowNotification);
        }

        private void RemoveObservers()
        {
            NSNotificationCenter.DefaultCenter.RemoveObserver(hideObserver);
            NSNotificationCenter.DefaultCenter.RemoveObserver(showObserver);
        }

        private void OnKeyboardShowNotification (NSNotification notification)
        {
            NSDictionary info = notification.UserInfo;
            CGSize keyboardSize = ((NSValue)info.ObjectForKey(UIKeyboard.FrameBeginUserInfoKey)).RectangleFValue.Size;
            UIEdgeInsets contentInsets = new UIEdgeInsets(0, 0, keyboardSize.Height, 0);
            Scroller.ContentInset = contentInsets;
            Scroller.ScrollIndicatorInsets = contentInsets;

            nfloat currentScroll = Scroller.ContentOffset.Y;
            nfloat minScroll = (activeField.Frame.Bottom + 4) - keyboardSize.Height;

            if (currentScroll < minScroll)
            {
                Scroller.SetContentOffset(new CGPoint(0, minScroll), true);
            }

               
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            AddObservers();
        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();
            Scroller.ContentSize = this.View.Frame.Size;// new CGSize(320, 568);
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            RemoveObservers();
            UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
        }

        private void OnKeyboardHideNotification (NSNotification notification)
        {
            UIEdgeInsets contentInsets = UIEdgeInsets.Zero;
            Scroller.ContentInset = contentInsets;
            Scroller.ScrollIndicatorInsets = contentInsets;
        }


        public override void ViewDidLoad()
        {
			base.ViewDidLoad();

            Scroller.KeyboardDismissMode = UIScrollViewKeyboardDismissMode.OnDrag;
            Scroller.TranslatesAutoresizingMaskIntoConstraints = false;
            Scroller.Frame = new CGRect(0, 0, this.View.Frame.Width, this.View.Frame.Height);
            Scroller.ContentSize = Scroller.Frame.Size;//new CGSize(320, 568);
            HandleTextValueChanged(null, null);

            indicator = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.Gray);
            indicator.Frame = new CGRect(0, 0, 40, 40);
            indicator.Center = this.View.Center;
            this.View.AddSubview(indicator);
            indicator.BringSubviewToFront(this.View);
            UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;

            usernameField.EditingDidEnd += HandleTextValueChanged;
            usernameField.ShouldReturn += TextFieldShouldReturn;
            usernameField.EditingDidBegin += SetCurrentField;
			usernameField.EditingChanged += HandleTextValueChanged;

            passwordField.EditingDidEnd += HandleTextValueChanged;
            passwordField.ShouldReturn += TextFieldShouldReturn;
            passwordField.EditingDidBegin += SetCurrentField;
			passwordField.EditingChanged += HandleTextValueChanged;

            confirmPassword.EditingDidEnd += HandleTextValueChanged;
            confirmPassword.EditingDidBegin += SetCurrentField;
			confirmPassword.EditingChanged += HandleTextValueChanged;
			confirmPassword.ShouldReturn += (textField) => 
                { 
                    textField.ResignFirstResponder();
                    return false; 
                };

            emailField.EditingDidEnd += HandleTextValueChanged;
            emailField.EditingDidBegin += SetCurrentField;
            emailField.ShouldReturn += (textField) => { 
                textField.ResignFirstResponder();
                return false; 
            };


            signInBtn.TouchUpInside += (object sender, EventArgs e) => 
                {
                    indicator.StartAnimating();
                    string userName = usernameField.Text.Trim();
                    string password = passwordField.Text;
                    signInBtn.Enabled = false;
                    createAccountBtn.Enabled = false;
                    skipButton.Enabled = false;

                    // sign in
                    BlahguaAPIObject.Current.SignIn(userName, password, true, SiginInResultCallback);
                };

            createAccountBtn.TouchUpInside += (object sender, EventArgs e) => 
                {
                    indicator.StartAnimating();
                    string userName = usernameField.Text.Trim();
                    string password = passwordField.Text;
                    signInBtn.Enabled = false;
                    createAccountBtn.Enabled = false;
                    skipButton.Enabled = false;


                    // sign in
                    BlahguaAPIObject.Current.Register(userName, password, true, CreateAccountResultCallback);


                };


            skipButton.TouchUpInside += (object sender, EventArgs e) => 
                {

                    ((BGSignOnPageViewController)ParentViewController).Finish();


                };

            PrepSignIn.TouchUpInside += (object sender, EventArgs e) => 
                {
                    PrepForSignIn();
                };

        }

        private void SiginInResultCallback(string result)
        {

            if (result == null)
            {
                AppDelegate.analytics.PostLogin();
                InvokeOnMainThread(() =>
                    {
                        indicator.StopAnimating();
                        ((BGSignOnPageViewController)ParentViewController).Finish();
                    });
            }
            else
            {
                AppDelegate.analytics.PostSessionError("signinfailed-" + result);

                DisplayAlert(result, "Unable to sign in.  Check username and password");
                InvokeOnMainThread(() =>
                    {
                        indicator.StopAnimating();
                        signInBtn.Enabled = true;
                        createAccountBtn.Enabled = true;
                        skipButton.Enabled = true;
                        HandleTextValueChanged(null, null);
                    });
            }

        }

        private void CreateAccountResultCallback(string result)
        {
            if (result == null)
            {
                AppDelegate.analytics.PostRegisterUser();
                InvokeOnMainThread(() =>
					{
						((BGSignOnPageViewController)ParentViewController).Finish();
					/*
						NSUserDefaults.StandardUserDefaults.SetInt(2,"signupStage");
						NSUserDefaults.StandardUserDefaults.Synchronize();
                        indicator.StopAnimating();
                        string emailAddr = emailField.Text.Trim();

                        if (!String.IsNullOrEmpty(emailAddr))
                        {
                            BlahguaAPIObject.Current.SetRecoveryEmail(emailAddr, (resultStr) =>
                                {

                                    InvokeOnMainThread(() => ((BGSignOnPageViewController)ParentViewController).GoToNext());
                                });
                        }
                        else
                            ((BGSignOnPageViewController)ParentViewController).GoToNext();
                            */
                    });
            }
            else
            {
                AppDelegate.analytics.PostSessionError("registerfailed-" + result);

                DisplayAlert(result, "Unable to create account.  Check username");
                InvokeOnMainThread(() =>
                    {
                        indicator.StopAnimating();
                        signInBtn.Enabled = true;
                        createAccountBtn.Enabled = true;
                        skipButton.Enabled = true;
                        HandleTextValueChanged(null, null);
                    });
            }
        }

        public void DisplayAlert(string titleString, string descString)
        {
            InvokeOnMainThread(() => 
                {
                    UIAlertController alert = UIAlertController.Create(titleString, descString, UIAlertControllerStyle.Alert);

                    if (alert != null)
                    {
                        alert.AddAction(UIAlertAction.Create("ok", UIAlertActionStyle.Default, null));

                        PresentViewController(alert, true, null);
                    }
                    else
                    {
                        // use UI AlertView
                        UIAlertView oldAlert = new UIAlertView(titleString, descString, null, "ok", null);
                        oldAlert.Show();
                    }
                });

        }


        private bool TextFieldShouldReturn (UITextField textfield){
            nint nextTag = textfield.Tag + 1;    
            UIResponder nextResponder = this.View.ViewWithTag (nextTag);
            if (nextResponder != null) {
                nextResponder.BecomeFirstResponder ();
            } else {
                // Not found, so remove keyboard.
                textfield.ResignFirstResponder ();
            }
            return false; // We do not want UITextField to insert line-breaks.
        }

        protected  UIView KeyboardGetActiveView()
        {
            return FindFirstResponder(this.View);
        }

        private  UIView FindFirstResponder(UIView view)
        {
            if (view.IsFirstResponder)
            {
                return view;
            }
            foreach (UIView subView in view.Subviews)
            {
                var firstResponder = FindFirstResponder(subView);
                if (firstResponder != null)
                    return firstResponder;
            }
            return null;
        }

        public override void TouchesBegan (NSSet touches, UIEvent evt)
        {
            var activeView = KeyboardGetActiveView();
            if (activeView != null)
                activeView.ResignFirstResponder();
        }

        void SetCurrentField (object sender, EventArgs e)
        {
            activeField = (UITextField)sender;
        }

        void HandleTextValueChanged (object sender, EventArgs e)
        {
            activeField = null;
            string usernameText = usernameField.Text;
            string passwordText = passwordField.Text;
            string confirmText = confirmPassword.Text;
            string emailText = emailField.Text;

            if (String.IsNullOrEmpty(usernameText) || String.IsNullOrEmpty(passwordText) ||
                (usernameText.Length < 3) || (passwordText.Length < 3))
            {
                signInBtn.Enabled = false;
                createAccountBtn.Enabled = false;

            }
            else
            {
                signInBtn.Enabled = true;

                if (passwordText == confirmText)
                    createAccountBtn.Enabled = true;
                else
                    createAccountBtn.Enabled = false;
            }
        }

        void PrepForSignIn()
        {
            PrepSignIn.Hidden = true;
            createAccountBtn.Hidden = true;
            emailField.Hidden = true;
            confirmPassword.Hidden = true;
            signInBtn.Hidden = false;
            recoveryLabel.Hidden = true;
            skipButton.Frame.Offset(new CGPoint(0, -124));
            passwordField.ReturnKeyType = UIReturnKeyType.Default;

        }
	}
}
