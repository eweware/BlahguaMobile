// This file has been autogenerated from a class added in the UI designer.

using System;
using System.Drawing;
using System.ComponentModel;

using BlahguaMobile.BlahguaCore;

using MonoTouch.SlideMenu;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.Dialog.Utilities;

namespace BlahguaMobile.IOS
{
	public partial class BGRollViewController : UICollectionViewController, IImageUpdated
	{
		#region Fields

		private SlideMenuController leftSlidingPanel;
		private BGRollViewCellsSizeManager manager;
		private UIButton profile;
		private UIButton newBlah;

		public bool NaturalScrollInProgress = false;
		public bool IsAutoScrollingEnabled = false;

		private UIView rightViewContainer;
		private UIView rightView;
		private UIImageView profileImage;
		private UILabel usernameLabel;
		private UIButton profileButton;
		private UIButton badgesButton;
		private UIButton demographicsButton;
		private UIButton historyButton;
		private UIButton statsButton;
		private UIButton logoutButton;
		private bool isOpened = false;

		private bool isNewPostMode;

		#endregion

		#region Properties

		private BGNewPostViewController newPostViewController;

		#endregion

		public BGRollViewController (IntPtr handle) : base (handle)
		{
		}

		#region View Controller Overriden Methods

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			Title = BlahguaAPIObject.Current.CurrentChannel.ChannelName;

            this.View.BackgroundColor = UIColor.FromPatternImage (UIImage.FromBundle ("texture_01"));
            CollectionView.BackgroundColor = UIColor.Clear;

			leftSlidingPanel = ((AppDelegate)UIApplication.SharedApplication.Delegate).SlideMenu;

            NavigationItem.LeftBarButtonItem = new UIBarButtonItem (UIImage.FromBundle("leftMenuButton"), UIBarButtonItemStyle.Plain, MenuButtonClicked);

			BlahguaAPIObject.Current.PropertyChanged += (object sender, PropertyChangedEventArgs e) => 
			{
				if(e.PropertyName == "CurrentChannel")
				{
					CollectionView.ScrollToItem(NSIndexPath.FromItemSection(0, 0), UICollectionViewScrollPosition.Top, true);
					InvokeOnMainThread(() => {
						Title = BlahguaAPIObject.Current.CurrentChannel.ChannelName;
						var dataSource = ((BGRollViewDataSource)CollectionView.DataSource);
						dataSource.DataSource.Clear();
						CollectionView.ReloadData();
					});
					BlahguaAPIObject.Current.GetInbox (InboxLoadingCompleted);
				}
			};

			if(BlahguaAPIObject.Current.CurrentUser != null)
				BlahguaAPIObject.Current.CurrentUser.PropertyChanged += (object sender, PropertyChangedEventArgs e) => 
				{
					if(usernameLabel.Text != ((User)sender).UserName)
					{
						SetUsername(BlahguaAPIObject.Current.CurrentUser.UserName);
					}
				};



