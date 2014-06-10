using System;

using MonoTouch.UIKit;
using MonoTouch.Foundation;

using BlahguaMobile.BlahguaCore;

namespace BlahguaMobile.IOS
{

	public class BGRollViewDataSource : UICollectionViewDataSource
	{
		#region Fields

		private Inbox inbox;

		#endregion

		public BGRollViewDataSource () : base()
		{

			BlahguaAPIObject.Current.GetInbox (InboxLoadingCompleted);
		}

		#region Collection View Data Source overriden methods

		public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
		{
		}

		public override int NumberOfSections (UICollectionView collectionView)
		{
			return 1;
		}

		public override int GetItemsCount (UICollectionView collectionView, int section)
		{
			return inbox.Count;
		}

		#endregion

		#region Methods

		private void InboxLoadingCompleted(Inbox inbox)
		{
			if(inbox == null)
			{
				inbox.PrepareBlahs ();
				inbox = inbox;
			}
			else
			{
				inbox.PrepareBlahs ();
				inbox.AddRange (inbox);
			}
		}

		#endregion
	}
}

