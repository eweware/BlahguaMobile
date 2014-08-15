// This file has been autogenerated from a class added in the UI designer.

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections;
using System.Collections.Generic;

using BlahguaMobile.BlahguaCore;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Drawing;

namespace BlahguaMobile.IOS
{
    public partial class BGStatsTableViewController : UIViewController
    {
        private UIViewController parentViewController;

		private UIButton upVoteButton;
		private UIButton downVoteButton;

		private UIButton summaryButton;
		private UIButton commentsButton;
		private UIButton statsButton;

        private Blah CurrentBlah
        {
            get
            {
                return ((AppDelegate)UIApplication.SharedApplication.Delegate).CurrentBlah;
            }
        }

        public BGStatsTableViewController(IntPtr handle)
            : base(handle)
        {

        }

        public override void ViewDidLoad()
        {

            try
            {
                AppDelegate.analytics.PostPageView("/blah/stats");
                base.ViewDidLoad();

                //Synsoft on 9 July 2014 added title
                this.Title = "Stats";

				this.NavigationController.SetNavigationBarHidden(false, true);

                //Synsoft on 14 July 2014 for swipping between screens                
                UISwipeGestureRecognizer objUISwipeGestureRecognizer = new UISwipeGestureRecognizer(SwipeToCommentsController);
                objUISwipeGestureRecognizer.Direction = UISwipeGestureRecognizerDirection.Right;
                this.View.AddGestureRecognizer(objUISwipeGestureRecognizer);


                scrollView.ContentSize = new SizeF(320, 640);
             
               

                scrollView.ScrollEnabled = true;

                //Synsoft on 10 June 2014 
                if (CurrentBlah != null)
                {
                    lblConversionRatio.Text = CurrentBlah.ConversionString.ToString();
                    lblOpen.Text = CurrentBlah.O.ToString();
                    lblDemotes.Text = CurrentBlah.D.ToString();
                    lblPromotes.Text = CurrentBlah.P.ToString();
                    lblComment.Text = CurrentBlah.C.ToString();
                    lblImpression.Text = CurrentBlah.V.ToString();
                    lblHeardRatio.Text = CurrentBlah.S.ToString("0.00") + "%";
                    lblOpenedImpression.Text = CurrentBlah.O.ToString() + "/" + CurrentBlah.V.ToString();
                }
                else if (BlahguaAPIObject.Current.CurrentUser != null && BlahguaAPIObject.Current.CurrentUser.UserInfo != null)
                {
                    var userinfo = BlahguaAPIObject.Current.CurrentUser.UserInfo;
                    int userviews, opens, creates, comments, views;
                    userviews = opens = creates = comments = views = 0;

                    for (int i = 0; i < userinfo.DayCount; i++)
                    {
                        userviews += userinfo.UserViews(i);
                        opens += userinfo.Opens(i);
                        creates += userinfo.UserCreates(i);
                        comments += userinfo.UserComments(i);
                        views += userinfo.Views(i);
                    }

                    lblOpen.Text = opens.ToString();
                    lblComment.Text = comments.ToString();
                    lblImpression.Text = views.ToString();
                }
            }
            catch (Exception e)
            {
                e.Message.ToString();

            }
			SetUpBaseLayout ();
			SetUpToolbar ();

        }
		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear (animated);
			scrollView.TranslatesAutoresizingMaskIntoConstraints = false;
			if(CurrentBlah != null)
				SetModeButtonsImages(UIImage.FromBundle("summary"), UIImage.FromBundle("comments"), UIImage.FromBundle("stats_dark"));

		}
		private void SetUpBaseLayout()
		{
			View.BackgroundColor = UIColor.FromPatternImage(UIImage.FromBundle("grayBack"));

			if (CurrentBlah != null) {
				//bottomToolBar.TranslatesAutoresizingMaskIntoConstraints = true;

				bottomToolBar.BackgroundColor = BGAppearanceConstants.TealGreen;
				bottomToolBar.BarTintColor = BGAppearanceConstants.TealGreen;
			}
		}

