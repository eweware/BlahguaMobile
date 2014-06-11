using System;

using MonoTouch.UIKit;
using MonoTouch.Foundation;

using BlahguaMobile.BlahguaCore;

namespace BlahguaMobile.IOS
{

	public class BGRollViewDataSource : UICollectionViewDataSource
	{
		#region Fields

		private Inbox inbox = new Inbox();
		private Inbox dataSource = new Inbox();
		private BGRollViewCellsSizeManager manager;
		private BGRollViewController viewController;

		#endregion

		#region Properties

		public Inbox DataSource
		{
			get
			{
				return dataSource;
			}
		}

		public Inbox InboxBlahs
		{
			get
			{
				return inbox;
			}
		}

		#endregion

		public BGRollViewDataSource (BGRollViewCellsSizeManager manager, BGRollViewController viewController) : base()
		{
			this.manager = manager;
			this.viewController = viewController;
		}

		#region Collection View Data Source overriden methods

		public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
		{
			string reusableId = manager.GetCellSize (indexPath);
			var cell = (UICollectionViewCell)collectionView.DequeueReusableCell (new NSString(reusableId), indexPath);
			if(indexPath.Item >= dataSource.Count && inbox.Count > 0)
			{
				dataSource.Add (inbox.PopBlah (GetInboxBlahSize (reusableId)));
			}
			if(indexPath.Item == dataSource.Count + inbox.Count - 99)
			{
				if(dataSource.Count + inbox.Count > 1000)
				{
					viewController.CollectionView.PerformBatchUpdates(()=> {
						dataSource.RemoveRange (0, 100);
						var indexPathes = new NSIndexPath[100];
						for(int i = 0; i < 100; i++)
						{
							indexPathes[i] = NSIndexPath.FromItemSection(i, 0);
						}
						viewController.CollectionView.DeleteItems(indexPathes);
					}, (bool finished) => Console.WriteLine("Deleted: " + finished.ToString())); 
				}
				viewController.RefreshData ();
			}

			return cell;
		}

		public override int NumberOfSections (UICollectionView collectionView)
		{
			return 1;
		}

		public override int GetItemsCount (UICollectionView collectionView, int section)
		{
			//viewController.CollectionView.CollectionViewLayout.InvalidateLayout ();
			return dataSource.Count + inbox.Count;
		}

		#endregion

		#region Methods

		private int GetInboxBlahSize(string size)
		{
			if(size == BGBlahCellSizesConstants.TinyReusableId)
			{
				return 4;
			}
			else if(size == BGBlahCellSizesConstants.SmallReusableId)
			{
				return 3;
			}
			else if(size == BGBlahCellSizesConstants.MediumReusableId)
			{
				return 2;
			}
			else 
			{
				return 1;
			}
		}

		#endregion
	}
}

