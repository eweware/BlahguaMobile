using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace BlahguaMobile.IOS
{
	public class BGSlidingMenuTableSource : UITableViewSource
	{
		private BGLeftMenuType type;

		public BGSlidingMenuTableSource(BGLeftMenuType type)
		{
			this.type = type;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			BGMenuTableCellView cell = (BGMenuTableCellView)tableView.DequeueReusableCell ("SimpleRow");

			if(tableView.NumberOfSections() == 2)
			{
				if(indexPath.Section == 0)
				{
					var channel = BlahguaCore.BlahguaAPIObject.Current.CurrentChannelList.ElementAt (indexPath.Row);
					cell.Text = channel.ChannelName;
					if(indexPath.Row == 0)
					{
						BlahguaCore.BlahguaAPIObject.Current.CurrentChannel = channel;
						cell.SelectRow ();
						tableView.SelectRow (indexPath, true, UITableViewScrollPosition.None);
					}
				}
				else
				{
					var type = BlahguaCore.BlahguaAPIObject.Current.CurrentChannelTypeList.ElementAt (indexPath.Row);
					cell.Text = type.N;
				}
			}
			else
			{
				var type = BlahguaCore.BlahguaAPIObject.Current.CurrentBlahTypes.ElementAt(indexPath.Row);
				cell.Text = type.N;
//				BlahguaCore.BlahguaAPIObject.Current.CurrentBlah.ChannelName = BlahguaCore.BlahguaAPIObject.Current.CurrentChannel.ChannelName;
//				BlahguaCore.BlahguaAPIObject.Current.Cu ;
			}
			return cell;
		}

		public override int RowsInSection (UITableView tableView, int section)
		{
			if(tableView.NumberOfSections() == 2)
			{
				if(section == 0)
				{
					return BlahguaCore.BlahguaAPIObject.Current.CurrentChannelList.Count;
				} else
				{
					return BlahguaCore.BlahguaAPIObject.Current.CurrentChannelTypeList.Count;
				}
			}
			else
			{
				return BlahguaCore.BlahguaAPIObject.Current.CurrentBlahTypes.Count;
			}
		}

		public override int NumberOfSections (UITableView tableView)
		{
			if(type != BGLeftMenuType.BlahType)
			{
				return 2;
			}
			else
			{
				return 1;
			}
		}

		public override UIView GetViewForHeader (UITableView tableView, int section)
		{
			BGMenuTableHeaderView headerCell = (BGMenuTableHeaderView)tableView.DequeueReusableCell ("HeaderCell");
			if (tableView.NumberOfSections() == 2)
			{
				if (section == 0)
				{
					headerCell.Header = "Channels";
				} else if (section == 1)
				{
					headerCell.Header = "View";
				} 
			}
			else
			{
				headerCell.Header = "Type of post";
			}
			return headerCell;
		}

		public override NSIndexPath WillSelectRow (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = (BGMenuTableCellView)tableView.CellAt (indexPath);
			if(!cell.Selected)
			{
				var selectedCellsPaths = tableView.IndexPathsForSelectedRows;
				var currentSectionSelection = selectedCellsPaths == null ? null : 
					selectedCellsPaths.FirstOrDefault (scp => scp.Section == indexPath.Section && 
															  scp.Row != indexPath.Row);
				if(currentSectionSelection != null)
				{
					var currentSelectedCell = (BGMenuTableCellView)tableView.CellAt (currentSectionSelection);
					currentSelectedCell.DeselectRow ();
					tableView.DeselectRow (currentSectionSelection, false);
				}
			}
			BlahguaCore.BlahguaAPIObject.Current.CurrentChannel = BlahguaCore.BlahguaAPIObject.Current.CurrentChannelList.ElementAt (indexPath.Row);
			cell.SelectRow ();
			BlahguaCore.BlahguaAPIObject.Current.CurrentChannel = BlahguaCore.BlahguaAPIObject.Current.CurrentChannelList.ElementAt (indexPath.Row);
			if(type == BGLeftMenuType.Channels)
			{
				if(indexPath.Section == 0)
				{
					BlahguaCore.BlahguaAPIObject.Current.CurrentChannel = BlahguaCore.BlahguaAPIObject.Current.CurrentChannelList.ElementAt (indexPath.Row);
				}
				else
				{
					//BlahguaCore.BlahguaAPIObject.Current.CurrentChannel.ChannelTypeId = BlahguaCore.BlahguaAPIObject.Current.CurrentChannelList.ElementAt (indexPath.Row).ChannelTypeId;
				}
			}
			else
			{
				///BlahguaCore.BlahguaAPIObject.Current.Curr*/
			}
			return indexPath;
		}

		public override NSIndexPath WillDeselectRow (UITableView tableView, NSIndexPath indexPath)
		{
			var selectedCellsPaths = tableView.IndexPathsForSelectedRows;
			var currentSectionSelection = selectedCellsPaths == null ? null : 
				selectedCellsPaths.Where (scp => scp.Section == indexPath.Section);
			if (currentSectionSelection != null && 
				currentSectionSelection.Count() == 1 && 
				currentSectionSelection.First().Section == indexPath.Section && 
				currentSectionSelection.First().Row == indexPath.Row)
			{
				tableView.SelectRow (indexPath, false, UITableViewScrollPosition.None);
			}
			return null;
		}
	}
}

