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
    [Activity]
    public class HistoryActivity : Activity
    {
        private TextView title;

        private HistoryCommentsFragment commentsFragment;
        private HistoryPostsFragment postsFragment;

		protected override void OnCreate (Bundle bundle)
		{
            base.OnCreate(bundle);

            RequestWindowFeature(WindowFeatures.NoTitle);
			SetContentView (Resource.Layout.activity_history);

            Button btn_back = FindViewById<Button>(Resource.Id.btn_back);
            btn_back.Click += delegate
            {
                Finish();
			};

            title = FindViewById<TextView>(Resource.Id.title);

            Button btn_summary = FindViewById<Button>(Resource.Id.btn_summary);
            Button btn_comments = FindViewById<Button>(Resource.Id.btn_comments);

            btn_comments.Click += btn_comments_Click;
            btn_summary.Click += btn_summary_Click;

            btn_summary_Click(null, null);
        }

        private void btn_summary_Click(object sender, EventArgs e)
        {
            commentsFragment = null;

            postsFragment = HistoryPostsFragment.NewInstance();
            var fragmentTransaction = FragmentManager.BeginTransaction();
            fragmentTransaction.Replace(Resource.Id.content_fragment, postsFragment);
            fragmentTransaction.Commit();
        }

        private void btn_comments_Click(object sender, EventArgs e)
        {
            postsFragment = null;

            commentsFragment = HistoryCommentsFragment.NewInstance();
            var fragmentTransaction = FragmentManager.BeginTransaction();
            fragmentTransaction.Replace(Resource.Id.content_fragment, commentsFragment);
            fragmentTransaction.Commit();
        }
    }
}


