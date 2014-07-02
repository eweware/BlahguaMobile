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
	[Register ("BGBlahBadgeCell")]
	partial class BGBlahBadgeCell
	{
		[Outlet]
		MonoTouch.UIKit.UIImageView badgeImage { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel badgeName { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel verifiedLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (badgeImage != null) {
				badgeImage.Dispose ();
				badgeImage = null;
			}

			if (verifiedLabel != null) {
				verifiedLabel.Dispose ();
				verifiedLabel = null;
			}

			if (badgeName != null) {
				badgeName.Dispose ();
				badgeName = null;
			}
		}
	}
}