        //Synsoft on 14 July 2014
        private void SwipeToCommentsController()
        {
			((AppDelegate)UIApplication.SharedApplication.Delegate).swipeView.SwipeToRight ();
        }

        public void SetParentViewController(UIViewController parentViewController)
        {
            this.parentViewController = parentViewController;
        }

		public void SetUpToolbar()
		{
			if (CurrentBlah != null) {
				bottomToolBar.TintColor = UIColor.Clear;
				//SetUpVotesButtons ();

				if (BlahguaAPIObject.Current.CurrentUser == null) {
					var btnSignInRect = new RectangleF (0, 0, 80, 60);
					var btnSignIn = new UIButton (UIButtonType.Custom);
					btnSignIn.Frame = btnSignInRect;
					btnSignIn.SetTitle ("Sign In", UIControlState.Normal);
					btnSignIn.TouchUpInside += (object sender, EventArgs e) => {
						this.PerformSegue ("fromStatsToLogin", this);
					};

					signInBtn.CustomView = btnSignIn;
				} else {
					var btnSignInRect = new RectangleF (0, 0, 0, 60);
					var btnSignIn = new UIButton (UIButtonType.Custom);
					btnSignIn.Frame = btnSignInRect;
					btnSignIn.SetTitle ("", UIControlState.Normal);
					btnSignIn.TouchUpInside += (object sender, EventArgs e) => {

					};

					signInBtn.CustomView = btnSignIn;
				}

				SetUpModesButtons ();
			} else {
				bottomToolBar.Hidden = true;
			}
		}

		void _alert_Clicked(object sender, UIButtonEventArgs e)
		{
			this.PerformSegue("fromStatsToLogin", this);
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


		private void SetUpModesButtons ()
		{
			summaryButton = new UIButton (UIButtonType.Custom);
			summaryButton.Frame = new RectangleF (0, 0, 20, 16);
			summaryButton.SetImage (UIImage.FromBundle ("summary"), UIControlState.Normal);
			summaryButton.TouchUpInside += (object sender, EventArgs e) => {
				//SetModeButtonsImages(UIImage.FromBundle ("summary_dark"), UIImage.FromBundle ("comments"), UIImage.FromBundle ("stats"));

				((AppDelegate)UIApplication.SharedApplication.Delegate).swipeView.SwipeFromStatsToSummary ();
			};
			summaryView.CustomView = summaryButton;

			commentsButton = new UIButton (UIButtonType.Custom);
			commentsButton.Frame = new RectangleF (0, 0, 22, 19);
			commentsButton.SetImage (UIImage.FromBundle ("comments"), UIControlState.Normal);
			commentsButton.TouchUpInside += (object sender, EventArgs e) => {

				//SetModeButtonsImages(UIImage.FromBundle ("summary"), UIImage.FromBundle ("comments_dark"), UIImage.FromBundle ("stats"));

				((AppDelegate)UIApplication.SharedApplication.Delegate).swipeView.SwipeToRight ();
			};
			commentsView.CustomView = commentsButton;

			statsButton = new UIButton (UIButtonType.Custom);
			statsButton.Frame = new RectangleF (0, 0, 26, 17);
			statsButton.SetImage (UIImage.FromBundle ("stats_dark"), UIControlState.Normal);
			statsButton.TouchUpInside += (object sender, EventArgs e) => {
				SetModeButtonsImages(UIImage.FromBundle ("summary"), UIImage.FromBundle ("comments"), UIImage.FromBundle ("stats_dark"));
				//Commented by Synsoft on 9 July 2014
				// PerformSegue("fromCommentsToStats", this);

				//Synsoft on 9 July 2014 to add popup animation


			};
			statsView.CustomView = statsButton;
		}

		private void SetModeButtonsImages(UIImage sumImage, UIImage commentsImage, UIImage statsImage)
		{
			commentsButton.SetImage (commentsImage.ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal), UIControlState.Normal);
			summaryButton.SetImage (sumImage.ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal), UIControlState.Normal);
			statsButton.SetImage (statsImage.ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal), UIControlState.Normal);
		}

    }


}
