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
    public class ViewPostActivity : Activity
    {
        private Button btn_right;
        private TextView title;

        private ViewPostCommentsFragment commentsFragment;
        private ViewPostSummaryFragment summaryFragment;
        private ViewPostStatsFragment statsFragment;

        private Button btn_promote, btn_demote, btn_login;
        private Button btn_summary, btn_comments, btn_stats;


        private GestureDetector _gestureDetector;
        private GestureListener _gestureListener;


		protected override void OnCreate (Bundle bundle)
		{
            base.OnCreate(bundle);

            RequestWindowFeature(WindowFeatures.NoTitle);
			SetContentView (Resource.Layout.activity_viewpost);

            _gestureListener = new GestureListener();
            _gestureDetector = new GestureDetector(this, _gestureListener);
            _gestureListener.SwipeLeftEvent += swipeLeftEvent;
            _gestureListener.SwipeRightEvent += swipeRightEvent;

            Button btn_back = FindViewById<Button>(Resource.Id.btn_back);
            btn_back.Click += delegate
            {
                Finish();
			};

            title = FindViewById<TextView>(Resource.Id.title);
            btn_right = FindViewById<Button>(Resource.Id.btn_right);
            title.SetTypeface(MainActivity.merriweatherFont, Android.Graphics.TypefaceStyle.Normal);

            btn_right.Click += btn_right_Click;

            title.Visibility = ViewStates.Gone;
            btn_right.Visibility = ViewStates.Gone;

            btn_login = FindViewById<Button>(Resource.Id.btn_login);
            btn_login.Click += delegate
            {
                StartActivity(typeof(LoginActivity));
            };
            btn_promote = FindViewById<Button>(Resource.Id.btn_promote);
            btn_demote = FindViewById<Button>(Resource.Id.btn_demote);

            btn_summary = FindViewById<Button>(Resource.Id.btn_summary);
            btn_comments = FindViewById<Button>(Resource.Id.btn_comments);
            btn_stats = FindViewById<Button>(Resource.Id.btn_stats);

            btn_promote.Click += HandlePromoteBlah;
            btn_demote.Click += HandleDemoteBlah;

            btn_comments.Click += btn_comments_Click;
            btn_summary.Click += btn_summary_Click;
            btn_stats.Click += btn_stats_Click;

            btn_summary_Click(null, null);
            MainActivity.analytics.PostPageView("/blah");
        }

        private void swipeLeftEvent(MotionEvent first, MotionEvent second)
        {
            if (summaryFragment != null)
            {
                btn_comments_Click(null, null);
            }
            else if (commentsFragment != null)
            {
                btn_stats_Click(null, null);
            }
        }
        private void swipeRightEvent(MotionEvent first, MotionEvent second)
        {
            if (commentsFragment != null)
            {
                btn_summary_Click(null, null);
            }
            else if (statsFragment != null)
            {
                btn_comments_Click(null, null);
            }
        }

        public override bool DispatchTouchEvent(MotionEvent ev)
        {
            base.DispatchTouchEvent(ev);
            return _gestureDetector.OnTouchEvent(ev);
        }

        private void initUserUi()
        {
            if (BlahguaAPIObject.Current.CurrentUser != null)
            {
                btn_promote.Visibility = ViewStates.Visible;
                btn_demote.Visibility = ViewStates.Visible;

                btn_login.Visibility = ViewStates.Gone;
            }
            else
            {
                //btn_right.Visibility = ViewStates.Gone;

                btn_promote.Visibility = ViewStates.Gone;
                btn_demote.Visibility = ViewStates.Gone;

                btn_login.Visibility = ViewStates.Visible;
            }
        }

        protected override void OnResume()
        {
            base.OnResume();

            initUserUi();
        }

        private void btn_right_Click(object sender, EventArgs e)
        {
            if (commentsFragment != null) // that means it is active
            {
                if (BlahguaAPIObject.Current.CurrentUser != null)
                {
                    commentsFragment.triggerCreateBlock();
                }
                else
                {
                    StartActivity(typeof(LoginActivity));
                }
            }
        }

        #region TabBar buttons
        private void btn_stats_Click(object sender, EventArgs e)
        {
            // disable stats button
            if (statsFragment != null)
                return;
            btn_stats.SetBackgroundResource(Resource.Drawable.btn_stats_pressed);
            btn_summary.SetBackgroundResource(Resource.Drawable.btn_summary);
            btn_comments.SetBackgroundResource(Resource.Drawable.btn_comments);

            // do the rest
            title.Text = "Statistics";
            title.Visibility = ViewStates.Visible;
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
            // disable summary button
            if (summaryFragment != null)
                return;
            btn_stats.SetBackgroundResource(Resource.Drawable.btn_stats);
            btn_summary.SetBackgroundResource(Resource.Drawable.btn_summary_pressed);
            btn_comments.SetBackgroundResource(Resource.Drawable.btn_comments);

            // do the rest
            title.Text = "Summary";
            title.Visibility = ViewStates.Visible;
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
            // disable comments button
            if (commentsFragment != null)
                return;
            btn_stats.SetBackgroundResource(Resource.Drawable.btn_stats);
            btn_summary.SetBackgroundResource(Resource.Drawable.btn_summary);
            btn_comments.SetBackgroundResource(Resource.Drawable.btn_comments_pressed);

            // do the rest
            title.Text = "Comments";
            title.Visibility = ViewStates.Visible;
            //if (BlahguaAPIObject.Current.CurrentUser != null)
            //{
                btn_right.Visibility = ViewStates.Visible;
                btn_right.Text = "Write";
            //}

            summaryFragment = null;
            statsFragment = null;

            commentsFragment = ViewPostCommentsFragment.NewInstance();
            var fragmentTransaction = FragmentManager.BeginTransaction();
            fragmentTransaction.Replace(Resource.Id.content_fragment, commentsFragment);
            fragmentTransaction.Commit();
        }
        #endregion

        #region Handles

        private void HandlePromoteBlah(object sender, EventArgs e)
        {
            //if (currentPage == "summary")
            //{
                BlahguaAPIObject.Current.SetBlahVote(1, (newVote) =>
                {
                    UpdateSummaryButtons();
                    MainActivity.analytics.PostBlahVote(1);
                });
            //}
            //else
            //{
            //    Comment curComment = (Comment)AllCommentList.SelectedItem;

            //    if (curComment != null)
            //    {
            //        BlahguaAPIObject.Current.SetCommentVote(curComment, 1, (newVote) =>
            //        {
            //            UpdateCommentButtons();
            //            App.analytics.PostCommentVote(1);
            //        }
            //        );
            //    }
            //}
        }

        private void HandleDemoteBlah(object sender, EventArgs e)
        {
            //if (currentPage == "summary")
            //{
                BlahguaAPIObject.Current.SetBlahVote(-1, (newVote) =>
                {
                    UpdateSummaryButtons();
                    MainActivity.analytics.PostBlahVote(-1);
                });
            //}
            //else
            //{
            //    Comment curComment = (Comment)AllCommentList.SelectedItem;

            //    if (curComment != null)
            //    {
            //        BlahguaAPIObject.Current.SetCommentVote(curComment, -1, (newVote) =>
            //        {
            //            UpdateCommentButtons();
            //            App.analytics.PostCommentVote(-1);
            //        }
            //        );
            //    }
            //}
        }
        #endregion

        public void UpdateSummaryButtons()
        {
            //btn_promote.IconUri = new Uri("/Images/Icons/white_promote.png", UriKind.Relative);
            //btn_demote.IconUri = new Uri("/Images/Icons/white_demote.png", UriKind.Relative);
            Blah curBlah = BlahguaAPIObject.Current.CurrentBlah;

            if (BlahguaAPIObject.Current.CurrentUser != null)
            {
				RunOnUiThread (() => {
					if (curBlah.A == BlahguaAPIObject.Current.CurrentUser._id) {
						btn_promote.Enabled = false;
						btn_demote.Enabled = false;
					} else if (curBlah.uv == 0) {
						btn_promote.Enabled = true;
						btn_demote.Enabled = true;
					} else {
						btn_promote.Enabled = false;
						btn_demote.Enabled = false;
						if (curBlah.uv == 1) {
							btn_promote.SetBackgroundResource (Resource.Drawable.btn_promote_active);
						} else {
                            btn_demote.SetBackgroundResource(Resource.Drawable.btn_demote_active);
						}
					}
				});
            }
        }
    }
}


