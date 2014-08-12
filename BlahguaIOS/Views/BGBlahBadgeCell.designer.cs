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
	[Register ("BGBlahBadgeCell")]
	partial class BGBlahBadgeCell
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIImageView badgeImage { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel badgeName { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel verifiedLabel { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (badgeImage != null) {
				badgeImage.Dispose ();
				badgeImage = null;
			}
			if (badgeName != null) {
				badgeName.Dispose ();
				badgeName = null;
			}
			if (verifiedLabel != null) {
				verifiedLabel.Dispose ();
				verifiedLabel = null;
			}
		}
	}
}
