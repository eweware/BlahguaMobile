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
	[Register ("BGHistoryViewController")]
	partial class BGHistoryViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		MonoTouch.UIKit.UITableView mainTable { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (mainTable != null) {
				mainTable.Dispose ();
				mainTable = null;
			}
		}
	}
}
