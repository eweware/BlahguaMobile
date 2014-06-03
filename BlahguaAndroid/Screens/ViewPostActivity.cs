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

        private Button btn_promote, btn_demote, btn_login;

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

            btn_login = FindViewById<Button>(Resource.Id.btn_login);
            btn_login.Click += delegate
            {
                StartActivity(typeof(LoginActivity));
            };
            btn_promote = FindViewById<Button>(Resource.Id.btn_promote);
            btn_demote = FindViewById<Button>(Resource.Id.btn_demote);
            Button btn_summary = FindViewById<Button>(Resource.Id.btn_summary);
            Button btn_comments = FindViewById<Button>(Resource.Id.btn_comments);
            Button btn_stats = FindViewById<Button>(Resource.Id.btn_stats);

            btn_promote.Click += HandlePromoteBlah;
            btn_demote.Click += HandleDemoteBlah;

            btn_comments.Click += btn_comments_Click;
            btn_summary.Click += btn_summary_Click;
            btn_stats.Click += btn_stats_Click;

            btn_summary_Click(null, null);
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
                btn_right.Visibility = ViewStates.Gone;

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
            if (BlahguaAPIObject.Current.CurrentUser != null)
            {
                btn_right.Visibility = ViewStates.Visible;
                btn_right.Text = "Write";
            }

            summaryFragment = null;
            statsFragment = null;

            commentsFragment = ViewPostCommentsFragment.NewInstance();
            var fragmentTransaction = FragmentManager.BeginTransaction();
            fragmentTransaction.Replace(Resource.Id.content_fragment, commentsFragment);
            fragmentTransaction.Commit();
        }


        #region Handles

        private void HandlePromoteBlah(object sender, EventArgs e)
        {
            //if (currentPage == "summary")
            //{
                BlahguaAPIObject.Current.SetBlahVote(1, (newVote) =>
                {
                    UpdateSummaryButtons();
                    //App.analytics.PostBlahVote(1);
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
                    //App.analytics.PostBlahVote(-1);
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

        void UpdateSummaryButtons()
        {
            //btn_promote.IconUri = new Uri("/Images/Icons/white_promote.png", UriKind.Relative);
            //btn_demote.IconUri = new Uri("/Images/Icons/white_demote.png", UriKind.Relative);
            Blah curBlah = BlahguaAPIObject.Current.CurrentBlah;

            if (BlahguaAPIObject.Current.CurrentUser != null)
            {
                if (curBlah.A == BlahguaAPIObject.Current.CurrentUser._id)
                {
                    btn_promote.Enabled = false;
                    btn_demote.Enabled = false;
                }
                else if (curBlah.uv == 0)
                {
                    btn_promote.Enabled = true;
                    btn_demote.Enabled = true;
                }
                else
                {
                    btn_promote.Enabled = false;
                    btn_demote.Enabled = false;
                    if (curBlah.uv == 1)
                    {
                        btn_promote.SetBackgroundResource(Resource.Drawable.btn_promote_active);
                    }
                    else
                    {
                        btn_promote.SetBackgroundResource(Resource.Drawable.btn_demote_active);
                    }
                }
            }
        }
    }
}


