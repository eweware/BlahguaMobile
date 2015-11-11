// This file has been autogenerated from a class added in the UI designer.

using System;
using System.Globalization;

using BlahguaMobile.BlahguaCore;

using Foundation;
using UIKit;
using SharpMobileCode.ModalPicker;

namespace BlahguaMobile.IOS
{
	public partial class BGDemographicsInputCell : UITableViewCell
	{
		private int index;
		private bool isPublic;

		public BGDemographicsViewController viewController;

		public BGDemographicsInputCell (IntPtr handle) : base (handle)
		{
		}


		public void SetUp (int section)
		{
			this.index = section;
			ContentView.BackgroundColor = UIColor.FromRGB (248, 248, 248);

			publicLabel.AttributedText = new NSAttributedString ("", UIFont.FromName (BGAppearanceConstants.FontName, 14), UIColor.Black);

            isPublicButton.SetBackgroundImage(UIImage.FromBundle("signupRadioButtonUn"), UIControlState.Normal);

            publicLabel.AttributedText = new NSAttributedString ("Public", UIFont.FromName (BGAppearanceConstants.FontName, 14), UIColor.Black);

            if(viewController.GetPermission(index))
			{
               isPublicButton.SetImage(UIImage.FromBundle("signupRadioButton"), UIControlState.Normal);
				isPublic = true;
			}
			else
			{
                isPublicButton.SetImage(UIImage.FromBundle("signupRadioButtonUn"), UIControlState.Normal);
				isPublic = false;
			}
			isPublicButton.TouchUpInside += (object sender, EventArgs e) => {
				isPublic = !isPublic;
				viewController.SetPermission(index, isPublic);
                isPublicButton.SetImage(UIImage.FromBundle(isPublic ? "signupRadioButton" : "signupRadioButtonUn"), UIControlState.Normal);
			};
			input.AttributedPlaceholder = new NSAttributedString ("Type here", UIFont.FromName (BGAppearanceConstants.FontName, 14), UIColor.Black);

			input.EditingDidEndOnExit += (sender, e) => {
				viewController.SetValue(index, input.Text);
			};

			switch(section)
			{
			case 1:
				{
					input.Text = String.IsNullOrEmpty (viewController.GetValue (index)) ? String.Empty : viewController.GetValue (index);

					input.ShouldBeginEditing = delegate 
                            {
                                DateTime curTime = DateTime.Now - TimeSpan.FromDays(365 * 25);

                                var modalPicker = new ModalPickerViewController(ModalPickerType.Date, "Select A Date", viewController)
                                    {
                                        HeaderBackgroundColor = UIColor.Red,
                                        HeaderTextColor = UIColor.White,
                                        TransitioningDelegate = new ModalPickerTransitionDelegate(),
                                        ModalPresentationStyle = UIModalPresentationStyle.Custom
                                    };
                    
                                modalPicker.DatePicker.Date = (Foundation.NSDate)curTime;
                                modalPicker.DatePicker.Mode = UIDatePickerMode.Date;
                                modalPicker.DatePicker.MaximumDate = (Foundation.NSDate)(DateTime.Now - TimeSpan.FromDays(365 * 18));
                                modalPicker.DatePicker.MinimumDate = (Foundation.NSDate)(DateTime.Now - TimeSpan.FromDays(365 * 145));


                                modalPicker.OnModalPickerDismissed += (s, ea) => 
                                {
                                    var dateFormatter = new NSDateFormatter()
                                        {
                                                DateFormat ="MM/dd/yyyy"
                                        };
                                                    
                                        input.Text = dateFormatter.ToString(modalPicker.DatePicker.Date);
                                        viewController.SetValue(index, input.Text);
                                };

                                viewController.PresentViewController(modalPicker, true, null);

                                /*
        						viewController.BirthDatePicker = new ActionSheetDatePicker (viewController.View);
        						viewController.BirthDatePicker.Title = "Choose Date";
                                if (!String.IsNullOrEmpty(input.Text))
                                    DateTime.TryParse(input.Text, out curTime);

                                viewController.BirthDatePicker.DatePicker.Date = curTime;
                                
        						viewController.BirthDatePicker.DatePicker.Mode = UIDatePickerMode.Date;
                                viewController.BirthDatePicker.DatePicker.MaximumDate = DateTime.Now - TimeSpan.FromDays(365 * 18);
                                viewController.BirthDatePicker.DatePicker.MinimumDate = DateTime.Now - TimeSpan.FromDays(365 * 145);
        						NSDateFormatter dateFormatter = new NSDateFormatter();
        						dateFormatter.DateFormat = "MM/dd/yyyy";

        						//input.Text = DateTime.Now.ToString();

        						viewController.BirthDatePicker.DatePicker.ValueChanged += (s, e) => 
                                    {
            							NSDate dateValue = (s as UIDatePicker).Date;
            							input.Text = new NSString(dateFormatter.ToString(dateValue));
            							viewController.SetValue(index, input.Text);
        						    };

        						viewController.BirthDatePicker.Show ();
        						NSDate dv = viewController.BirthDatePicker.DatePicker.Date;
        						if(input.Text.Length == 0)
        							input.Text = new NSString(dateFormatter.ToString(dv));
*/
        						return false;
        					};

					input.ShouldReturn = delegate 
                            {
        						DateTime dt;
        						return DateTime.TryParse (input.Text, new DateTimeFormatInfo() { FullDateTimePattern = "mm/dd/yyyy"}, DateTimeStyles.None, out dt);
        					};
					input.AttributedPlaceholder = new NSAttributedString ("mm/dd/yyyy", UIFont.FromName (BGAppearanceConstants.FontName, 14), UIColor.Black);
					return;
				}
			
			case 3:
				{
			
					break;
				}
			case 4:
				{
			
					break;
				}
			default:
			case 5:
				{
			
					break;
				}
			}

			input.Text = String.IsNullOrEmpty (viewController.GetValue (index)) ? String.Empty : viewController.GetValue (index);
		}
	}
}
