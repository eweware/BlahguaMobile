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
		private bool isEmail;

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

			labelPrivacy.AttributedText = new NSAttributedString("Only badges (not your email address) will be sent to Heard",
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
			labelVerifyCodeText.AttributedText = new NSAttributedString("Please check your email for a confirmation code and enter it below.  If you did not receive the code, please try your request again.",
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
					verifyCodeBtn.Enabled = true;
				}
				else
				{
					verifyCodeBtn.Enabled = false;
				}
			};

			verifyCodeBtn.Enabled = false;
			verifyCodeBtn.SetBackgroundImage (UIImage.FromBundle ("long_button_gray"), UIControlState.Disabled);
			verifyCodeBtn.SetAttributedTitle(new NSAttributedString("Verify", UIFont.FromName(BGAppearanceConstants.BoldFontName, 17), UIColor.White), UIControlState.Normal);



			// page three
			labelRequestBadge.AttributedText = new NSAttributedString("Only badges (not your email address) will be sent to Heard",
				UIFont.FromName(BGAppearanceConstants.FontName, 13),
				UIColor.Black
			);

			RequestBadgeBtn.Enabled = true;
			RequestBadgeBtn.SetAttributedTitle(new NSAttributedString("Request", UIFont.FromName(BGAppearanceConstants.BoldFontName, 17), UIColor.White), UIControlState.Normal);

			// events


			//doneButton.TouchUpInside += (object sender, EventArgs e) => ;

		}

		private void ClickBadgeSubmitBtn(object sender, EventArgs e)
		{
			ticketString = "";
			emailTextField.ResignFirstResponder();
			BlahguaAPIObject.Current.GetBadgeAuthorities((authorities) =>{
				InvokeOnMainThread(() => {
					string authId = authorities[0]._id;
					string emailAddr = emailTextField.Text;
					BlahguaAPIObject.Current.GetBadgeForUser(authId, (ticket) => {
						InvokeOnMainThread(() => {
							if(ticket == String.Empty)
							{
								doneButton.Enabled = false;
								RequestBadgeBtn.Enabled = true;
								BadgeRequestView.Hidden = false;
								BadgeSubmitView.Hidden = true;

							}
							else
							{
								ticketString = ticket;
								doneButton.Enabled = false;
								emailTextField.Text = String.Empty;
								verifyCodeTextField.Text = String.Empty;
								verifyCodeBtn.Enabled = false;
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
					if (result == String.Empty) {
						UIAlertView alert = new UIAlertView ("", "That validation code was not valid.  Please retry your badging attempt.", null, "OK");
						alert.Show ();
					} else {
						UIAlertView alert = new UIAlertView ("", "Code accepted.  Your new badge will be issued shortly.", null, "OK");

						NavigationController.PopViewControllerAnimated (true);
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
