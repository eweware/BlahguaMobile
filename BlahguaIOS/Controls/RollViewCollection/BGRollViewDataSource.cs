using System;
using CoreGraphics;

using UIKit;
using Foundation;

using BlahguaMobile.BlahguaCore;

namespace BlahguaMobile.IOS
{

	public class BGRollViewDataSource : UICollectionViewDataSource
	{
		#region Fields

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

		#endregion

		public BGRollViewDataSource (BGRollViewCellsSizeManager manager, BGRollViewController viewController) : base()
		{
			this.manager = manager;
			this.viewController = viewController;
		}

		public int IndexOf(InboxBlah blahToFind)
		{
			return dataSource.IndexOf (blahToFind);
		}

		public void ReplaceItem(Blah newItem, int oldIndex)
		{
			if (newItem != null) {
				if ((oldIndex >= 0) && (oldIndex < dataSource.Count)) {
					InboxBlah newBlah = new InboxBlah (newItem);
					int oldSize = dataSource [oldIndex].displaySize;
					newBlah.displaySize = oldSize;
					dataSource [oldIndex] = newBlah;
				}
			}
		}

		#region Collection View Data Source overriden methods

		public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
		{
			string reusableId = manager.GetCellSize (indexPath);
			var cell = (BGRollViewCell)collectionView.DequeueReusableCell (new NSString(reusableId), indexPath);
            InboxBlah inboxBlah = dataSource [(int)indexPath.Item];
			var size = manager.GetCellSizeF (reusableId);

			cell.SetCellProperties (inboxBlah, reusableId, size, indexPath);


			if(indexPath.Item == dataSource.Count - 1 && dataSource.Count > 1000)
			{
				//DeleteFirst10Items ();
			}

			return cell;
		}

		public override nint NumberOfSections (UICollectionView collectionView)
		{
			return 1;
		}

		public override nint GetItemsCount (UICollectionView collectionView, nint section)
		{
			return dataSource.Count;
		}

		#endregion

		#region Methods

		public void InsertItems(Inbox inb)
		{
			viewController.CollectionView.PerformBatchUpdates (() => {
				inb.PrepareBlahs ();
				NSIndexPath[] paths = new NSIndexPath[100];
				int count = dataSource.Count;

				if(inb != null && inb.Count > 0)
				{
					for (int i = count, j = 0; i < count + 100; i++, j++) {
						paths [j] = NSIndexPath.FromItemSection (i, 0);
						string sizeName = manager.GetCellSize (paths [j]);
						dataSource.Add (inb.PopBlah (GetInboxBlahSize (sizeName)));
					}
					viewController.CollectionView.InsertItems (paths);
				}

			}, (bool finished) => {
				Console.WriteLine ("Inserted - now have total: " + dataSource.Count.ToString ());
				if(dataSource.Count > 500)
				{
					viewController.DeleteFirst100Items ();
				}
                if(true)//dataSource.Count <= 100)
				{
					if(!viewController.IsAutoScrollingEnabled)
					{
						viewController.IsAutoScrollingEnabled = true;
						viewController.AutoScroll();
					}
				}
			});
		}

        public void InsertAd(InboxBlah theAd)
        {
            int adLoc;
            dataSource.Add(theAd);
            adLoc = dataSource.Count - 1;
            NSIndexPath[] paths = new NSIndexPath[1];
            paths [0] = NSIndexPath.FromItemSection (adLoc, 0);
      
            manager.AddAd(adLoc);
            string sizeName = manager.GetCellSize (paths [0]);


            viewController.CollectionView.InsertItems(paths);

        }

		public void DeleteFirst100Items()
		{
			viewController.CollectionView.PerformBatchUpdates(()=> {
				dataSource.RemoveRange (0, 100);
				var indexPaths = new NSIndexPath[100];
				for(int i = 0; i < 100; i++)
				{
					indexPaths[i] = NSIndexPath.FromItemSection(i, 0);
				}
				viewController.CollectionView.DeleteItems(indexPaths);
			}, (bool finished) => {
				Console.WriteLine("Deleted 100 items");
				viewController.RefreshData();
			});
		}

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

