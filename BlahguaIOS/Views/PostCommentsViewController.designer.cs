// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace Eweware
{
	[Register ("PostCommentsViewController")]
	partial class PostCommentsViewController
	{
		[Outlet]
		MonoTouch.UIKit.UITableView commentsTableView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIToolbar toolbar { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem toolbarButtonArrowDown { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem toolbarButtonArrowUp { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem toolbarItemCommentsButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView view { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (view != null) {
				view.Dispose ();
				view = null;
			}

			if (commentsTableView != null) {
				commentsTableView.Dispose ();
				commentsTableView = null;
			}

			if (toolbar != null) {
				toolbar.Dispose ();
				toolbar = null;
			}

			if (toolbarButtonArrowUp != null) {
				toolbarButtonArrowUp.Dispose ();
				toolbarButtonArrowUp = null;
			}

			if (toolbarButtonArrowDown != null) {
				toolbarButtonArrowDown.Dispose ();
				toolbarButtonArrowDown = null;
			}

			if (toolbarItemCommentsButton != null) {
				toolbarItemCommentsButton.Dispose ();
				toolbarItemCommentsButton = null;
			}
		}
	}
}
