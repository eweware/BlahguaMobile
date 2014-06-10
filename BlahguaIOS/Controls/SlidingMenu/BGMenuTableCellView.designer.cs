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
	[Register ("BGMenuTableCellView")]
	partial class BGMenuTableCellView
	{
		[Outlet]
		MonoTouch.UIKit.UILabel label { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView selectedImage { get; set; }
		
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
