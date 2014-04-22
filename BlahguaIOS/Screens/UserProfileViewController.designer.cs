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
	[Register ("UserProfileViewController")]
	partial class UserProfileViewController
	{
		[Outlet]
		MonoTouch.UIKit.UIImageView userProfileAvatarImageView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel userProfileBadgesLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView userProfileBadgesView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel userProfileGeneralLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView userProfileMainPartView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel userProfileUserNameLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (userProfileGeneralLabel != null) {
				userProfileGeneralLabel.Dispose ();
				userProfileGeneralLabel = null;
			}

			if (userProfileMainPartView != null) {
				userProfileMainPartView.Dispose ();
				userProfileMainPartView = null;
			}

			if (userProfileAvatarImageView != null) {
				userProfileAvatarImageView.Dispose ();
				userProfileAvatarImageView = null;
			}

			if (userProfileUserNameLabel != null) {
				userProfileUserNameLabel.Dispose ();
				userProfileUserNameLabel = null;
			}

			if (userProfileBadgesLabel != null) {
				userProfileBadgesLabel.Dispose ();
				userProfileBadgesLabel = null;
			}

			if (userProfileBadgesView != null) {
				userProfileBadgesView.Dispose ();
				userProfileBadgesView = null;
			}
		}
	}
}
