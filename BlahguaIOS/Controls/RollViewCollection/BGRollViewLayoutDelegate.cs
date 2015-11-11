using System;
using System.Linq;
using System.Linq.Expressions;
using CoreGraphics;

using BlahguaMobile.BlahguaCore;

using UIKit;
using Foundation;
using MonoTouch.SlideMenu;

namespace BlahguaMobile.IOS
{
	public class BGRollViewLayoutDelegate : UICollectionViewDelegateFlowLayout
	{
		#region Fields

		private BGRollViewCellsSizeManager manager;
		private BGRollViewController viewController;
		private bool _doingItemSelect = false;

		#endregion

		#region Properties



		#endregion

		#region Constructors

		public BGRollViewLayoutDelegate (BGRollViewCellsSizeManager manager, BGRollViewController viewController) : base()
		{
			this.manager = manager;
			this.viewController = viewController;
		}

		#endregion

		#region UICollectionViewDelegateFlowLayout Overriden Methods

		public override CGSize GetSizeForItem (UICollectionView collectionView, UICollectionViewLayout layout, NSIndexPath indexPath)
		{
			return manager.GetCellSizeF (manager.GetCellSize(indexPath));
		}

		public override UIEdgeInsets GetInsetForSection (UICollectionView collectionView, UICollectionViewLayout layout, nint section)
		{
			return new UIEdgeInsets (0, 0, 0, 0);
		}

		public override nfloat GetMinimumInteritemSpacingForSection (UICollectionView collectionView, UICollectionViewLayout layout, nint section)
		{
			return 0.0f;
		}

		public override nfloat GetMinimumLineSpacingForSection (UICollectionView collectionView, UICollectionViewLayout layout, nint section)
		{
			return 0.0f;
		}

		public override void ItemSelected (UICollectionView collectionView, NSIndexPath indexPath)
		{
			if (!_doingItemSelect) {
				_doingItemSelect = true;
				viewController.NaturalScrollInProgress = true;
                var inboxBlah = ((BGRollViewDataSource)viewController.CollectionView.DataSource).DataSource.ElementAt ((int)indexPath.Item);

				BlahguaAPIObject.Current.SetCurrentBlahFromId (inboxBlah.I, (blah) => {
					InvokeOnMainThread (() => {
						((AppDelegate)UIApplication.SharedApplication.Delegate).CurrentBlah = BlahguaAPIObject.Current.CurrentBlah;

						var myStoryboard = ((AppDelegate)UIApplication.SharedApplication.Delegate).MainStoryboard;
						BGBlahViewController objBGBlahViewController = myStoryboard.InstantiateViewController ("BGBlahViewController") as BGBlahViewController;
						BGCommentsViewController commentView = myStoryboard.InstantiateViewController ("BGCommentsViewController") as BGCommentsViewController;
						BGStatsTableViewController statsView = myStoryboard.InstantiateViewController ("BGStatsTableViewController") as BGStatsTableViewController;


						SwipeViewController swipeView = new SwipeViewController (objBGBlahViewController, commentView, statsView);
						((AppDelegate)UIApplication.SharedApplication.Delegate).swipeView = swipeView;
						((AppDelegate)UIApplication.SharedApplication.Delegate).SlideMenu.NavigationController.PushViewController (swipeView, false);


						_doingItemSelect = false;
					});
				});
			}
		}

		public override void DecelerationEnded (UIScrollView scrollView)
		{
			viewController.NaturalScrollInProgress = false;
		}

		public override void ScrollAnimationEnded (UIScrollView scrollView)
		{
			viewController.NaturalScrollInProgress = false;
		}

		public override void DraggingStarted (UIScrollView scrollView)
		{
			viewController.NaturalScrollInProgress = true;
		}

		public override void DraggingEnded (UIScrollView scrollView, bool willDecelerate)
		{
			viewController.NaturalScrollInProgress = false;
		}

		public override void Scrolled (UIScrollView scrollView)
		{
			nfloat scrollViewHeight = scrollView.Frame.Height;// scrollView.frame.size.height;
			nfloat scrollContentSizeHeight = scrollView.ContentSize.Height;// scrollView.contentSize.height;
			nfloat scrollOffset = scrollView.ContentOffset.Y;// scrollView.contentOffset.y;

			if (scrollOffset + scrollViewHeight >= scrollContentSizeHeight)
			{

				viewController.RefreshData ();
			}	
		}
		#endregion

		#region Methods

		#endregion
	}
}

