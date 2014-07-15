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
		[GeneratedCode ("iOS Designer", "1.0")]
		UIImageView AskBtn { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIImageView back { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextField bodyInput { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton done { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIImageView LeakBtn { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIImageView PollBtn { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITableView pollItemsTableView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIImageView PredictBtn { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIImageView SayBtn { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton selectImageButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton selectSignature { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextField titleInput { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (AskBtn != null) {
				AskBtn.Dispose ();
				AskBtn = null;
			}
			if (back != null) {
				back.Dispose ();
				back = null;
			}
			if (bodyInput != null) {
				bodyInput.Dispose ();
				bodyInput = null;
			}
			if (done != null) {
				done.Dispose ();
				done = null;
			}
			if (LeakBtn != null) {
				LeakBtn.Dispose ();
				LeakBtn = null;
			}
			if (PollBtn != null) {
				PollBtn.Dispose ();
				PollBtn = null;
			}
			if (pollItemsTableView != null) {
				pollItemsTableView.Dispose ();
				pollItemsTableView = null;
			}
			if (PredictBtn != null) {
				PredictBtn.Dispose ();
				PredictBtn = null;
			}
			if (SayBtn != null) {
				SayBtn.Dispose ();
				SayBtn = null;
			}
			if (selectImageButton != null) {
				selectImageButton.Dispose ();
				selectImageButton = null;
			}
			if (selectSignature != null) {
				selectSignature.Dispose ();
				selectSignature = null;
			}
			if (titleInput != null) {
				titleInput.Dispose ();
				titleInput = null;
			}
		}
	}
}
