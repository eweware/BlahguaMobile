// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using UIKit;
using System;
using System.CodeDom.Compiler;

namespace BlahguaMobile.IOS
{
	[Register ("BGNewPostViewController")]
	partial class BGNewPostViewController
	{
		[Outlet]
		UIKit.UIButton AskBtn { get; set; }

		[Outlet]
		UIKit.UILabel AskBtnText { get; set; }

		[Outlet]
		UIKit.UIImageView back { get; set; }

		[Outlet]
		UIKit.UITextView bodyInput { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint buttonTopOffset { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint containerHeight { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint containerOffset { get; set; }

		[Outlet]
		UIKit.UIScrollView containerScrollView { get; set; }

		[Outlet]
		UIKit.UIButton done { get; set; }

		[Outlet]
		UIKit.UIButton LeakBtn { get; set; }

		[Outlet]
		UIKit.UILabel LeakBtnText { get; set; }

		[Outlet]
		UIKit.UIButton PollBtn { get; set; }

		[Outlet]
		UIKit.UILabel PollBtnText { get; set; }

		[Outlet]
		UIKit.UITableView pollItemsTableView { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint pollOptionTableHeight { get; set; }

		[Outlet]
		UIKit.UIButton PredictBtn { get; set; }

		[Outlet]
		UIKit.UILabel PredictBtnText { get; set; }

		[Outlet]
		UIKit.UIButton SayBtn { get; set; }

		[Outlet]
		UIKit.UILabel SayBtnText { get; set; }

		[Outlet]
		UIKit.UIButton selectImageButton { get; set; }

		[Outlet]
		UIKit.UIButton selectSignature { get; set; }

		[Outlet]
		UIKit.UITextField titleInput { get; set; }

		[Action ("HandleTitleChanged:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void HandleTitleChanged (UITextField sender);

		void ReleaseDesignerOutlets ()
		{
		}
	}
}
