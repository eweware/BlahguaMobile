// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using UIKit;
using System;
using System.CodeDom.Compiler;

namespace BlahguaMobile.IOS
{
	[Register ("BGMenuTableHeaderView")]
	partial class BGMenuTableHeaderView
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel headerLabel { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (headerLabel != null) {
				headerLabel.Dispose ();
				headerLabel = null;
			}
		}
	}
}
