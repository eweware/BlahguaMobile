using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.CodeDom.Compiler;
using System.Drawing;

using BlahguaMobile.BlahguaCore;

namespace BlahguaMobile.IOS
{
	partial class BGSignUpPage01 : UIViewController
	{
        UITextField activeField = null;
        NSObject hideObserver;
        NSObject showObserver;

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
            SizeF keyboardSize = ((NSValue)info.ObjectForKey(UIKeyboard.FrameBeginUserInfoKey)).RectangleFValue.Size;
            UIEdgeInsets contentInsets = new UIEdgeInsets(0, 0, keyboardSize.Height, 0);
            Scroller.ContentInset = contentInsets;
            Scroller.ScrollIndicatorInsets = contentInsets;


            RectangleF viewRect = this.View.Frame;
            viewRect.Height -= keyboardSize.Height;
            if (!viewRect.Contains(activeField.Frame.Location))
            {
                Scroller.ScrollRectToVisible(viewRect, true);
            }
               
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            AddObservers();
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            RemoveObservers();
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

            Scroller.ContentSize = this.View.Frame.Size;
            HandleTextValueChanged(null, null);

            usernameField.EditingDidEnd += HandleTextValueChanged;
            usernameField.ShouldReturn += TextFieldShouldReturn;
            usernameField.EditingDidBegin += SetCurrentField;

            passwordField.EditingDidEnd += HandleTextValueChanged;
            passwordField.ShouldReturn += TextFieldShouldReturn;
            passwordField.EditingDidBegin += SetCurrentField;

            confirmPassword.EditingDidEnd += HandleTextValueChanged;
            confirmPassword.EditingDidBegin += SetCurrentField;
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
                    string userName = usernameField.Text.Trim();
                    string password = passwordField.Text;

                    // sign in
                    BlahguaAPIObject.Current.SignIn(userName, password, true, SiginInResultCallback);
                };

            createAccountBtn.TouchUpInside += (object sender, EventArgs e) => 
                {
                    string userName = usernameField.Text.Trim();
                    string password = passwordField.Text;

                    // sign in
                    BlahguaAPIObject.Current.Register(userName, password, true, CreateAccountResultCallback);


                };


            skipButton.TouchUpInside += (object sender, EventArgs e) => 
                {

                    ((BGSignOnPageViewController)ParentViewController).Finish();


                };

        }

        private void SiginInResultCallback(string result)
        {

            if (result == null)
            {
                AppDelegate.analytics.PostLogin();
                InvokeOnMainThread(() => ((BGSignOnPageViewController)ParentViewController).Finish());
            }
            else
            {
                AppDelegate.analytics.PostSessionError("signinfailed-" + result);

                DisplayAlert(result, "Unable to sign in.  Check username and password");
            }

        }

        private void CreateAccountResultCallback(string result)
        {
            if (result == null)
            {
                AppDelegate.analytics.PostRegisterUser();
                InvokeOnMainThread(() =>
                    {
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
                    });
            }
            else
            {
                AppDelegate.analytics.PostSessionError("registerfailed-" + result);

                DisplayAlert(result, "Unable to create account.  Check username");
            }
        }

        public void DisplayAlert(string titleString, string descString)
        {
            InvokeOnMainThread(() => 
                {
                    UIAlertController alert = UIAlertController.Create(titleString, descString, UIAlertControllerStyle.Alert);
                    alert.AddAction(UIAlertAction.Create("ok", UIAlertActionStyle.Default, null));

                    PresentViewController(alert, true, null);
                });

        }


        private bool TextFieldShouldReturn (UITextField textfield){
            int nextTag = textfield.Tag + 1;    
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
	}
}
