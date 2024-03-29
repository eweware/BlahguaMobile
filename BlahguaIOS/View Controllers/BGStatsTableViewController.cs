// This file has been autogenerated from a class added in the UI designer.

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections;
using System.Collections.Generic;

using BlahguaMobile.BlahguaCore;

using Foundation;
using UIKit;
using CoreGraphics;

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
            Console.WriteLine("view did load");
            try
            {
               
                base.ViewDidLoad();

				this.NavigationController.SetNavigationBarHidden(false, true);
                this.Title = "Statistics";
                this.NavigationController.NavigationBar.TitleTextAttributes = new UIStringAttributes () {
                    Font = UIFont.FromName ("Merriweather", 20),
                    ForegroundColor = BGAppearanceConstants.TealGreen
                };
                //Synsoft on 14 July 2014 for swipping between screens                
                UISwipeGestureRecognizer objUISwipeGestureRecognizer = new UISwipeGestureRecognizer(SwipeToCommentsController);
                objUISwipeGestureRecognizer.Direction = UISwipeGestureRecognizerDirection.Right;
                //this.View.AddGestureRecognizer(objUISwipeGestureRecognizer);


                scrollView.ScrollEnabled = true;

				//Synsoft on 10 June 2014 

				if (CurrentBlah != null)
				{
					AppDelegate.analytics.PostPageView("/blah/stats");
					lblConversionRatio.Text = CurrentBlah.ConversionString;
					lblOpen.Text = CurrentBlah.O.ToString();
					lblDemotes.Text = CurrentBlah.D.ToString();
					lblPromotes.Text = CurrentBlah.P.ToString();
					lblComment.Text = CurrentBlah.C.ToString();
					lblImpression.Text = CurrentBlah.V.ToString();
					lblHeardRatio.Text = CurrentBlah.StrengthString;
					lblOpenedImpression.Text = CurrentBlah.O + "/" + CurrentBlah.V;
					BlahguaAPIObject.Current.LoadBlahStats((statList) =>
					{
						if (statList != null)
							UpdateBlahStats(statList);
					});
				}

                // change everything to a certain
                ChangeLabelFonts(this.View, BGAppearanceConstants.MediumFontName);
            }
            catch (Exception e)
            {
                e.Message.ToString();

            }
			SetUpBaseLayout ();
			SetUpToolbar ();

        }

		private void UpdateBlahStats(StatsList theStats)
		{
			int totalViews = 0, totalOpens = 0, totalComments = 0;

			foreach (StatDayRecord curDay in theStats)
			{
				totalViews += curDay.views;
				totalComments += curDay.comments;
				totalOpens += curDay.opens;
			}
			InvokeOnMainThread(() =>
			{
				lblOpen.Text = totalOpens.ToString();
				lblComment.Text = totalComments.ToString();
				lblImpression.Text = totalViews.ToString();
			});
		}

        private void ChangeLabelFonts(UIView anyView, string fontName)
        {
            if (anyView is UILabel)
            {
                UILabel curLabel = anyView as UILabel;
                NSAttributedString oldString = curLabel.AttributedText;

                curLabel.AttributedText = new NSAttributedString(curLabel.Text, 
                    UIFont.FromName(fontName, curLabel.Font.PointSize), 
                    curLabel.TextColor);
            }

            foreach (UIView subView in anyView.Subviews)
            {
                ChangeLabelFonts(subView, fontName);
            }

        }

		public override void ViewDidAppear(bool animated)
		{
            this.NavigationController.NavigationBar.TitleTextAttributes  = new UIStringAttributes () {
                Font = UIFont.FromName ("Merriweather", 20),
                ForegroundColor = BGAppearanceConstants.TealGreen
            };
			base.ViewDidAppear (animated);

			if(CurrentBlah != null)
				SetModeButtonsImages(UIImage.FromBundle("summary"), UIImage.FromBundle("comments"), UIImage.FromBundle("stats_dark"));
                
            InvokeOnMainThread(  () =>
                {
                    scrollView.ScrollEnabled = true;
                    scrollView.ContentSize = this.View.Frame.Size;// new CGSize(320f, 540f);
                });

		}


		private void SetUpBaseLayout()
		{
			View.BackgroundColor = UIColor.White;

			if (CurrentBlah != null) {
				bottomToolBar.BackgroundColor = BGAppearanceConstants.TealGreen;
				bottomToolBar.BarTintColor = BGAppearanceConstants.TealGreen;
			}
		}

  
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
					var btnSignInRect = new CGRect (0, 0, 80, 60);
					var btnSignIn = new UIButton (UIButtonType.Custom);
					btnSignIn.Frame = btnSignInRect;
					btnSignIn.SetTitle ("Sign In", UIControlState.Normal);
                    BGAppearanceHelper.SetButtonFont(btnSignIn, "Merriweather");
					btnSignIn.TouchUpInside += (object sender, EventArgs e) => {
						this.PerformSegue ("fromStatsToLogin", this);
					};

					signInBtn.CustomView = btnSignIn;
				} else {
					var btnSignInRect = new CGRect (0, 0, 0, 60);
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
			summaryButton.Frame = new CGRect (0, 0, 20, 16);
			summaryButton.SetImage (UIImage.FromBundle ("summary"), UIControlState.Normal);
			summaryButton.TouchUpInside += (object sender, EventArgs e) => {
				//SetModeButtonsImages(UIImage.FromBundle ("summary_dark"), UIImage.FromBundle ("comments"), UIImage.FromBundle ("stats"));

				((AppDelegate)UIApplication.SharedApplication.Delegate).swipeView.SwipeFromStatsToSummary ();
			};
			summaryView.CustomView = summaryButton;

			commentsButton = new UIButton (UIButtonType.Custom);
			commentsButton.Frame = new CGRect (0, 0, 22, 19);
			commentsButton.SetImage (UIImage.FromBundle ("comments"), UIControlState.Normal);
			commentsButton.TouchUpInside += (object sender, EventArgs e) => {

				//SetModeButtonsImages(UIImage.FromBundle ("summary"), UIImage.FromBundle ("comments_dark"), UIImage.FromBundle ("stats"));

				((AppDelegate)UIApplication.SharedApplication.Delegate).swipeView.SwipeToRight ();
			};
			commentsView.CustomView = commentsButton;

			statsButton = new UIButton (UIButtonType.Custom);
			statsButton.Frame = new CGRect (0, 0, 26, 17);
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
