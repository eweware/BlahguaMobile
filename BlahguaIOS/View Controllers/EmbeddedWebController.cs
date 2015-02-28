using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System;
using System.CodeDom.Compiler;

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
			string urlString = "http://www.sheep.com";
			NSUrl	theURL = new NSUrl (urlString);
			NSUrlRequest theRequest = new NSUrlRequest (theURL);
			EmbeddedWebView.LoadFinished += (object sender, EventArgs e) => {
				// update the title
				string theTitle = EmbeddedWebView.EvaluateJavascript("document.title");
				WebNavBar.TopItem.Title = theTitle;
			};

			EmbeddedWebView.LoadRequest (theRequest);

		}
	}
}
