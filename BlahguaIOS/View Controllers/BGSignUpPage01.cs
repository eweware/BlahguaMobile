using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.CodeDom.Compiler;

namespace BlahguaMobile.IOS
{
	partial class BGSignUpPage01 : UIViewController
	{
		public BGSignUpPage01 (IntPtr handle) : base (handle)
		{


		}

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            signInBtn.TouchUpInside += (object sender, EventArgs e) => 
                {

                    ((BGSignOnPageViewController)ParentViewController).GoToNext();
                        

                };

            createAccountBtn.TouchUpInside += (object sender, EventArgs e) => 
                {

                    ((BGSignOnPageViewController)ParentViewController).GoToNext();


                };


            skipButton.TouchUpInside += (object sender, EventArgs e) => 
                {

                    ((BGSignOnPageViewController)ParentViewController).Finish();


                };

        }
	}
}
