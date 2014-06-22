// This file has been autogenerated from a class added in the UI designer.

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections;
using System.Collections.Generic;

using BlahguaMobile.BlahguaCore;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace BlahguaMobile.IOS
{
	public partial class BGStatsTableViewController : UITableViewController
	{
		private UIViewController parentViewController;

		private Blah CurrentBlah
		{
			get
			{
				return ((AppDelegate)UIApplication.SharedApplication.Delegate).CurrentBlah;
			}
		}

		public BGStatsTableViewController (IntPtr handle) : base (handle)
		{

		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			Dictionary<string, string> source = new Dictionary<string, string> ();
			if(CurrentBlah != null)
			{
				source.Add ("Opened: ", CurrentBlah.O.ToString ());
				source.Add ("Viewed: ", CurrentBlah.V.ToString ());
				source.Add ("Promotes: ", CurrentBlah.P.ToString ());
				source.Add ("Demotes: ", CurrentBlah.D.ToString ());
				source.Add ("Comments: ", CurrentBlah.C.ToString ());
			}
			else if(BlahguaAPIObject.Current.CurrentUser != null && BlahguaAPIObject.Current.CurrentUser.UserInfo != null)
			{
				var userInfo = BlahguaAPIObject.Current.CurrentUser.UserInfo;
				int userViews, opens, creates, comments, views;
				userViews = opens = creates = comments = views = 0;

				for(int i = 0; i < userInfo.DayCount; i++)
				{
					userViews += userInfo.UserViews (i);
					opens += userInfo.Opens (i);
					creates += userInfo.UserCreates (i);
					comments += userInfo.UserComments (i);
					views += userInfo.Views (i);
				}
				source.Add ("Openes: ", opens.ToString());
				source.Add ("Viewes: ", views.ToString());
				source.Add ("Creates: ", creates.ToString ());
				source.Add ("User Views: ", userViews.ToString ());
				source.Add ("Comments: ", comments.ToString ());
			}
			TableView.Source = new BGStatsTableSource (source);
		}

		public void SetParentViewController (UIViewController parentViewController)
		{
			this.parentViewController = parentViewController;
		}
	}

	public class BGStatsTableSource : UITableViewSource
	{
		private Dictionary<string, string> source;

		public BGStatsTableSource(Dictionary<string, string> source) : base()
		{
			this.source = source;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = (BGStatsTableCell)tableView.DequeueReusableCell ("cell");
			var element = source.ElementAt (indexPath.Row);
			cell.SetKeyValue (element.Key, element.Value);
			return cell;
		}

		public override int RowsInSection (UITableView tableview, int section)
		{
			return source.Count;
		}

		public override int NumberOfSections (UITableView tableView)
		{
			return 1;
		}
	}
}
