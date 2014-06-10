using System;
using System.Drawing;

using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace BlahguaMobile.IOS
{
	[Register("BGRollViewLayout")]
	public class BGRollViewLayout : UICollectionViewFlowLayout
	{
		public BGRollViewLayout () : base()
		{
		}

		public override UICollectionViewLayoutAttributes[] LayoutAttributesForElementsInRect (System.Drawing.RectangleF rect)
		{
			return base.LayoutAttributesForElementsInRect (rect);
		}

		public override UICollectionViewLayoutAttributes LayoutAttributesForItem (NSIndexPath indexPath)
		{
			return base.LayoutAttributesForItem (indexPath);
		}
	}

	public class BGRollViewLayoutDelegate : UICollectionViewDelegateFlowLayout
	{
		public override SizeF GetSizeForItem (UICollectionView collectionView, UICollectionViewLayout layout, NSIndexPath indexPath)
		{

		}
	}
}

