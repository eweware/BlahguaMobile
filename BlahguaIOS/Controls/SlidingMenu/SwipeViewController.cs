using System;
using System.Collections.Generic;
using UIKit;
using Foundation;
using CoreAnimation;
using CoreGraphics;
using BlahguaMobile.IOS;
using BlahguaMobile.BlahguaCore;
using ServiceStack.Text;


namespace MonoTouch.SlideMenu
{
	public enum VIEW_TYPE
	{
		SUMMARY_VIEW = 0,
		COMMENT_VIEW,
		STATS_VIEW
	}
	public class SwipeViewController : UIViewController
	{
		const float WIDTH_OF_CONTENT_VIEW_VISIBLE = 160.0f;
		const float ANIMATION_DURATION = 0.3f;
		const float SCALE = 1.0f;

		private VIEW_TYPE view_type;

		public BGCommentsViewController commentViewController;
		public BGStatsTableViewController statsViewController;
		public BGBlahViewController summaryViewController;

		CGRect leftFrame ;
		CGRect centerFrame ;
		CGRect rightFrame;

		private static string ChannelName;
		public static int NewMessageCount = 0;

		public SwipeViewController (BGBlahViewController blahView,BGCommentsViewController commentView, BGStatsTableViewController statsView)
		{
			this.SetCommentViewController(commentView);
			this.SetStatsViewController  (statsView);
			this.SetSummaryViewController(blahView);
		}

		protected override void Dispose (bool disposing)
		{
			if (disposing) {
				commentViewController.Dispose();
				statsViewController.Dispose ();
			summaryViewController.Dispose ();

			summaryViewController = null;
			commentViewController = null;
			statsViewController = null;

			}

			base.Dispose (disposing);
		}

		// - (void)setMenuViewController:(UIViewController *)menuViewController
		public void SetCommentViewController (BGCommentsViewController controller)
		{
			if (commentViewController != controller) {

				if (commentViewController != null) {
					commentViewController.WillMoveToParentViewController (null);
					commentViewController.RemoveFromParentViewController ();
					commentViewController.Dispose ();
				}

				commentViewController = controller;
				AddChildViewController (commentViewController);
				commentViewController.DidMoveToParentViewController (this);
			}
		}
		public void SetStatsViewController (BGStatsTableViewController controller)
		{
			if (statsViewController != controller) {

				if (statsViewController != null) {
					statsViewController.WillMoveToParentViewController (null);
					statsViewController.RemoveFromParentViewController ();
					statsViewController.Dispose ();
				}

				statsViewController = controller;
				AddChildViewController (statsViewController);
				statsViewController.DidMoveToParentViewController (this);
			}
		}
		// - (void)setContentViewController:(UIViewController *)contentViewController
		public void SetSummaryViewController (BGBlahViewController controller)
		{
			if (summaryViewController != controller) 
			{
				if (summaryViewController != null) {
					summaryViewController.WillMoveToParentViewController(null);
					summaryViewController.RemoveFromParentViewController();
					summaryViewController.Dispose();
				}

				summaryViewController = controller;
				summaryViewController.WillMoveToParentViewController(this);
				AddChildViewController(summaryViewController);
				summaryViewController.DidMoveToParentViewController(this);
			}
		}


		public BGBlahViewController SummaryViewController
		{
			get{
				return summaryViewController;
			}
		}
		// #pragma mark - View lifecycle

		// - (void)viewDidLoad
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			//leftFrame = new RectangleF (-View.Bounds.Width, View.Bounds.Y + 44, View.Bounds.Width, View.Bounds.Height - 44);
			leftFrame = new CGRect (View.Bounds.Width, View.Bounds.Y + 44, View.Bounds.Width, View.Bounds.Height - 44);
			centerFrame = new CGRect (View.Bounds.X, View.Bounds.Y + 44, View.Bounds.Width, View.Bounds.Height - 44);
			rightFrame = new CGRect (View.Bounds.Width, View.Bounds.Y + 44, View.Bounds.Width, View.Bounds.Height - 44);
			summaryViewController.View.Frame = centerFrame;
			commentViewController.View.Frame = leftFrame;
			statsViewController.View.Frame = rightFrame;

			View.AddSubview(summaryViewController.View);
			View.AddSubview (commentViewController.View);
			View.AddSubview (statsViewController.View);

			view_type = VIEW_TYPE.SUMMARY_VIEW;
			this.NavigationItem.Title = @"Summary";

			NavigationItem.LeftBarButtonItem = new UIBarButtonItem("Back", UIBarButtonItemStyle.Plain, BackHandler);
            NavigationItem.LeftBarButtonItem.SetTitleTextAttributes(new UITextAttributes
                { 
                    TextColor = BGAppearanceConstants.TealGreen, 
                    TextShadowColor = UIColor.Clear, 
                    Font = UIFont.FromName("Merriweather", 16) 
                }, UIControlState.Normal);
		}

