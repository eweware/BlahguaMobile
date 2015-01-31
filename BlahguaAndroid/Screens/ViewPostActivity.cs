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
using Android.Graphics;

namespace BlahguaMobile.AndroidClient
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait, UiOptions = Android.Content.PM.UiOptions.SplitActionBarWhenNarrow)]
    public class ViewPostActivity : Activity
    {
        private Button btn_right;

        private ViewPostCommentsFragment commentsFragment;
        private ViewPostSummaryFragment summaryFragment;
        private ViewPostStatsFragment statsFragment;

        private Button btn_promote, btn_demote, btn_login;
        private Fragment curFragment = null;


        private GestureDetector _gestureDetector;
        private GestureListener _gestureListener;

        private ActionBar.Tab summaryTab, commentTab, statsTab;

		protected override void OnCreate (Bundle bundle)
		{
            ActionBar.NavigationMode = ActionBarNavigationMode.Tabs;
            ActionBar.SetDisplayShowTitleEnabled(false);
            ActionBar.SetDisplayShowHomeEnabled(false);
            base.OnCreate(bundle);

            //RequestWindowFeature(WindowFeatures.NoTitle);
			SetContentView (Resource.Layout.activity_viewpost);

            _gestureListener = new GestureListener();
            _gestureDetector = new GestureDetector(this, _gestureListener);
            _gestureListener.SwipeLeftEvent += swipeLeftEvent;
            _gestureListener.SwipeRightEvent += swipeRightEvent;

          
            btn_right = FindViewById<Button>(Resource.Id.btn_right);
			btn_right.SetTypeface(HomeActivity.merriweatherFont, TypefaceStyle.Normal);
            btn_right.Click += btn_right_Click;
            btn_right.Visibility = ViewStates.Gone;

            btn_login = FindViewById<Button>(Resource.Id.btn_login);
			btn_login.SetTypeface(HomeActivity.merriweatherFont, TypefaceStyle.Normal);
            btn_login.Click += delegate
            {
                StartActivity(typeof(LoginActivity));
            };
            btn_promote = FindViewById<Button>(Resource.Id.btn_promote);
            btn_demote = FindViewById<Button>(Resource.Id.btn_demote);

            btn_promote.Click += HandlePromoteBlah;
            btn_demote.Click += HandleDemoteBlah;

            btn_summary_Click(null, null);
			HomeActivity.analytics.PostPageView("/blah");

            // set up tabs
            summaryTab = ActionBar.NewTab();
            summaryTab.SetIcon(Resource.Drawable.btn_summary);
            summaryTab.TabSelected += btn_summary_Click;
            ActionBar.AddTab(summaryTab);

            commentTab = ActionBar.NewTab();
            commentTab.SetIcon(Resource.Drawable.btn_comments);
            commentTab.TabSelected += btn_comments_Click;
            ActionBar.AddTab(commentTab);

            statsTab = ActionBar.NewTab();
            statsTab.SetIcon(Resource.Drawable.btn_stats);
            statsTab.TabSelected += btn_stats_Click;
            ActionBar.AddTab(statsTab);
            this.ActionBar.SetStackedBackgroundDrawable(new Android.Graphics.Drawables.ColorDrawable(Resources.GetColor(Resource.Color.heard_black)));


        }

        private void swipeLeftEvent(MotionEvent first, MotionEvent second)
        {
            if (curFragment == summaryFragment)
                ActionBar.SelectTab(commentTab);
            else if (curFragment == commentsFragment)
                ActionBar.SelectTab(statsTab);
           
        }
        private void swipeRightEvent(MotionEvent first, MotionEvent second)
        {
            if (curFragment == statsFragment)
                ActionBar.SelectTab(commentTab);
            else if (curFragment == commentsFragment)
                ActionBar.SelectTab(summaryTab);
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
            if (statsFragment == null)
                statsFragment = ViewPostStatsFragment.NewInstance();

            if (curFragment != statsFragment)
            {
                curFragment = statsFragment;
                btn_right.Visibility = ViewStates.Gone;

                var fragmentTransaction = FragmentManager.BeginTransaction();
                fragmentTransaction.Replace(Resource.Id.content_fragment, statsFragment);
                fragmentTransaction.Commit();
            }
        }

        private void btn_summary_Click(object sender, EventArgs e)
        {
            if (summaryFragment == null)
                summaryFragment = ViewPostSummaryFragment.NewInstance();

            if (curFragment != summaryFragment)
            {
                curFragment = summaryFragment;
                btn_right.Visibility = ViewStates.Gone;

                var fragmentTransaction = FragmentManager.BeginTransaction();
                fragmentTransaction.Replace(Resource.Id.content_fragment, summaryFragment);
                fragmentTransaction.Commit();
            }
        }

        private void btn_comments_Click(object sender, EventArgs e)
        {
            if (commentsFragment == null)
                commentsFragment = ViewPostCommentsFragment.NewInstance();

            if (curFragment != commentsFragment)
            {
                curFragment = commentsFragment;
                btn_right.Visibility = ViewStates.Visible;

                var fragmentTransaction = FragmentManager.BeginTransaction();
                fragmentTransaction.Replace(Resource.Id.content_fragment, commentsFragment);
                fragmentTransaction.Commit();
            }
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


