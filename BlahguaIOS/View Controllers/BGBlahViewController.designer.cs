// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace BlahguaMobile.IOS
{
	[Register ("BGBlahViewController")]
	partial class BGBlahViewController
	{
		[Outlet]
		MonoTouch.UIKit.UILabel author { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton badgeImage { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITableView badgesTableView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextView blahBodyView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView blahImage { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel blahTimespan { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel blahTitle { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIToolbar bottomToolbar { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem commentsView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIScrollView contentView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem downVote { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem signInBtn { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem statsView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem summaryView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextView txtBlahTitle { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem upVote { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel userDescription { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView userImage { get; set; }
		void ReleaseDesignerOutlets ()
		{
			if (author != null) {
				author.Dispose ();
				author = null;
			}











			if (signInBtn != null) {
				signInBtn.Dispose ();
				signInBtn = null;
			}






			if (txtBlahTitle != null) {
				txtBlahTitle.Dispose ();
				txtBlahTitle = null;
			}
		}
	}
}
