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
	[Register ("BlahRoll")]
	partial class BlahRoll
	{
		[Outlet]
		MonoTouch.UIKit.UIScrollView blahContainer { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton detailsBtn { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton profileBtn { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton signInBtn { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (detailsBtn != null) {
				detailsBtn.Dispose ();
				detailsBtn = null;
			}

			if (profileBtn != null) {
				profileBtn.Dispose ();
				profileBtn = null;
			}

			if (signInBtn != null) {
				signInBtn.Dispose ();
				signInBtn = null;
			}

			if (blahContainer != null) {
				blahContainer.Dispose ();
				blahContainer = null;
			}
		}
	}
}
