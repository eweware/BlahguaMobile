using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System;
using System.CodeDom.Compiler;
using System.Drawing;

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
			
            //EmbeddedWebView.Frame = new RectangleF (0, 0, UIScreen.MainScreen.Bounds.Width, UIScreen.MainScreen.Bounds.Height);
			EmbeddedWebView.LoadFinished += (object sender, EventArgs e) => {
				// update the title
				string theTitle = EmbeddedWebView.EvaluateJavascript("document.title");
				Title = theTitle;
			};
            EmbeddedWebView.ScalesPageToFit = true;

		}

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            OpenURL(UIApplicationHeard.TargetURL);
        }

        public void OpenURL(NSUrl theURL)
        {
            NSUrlRequest theRequest = new NSUrlRequest (theURL);

            EmbeddedWebView.LoadRequest (theRequest);
        }
	}
}
