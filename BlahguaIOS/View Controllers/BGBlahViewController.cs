
// This file has been autogenerated from a class added in the UI designer.

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;

using BlahguaMobile.BlahguaCore;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.Dialog.Utilities;

namespace BlahguaMobile.IOS
{
    public partial class BGBlahViewController : UIViewController, IImageUpdated
    {
        #region Fields
		private bool badgesShown = true;

        private UITableView tableView;


        private UIButton upVoteButton;
        private UIButton downVoteButton;

        private UIButton summaryButton;
        private UIButton commentsButton;
        private UIButton statsButton;

        #endregion

        #region Properties

        private Blah CurrentBlah
        {
            get
            {
                return BlahguaAPIObject.Current.CurrentBlah;
            }
        }

        public bool ShouldMoveToStats { get; set; }

        #endregion

        #region Contructors

        public BGBlahViewController(IntPtr handle)
            : base(handle)
        {
        }

        #endregion

        #region View Controller

        public override void ViewDidLoad()
        {
            AppDelegate.analytics.PostPageView("/blah");
            base.ViewDidLoad();

            //Synsoft on 9 July 2014 added title
            this.Title = "Summary";

            SetUpBaseLayout();

            SetUpHeaderView();

            SetUpContentView();

            SetUpToolbar();

            //Synsoft on 14 July 2014 for swipping between screens

            UISwipeGestureRecognizer objUISwipeGestureRecognizer = new UISwipeGestureRecognizer(SwipeToCommentController);
            objUISwipeGestureRecognizer.Direction = UISwipeGestureRecognizerDirection.Left;
            this.View.AddGestureRecognizer(objUISwipeGestureRecognizer);

			NSAction showFullScreen = () => {
				if(blahImage.Image != null)
				{
					FullScreenView fs = new FullScreenView(blahImage.Image);

					((AppDelegate)UIApplication.SharedApplication.Delegate).swipeView.NavigationController.PushViewController(fs,false);
				}

			};

			UITapGestureRecognizer imageTapRecognizer = new UITapGestureRecognizer (showFullScreen);
			imageTapRecognizer.Delegate = new PanGestureRecognizerDelegate ();
			imageTapRecognizer.NumberOfTapsRequired = 1;
			blahImage.AddGestureRecognizer (imageTapRecognizer);
        }

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear (animated);

			SetModeButtonsImages(UIImage.FromBundle("summary_dark"), UIImage.FromBundle("comments"), UIImage.FromBundle("stats"));

			commentsButton.SetImage (UIImage.FromBundle ("comments"), UIControlState.Normal);

			statsButton.SetImage (UIImage.FromBundle ("stats"), UIControlState.Normal);

		}

        //Synsoft on 14 July 2014 
        private void SwipeToCommentController()
        {
			((AppDelegate)UIApplication.SharedApplication.Delegate).swipeView.SwipeToLeft ();
        }

        //Synsoft on 11 July 2014            
        private void BackHandler(object sender, EventArgs args)
        {
            DismissViewController(true, null);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            if (ShouldMoveToStats)
            {
                ShouldMoveToStats = false;
            }

            contentView.ContentSize = new SizeF(320, tableView == null ? blahBodyView.Frame.Bottom : tableView.Frame.Bottom);
        }

        public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            if (segue.Identifier == "fromBlahViewToComments")
            {
                var vc = (BGCommentsViewController)segue.DestinationViewController;
                BlahguaAPIObject.Current.LoadBlahComments(vc.CommentsLoaded);
                vc.parentViewController = this;
            }
				
