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
	[Register ("BGDemographicsInputCell")]
	partial class BGDemographicsInputCell
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		BlahguaMobile.IOS.BGTextField input { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		MonoTouch.UIKit.UIButton isPublicButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
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
