using System;
using System.Drawing;

using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace BlahguaMobile.IOS
{
	public class BGRollViewLayoutDelegate : UICollectionViewDelegateFlowLayout
	{
		#region Fields

		private BGRollViewCellsSizeManager manager;

		#endregion

		#region Properties



		#endregion

		#region Constructors

		public BGRollViewLayoutDelegate (BGRollViewCellsSizeManager manager) : base()
		{
			this.manager = manager;
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
			Console.WriteLine (indexPath.Row);
		}

		#endregion

		#region Methods


		#endregion
	}
}

