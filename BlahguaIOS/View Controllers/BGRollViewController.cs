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

		private SlideMenuController leftSlidingMenu;
		private BGRollViewCellsSizeManager manager;
		private UIButton profile;
		private UIButton newBlah;

		public UIPanGestureRecognizer RightMenuPanRecognizer;
		private PointF panStartPoint;
		private float startingLayoutRight = 0;
		private NSLayoutConstraint rightViewContainerXConstraint;

		public bool NaturalScrollInProgress = false;
		public bool IsAutoScrollingEnabled = false;

		private UIView rightViewContainer;
		private UIView rightView;
		private UIImageView profileImage;
		private UILabel usernameLabel;

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

			leftSlidingMenu = ((AppDelegate)UIApplication.SharedApplication.Delegate).SlideMenu;

			NavigationItem.LeftBarButtonItem = new UIBarButtonItem (UIImage.FromBundle ("leftMenuButton"), UIBarButtonItemStyle.Plain, MenuButtonClicked);
			//Synsoft on 9 July 2014 for active color #1FBBD1
			NavigationItem.LeftBarButtonItem.TintColor = UIColor.FromRGB (31, 187, 209);

			BlahguaAPIObject.Current.PropertyChanged += (object sender, PropertyChangedEventArgs e) => {
				if (e.PropertyName == "CurrentChannel") {
					CollectionView.ScrollToItem (NSIndexPath.FromItemSection (0, 0), UICollectionViewScrollPosition.Top, true);
					InvokeOnMainThread (() => {
						Title = BlahguaAPIObject.Current.CurrentChannel.ChannelName;
						var dataSource = ((BGRollViewDataSource)CollectionView.DataSource);
						dataSource.DataSource.Clear ();
						CollectionView.ReloadData ();
					});
					BlahguaAPIObject.Current.GetInbox (InboxLoadingCompleted);
				}
			};

			if (BlahguaAPIObject.Current.CurrentUser != null)
				BlahguaAPIObject.Current.CurrentUser.PropertyChanged += (object sender, PropertyChangedEventArgs e) => {
					if (usernameLabel.Text != ((User)sender).UserName) {
						SetUsername (BlahguaAPIObject.Current.CurrentUser.UserName);
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
			leftSlidingMenu.SetGesturesState (true);
			SetSrollingAvailability (true);
            //CollectionView.ScrollToItem(NSIndexPath.FromItemSection(0, 0), UICollectionViewScrollPosition.Top, true);

            BlahguaAPIObject.Current.PropertyChanged += (object sender, PropertyChangedEventArgs e) =>
            {
                if (e.PropertyName == "CurrentChannel")
                {
                    CollectionView.ScrollToItem(NSIndexPath.FromItemSection(0, 0), UICollectionViewScrollPosition.Top, true);
                    InvokeOnMainThread(() =>
                    {
                        Title = BlahguaAPIObject.Current.CurrentChannel.ChannelName;
                        var dataSource = ((BGRollViewDataSource)CollectionView.DataSource);
                        dataSource.DataSource.Clear();
                        CollectionView.ReloadData();
                    });
                    BlahguaAPIObject.Current.GetInbox(InboxLoadingCompleted);
                }
            };
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

		public void AutoScroll ()
		{
			UIView.Animate (0.05, 0, 
				UIViewAnimationOptions.CurveLinear |
				UIViewAnimationOptions.AllowUserInteraction |
				UIViewAnimationOptions.AllowAnimatedContent,
				() => { 
					if (!NaturalScrollInProgress)
						CollectionView.ContentOffset = new PointF (0, CollectionView.ContentOffset.Y + 1);
				}, AutoScroll);
		}


		public void RefreshData ()
		{
			BlahguaAPIObject.Current.GetInbox (InboxLoadingCompleted);
		}

		public void DeleteFirst200Items ()
		{
			((BGRollViewDataSource)CollectionView.DataSource).DeleteFirst350Items ();
		}

		private void SetSrollingAvailability (bool enabled)
		{
			NaturalScrollInProgress = !enabled;
		}

		private void MenuButtonClicked (object sender, EventArgs args)
		{
			leftSlidingMenu.ToggleMenuAnimated ();
		}

		private void LoginButtonClicked (object sender, EventArgs args)
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

		private void InboxLoadingCompleted (Inbox inbox)
		{
			InvokeOnMainThread (() => {
				((BGRollViewDataSource)CollectionView.DataSource).InsertItems (inbox);
			});
		}

		private void PrepareRightBarButton ()
		{
			if (BlahguaCore.BlahguaAPIObject.Current.CurrentUser == null) {
				if (NavigationItem.RightBarButtonItems == null) {
					NavigationItem.RightBarButtonItem = new UIBarButtonItem ("Log in", UIBarButtonItemStyle.Plain, LoginButtonClicked);
					//Synsoft On 9 July 2014
					NavigationItem.RightBarButtonItem.TintColor = UIColor.FromRGB (31, 187, 209);
					//commented by Synsoft on 9 July 2014
					//NavigationItem.RightBarButtonItem.TintColor = BGAppearanceConstants.TealGreen;
				}
			} else {
				if (NavigationItem.RightBarButtonItems.Length < 2) {

					profile = new UIButton (new RectangleF (44, 0, 44, 44));
					profile.SetImage (GetProfileImage (), UIControlState.Normal);
					newBlah = new UIButton (new RectangleF (0, 0, 44, 44));
					//newBlah.SetBackgroundImage (UIImage.FromBundle ("new_post_tap"), UIControlState.Normal);
					newBlah.SetImage (UIImage.FromBundle ("icon_createpost"), UIControlState.Normal);
					profile.TouchUpInside += (object sender, EventArgs e) => ToggleRightMenu ();
					newBlah.TouchUpInside += NewBlah;

					var negativeSpacer = new UIBarButtonItem (UIBarButtonSystemItem.FixedSpace);
					negativeSpacer.Width = -15f;

					UIView view = new UIView (new RectangleF (0, 0, 88, 44));
					view.AddSubviews (new UIView[] { profile, newBlah });
					var rightBarButton = new UIBarButtonItem (view);

					NavigationItem.SetRightBarButtonItems (new UIBarButtonItem[] { negativeSpacer, rightBarButton }, true);
				}
			}
		}

		private void NewBlah (object sender, EventArgs e)
		{
			UIView.BeginAnimations (null);
			UIView.SetAnimationDuration (0.5f);
			if (isNewPostMode) {
				//CollectionView.Hidden = false;
				isNewPostMode = false;
				newPostViewController.View.Frame =new RectangleF (0, - View.Bounds.Height, 320, UIScreen.MainScreen.Bounds.Height);
				UIView.CommitAnimations ();
				//newPostViewController.View.RemoveFromSuperview ();
				SetSrollingAvailability (true);

				((AppDelegate)UIApplication.SharedApplication.Delegate).Menu.SwitchTableSource (BGLeftMenuType.Channels);
			}  else {
				SetSrollingAvailability (false);
				if (newPostViewController == null) {
					newPostViewController = (BGNewPostViewController)((AppDelegate)UIApplication.SharedApplication.Delegate)
						.MainStoryboard
						.InstantiateViewController ("BGNewPostViewController");
					newPostViewController.View.BackgroundColor = new UIColor(50/255.0f, 50/255.0f, 50/255.0f, 0.7f);
					newPostViewController.ParentViewController = this;
					this.AddChildViewController (newPostViewController);
					newPostViewController.View.Frame =new RectangleF (0, - View.Bounds.Height, 320, UIScreen.MainScreen.Bounds.Height);
					View.AddSubview (newPostViewController.View);
				}
				((UIScrollView)newPostViewController.View).ContentInset = new UIEdgeInsets (0, 0, 14, 0);
				newPostViewController.View.Frame = new RectangleF (0, 0, 320, UIScreen.MainScreen.Bounds.Height);

				isNewPostMode = true;
				((AppDelegate)UIApplication.SharedApplication.Delegate).Menu.SwitchTableSource (BGLeftMenuType.Channels );
				//CollectionView.Hidden = true;

			}
			UIView.CommitAnimations ();
		}

		private void UpdateRightMenu ()
		{
			profileImage.Image = GetProfileImage ();
			SetUsername (BlahguaAPIObject.Current.CurrentUser.UserName);
		}


		private void ToggleRightMenu ()
		{
			/*
			if (isOpened) {
				SetSrollingAvailability (true);
				ResetToStartPosition (true);
				isOpened = false;
			} else {
				SetSrollingAvailability (false);
				SetFinalContainerViewPosition (true);
				isOpened = true;
			}
			*/
			leftSlidingMenu.ToggleRightMenuAnimated ();
		}

		public void PanRightView (UIPanGestureRecognizer recognizer)
		{
			switch (recognizer.State) {

			case UIGestureRecognizerState.Began:
				panStartPoint = recognizer.TranslationInView (rightViewContainer);
				break;
			case UIGestureRecognizerState.Changed:
				PointF currentPoint = recognizer.TranslationInView (rightViewContainer);
				float deltaX = currentPoint.X - panStartPoint.X;
				bool panningLeft = false; 
				if (currentPoint.X < panStartPoint.X) { 
					panningLeft = true;
				}

				if (startingLayoutRight == 0) { 
					if (!panningLeft) {
						float constant = Math.Max (-deltaX, 0);
						if (constant == 0) {
							ResetToStartPosition (true);
						} else { 
							rightViewContainerXConstraint.Constant = -constant;
						}
					} else {
						float constant = Math.Min (-deltaX, BGAppearanceConstants.RightViewFrame.Width);
						if (constant == BGAppearanceConstants.RightViewFrame.Width) {
							SetFinalContainerViewPosition (true);
						} else {
							rightViewContainerXConstraint.Constant = -constant;
						}
					}
				} else {
					float adjustment = startingLayoutRight - deltaX;
					if (!panningLeft) {
						float constant = Math.Max (adjustment, 0);
						if (constant == 0) {
							ResetToStartPosition (true);
						} else {
							rightViewContainerXConstraint.Constant = -constant;
						}
					} else {
						float constant = Math.Min (adjustment, BGAppearanceConstants.RightViewFrame.Width);
						if (constant == BGAppearanceConstants.RightViewFrame.Width) {
							SetFinalContainerViewPosition (true);
						} else {
							rightViewContainerXConstraint.Constant = -constant;
						}
					}
				}
				break;
			case UIGestureRecognizerState.Ended:
				if (startingLayoutRight == 0) {
					float position = BGAppearanceConstants.RightViewFrame.Width / 2 - 1;
					if (rightViewContainer.Frame.X < position) {
						SetFinalContainerViewPosition (true);
					} else {
						ResetToStartPosition (true);
					}
				} else {
					float position = BGAppearanceConstants.RightViewFrame.Width / 2;
					if (rightViewContainer.Frame.X < position) {
						SetFinalContainerViewPosition (true);
					} else {
						ResetToStartPosition (true);
					}
				}
				break;
			case UIGestureRecognizerState.Cancelled:
				if (startingLayoutRight == 0) {
					ResetToStartPosition (true);
				} else {
					SetFinalContainerViewPosition (true);
				}
				break;
			default:
				break;
			}
		}

		public void ResetToStartPosition (bool animated)
		{
			if (startingLayoutRight == 0 &&
			    rightViewContainer.Frame.X == 0) {
				return;
			}

			rightViewContainerXConstraint.Constant = 0;

			UpdateConstraintsIfNeeded (animated, () => {
				CollectionView.UserInteractionEnabled = true;
				leftSlidingMenu.SetGesturesState (true);
				startingLayoutRight = rightViewContainerXConstraint.Constant;
			});
		}

		public void SetFinalContainerViewPosition (bool animated)
		{
			if (startingLayoutRight == BGAppearanceConstants.RightViewFrame.Width &&
			    rightViewContainer.Frame.X == BGAppearanceConstants.RightViewFrame.Width) {
				return;
			}

			rightViewContainerXConstraint.Constant = -320;

			UpdateConstraintsIfNeeded (animated, () => {
				CollectionView.UserInteractionEnabled = false;
				View.BringSubviewToFront (rightViewContainer);
				leftSlidingMenu.SetGesturesState (false);
				startingLayoutRight = -rightViewContainerXConstraint.Constant;
			});
		}

		private void UpdateConstraintsIfNeeded (bool animated, NSAction completionHandler)
		{
			float duration = 0;
			if (animated) {
				duration = 0.3f;
			}

			UIView.Animate (duration, () => {
				View.LayoutIfNeeded ();
			}, completionHandler);
		}

		private UIImage GetProfileImage ()
		{
			return ImageLoader.DefaultRequestImage (new Uri (BlahguaCore.BlahguaAPIObject.Current.CurrentUser.UserImage), this);
		}

		private void SetUsername (string text)
		{
			usernameLabel.AttributedText = new NSAttributedString (text, 
				UIFont.FromName (BGAppearanceConstants.BoldFontName, 15), UIColor.Black);
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

	public class TapGestureRecognizerDelegate : UIGestureRecognizerDelegate
	{
		public override bool ShouldRecognizeSimultaneously (UIGestureRecognizer gestureRecognizer, UIGestureRecognizer otherGestureRecognizer)
		{
			return true;
		}

		public override bool ShouldReceiveTouch (UIGestureRecognizer recognizer, UITouch touch)
		{
			if (touch.View is UIButton) {
				return false;
			}
			return true;
		}
	}
}
