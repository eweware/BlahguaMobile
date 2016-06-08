// This file has been autogenerated from a class added in the UI designer.

using System;
using CoreGraphics;

using BlahguaMobile.BlahguaCore;

using Foundation;
using UIKit;
using System.Linq;

using System.Collections.Generic;

namespace BlahguaMobile.IOS
{
	public partial class BGHistoryViewController : UITableViewController
	{
		public bool isComments;
		public bool isBlahs;

        public List<Blah> UserBlahs 
		{
			get;
			set;
		}

        public List<Comment> UserComments 
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
            NavigationItem.LeftBarButtonItem = new UIBarButtonItem("Back", UIBarButtonItemStyle.Plain, (s, e)=> 
                {
                    this.NavigationController.PopViewController(true);
                });
            NavigationItem.LeftBarButtonItem.SetTitleTextAttributes(new UITextAttributes
                { 
                    TextColor = BGAppearanceConstants.TealGreen, 
                    TextShadowColor = UIColor.Clear, 
                    Font = UIFont.FromName("Merriweather", 16) 
                }, UIControlState.Normal);


		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
            this.Title = "History";
            this.NavigationController.NavigationBar.TitleTextAttributes  = new UIStringAttributes () {
                Font = UIFont.FromName ("Merriweather", 20),
                ForegroundColor = UIColor.FromRGB (96, 191, 164)
            };
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
            UserBlahs = blahs.OrderByDescending(b => b.cdate).ToList();
			InvokeOnMainThread (() => {
				TableView.ReloadData();
			});
		}

		private void CommentsLoaded(CommentList comments)
		{
            UserComments = comments.OrderByDescending(b => b.c).ToList();;
			InvokeOnMainThread (() => {
				TableView.ReloadData();
			});
		}
	}
}
