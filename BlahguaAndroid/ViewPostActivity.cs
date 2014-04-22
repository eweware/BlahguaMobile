using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Util;
using Android.Content.PM;
using Android.Support.V4.Widget;
using BlahguaMobile.BlahguaCore;
using System.Timers;
using System.ComponentModel;
using Android.Support.V4.App;
using Android.Graphics.Drawables;
using System;
using BlahguaMobile.AndroidClient.ThirdParty.UrlImageViewHelper;
using SlidingMenuSharp.App;
using SlidingMenuSharp;

namespace BlahguaMobile.AndroidClient
{
    [Activity]
    public class ViewPostActivity : FragmentActivity
    {

		protected override void OnCreate (Bundle bundle)
		{
            base.OnCreate(bundle);

            RequestWindowFeature(WindowFeatures.NoTitle);
			SetContentView (Resource.Layout.activity_viewpost);

            Button btn_back = FindViewById<Button>(Resource.Id.btn_back);
            btn_back.Click += delegate
            {
                Finish();
			};
        }


        protected override void OnStart()
        {
            base.OnStart();
        }
    }
}


