// This file has been autogenerated from a class added in the UI designer.

using System;
using System.Drawing;

using BlahguaMobile.BlahguaCore;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

using Comment = BlahguaMobile.BlahguaCore.Comment;

namespace BlahguaMobile.IOS
{
	public partial class BGCommentsViewController : UIViewController
	{
		#region Fields

		private UIButton upVoteButton;
		private UIButton downVoteButton;

		private UIButton summaryButton;
		private UIButton commentsButton;
		private UIButton statsButton;

		private Comment currentComment = null;

		public UIViewController parentViewController;

		private bool isWriteMode { get; set; }

		private BGNewCommentViewController newCommentViewController;

		#endregion

		#region Properties

		public Blah CurrentBlah
		{
			get
			{
				return BlahguaAPIObject.Current.CurrentBlah;
			}
		}

		public Comment CurrentComment
		{
			get
			{
				return currentComment;
			}
			set
			{
				if (currentComment != value)
					currentComment = value;
			}
		}

		#endregion

		#region Contructors


		public BGCommentsViewController (IntPtr handle) : base (handle)
		{
		}
        public BGCommentsViewController()
        { 
        
        }

		#endregion

		#region View Controller

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			SetUpBaseLayout ();

			SetUpHeaderView ();

			SetUpContentView ();

			SetUpToolbar ();

            //Synsoft on 14 July 2014 for swiping between screens
            UISwipeGestureRecognizer objUISwipeGestureRecognizer2 = new UISwipeGestureRecognizer(SwipeToStatsController);
            objUISwipeGestureRecognizer2.Direction = UISwipeGestureRecognizerDirection.Left;
            this.View.AddGestureRecognizer(objUISwipeGestureRecognizer2);

            UISwipeGestureRecognizer objUISwipeGestureRecognizer = new UISwipeGestureRecognizer(SwipeToSummaryController);
            objUISwipeGestureRecognizer.Direction = UISwipeGestureRecognizerDirection.Right;
            this.View.AddGestureRecognizer(objUISwipeGestureRecognizer);  

