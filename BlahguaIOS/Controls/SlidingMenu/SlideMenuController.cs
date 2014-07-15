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
	public class SlideMenuController : UIViewController
	{
		const float WIDTH_OF_CONTENT_VIEW_VISIBLE = 160.0f;
		const float ANIMATION_DURATION = 0.3f;
		const float SCALE = 1.0f;
		private float currentScale = 1.0f;

		BGLeftMenuTableViewController menuViewController;
		BGRightMenuViewController rightMenuViewController;

		UIViewController contentViewController;

		public UINavigationController ContentViewNavigationController {
			get { 
				if (contentViewController.NavigationController != null)
				{
					return contentViewController.NavigationController;
				}
				return null;
			}
		}

        public UIViewController ContentViewController { get { return contentViewController; } }

		RectangleF contentViewControllerFrame;
		bool menuWasOpenAtPanBegin;
        bool statusBarHidden = false;
        bool scaleEnabled = true;

        bool contentViewScaled = false;

        UIView contentViewSnapshot;

		// When the menu is hidden, does the pan gesture trigger ? Default is true.
		bool panEnabledWhenSlideMenuIsHidden;

        public override UIStatusBarStyle PreferredStatusBarStyle()
        {
            return UIStatusBarStyle.LightContent;
        }
		public SlideMenuController (BGLeftMenuTableViewController menuViewController,BGRightMenuViewController rightMenuViewController, UIViewController contentViewController)
		{

			this.SetMenuViewController(menuViewController);
			this.SetRightMenuViewController (rightMenuViewController);
			this.SetContentViewController(contentViewController);



			panEnabledWhenSlideMenuIsHidden = true;
		}

		protected override void Dispose (bool disposing)
		{
			if (disposing) {
				menuViewController.Dispose();
				rightMenuViewController.Dispose ();

				rightMenuViewController = null;
				menuViewController = null;
				
				contentViewController.Dispose();
				contentViewController = null;
			}

			base.Dispose (disposing);
		}

		// - (void)setMenuViewController:(UIViewController *)menuViewController
		public void SetMenuViewController (BGLeftMenuTableViewController controller)
		{
			if (menuViewController != controller) {

				if (menuViewController != null) {
					menuViewController.WillMoveToParentViewController (null);
					menuViewController.RemoveFromParentViewController ();
					menuViewController.Dispose ();
				}

				menuViewController = controller;
				AddChildViewController (menuViewController);
				menuViewController.DidMoveToParentViewController (this);
			}
		}
		public void SetRightMenuViewController (BGRightMenuViewController controller)
		{
			if (rightMenuViewController != controller) {

				if (rightMenuViewController != null) {
					rightMenuViewController.WillMoveToParentViewController (null);
					rightMenuViewController.RemoveFromParentViewController ();
					rightMenuViewController.Dispose ();
				}

				rightMenuViewController = controller;
				AddChildViewController (rightMenuViewController);
				rightMenuViewController.DidMoveToParentViewController (this);
			}
		}
		// - (void)setContentViewController:(UIViewController *)contentViewController
		public void SetContentViewController (UIViewController controller)
		{
			if (contentViewController != controller) 
			{
				if (contentViewController != null) {
					contentViewController.WillMoveToParentViewController(null);
					contentViewController.RemoveFromParentViewController();
					contentViewController.Dispose();
				}

				contentViewController = controller;
				contentViewController.WillMoveToParentViewController(this);
				AddChildViewController(contentViewController);
				contentViewController.DidMoveToParentViewController(this);
			}
		}

		// #pragma mark - View lifecycle

		// - (void)viewDidLoad
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			View.AddSubview(contentViewController.View);
			//SetShadowOnContentViewControllerView();

//			contentViewController.View.AddGestureRecognizer(TapGesture);
//			TapGesture.Enabled = false;

			contentViewController.View.AddGestureRecognizer(PanGesture);
			PanGesture.Enabled = panEnabledWhenSlideMenuIsHidden;


		}

		// - (void)viewWillAppear:(BOOL)animated
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			if (!IsMenuOpen ()) {
				contentViewController.View.Frame = View.Bounds;
			}

			contentViewController.BeginAppearanceTransition (true, animated);
			if (menuViewController.IsViewLoaded && menuViewController.View.Superview != null) {
				menuViewController.BeginAppearanceTransition (true, animated);

			}
		}

		// - (void)viewDidAppear:(BOOL)animated
		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);

			contentViewController.EndAppearanceTransition();
			if (menuViewController.IsViewLoaded && menuViewController.View.Superview != null) {
				menuViewController.EndAppearanceTransition();
			}
		}

		// - (void)viewWillDisappear:(BOOL)animated
		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);

			contentViewController.BeginAppearanceTransition (false, animated);
			if (menuViewController.IsViewLoaded) {
				menuViewController.BeginAppearanceTransition(false, animated);
			}
		}

		// - (void)viewDidDisappear:(BOOL)animated
		public override void ViewDidDisappear (bool animated)
		{
			base.ViewDidDisappear (animated);

			contentViewController.EndAppearanceTransition ();
			if (menuViewController.IsViewLoaded) {
				menuViewController.EndAppearanceTransition();
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

		// #pragma mark - Rotation

		// - (BOOL)shouldAutorotate
		public override bool ShouldAutorotate ()
		{
			return menuViewController.ShouldAutorotate() && contentViewController.ShouldAutorotate();
		}

		// - (NSUInteger)supportedInterfaceOrientations
		public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations ()
		{
			return menuViewController.GetSupportedInterfaceOrientations() & 
				contentViewController.GetSupportedInterfaceOrientations();
		}

		// - (BOOL)shouldAutorotateToInterfaceOrientation:(UIInterfaceOrientation)toInterfaceOrientation
		[Obsolete]
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			return menuViewController.ShouldAutorotateToInterfaceOrientation(toInterfaceOrientation) && 
				contentViewController.ShouldAutorotateToInterfaceOrientation(toInterfaceOrientation);
		}

		// - (void)willAnimateRotationToInterfaceOrientation:(UIInterfaceOrientation)toInterfaceOrientation duration:(NSTimeInterval)duration
		public override void WillAnimateRotation (UIInterfaceOrientation toInterfaceOrientation, double duration)
		{
			if (IsMenuOpen ()) {
				RectangleF frame = contentViewController.View.Frame;
				frame.X = OffsetXWhenMenuIsOpen();
				UIView.Animate(duration, () => {
					contentViewController.View.Frame = frame;
				});
			}
		}


		// #pragma mark - Menu view lazy load


		// - (void)loadMenuViewControllerViewIfNeeded
		public void LoadMenuViewControllerViewIfNeeded ()
		{
			if (menuViewController.TableView.Superview == null) {
				RectangleF menuFrame = new RectangleF (View.Bounds.X, View.Bounds.Top, WIDTH_OF_CONTENT_VIEW_VISIBLE, View.Bounds.Height);   //View.Bounds;
				//menuFrame.Width -= WIDTH_OF_CONTENT_VIEW_VISIBLE; 
				menuViewController.TableView.Frame = menuFrame;

				View.InsertSubview(menuViewController.TableView, 0);
			}
		}
		public void LoadRightMenuViewControllerViewIfNeeded ()
		{
			//System.Console.WriteLine (rightMenuViewController);
			if (rightMenuViewController.View.Superview == null) {
				RectangleF menuFrame = new RectangleF (View.Bounds.Width - WIDTH_OF_CONTENT_VIEW_VISIBLE, View.Bounds.Top, View.Bounds.Width, View.Bounds.Height);// View.Bounds;
				//menuFrame.Width -= WIDTH_OF_CONTENT_VIEW_VISIBLE; 
				rightMenuViewController.View.Frame = menuFrame;
				View.InsertSubview(rightMenuViewController.View, 0);
			}
		}

		// #pragma mark - Navigation


		// - (void)setContentViewController:(UIViewController *)contentViewController animated:(BOOL)animated completion:(void(^)(BOOL finished))completion
		public void SetContentViewControllerAnimated (UIViewController controller, bool animated)
		{
			if (controller == null)
				throw new InvalidOperationException ("Can't show a null content view controller");

			if (contentViewController != controller) 
			{
				// Preserve the frame
				RectangleF frame = contentViewController.View.Frame;

				// Remove old content view
				//contentViewController.View.RemoveGestureRecognizer(TapGesture);
				contentViewController.View.RemoveGestureRecognizer(PanGesture);
				contentViewController.BeginAppearanceTransition(false, false);
				contentViewController.View.RemoveFromSuperview();
				contentViewController.EndAppearanceTransition();

                CGAffineTransform tranfsorm = contentViewController.View.Transform;
                contentViewController.View.Transform = CGAffineTransform.MakeIdentity();

				contentViewController.BeginAppearanceTransition(true, false);

				// Add the new content view
				SetContentViewController(controller);

				//add for right menu 
				this.rightMenuViewController.SetContentView (controller);


                controller.View.Transform = tranfsorm;
				contentViewController.View.Frame = frame;
				//contentViewController.View.AddGestureRecognizer(TapGesture);
				contentViewController.View.AddGestureRecognizer(PanGesture);
				View.AddSubview(contentViewController.View);
				contentViewController.EndAppearanceTransition();

   			}

            ShowContentViewControllerAnimated(animated, null, true);
		}

		// - (void)showContentViewControllerAnimated:(BOOL)animated completion:(void(^)(BOOL finished))completion
		public void ShowContentViewControllerAnimated (bool animated, UICompletionHandler completion, bool lackStatusBar = false)
		{


			// Remove gestures
			//TapGesture.Enabled = false;
			PanGesture.Enabled = panEnabledWhenSlideMenuIsHidden;

			var duration = animated ? ANIMATION_DURATION : 0;

			UIView contentView = contentViewController.View;

			menuViewController.BeginAppearanceTransition(false, animated);
			contentViewController.BeginAppearanceTransition(true, animated);

			RectangleF targetFrame = View.Bounds;
			float statusBarDelta = UIApplication.SharedApplication.StatusBarFrame.Height;

			if (lackStatusBar) {
                targetFrame.Y += statusBarDelta;
                targetFrame.Height -= statusBarDelta;
			}

			UIView.AnimateNotify(duration, 0, UIViewAnimationOptions.CurveEaseInOut, () => {
				DeScaleContentView();
				contentView.Frame = targetFrame;
			}, (finished) => {
				if (lackStatusBar) {
                    targetFrame.Y -= statusBarDelta;
                    targetFrame.Height += statusBarDelta;
				}
				contentView.Frame = targetFrame;
				menuViewController.EndAppearanceTransition();
				contentViewController.EndAppearanceTransition();
				if (completion != null) {
					completion (finished);
				}
			});
		}

		// - (IBAction)toggleMenuAnimated:(id)sender;
		public void ToggleMenuAnimated ()
		{
			if (IsMenuOpen ()) {
//				UIApplication.SharedApplication.SetStatusBarHidden (false, UIStatusBarAnimation.Slide);
				ShowContentViewControllerAnimated(true, null);
			} else {
//				UIApplication.SharedApplication.SetStatusBarHidden (true, UIStatusBarAnimation.Slide);
				ShowMenuAnimated(true, null);
			}
		}

		public void ToggleRightMenuAnimated ()
		{
			if (IsRightMenuOpen ()) {
				//				UIApplication.SharedApplication.SetStatusBarHidden (false, UIStatusBarAnimation.Slide);
				ShowContentViewControllerAnimated (true, null);
			} else {
				//				UIApplication.SharedApplication.SetStatusBarHidden (true, UIStatusBarAnimation.Slide);
				ShowRightMenuAnimated (true, null);
			}
		}
		


        public void ApplySnapshot(bool updateStatusBar = true) {
            // Previous snapshot has to be removed to preserve overlapping. 
			/*if (contentViewSnapshot == null)
            {
                Snapshot();
                contentViewController.View.AddSubview(contentViewSnapshot);
                if (updateStatusBar)
                {
                    statusBarHidden = true;
                    SetNeedsStatusBarAppearanceUpdate();
                }

            }*/
        }

        public void RemoveSnapshot(bool updateStatusBar = true) {
            if (contentViewSnapshot != null)
            {
                if (updateStatusBar)
                {
                    statusBarHidden = false;
                    SetNeedsStatusBarAppearanceUpdate();
                }
                NSTimer.CreateScheduledTimer(0.01, delegate
                {
                    contentViewSnapshot.RemoveFromSuperview();
                    contentViewSnapshot = null;
                });
            }
        }

		// - (void)showMenuAnimated:(BOOL)animated completion:(void(^)(BOOL finished))completion;
		public void ShowMenuAnimated (bool animated, UICompletionHandler completion)
		{
			var duration = animated ? ANIMATION_DURATION : 0;

			UIView contentView = contentViewController.View;
			RectangleF contentViewFrame = contentView.Frame;
			contentViewFrame.X = OffsetXWhenMenuIsOpen();

			LoadMenuViewControllerViewIfNeeded();
			contentViewController.BeginAppearanceTransition(false, true);
			menuViewController.BeginAppearanceTransition(true, true);

			ApplySnapshot();

			UIView.AnimateNotify(duration, 0, UIViewAnimationOptions.CurveEaseInOut, () => {
				ScaleContentView();

				contentViewFrame.Height = UIScreen.MainScreen.Bounds.Height * SCALE;
				contentViewFrame.Y = (UIScreen.MainScreen.Bounds.Height - contentViewFrame.Height) / 2;
				contentView.Frame = contentViewFrame;

			}, (finished) => {

				//TapGesture.Enabled = true;
				PanGesture.Enabled = true;

				if (completion != null) {
					completion (finished);
				}

				menuViewController.EndAppearanceTransition ();
				contentViewController.EndAppearanceTransition();
				((BGRollViewController)((BGMainNavigationController)ContentViewController).ViewControllers[0]).NaturalScrollInProgress = true;
			});
		}
		public void ShowRightMenuAnimated (bool animated, UICompletionHandler completion)
		{
			var duration = animated ? ANIMATION_DURATION : 0;

			UIView contentView = contentViewController.View;
			RectangleF contentViewFrame = contentView.Frame;
			contentViewFrame.X = OffsetXWhenRightMenuIsOpen();

			LoadRightMenuViewControllerViewIfNeeded();
			contentViewController.BeginAppearanceTransition(false, true);
			rightMenuViewController.BeginAppearanceTransition(true, true);

			ApplySnapshot();

			UIView.AnimateNotify(duration, 0, UIViewAnimationOptions.CurveEaseInOut, () => {
				ScaleContentView();

				contentViewFrame.Height = UIScreen.MainScreen.Bounds.Height * SCALE;
				contentViewFrame.Y = (UIScreen.MainScreen.Bounds.Height - contentViewFrame.Height) / 2;
				contentView.Frame = contentViewFrame;

			}, (finished) => {

				//TapGesture.Enabled = true;
				PanGesture.Enabled = true;

				if (completion != null) {
					completion (finished);
				}

				rightMenuViewController.EndAppearanceTransition ();
				contentViewController.EndAppearanceTransition();
				((BGRollViewController)((BGMainNavigationController)ContentViewController).ViewControllers[0]).NaturalScrollInProgress = true;
			});
		}


		// #pragma mark - Gestures




		UITapGestureRecognizer _tapGesture;

		public UITapGestureRecognizer TapGesture {
			get {
				if (_tapGesture == null) {
					_tapGesture = new UITapGestureRecognizer(TapGestureTriggered);
				}
				return _tapGesture;
			}
		}

		void TapGestureTriggered()
		{
			if (TapGesture.State == UIGestureRecognizerState.Ended) {
				if (IsMenuOpen())
				{
					ShowContentViewControllerAnimated (true, null);
				}
			}
		}

		UIPanGestureRecognizer _panGesture;

		public UIPanGestureRecognizer PanGesture {
			get {
				if (_panGesture == null) {
					_panGesture = new UIPanGestureRecognizer(PanGestureTriggered);
					_panGesture.Delegate = new PanGestureRecognizerDelegate ();
					//_panGesture.RequireGestureRecognizerToFail(TapGesture);
				}
				return _panGesture;
			}
		}


        Stack<SlideMenuGesturesState> undoStack = new Stack<SlideMenuGesturesState>();

        public void SetGesturesState(bool enabled)
        {
            undoStack.Push(new SlideMenuGesturesState(PanGesture.Enabled, TapGesture.Enabled));
			PanGesture.Enabled = enabled;// TapGesture.Enabled = enabled;
        }


        public void UndoGesturesStateChange()
        {
            if (undoStack.Count > 0)
            {
                var stateObj = undoStack.Pop();
                PanGesture.Enabled = stateObj.PanGestureEnabled;
				//TapGesture.Enabled = stateObj.TapGestureEnabled;
            }
            else
            {
            }
        }


		void PanGestureTriggered ()
		{
			if (PanGesture.State == UIGestureRecognizerState.Began) {
				contentViewControllerFrame = contentViewController.View.Frame;
				menuWasOpenAtPanBegin = IsMenuOpen ();

				if (!menuWasOpenAtPanBegin) {
					LoadMenuViewControllerViewIfNeeded (); // Menu is closed, load it if needed
					contentViewController.View.EndEditing(true); // Dismiss any open keyboards.
					menuViewController.BeginAppearanceTransition(true, true); // Menu is appearing
				}

				if (!IsRightMenuOpen()) {
					if (BlahguaAPIObject.Current.CurrentUser != null) {
						LoadRightMenuViewControllerViewIfNeeded (); //Right Menu is closed, load it if needed
						contentViewController.View.EndEditing (true); // Dismiss any open keyboards.
						rightMenuViewController.BeginAppearanceTransition (true, true); //Right Menu is appearing
					}
				}

			}

			PointF translation = PanGesture.TranslationInView (PanGesture.View);

			RectangleF frame = contentViewControllerFrame;
			frame.X += translation.X;

			float offsetXWhenMenuIsOpen = OffsetXWhenMenuIsOpen ();

			float offsetXWhenRightMenuIsOpen = OffsetXWhenRightMenuIsOpen ();

			/*
            float offsetXWhenMenuIsClose = OffsetXWhenMenuIsClose ();
			if (frame.X <= offsetXWhenRightMenuIsOpen)
				frame.X = offsetXWhenRightMenuIsOpen;
			else if (frame.X > offsetXWhenMenuIsOpen) {
				frame.X = offsetXWhenMenuIsOpen;
			}
*/
			currentScale = 1.0f-(1.0f-SCALE)*(frame.X/offsetXWhenMenuIsOpen);
			//contentViewController.View.Transform = CGAffineTransform.MakeScale(currentScale, currentScale);
			frame.Height = UIScreen.MainScreen.Bounds.Height * currentScale;
			frame.Y = (UIScreen.MainScreen.Bounds.Height - frame.Height) / 2;

			//contentViewController.View.Frame = frame;


			if (PanGesture.State == UIGestureRecognizerState.Ended) {
				PointF velocity = PanGesture.VelocityInView(PanGesture.View);
				float distance = 0;
				double animationDuration = 0;

				if (velocity.X < 0) // close
				{
					// Compute animation duration
					distance = Math.Abs( frame.X);
					if (distance < 150 && !IsMenuOpen() && !IsRightMenuOpen())
						return;
					animationDuration = Math.Abs(distance / velocity.X);

					if (animationDuration > ANIMATION_DURATION) {
						animationDuration = ANIMATION_DURATION;
					}

					// Remove gestures
					//TapGesture.Enabled = false;
					PanGesture.Enabled = panEnabledWhenSlideMenuIsHidden;

					//frame.X = OffsetXWhenMenuIsClose();

					if (!menuWasOpenAtPanBegin) {
						menuViewController.EndAppearanceTransition();
					}
					if (!IsRightMenuOpen()) {
						//rightMenuViewController.BeginAppearanceTransition(true,true);
					}

					if (menuWasOpenAtPanBegin) {
						menuViewController.BeginAppearanceTransition(false, true);
						contentViewController.BeginAppearanceTransition(true, true);

						UIView.AnimateNotify(animationDuration, 0, UIViewAnimationOptions.CurveEaseInOut, () => {
							DeScaleContentView();
							contentViewController.View.Frame = View.Bounds;
						}, (finished) => {
							RemoveSnapshot();
							menuViewController.EndAppearanceTransition();
							contentViewController.EndAppearanceTransition();
							if (finished) {
								contentViewController.View.LayoutIfNeeded();
							}

							((BGRollViewController)((BGMainNavigationController)ContentViewController).ViewControllers[0]).NaturalScrollInProgress = false;
						});

					}

					else if(!IsRightMenuOpen() && BlahguaAPIObject.Current.CurrentUser != null)
					{
						menuViewController.BeginAppearanceTransition(false, true);
						rightMenuViewController.BeginAppearanceTransition (true, true);

						UIView.AnimateNotify(animationDuration, 0, UIViewAnimationOptions.CurveEaseInOut, () => {
							DeScaleContentView();
							contentViewController.View.Frame =new RectangleF( View.Bounds.X - WIDTH_OF_CONTENT_VIEW_VISIBLE, View.Bounds.Y, View.Bounds.Width, View.Bounds.Height) ;

						}, (finished) => {
							RemoveSnapshot();
							menuViewController.EndAppearanceTransition();
							rightMenuViewController.EndAppearanceTransition();
							contentViewController.EndAppearanceTransition();

							((BGRollViewController)((BGMainNavigationController)ContentViewController).ViewControllers[0]).NaturalScrollInProgress = true;
						});
					} 



				} 
				else // open
				{

					//distance = Math.Abs(offsetXWhenMenuIsOpen - frame.X);
					distance = Math.Abs (frame.X);
					if (distance < 150 && !IsMenuOpen() && !IsRightMenuOpen()) // threshold for swiping
						return;
					animationDuration = Math.Abs(distance / velocity.X);
					if (animationDuration > ANIMATION_DURATION){
						animationDuration = ANIMATION_DURATION;
					}
					if (IsRightMenuOpen ())
						frame.X = OffsetXWhenMenuIsClose ();
					else
						frame.X = OffsetXWhenMenuIsOpen();
					UIView.AnimateNotify(animationDuration, 0, UIViewAnimationOptions.CurveEaseInOut, () => {
						ScaleContentView();
						frame.Height = UIScreen.MainScreen.Bounds.Height * SCALE;
						frame.Y = (UIScreen.MainScreen.Bounds.Height - frame.Height) / 2;
						contentViewController.View.Frame = frame;
						ScaleContentView();
					}, (finished) => {
						//TapGesture.Enabled = true;
						if (!menuWasOpenAtPanBegin){
							menuViewController.EndAppearanceTransition();
						}

						if(frame.X == OffsetXWhenMenuIsClose())
							((BGRollViewController)((BGMainNavigationController)ContentViewController).ViewControllers[0]).NaturalScrollInProgress = false;
						else
							((BGRollViewController)((BGMainNavigationController)ContentViewController).ViewControllers[0]).NaturalScrollInProgress = true;
					});
				}

				contentViewControllerFrame = frame;
			}
			/*
			if(ContentViewController is BGMainNavigationController && 
				((BGMainNavigationController)ContentViewController).ViewControllers[0] is BGRollViewController)
			{
				if(((BGRollViewController)((BGMainNavigationController)ContentViewController).ViewControllers[0]).RightMenuPanRecognizer != null)
					if(IsMenuOpen())
						((BGRollViewController)((BGMainNavigationController)ContentViewController).ViewControllers[0]).RightMenuPanRecognizer.Enabled = false;
					else
						((BGRollViewController)((BGMainNavigationController)ContentViewController).ViewControllers[0]).RightMenuPanRecognizer.Enabled = true;
			}
			*/
		}

		// - (BOOL)isMenuOpen;
		public bool IsMenuOpen ()
		{
			return contentViewController.View.Frame.X > 0;
		}
		public bool IsRightMenuOpen ()
		{
			return contentViewController.View.Frame.X < 0;
		}

        public float ScaleCorrection() {
			return (contentViewController.View.Bounds.Width * (1.0f - currentScale)) / 2;
        }

		public void ScaleContentView() {
			if (contentViewScaled)
            {
                return;
            }
			contentViewController.View.Transform = CGAffineTransform.MakeScale(SCALE, SCALE);
            contentViewScaled = true;
        }

        public void DeScaleContentView() {
			if (!contentViewScaled)
            {
                return;
            }
			contentViewController.View.Transform = CGAffineTransform.MakeScale(1.0f, 1.0f);
			currentScale = 1.0f;
            contentViewScaled = false;
        }

		// - (CGFloat)offsetXWhenMenuIsOpen
		public float OffsetXWhenMenuIsOpen ()
		{
			float baseOffset = WIDTH_OF_CONTENT_VIEW_VISIBLE;   //View.Bounds.Width - WIDTH_OF_CONTENT_VIEW_VISIBLE;
			baseOffset -= ScaleCorrection();
			return baseOffset;
		}

		public float OffsetXWhenRightMenuIsOpen ()
		{
			float baseOffset = - WIDTH_OF_CONTENT_VIEW_VISIBLE;
			baseOffset += ScaleCorrection();
			return baseOffset;
		}


        public float OffsetXWhenMenuIsClose () 
        {
            float baseOffset = 0.0f;
			baseOffset += contentViewScaled ? ScaleCorrection() : 0.0f;
            return baseOffset;
        }

	}
}