		// - (void)viewWillAppear:(BOOL)animated
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

		}

		public UIViewController	CurrentController
		{
			get {
				if (view_type == VIEW_TYPE.SUMMARY_VIEW) {
					return summaryViewController;

				} else if (view_type == VIEW_TYPE.COMMENT_VIEW) {
					return commentViewController;
				} else if (view_type == VIEW_TYPE.STATS_VIEW) {
					return statsViewController;
				} else return null;
			}
		}

		public void NotifyNewComment(Comment theComment)
		{
			// post the notification on the new comment
			PublishAction theAction = new PublishAction();
			theAction.action = "comment";
			theAction.commentid = theComment._id;
			theAction.userid = BlahguaAPIObject.Current.CurrentUser._id;
			string theStr = theAction.ToJson<PublishAction>();

			BGRollViewController.pubnub.Publish<PublishAction>(ChannelName, theAction, DisplayPublishReturnMessage, DisplayErrorMessage);
			BGRollViewController.NotifyBlahActivity ();

		}
		private void DisplayPublishReturnMessage(PublishAction theMsg)
		{
			Console.WriteLine("[pubnub] publish: " + theMsg);
		}

		public void SubscribeToBlahChannel()
		{
			if (BlahguaAPIObject.Current.CurrentBlah != null)
			{
				long blahId = BlahguaAPIObject.Current.CurrentBlah._id;

				ChannelName = "blah" + blahId.ToString();
				BGRollViewController.pubnub.Subscribe<string>(ChannelName, DisplaySubscribeReturnMessage, DisplaySubscribeConnectStatusMessage, DisplayErrorMessage);
				BGRollViewController.pubnub.Presence<string>(ChannelName, DisplayPresenceReturnMessage, DisplayPresenceConnectStatusMessage, DisplayErrorMessage);
			}
		}

		public void UnsubscribeFromBlahChannel()
		{
			if (BlahguaAPIObject.Current.CurrentBlah != null)
			{
				BGRollViewController.pubnub.Unsubscribe<string>(ChannelName, DisplayUnsubscribeReturnMessage, DisplaySubscribeConnectStatusMessage, DisplaySubscribeDisconnectStatusMessage, DisplayErrorMessage);
				BGRollViewController.pubnub.PresenceUnsubscribe<string>(ChannelName, DisplayUnsubscribeReturnMessage, DisplayPresenceConnectStatusMessage, DisplayPresenceDisconnectStatusMessage, DisplayErrorMessage);
			}

		}


		private void DisplaySubscribeReturnMessage(string theMsg)
		{
			try
			{
				string jsonMsg = theMsg.Substring(theMsg.IndexOf("{"), theMsg.IndexOf("}"));
				PublishAction theAction = jsonMsg.FromJson<PublishAction>();
				BGCommentsViewController CommentsView = ((AppDelegate)UIApplication.SharedApplication.Delegate).swipeView.commentViewController ;

				if ((theAction != null) && (CommentsView != null) && theAction.action == "comment" )
				{
					CommentsView.ShowComment(theAction);
					IncrementMessageCount(1);
				}
			}
			catch (Exception exp)
			{
				Console.WriteLine("[pubnub] subscribe: invalid PublishAction " + theMsg);
			}
		}

		public void IncrementMessageCount(int numMessages)
		{
			MonoTouch.SlideMenu.SwipeViewController swipeView = ((AppDelegate)UIApplication.SharedApplication.Delegate).swipeView ;

			if (swipeView.CurrentController != swipeView.commentViewController)
			{
				NewMessageCount += numMessages;
				UpdateMessageCountIndicator();
			}
			else
				NewMessageCount = 0;
		}

		public void ClearMessageCount()
		{
			NewMessageCount = 0;
			UpdateMessageCountIndicator();
		}

		private void UpdateMessageCountIndicator()
		{
			/*
				LinearLayout tabHolder = tabs.GetChildAt(0) as LinearLayout;
				LinearLayout tab = tabHolder.GetChildAt(1) as LinearLayout;
				if (tab != null)
				{
					RunOnUiThread(() => {

						var counter = tab.FindViewById<TextView>(Resource.Id.counter);
						if (NewMessageCount == 0)
							counter.Visibility = ViewStates.Gone;
						else
						{
							counter.Visibility = ViewStates.Visible;
							counter.Text = NewMessageCount.ToString();
						}

					});
				}
				*/
		}



		private void DisplaySubscribeConnectStatusMessage(string theMsg)
		{
			Console.WriteLine("[pubnub] sub connect: " + theMsg);
		}

		private void DisplayPresenceConnectStatusMessage(string theMsg)
		{
			Console.WriteLine("[pubnub] presence connect: " + theMsg);
		}

		private void DisplaySubscribeDisconnectStatusMessage(string theMsg)
		{
			Console.WriteLine("[pubnub] sub disconnect: " + theMsg);
		}

		private void DisplayPresenceDisconnectStatusMessage(string theMsg)
		{
			Console.WriteLine("[pubnub] presence disconnect: " + theMsg);
		}

		private void DisplayErrorMessage(PubNubMessaging.Core.PubnubClientError pubnubError)
		{
			Console.WriteLine("[pubnub] Error: " + pubnubError.Message);
		}


		private void DisplayUnsubscribeReturnMessage(string theMsg)
		{
			Console.WriteLine("[pubnub] unsubscribe: " + theMsg);
		}


		private void DisplayPresenceReturnMessage(string theMsg)
		{
			try
			{
				string jsonMsg = theMsg.Substring(theMsg.IndexOf("{"), theMsg.IndexOf("}"));
				PresenceMessage msg = jsonMsg.FromJson<PresenceMessage>();
				if ((msg != null) && (msg.occupancy > 0))
				{
					string countStr = "people";
					if (msg.occupancy == 1)
						countStr = "person";
					string toastStr = string.Format("{0} {1} viewing post", msg.occupancy, countStr);
					InvokeOnMainThread(() =>
						{
							ToastIOS.Toast.MakeText(toastStr, ToastIOS.Toast.LENGTH_LONG).Show();
						});
				}
			}
			catch (Exception exp)
			{
				Console.WriteLine("[pubnub] presence err: " + theMsg);
			}
		}



		private void BackHandler(object sender, EventArgs args)
		{
			this.NavigationController.PopViewController(true);
		}


		// - (void)viewDidAppear:(BOOL)animated
		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
			this.NavigationController.SetNavigationBarHidden (false, false);
			summaryViewController.AdjustViewLayout ();

			summaryViewController.SetUpToolbar ();
			commentViewController.SetUpToolbar ();
			statsViewController.SetUpToolbar ();

			UIApplication.SharedApplication.SetStatusBarHidden (true, false);
			SubscribeToBlahChannel ();
		}

		// - (void)viewWillDisappear:(BOOL)animated
		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
		}

		// - (void)viewDidDisappear:(BOOL)animated
		public override void ViewDidDisappear (bool animated)
		{
			UnsubscribeFromBlahChannel ();
			base.ViewDidDisappear (animated);

		}

		public void SwipeToLeft()
		{
			if (view_type == VIEW_TYPE.SUMMARY_VIEW) {
				summaryViewController.View.Frame = leftFrame;
				commentViewController.View.Frame = centerFrame;

				view_type = VIEW_TYPE.COMMENT_VIEW;
				this.NavigationItem.Title = "Comments";

			} else if (view_type == VIEW_TYPE.COMMENT_VIEW) {
				commentViewController.View.Frame = leftFrame;
				statsViewController.View.Frame = centerFrame;

				view_type = VIEW_TYPE.STATS_VIEW;

				this.NavigationItem.Title = "Statistics";
			}
		}

		public void SwipeToRight()
		{
			if (view_type == VIEW_TYPE.STATS_VIEW) {
				statsViewController.View.Frame = rightFrame;

				commentViewController.View.Frame = centerFrame;

				view_type = VIEW_TYPE.COMMENT_VIEW;

				this.NavigationItem.Title = "Comments";

			} else if (view_type == VIEW_TYPE.COMMENT_VIEW) {

				commentViewController.View.Frame = rightFrame;
				summaryViewController.View.Frame = centerFrame;
				view_type = VIEW_TYPE.SUMMARY_VIEW;
				this.NavigationItem.Title = "Summary";
			}
		}

		public void SwipeFromSummaryToStats()
		{
			if (view_type == VIEW_TYPE.SUMMARY_VIEW) {

				summaryViewController.View.Frame = leftFrame;
				statsViewController.View.Frame = centerFrame;

				view_type = VIEW_TYPE.STATS_VIEW;
                this.NavigationItem.Title = "Statistics";
			}
		}
		public void SwipeFromStatsToSummary()
		{
			if (view_type == VIEW_TYPE.STATS_VIEW) {

				statsViewController.View.Frame = rightFrame;
				summaryViewController.View.Frame = centerFrame;

				view_type = VIEW_TYPE.SUMMARY_VIEW;
				this.NavigationItem.Title = "Summary";
			}
		}

		// #pragma mark - Appearance & rotation callbacks


		// - (BOOL)shouldAutomaticallyForwardAppearanceMethods
		public override bool ShouldAutomaticallyForwardAppearanceMethods {
			get {
				return false;
			}
		}

		// - (BOOL)shouldAutomaticallyForwardRotationMethods
		public override bool ShouldAutomaticallyForwardRotationMethods {
			get {
				return true;
			}
		}

		// - (BOOL)automaticallyForwardAppearanceAndRotationMethodsToChildViewControllers

		public override bool AutomaticallyForwardAppearanceAndRotationMethodsToChildViewControllers {
			get {
				return false;
			}
		}
	}
}

		// #pragma mark - Rotation
