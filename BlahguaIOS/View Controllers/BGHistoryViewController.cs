// This file has been autogenerated from a class added in the UI designer.

using System;
using System.Drawing;

using BlahguaMobile.BlahguaCore;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace BlahguaMobile.IOS
{
	public partial class BGHistoryViewController : UITableViewController
	{
		public bool isComments;
		public bool isBlahs;

		public BlahList UserBlahs 
		{
			get;
			set;
		}

		public CommentList UserComments 
		{
			get;
			set;
		}

		public BGHistoryViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			this.NavigationController.SetNavigationBarHidden(false, true);

			Title = "History";
			isComments = false;
			isBlahs = false;
			TableView.Source = new BGHistoryTableSource (this);
			TableView.BackgroundColor = UIColor.White;
			TableView.TableFooterView = new UIView ();
			BlahguaAPIObject.Current.LoadUserPosts (BlahsLoaded);
			BlahguaAPIObject.Current.LoadUserComments (CommentsLoaded);
            AppDelegate.analytics.PostPageView("/self/history");
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
            this.Title = "History";
            this.NavigationController.NavigationBar .SetTitleTextAttributes  (new UITextAttributes () {
                Font = UIFont.FromName ("Merriweather", 20),
                TextColor = UIColor.FromRGB (96, 191, 164)
            });
			isComments = false;
			isBlahs = false;
			TableView.ReloadData ();
		}

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			((BGHistoryDetailViewController)segue.DestinationViewController).ParentViewController = this; 
		}

		public void SetMode(bool isComments, bool isBlahs)
		{
			this.isComments = isComments;
			this.isBlahs = isBlahs;
		}

		private void BlahsLoaded(BlahList blahs)
		{
			UserBlahs = blahs;
			InvokeOnMainThread (() => {
				TableView.ReloadData();
			});
		}

		private void CommentsLoaded(CommentList comments)
		{
			UserComments = comments;
			InvokeOnMainThread (() => {
				TableView.ReloadData();
			});
		}
	}
}
