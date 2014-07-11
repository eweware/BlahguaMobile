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
	[Register ("BGNewPostPollItemCell")]
	partial class BGNewPostPollItemCell
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		MonoTouch.UIKit.UITextField pollItemText { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (pollItemText != null) {
				pollItemText.Dispose ();
				pollItemText = null;
			}
		}
	}
}
