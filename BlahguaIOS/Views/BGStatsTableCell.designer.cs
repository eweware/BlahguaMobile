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
	[Register ("BGStatsTableCell")]
	partial class BGStatsTableCell
	{
		[Outlet]
		MonoTouch.UIKit.UILabel key { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel value { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (key != null) {
				key.Dispose ();
				key = null;
			}

			if (value != null) {
				value.Dispose ();
				value = null;
			}
		}
	}
}
