using System;

using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace BlahguaMobile.IOS
{
	[Register ("BGPollTableHeaderView")]
	public partial class BGPollTableHeaderView : UIView
	{

		public static readonly UINib Nib = UINib.FromName ("BGPollTableHeaderView", NSBundle.MainBundle);
		public static readonly NSString Key = new NSString ("BGPollTableHeaderView");

		public BGPollTableHeaderView (IntPtr handle) : base (handle)
		{
		}

		public static BGPollTableHeaderView Create (string headerText)
		{
			var view = (BGPollTableHeaderView)Nib.Instantiate (null, null) [0];
			view.BackgroundColor = UIColor.Red;
			view.headerText.AttributedText = new NSAttributedString (
				headerText, 
				UIFont.FromName (BGAppearanceConstants.MediumFontName, 17), 
				UIColor.Black
			);

			view.headerText.TextAlignment = UITextAlignment.Center;
			return view;
		}
	}
}

