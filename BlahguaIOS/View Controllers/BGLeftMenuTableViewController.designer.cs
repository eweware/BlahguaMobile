// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System;
using System.CodeDom.Compiler;

namespace BlahguaMobile.IOS
{
	[Register ("BGLeftMenuTableViewController")]
	partial class BGLeftMenuTableViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITableView mainTableView { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (mainTableView != null) {
				mainTableView.Dispose ();
				mainTableView = null;
			}
		}
	}
}
