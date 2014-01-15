using System;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace BlahguaMobile.IOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : UIApplicationDelegate
    {
        UIWindow window;

        

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            window = new UIWindow(UIScreen.MainScreen.Bounds);
			var rootNavigationController = new UINavigationController ();
			BlahRoll blahRoll = new BlahRoll ();
			rootNavigationController.PushViewController (blahRoll, false);
			this.window.RootViewController = rootNavigationController;
			this.window.MakeKeyAndVisible ();

            return true;
        }
    }
}

