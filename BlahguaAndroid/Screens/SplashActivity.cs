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
using Android.Graphics;

namespace BlahguaMobile.AndroidClient
{
    [Activity(MainLauncher = true, ScreenOrientation = ScreenOrientation.Portrait)]
    public class SplashActivity : Activity
    {

		protected override void OnCreate (Bundle bundle)
		{
            int delayTime;
            base.OnCreate(bundle);

            RequestWindowFeature(WindowFeatures.NoTitle);
			
            ISharedPreferences _sharedPref = Android.Preferences.PreferenceManager.GetDefaultSharedPreferences(this);
            String seenIt = _sharedPref.GetString("firsttime", "");
            if (String.IsNullOrEmpty(seenIt))
            {
                delayTime = 3000;
                SetContentView (Resource.Layout.activity_splash);

                _sharedPref.Edit().PutString("firsttime", "true").Commit();

            }
            else
                delayTime = 1;

            Handler h = new Handler();
            h.PostDelayed(aaa, delayTime);

        }

        void aaa()
        {
            Finish();
            StartActivity(typeof(MainActivity));
        }

        public override void OnBackPressed()
        {
            //base.OnBackPressed();
        }
    }


}


