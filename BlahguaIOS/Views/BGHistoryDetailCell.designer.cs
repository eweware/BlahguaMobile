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
	[Register ("BGHistoryDetailCell")]
	partial class BGHistoryDetailCell
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextView text { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel upAndDownVotes { get; set; }

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
