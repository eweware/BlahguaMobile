using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace BlahguaMobile.IOS
{
	[Register ("UIApplicationHeard")]
	public class UIApplicationHeard : UIApplication
	{
		private EmbeddedWebController m_webViewController = null;

		public UIApplicationHeard () : base()
		{
		}

		public override bool OpenUrl (NSUrl url)
		{
			//return base.OpenUrl (url);
			ShowWebView (url);
			return true;
		}

		public override void SendEvent (UIEvent uievent)
		{
			base.SendEvent (uievent);
		}

		public void ShowWebView(NSUrl theURL)
		{
			UIWindow theWindow = UIApplication.SharedApplication.KeyWindow;

			if (m_webViewController == null) {

				UIStoryboard webStoryBoard = UIStoryboard.FromName("WebPageStoryBoard", null);

				m_webViewController = (EmbeddedWebController)webStoryBoard.InstantiateViewController ("EmbeddedWebViewController");
				//m_webViewController.ParentViewController = theWindow.RootViewController;
				//theWindow.RootViewController.AddChildViewController (m_webViewController);

				m_webViewController.View.Frame = new RectangleF (0, 0, UIScreen.MainScreen.Bounds.Width, UIScreen.MainScreen.Bounds.Height);
			}

			theWindow.RootViewController.NavigationController.PushViewController (m_webViewController, true);

		}
	}

    public class Application
    {



        // This is the main entry point of the application.
        static void Main(string[] args)
        {
            // if you want to use a different Application Delegate class from "AppDelegate"
            // you can specify it here.

            try
            {
				UIApplication.Main (args, "UIApplicationHeard", "AppDelegate");

            }
            catch (Exception e)
            {
				e.Message.ToString ();
                
            }
        }
    }
}