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
	[Register ("BGDemographicsDropDownCell")]
	partial class BGDemographicsDropDownCell
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton ddButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton isPublicButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel publicLabel { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (ddButton != null) {
				ddButton.Dispose ();
				ddButton = null;
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
