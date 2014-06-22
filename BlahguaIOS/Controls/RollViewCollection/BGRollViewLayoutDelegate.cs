using System;
using System.Linq;
using System.Linq.Expressions;
using System.Drawing;

using BlahguaMobile.BlahguaCore;

using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace BlahguaMobile.IOS
{
	public class BGRollViewLayoutDelegate : UICollectionViewDelegateFlowLayout
	{
		#region Fields

		private BGRollViewCellsSizeManager manager;
		private BGRollViewController viewController;

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

		public override SizeF GetSizeForItem (UICollectionView collectionView, UICollectionViewLayout layout, NSIndexPath indexPath)
		{
			return manager.GetCellSizeF (manager.GetCellSize(indexPath));
		}

		public override UIEdgeInsets GetInsetForSection (UICollectionView collectionView, UICollectionViewLayout layout, int section)
		{
			return new UIEdgeInsets (0, 0, 0, 0);
		}

		public override float GetMinimumInteritemSpacingForSection (UICollectionView collectionView, UICollectionViewLayout layout, int section)
		{
			return 0.0f;
		}

		public override float GetMinimumLineSpacingForSection (UICollectionView collectionView, UICollectionViewLayout layout, int section)
		{
			return 0.0f;
		}

		public override void ItemSelected (UICollectionView collectionView, NSIndexPath indexPath)
		{
			var inboxBlah = ((BGRollViewDataSource)viewController.CollectionView.DataSource).DataSource.ElementAt (indexPath.Item);

			BlahguaAPIObject.Current.SetCurrentBlahFromId (inboxBlah.I, (blah) => {
				InvokeOnMainThread(() => {
					((AppDelegate)UIApplication.SharedApplication.Delegate).CurrentBlah = BlahguaAPIObject.Current.CurrentBlah;
					viewController.PerformSegue("fromRollToBlah", viewController);
				});
			});
		}

		#endregion

		#region Methods

		#endregion
	}
}

