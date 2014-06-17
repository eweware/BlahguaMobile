using System;
using System.Drawing;

using MonoTouch.UIKit;
using MonoTouch.Foundation;

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

		#region Collection View Data Source overriden methods

		public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
		{
			string reusableId = manager.GetCellSize (indexPath);
			var cell = (BGRollViewCell)collectionView.DequeueReusableCell (new NSString(reusableId), indexPath);
			InboxBlah inboxBlah = dataSource [indexPath.Item];
			var size = manager.GetCellSizeF (reusableId);
			cell.SetCellProperties (inboxBlah, reusableId, size, indexPath);

            // if we have an image AND a string, we do the animation
            if ((!String.IsNullOrEmpty(inboxBlah.T)) && (!String.IsNullOrEmpty(inboxBlah.ImageURL)))
            {
                    UIView.Animate(2, 2, UIViewAnimationOptions.CurveEaseInOut | UIViewAnimationOptions.Autoreverse | UIViewAnimationOptions.Repeat | UIViewAnimationOptions.AllowUserInteraction, 
                        () => 
                        {
                            cell.textView.Alpha = 0;
                        }, 
                        () => 
                        {
                            cell.textView.Alpha = .9f; 
                        }
                    );

            }


			if(indexPath.Item == dataSource.Count - 1 && dataSource.Count > 1000)
			{
				DeleteFirst350Items ();
			}
			return cell;
		}

		public override int NumberOfSections (UICollectionView collectionView)
		{
			return 1;
		}

		public override int GetItemsCount (UICollectionView collectionView, int section)
		{
			return dataSource.Count;
		}

		#endregion

		#region Methods

		public void InsertItems(Inbox inb)
		{
			viewController.CollectionView.PerformBatchUpdates (() => {
				inb.PrepareBlahs ();
				var preparedInbox = new Inbox ();
				NSIndexPath[] paths = new NSIndexPath[100];
				int count = dataSource.Count;

				if( inb != null && inb.Count > 0 )
				{
					for (int i = count, j = 0; i < count + 100; i++, j++) {
						paths [j] = NSIndexPath.FromItemSection (i, 0);
						string sizeName = manager.GetCellSize (paths [j]);
						dataSource.Add (inb.PopBlah (GetInboxBlahSize (sizeName)));
					}
					viewController.CollectionView.InsertItems (paths);
				}

			}, (bool finished) => {
				Console.WriteLine ("Inserted: " + finished.ToString ());
				if(dataSource.Count > 1000)
				{
					viewController.DeleteFirst200Items ();
				}
			});
		}

		public void DeleteFirst350Items()
		{
			viewController.CollectionView.PerformBatchUpdates(()=> {
				dataSource.RemoveRange (0, 350);
				var indexPaths = new NSIndexPath[350];
				for(int i = 0; i < 350; i++)
				{
					indexPaths[i] = NSIndexPath.FromItemSection(i, 0);
				}
				viewController.CollectionView.DeleteItems(indexPaths);
			}, (bool finished) => {
				Console.WriteLine("Deleted: " + finished.ToString());
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

