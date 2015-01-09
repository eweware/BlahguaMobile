using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.CodeDom.Compiler;

namespace BlahguaMobile.IOS
{
	partial class BGSignUpPage02 : UIViewController
	{
		public BGSignUpPage02 (IntPtr handle) : base (handle)
		{
		}

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            this.techBtn.TouchUpInside += (object sender, EventArgs e) => 
                {
                    // we are done - dismiss it
                    ((BGSignOnPageViewController)ParentViewController).GoToNext();
                };

            this.entertainmentBtn.TouchUpInside += (object sender, EventArgs e) => 
                {
                    // we are done - dismiss it
                    ((BGSignOnPageViewController)ParentViewController).GoToNext();
                };


            this.publicBtn.TouchUpInside += (object sender, EventArgs e) => 
                {
                    // we are done - dismiss it
                    ((BGSignOnPageViewController)ParentViewController).Finish();
                };


        }
	}
}
