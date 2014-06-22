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
	[Register ("BGDemographicsInputCell")]
	partial class BGDemographicsInputCell
	{
		[Outlet]
		MonoTouch.UIKit.UITextField input { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton isPublicButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel publicLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (input != null) {
				input.Dispose ();
				input = null;
			}

			if (isPublicButton != null) {
				isPublicButton.Dispose ();
				isPublicButton = null;
			}

			if (publicLabel != null) {
				publicLabel.Dispose ();
				publicLabel = null;
			}
		}
	}
}
