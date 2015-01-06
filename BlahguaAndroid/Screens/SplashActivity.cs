using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Util;
using Android.Content.PM;
using BlahguaMobile.BlahguaCore;
using System.Timers;
using System.ComponentModel;
using Android.Graphics.Drawables;
using System;
using SlidingMenuSharp.App;
using SlidingMenuSharp;
using BlahguaMobile.AndroidClient.Screens;
using Android.App;

namespace BlahguaMobile.AndroidClient
{
    [Activity(MainLauncher = true, ScreenOrientation = ScreenOrientation.Portrait)]
    public class SplashActivity : Activity
    {

		protected override void OnCreate (Bundle bundle)
		{
            base.OnCreate(bundle);

            RequestWindowFeature(WindowFeatures.NoTitle);
			SetContentView (Resource.Layout.activity_splash);

            Handler h = new Handler();
            h.PostDelayed(aaa, 3000);
        }

        void aaa()
        {
            Finish();
            //StartActivity(typeof(MainActivity));
			StartActivity(typeof(HomeActivity));
        }

        public override void OnBackPressed()
        {
            //base.OnBackPressed();
        }
    }
}


