﻿using System;
using System.Collections.Generic;
using System.Linq;
using CoreGraphics;

using Foundation;
using UIKit;

namespace BlahguaMobile.IOS
{
	[Register ("UIApplicationHeard")]
	public class UIApplicationHeard : UIApplication
	{
		private EmbeddedWebController m_webViewController = null;
        public static NSUrl TargetURL;

		public UIApplicationHeard () : base()
		{
		}

		public override bool OpenUrl (NSUrl url)
		{
            if (true)
            { //todo:  fix web links
                TargetURL = url;
                ShowWebView();
           
                return true;
            }
            else
                return base.OpenUrl(url);
		}

		public override void SendEvent (UIEvent uievent)
		{
			base.SendEvent (uievent);
		}

		public void ShowWebView()
		{
			if (m_webViewController == null) {

				UIStoryboard webStoryBoard = UIStoryboard.FromName("WebPageStoryBoard", null);

				m_webViewController = (EmbeddedWebController)webStoryBoard.InstantiateViewController ("EmbeddedWebViewController");

				m_webViewController.View.Frame = new CGRect (0, 0, UIScreen.MainScreen.Bounds.Width, UIScreen.MainScreen.Bounds.Height);
			}

			AppDelegate.slideMenu.NavigationController.PushViewController (m_webViewController, true);

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
            catch (Exception exp)
            {
				exp.Message.ToString ();
                
            }
        }
    }
}