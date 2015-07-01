// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace BlahguaMobile.IOS
{
	[Register ("BGMenuTableCellView")]
	partial class BGMenuTableCellView
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel label { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIImageView selectedImage { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (label != null) {
				label.Dispose ();
				label = null;
			}
			if (selectedImage != null) {
				selectedImage.Dispose ();
				selectedImage = null;
			}
		}
	}
}
