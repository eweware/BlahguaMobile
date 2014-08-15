// This file has been autogenerated from a class added in the UI designer.

using System;

using BlahguaMobile.BlahguaCore;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace BlahguaMobile.IOS
{
	public partial class BGBadgeAddViewController : UIViewController
	{
		private string ticketString;

		public BGBadgeAddViewController (IntPtr handle) : base (handle)
		{
		}


		public override void ViewDidLoad ()
		{
            base.ViewDidLoad ();

            View.BackgroundColor = UIColor.FromPatternImage (UIImage.FromBundle ("grayBack"));

			// page one
			infoTitle.AttributedText = new NSAttributedString("Badges add credibility to content by letting users attach verified facts about themselves.  (e.g.I work at Microsoft, I am in Chicago, etc.)  Currently only email-based badges are issued.",
				UIFont.FromName(BGAppearanceConstants.FontName, 17),
				UIColor.Black
			);

			labelPrivacyNotice.AttributedText = new NSAttributedString("Only badges (not your email address) will be sent to Heard",
				UIFont.FromName(BGAppearanceConstants.FontName, 13),
				UIColor.Black
			);

			emailTextField.AttributedPlaceholder = new NSAttributedString(
				"email address", UIFont.FromName(BGAppearanceConstants.FontName, 17), 
				UIColor.LightGray
			);

			emailTextField.AttributedText = new NSAttributedString(
				"", 
				UIFont.FromName(BGAppearanceConstants.FontName, 17), 
				UIColor.Black
			);


			emailTextField.AllEditingEvents += (object sender, EventArgs e) => {
				if(!String.IsNullOrEmpty(emailTextField.Text))
				{
					doneButton.Enabled = true;
				}
				else
				{
					doneButton.Enabled = false;
				}
			};

			doneButton.Enabled = false;
            doneButton.SetBackgroundImage (UIImage.FromBundle ("long_button_gray"), UIControlState.Disabled);
			doneButton.SetAttributedTitle(new NSAttributedString("Submit", UIFont.FromName(BGAppearanceConstants.BoldFontName, 17), UIColor.White), UIControlState.Normal);

			// page two
			labelVerifyText.AttributedText = new NSAttributedString("Please check your email for a confirmation code and enter it below.  If you did not receive the code, please try your request again.",
				UIFont.FromName(BGAppearanceConstants.FontName, 13),
				UIColor.Black
			);

			verifyCodeTextField.AttributedPlaceholder = new NSAttributedString(
				"verification code", UIFont.FromName(BGAppearanceConstants.FontName, 17), 
				UIColor.LightGray
			);

			verifyCodeTextField.AttributedText = new NSAttributedString(
				"", 
				UIFont.FromName(BGAppearanceConstants.FontName, 17), 
				UIColor.Black
			);
				
			verifyCodeTextField.AllEditingEvents += (object sender, EventArgs e) => {
				if(!String.IsNullOrEmpty(verifyCodeTextField.Text))
				{
                    verifyButton.Enabled = true;
				}
				else
				{
                    verifyButton.Enabled = false;
				}
			};

			verifyButton.Enabled = false;
            verifyButton.SetBackgroundImage (UIImage.FromBundle ("long_button_gray"), UIControlState.Disabled);
            verifyButton.SetAttributedTitle(new NSAttributedString("Verify", UIFont.FromName(BGAppearanceConstants.BoldFontName, 17), UIColor.White), UIControlState.Normal);



			// page three
            labelRequestText.AttributedText = new NSAttributedString("No badges are available for your domain.  Click REQUEST to request new badges from the authority.",
				UIFont.FromName(BGAppearanceConstants.FontName, 13),
				UIColor.Black
			);

			requestButton.Enabled = true;
            requestButton.SetAttributedTitle(new NSAttributedString("Request", UIFont.FromName(BGAppearanceConstants.BoldFontName, 17), UIColor.White), UIControlState.Normal);

			// events
            doneButton.TouchUpInside += ClickBadgeSubmitBtn;
            verifyButton.TouchUpInside += ClickBadgeVerifyBtn;
            requestButton.TouchUpInside += ClickBadgeRequestBtn;

		}

		private void ClickBadgeSubmitBtn(object sender, EventArgs e)
		{
			ticketString = "";
			emailTextField.ResignFirstResponder();
			BlahguaAPIObject.Current.GetBadgeAuthorities((authorities) =>{
                if (authorities == null)
                    return;
				InvokeOnMainThread(() => {
					string authId = authorities[0]._id;
					string emailAddr = emailTextField.Text;
                    BlahguaAPIObject.Current.GetEmailBadgeForUser(authId, emailAddr, (ticket) => {
						InvokeOnMainThread(() => {
							if(ticket == String.Empty)
							{
								doneButton.Enabled = false;
								requestButton.Enabled = true;
								BadgeRequestView.Hidden = false;
								BadgeSubmitView.Hidden = true;
                                AppDelegate.analytics.PostBadgeNoEmail(emailAddr);

							}
							else
							{
                                AppDelegate.analytics.PostRequestBadge(authId);
								ticketString = ticket;
								doneButton.Enabled = false;
								emailTextField.Text = String.Empty;
								verifyCodeTextField.Text = String.Empty;
								verifyButton.Enabled = false;
								BadgeVerifyView.Hidden = false;
								BadgeSubmitView.Hidden = true;
							}
						});
					});
				});
			});
		}

		private void ClickBadgeVerifyBtn(object sender, EventArgs args)
		{
			BlahguaAPIObject.Current.VerifyEmailBadge (verifyCodeTextField.Text, ticketString, (result) => {
				InvokeOnMainThread (() => {
					if (result == "fail") {
						UIAlertView alert = new UIAlertView ("", "That validation code was not valid.  Please retry your badging attempt.", null, "OK");
						alert.Show ();
                        verifyCodeTextField.SelectAll(this);
                        AppDelegate.analytics.PostBadgeValidateFailed();
					} else {
                        AppDelegate.analytics.PostGotBadge();
                        BlahguaAPIObject.Current.RefreshUserBadges((theStr) =>
                            {
                                InvokeOnMainThread(() => {
                                    UIAlertView alert = new UIAlertView ("", "Code accepted.  Your new badge will be issued shortly.", null, "OK");
                                    alert.Show();
                                    // TO DO:  Update the badge area
                                    NavigationController.PopViewControllerAnimated (true);
                                    });
                                }
                        );
					}
				});
			});
		}

		private void ClickBadgeRequestBtn(object sender, EventArgs args)
		{
			var addr = new System.Net.Mail.MailAddress(emailTextField.Text);
			string emailAddr = addr.Address;
			string domainName = addr.Host;
			BlahguaAPIObject.Current.RequestBadgeForDomain(emailAddr, domainName, (resultStr) =>
				{
					InvokeOnMainThread(() =>{
						if(resultStr == "ok")
						{
							UIAlertView alert = new UIAlertView("", "Domain requested!", null, "OK");
							alert.Show();
						}
							
						NavigationController.PopViewControllerAnimated(true);
					});
				});
		}
	}
}
