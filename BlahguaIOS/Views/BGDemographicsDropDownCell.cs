// This file has been autogenerated from a class added in the UI designer.

using System;
using System.ComponentModel;

using BlahguaMobile.BlahguaCore;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace BlahguaMobile.IOS
{
	public partial class BGDemographicsDropDownCell : UITableViewCell
	{
		public BGDemographicsViewController viewController;
		private int index;
		private bool isPublic;

		public BGDemographicsDropDownCell (IntPtr handle) : base (handle)
		{
		}

		public void SetUp (int section)
		{
			this.index = section;

			ContentView.BackgroundColor = UIColor.FromRGB (248, 248, 248);

			ddButton.SetAttributedTitle (new NSAttributedString ("Select", UIFont.FromName (BGAppearanceConstants.BoldFontName, 15), UIColor.White), UIControlState.Normal);
            //ddButton.SetBackgroundImage (UIImage.FromBundle ("short_button_normal"), UIControlState.Normal);
			publicLabel.AttributedText = new NSAttributedString ("", UIFont.FromName (BGAppearanceConstants.FontName, 14), UIColor.Black);

            isPublicButton.SetBackgroundImage(UIImage.FromBundle("signupRadioButtonUn"), UIControlState.Normal);

			if(viewController.GetPermission(index))
			{
				publicLabel.AttributedText = new NSAttributedString ("Public", UIFont.FromName (BGAppearanceConstants.FontName, 14), UIColor.Black);
                isPublicButton.SetImage(UIImage.FromBundle("signupRadioButton"), UIControlState.Normal);
				isPublic = true;
			}
			else
			{
				publicLabel.AttributedText = new NSAttributedString ("Private", UIFont.FromName (BGAppearanceConstants.FontName, 14), UIColor.Black);
                isPublicButton.SetImage(UIImage.FromBundle("signupRadioButtonUn"), UIControlState.Normal);
				isPublic = false;
			}
			isPublicButton.TouchUpInside += (object sender, EventArgs e) => {
				isPublic = !isPublic;
				viewController.SetPermission(index, isPublic);
				publicLabel.AttributedText = new NSAttributedString (isPublic ? "Public" : "Private", UIFont.FromName (BGAppearanceConstants.FontName, 14), UIColor.Black);
                isPublicButton.SetImage(UIImage.FromBundle(isPublic ? "signupRadioButton" : "signupRadioButtonUn"), UIControlState.Normal);
			};
			ddButton.TouchUpInside += (sender, e) => {
				viewController.PushSelectingTable(index);
			};
			BlahguaAPIObject.Current.CurrentUser.Profile.PropertyChanged += (sender, e) => 
			{
				switch(section)
				{
				case 0:
					{
						if(e.PropertyName == "Gender")
						{
							ddButton.SetAttributedTitle (new NSAttributedString (
								BlahguaAPIObject.Current.CurrentUser.Profile.Gender, 
								UIFont.FromName (BGAppearanceConstants.BoldFontName, 15), 
								UIColor.Black
							), UIControlState.Normal);
						}

						break;
					}
				case 2:
					{
						if(e.PropertyName == "Race")
						{
							ddButton.SetAttributedTitle (new NSAttributedString (
								BlahguaAPIObject.Current.CurrentUser.Profile.Race, 
								UIFont.FromName (BGAppearanceConstants.BoldFontName, 15), 
								UIColor.Black
							), UIControlState.Normal);
						}

						break;
					}
				case 6:
					{
						if(e.PropertyName == "Country")
						{
							ddButton.SetAttributedTitle (new NSAttributedString (
								BlahguaAPIObject.Current.CurrentUser.Profile.Country, 
								UIFont.FromName (BGAppearanceConstants.BoldFontName, 15), 
								UIColor.Black
							), UIControlState.Normal);
						}
						break;
					}
				case 7:
				default:
					{
						if(e.PropertyName == "Income")
						{
							ddButton.SetAttributedTitle (new NSAttributedString (
								BlahguaAPIObject.Current.CurrentUser.Profile.Income, 
								UIFont.FromName (BGAppearanceConstants.BoldFontName, 15), 
								UIColor.Black
							), UIControlState.Normal);
						}
						break;
					}
				}

			};

		}
	}
}
