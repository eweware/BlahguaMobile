// This file has been autogenerated from a class added in the UI designer.

using System;
using System.Drawing;

using BlahguaMobile.BlahguaCore;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.Dialog.Utilities;



namespace BlahguaMobile.IOS
{
	public class BGHistoryTableSource : UITableViewSource
	{
		private BGHistoryViewController vc;

		public BGHistoryTableSource(BGHistoryViewController vc) : base()
		{
			this.vc = vc;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = (BGHistoryTableCell)tableView.DequeueReusableCell ("historyCell");
			if(indexPath.Row == 0)
			{
				int count = vc.UserBlahs == null ? 0 : vc.UserBlahs.Count;
				cell.SetUp ("Blahs", count.ToString());
			}
			else
			{
				int count = vc.UserComments == null ? 0 : vc.UserComments.Count;
				cell.SetUp ("Comments", count.ToString());
			}
			return cell;
		}

		public override float GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
		{
			return 64f;
		}

		public override int NumberOfSections (UITableView tableView)
		{
			return 1;
		}

		public override int RowsInSection (UITableView tableview, int section)
		{
			return 2;
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			if(indexPath.Row == 0)
			{
				vc.SetMode (false, true);
			}
			else
			{
				vc.SetMode (true, false);
			}
			vc.PerformSegue ("fromBhushanToYogi",vc);
	   }
	}

}
