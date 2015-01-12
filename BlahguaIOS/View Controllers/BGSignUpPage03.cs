using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.CodeDom.Compiler;
using BlahguaMobile.BlahguaCore;
using System.Drawing;


namespace BlahguaMobile.IOS
{
	partial class BGSignUpPage03 : UIViewController
	{
        private string emailAddr;
        private string currentTicket;
        private BadgeAuthority emailAuthority = null;
        UIActivityIndicatorView indicator = null;

        UITextField activeField = null;
        NSObject hideObserver;
        NSObject showObserver;

		public BGSignUpPage03 (IntPtr handle) : base (handle)
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

            float currentScroll = Scroller.ContentOffset.Y;
            float minScroll = (activeField.Frame.Bottom + 4) - keyboardSize.Height;

            if (currentScroll < minScroll)
            {
                Scroller.SetContentOffset(new PointF(0, minScroll), true);
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
            checkBtn.Enabled = false;
            skipBtn.Enabled = false;
            emailAddr = "";

            base.ViewDidLoad();
            Scroller.KeyboardDismissMode = UIScrollViewKeyboardDismissMode.OnDrag;

            Scroller.ContentSize = new SizeF(320, 568);
            Scroller.Frame = new RectangleF(0, 0, this.View.Frame.Width, this.View.Frame.Height);
            HandleTextValueChanged(null, null);

            indicator = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.Gray);
            indicator.Frame = new RectangleF(0, 0, 40, 40);
            indicator.Center = this.View.Center;
            this.View.AddSubview(indicator);
            indicator.BringSubviewToFront(this.View);
            UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;


            emailAddrField.EditingDidEnd += HandleTextValueChangeEnd;
            emailAddrField.EditingChanged += HandleTextValueChanged;
            emailAddrField.EditingDidBegin += SetCurrentField;
            emailAddrField.ShouldReturn += (textField) => 
                { 
                    emailAddrField.ResignFirstResponder();
                    return false; 
                };

            verificationField.EditingDidEnd += HandleTextValueChangeEnd;
            verificationField.EditingDidBegin += SetCurrentField;
            verificationField.ShouldReturn += (textField) => 
                { 
                    verificationField.ResignFirstResponder();
                    return false; 
                };
            verificationField.EditingChanged += HandleTextValueChanged;

            if (!String.IsNullOrEmpty(BlahguaAPIObject.Current.CurrentUser.RecoveryEmail))
                emailAddr = BlahguaAPIObject.Current.CurrentUser.RecoveryEmail;

            // enter email

            this.checkBtn.TouchUpInside += (object sender, EventArgs e) => 
                {
                    indicator.StartAnimating();
                    // submit email
                    this.checkBtn.Enabled = false;
                    this.skipBtn.Enabled = false;
                    emailAddr = this.emailAddrField.Text;
                    emailAddrField.ResignFirstResponder();
                    string authId = emailAuthority._id;

                    // make formal request
                    BlahguaAPIObject.Current.GetEmailBadgeForUser(authId, emailAddr, (ticket) => {
                        InvokeOnMainThread(() => {
                            indicator.StopAnimating();
                            UIAlertController alert;
                            if(ticket == String.Empty)
                            {
                                AppDelegate.analytics.PostBadgeNoEmail(emailAddr);
                                alert = UIAlertController.Create("No Result", "No badges are available for your domain.  You can try again from your profile page.", UIAlertControllerStyle.Alert);
                                alert.AddAction(UIAlertAction.Create("got it", UIAlertActionStyle.Default,
                                    Action =>
                                    {
                                        ((BGSignOnPageViewController)this.ParentViewController).Finish();
                                    }));
                            }
                            else if (ticket == "existing")
                            {
                                AppDelegate.analytics.PostBadgeNoEmail(emailAddr);
                                alert = UIAlertController.Create("Issued", "A badge has previously been issued to this email address.", UIAlertControllerStyle.Alert);
                                alert.AddAction(UIAlertAction.Create("got it", UIAlertActionStyle.Default,
                                    Action =>
                                    {
                                        ((BGSignOnPageViewController)this.ParentViewController).Finish();
                                    }));
                            }
                            else
                            {
                                AppDelegate.analytics.PostRequestBadge(authId);
                                currentTicket = ticket;
                                alert = UIAlertController.Create("Badges Success", "Badges are available for your email address.", UIAlertControllerStyle.Alert);
                                alert.AddAction(UIAlertAction.Create("next", UIAlertActionStyle.Default,
                                    Action =>
                                    {
                                        PrepPhaseTwo();
                                    }));
                            }
                            PresentViewController(alert, true, null);
                        });
                    });
                };

