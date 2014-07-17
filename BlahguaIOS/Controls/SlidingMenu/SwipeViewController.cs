using System;
using System.Collections.Generic;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using MonoTouch.CoreAnimation;
using MonoTouch.CoreGraphics;
using System.Drawing;
using BlahguaMobile.IOS;
using BlahguaMobile.BlahguaCore;

namespace MonoTouch.SlideMenu
{
	public enum VIEW_TYPE
	{
		SUMMARY_VIEW = 0,
		COMMENT_VIEW,
		STATS_VIEW
	}
	public class SwipeViewController : UIViewController
	{
		const float WIDTH_OF_CONTENT_VIEW_VISIBLE = 160.0f;
		const float ANIMATION_DURATION = 0.3f;
		const float SCALE = 1.0f;
		private float currentScale = 1.0f;

		private VIEW_TYPE view_type;

		BGCommentsViewController commentViewController;
		BGStatsTableViewController statsViewController;
		BGBlahViewController summaryViewController;

		public SwipeViewController (BGBlahViewController blahView,BGCommentsViewController commentView, BGStatsTableViewController statsView)
		{
			this.SetCommentViewController(commentView);
			this.SetStatsViewController  (statsView);
			this.SetSummaryViewController(blahView);
		}

		protected override void Dispose (bool disposing)
		{
			if (disposing) {
				commentViewController.Dispose();
				statsViewController.Dispose ();
			summaryViewController.Dispose ();

			summaryViewController = null;
			commentViewController = null;
			statsViewController = null;

			}

			base.Dispose (disposing);
		}

		// - (void)setMenuViewController:(UIViewController *)menuViewController
		public void SetCommentViewController (BGCommentsViewController controller)
		{
			if (commentViewController != controller) {

				if (commentViewController != null) {
					commentViewController.WillMoveToParentViewController (null);
					commentViewController.RemoveFromParentViewController ();
					commentViewController.Dispose ();
				}

				commentViewController = controller;
				AddChildViewController (commentViewController);
				commentViewController.DidMoveToParentViewController (this);
			}
		}
		public void SetStatsViewController (BGStatsTableViewController controller)
		{
			if (statsViewController != controller) {

				if (statsViewController != null) {
					statsViewController.WillMoveToParentViewController (null);
					statsViewController.RemoveFromParentViewController ();
					statsViewController.Dispose ();
				}

				statsViewController = controller;
				AddChildViewController (statsViewController);
				statsViewController.DidMoveToParentViewController (this);
			}
		}
		// - (void)setContentViewController:(UIViewController *)contentViewController
		public void SetSummaryViewController (BGBlahViewController controller)
		{
			if (summaryViewController != controller) 
			{
				if (summaryViewController != null) {
					summaryViewController.WillMoveToParentViewController(null);
					summaryViewController.RemoveFromParentViewController();
					summaryViewController.Dispose();
				}

				summaryViewController = controller;
				summaryViewController.WillMoveToParentViewController(this);
				AddChildViewController(summaryViewController);
				summaryViewController.DidMoveToParentViewController(this);
			}
		}

		// #pragma mark - View lifecycle

		// - (void)viewDidLoad
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			summaryViewController.View.Frame = new RectangleF (View.Bounds.X, View.Bounds.Y + 44, View.Bounds.Width, View.Bounds.Height - 44);
			View.AddSubview(summaryViewController.View);

			view_type = VIEW_TYPE.SUMMARY_VIEW;
			this.NavigationItem.Title = @"Summary";
			//SetShadowOnContentViewControllerView();

//			contentViewController.View.AddGestureRecognizer(TapGesture);
//			TapGesture.Enabled = false;

			//contentViewController.View.AddGestureRecognizer(PanGesture);
			//PanGesture.Enabled = panEnabledWhenSlideMenuIsHidden;


		}

