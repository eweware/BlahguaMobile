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
	[Register ("BGHistoryDetailCell")]
	partial class BGHistoryDetailCell
	{
		[Outlet]
		MonoTouch.UIKit.UITextView text { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel upAndDownVotes { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (text != null) {
				text.Dispose ();
				text = null;
			}

			if (upAndDownVotes != null) {
				upAndDownVotes.Dispose ();
				upAndDownVotes = null;
			}
		}
	}
}
