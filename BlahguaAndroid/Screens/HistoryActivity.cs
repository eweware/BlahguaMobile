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
using BlahguaMobile.AndroidClient.HelpingClasses;

namespace BlahguaMobile.AndroidClient
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait)]
    public class HistoryActivity : Activity
    {
        private TextView title;

        private HistoryCommentsFragment commentsFragment;
        private HistoryPostsFragment postsFragment;

        private GestureDetector _gestureDetector;
        private GestureListener _gestureListener;

        private FrameLayout contentFragment;

		protected override void OnCreate (Bundle bundle)
		{
            base.OnCreate(bundle);

            RequestWindowFeature(WindowFeatures.NoTitle);
            SetContentView(Resource.Layout.activity_history);

            _gestureListener = new GestureListener();
            _gestureDetector = new GestureDetector(this, _gestureListener);

            Button btn_back = FindViewById<Button>(Resource.Id.btn_back);
            btn_back.Click += delegate
            {
                Finish();
			};

            title = FindViewById<TextView>(Resource.Id.title);
            contentFragment = FindViewById<FrameLayout>(Resource.Id.content_fragment);

            Button btn_summary = FindViewById<Button>(Resource.Id.btn_summary);
            Button btn_comments = FindViewById<Button>(Resource.Id.btn_comments);

            btn_comments.Click += btn_comments_Click;
            btn_summary.Click += btn_summary_Click;
            btn_summary_Click(null, null);
        }

        public int GetContentPositionY()
        {
            return contentFragment.Top;
        }

        public override bool DispatchTouchEvent(MotionEvent ev)
        {
            base.DispatchTouchEvent(ev);
            return _gestureDetector.OnTouchEvent(ev);
        }

        private void btn_summary_Click(object sender, EventArgs e)
        {
            commentsFragment = null;

            postsFragment = HistoryPostsFragment.NewInstance();
            var fragmentTransaction = FragmentManager.BeginTransaction();
            fragmentTransaction.Replace(Resource.Id.content_fragment, postsFragment);
            fragmentTransaction.Commit();

            _gestureListener.SwipeLeftEvent -= postsFragment.GestureLeft;
            _gestureListener.SwipeRightEvent -= postsFragment.GestureRight;

            _gestureListener.SwipeLeftEvent += postsFragment.GestureLeft;
            _gestureListener.SwipeRightEvent += postsFragment.GestureRight;
        }

        private void btn_comments_Click(object sender, EventArgs e)
        {
            postsFragment = null;

            commentsFragment = HistoryCommentsFragment.NewInstance();
            var fragmentTransaction = FragmentManager.BeginTransaction();
            fragmentTransaction.Replace(Resource.Id.content_fragment, commentsFragment);
            fragmentTransaction.Commit();

            _gestureListener.SwipeLeftEvent -= commentsFragment.GestureLeft;
            _gestureListener.SwipeRightEvent -= commentsFragment.GestureRight;

            _gestureListener.SwipeLeftEvent += commentsFragment.GestureLeft;
            _gestureListener.SwipeRightEvent += commentsFragment.GestureRight;
        }
    }
}