            this.skipBtn.TouchUpInside += (object sender, EventArgs e) => 
                {
                    // we are done - dismiss it
                    ((BGSignOnPageViewController)this.ParentViewController).Finish();
                };

            // verify
            this.verifyBtn.TouchUpInside += (object sender, EventArgs e) =>
                {
                    BlahguaAPIObject.Current.VerifyEmailBadge (verificationField.Text, currentTicket, (result) => {
                        InvokeOnMainThread (() => 
                            {
                                UIAlertController alert;

                                if (result == "fail") 
                                {
                                    AppDelegate.analytics.PostBadgeValidateFailed();
                                    alert = UIAlertController.Create("Verification Failure", "That validation code was not valid.  Please retry your badging attempt.", UIAlertControllerStyle.Alert);
                                    alert.AddAction(UIAlertAction.Create("retry", UIAlertActionStyle.Default, 
                                        Action =>
                                        {
                                            verificationField.SelectAll(this);
                                        }));
                                } 
                                else 
                                {
                                    AppDelegate.analytics.PostGotBadge();
                                    BlahguaAPIObject.Current.RefreshUserBadges(null);
                                    alert = UIAlertController.Create("Verified", "You are ready to use badges in Heard.", UIAlertControllerStyle.Alert);
                                    alert.AddAction(UIAlertAction.Create("let's go", UIAlertActionStyle.Default, 
                                        Action =>
                                        {
                                            ((BGSignOnPageViewController)this.ParentViewController).Finish();
                                        }));
                                }

                                PresentViewController(alert, true, null);
                        });
                    });






                };

            this.noCodeBtn.TouchUpInside += (object sender, EventArgs e) => 
                {
                    // we are done - dismiss it
                    verificationField.Text = "";
                    PrepPhaseOne();
                };

            PrepPhaseOne();
            InitBadgeAuthorities();
        }


        private void InitBadgeAuthorities()
        {
            BlahguaAPIObject.Current.GetBadgeAuthorities((authorities) =>
                {
                    InvokeOnMainThread( () => 
                        {
                            if ((authorities == null) || (authorities.Count == 0))
                            {
                                Console.WriteLine("Error:  no badge authories available");

                                 ((BGSignOnPageViewController)this.ParentViewController).Finish();
                            }
                            else
                            {
                                emailAuthority = authorities[0];
                                skipBtn.Enabled = true;
                            }
                        });
                });
        }

        private void PrepPhaseOne()
        {
            InvokeOnMainThread(() =>
                {
                    this.emailAddrField.Hidden = false;
                    this.preBadgeLabel.Hidden = false;
                    this.skipBtn.Hidden = false;
                    this.checkBtn.Hidden = false;

                    this.badgeReceivedLabel.Hidden = true;
                    this.noCodeBtn.Hidden = true;
                    this.verificationField.Hidden = true;
                    this.verifyBtn.Hidden = true;
                });
        }

        private void PrepPhaseTwo()
        {
            InvokeOnMainThread(() =>
                {
                    verificationField.Text = "";
                    this.emailAddrField.Hidden = true;
                    this.preBadgeLabel.Hidden = true;
                    this.skipBtn.Hidden = true;
                    this.checkBtn.Hidden = true;

                    this.badgeReceivedLabel.Hidden = false;
                    this.noCodeBtn.Hidden = false;
                    this.verificationField.Hidden = false;
                    this.verifyBtn.Hidden = false;
                });
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
            string emailText = emailAddrField.Text;
            string verifyText = verificationField.Text;

            if (String.IsNullOrEmpty(emailText))
                checkBtn.Enabled = false;
            else
                checkBtn.Enabled = true;

            if (String.IsNullOrEmpty(verifyText))
                verifyBtn.Enabled = false;
            else
                verifyBtn.Enabled = true;
        }

        void HandleTextValueChangeEnd (object sender, EventArgs e)
        {
            activeField = null;
            string emailText = emailAddrField.Text;
            string verifyText = verificationField.Text;

            if (String.IsNullOrEmpty(emailText))
                checkBtn.Enabled = false;
            else
                checkBtn.Enabled = true;

            if (String.IsNullOrEmpty(verifyText))
                verifyBtn.Enabled = false;
            else
                verifyBtn.Enabled = true;
        }
	}
}
