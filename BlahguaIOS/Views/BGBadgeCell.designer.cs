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
	[Register ("BGBadgeCell")]
	partial class BGBadgeCell
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		MonoTouch.UIKit.UIImageView badgeImage { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		MonoTouch.UIKit.UILabel name { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (badgeImage != null) {
				badgeImage.Dispose ();
				badgeImage = null;
			}
			if (name != null) {
				name.Dispose ();
				name = null;
			}
		}
	}
}
