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
	[Register ("EmbeddedWebView")]
	partial class EmbeddedWebView
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIWebView EmbeddedWebView { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (EmbeddedWebView != null) {
				EmbeddedWebView.Dispose ();
				EmbeddedWebView = null;
			}
		}
	}
}
