// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System;
using System.CodeDom.Compiler;

namespace BlahguaMobile.IOS
{
	[Register ("BGNewPostViewController")]
	partial class BGNewPostViewController
	{
		[Outlet]
		MonoTouch.UIKit.UIButton AskBtn { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel AskBtnText { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView back { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextView bodyInput { get; set; }

		[Outlet]
		MonoTouch.UIKit.NSLayoutConstraint buttonTopOffset { get; set; }

		[Outlet]
		MonoTouch.UIKit.NSLayoutConstraint containerHeight { get; set; }

		[Outlet]
		MonoTouch.UIKit.NSLayoutConstraint containerOffset { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIScrollView containerScrollView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton done { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton LeakBtn { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel LeakBtnText { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton PollBtn { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel PollBtnText { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITableView pollItemsTableView { get; set; }

		[Outlet]
		MonoTouch.UIKit.NSLayoutConstraint pollOptionTableHeight { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton PredictBtn { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel PredictBtnText { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton SayBtn { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel SayBtnText { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton selectImageButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton selectSignature { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextField titleInput { get; set; }

		[Action ("HandleTitleChanged:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void HandleTitleChanged (UITextField sender);

		void ReleaseDesignerOutlets ()
		{
		}
	}
}
