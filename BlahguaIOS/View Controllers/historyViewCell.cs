
using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace BlahguaMobile.IOS
{
	public class historyViewCell : UITableViewCell
	{
		public static readonly NSString Key = new NSString ("historyViewCell");

		public historyViewCell () : base (UITableViewCellStyle.Value1, Key)
		{
			// TODO: add subviews to the ContentView, set various colors, etc.
			TextLabel.Text = "TextLabel";
		}
	}
}

