// This file has been autogenerated from a class added in the UI designer.

using System;
using CoreGraphics;
using System.ComponentModel;

using BlahguaMobile.BlahguaCore;

using MonoTouch.SlideMenu;
using Foundation;
using UIKit;
using MonoTouch.Dialog.Utilities;
using System.Threading;

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

		public bool NaturalScrollInProgress = false;
		public bool IsAutoScrollingEnabled = false;

		private bool isNewPostMode;
		private Timer toastTimer;
		private UIAlertView toast;
        private bool firstTime = true;
		private bool isRefreshing = false;


		#endregion



		public BGRollViewController (IntPtr handle) : base (handle)
		{
			TimerCallback tcb = HideToastDialog;
			toastTimer = new Timer (tcb);
			toast = new UIAlertView ("Heard", "test", null, null, null);
		}

		private void StartToastTimer()
		{
			toastTimer.Change (3000, -1);
		}


		private void HideToastDialog(object stateObj)
		{
			InvokeOnMainThread(() => {
					toast.DismissWithClickedButtonIndex(0, true);
				});
		}

		#region View Controller Overriden Methods

        public override void ViewWillDisappear(bool animated)
        {
			ResetChannelBar ();
            base.ViewWillDisappear(animated);
            BlahguaAPIObject.Current.FlushImpressionList();
        }

		public void ResetChannelBar()
		{
			this.NavigationController.NavigationBar.SetBackgroundImage(null, UIBarMetrics.Default);
		}

		private void PrepareChannelBar ()
		{
			string headerImage = BlahguaAPIObject.Current.CurrentChannel.HeaderImage;
			if (String.IsNullOrEmpty(headerImage)) {
				Title = BlahguaAPIObject.Current.CurrentChannel.ChannelName;
				this.NavigationController.NavigationBar.SetBackgroundImage(null, UIBarMetrics.Default);
			} else {
				Title = "";
                UIImage theImage = UIImageHelper.ImageFromUrl(headerImage);
				if (theImage != null) {
					CGRect theRect = this.NavigationController.NavigationBar.Frame;
					theRect.Width *= UIScreen.MainScreen.Scale;
					theRect.Height *= UIScreen.MainScreen.Scale;
					nfloat aspectRatio = theImage.Size.Width / theImage.Size.Height;
					nfloat newWidth = theRect.Width;
					nfloat newHeight = newWidth / aspectRatio;
					nfloat offset = (newHeight - theRect.Height) / 2;
					if (newHeight < theRect.Height) {
						newHeight = theRect.Height;
						newWidth = newHeight * aspectRatio;
						offset = 0;
					}

					UIGraphics.BeginImageContext (theRect.Size);
					theImage.Draw (new CGRect (0, -offset, newWidth, newHeight));
					UIImage newImage = UIGraphics.GetImageFromCurrentImageContext (); 
					UIGraphics.EndImageContext();
					this.NavigationController.NavigationBar.SetBackgroundImage (newImage, UIBarMetrics.Default);
					theImage.Dispose ();
				}
			}
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			PrepareChannelBar ();

			if (BGAppearanceHelper.DeviceType == DeviceType.iPhone4) {

				this.View.BackgroundColor = UIColor.FromPatternImage (UIImage.FromBundle ("texture"));

			} else if (BGAppearanceHelper.DeviceType == DeviceType.iPhone5) {

				this.View.BackgroundColor = UIColor.FromPatternImage (UIImage.FromBundle ("texture-568h"));
			}

			else {

				this.View.BackgroundColor = UIColor.FromPatternImage (UIImage.FromBundle ("texture"));

			}

			CollectionView.BackgroundColor = UIColor.Clear;

			leftSlidingMenu = ((AppDelegate)UIApplication.SharedApplication.Delegate).SlideMenu;
			this.NavigationController.NavigationBar.TitleTextAttributes = new UIStringAttributes () {
				Font = UIFont.FromName ("Merriweather", 20),
				ForegroundColor = UIColor.FromRGB (96, 191, 164)
			};

			NavigationItem.LeftBarButtonItem = new UIBarButtonItem (UIImage.FromBundle ("hamburger_teal"), UIBarButtonItemStyle.Plain, MenuButtonClicked);
			NavigationItem.LeftBarButtonItem.SetBackgroundImage (UIImage.FromBundle ("leftMenuButton"), UIControlState.Highlighted, UIBarMetrics.Default);
			//Synsoft on 9 July 2014 for active color #1FBBD1
			NavigationItem.LeftBarButtonItem.TintColor = UIColor.FromRGB (96, 191, 164);

			BlahguaAPIObject.Current.PropertyChanged += (object sender, PropertyChangedEventArgs e) => {
				if (e.PropertyName == "CurrentChannel") {
                    ImageLoader.Purge();
					
					InvokeOnMainThread (() => {
						PrepareChannelBar();
						var dataSource = ((BGRollViewDataSource)CollectionView.DataSource);
						dataSource.DataSource.Clear ();
						CollectionView.ReloadData ();
                        CollectionView.SetContentOffset(new CGPoint(0,0), false);
					});
					BlahguaAPIObject.Current.GetInbox (InboxLoadingCompleted);
					BlahguaAPIObject.Current.GetCurrentChannelPermission((thePerm) => {
						if (BlahguaAPIObject.Current.CurrentUser != null) {

							InvokeOnMainThread(() => {
								if (newBlah == null)
									PrepareRightBarButton();
								if (thePerm.post == false)
									newBlah.Hidden = true;
								else
									newBlah.Hidden = false;
							});
						}
					});
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
			if(!IsNewPostMode)
				SetSrollingAvailability (true);
			((AppDelegate)UIApplication.SharedApplication.Delegate).RightMenu.UpdateForUser();
			PrepareChannelBar ();
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
            if (CollectionView.NumberOfItemsInSection (0) > 0)
            {
				if (!IsNewPostMode)
					NaturalScrollInProgress = false;
				else
					NaturalScrollInProgress = true;
    		}
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
                    {
    				    CollectionView.ContentOffset = new CGPoint (0, CollectionView.ContentOffset.Y + 1);
                    }
				}, AutoScroll);
		}


		public void RefreshData ()
		{
			if (!isRefreshing) {
				isRefreshing = true;
				BlahguaAPIObject.Current.GetInbox (InboxLoadingCompleted);
			}
		}

		public void DeleteFirst100Items ()
		{
			((BGRollViewDataSource)CollectionView.DataSource).DeleteFirst100Items ();
		}

		private void SetSrollingAvailability (bool enabled)
		{
			NaturalScrollInProgress = !enabled;
		}

		private void MenuButtonClicked (object sender, EventArgs args)
		{
			if (IsNewPostMode)
				return;
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

            if (firstTime)
            {
                BlahguaAPIObject.Current.GetWhatsNew((whatsNew) =>
                    {
                        string whatsNewString = "";

                        whatsNewString = whatsNew.SummaryString;
						if (String.IsNullOrEmpty(whatsNewString))
						{
							whatsNewString = "Check out what is new in the stream today!";
						}
                        InvokeOnMainThread (() => {
                            ShowToast(whatsNewString);
                        });
                    });

            }



		}

		private void InboxLoadingCompleted (Inbox inbox)
		{
            AppDelegate.analytics.PostPageView("/channel/" + BlahguaAPIObject.Current.CurrentChannel.ChannelName);
			InvokeOnMainThread (() => {
                if (inbox != null) {
				    ((BGRollViewDataSource)CollectionView.DataSource).InsertItems (inbox);
                    NaturalScrollInProgress = false;
                    BlahguaAPIObject.Current.GetAdForUser((theAd) => 
                        {
                            if (theAd != null)
                            {
                                InvokeOnMainThread(() => 
                                    {
                                        ((BGRollViewDataSource)CollectionView.DataSource).InsertAd(theAd);
                                        NaturalScrollInProgress = false;
										isRefreshing = false;
                                    });
                            }
							else
								isRefreshing = false;
                        });
                }
                else {        
					isRefreshing = false;
                    ((BGRollViewDataSource)CollectionView.DataSource).DataSource.Clear();
                    UIAlertView msg = new UIAlertView("Empty Channel", "This channel currently has no posts.", null, "Got it");
                    msg.Show();
                }
			});
		}

		public void UpdateProfileImage()
		{
			if(profile != null)
				profile.SetImage (GetProfileImage (), UIControlState.Normal);
		}

		public void ClearRightBarButton()
		{
			if (BlahguaCore.BlahguaAPIObject.Current.CurrentUser == null) {

				NavigationItem.SetRightBarButtonItems (new UIBarButtonItem[]{ new UIBarButtonItem ("Log in", UIBarButtonItemStyle.Plain, LoginButtonClicked) }, true);

				NavigationItem.RightBarButtonItem.SetTitleTextAttributes  (new UITextAttributes () {
					Font = UIFont.FromName ("Merriweather", 20),
					TextColor = UIColor.FromRGB (96, 191, 164)
				}, UIControlState.Normal);
			}
		}
		private void PrepareRightBarButton ()
		{
			if (BlahguaCore.BlahguaAPIObject.Current.CurrentUser == null) {
				if (NavigationItem.RightBarButtonItems == null) {
					NavigationItem.RightBarButtonItem = new UIBarButtonItem ("Log in", UIBarButtonItemStyle.Plain, LoginButtonClicked);
					NavigationItem.RightBarButtonItem.SetTitleTextAttributes  (new UITextAttributes () {
						Font = UIFont.FromName ("Merriweather", 16),
						TextColor = UIColor.FromRGB (96, 191, 164)
					}, UIControlState.Normal);
				}
			} else {
				if ((NavigationItem.RightBarButtonItems == null) || (NavigationItem.RightBarButtonItems.Length < 2))
				{
					profile = new UIButton(new CGRect(44, 0, 44, 44));
					profile.SetImage(GetProfileImage(), UIControlState.Normal);
					newBlah = new UIButton(new CGRect(0, 0, 44, 44));
					//newBlah.SetBackgroundImage (UIImage.FromBundle ("new_post_tap"), UIControlState.Normal);
					newBlah.SetImage(UIImage.FromBundle("icon_createpost"), UIControlState.Normal);
					profile.TouchUpInside += (object sender, EventArgs e) => ToggleRightMenu();
					newBlah.TouchUpInside += NewBlah;

					var negativeSpacer = new UIBarButtonItem(UIBarButtonSystemItem.FixedSpace);
					negativeSpacer.Width = -15f;

					UIView view = new UIView(new CGRect(0, 0, 88, 44));
					view.AddSubviews(new UIView[] { profile, newBlah });
					var rightBarButton = new UIBarButtonItem(view);

					NavigationItem.SetRightBarButtonItems(new UIBarButtonItem[] { negativeSpacer, rightBarButton }, true);
					BlahguaAPIObject.Current.GetCurrentChannelPermission((thePerm) => {
						if (BlahguaAPIObject.Current.CurrentUser != null) {
							InvokeOnMainThread(() => {
								if (thePerm.post == false)
									newBlah.Hidden = true;
								else
									newBlah.Hidden = false;
							});
						}
					});



				}

			}
		}

		public bool IsNewPostMode
		{
			get
			{
				return isNewPostMode;
			}
		}



		public void AddNewBlahToView(Blah newBlah)
		{
			// determine the last visible item
			BGRollViewDataSource dataSource = (BGRollViewDataSource)CollectionView.DataSource;
			UICollectionViewCell[]	theCells = CollectionView.VisibleCells;
			int lastBlahLoc = -1, curLoc;

			foreach (BGRollViewCell curCell in theCells) {
				curLoc = dataSource.IndexOf (curCell.Blah);
				if (curLoc > lastBlahLoc)
					lastBlahLoc = curLoc;
			}

			if (lastBlahLoc < dataSource.GetItemsCount (CollectionView, 0) - 1)
				lastBlahLoc++;
				
			// replace the next blah in the roll with the new one
			Console.WriteLine ("inserting new blah!");
			dataSource.ReplaceItem (newBlah, lastBlahLoc);

			// notify the user
			ShowToast("New Post created - now look for it in the stream!");
            NaturalScrollInProgress = false;

		}

		public void ShowToast(string toastMsg)
		{
			toast.Message = toastMsg;
			
			toast.Show ();
			StartToastTimer ();
		}

		private void NewBlah (object sender, EventArgs e)
		{
            if (leftSlidingMenu.IsRightMenuOpen())
                return;

			if (isNewPostMode) {
				SetSrollingAvailability (true);
				leftSlidingMenu.HideNewBlahDialog ();
				isNewPostMode = false;
			} else {
				SetSrollingAvailability (false);
				leftSlidingMenu.ShowNewPostView ();
				isNewPostMode = true;
			}
		}

       
			
		private void ToggleRightMenu ()
		{
			if(IsNewPostMode)
				return;
			leftSlidingMenu.ToggleRightMenuAnimated ();
		}

	
		private void UpdateConstraintsIfNeeded (bool animated, Action completionHandler)
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
			UIImage theImage = ImageLoader.DefaultRequestImage (new Uri (BlahguaCore.BlahguaAPIObject.Current.CurrentUser.UserImage), this);
			if (theImage != null)
				UpdatedImage(null);

			return theImage;
		}



		#endregion

		#region IImageUpdated implementation

		public void UpdatedImage (Uri uri)
		{
			var image = ImageLoader.DefaultRequestImage (new Uri (BlahguaCore.BlahguaAPIObject.Current.CurrentUser.UserImage), this);
			if (image != null)
			{
				if (profile != null)
					profile.SetImage(image, UIControlState.Normal);
			}
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
