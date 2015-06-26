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
	[Register ("BGHistoryTableCell")]
	partial class BGHistoryTableCell
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel count { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel name { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (count != null) {
				count.Dispose ();
				count = null;
			}
			if (name != null) {
				name.Dispose ();
				name = null;
			}
		}
	}
}
