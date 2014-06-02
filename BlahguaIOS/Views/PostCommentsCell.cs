using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Eweware
{
	public partial class PostCommentsCell : UITableViewCell
	{
		public static readonly UINib Nib = UINib.FromName ("PostCommentsCell", NSBundle.MainBundle);
		public static readonly NSString Key = new NSString ("PostCommentsCell");

		public PostCommentsCell (IntPtr handle) : base (handle)
		{
		}

		public static PostCommentsCell Create ()
		{
			return (PostCommentsCell)Nib.Instantiate (null, null) [0];
		}
	}
}

