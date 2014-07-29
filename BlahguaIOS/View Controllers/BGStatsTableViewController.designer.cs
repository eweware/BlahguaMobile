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
	[Register ("BGStatsTableViewController")]
	partial class BGStatsTableViewController
	{
		[Outlet]
		MonoTouch.UIKit.UIToolbar bottomToolBar { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem commentsView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblComment { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblConversionRatio { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblDemotes { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblHeardRatio { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblImpression { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblOpen { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblOpenedImpression { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblPromotes { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIScrollView scrollView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem signInBtn { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem statsView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem summaryView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (bottomToolBar != null) {
				bottomToolBar.Dispose ();
				bottomToolBar = null;
			}

			if (commentsView != null) {
				commentsView.Dispose ();
				commentsView = null;
			}

			if (signInBtn != null) {
				signInBtn.Dispose ();
				signInBtn = null;
			}

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

			if (statsView != null) {
				statsView.Dispose ();
				statsView = null;
			}

			if (summaryView != null) {
				summaryView.Dispose ();
				summaryView = null;
			}
		}
	}
}
