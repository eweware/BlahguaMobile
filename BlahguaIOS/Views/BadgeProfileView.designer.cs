// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace TestApp
{
	partial class BadgeProfileView
	{
		[Outlet]
		MonoTouch.UIKit.UIImageView badgeImageView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel badgeNameLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (badgeNameLabel != null) {
				badgeNameLabel.Dispose ();
				badgeNameLabel = null;
			}

			if (badgeImageView != null) {
				badgeImageView.Dispose ();
				badgeImageView = null;
			}
		}
	}
}
