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
	[Register ("BGNewPostPollItemCell")]
	partial class BGNewPostPollItemCell
	{
		[Outlet]
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
