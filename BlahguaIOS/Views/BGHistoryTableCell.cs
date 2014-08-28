// This file has been autogenerated from a class added in the UI designer.

using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace BlahguaMobile.IOS
{
	public partial class BGHistoryTableCell : UITableViewCell
	{
		public BGHistoryTableCell (IntPtr handle) : base (handle)
		{

		}

		public void SetUp(string name, string count)
		{
            this.name.AttributedText = new NSAttributedString (name, UIFont.FromName (BGAppearanceConstants.FontName, 17), BGAppearanceConstants.DarkBrown);;
            this.count.AttributedText = new NSAttributedString (count, UIFont.FromName (BGAppearanceConstants.FontName, 17), BGAppearanceConstants.DarkBrown);;
			
		}
	}
}