		// - (void)viewWillAppear:(BOOL)animated
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

		}

		// - (void)viewDidAppear:(BOOL)animated
		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
			this.NavigationController.SetNavigationBarHidden (false, false);

		}

		// - (void)viewWillDisappear:(BOOL)animated
		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);

		}

		// - (void)viewDidDisappear:(BOOL)animated
		public override void ViewDidDisappear (bool animated)
		{
			base.ViewDidDisappear (animated);

		}

		public void SwipeToLeft()
		{
			if (view_type == VIEW_TYPE.SUMMARY_VIEW) {
				summaryViewController.View.RemoveFromSuperview ();
				commentViewController.View.Frame = new RectangleF (View.Bounds.X, View.Bounds.Y + 44, View.Bounds.Width, View.Bounds.Height - 44);
				View.AddSubview (commentViewController.View);
				view_type = VIEW_TYPE.COMMENT_VIEW;
				this.NavigationItem.Title = "Comment";

			} else if (view_type == VIEW_TYPE.COMMENT_VIEW) {
				commentViewController.View.RemoveFromSuperview ();
				statsViewController.View.Frame = new RectangleF (View.Bounds.X, View.Bounds.Y + 44, View.Bounds.Width, View.Bounds.Height - 44);
				View.AddSubview (statsViewController.View);
				view_type = VIEW_TYPE.STATS_VIEW;

				this.NavigationItem.Title = "Stats";
			}
		}

		public void SwipeToRight()
		{
			if (view_type == VIEW_TYPE.STATS_VIEW) {
				statsViewController.View.RemoveFromSuperview ();
				commentViewController.View.Frame = new RectangleF (View.Bounds.X, View.Bounds.Y + 44, View.Bounds.Width, View.Bounds.Height - 44);
				View.AddSubview (commentViewController.View);
				view_type = VIEW_TYPE.COMMENT_VIEW;

				this.NavigationItem.Title = "Comment";

			} else if (view_type == VIEW_TYPE.COMMENT_VIEW) {
				commentViewController.View.RemoveFromSuperview ();
				summaryViewController.View.Frame = new RectangleF (View.Bounds.X, View.Bounds.Y + 44, View.Bounds.Width, View.Bounds.Height - 44);
				View.AddSubview (summaryViewController.View);
				view_type = VIEW_TYPE.SUMMARY_VIEW;
				this.NavigationItem.Title = "Summary";
			}
		}

		public void SwipeFromSummaryToStats()
		{
			if (view_type == VIEW_TYPE.SUMMARY_VIEW) {
				summaryViewController.View.RemoveFromSuperview ();
				statsViewController.View.Frame = new RectangleF (View.Bounds.X, View.Bounds.Y + 44, View.Bounds.Width, View.Bounds.Height - 44);
				View.AddSubview (statsViewController.View);
				view_type = VIEW_TYPE.STATS_VIEW;
				this.NavigationItem.Title = "Stats";
			}
		}
		public void SwipeFromStatsToSummary()
		{
			if (view_type == VIEW_TYPE.STATS_VIEW) {
				statsViewController .View.RemoveFromSuperview ();
				summaryViewController.View.Frame = new RectangleF (View.Bounds.X, View.Bounds.Y + 44, View.Bounds.Width, View.Bounds.Height - 44);
				View.AddSubview (summaryViewController.View);
				view_type = VIEW_TYPE.SUMMARY_VIEW;
				this.NavigationItem.Title = "Summary";
			}
		}

		// #pragma mark - Appearance & rotation callbacks


		// - (BOOL)shouldAutomaticallyForwardAppearanceMethods
		public override bool ShouldAutomaticallyForwardAppearanceMethods {
			get {
				return false;
			}
		}

		// - (BOOL)shouldAutomaticallyForwardRotationMethods
		public override bool ShouldAutomaticallyForwardRotationMethods {
			get {
				return true;
			}
		}

		// - (BOOL)automaticallyForwardAppearanceAndRotationMethodsToChildViewControllers
		[Obsolete]
		public override bool AutomaticallyForwardAppearanceAndRotationMethodsToChildViewControllers {
			get {
				return false;
			}
		}
	}
}

		// #pragma mark - Rotation
