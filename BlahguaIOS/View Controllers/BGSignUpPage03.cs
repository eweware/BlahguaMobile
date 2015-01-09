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

		public BGSignUpPage03 (IntPtr handle) : base (handle)
		{
		}

        public override void ViewDidLoad()
        {
            emailAddr = "";

            base.ViewDidLoad();

            if (!String.IsNullOrEmpty(BlahguaAPIObject.Current.CurrentUser.RecoveryEmail))
                emailAddr = BlahguaAPIObject.Current.CurrentUser.RecoveryEmail;

            // enter email

            this.checkBtn.TouchUpInside += (object sender, EventArgs e) => 
                {
                    // submit email
                    UIAlertController alert = UIAlertController.Create("Badges Success", "Badges are available for your email address.", UIAlertControllerStyle.Alert);
                    alert.AddAction(UIAlertAction.Create("Next Step", UIAlertActionStyle.Default,
                        Action =>
                        {
                            PrepPhaseTwo();
                        }));
                    PresentViewController(alert, true, null);
                };

            this.skipBtn.TouchUpInside += (object sender, EventArgs e) => 
                {
                    // we are done - dismiss it
                    ((BGSignOnPageViewController)this.ParentViewController).Finish();
                };

            // verify
            this.verifyBtn.TouchUpInside += (object sender, EventArgs e) =>
            {
                var alert = UIAlertController.Create("Verified!", "You are ready to use badges in Heard!", UIAlertControllerStyle.Alert);
                alert.AddAction(UIAlertAction.Create("Let's Go!", UIAlertActionStyle.Default, 
                        Action =>
                        {
                            ((BGSignOnPageViewController)this.ParentViewController).Finish();
                        }));
                PresentViewController(alert, true, null);
            };

            this.noCodeBtn.TouchUpInside += (object sender, EventArgs e) => 
                {
                    // we are done - dismiss it
                    verificationField.Text = "";
                    PrepPhaseOne();
                };

            PrepPhaseOne();
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
