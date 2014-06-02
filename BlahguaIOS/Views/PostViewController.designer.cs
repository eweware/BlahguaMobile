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
	[Register ("PostViewController")]
	partial class PostViewController
	{
		[Outlet]
		MonoTouch.UIKit.UIImageView postAvatarImageView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView postImageView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIScrollView postScrollView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextView postTextView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel postTitleLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel postUsernameLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIToolbar toolbar { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem toolbarItebStatisticButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem toolbarItemArrowDownButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem toolbarItemArrowUpButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem toolbarItemCommentsButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem toolbarItemPostButton { get; set; }

		[Action ("toolbarItemArrowDownButtonPressed:")]
		partial void toolbarItemArrowDownButtonPressed (MonoTouch.Foundation.NSObject sender);

		[Action ("toolBarItemArrowUpButtnPressed:")]
		partial void toolBarItemArrowUpButtnPressed (MonoTouch.Foundation.NSObject sender);

		[Action ("toolbarItemCommentsButtonPressed:")]
		partial void toolbarItemCommentsButtonPressed (MonoTouch.Foundation.NSObject sender);

		[Action ("toolbarItemPostButtonPressed:")]
		partial void toolbarItemPostButtonPressed (MonoTouch.Foundation.NSObject sender);

		[Action ("toolbarItemStatisticButtonPressed:")]
		partial void toolbarItemStatisticButtonPressed (MonoTouch.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (postUsernameLabel != null) {
				postUsernameLabel.Dispose ();
				postUsernameLabel = null;
			}

			if (postAvatarImageView != null) {
				postAvatarImageView.Dispose ();
				postAvatarImageView = null;
			}

			if (postImageView != null) {
				postImageView.Dispose ();
				postImageView = null;
			}

			if (postScrollView != null) {
				postScrollView.Dispose ();
				postScrollView = null;
			}

			if (postTextView != null) {
				postTextView.Dispose ();
				postTextView = null;
			}

			if (postTitleLabel != null) {
				postTitleLabel.Dispose ();
				postTitleLabel = null;
			}

			if (toolbar != null) {
				toolbar.Dispose ();
				toolbar = null;
			}

			if (toolbarItebStatisticButton != null) {
				toolbarItebStatisticButton.Dispose ();
				toolbarItebStatisticButton = null;
			}

			if (toolbarItemArrowDownButton != null) {
				toolbarItemArrowDownButton.Dispose ();
				toolbarItemArrowDownButton = null;
			}

			if (toolbarItemArrowUpButton != null) {
				toolbarItemArrowUpButton.Dispose ();
				toolbarItemArrowUpButton = null;
			}

			if (toolbarItemCommentsButton != null) {
				toolbarItemCommentsButton.Dispose ();
				toolbarItemCommentsButton = null;
			}

			if (toolbarItemPostButton != null) {
				toolbarItemPostButton.Dispose ();
				toolbarItemPostButton = null;
			}
		}
	}
}
