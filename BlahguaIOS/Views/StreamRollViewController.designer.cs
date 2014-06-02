// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace Eweware
{
	[Register ("StreamRollViewController")]
	partial class StreamRollViewController
	{
		[Outlet]
		MonoTouch.UIKit.UIScrollView streamRollScrollView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (streamRollScrollView != null) {
				streamRollScrollView.Dispose ();
				streamRollScrollView = null;
			}
		}
	}
}
