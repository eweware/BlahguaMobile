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

			if(type == BGLeftMenuType.Channels)
			{
				var channel = BlahguaCore.BlahguaAPIObject.Current.CurrentChannelList.ElementAt (indexPath.Row);
				cell.Text = channel.ChannelName;
				if(BlahguaCore.BlahguaAPIObject.Current.CurrentChannel == null || 
					channel._id == BlahguaCore.BlahguaAPIObject.Current.CurrentChannel._id)
				{
					BlahguaCore.BlahguaAPIObject.Current.CurrentChannel = channel;
					cell.SelectRow ();
					tableView.SelectRow (indexPath, true, UITableViewScrollPosition.None);
				}
				else
				{
					cell.DeselectRow ();
				}
			}
			else
			{
				var blahType = BlahguaCore.BlahguaAPIObject.Current.CurrentBlahTypes.ElementAt(indexPath.Row);
				cell.Text = blahType.N;
//				BlahguaCore.BlahguaAPIObject.Current.CurrentBlah.ChannelName = BlahguaCore.BlahguaAPIObject.Current.CurrentChannel.ChannelName;
//				BlahguaCore.BlahguaAPIObject.Current.Cu ;
			}
			return cell;
		}

		public override int RowsInSection (UITableView tableView, int section)
		{
			if(type == BGLeftMenuType.Channels)
			{
				return BlahguaCore.BlahguaAPIObject.Current.CurrentChannelList.Count;
			}
			else
			{
				return BlahguaCore.BlahguaAPIObject.Current.CurrentBlahTypes.Count;
			}
		}

		public override int NumberOfSections (UITableView tableView)
		{
			return 1;
		}

		public override UIView GetViewForHeader (UITableView tableView, int section)
		{
			BGMenuTableHeaderView headerCell = (BGMenuTableHeaderView)tableView.DequeueReusableCell ("HeaderCell");
			if (type == BGLeftMenuType.Channels)
			{
				headerCell.Header = "Channels";
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
				cell.SelectRow ();
				if(type == BGLeftMenuType.Channels)
				{
					BlahguaCore.BlahguaAPIObject.Current.CurrentChannel = BlahguaCore.BlahguaAPIObject.Current.CurrentChannelList.ElementAt (indexPath.Row);

				}
				else
				{
					///BlahguaCore.BlahguaAPIObject.Current.Curr*/
				}
			}
			return indexPath;
		}

		public override NSIndexPath WillDeselectRow (UITableView tableView, NSIndexPath indexPath)
		{
			((BGMenuTableCellView)tableView.CellAt (indexPath)).DeselectRow ();
			return indexPath;
		}
	}
}