			manager = new BGRollViewCellsSizeManager ();
			BlahguaAPIObject.Current.GetInbox (InitialInboxLoadingCompleted);
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			PrepareRightBarButton ();
			((AppDelegate)UIApplication.SharedApplication.Delegate).CurrentBlah = null;
			((AppDelegate)UIApplication.SharedApplication.Delegate).SlideMenu.SetGesturesState (true);
			SetSrollingAvailability (true);
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);

		}

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			SetSrollingAvailability (false);
			((AppDelegate)UIApplication.SharedApplication.Delegate).SlideMenu.SetGesturesState (false);
			base.PrepareForSegue (segue, sender);
		}

		#endregion

		#region Methods

		public void AutoScroll()
		{
			UIView.Animate(0.05, 0, 
				UIViewAnimationOptions.CurveLinear | 
				UIViewAnimationOptions.AllowUserInteraction | 
				UIViewAnimationOptions.AllowAnimatedContent,
				() => { 
					if(!NaturalScrollInProgress)
						CollectionView.ContentOffset = new PointF(0, CollectionView.ContentOffset.Y + 1);
				}, AutoScroll);
		}
			

		public void RefreshData()
		{
			BlahguaAPIObject.Current.GetInbox (InboxLoadingCompleted);
		}

		public void DeleteFirst200Items()
		{
			((BGRollViewDataSource)CollectionView.DataSource).DeleteFirst350Items ();
		}

		private void SetSrollingAvailability(bool enabled)
		{
			NaturalScrollInProgress = !enabled;
		}

		private void MenuButtonClicked(object sender, EventArgs args)
		{
			leftSlidingPanel.ToggleMenuAnimated ();
		}

		private void LoginButtonClicked(object sender, EventArgs args)
		{
			PerformSegue ("fromRollToLogin", this);
		}



		private void InitialInboxLoadingCompleted (Inbox theList)
		{
			InvokeOnMainThread (() => {
				CollectionView.DataSource = new BGRollViewDataSource (manager, this);
				CollectionView.CollectionViewLayout = new BGRollViewLayout (manager, this);
				CollectionView.Delegate = new BGRollViewLayoutDelegate (manager, this);
				InboxLoadingCompleted (theList);
			});
		}
			
		private void InboxLoadingCompleted(Inbox inbox)
		{
			InvokeOnMainThread (() => {
				((BGRollViewDataSource)CollectionView.DataSource).InsertItems(inbox);
			});
		}

		private void PrepareRightBarButton()
		{
			if(BlahguaCore.BlahguaAPIObject.Current.CurrentUser == null)
			{

        NavigationItem.RightBarButtonItem = new UIBarButtonItem ("Log in", UIBarButtonItemStyle.Plain, LoginButtonClicked);
        NavigationItem.RightBarButtonItem.TintColor = BGAppearanceConstants.TealGreen;
			}
			else
			{
				if (rightViewContainer == null)
					PrepareRigthMenu ();
				profile = new UIButton (new RectangleF (44, 0, 44, 44));
				profile.SetImage (GetProfileImage(), UIControlState.Normal);
				newBlah = new UIButton (new RectangleF (0, 0, 44, 44));
				newBlah.SetBackgroundImage (UIImage.FromBundle ("new_post_tap"), UIControlState.Normal);
                newBlah.SetImage (UIImage.FromBundle ("newBlahImage"), UIControlState.Normal);
				profile.TouchUpInside += (object sender, EventArgs e) => ToggleRightMenu();
				newBlah.TouchUpInside += NewBlah;

				var negativeSpacer = new UIBarButtonItem (UIBarButtonSystemItem.FixedSpace);
				negativeSpacer.Width = -15f;

				UIView view = new UIView (new RectangleF (0, 0, 88, 44));
				view.AddSubviews (new UIView[] { profile, newBlah });
				var rightBarButton = new UIBarButtonItem (view);

				NavigationItem.SetRightBarButtonItems (new UIBarButtonItem[] { negativeSpacer, rightBarButton }, true);
			}
		}

		private void NewBlah (object sender, EventArgs e)
		{
			UIView.BeginAnimations (null);
			UIView.SetAnimationDuration (0.5f);
			if(isNewPostMode)
			{
				CollectionView.Hidden = false;
				isNewPostMode = false;
				newPostViewController.View.RemoveFromSuperview ();
				SetSrollingAvailability (true);
				((AppDelegate)UIApplication.SharedApplication.Delegate).Menu.SwitchTableSource (BGLeftMenuType.Channels);
			}
			else
			{
				SetSrollingAvailability (false);
				if(newPostViewController == null)
				{
					newPostViewController = (BGNewPostViewController)((AppDelegate)UIApplication.SharedApplication.Delegate)
						.MainStoryboard
						.InstantiateViewController ("BGNewPostViewController");
					newPostViewController.ParentViewController = this;
				}
				((UIScrollView)newPostViewController.View).ContentInset = new UIEdgeInsets (0, 0, 14, 0);
				newPostViewController.View.Frame = new RectangleF (0, 0, 320, UIScreen.MainScreen.Bounds.Height);
				isNewPostMode = true;
				((AppDelegate)UIApplication.SharedApplication.Delegate).Menu.SwitchTableSource (BGLeftMenuType.BlahType);
				CollectionView.Hidden = true;
				View.AddSubview (newPostViewController.View);
			}
			UIView.CommitAnimations ();
		}

		private void PrepareRigthMenu()
		{
			rightViewContainer = new UIView (BGAppearanceConstants.InitialRightViewContainerFrame);
			rightViewContainer.BackgroundColor = UIColor.Clear;
			rightViewContainer.AddGestureRecognizer (new UITapGestureRecognizer (() => ToggleRightMenu ()));
			rightView = new UIView (BGAppearanceConstants.RightViewFrame);
            rightView.BackgroundColor = UIColor.FromPatternImage(UIImage.FromBundle("grayBack"));

			float yCoord = 0;

			float profileImageHeight = BGAppearanceConstants.RightViewFrame.Width, profileImageWidth = profileImageHeight;
			profileImage = new UIImageView(new RectangleF (0, yCoord, profileImageHeight, profileImageWidth));
			profileImage.Image = GetProfileImage ();
			yCoord += profileImageHeight;

			rightView.Add (profileImage);

			usernameLabel = new UILabel(new RectangleF (0, yCoord, BGAppearanceConstants.RightViewFrame.Width, 20));
			usernameLabel.TextAlignment = UITextAlignment.Center;
			usernameLabel.BackgroundColor = UIColor.White;
			yCoord += 21;
			SetUsername (BlahguaAPIObject.Current.CurrentUser.UserName);

			rightView.Add (usernameLabel);

			profileButton = new UIButton(new RectangleF (0, yCoord, BGAppearanceConstants.RightViewFrame.Width, 44.0f));
			profileButton.HorizontalAlignment = UIControlContentHorizontalAlignment.Left;
			profileButton.VerticalAlignment = UIControlContentVerticalAlignment.Center;
			profileButton.SetAttributedTitle (new NSAttributedString("Profile", UIFont.FromName(BGAppearanceConstants.BoldFontName, 17), UIColor.Black), 
				UIControlState.Normal);
			profileButton.SetAttributedTitle (new NSAttributedString("Profile", UIFont.FromName(BGAppearanceConstants.BoldFontName, 17), UIColor.White), 
				UIControlState.Selected);
            profileButton.SetBackgroundImage (UIImage.FromBundle ("greenBack"), UIControlState.Selected);
			profileButton.BackgroundColor = UIColor.White;
			profileButton.TouchUpInside += ProfileButtonClicked;
			profileButton.ContentEdgeInsets = new UIEdgeInsets (0, 22, 0, 0);

			rightView.Add (profileButton);

			yCoord += 45.0f;

			badgesButton = new UIButton(new RectangleF (0, yCoord, BGAppearanceConstants.RightViewFrame.Width, 32.0f));
			badgesButton.HorizontalAlignment = UIControlContentHorizontalAlignment.Left;
			badgesButton.VerticalAlignment = UIControlContentVerticalAlignment.Center;
			badgesButton.SetAttributedTitle (new NSAttributedString("Badges", UIFont.FromName(BGAppearanceConstants.FontName, 17), UIColor.Black), 
				UIControlState.Normal);
			badgesButton.SetAttributedTitle (new NSAttributedString("Badges", UIFont.FromName(BGAppearanceConstants.FontName, 17), UIColor.White), 
				UIControlState.Selected);
            badgesButton.SetBackgroundImage (UIImage.FromBundle ("greenBack"), UIControlState.Selected);
			badgesButton.BackgroundColor = UIColor.White;
			badgesButton.TouchUpInside += BadgesButtonClicked;
			badgesButton.ContentEdgeInsets = new UIEdgeInsets (0, 22, 0, 0);

			rightView.Add (badgesButton);

			yCoord += 33.0f;

			demographicsButton = new UIButton(new RectangleF (0, yCoord, BGAppearanceConstants.RightViewFrame.Width, 32.0f));
			demographicsButton.HorizontalAlignment = UIControlContentHorizontalAlignment.Left;
			demographicsButton.VerticalAlignment = UIControlContentVerticalAlignment.Center;
			demographicsButton.SetAttributedTitle (new NSAttributedString("Demographics", UIFont.FromName(BGAppearanceConstants.FontName, 17), UIColor.Black), 
				UIControlState.Normal);
			demographicsButton.SetAttributedTitle (new NSAttributedString("Demographics", UIFont.FromName(BGAppearanceConstants.FontName, 17), UIColor.White), 
				UIControlState.Selected);
            demographicsButton.SetBackgroundImage (UIImage.FromBundle ("greenBack"), UIControlState.Selected);
			demographicsButton.BackgroundColor = UIColor.White;
			demographicsButton.TouchUpInside += DemographicsButtonClicked;
			demographicsButton.ContentEdgeInsets = new UIEdgeInsets (0, 22, 0, 0);

			rightView.Add (demographicsButton);

			yCoord += 33.0f;

			historyButton = new UIButton(new RectangleF (0, yCoord, BGAppearanceConstants.RightViewFrame.Width, 44.0f));
			historyButton.HorizontalAlignment = UIControlContentHorizontalAlignment.Left;
			historyButton.VerticalAlignment = UIControlContentVerticalAlignment.Center;
			historyButton.SetAttributedTitle (new NSAttributedString("History", UIFont.FromName(BGAppearanceConstants.BoldFontName, 17), UIColor.Black), 
				UIControlState.Normal);
			historyButton.SetAttributedTitle (new NSAttributedString("History", UIFont.FromName(BGAppearanceConstants.BoldFontName, 17), UIColor.White), 
				UIControlState.Selected);
            historyButton.SetBackgroundImage (UIImage.FromBundle ("greenBack"), UIControlState.Selected);
			historyButton.BackgroundColor = UIColor.White;
			historyButton.TouchUpInside += HistoryButtonClicked;
			historyButton.ContentEdgeInsets = new UIEdgeInsets (0, 22, 0, 0);

			rightView.Add (historyButton);

			yCoord += 45.0f;

			statsButton = new UIButton(new RectangleF (0, yCoord, BGAppearanceConstants.RightViewFrame.Width, 44.0f));
			statsButton.HorizontalAlignment = UIControlContentHorizontalAlignment.Left;
			statsButton.VerticalAlignment = UIControlContentVerticalAlignment.Center;
			statsButton.SetAttributedTitle (new NSAttributedString("Stats", UIFont.FromName(BGAppearanceConstants.BoldFontName, 17), UIColor.Black), 
				UIControlState.Normal);
			statsButton.SetAttributedTitle (new NSAttributedString("Stats", UIFont.FromName(BGAppearanceConstants.BoldFontName, 17), UIColor.White), 
				UIControlState.Selected);
            statsButton.SetBackgroundImage (UIImage.FromBundle ("greenBack"), UIControlState.Selected);
			statsButton.BackgroundColor = UIColor.White;
			statsButton.TouchUpInside += StatsButtonClicked;
			statsButton.ContentEdgeInsets = new UIEdgeInsets (0, 22, 0, 0);

			rightView.Add (statsButton);

			yCoord += 45.0f;

			logoutButton = new UIButton(new RectangleF (0, yCoord, BGAppearanceConstants.RightViewFrame.Width, 44.0f));
			logoutButton.HorizontalAlignment = UIControlContentHorizontalAlignment.Left;
			logoutButton.VerticalAlignment = UIControlContentVerticalAlignment.Center;
			logoutButton.SetAttributedTitle (new NSAttributedString("LOG OUT", UIFont.FromName(BGAppearanceConstants.BoldFontName, 17), UIColor.Black), 
				UIControlState.Normal);
			logoutButton.SetAttributedTitle (new NSAttributedString("LOG OUT", UIFont.FromName(BGAppearanceConstants.BoldFontName, 17), UIColor.White), 
				UIControlState.Selected);
            logoutButton.SetBackgroundImage (UIImage.FromBundle ("greenBack"), UIControlState.Selected);
			logoutButton.BackgroundColor = UIColor.Clear;
			logoutButton.TouchUpInside += LogoutButtonClicked;
			logoutButton.ContentEdgeInsets = new UIEdgeInsets (0, 22, 0, 0);

			rightView.Add (logoutButton);

			rightViewContainer.Add (rightView);
			View.Add (rightViewContainer);
			View.BringSubviewToFront (rightViewContainer);
		}

		private void ProfileButtonClicked(object sender, EventArgs args)
		{
			PerformSegue ("fromRollToProfile", this);
		}

		private void BadgesButtonClicked(object sender, EventArgs args)
		{
			PerformSegue ("fromRollToBadges", this);
		}

		private void DemographicsButtonClicked (object sender, EventArgs e)
		{
			BlahguaAPIObject.Current.GetUserProfile ((profile) => {
				InvokeOnMainThread (() => PerformSegue ("fromRollToDemographics", this));
			});
		}

		private void HistoryButtonClicked(object sender, EventArgs args)
		{
			PerformSegue ("fromRollToHistory", this);
		}

		private void StatsButtonClicked(object sender, EventArgs args)
		{
			BlahguaAPIObject.Current.LoadUserStatsInfo ((userInfo) => {
				InvokeOnMainThread (() => {
					PerformSegue ("fromRollToStats", this);
				});
			});
		}

		private void LogoutButtonClicked(object sender, EventArgs args)
		{
			BlahguaAPIObject.Current.SignOut (null, SignoutCompleted);

		}

		private void SignoutCompleted (bool didIt)
		{
			if(didIt)
			{
				InvokeOnMainThread (() => {
					ToggleRightMenu ();
					PrepareRightBarButton ();
				});
			}
		}

		private void ToggleRightMenu()
		{
			UIView.BeginAnimations (null);
			UIView.SetAnimationDuration (0.3f);
			if(isOpened)
			{
				SetSrollingAvailability (true);
				rightViewContainer.Frame = BGAppearanceConstants.InitialRightViewContainerFrame;
				isOpened = false;
			}
			else
			{
				SetSrollingAvailability (false);
				rightViewContainer.Frame = BGAppearanceConstants.OpenedRightViewContainerFrame;
				View.BringSubviewToFront (rightViewContainer);
				isOpened = true;
			}
			UIView.CommitAnimations ();
		}

		private UIImage GetProfileImage()
		{
			return ImageLoader.DefaultRequestImage (new Uri (BlahguaCore.BlahguaAPIObject.Current.CurrentUser.UserImage), this);
		}

		private void SetUsername(string text)
		{
			usernameLabel.AttributedText = new NSAttributedString (text, 
				UIFont.FromName(BGAppearanceConstants.BoldFontName, 15), UIColor.Black);
		}

		#endregion


		#region IImageUpdated implementation

		public void UpdatedImage (Uri uri)
		{
			var image = ImageLoader.DefaultRequestImage (new Uri (BlahguaCore.BlahguaAPIObject.Current.CurrentUser.UserImage), this);
			profile.SetImage (image, UIControlState.Normal);
			profileImage.Image = image;
		}

		#endregion

	}
}
