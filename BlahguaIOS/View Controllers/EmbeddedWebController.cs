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
		private NSUrlRequest curRequest = null;

		public EmbeddedWebController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			    
            EmbeddedWebView.ScalesPageToFit = true;

			EmbeddedWebView.LoadFinished += (object sender, EventArgs e) => {
				// placeholder

			};

			EmbeddedWebView.ShouldStartLoad += HandleShouldStartLoad;

			EmbeddedWebView.LoadHtmlString("", null);
		}

		bool HandleShouldStartLoad (UIWebView webView, NSUrlRequest request, UIWebViewNavigationType navigationType)
		{
			if (request.Url.Scheme.Contains ("http"))// != scheme.Replace (":", ""))
				return true;
			else
				return false;

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


			InvokeOnMainThread (() => {
				OpenURL (UIApplicationHeard.TargetURL);
			});
        }

        public void OpenURL(NSUrl theURL)
        {
            try {
				string URLString = theURL.ToString();

				curRequest = new NSUrlRequest(new NSUrl(URLString));

				EmbeddedWebView.LoadRequest (curRequest);
            } catch (Exception exp) {
                Console.WriteLine("error loading page: " + exp.Message);
            }
        }
	}
}