			BlahguaAPIObject.Current.LoadBlahComments(CommentsLoaded);
            AppDelegate.analytics.PostPageView("/blah/comments");
		}

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear (animated);
			SetModeButtonsImages(UIImage.FromBundle("summary"), UIImage.FromBundle("comments_dark"), UIImage.FromBundle("stats"));


		}

		public void ReloadComments()
		{
			BlahguaAPIObject.Current.LoadBlahComments(CommentsLoaded);
		}
        //Synsoft on 14 July 2014
        private void SwipeToSummaryController()
        {
			((AppDelegate)UIApplication.SharedApplication.Delegate).swipeView.SwipeToRight ();
        }

        //Synsoft on 14 July 2014
        private void SwipeToStatsController()
        {
            //PerformSegue("fromCommentsToStats", this);
			((AppDelegate)UIApplication.SharedApplication.Delegate).swipeView.SwipeToLeft ();
        }

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			if(segue.Identifier == "fromCommentsToStats")
			{
				((BGStatsTableViewController)segue.DestinationViewController).SetParentViewController(parentViewController);
			}
			base.PrepareForSegue (segue, sender);
		}

		#endregion

		#region Methods

        //Synsoft on 9 July 2014 for back handler
        private void BackHandler(object sender, EventArgs args)
        {
            //Synsoft on 9 July 2014 for active color  #1FBBD1
            NavigationItem.LeftBarButtonItem.TintColor = UIColor.FromRGB(31, 187, 209);
            if (CurrentComment != null)
            {
                CurrentComment = null;
                contentView.ReloadData();
            }
            else
            {
                DismissViewController(true, null);
            }
            
           
        }

		private void SetUpBaseLayout()
		{
            View.BackgroundColor = UIColor.FromPatternImage (UIImage.FromBundle ("grayBack"));
			contentView.BackgroundColor = UIColor.White;
			contentView.ScrollEnabled = true;

			contentView.ClipsToBounds = true;
			contentView.ContentOffset = new PointF(0,0);
			contentView.BackgroundColor = UIColor.White;

			bottomToolbar.BackgroundColor = BGAppearanceConstants.TealGreen;  //UIColor.FromPatternImage (UIImage.FromBundle ("greenBack"));
			bottomToolbar.BarTintColor =  BGAppearanceConstants.TealGreen;  //UIColor.FromPatternImage (UIImage.FromBundle ("greenBack"));

		}

		public void SetUpHeaderView()
		{
			if(CurrentComment != null)
			{
				SetUpViewTitleForComment ();
			}
			else
			{
				SetUpViewTitleForBlah ();
			}
		}

		private void SetUpContentView()
		{
			contentView.ScrollEnabled = true;

			contentView.Source = new BGCommentsTableSource (CurrentComment == null ? 
				BGCommentsTableSourceType.Blah : BGCommentsTableSourceType.Comment, this);

			contentView.TableFooterView = new UIView ();

			//contentView.ContentSize = new SizeF (defaultWidthOfContent, currentYCoord);
		}

		public void SetUpToolbar()
		{
			bottomToolbar.TintColor = UIColor.Clear;
			//SetUpVotesButtons ();
			if (BlahguaAPIObject.Current.CurrentUser == null) {
				var btnSignInRect = new RectangleF (0, 0, 80, 60);
				var btnSignIn = new UIButton (UIButtonType.Custom);
				btnSignIn.Frame = btnSignInRect;
				btnSignIn.SetTitle ("Sign In", UIControlState.Normal);
                BGAppearanceHelper.SetButtonFont(btnSignIn, "Merriweather");
				btnSignIn.TouchUpInside += (object sender, EventArgs e) => {
					this.PerformSegue ("fromCommentsToLogin", this);
				};

				btnComment.CustomView = btnSignIn;
			} else {
				var btnSignInRect = new RectangleF (0, 0, 80, 60);
				var btnSignIn = new UIButton (UIButtonType.Custom);
				btnSignIn.Frame = btnSignInRect;
				btnSignIn.SetTitle ("Comment", UIControlState.Normal);
				btnSignIn.TouchUpInside += (object sender, EventArgs e) => {
					WriteCommentAction();
				};

				btnComment.CustomView = btnSignIn;
			}

			SetUpModesButtons ();
		}
		
		void _alert_Clicked(object sender, UIButtonEventArgs e)
		{
			this.PerformSegue("fromCommentsToLogin", this);
		}
		private void SetVoteButtonsImages()
		{
			int uv = CurrentComment != null ? CurrentComment.UserVote : CurrentBlah.uv;
			switch(uv)
			{
			case 1:
				{
                        SetVoteButtonsImages (UIImage.FromBundle ("arrow_up_dark"), UIImage.FromBundle ("arrow_down"));
					break;
				}
			case -1:
				{
                        SetVoteButtonsImages (UIImage.FromBundle ("arrow_up"), UIImage.FromBundle ("arrow_down_dark"));
					break;
				}
			default:
				{
                        SetVoteButtonsImages (UIImage.FromBundle ("arrow_up"), UIImage.FromBundle ("arrow_down"));
					break;
				}
			}
		}

		private void SetVoteButtonsImages(UIImage upVoteImage, UIImage downVoteImage)
		{
			upVoteButton.SetImage(upVoteImage
				.ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal), 
				UIControlState.Normal);
			downVoteButton.SetImage(downVoteImage
				.ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal), 
				UIControlState.Normal);
		}

		private void SetUpModesButtons ()
		{
			summaryButton = new UIButton (UIButtonType.Custom);
			summaryButton.Frame = new RectangleF (0, 0, 20, 16);
            summaryButton.SetImage (UIImage.FromBundle ("summary"), UIControlState.Normal);
			summaryButton.TouchUpInside += (object sender, EventArgs e) => {
                //SetModeButtonsImages(UIImage.FromBundle ("summary_dark"), UIImage.FromBundle ("comments"), UIImage.FromBundle ("stats"));

				((AppDelegate)UIApplication.SharedApplication.Delegate).swipeView.SwipeToRight ();
			};
			summaryView.CustomView = summaryButton;

			commentsButton = new UIButton (UIButtonType.Custom);
			commentsButton.Frame = new RectangleF (0, 0, 22, 19);
            commentsButton.SetImage (UIImage.FromBundle ("comments_dark"), UIControlState.Normal);
			commentsButton.TouchUpInside += (object sender, EventArgs e) => {
				if(CurrentComment != null)
				{
					CurrentComment = null;
					contentView.ReloadData();
				}
                SetModeButtonsImages(UIImage.FromBundle ("summary"), UIImage.FromBundle ("comments_dark"), UIImage.FromBundle ("stats"));
			};
			commentsView.CustomView = commentsButton;

			statsButton = new UIButton (UIButtonType.Custom);
			statsButton.Frame = new RectangleF (0, 0, 26, 17);
            statsButton.SetImage (UIImage.FromBundle ("stats"), UIControlState.Normal);
			statsButton.TouchUpInside += (object sender, EventArgs e) => {
                //SetModeButtonsImages(UIImage.FromBundle ("summary"), UIImage.FromBundle ("comments"), UIImage.FromBundle ("stats_dark"));
                //Commented by Synsoft on 9 July 2014
               // PerformSegue("fromCommentsToStats", this);
               
                //Synsoft on 9 July 2014 to add popup animation

				((AppDelegate)UIApplication.SharedApplication.Delegate).swipeView.SwipeToLeft ();
			};
			statsView.CustomView = statsButton;
		}

		private void SetModeButtonsImages(UIImage sumImage, UIImage commentsImage, UIImage statsImage)
		{
			commentsButton.SetImage (commentsImage.ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal), UIControlState.Normal);
			summaryButton.SetImage (sumImage.ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal), UIControlState.Normal);
			statsButton.SetImage (statsImage.ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal), UIControlState.Normal);
		}

		private void SetUpViewTitleForComment()
		{
            string title;
            int count;

            if (CurrentComment.HasComments)
            {
                count = CurrentComment.subComments.Count;
                if (count == 1)
                    title = "There is one comment";
                else
                    title = "There are " + count.ToString() + " comments";
            }
            else
                title = "There are no comments";

			SetUpViewTitle (title);
		}

		private void SetUpViewTitleForBlah()
		{
            string title;
            int count;

            if (CurrentBlah.Comments != null)
            {
                count = CurrentBlah.Comments.Count;
                if (count == 1)
                    title = "There is one comment";
                else
                    title = "There are " + count.ToString() + " comments";
            }
            else
                title = "There are no comments";

            SetUpViewTitle (title);

		}

		private void SetUpViewTitle(string title)
		{
			viewTitle.AttributedText = new NSAttributedString (title, 
				UIFont.FromName (BGAppearanceConstants.BoldFontName, 14), 
				UIColor.Black);
		}
          

		public void CommentsLoaded(CommentList comments)
		{
			InvokeOnMainThread(() => {

				CurrentBlah.Comments.CollectionChanged += (sender, e) => contentView.ReloadData();
				SetUpHeaderView();
				contentView.ReloadData ();
			});
		}

		private void WriteCommentAction()
		{
			UIView.BeginAnimations (null);
			UIView.SetAnimationDuration (0.5f);
			float newYCoordDiff = 0f;

			if(newCommentViewController == null)
			{
				newCommentViewController = (BGNewCommentViewController)((AppDelegate)UIApplication.SharedApplication.Delegate).MainStoryboard
						.InstantiateViewController ("BGNewCommentViewController");
				newCommentViewController.ParentViewController = this;
				AddChildViewController (newCommentViewController);
			}
			newCommentViewController.View.Frame = new RectangleF(new PointF (0, 44), newCommentViewController.View.Frame.Size);
			View.AddSubview (newCommentViewController.View);
            newYCoordDiff += 1000f;//246f
			isWriteMode = true;
			foreach(var subView in View.Subviews)
			{
				if(subView != newCommentViewController.View)
					subView.Frame = new RectangleF (new PointF (subView.Frame.X, subView.Frame.Y + newYCoordDiff), subView.Frame.Size);
			}
			UIView.CommitAnimations ();
		}

		public void SwitchNewCommentMode()
		{
			contentView.ReloadData ();
            SetUpHeaderView();
		}

		#endregion

		private enum BGCommentsTableSourceType
		{
			Blah,
			Comment
		}

		private class BGCommentsTableSource : UITableViewSource
		{
			private BGCommentsTableSourceType type;
			private BGCommentsViewController vc;

			public BGCommentsTableSource(BGCommentsTableSourceType type, BGCommentsViewController vc)
			{
				this.type = type;
				this.vc = vc;
			}

			public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
			{
				var cell = (BGCommentTableCell)tableView.DequeueReusableCell ("commentWithImage");

				cell.SetUp (type == BGCommentsTableSourceType.Blah ? 
					vc.CurrentBlah.Comments [indexPath.Row] : 
					vc.CurrentComment.subComments [indexPath.Row], tableView);			
				return cell;
			}

			public override int NumberOfSections (UITableView tableView)
			{
				return 1;
			}

			public override int RowsInSection (UITableView tableview, int section)
			{
				if(type == BGCommentsTableSourceType.Blah)
				{
					if (vc.CurrentBlah.Comments != null)
						return vc.CurrentBlah.Comments.Count;
				}
				else
				{
					if(vc.CurrentComment.HasComments)
					{
						return vc.CurrentComment.subComments.Count;
					}
				}
				return 0;
			}

			public override float GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
			{
				var cell = (BGCommentTableCell)tableView.DequeueReusableCell ("commentWithImage");
				cell.SetUp (type == BGCommentsTableSourceType.Blah ? 
					vc.CurrentBlah.Comments [indexPath.Row] : 
					vc.CurrentComment.subComments [indexPath.Row], tableView);	
				cell.SetNeedsUpdateConstraints();
				cell.UpdateConstraintsIfNeeded();
				cell.Bounds = RectangleF.FromLTRB (0, 0, 320, 64);
				cell.SetNeedsLayout();
				cell.LayoutIfNeeded();
				return cell.ContentView.SystemLayoutSizeFittingSize(UIView.UILayoutFittingCompressedSize).Height + 1;
			}

			public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
			{
				vc.CurrentComment = vc.CurrentBlah.Comments [indexPath.Row];

				tableView.ReloadData ();
			}
		}
	}

	public class AlertDelegateForComment : UIAlertViewDelegate
	{
		public override void Clicked(UIAlertView alertview, int buttonIndex)
		{
			base.Clicked(alertview, buttonIndex);

		}
	}
}
