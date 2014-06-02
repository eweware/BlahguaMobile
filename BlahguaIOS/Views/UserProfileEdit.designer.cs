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
	[Register ("UserProfileEdit")]
	partial class UserProfileEdit
	{
		[Outlet]
		MonoTouch.UIKit.UIImageView userProfileAvatarImageView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView userProfileMainInfoView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView userProfileSeparatorView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel userProfileUsernameLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView view { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (view != null) {
				view.Dispose ();
				view = null;
			}

			if (userProfileMainInfoView != null) {
				userProfileMainInfoView.Dispose ();
				userProfileMainInfoView = null;
			}

			if (userProfileSeparatorView != null) {
				userProfileSeparatorView.Dispose ();
				userProfileSeparatorView = null;
			}

			if (userProfileUsernameLabel != null) {
				userProfileUsernameLabel.Dispose ();
				userProfileUsernameLabel = null;
			}

			if (userProfileAvatarImageView != null) {
				userProfileAvatarImageView.Dispose ();
				userProfileAvatarImageView = null;
			}
		}
	}
}
