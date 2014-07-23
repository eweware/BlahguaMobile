// This file has been autogenerated from a class added in the UI designer.

using System;
using System.Text;
using System.Globalization;
using System.Drawing;

using BlahguaMobile.BlahguaCore;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace BlahguaMobile.IOS
{
	public partial class BGDemographicsViewController : UITableViewController
	{
		public int index;

		public BGDemographicsViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			this.NavigationController.SetNavigationBarHidden(false, true);

			TableView.BackgroundColor = UIColor.FromRGB (255, 255, 248);
			TableView.TableFooterView = new UIView ();
			TableView.Source = new BGDemographicsTableSource (this);
			TableView.ReloadData ();

			NavigationItem.RightBarButtonItem = new UIBarButtonItem ("Done", UIBarButtonItemStyle.Done, (s, e) => {
				BlahguaAPIObject.Current.UpdateUserProfile((result) => {
					Console.WriteLine(result);
					InvokeOnMainThread(() => NavigationController.PopViewControllerAnimated(true));
				});
			});
		}

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
        }
       
        public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            base.PrepareForSegue(segue, sender);
            if (segue.Identifier == "fromDemographicsToItemSelection")
            {
                var vc = (BGItemsSelectionTableViewCotroller)segue.DestinationViewController;
                if (vc != null)
                {
                    vc.ParentViewController = this;
                    vc.index = index;

                    switch (index)
                    {
                        case 0:
                            {
                                vc.source = BlahguaAPIObject.Current.UserProfileSchema.GenderChoices;
                                break;
                            }

                        case 2:
                            {
                                vc.source = BlahguaAPIObject.Current.UserProfileSchema.RaceChoices;
                                break;
                            }
                        case 6:
                            {
                                vc.source = BlahguaAPIObject.Current.UserProfileSchema.CountryChoices;
                                break;
                            }
                        case 7:
                            {
                                vc.source = BlahguaAPIObject.Current.UserProfileSchema.IncomeChoices;
                                break;
                            }
                        default:
                            {
                                vc.source = BlahguaAPIObject.Current.UserProfileSchema.IncomeChoices;
                                break;
                            }
                    }
                    index = 0;
                }
                  base.PrepareForSegue(segue, sender);
        
            }
        }
		public void SetValue(int index, string value)
		{
			switch(index)
			{
			case 0:
				{
					BlahguaAPIObject.Current.CurrentUser.Profile.Gender = value;
					break;
				}
			case 1:
				{
					BlahguaAPIObject.Current.CurrentUser.Profile.DOB = DateTime.Parse (value, new DateTimeFormatInfo() { FullDateTimePattern = "mm/dd/yyyy"});
					break;
				}
			case 2:
				{
					BlahguaAPIObject.Current.CurrentUser.Profile.Race = value;
					break;
				}
			case 3:
				{
					BlahguaAPIObject.Current.CurrentUser.Profile.City = value;
					break;
				}
			case 4:
				{
					BlahguaAPIObject.Current.CurrentUser.Profile.State = value;
					break;
				}
			case 5:
				{
					BlahguaAPIObject.Current.CurrentUser.Profile.Zipcode = value;
					break;
				}
			case 6:
				{
					BlahguaAPIObject.Current.CurrentUser.Profile.Country = value;
					break;
				}
			case 7:
               
				{
					BlahguaAPIObject.Current.CurrentUser.Profile.Income = value;
					break;
				}
                default:
                {
                    BlahguaAPIObject.Current.CurrentUser.Profile.Income = value;
                    break;
                }
			}
		}

		public string GetValue(int index)
		{
			switch(index)
			{
			case 0:
				{
					return BlahguaAPIObject.Current.CurrentUser.Profile.Gender;
				}
			case 1:
				{
					if(BlahguaAPIObject.Current.CurrentUser.Profile.DOB != null)
					{
						return ((DateTime)BlahguaAPIObject.Current.CurrentUser.Profile.DOB).ToString ("mm/dd/yyyy");
					}
					else 
					{
						return String.Empty;
					}
				}
			case 2:
				{
					return BlahguaAPIObject.Current.CurrentUser.Profile.Race;
				}
			case 3:
				{
					return BlahguaAPIObject.Current.CurrentUser.Profile.City;
				}
			case 4:
				{
					return BlahguaAPIObject.Current.CurrentUser.Profile.State;
				}
			case 5:
				{
					return BlahguaAPIObject.Current.CurrentUser.Profile.Zipcode;
				}
			case 6:
				{
					return BlahguaAPIObject.Current.CurrentUser.Profile.Country;
				}
			case 7:
             
				{
					return BlahguaAPIObject.Current.CurrentUser.Profile.Income;
				}
                default:
                {
                    return BlahguaAPIObject.Current.CurrentUser.Profile.Income;
                }
			}
		}

		public void SetPermission(int index, bool perm)
		{
			switch(index)
			{
			case 0:
				{
					BlahguaAPIObject.Current.CurrentUser.Profile.GenderPerm = perm;
					break;
				}
			case 1:
				{
					BlahguaAPIObject.Current.CurrentUser.Profile.DOBPerm = perm;
					break;
				}
			case 2:
				{
					BlahguaAPIObject.Current.CurrentUser.Profile.RacePerm = perm;
					break;
				}
			case 3:
				{
					BlahguaAPIObject.Current.CurrentUser.Profile.CityPerm = perm;
					break;
				}
			case 4:
				{
					BlahguaAPIObject.Current.CurrentUser.Profile.StatePerm = perm;
					break;
				}
			case 5:
				{
					BlahguaAPIObject.Current.CurrentUser.Profile.ZipcodePerm = perm;
					break;
				}
			case 6:
				{
					BlahguaAPIObject.Current.CurrentUser.Profile.CountryPerm = perm;
					break;
				}
			case 7:
                
				{
					BlahguaAPIObject.Current.CurrentUser.Profile.IncomePerm = perm;
					break;
				}
                default:
                {
                    BlahguaAPIObject.Current.CurrentUser.Profile.IncomePerm = perm;
                    break;
                }
			}
		}

		public bool GetPermission(int index)
		{
			switch(index)
			{
			case 0:
				{
					return BlahguaAPIObject.Current.CurrentUser.Profile.GenderPerm;
				}
			case 1:
				{
					return BlahguaAPIObject.Current.CurrentUser.Profile.DOBPerm;
				}
			case 2:
				{
					return BlahguaAPIObject.Current.CurrentUser.Profile.RacePerm;
				}
			case 3:
				{
					return BlahguaAPIObject.Current.CurrentUser.Profile.CityPerm;
				}
			case 4:
				{
					return BlahguaAPIObject.Current.CurrentUser.Profile.StatePerm;
				}
			case 5:
				{
					return BlahguaAPIObject.Current.CurrentUser.Profile.ZipcodePerm;
				}
			case 6:
				{
					return BlahguaAPIObject.Current.CurrentUser.Profile.CountryPerm;
				}
			case 7:
                {
                    return BlahguaAPIObject.Current.CurrentUser.Profile.IncomePerm;

                }
                default:
				{
					return BlahguaAPIObject.Current.CurrentUser.Profile.IncomePerm;
                  
				}

			}
		}

		public void PushSelectingTable(int index)
		{
			this.index = index;
            PerformSegue("fromDemographicsToItemSelection", this);
		}
	}

	public class BGDemographicsTableSource : UITableViewSource
	{
		private BGDemographicsViewController vc;
		public BGDemographicsTableSource (BGDemographicsViewController vc)
		{
			this.vc = vc;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			switch(indexPath.Section)
			{
			case 1:
			case 3:
			case 4:
			case 5:
				{
					var cell = (BGDemographicsInputCell)tableView.DequeueReusableCell ("inputCell");
					cell.viewController = vc;
					cell.SetUp (indexPath.Section);
					return cell;
				}
			case 0:
			case 6:
			case 2:
			case 7:
                {
                    var cell = (BGDemographicsDropDownCell)tableView.DequeueReusableCell("dropdownCell");
                    cell.viewController = vc;
                    cell.SetUp(indexPath.Section);
                    return cell;
                }
			default:
				{
					var cell = (BGDemographicsDropDownCell)tableView.DequeueReusableCell ("dropdownCell");
					cell.viewController = vc;
					cell.SetUp (indexPath.Section);
					return cell;
				}
			}
		}

		public override float GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
		{
			return 52f;
		}

		public override UIView GetViewForHeader (UITableView tableView, int section)
		{
			UIView view = new UIView (new RectangleF (0, 0, 320, GetHeightForHeader (tableView, section)));
			view.BackgroundColor = UIColor.FromRGB (255, 255, 248);

			UILabel label = new UILabel (new RectangleF (20, 7, 280, 21));
			string labelText = String.Empty;
			switch(section)
			{
			case 0:
				{
					labelText = "Gender";
					break;
				}
			case 1:
				{
					labelText = "Date Of Birth";
					break;
				}
			case 2:
				{
					labelText = "Ethnicity";
					break;
				}
			case 3:
				{
					labelText = "City";
					break;
				}
			case 4:
				{
					labelText = "State";
					break;
				}
			case 5:
				{
					labelText = "Post code";
					break;
				}
			case 6:
				{
					labelText = "Country";
					break;
				}
			case 7:
			
				{
					labelText = "Income";
					break;
				}
			}
			label.AttributedText = new NSAttributedString (labelText, UIFont.FromName (BGAppearanceConstants.BoldFontName, 17), UIColor.Black);
			view.AddSubview (label);
			return view;
		}

		public override float GetHeightForHeader (UITableView tableView, int section)
		{
			return 31f;
		}

		public override int NumberOfSections (UITableView tableView)
		{
			return 8;
		}

		public override int RowsInSection (UITableView tableview, int section)
		{
			return 1;
		}
	}
}
