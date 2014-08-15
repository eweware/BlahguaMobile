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
		MonoTouch.UIKit.UILabel AskBtnText { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView back { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextField bodyInput { get; set; }

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
        partial void HandleTitleChanged (MonoTouch.UIKit.UITextField sender);
        
        void ReleaseDesignerOutlets()
        {
            if (AskBtn != null) {
                AskBtn.Dispose ();
                AskBtn = null;
            }

            if (AskBtnText != null) {
                AskBtnText.Dispose ();
                AskBtnText = null;
            }

            if (back != null) {
                back.Dispose ();
                back = null;
            }

            if (bodyInput != null) {
                bodyInput.Dispose ();
                bodyInput = null;
            }

            if (containerHeight != null) {
                containerHeight.Dispose ();
                containerHeight = null;
            }

            if (containerOffset != null) {
                containerOffset.Dispose ();
                containerOffset = null;
            }

            if (containerScrollView != null) {
                containerScrollView.Dispose ();
                containerScrollView = null;
            }

            if (done != null) {
                done.Dispose ();
                done = null;
            }

            if (LeakBtn != null) {
                LeakBtn.Dispose ();
                LeakBtn = null;
            }

            if (LeakBtnText != null) {
                LeakBtnText.Dispose ();
                LeakBtnText = null;
            }

            if (PollBtn != null) {
                PollBtn.Dispose ();
                PollBtn = null;
            }

            if (PollBtnText != null) {
                PollBtnText.Dispose ();
                PollBtnText = null;
            }

            if (pollItemsTableView != null) {
                pollItemsTableView.Dispose ();
                pollItemsTableView = null;
            }

            if (pollOptionTableHeight != null) {
                pollOptionTableHeight.Dispose ();
                pollOptionTableHeight = null;
            }

            if (PredictBtn != null) {
                PredictBtn.Dispose ();
                PredictBtn = null;
            }

            if (PredictBtnText != null) {
                PredictBtnText.Dispose ();
                PredictBtnText = null;
            }

            if (SayBtn != null) {
                SayBtn.Dispose ();
                SayBtn = null;
            }

            if (SayBtnText != null) {
                SayBtnText.Dispose ();
                SayBtnText = null;
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
