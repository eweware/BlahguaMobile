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

            Button btn_promote = FindViewById<Button>(Resource.Id.btn_promote);
            Button btn_demote = FindViewById<Button>(Resource.Id.btn_demote);
            Button btn_summary = FindViewById<Button>(Resource.Id.btn_summary);
            Button btn_comments = FindViewById<Button>(Resource.Id.btn_comments);
            Button btn_stats = FindViewById<Button>(Resource.Id.btn_stats);
        }

        protected override void OnStart()
        {
            base.OnStart();
        }
    }
}


