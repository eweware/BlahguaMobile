using Foundation;
using UIKit;
using System;
using System.CodeDom.Compiler;
using CoreGraphics;

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

            ClearPage();
			EmbeddedWebView.ShouldStartLoad += HandleShouldStartLoad;

			
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

        private void ClearPage()
        {
            EmbeddedWebView.LoadHtmlString("", null);
        }

        public override void ViewDidDisappear(bool animated)
        {

            ClearPage();
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
