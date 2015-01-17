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
//using Android.App;
using Android.Support.V4.App;
using BlahguaMobile.AndroidClient.HelpingClasses;
using Android.Graphics;

namespace BlahguaMobile.AndroidClient
{
    //[Activity(ScreenOrientation = ScreenOrientation.Portrait)]
	public class ViewPostFragment : Fragment
    {
        private Button btn_right;

        private ViewPostCommentsFragment commentsFragment;
        private ViewPostSummaryFragment summaryFragment, summaryInitFragment;
        private ViewPostStatsFragment statsFragment;

        private Button btn_promote, btn_demote, btn_login;
        private Button btn_summary, btn_comments, btn_stats;


        private GestureDetector _gestureDetector;
        private GestureListener _gestureListener;

		HomeActivity homeActivity = null;



		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView (inflater, container, savedInstanceState);

			View fragment = inflater.Inflate (Resource.Layout.activity_viewpost, null);

			homeActivity = (HomeActivity)this.Activity;

            _gestureListener = new GestureListener();
            //_gestureDetector = new GestureDetector(this, _gestureListener);
            _gestureListener.SwipeLeftEvent += swipeLeftEvent;
            _gestureListener.SwipeRightEvent += swipeRightEvent;

			Button btn_back = fragment.FindViewById<Button>(Resource.Id.btn_back);
            btn_back.Click += delegate
            {
				if(FragmentManager.BackStackEntryCount >0)
				{
					FragmentManager.PopBackStack();
					homeActivity.RestoreTitle();
				}
			};

			btn_right = fragment.FindViewById<Button>(Resource.Id.btn_right);
            btn_right.SetTypeface(MainActivity.merriweatherFont, TypefaceStyle.Normal);

            btn_right.Click += btn_right_Click;

            btn_right.Visibility = ViewStates.Gone;

			btn_login = fragment.FindViewById<Button>(Resource.Id.btn_login);
            btn_login.SetTypeface(MainActivity.merriweatherFont, TypefaceStyle.Normal);
            btn_login.Click += delegate
            {
                //StartActivity(typeof(LoginActivity));
				FragmentTransaction fragmentTx = this.FragmentManager.BeginTransaction ();
				LoginFragment loginFragment = new LoginFragment ();

				fragmentTx.Replace (Resource.Id.content_frame, loginFragment);
				fragmentTx.AddToBackStack (null);
				fragmentTx.Commit ();
            };
			btn_promote = fragment.FindViewById<Button>(Resource.Id.btn_promote);
			btn_demote = fragment.FindViewById<Button>(Resource.Id.btn_demote);

			btn_summary = fragment.FindViewById<Button>(Resource.Id.btn_summary);
			btn_comments = fragment.FindViewById<Button>(Resource.Id.btn_comments);
			btn_stats = fragment.FindViewById<Button>(Resource.Id.btn_stats);

            btn_promote.Click += HandlePromoteBlah;
            btn_demote.Click += HandleDemoteBlah;

            btn_comments.Click += btn_comments_Click;
            btn_summary.Click += btn_summary_Click;
            btn_stats.Click += btn_stats_Click;

            btn_summary_Click(null, null);
			HomeActivity.analytics.PostPageView("/blah");


			return fragment;
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
		/*
        public override bool DispatchTouchEvent(MotionEvent ev)
        {
            base.DispatchTouchEvent(ev);
            return _gestureDetector.OnTouchEvent(ev);
        }
		*/
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

		public override void OnResume()
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
                    //StartActivity(typeof(LoginActivity));
					FragmentTransaction fragmentTx = this.FragmentManager.BeginTransaction ();
					LoginFragment loginFragment = new LoginFragment ();

					fragmentTx.Replace (Resource.Id.content_frame, loginFragment);
					fragmentTx.AddToBackStack (null);
					fragmentTx.Commit ();
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
			homeActivity.SetTitle ("Statistics");
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

			homeActivity.SetTitle ("Summary");

            btn_right.Visibility = ViewStates.Gone;

            commentsFragment = null;
            statsFragment = null;

            if (summaryInitFragment == null)
            {
                summaryInitFragment = ViewPostSummaryFragment.NewInstance();
            }
            summaryFragment = summaryInitFragment;
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
			homeActivity.SetTitle ("Comments");
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
            BlahguaAPIObject.Current.SetBlahVote(1, (newVote) =>
            {
                UpdateSummaryButtons();
					HomeActivity.analytics.PostBlahVote(1);
            });
            
        }

        private void HandleDemoteBlah(object sender, EventArgs e)
        {
            BlahguaAPIObject.Current.SetBlahVote(-1, (newVote) =>
            {
                UpdateSummaryButtons();
					HomeActivity.analytics.PostBlahVote(-1);
            });
            
        }
        #endregion

        public void UpdateSummaryButtons()
        {
            //btn_promote.IconUri = new Uri("/Images/Icons/white_promote.png", UriKind.Relative);
            //btn_demote.IconUri = new Uri("/Images/Icons/white_demote.png", UriKind.Relative);
            Blah curBlah = BlahguaAPIObject.Current.CurrentBlah;

            if (BlahguaAPIObject.Current.CurrentUser != null)
            {
				Activity.RunOnUiThread (() => {
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


