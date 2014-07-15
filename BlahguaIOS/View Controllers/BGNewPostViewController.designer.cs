// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.CodeDom.Compiler;

namespace BlahguaMobile.IOS
{
	[Register ("BGNewPostViewController")]
	partial class BGNewPostViewController
	{
		[Outlet]
		MonoTouch.UIKit.UIButton AskBtn { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView back { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextField bodyInput { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton done { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton LeakBtn { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton PollBtn { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITableView pollItemsTableView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton PredictBtn { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton SayBtn { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton selectImageButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton selectSignature { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextField titleInput { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel AskBtnText { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel LeakBtnText { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel PollBtnText { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel PredictBtnText { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel SayBtnText { get; set; }

		[Action ("HandleTitleChanged:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void HandleTitleChanged (UITextField sender);

		void ReleaseDesignerOutlets ()
		{
			if (AskBtnText != null) {
				AskBtnText.Dispose ();
				AskBtnText = null;
			}
			if (LeakBtnText != null) {
				LeakBtnText.Dispose ();
				LeakBtnText = null;
			}
			if (PollBtnText != null) {
				PollBtnText.Dispose ();
				PollBtnText = null;
			}
			if (PredictBtnText != null) {
				PredictBtnText.Dispose ();
				PredictBtnText = null;
			}
			if (SayBtnText != null) {
				SayBtnText.Dispose ();
				SayBtnText = null;
			}
		}
	}
}
