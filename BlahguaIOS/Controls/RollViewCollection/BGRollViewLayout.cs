﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using CoreGraphics;

using UIKit;
using Foundation;

namespace BlahguaMobile.IOS
{
	[Register("BGRollViewLayout")]
	public class BGRollViewLayout : UICollectionViewFlowLayout
	{
		#region Fields

		private BGRollViewCellsSizeManager manager;
		private BGRollViewController viewController;

		#endregion

		#region Properties

		private int ElementsCount
		{
			get
			{
                return (int)((BGRollViewDataSource)viewController.CollectionView.DataSource).GetItemsCount (viewController.CollectionView, 0);
			}
		}

		#endregion

		#region Constructors

		public BGRollViewLayout (BGRollViewCellsSizeManager manager, BGRollViewController viewController) : base()
		{
			this.manager = manager;
			this.viewController = viewController;
		}

		#endregion

		#region UICollectionViewFlowLayout Overriden Methods and Properties

		public override CGSize CollectionViewContentSize 
		{
			get 
			{
				return manager.GetContentViewSize(ElementsCount);
			}
		}

		public override UICollectionViewLayoutAttributes[] LayoutAttributesForElementsInRect (CGRect rect)
		{
			List<UICollectionViewLayoutAttributes> attributes = new List<UICollectionViewLayoutAttributes> ();
			int itemsCount = ElementsCount;
			if(itemsCount != 0)
			{
				var indexPathes = manager.GetIndexPathForRect (rect);
				bool shouldRefresh = false;

				foreach(var indexPath in indexPathes)
				{
					if (indexPath.Item == itemsCount - 99)
						shouldRefresh = true;

					if(indexPath.Item < itemsCount)
						attributes.Add (LayoutAttributesForItem (indexPath));
				}

				//if (shouldRefresh)
				//	viewController.RefreshData ();

			}
			return attributes.ToArray();
		}

		public override UICollectionViewLayoutAttributes LayoutAttributesForItem (NSIndexPath indexPath)
		{
			UICollectionViewLayoutAttributes attributes = UICollectionViewLayoutAttributes.CreateForCell (indexPath);
			attributes.Frame = manager.GetFrameForItem (indexPath);
			return attributes;
		}


		#endregion

		#region Methods



		#endregion
	}
}

