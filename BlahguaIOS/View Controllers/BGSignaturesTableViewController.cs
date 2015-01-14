// This file has been autogenerated from a class added in the UI designer.

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;

using BlahguaMobile.BlahguaCore;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Drawing;

namespace BlahguaMobile.IOS
{
	public partial class BGSignaturesTableViewController : UITableViewController
	{
		public new UIViewController ParentViewController 
		{
			get;
			set;
		}

		public BGSignaturesTableViewController (IntPtr handle) : base (handle)
		{
		}

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

        }
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
 
            this.navBar .SetTitleTextAttributes  (new UITextAttributes () {
                Font = UIFont.FromName ("Merriweather", 20),
                TextColor = UIColor.FromRGB (96, 191, 164)
            });
             
            foreach (var theItem in navBar)
            {
                if (theItem is UIButton)
                {
                    UIButton theBtn = (UIButton)theItem;
                    theBtn.SetAttributedTitle(
                        new NSAttributedString(theBtn.Title(UIControlState.Normal).Replace("xx", ""), UIFont.FromName("Merriweather", 16) , BGAppearanceConstants.TealGreen)
                        , UIControlState.Normal);

                }
            }

			TableView.TableFooterView = new UIView ();
			TableView.Source = new BGSignaturesTableSource (this);
		}

		partial void Done (MonoTouch.UIKit.UIBarButtonItem sender)
		{
			DismissViewController(true, null);
		}

        partial void SelectAll(MonoTouch.UIKit.UIBarButtonItem sender)
		{
			int count = TableView.NumberOfRowsInSection(1);

			for(int i = 0; i < count; i++)
			{
				var ip = NSIndexPath.FromRowSection(i, 1);
				if (TableView.CellAt(ip).Accessory != UITableViewCellAccessory.Checkmark)
				{
					TableView.SelectRow(ip, true, UITableViewScrollPosition.None);
					TableView.Source.RowSelected(TableView, ip);
				}
			}
		}

		public class BGSignaturesTableSource : UITableViewSource
		{
			private BGSignaturesTableViewController vc;

			public BGSignaturesTableSource(BGSignaturesTableViewController vc) : base()
			{
				this.vc = vc;
			}

			#region implemented abstract members of UITableViewSource

			public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
			{
				var cell = (BGSignaturesCell)tableView.DequeueReusableCell ("cell");
				bool	decorate = false;

				if (indexPath.Section == 0) 
				{
                    switch (indexPath.Item)
                    {
                        case 0:
                            cell.SetUp("use public profile");
                            break;
                        case 1:
                            cell.SetUp("mature content");
                            break;
                    }
				}
				else
				{
					if (BlahguaAPIObject.Current.CurrentUser.Badges != null && BlahguaAPIObject.Current.CurrentUser.Badges.Count > 0)
					{
						cell.SetUp(BlahguaAPIObject.Current.CurrentUser.Badges[indexPath.Row].BadgeName);
						decorate = true;
					}
					else
					{
						cell.SetUp("you do not have any badges yet", false);
						cell.Accessory = UITableViewCellAccessory.None;
					}
				}
					
				if(vc.ParentViewController is BGNewPostViewController)
				{
					if(indexPath.Section == 0)
					{
                        switch (indexPath.Item)
                        {
                            case 0:
                                if (BlahguaAPIObject.Current.CreateRecord.UseProfile)
                                    cell.Accessory = UITableViewCellAccessory.Checkmark;
                                else
                                    cell.Accessory = UITableViewCellAccessory.None;
                                break;
                            case 1:
                                if (BlahguaAPIObject.Current.CreateRecord.IsMature)
                                    cell.Accessory = UITableViewCellAccessory.Checkmark;
                                else
                                    cell.Accessory = UITableViewCellAccessory.None;
                                break;
                        }

						
					}
					else if (decorate)
					{
						BadgeReference badge = BlahguaAPIObject.Current.CurrentUser.Badges [indexPath.Row];

						if ((BlahguaAPIObject.Current.CreateRecord.B != null) &&
							BlahguaAPIObject.Current.CreateRecord.B.Contains(badge.ID))
							cell.Accessory = UITableViewCellAccessory.Checkmark;
						else
							cell.Accessory = UITableViewCellAccessory.None ;
					}
				}
				else if(vc.ParentViewController is BGNewCommentViewController)
				{
					if(indexPath.Section == 0)
					{
                        switch (indexPath.Item)
                        {
                            case 0:
                                if (BlahguaAPIObject.Current.CreateCommentRecord.UseProfile)
                                    cell.Accessory = UITableViewCellAccessory.Checkmark;
                                else
                                    cell.Accessory = UITableViewCellAccessory.None;
                                break;
                            case 1:
                                if (BlahguaAPIObject.Current.CreateCommentRecord.IsMature)
                                    cell.Accessory = UITableViewCellAccessory.Checkmark;
                                else
                                    cell.Accessory = UITableViewCellAccessory.None;
                                break;
                        }
					}
					else if (decorate)
					{
						BadgeReference badge = BlahguaAPIObject.Current.CurrentUser.Badges [indexPath.Row];

						if ((BlahguaAPIObject.Current.CreateCommentRecord.BD != null) &&
							BlahguaAPIObject.Current.CreateCommentRecord.BD.Contains(badge.ID))
							cell.Accessory = UITableViewCellAccessory.Checkmark;
						else
							cell.Accessory = UITableViewCellAccessory.None ;
					}
				}

				return cell;
			}

			public override int RowsInSection (UITableView tableview, int section)
			{
				if (section == 0)
				{
					return 2;
				}
				else
				{
					int count = 0;
					if (BlahguaAPIObject.Current.CurrentUser.Badges != null)
					{
						count += BlahguaAPIObject.Current.CurrentUser.Badges.Count;
					}
					if (count == 0)
						count = 1; // add the warning
					return count;
				}
			}



			public override int NumberOfSections (UITableView tableView)
			{
				return 2;
			}


			public override float GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
			{
				if(indexPath.Section == 0)
				{
					return 36f;
				}
				else
				{
					return 28f;
				}
			}

			private void HandleCellClick(UITableView tableView, NSIndexPath indexPath)
			{
				UITableViewCell curCell = tableView.CellAt (indexPath);

				if (vc.ParentViewController is BGNewPostViewController) 
				{
					if (indexPath.Section == 0) 
					{
                        switch (indexPath.Item)
                        {
                            case 0:
                                if (BlahguaAPIObject.Current.CreateRecord.UseProfile)
                                {
                                    BlahguaAPIObject.Current.CreateRecord.UseProfile = false;
                                    curCell.Accessory = UITableViewCellAccessory.None;
                                }
                                else
                                {
                                    BlahguaAPIObject.Current.CreateRecord.UseProfile = true;
                                    curCell.Accessory = UITableViewCellAccessory.Checkmark;
                                }
                                break;
                            case 1:
                                if (BlahguaAPIObject.Current.CreateRecord.IsMature)
                                {
                                    BlahguaAPIObject.Current.CreateRecord.IsMature = false;
                                    curCell.Accessory = UITableViewCellAccessory.None;
                                }
                                else
                                {
                                    BlahguaAPIObject.Current.CreateRecord.IsMature = true;
                                    curCell.Accessory = UITableViewCellAccessory.Checkmark;
                                }
                                break;
                        }
					} 
					else 
					{
						if ((BlahguaAPIObject.Current.CurrentUser.Badges == null) ||
						    (BlahguaAPIObject.Current.CurrentUser.Badges.Count == 0))
						{
							return;
						}
						else
						{
							BadgeReference theBadge = BlahguaAPIObject.Current.CurrentUser.Badges[indexPath.Row];

							if (BlahguaAPIObject.Current.CreateRecord.B == null)
								BlahguaAPIObject.Current.CreateRecord.B = new List<string>();

							if (BlahguaAPIObject.Current.CreateRecord.B.Contains(theBadge.ID))
							{
								BlahguaAPIObject.Current.CreateRecord.B.Remove(theBadge.ID);
								curCell.Accessory = UITableViewCellAccessory.None;
							}
							else
							{
								BlahguaAPIObject.Current.CreateRecord.B.Add(theBadge.ID);
								curCell.Accessory = UITableViewCellAccessory.Checkmark;
							}
						}
					}
				} 
				else if (vc.ParentViewController is BGNewCommentViewController) 
				{
					if (indexPath.Section == 0) 
					{
                        switch (indexPath.Item)
                        {
                            case 0:
                                if (BlahguaAPIObject.Current.CreateCommentRecord.UseProfile)
                                {
                                    BlahguaAPIObject.Current.CreateCommentRecord.UseProfile = false;
                                    curCell.Accessory = UITableViewCellAccessory.None;
                                }
                                else
                                {
                                    BlahguaAPIObject.Current.CreateCommentRecord.UseProfile = true;
                                    curCell.Accessory = UITableViewCellAccessory.Checkmark;
                                }
                                break;
                            case 1:
                                if (BlahguaAPIObject.Current.CreateCommentRecord.IsMature)
                                {
                                    BlahguaAPIObject.Current.CreateCommentRecord.IsMature = false;
                                    curCell.Accessory = UITableViewCellAccessory.None;
                                }
                                else
                                {
                                    BlahguaAPIObject.Current.CreateCommentRecord.IsMature = true;
                                    curCell.Accessory = UITableViewCellAccessory.Checkmark;
                                }
                                break;
                        }
					} 
					else 
					{
						if ((BlahguaAPIObject.Current.CurrentUser.Badges == null) ||
						    (BlahguaAPIObject.Current.CurrentUser.Badges.Count == 0))
						{
							return;
						}
						else
						{
							BadgeReference theBadge = BlahguaAPIObject.Current.CurrentUser.Badges[indexPath.Row];

							if (BlahguaAPIObject.Current.CreateCommentRecord.BD == null)
								BlahguaAPIObject.Current.CreateCommentRecord.BD = new List<string>();

							if (BlahguaAPIObject.Current.CreateCommentRecord.BD.Contains(theBadge.ID))
							{
								BlahguaAPIObject.Current.CreateCommentRecord.BD.Remove(theBadge.ID);
								curCell.Accessory = UITableViewCellAccessory.None;
							}
							else
							{
								BlahguaAPIObject.Current.CreateCommentRecord.BD.Add(theBadge.ID);
								curCell.Accessory = UITableViewCellAccessory.Checkmark;
							}
						}
					}
				}
			}
				
			public override string TitleForHeader (UITableView tableView, int section)
			{
				if (section == 0)
					return "    description";
				else
					return "    badges";
			}


			public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
			{
				HandleCellClick(tableView, indexPath);
			}

			public override void RowDeselected (UITableView tableView, NSIndexPath indexPath)
			{
				HandleCellClick(tableView, indexPath);
			}

			#endregion


		}
	}
}
