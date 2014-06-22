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
	[Register ("BGHistoryTableCell")]
	partial class BGHistoryTableCell
	{
		[Outlet]
		MonoTouch.UIKit.UILabel count { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel name { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (name != null) {
				name.Dispose ();
				name = null;
			}

			if (count != null) {
				count.Dispose ();
				count = null;
			}
		}
	}
}
