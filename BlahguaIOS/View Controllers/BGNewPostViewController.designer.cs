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
	[Register ("BGNewPostViewController")]
	partial class BGNewPostViewController
	{
		[Outlet]
		MonoTouch.UIKit.UIImageView back { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextField bodyInput { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton done { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITableView pollItemsTableView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton selectImageButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton selectSignature { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextField titleInput { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
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

			if (pollItemsTableView != null) {
				pollItemsTableView.Dispose ();
				pollItemsTableView = null;
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
