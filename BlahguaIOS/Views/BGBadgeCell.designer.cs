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
	[Register ("BGBadgeCell")]
	partial class BGBadgeCell
	{
		[Outlet]
		MonoTouch.UIKit.UIImageView badgeImage { get; set; }

		[Outlet]
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
