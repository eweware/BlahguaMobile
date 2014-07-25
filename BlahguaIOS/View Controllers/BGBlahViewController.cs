
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

        private ImageUpdateDelegate badgeImageUpdateDelegate;

        private float textInsetDefaultValue = 11.0f;
        private float defaultWidthOfContent = 320.0f;
        private float defaultContentViewStartYCoor = 97.0f;
        private float iphone4ContentViewHeight = 339f;
        private float iphone5ContentViewHeight = 427f;
        private SizeF toolbarViewSize = new SizeF(320f, 44f);

        private bool badgesShown = false;

        private UITableView tableView;

        private UITableView itemsTable;

        private UIButton upVoteButton;
        private UIButton downVoteButton;

        private UIButton summaryButton;
        private UIButton commentsButton;
        private UIButton statsButton;

		private float offset_Y = 0;

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
            base.ViewDidLoad();

            //Synsoft on 9 July 2014 added title
            this.Title = "Summary";

			offset_Y = 44;
            //Synsoft on 11 July 2014            
            //NavigationItem.LeftBarButtonItem = new UIBarButtonItem("Back", UIBarButtonItemStyle.Plain, BackHandler);
            //Synsoft on 11 July 2014 for active color  #1FBBD1
            //NavigationItem.LeftBarButtonItem.TintColor = UIColor.FromRGB(31, 187, 209);

            SetUpBaseLayout();

            SetUpHeaderView();

            SetUpContentView();

            SetUpToolbar();

            //Synsoft on 14 July 2014 for swipping between screens

            UISwipeGestureRecognizer objUISwipeGestureRecognizer = new UISwipeGestureRecognizer(SwipeToCommentController);
            objUISwipeGestureRecognizer.Direction = UISwipeGestureRecognizerDirection.Left;
            this.View.AddGestureRecognizer(objUISwipeGestureRecognizer);
        }

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear (animated);
			//summaryButton.SetImage (UIImage.FromBundle ("summary_dark"), UIControlState.Normal);
			SetModeButtonsImages(UIImage.FromBundle("summary_dark"), UIImage.FromBundle("comments"), UIImage.FromBundle("stats"));

			commentsButton.SetImage (UIImage.FromBundle ("comments"), UIControlState.Normal);

			statsButton.SetImage (UIImage.FromBundle ("stats"), UIControlState.Normal);

		}

        //Synsoft on 14 July 2014 
        private void SwipeToCommentController()
        {
           // PerformSegue("fromBlahViewToComments", this);
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
            //SetModeButtonsImages(UIImage.FromBundle("summary_dark"), UIImage.FromBundle("comments"), UIImage.FromBundle("stats"));
            if (ShouldMoveToStats)
            {
                ShouldMoveToStats = false;
                //PerformSegue("fromBlahViewToStats", this);
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

            bottomToolbar.BackgroundColor = UIColor.FromPatternImage(UIImage.FromBundle("greenBack"));
            bottomToolbar.BarTintColor = UIColor.FromPatternImage(UIImage.FromBundle("greenBack"));

            View.AddSubviews(new UIView[] { contentView, bottomToolbar });
        }

        private void SetUpHeaderView()
        {
            userImage.Image = ImageLoader.DefaultRequestImage(new Uri(CurrentBlah.UserImage),
                new ImageUpdateDelegate(userImage));

            SetAuthorName();
            SetAuthorDescription();

            //badgeImage.Frame = new RectangleF(new PointF(author.Frame.Right + 8, badgeImage.Frame.Top), badgeImage.Frame.Size);

            badgeImage.SetImage(UIImage.FromBundle("badges"), UIControlState.Normal);
            badgeImage.TouchUpInside += (sender, e) =>
            {
                AdjustBadgesTableView();
            };
            badgesTableView.Source = new BGBlahBadgesTableSource();
            if (CurrentBlah.B != null && CurrentBlah.B.Any())
            {
                badgeImage.Hidden = false;
				//badgesShown = true;
				var count = 0;
				if(CurrentBlah.B != null)
					count = CurrentBlah.B.Count ;
				///badgesTableViewHeight.Constant = count * 28;
				badgesTableView.Frame = new RectangleF (badgesTableView.Frame.X, badgesTableView.Frame.Y, badgesTableView.Frame.Width,count * 28);
				//badgesTableView.ContentSize = badgesTableView.Frame.Size;
				offset_Y += count * 28;
				RecalcContentViewFrame ();

				badgesTableView.ReloadData();
            }
            else
            {
                badgeImage.Hidden = true;

				//badgesTableViewHeight.Constant = 0;
				badgesTableView.Frame = new RectangleF (badgesTableView.Frame.X, badgesTableView.Frame.Y, badgesTableView.Frame.Width,0);
            }

            blahTimespan.AttributedText = new NSAttributedString(
                CurrentBlah.ElapsedTimeString ?? "",
                UIFont.FromName(BGAppearanceConstants.MediumFontName, 12),
                UIColor.Black
            );
        }
		void RecalcContentViewFrame()
		{
			contentView.Frame = new RectangleF (contentView.Frame.X, offset_Y, contentView.Frame.Width, this.View.Bounds.Height -  44 - offset_Y);
		}
		private void AdjustBadgesTableView()
        {
            if (badgesShown)
            {
                //badgesTableViewHeight.Constant = 0;
				badgesTableView.Frame = new RectangleF (badgesTableView.Frame.X, badgesTableView.Frame.Y, badgesTableView.Frame.Width, 0);

				offset_Y = 44;
				RecalcContentViewFrame ();
            }
            else
            {
                var count = badgesTableView.NumberOfRowsInSection(0);
				badgesTableView.Frame = new RectangleF (badgesTableView.Frame.X, badgesTableView.Frame.Y, badgesTableView.Frame.Width, count <= 1 ? 28 : 56); 

				offset_Y = 44 + (count <= 1 ? 28 : 56);
				RecalcContentViewFrame ();

            }
            badgesShown = !badgesShown;
            badgesTableView.ReloadData();
        }

        private void SetUpContentView()
        {
			float offset_Y = 0;
            if (!String.IsNullOrEmpty(CurrentBlah.T))
            {
                //blahTitle.Hidden = false;
                var blahTitleAttributes = new UIStringAttributes
                {
                    Font = UIFont.FromName(BGAppearanceConstants.BoldFontName, 19),
                    ForegroundColor = UIColor.Black,
                };

                //blahTitle.LineBreakMode = UILineBreakMode.WordWrap;
				//blahTitle.Lines = 0;


                //blahTitle.AttributedText = new NSAttributedString(CurrentBlah.T, blahTitleAttributes);

                //blahTitle.PreferredMaxLayoutWidth = defaultWidthOfContent - textInsetDefaultValue * 2;

				/*
				txtBlahTitle.Hidden = false;
				txtBlahTitle.AttributedText = new NSAttributedString(CurrentBlah.T, blahTitleAttributes);
				txtBlahTitle.TextAlignment = UITextAlignment.Left;
				txtBlahTitle.ScrollEnabled = false;
				txtBlahTitle.Editable = false;
				txtBlahTitle.ContentInset = new UIEdgeInsets(8, 8, 8, 8);
				var ctxt = new NSStringDrawingContext();
				var text = new NSMutableAttributedString(CurrentBlah.T);
				text.AddAttribute(UIStringAttributeKey.Font,  UIFont.FromName(BGAppearanceConstants.BoldFontName, 21.0f), new NSRange(0, text.Length));
				var boundingRect = text.GetBoundingRect(new SizeF(txtBlahTitle.Frame.Width, float.MaxValue), NSStringDrawingOptions.UsesFontLeading | NSStringDrawingOptions.UsesLineFragmentOrigin, ctxt);
				//Add some padding 

				txtBlahTitle.Frame = new RectangleF(blahBodyView.Frame.X, txtBlahTitle.Frame.Y ,blahBodyView.Frame.Width, boundingRect.Height + 10);
				offset_Y = txtBlahTitle.Frame.Y + txtBlahTitle.Frame.Height;
				*/

				txtBlahTitle.AttributedText = new NSAttributedString(CurrentBlah.T, blahTitleAttributes);

				txtBlahTitle.SizeToFit ();

				offset_Y = txtBlahTitle.Frame.Y + txtBlahTitle.Frame.Height - 8;
            }
            else
            {
                blahTitle.Hidden = true;
				offset_Y = 0;
            }


            if (CurrentBlah.ImageURL != null)
            {
				UIImage img = ImageLoader.DefaultRequestImage(
					new Uri(CurrentBlah.ImageURL),
					this
				);
                
				if (img != null) {
					float newHeight = img.Size.Height / img.Size.Width * 320;

					blahImage.Image = img;
					//imageHeightViewHeight.Constant = newHeight;
					blahImage.Frame = new RectangleF (blahImage.Frame.X, offset_Y , blahImage.Frame.Width, newHeight);

					offset_Y += newHeight;
				}
            }
            else
            {
                //imageHeightViewHeight.Constant = 0;
				blahImage.Frame = new RectangleF (blahImage.Frame.X, offset_Y, blahImage.Frame.Width, 0);
            }

            if (!String.IsNullOrEmpty(CurrentBlah.F))
            {
                var blahBodyAttributes = new UIStringAttributes
                {
                    Font = UIFont.FromName(BGAppearanceConstants.FontName, 12.0f),
                    ForegroundColor = UIColor.Black,
                };

                blahBodyView.Hidden = false;
                blahBodyView.AttributedText = new NSAttributedString(CurrentBlah.F, blahBodyAttributes);
                blahBodyView.TextAlignment = UITextAlignment.Left;
                blahBodyView.ContentInset = new UIEdgeInsets(4, 4, 4, 4);

				blahBodyView.SizeToFit ();

				blahBodyView.Frame = new RectangleF(blahBodyView.Frame.X, offset_Y - 8 ,blahBodyView.Frame.Width, blahBodyView.Frame.Height);
			}
			else
			{
				blahBodyView.Text = "";
				blahBodyView.Frame = new RectangleF(blahBodyView.Frame.X, offset_Y - 8 ,blahBodyView.Frame.Width,0);
			}
			contentView.ContentSize = new SizeF (blahBodyView.Frame.Width, blahBodyView.Frame.Bottom);

            if (CurrentBlah.TypeName == "polls" || CurrentBlah.TypeName == "predicts")
            {

                tableView = new UITableView();
                tableView.ScrollEnabled = false;
                tableView.AllowsMultipleSelection = false;
                if (BlahguaAPIObject.Current.CurrentUser == null)
                {
                    tableView.AllowsSelection = false;
                }
                tableView.BackgroundColor = UIColor.Clear;
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

				/*
                var width = NSLayoutConstraint.Create(tableView, NSLayoutAttribute.Width, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 1, 320);
                var height = NSLayoutConstraint.Create(
                    tableView,
                    NSLayoutAttribute.Height,
                    NSLayoutRelation.Equal,
                    null,
                    NSLayoutAttribute.NoAttribute,
                    1,
                    tableView.NumberOfRowsInSection(0) * 64.0f
                );
*/
                //tableView.AddConstraints(new NSLayoutConstraint[] { width, height });

                contentView.AddSubview(tableView);

               // var positionYTop = NSLayoutConstraint.Create(tableView, NSLayoutAttribute.Top, NSLayoutRelation.Equal, blahBodyView, NSLayoutAttribute.Bottom, 1, 8);
               // var positionYBottom = NSLayoutConstraint.Create(tableView, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, contentView, NSLayoutAttribute.Bottom, 1, 0);
               // var positionXLeft = NSLayoutConstraint.Create(tableView, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, contentView, NSLayoutAttribute.Leading, 1, 0);
               // var positionXRight = NSLayoutConstraint.Create(tableView, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal, contentView, NSLayoutAttribute.Trailing, 1, 0);

               // contentView.AddConstraints(new NSLayoutConstraint[] { positionYTop, positionYBottom, positionXLeft, positionXRight });
            }
            else
            {
                //var constraint = NSLayoutConstraint.Create(blahBodyView, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, contentView, NSLayoutAttribute.Bottom, 1, 0);
                //contentView.AddConstraint(constraint);
            }
        }
		public void AdjustViewLayout()
		{
			//badge table view
			if (CurrentBlah.B != null && CurrentBlah.B.Any())
			{
				badgeImage.Hidden = false;
				//badgesShown = true;
				var count = 0;
				if(CurrentBlah.B != null)
					count = CurrentBlah.B.Count ;
				///badgesTableViewHeight.Constant = count * 28;
				badgesTableView.Frame = new RectangleF (badgesTableView.Frame.X, badgesTableView.Frame.Y, badgesTableView.Frame.Width,count * 28);
				//badgesTableView.ContentSize = badgesTableView.Frame.Size;
				offset_Y =44 + count * 28;
				RecalcContentViewFrame ();

				badgesTableView.ReloadData();
			}
			else
			{
				badgeImage.Hidden = true;

				//badgesTableViewHeight.Constant = 0;
				badgesTableView.Frame = new RectangleF (badgesTableView.Frame.X, badgesTableView.Frame.Y, badgesTableView.Frame.Width,0);
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
                //======by synsoft on 14 July 2014
                //if (CurrentBlah.uv != 1)
                //{
                //    upVoteButton.SetImage(UIImage.FromBundle("arrow_up_dark").ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal), UIControlState.Normal);
                //    BlahguaAPIObject.Current.SetBlahVote(1, (value) =>
                //    {
                //        Console.WriteLine(value);
                //    });
                //}
                //=======================*/

                //by synsoft on 14 July 2014 for voting first go to login page if not login   
                if (BlahguaAPIObject.Current.CurrentUser != null)
                {
                    if (CurrentBlah.uv != 1)
                    {
                        upVoteButton.SetImage(UIImage.FromBundle("arrow_up_dark").ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal), UIControlState.Normal);
                        BlahguaAPIObject.Current.SetBlahVote(1, (value) =>
                        {
                            Console.WriteLine(value);
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

                // comment by synsoft on 14 July 2014 --old code

                //if (CurrentBlah.uv != -1)
                //{
                //    downVoteButton.SetImage(UIImage.FromBundle("arrow_down_dark").ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal), UIControlState.Normal);
                //    BlahguaAPIObject.Current.SetBlahVote(-1, (value) =>
                //    {
                //        Console.WriteLine(value);
                //    });
                //}

                // synsoft on 14 July 2014
                if (BlahguaAPIObject.Current.CurrentUser != null)
                {
                    if (CurrentBlah.uv != -1)
                    {
						downVoteButton.SetImage(UIImage.FromBundle("arrow_down_dark").ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal), UIControlState.Normal);
                        BlahguaAPIObject.Current.SetBlahVote(-1, (value) =>
                        {
                            Console.WriteLine(value);
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
                UIFont.FromName(BGAppearanceConstants.BoldFontName, 10),
                UIColor.Black);
        }

        private void SetAuthorDescription()
        {
            userDescription.AttributedText = new NSAttributedString(CurrentBlah.DescriptionString, UIFont.FromName(BGAppearanceConstants.BoldFontName, 10),
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
            private bool isUserVoted;

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

            public override UIView GetViewForHeader(UITableView tableView, int section)
            {
                var header = type == BlahPollType.Poll ?
                    new UIView() :
                    BGPollTableHeaderView.Create((BlahguaAPIObject.Current.CurrentBlah.IsPredictionExpired ?
                        "Predection was expired at " :
                        "Predection will expire at ") + BlahguaAPIObject.Current.CurrentBlah.ExpireDate.ToString());
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
