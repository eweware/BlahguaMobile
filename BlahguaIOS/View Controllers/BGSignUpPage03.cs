using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.CodeDom.Compiler;
using BlahguaMobile.BlahguaCore;


namespace BlahguaMobile.IOS
{
	partial class BGSignUpPage03 : UIViewController
	{
        private string emailAddr;
        private string currentTicket;
        private BadgeAuthority emailAuthority = null;


		public BGSignUpPage03 (IntPtr handle) : base (handle)
		{
		}

        public override void ViewDidLoad()
        {
            checkBtn.Enabled = false;
            emailAddr = "";

            base.ViewDidLoad();

            if (!String.IsNullOrEmpty(BlahguaAPIObject.Current.CurrentUser.RecoveryEmail))
                emailAddr = BlahguaAPIObject.Current.CurrentUser.RecoveryEmail;

            // enter email

            this.checkBtn.TouchUpInside += (object sender, EventArgs e) => 
                {
                    // submit email
                    this.checkBtn.Enabled = false;
                    emailAddr = this.emailAddrField.Text;
                    emailAddrField.ResignFirstResponder();
                    string authId = emailAuthority._id;

                    // make formal request
                    BlahguaAPIObject.Current.GetEmailBadgeForUser(authId, emailAddr, (ticket) => {
                        InvokeOnMainThread(() => {
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
                                alert.AddAction(UIAlertAction.Create("next step", UIAlertActionStyle.Default,
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
                                    alert.AddAction(UIAlertAction.Create("let's go!", UIAlertActionStyle.Default, 
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
                                checkBtn.Enabled = true;
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
	}
}