            base.PrepareForSegue(segue, sender);
        }

        #endregion

        #region Methods

        private void SetUpBaseLayout()
        {
            View.BackgroundColor = UIColor.FromPatternImage(UIImage.FromBundle("grayBack"));
            contentView.BackgroundColor = UIColor.White;
            contentView.ScrollEnabled = true;
            //bottomToolbar.TranslatesAutoresizingMaskIntoConstraints = true;

            contentView.ClipsToBounds = true;
            contentView.ContentOffset = new PointF(0, 0);
            contentView.BackgroundColor = UIColor.White;

			bottomToolbar.BackgroundColor =  BGAppearanceConstants.TealGreen; //UIColor.FromPatternImage(UIImage.FromBundle("greenBack"));
			bottomToolbar.BarTintColor = BGAppearanceConstants.TealGreen; //UIColor.FromPatternImage(UIImage.FromBundle("greenBack"));

            View.AddSubviews(new UIView[] { contentView, bottomToolbar });
        }

        private void SetUpHeaderView()
        {
            userImage.Image = ImageLoader.DefaultRequestImage(new Uri(CurrentBlah.UserImage),
                new ImageUpdateDelegate(userImage));

            SetAuthorName();
            SetAuthorDescription();


            badgesTableView.Source = new BGBlahBadgesTableSource();
            if (CurrentBlah.B != null && CurrentBlah.B.Any())
            {
                //badgeImage.Hidden = false;ƒ

				var count = 0;
				if(CurrentBlah.B != null)
					count = CurrentBlah.B.Count ;

				badgeTableHeight.Constant = count * 28;

				badgesTableView.ReloadData();
            }
            else
            {
                //badgeImage.Hidden = true;

				badgeTableHeight.Constant = 0;
            }

            blahTimespan.AttributedText = new NSAttributedString(
                CurrentBlah.ElapsedTimeString ?? "",
                UIFont.FromName(BGAppearanceConstants.MediumItalicFontName, 10),
                UIColor.Black
            );
        }
		private void AdjustBadgesTableView()
        {
            if (badgesShown)
            {
                badgeTableHeight.Constant = 0;
            }
            else
            {
                var count = badgesTableView.NumberOfRowsInSection(0);
				badgeTableHeight.Constant = count <= 1 ? 28 : 56;
            }
            badgesShown = !badgesShown;
            badgesTableView.ReloadData();
        }

        private void SetUpContentView()
        {
            float bottom = 640f;

            if (!String.IsNullOrEmpty(CurrentBlah.T))
            {

                var blahTitleAttributes = new UIStringAttributes
                {
                    Font = UIFont.FromName(BGAppearanceConstants.BoldFontName, 19),
                    ForegroundColor = UIColor.Black,
                };

				txtBlahTitle.AttributedText = new NSAttributedString(CurrentBlah.T, blahTitleAttributes);
				SizeF size = txtBlahTitle.SizeThatFits (new SizeF (320, 5000));
				txtBlahTitleHeight.Constant = size.Height;
            }
            else
            {
				txtBlahTitle.Hidden = true;
            }


            if (CurrentBlah.ImageURL != null)
            {
				Uri imageToLoad = new Uri (CurrentBlah.ImageURL);
				UIImage theImage = ImageLoader.DefaultRequestImage (imageToLoad, this);
				blahImage.Image = theImage;
                
				if (blahImage.Image != null) {
					UIImage img = blahImage.Image;
					float newHeight = img.Size.Height / img.Size.Width * 320;

					blahImageHeight.Constant = newHeight;
				}

            }
            else
            {
				blahImageHeight.Constant = 0;
            }

            if (!String.IsNullOrEmpty(CurrentBlah.F))
            {
                var blahBodyAttributes = new UIStringAttributes
                {
                    Font = UIFont.FromName(BGAppearanceConstants.FontName, 12.0f),
                    ForegroundColor = UIColor.Black,
                };

				blahBodyView.AttributedText = new NSAttributedString(CurrentBlah.F, blahBodyAttributes);

				SizeF size = blahBodyView.SizeThatFits (new SizeF (320, 5000));
				txtBlahBodyHeight.Constant = size.Height;
			}
			else
			{
				blahBodyView.Text = "";
			}
            bottom = blahBodyView.Frame.Bottom;


            if (CurrentBlah.TypeName == "polls" || CurrentBlah.TypeName == "predicts")
            {
                if (tableView == null)
                {
                    tableView = new UITableView();

                    tableView.ScrollEnabled = false;
                    tableView.AllowsMultipleSelection = false;
                    if (BlahguaAPIObject.Current.CurrentUser == null)
                    {
                        tableView.AllowsSelection = false;
                    }



                    var width = NSLayoutConstraint.Create(tableView, NSLayoutAttribute.Width, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 1, 320);
                    var height = NSLayoutConstraint.Create(
                                 tableView,
                                 NSLayoutAttribute.Height,
                                 NSLayoutRelation.Equal,
                                 null,
                                 NSLayoutAttribute.NoAttribute,
                                 1,
                                (BlahExtraItemCount + 1 ) * 64.0f);
                    var left = NSLayoutConstraint.Create(tableView, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, contentView, NSLayoutAttribute.Left, 1, 0);

                    tableView.AddConstraints(new NSLayoutConstraint[] { width, height});
                    var positionYTop = NSLayoutConstraint.Create(tableView, NSLayoutAttribute.Top, NSLayoutRelation.Equal, blahBodyView, NSLayoutAttribute.Bottom, 1, 8);                  
                    contentView.AddConstraints(new NSLayoutConstraint[] {left, positionYTop });

                    contentView.AddSubview(tableView);

                    if (CurrentBlah.TypeName == "polls")
                        BlahguaAPIObject.Current.GetUserPollVote(PollVoteFetched);
                    else
                    {
                        BlahguaAPIObject.Current.GetUserPredictionVote(PredictionVoteFetched);
                    }

                }

                bottom += (BlahExtraItemCount + 1) * 64.0f;
            }
            contentView.ContentSize = new SizeF (blahBodyView.Frame.Width, bottom);
            contentView.ScrollEnabled = true;
        }

        private int BlahExtraItemCount
        {
            get
            {
                switch (BlahguaAPIObject.Current.CurrentBlah.TypeName)
                {
                    case "polls":
                        if (BlahguaAPIObject.Current.CurrentBlah.I != null)
                            return BlahguaAPIObject.Current.CurrentBlah.I.Count;
                        else
                            return 0;
                        break;

                    case "predicts":
                        return 4;
                        break;

                    default:
                        return 0;
                }

            }
        }
        private void PollVoteFetched(UserPollVote theVote)
        {
            BlahguaAPIObject.Current.CurrentBlah.UpdateUserPollVote(theVote);
            FinalizeVote();
        }

        private void PredictionVoteFetched(UserPredictionVote theVote)
        {
            BlahguaAPIObject.Current.CurrentBlah.UpdateUserPredictionVote(theVote);
            FinalizeVote();
        }

        private void FinalizeVote()
        {
            InvokeOnMainThread(() =>
                {
                    if (CurrentBlah.TypeName == "polls")
                    {
                        tableView.Source = new BGBlahPollTableSource(BlahPollType.Poll);
                        tableView.Delegate = new BGBlahPollTableDelegate(BlahPollType.Poll, this);
                    }
                    else
                    {
                        tableView.Source = new BGBlahPollTableSource(BlahPollType.Predict);
                        tableView.Delegate = new BGBlahPollTableDelegate(BlahPollType.Predict, this);
                    }

                    tableView.ReloadData();
                    tableView.TranslatesAutoresizingMaskIntoConstraints = false;

                    float finalHeight = blahBodyView.Frame.Bottom;
                    finalHeight += (BlahExtraItemCount + 1) * 64f;
                    contentView.ContentSize = new SizeF(contentView.ContentSize.Width, finalHeight);
                }
            );

        }


		public void AdjustViewLayout()
		{
			//badge table view
			if (CurrentBlah.B != null && CurrentBlah.B.Any())
			{
				//badgeImage.Hidden = false;
				var count = 0;
				if(CurrentBlah.B != null)
					count = CurrentBlah.B.Count ;
				badgeTableHeight.Constant = count * 28;

				badgesTableView.ReloadData();
			}
			else
			{
				//badgeImage.Hidden = true;

				badgeTableHeight.Constant = 0;
				//badgesTableView.Frame = new RectangleF (badgesTableView.Frame.X, badgesTableView.Frame.Y, badgesTableView.Frame.Width,0);
			}

			SetUpContentView ();
		}
		public void SetUpToolbar()
        {
            bottomToolbar.TranslatesAutoresizingMaskIntoConstraints = false;
            bottomToolbar.TintColor = UIColor.Clear;

			if (BlahguaAPIObject.Current.CurrentUser != null) {
				UIBarButtonItem[] items = new UIBarButtonItem[6];// (7);

				items [0] = upVote;
				items [1] = downVote;

				items [2] = new UIBarButtonItem (UIBarButtonSystemItem.FlexibleSpace);

				items [3] = summaryView;
				items [4] = commentsView;
				items [5] = statsView;

				bottomToolbar.SetItems (items, true);

				SetUpVotesButtons();
			} else {
				UIBarButtonItem[] items = new UIBarButtonItem[5];// (7);

				items [0] = signInBtn ;

				items [1] = new UIBarButtonItem (UIBarButtonSystemItem.FlexibleSpace);

				items [2] = summaryView;
				items [3] = commentsView;
				items [4] = statsView;

				bottomToolbar.SetItems (items, true);

				bottomToolbar.Frame = new RectangleF (View.Bounds.X, View.Bounds.Height - 44, View.Bounds.Width, 44);

				var btnSignInRect = new RectangleF (0, 0, 11, 60);
				var btnSignIn = new UIButton (UIButtonType.Custom);
				btnSignIn.Frame = btnSignInRect;
				btnSignIn.SetTitle("Sign In", UIControlState.Normal) ;
                BGAppearanceHelper.SetButtonFont(btnSignIn, "Merriweather");
				btnSignIn.TouchUpInside += (object sender, EventArgs e) => {
					this.PerformSegue("SummaryToLogin", this);
				};

				signInBtn.CustomView = btnSignIn;
			}
            
            SetUpModesButtons();
        }

        private void SetUpVotesButtons()
        {
            var votesButtonRect = new RectangleF(0, 0, 11, 19);
            upVoteButton = new UIButton(UIButtonType.Custom);
            upVoteButton.Frame = votesButtonRect;
            upVoteButton.TouchUpInside += (object sender, EventArgs e) =>
                {
                    //by synsoft on 14 July 2014 for voting first go to login page if not login   
                    if (BlahguaAPIObject.Current.CurrentUser != null)
                    {
                        if (CurrentBlah.uv == 0)
                        {
                            upVoteButton.SetImage(UIImage.FromBundle("arrow_up_dark").ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal), UIControlState.Normal);
                            BlahguaAPIObject.Current.SetBlahVote(1, (value) =>
                                {
                                    AppDelegate.analytics.PostBlahVote(1);
                                    CurrentBlah.uv = value;
                                    InvokeOnMainThread(() => SetVoteButtonsImages());
                                    
                                });
                        }
                    }
                    else
                    {
                        AlertDelegate obj = new AlertDelegate();
                        UIAlertView _alert = new UIAlertView("Alert", "Please Login First", obj, "OK", null);
                        _alert.Clicked += _alert_Clicked;
                        _alert.Show();
                    }

                };



            downVoteButton = new UIButton(UIButtonType.Custom);
            downVoteButton.Frame = votesButtonRect;
            downVoteButton.TouchUpInside += (object sender, EventArgs e) =>
                {
                    if (BlahguaAPIObject.Current.CurrentUser != null)
                    {
                        if (CurrentBlah.uv == 0)
                        {
    						downVoteButton.SetImage(UIImage.FromBundle("arrow_down_dark").ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal), UIControlState.Normal);
                            BlahguaAPIObject.Current.SetBlahVote(-1, (value) =>
                            {
                                AppDelegate.analytics.PostBlahVote(-1);
                                CurrentBlah.uv = value;
                                InvokeOnMainThread(() => SetVoteButtonsImages());
                            });
                        }
                    }
                    else
                    {
                        AlertDelegate obj = new AlertDelegate();
                        UIAlertView _alert = new UIAlertView("Alert", "Please Login First", obj, "OK", null);
                        _alert.Clicked += _alert_Clicked;
                        _alert.Show();
                    }
                };

            SetVoteButtonsImages();

            upVote.CustomView = upVoteButton;
            downVote.CustomView = downVoteButton;
        }

        //Synsoft on 14 July 2014 
        void _alert_Clicked(object sender, UIButtonEventArgs e)
        {
            this.PerformSegue("SummaryToLogin", this);
        }

        private void SetVoteButtonsImages()
        {
            switch (CurrentBlah.uv)
            {
                case 1:
                    {
                        SetVoteButtonsImages(UIImage.FromBundle("arrow_up_dark"), UIImage.FromBundle("arrow_down"));
                        break;
                    }
                case -1:
                    {
                        SetVoteButtonsImages(UIImage.FromBundle("arrow_up"), UIImage.FromBundle("arrow_down_dark"));
                        break;
                    }
                default:
                    {
                        SetVoteButtonsImages(UIImage.FromBundle("arrow_up"), UIImage.FromBundle("arrow_down"));
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

        private void SetUpModesButtons()
        {
            summaryButton = new UIButton(UIButtonType.Custom);
            summaryButton.Frame = new RectangleF(0, 0, 20, 16);
            summaryButton.SetImage(UIImage.FromBundle("summary_dark"), UIControlState.Normal);
            summaryButton.TouchUpInside += (object sender, EventArgs e) =>
            {
                SetModeButtonsImages(UIImage.FromBundle("summary_dark"), UIImage.FromBundle("comments"), UIImage.FromBundle("stats"));
            };
            summaryView.CustomView = summaryButton;

            commentsButton = new UIButton(UIButtonType.Custom);
            commentsButton.Frame = new RectangleF(0, 0, 22, 19);
            commentsButton.SetImage(UIImage.FromBundle("comments"), UIControlState.Normal);
            commentsButton.TouchUpInside += (object sender, EventArgs e) =>
            {

				((AppDelegate)UIApplication.SharedApplication.Delegate).swipeView.SwipeToLeft ();
            };
            commentsView.CustomView = commentsButton;

            statsButton = new UIButton(UIButtonType.Custom);
            statsButton.Frame = new RectangleF(0, 0, 26, 17);
            statsButton.SetImage(UIImage.FromBundle("stats"), UIControlState.Normal);
            statsButton.TouchUpInside += (object sender, EventArgs e) =>
            {
                try
                {
					((AppDelegate)UIApplication.SharedApplication.Delegate).swipeView.SwipeFromSummaryToStats ();
                }
                catch (Exception ex)
                {
                    ex.Message.ToString();
                }

            };
            statsView.CustomView = statsButton;
        }

        private void SetModeButtonsImages(UIImage sumImage, UIImage commentsImage, UIImage statsImage)
        {
            commentsButton.SetImage(commentsImage.ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal), UIControlState.Normal);
            summaryButton.SetImage(sumImage.ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal), UIControlState.Normal);
            statsButton.SetImage(statsImage.ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal), UIControlState.Normal);
        }

        private void SetAuthorName()
        {
            author.AttributedText = new NSAttributedString(CurrentBlah.UserName,
                UIFont.FromName(BGAppearanceConstants.BoldFontName, 14),
                UIColor.Black);
        }

        private void SetAuthorDescription()
        {
            userDescription.AttributedText = new NSAttributedString(CurrentBlah.DescriptionString, UIFont.FromName(BGAppearanceConstants.MediumItalicFontName, 10),
                UIColor.Black);
        }

        public void PollVoted(UserPollVote item)
        {
            InvokeOnMainThread(() => tableView.ReloadData());
        }

        public void PredictionVoted(UserPredictionVote item)
        {
            InvokeOnMainThread(() => tableView.ReloadData());
        }

        #endregion

        #region IImageUpdated implementation

        public void UpdatedImage(Uri uri)
        {
            blahImage.Image = ImageLoader.DefaultRequestImage(uri, this);
			if (blahImage.Image != null) {
				UIImage img = blahImage.Image;
				float newHeight = img.Size.Height / img.Size.Width * 320;

				blahImageHeight.Constant = newHeight;
                float finalHeight = blahBodyView.Frame.Bottom;
                if (tableView != null)
                    finalHeight += (BlahExtraItemCount + 1) * 64f;
                finalHeight += newHeight;
                contentView.ContentSize = new SizeF (contentView.ContentSize.Width, finalHeight);
			}

        }

        #endregion


        private enum BlahPollType
        {
            Poll,
            Predict
        }

        private class BGBlahPollTableSource : UITableViewSource
        {
            private BlahPollType type;
 
            private bool IsUserVoted
            {
                get
                {
                    PollItem pi;
                    if (type == BlahPollType.Poll)
                    {
                        pi = BlahguaAPIObject.Current.CurrentBlah.I.FirstOrDefault(i => i.IsUserVote == true);
                    }
                    else
                    {

                        if (BlahguaAPIObject.Current.CurrentBlah.IsPredictionExpired)
                            pi = BlahguaAPIObject.Current.CurrentBlah.ExpPredictionItems.FirstOrDefault(i => i.IsUserVote == true);
                        else
                            pi = BlahguaAPIObject.Current.CurrentBlah.PredictionItems.FirstOrDefault(i => i.IsUserVote == true);
                    }
                    return pi != null;
                }
            }

            public BGBlahPollTableSource(BlahPollType type)
            {
                this.type = type;
            }

            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                var cell = BGPollTableViewCell.Create();
                cell.Frame = BGAppearanceConstants.PollCellRect;
                PollItem pollItem;
                if (type == BlahPollType.Poll)
                {
                    pollItem = BlahguaAPIObject.Current.CurrentBlah.I.ElementAt(indexPath.Row);
                }
                else
                {
                    if (!BlahguaAPIObject.Current.CurrentBlah.IsPredictInited)
                        BlahguaAPIObject.Current.CurrentBlah.UpdateUserPredictionVote(null);
                    if (BlahguaAPIObject.Current.CurrentBlah.IsPredictionExpired)
                    {
                        pollItem = BlahguaAPIObject.Current.CurrentBlah.ExpPredictionItems[indexPath.Row];
                    }
                    else
                    {
                        pollItem = BlahguaAPIObject.Current.CurrentBlah.PredictionItems[indexPath.Row];
                    }
                }
                cell.SetUp(pollItem, IsUserVoted);
                return cell;
            }

            public override int NumberOfSections(UITableView tableView)
            {
                return 1;
            }

            public override int RowsInSection(UITableView tableview, int section)
            {
                return type == BlahPollType.Poll ? BlahguaAPIObject.Current.CurrentBlah.I.Count : 3;
            }

            public override float GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
            {
                return 64f;
            }
            public override float GetHeightForHeader(UITableView tableView, int section)
            {
                return type == BlahPollType.Poll ? 0 : 64f;
            }

            public override string TitleForHeader(UITableView tableView, int section)
            {
                if (type == BlahPollType.Predict)
                {
                    if (BlahguaAPIObject.Current.CurrentBlah.IsPredictionExpired)
                        return "expired " + Utilities.ElapsedDateString(BlahguaAPIObject.Current.CurrentBlah.ExpireDate);
                    else
                        return "expires " + Utilities.ElapsedDateString(BlahguaAPIObject.Current.CurrentBlah.ExpireDate);
                }
                else
                    return null;
            }

            public override UIView GetViewForHeader(UITableView tableView, int section)
            {
                var header = type == BlahPollType.Poll ?
                    new UIView() :
                    BGPollTableHeaderView.Create((BlahguaAPIObject.Current.CurrentBlah.IsPredictionExpired ?
                        "Predection expired at " :
                        "Predection will expire at ") + BlahguaAPIObject.Current.CurrentBlah.ExpireDate.ToString());
                header.Frame = new RectangleF(0, 0, 320, 24);
                return header;
            }
        }

        private class BGBlahPollTableDelegate : UITableViewDelegate
        {
            private BlahPollType type;
            private BGBlahViewController vc;

            public BGBlahPollTableDelegate(BlahPollType type, BGBlahViewController vc)
                : base()
            {
                this.type = type;
                this.vc = vc;
            }

            public override float EstimatedHeight(UITableView tableView, NSIndexPath indexPath)
            {
                return 64f;
            }

            public override float GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
            {
                return 64f;
            }

            public override float GetHeightForHeader(UITableView tableView, int section)
            {
                return type == BlahPollType.Poll ? 0 : 64f;
            }

            public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
            {
                if (type == BlahPollType.Poll)
                {
                    var pollItem = BlahguaAPIObject.Current.CurrentBlah.I[indexPath.Row];
                    BlahguaAPIObject.Current.SetPollVote(pollItem, vc.PollVoted);
                    BlahguaAPIObject.Current.CurrentBlah.UpdateUserPollVote(new UserPollVote() { W = indexPath.Row });
                }
                else
                {
                    PollItem pollItem;
                    UserPredictionVote upv;
                    if (BlahguaAPIObject.Current.CurrentBlah.IsPredictionExpired)
                    {
                        pollItem = BlahguaAPIObject.Current.CurrentBlah.ExpPredictionItems[indexPath.Row];
                        switch (indexPath.Row)
                        {
                            case 0:
                                {
                                    upv = new UserPredictionVote { Z = "y" };
                                    break;
                                }
                            case 1:
                                {
                                    upv = new UserPredictionVote { Z = "n" };
                                    break;
                                }
                            case 3:
                            default:
                                {
                                    upv = new UserPredictionVote { Z = "u" };
                                    break;
                                }
                        }
                    }
                    else
                    {
                        pollItem = BlahguaAPIObject.Current.CurrentBlah.PredictionItems[indexPath.Row];
                        switch (indexPath.Row)
                        {
                            case 0:
                                {
                                    upv = new UserPredictionVote { D = "y" };
                                    break;
                                }
                            case 1:
                                {
                                    upv = new UserPredictionVote { D = "n" };
                                    break;
                                }
                            case 3:
                            default:
                                {
                                    upv = new UserPredictionVote { D = "u" };
                                    break;
                                }
                        }
                    }
                    BlahguaAPIObject.Current.SetPredictionVote(pollItem, vc.PredictionVoted);
                    BlahguaAPIObject.Current.CurrentBlah.UpdateUserPredictionVote(upv);
                }
            }
        }

        private class BGBlahBadgesTableSource : UITableViewSource
        {
            #region implemented abstract members of UITableViewSource

            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                var cell = (BGBlahBadgeCell)tableView.DequeueReusableCell("cell");
                cell.SetUp(
                    BlahguaAPIObject.Current.CurrentBlah.Badges[indexPath.Row].BadgeName,
                    BlahguaAPIObject.Current.CurrentBlah.Badges[indexPath.Row].BadgeImage
                );
                return cell;
            }

            public override int RowsInSection(UITableView tableview, int section)
            {
                return BlahguaAPIObject.Current.CurrentBlah.B == null || !BlahguaAPIObject.Current.CurrentBlah.B.Any() ?
                    0 : BlahguaAPIObject.Current.CurrentBlah.B.Count;
            }

            public override int NumberOfSections(UITableView tableView)
            {
                return 1;
            }

            #endregion


        }
    }

    public class ImageUpdateDelegate : IImageUpdated
    {
        private UIImageView image;

        public ImageUpdateDelegate(UIImageView image)
        {
            this.image = image;
			//if(((AppDelegate)UIApplication.SharedApplication.Delegate).swipeView != null)
			//	((AppDelegate)UIApplication.SharedApplication.Delegate).swipeView.SummaryViewController.AdjustViewLayout ();
        }

        #region IImageUpdated implementation

        public void UpdatedImage(Uri uri)
        {
            image.Image = ImageLoader.DefaultRequestImage(uri, this);

		}

        #endregion
    }
		
    public class AlertDelegate : UIAlertViewDelegate
    {
        public override void Clicked(UIAlertView alertview, int buttonIndex)
        {
            base.Clicked(alertview, buttonIndex);

        }
    }
}
