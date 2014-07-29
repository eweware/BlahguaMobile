// This file has been autogenerated from a class added in the UI designer.

using System;
using System.Collections;
using System.Collections.Generic;

using BlahguaMobile.BlahguaCore;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace BlahguaMobile.IOS
{
	public partial class BGItemsSelectionTableViewCotroller : UITableViewController
	{
		public List<string> source;
		public BGDemographicsViewController ParentViewController;
		public int index;

		public BGItemsSelectionTableViewCotroller (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			TableView.BackgroundColor = UIColor.White;
			TableView.TableFooterView = new UIView ();
			TableView.Source = new BGItemsSelectionTableSource (this);
		}
	}

	public class BGItemsSelectionTableSource : UITableViewSource
	{
		private BGItemsSelectionTableViewCotroller vc;

		public BGItemsSelectionTableSource (BGItemsSelectionTableViewCotroller vc)
		{
			this.vc = vc;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell("simpleCell");
			cell.TextLabel.AttributedText = new NSAttributedString(vc.source[indexPath.Row], UIFont.FromName(BGAppearanceConstants.FontName, 17), UIColor.Black);
			// Synsoft Global on 23 July 2014
			switch (vc.index)
			{
			case 0:
				{
					if (BlahguaAPIObject.Current.CurrentUser.Profile.Gender == vc.source[indexPath.Row])
					{

						cell.BackgroundColor = UIColor.LightGray;

					}
					break;
				}

			case 2:
				{
					if (BlahguaAPIObject.Current.CurrentUser.Profile.Race == vc.source[indexPath.Row])
					{

						cell.BackgroundColor = UIColor.LightGray;
					}
					break;
				}
			case 6:
				{
					if (BlahguaAPIObject.Current.CurrentUser.Profile.Country==vc.source[indexPath.Row])
					{

						cell.BackgroundColor = UIColor.LightGray;
					}
					break;
				}
			case 7:
				{
					if (BlahguaAPIObject.Current.CurrentUser.Profile.Income == vc.source[indexPath.Row])
					{

						cell.BackgroundColor = UIColor.LightGray;

					}
					break;
				}
			default:
				{
					if (BlahguaAPIObject.Current.CurrentUser.Profile.Income == vc.source[indexPath.Row])
					{

						cell.BackgroundColor = UIColor.LightGray;
					}
					break;
				}
			}



			return cell;
		}

		public override float GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
		{
			return 44f;
		}

		public override int NumberOfSections(UITableView tableView)
		{
			return 1;
		}

		public override int RowsInSection(UITableView tableview, int section)
		{
			return vc.source.Count;
		}
		public string value;

		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			vc.ParentViewController.SetValue(vc.index, vc.source[indexPath.Row]);
			vc.NavigationController.PopViewControllerAnimated(true);
		}
	}
}
