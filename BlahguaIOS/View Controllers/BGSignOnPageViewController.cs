using System;
using CoreGraphics;

using Foundation;
using UIKit;
using System.CodeDom.Compiler;


namespace BlahguaMobile.IOS
{
    partial class BGSignOnPageViewController : UIPageViewController
	{
        //UIPageControl pageControl;

        //int totalPages = 3;
        nint curPage = 0;

        private BGSignUpPage01 page01 = null;
        private BGSignUpPage02 page02 = null;
        private BGSignUpPage03 page03 = null;


        //private RectangleF m_Frame;

       
		public BGSignOnPageViewController (IntPtr handle) : base (handle)
		{
		}

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            this.DataSource = null;// new MyDataSource();

			nint lastPage = NSUserDefaults.StandardUserDefaults.IntForKey("signupStage");
			if (lastPage > 0)
				lastPage--;

            GoToPage(lastPage, UIPageViewControllerNavigationDirection.Forward, false);



        }

        public nint CurrentPage
        {
            get { return curPage; }
        }



        private UIViewController ControllerForIndex(nint index)
        {
            UIViewController newController = null;

            switch (index)
            {
                case 0:
                    if (page01 == null)
                        page01 = (BGSignUpPage01)this.Storyboard.InstantiateViewController("SignUpPage01");
                    newController = page01;
                    break;
                case 1:
                    if (page02 == null)
                        page02 = (BGSignUpPage02)this.Storyboard.InstantiateViewController("SignUpPage02");
                    newController = page02;
                    break;
                case 2:
                    if (page03 == null)
                        page03 = (BGSignUpPage03)this.Storyboard.InstantiateViewController("SignUpPage03");
                    newController = page03;
                    break;

            }

            return newController;
        }
           
        public UIViewController PrevPage()
        {
            return ControllerForIndex(CurrentPage - 1);
        }

        public UIViewController NextPage()
        {
            return ControllerForIndex(CurrentPage + 1);
        }

        public void GoToPage(nint whichPage, UIPageViewControllerNavigationDirection dir, bool animate)
        {
            UIViewController newPage = ControllerForIndex(whichPage);
            SetViewControllers(new UIViewController[] { newPage }, dir, animate, null);
            curPage = whichPage;
        }

        public void GoToPrev()
        {
            GoToPage(CurrentPage - 1, UIPageViewControllerNavigationDirection.Reverse, true);
        }

        public void GoToNext()
        {
            GoToPage(CurrentPage + 1, UIPageViewControllerNavigationDirection.Forward, true);
        }

        public void Finish()
        {
            // at this point we should be signed in and ready to go, in the correct channel, etc.
            AppDelegate appDelegate = (AppDelegate)UIApplication.SharedApplication.Delegate;

            var c =appDelegate.MainStoryboard.InstantiateViewController ("BGMainNavigationController");
            if (appDelegate.SlideMenu.ContentViewController != null && c.GetType() == appDelegate.SlideMenu.ContentViewController.GetType())
            {
                appDelegate.SlideMenu.ShowContentViewControllerAnimated(true, null, false);
            } 
            else
            {
                appDelegate.SlideMenu.SetContentViewControllerAnimated(c as UIViewController, true);
                if (appDelegate.Window.RootViewController != appDelegate.SlideMenu) {
                    UIView.Transition(appDelegate.Window.RootViewController.View, appDelegate.SlideMenu.View, 0.5, UIViewAnimationOptions.TransitionFlipFromRight, delegate {
                        appDelegate.Window.RootViewController = appDelegate.SlideMenu.NavigationController;
                    }); 
                }
            }

            this.DismissViewController(true, null);
			NSUserDefaults.StandardUserDefaults.SetBool(true,"isSecond");
			NSUserDefaults.StandardUserDefaults.Synchronize();
        }

    }

	
    class MyDataSource : UIPageViewControllerDataSource
    {
        public override UIViewController GetPreviousViewController (UIPageViewController pageViewController, 
            UIViewController referenceViewController)
        {
            return ((BGSignOnPageViewController)pageViewController).PrevPage();
        }

        public override UIViewController GetNextViewController (UIPageViewController pageViewController, 
            UIViewController referenceViewController)
        {
            return ((BGSignOnPageViewController)pageViewController).NextPage();
        }
    }
  
        


}
