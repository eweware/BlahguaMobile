using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System;
using System.CodeDom.Compiler;
using System.Drawing;
using MonoTouch.CoreGraphics;

namespace BlahguaMobile.IOS
{
	partial class EmbeddedWebController : UIViewController
	{
		public EmbeddedWebController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
            NSUrl	theURL = UIApplicationHeard.TargetURL;
			    
            EmbeddedWebView.ScalesPageToFit = true;

            /*
            UIDevice.CurrentDevice.BeginGeneratingDeviceOrientationNotifications();
            NSNotificationCenter.DefaultCenter.AddObserver(UIDevice.OrientationDidChangeNotification, notification =>
                {
                    float PI = (float)Math.PI;
                    // handle that
                    UIDeviceOrientation orientation = UIDevice.CurrentDevice.Orientation;

                    if (orientation == UIDeviceOrientation.LandscapeLeft)
                        this.View.Transform = CGAffineTransform.MakeRotation(PI / 2.0f);
                    else if (orientation == UIDeviceOrientation.LandscapeRight)
                        this.View.Transform = CGAffineTransform.MakeRotation(PI / -2.0f);
                    else if (orientation == UIDeviceOrientation.PortraitUpsideDown)
                        this.View.Transform = CGAffineTransform.MakeRotation(PI);
                    else if (orientation == UIDeviceOrientation.Portrait)
                        this.View.Transform = CGAffineTransform.MakeRotation(0.0f);
                });

            */  

		}

        public override void ViewWillAppear(bool animated)
        {

            base.ViewWillAppear(animated);

        }

        public override void ViewDidDisappear(bool animated)
        {
            EmbeddedWebView.LoadHtmlString("", null);
            base.ViewDidDisappear(animated);
        }
           

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            OpenURL(UIApplicationHeard.TargetURL);
        }

        public void OpenURL(NSUrl theURL)
        {
            try {
                NSUrlRequest theRequest = new NSUrlRequest (theURL);

                EmbeddedWebView.LoadRequest (theRequest);
            } catch (Exception exp) {
                Console.WriteLine("error loading page: " + exp.Message);
            }
        }
	}
}
