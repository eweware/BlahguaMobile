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
    public class ViewPostActivity : Activity
    {
        private Button btn_right;
        private TextView title;

        private ViewPostCommentsFragment commentsFragment;
        private ViewPostSummaryFragment summaryFragment;
        private ViewPostStatsFragment statsFragment;

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

            title = FindViewById<TextView>(Resource.Id.title);
            btn_right = FindViewById<Button>(Resource.Id.btn_right);

            btn_right.Click += btn_right_Click;

            title.Visibility = ViewStates.Gone;
            btn_right.Visibility = ViewStates.Gone;

            Button btn_promote = FindViewById<Button>(Resource.Id.btn_promote);
            Button btn_demote = FindViewById<Button>(Resource.Id.btn_demote);
            Button btn_summary = FindViewById<Button>(Resource.Id.btn_summary);
            Button btn_comments = FindViewById<Button>(Resource.Id.btn_comments);
            Button btn_stats = FindViewById<Button>(Resource.Id.btn_stats);

            btn_comments.Click += btn_comments_Click;
            btn_summary.Click += btn_summary_Click;
            btn_stats.Click += btn_stats_Click;

            btn_summary_Click(null, null);
        }

        private void btn_right_Click(object sender, EventArgs e)
        {
            if (commentsFragment != null) // that means it is active
            {
                commentsFragment.triggerCreateBlock();
            }
        }

        private void btn_stats_Click(object sender, EventArgs e)
        {
            title.Visibility = ViewStates.Gone;
            btn_right.Visibility = ViewStates.Gone;

            summaryFragment = null;
            commentsFragment = null;

            statsFragment = ViewPostStatsFragment.NewInstance();
            var fragmentTransaction = FragmentManager.BeginTransaction();
            fragmentTransaction.Replace(Resource.Id.content_fragment, statsFragment);
            fragmentTransaction.Commit();
        }

        private void btn_summary_Click(object sender, EventArgs e)
        {
            title.Visibility = ViewStates.Gone;
            btn_right.Visibility = ViewStates.Gone;

            commentsFragment = null;
            statsFragment = null;

            summaryFragment = ViewPostSummaryFragment.NewInstance();
            var fragmentTransaction = FragmentManager.BeginTransaction();
            fragmentTransaction.Replace(Resource.Id.content_fragment, summaryFragment);
            fragmentTransaction.Commit();
        }

        private void btn_comments_Click(object sender, EventArgs e)
        {
            title.Visibility = ViewStates.Visible;
            btn_right.Visibility = ViewStates.Visible;
            btn_right.Text = "Write";

            summaryFragment = null;
            statsFragment = null;

            commentsFragment = ViewPostCommentsFragment.NewInstance();
            var fragmentTransaction = FragmentManager.BeginTransaction();
            fragmentTransaction.Replace(Resource.Id.content_fragment, commentsFragment);
            fragmentTransaction.Commit();
        }

        protected override void OnStart()
        {
            base.OnStart();
        }
    }
}


