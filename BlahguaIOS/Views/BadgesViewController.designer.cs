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
	[Register ("BadgesViewController")]
	partial class BadgesViewController
	{
		[Outlet]
		MonoTouch.UIKit.UIButton badgesDoneButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextField badgesEmailLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView badgesExistBadgesView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel badgesTopLabel { get; set; }

		[Action ("badgesDoneButtonPressed:")]
		partial void badgesDoneButtonPressed (MonoTouch.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (badgesDoneButton != null) {
				badgesDoneButton.Dispose ();
				badgesDoneButton = null;
			}

			if (badgesEmailLabel != null) {
				badgesEmailLabel.Dispose ();
				badgesEmailLabel = null;
			}

			if (badgesExistBadgesView != null) {
				badgesExistBadgesView.Dispose ();
				badgesExistBadgesView = null;
			}

			if (badgesTopLabel != null) {
				badgesTopLabel.Dispose ();
				badgesTopLabel = null;
			}
		}
	}
}
