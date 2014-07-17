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
	[Register ("BGStatsTableViewController")]
	partial class BGStatsTableViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		MonoTouch.UIKit.UILabel lblComment { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		MonoTouch.UIKit.UILabel lblConversionRatio { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		MonoTouch.UIKit.UILabel lblDemotes { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		MonoTouch.UIKit.UILabel lblHeardRatio { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		MonoTouch.UIKit.UILabel lblImpression { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		MonoTouch.UIKit.UILabel lblOpen { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		MonoTouch.UIKit.UILabel lblOpenedImpression { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		MonoTouch.UIKit.UILabel lblPromotes { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		MonoTouch.UIKit.UIScrollView scrollView { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (lblComment != null) {
				lblComment.Dispose ();
				lblComment = null;
			}
			if (lblConversionRatio != null) {
				lblConversionRatio.Dispose ();
				lblConversionRatio = null;
			}
			if (lblDemotes != null) {
				lblDemotes.Dispose ();
				lblDemotes = null;
			}
			if (lblHeardRatio != null) {
				lblHeardRatio.Dispose ();
				lblHeardRatio = null;
			}
			if (lblImpression != null) {
				lblImpression.Dispose ();
				lblImpression = null;
			}
			if (lblOpen != null) {
				lblOpen.Dispose ();
				lblOpen = null;
			}
			if (lblOpenedImpression != null) {
				lblOpenedImpression.Dispose ();
				lblOpenedImpression = null;
			}
			if (lblPromotes != null) {
				lblPromotes.Dispose ();
				lblPromotes = null;
			}
			if (scrollView != null) {
				scrollView.Dispose ();
				scrollView = null;
			}
		}
	}
}
