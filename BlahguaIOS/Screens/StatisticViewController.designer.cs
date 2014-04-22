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
	[Register ("StatisticViewController")]
	partial class StatisticViewController
	{
		[Outlet]
		MonoTouch.UIKit.UILabel statisticBlahguaScoreLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel statisticBlahguaScoreLabelValueLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel statisticConversionRationLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel statisticConversionRatioValueLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel statisticDescriptionLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel statisticOpenedImpressionsLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel statisticOpenedImpressionsValueLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIScrollView statisticScrollView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel statisticTitleLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIToolbar toolbar { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem toolbarItebStatisticButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem toolbarItemArrowDownButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem toolbarItemArrowUpButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem toolbarItemCommentsButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem toolbarItemPostButton { get; set; }

		[Action ("toolbarItemArrowDownButtonPressed:")]
		partial void toolbarItemArrowDownButtonPressed (MonoTouch.Foundation.NSObject sender);

		[Action ("toolBarItemArrowUpButtnPressed:")]
		partial void toolBarItemArrowUpButtnPressed (MonoTouch.Foundation.NSObject sender);

		[Action ("toolbarItemCommentsButtonPressed:")]
		partial void toolbarItemCommentsButtonPressed (MonoTouch.Foundation.NSObject sender);

		[Action ("toolbarItemPostButtonPressed:")]
		partial void toolbarItemPostButtonPressed (MonoTouch.Foundation.NSObject sender);

		[Action ("toolbarItemStatisticButtonPressed:")]
		partial void toolbarItemStatisticButtonPressed (MonoTouch.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (statisticBlahguaScoreLabel != null) {
				statisticBlahguaScoreLabel.Dispose ();
				statisticBlahguaScoreLabel = null;
			}

			if (statisticBlahguaScoreLabelValueLabel != null) {
				statisticBlahguaScoreLabelValueLabel.Dispose ();
				statisticBlahguaScoreLabelValueLabel = null;
			}

			if (statisticConversionRationLabel != null) {
				statisticConversionRationLabel.Dispose ();
				statisticConversionRationLabel = null;
			}

			if (statisticConversionRatioValueLabel != null) {
				statisticConversionRatioValueLabel.Dispose ();
				statisticConversionRatioValueLabel = null;
			}

			if (statisticDescriptionLabel != null) {
				statisticDescriptionLabel.Dispose ();
				statisticDescriptionLabel = null;
			}

			if (statisticOpenedImpressionsLabel != null) {
				statisticOpenedImpressionsLabel.Dispose ();
				statisticOpenedImpressionsLabel = null;
			}

			if (statisticOpenedImpressionsValueLabel != null) {
				statisticOpenedImpressionsValueLabel.Dispose ();
				statisticOpenedImpressionsValueLabel = null;
			}

			if (statisticScrollView != null) {
				statisticScrollView.Dispose ();
				statisticScrollView = null;
			}

			if (statisticTitleLabel != null) {
				statisticTitleLabel.Dispose ();
				statisticTitleLabel = null;
			}

			if (toolbar != null) {
				toolbar.Dispose ();
				toolbar = null;
			}

			if (toolbarItemArrowUpButton != null) {
				toolbarItemArrowUpButton.Dispose ();
				toolbarItemArrowUpButton = null;
			}

			if (toolbarItemArrowDownButton != null) {
				toolbarItemArrowDownButton.Dispose ();
				toolbarItemArrowDownButton = null;
			}

			if (toolbarItemCommentsButton != null) {
				toolbarItemCommentsButton.Dispose ();
				toolbarItemCommentsButton = null;
			}

			if (toolbarItemPostButton != null) {
				toolbarItemPostButton.Dispose ();
				toolbarItemPostButton = null;
			}

			if (toolbarItebStatisticButton != null) {
				toolbarItebStatisticButton.Dispose ();
				toolbarItebStatisticButton = null;
			}
		}
	}
}
