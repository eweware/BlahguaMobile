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
using Android.Text;
using Android.Text.Style;


namespace BlahguaMobile.AndroidClient
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait, UiOptions = Android.Content.PM.UiOptions.SplitActionBarWhenNarrow)]
    public class ViewPostActivity : Activity
    {
        private ViewPostCommentsFragment commentsFragment;
        private ViewPostSummaryFragment summaryFragment;
        private ViewPostStatsFragment statsFragment;
        private bool isFromCommentBtn = false;
        private Fragment curFragment = null;


        //private GestureDetector _gestureDetector;
        //private GestureListener _gestureListener;

        private ActionBar.Tab summaryTab, commentTab, statsTab;

		protected override void OnCreate (Bundle bundle)
		{
            this.Window.SetUiOptions(UiOptions.SplitActionBarWhenNarrow);
			this.Window.DecorView.SystemUiVisibility = StatusBarVisibility.Hidden;
            this.Window.AddFlags(WindowManagerFlags.Fullscreen);
            ActionBar.NavigationMode = ActionBarNavigationMode.Tabs;
            base.OnCreate(bundle);
			this.ActionBar.SetDisplayHomeAsUpEnabled(false);
			this.ActionBar.SetHomeButtonEnabled(false);
			this.ActionBar.SetDisplayShowHomeEnabled(false);
			this.ActionBar.SetDisplayShowTitleEnabled (false);
			SetContentView (Resource.Layout.activity_viewpost);

			/*
            _gestureListener = new GestureListener();
            _gestureDetector = new GestureDetector(this, _gestureListener);
            _gestureListener.SwipeLeftEvent += swipeLeftEvent;
            _gestureListener.SwipeRightEvent += swipeRightEvent;
*/
            this.Title = "";


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
			this.ActionBar.SetSplitBackgroundDrawable(new Android.Graphics.Drawables.ColorDrawable(Resources.GetColor(Resource.Color.heard_teal)));
			this.ActionBar.SetBackgroundDrawable(new Android.Graphics.Drawables.ColorDrawable(Resources.GetColor(Resource.Color.heard_teal)));
           
        }

        public override Android.Content.Res.Resources Resources
        {
            get
            {
                return new ResourceFix(base.Resources);
            }
        }

        private class ResourceFix : Android.Content.Res.Resources
        {
            private int targetId = 0;

            public ResourceFix(Android.Content.Res.Resources resources)
                : base(resources.Assets, resources.DisplayMetrics, resources.Configuration)
            {
                
                targetId = Android.Content.Res.Resources.System.GetIdentifier("split_action_bar_is_narrow", "bool", "android");

            }

            public override bool GetBoolean(int id)
            {
                return targetId == id || base.GetBoolean(id);
            }

        }


        public override bool OnPrepareOptionsMenu(IMenu menu)
        {
            menu.Clear();
            if (BlahguaAPIObject.Current.CurrentUser == null)
                MenuInflater.Inflate(Resource.Menu.blahmenu_signedout, menu);
            else
            {
                MenuInflater.Inflate(Resource.Menu.BlahMenu, menu);
                IMenuItem upVote = menu.FindItem(Resource.Id.action_upvote);
                IMenuItem downVote = menu.FindItem(Resource.Id.action_downvote);

                if (BlahguaAPIObject.Current.CurrentBlah != null)
                {
                    if (BlahguaAPIObject.Current.CurrentUser._id == BlahguaAPIObject.Current.CurrentBlah.A)
                    {
                        // can't vote on own blah
                        upVote.SetEnabled(false);
                        upVote.SetIcon(Resource.Drawable.ic_thumb_up_grey);
                        downVote.SetEnabled(false);
                        downVote.SetIcon(Resource.Drawable.ic_thumb_down_grey);
                    }
                    else if (BlahguaAPIObject.Current.CurrentBlah.uv == 0)
                    {
                        // user can still vote
                        upVote.SetEnabled(true);
                        upVote.SetIcon(Resource.Drawable.ic_thumb_up_white);
                        downVote.SetEnabled(true);
                        downVote.SetIcon(Resource.Drawable.ic_thumb_down_white);
                    }
                    else if (BlahguaAPIObject.Current.CurrentBlah.uv == 1)
                    {
                        // user promoted it
                        upVote.SetEnabled(false);
                        upVote.SetIcon(Resource.Drawable.ic_thumb_up_white);
                        downVote.SetEnabled(false);
                        downVote.SetIcon(Resource.Drawable.ic_thumb_down_grey);

                    }
                    else
                    {
                        // user demoted it
                        upVote.SetEnabled(false);
                        upVote.SetIcon(Resource.Drawable.ic_thumb_up_grey);
                        downVote.SetEnabled(false);
                        downVote.SetIcon(Resource.Drawable.ic_thumb_down_white);
                    }
                }
            }

            

            return base.OnPrepareOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.action_upvote:
                    if (BlahguaAPIObject.Current.CurrentUser != null)
                    {
                        HandlePromoteBlah();
                    }
                    break;
                case Resource.Id.action_downvote:
                    if (BlahguaAPIObject.Current.CurrentUser != null)
                    {
                        HandleDemoteBlah();
                    }
                    break;
                case Resource.Id.action_report:
                   
                    break;
                case Resource.Id.action_report_infringe:
                    // send mail
                    HandleReportPost();
                    break;
                case Resource.Id.action_report_spam:
                    BlahguaAPIObject.Current.ReportPost(2);
                    RunOnUiThread(() =>
                    {
                        Toast.MakeText(this, "Spam reported.", ToastLength.Short).Show();
                    });
                    break;
                case Resource.Id.action_report_offensive:
                    BlahguaAPIObject.Current.ReportPost(1);
                    RunOnUiThread(() =>
                    {
                        Toast.MakeText(this, "Post reported.", ToastLength.Short).Show();
                    });
                    break;
                case Resource.Id.action_share:
                    HandleSharePost();
                    break;
                case Resource.Id.action_comment:
                    if (BlahguaAPIObject.Current.CurrentUser != null)
                    {
                        HandleAddComment();
                    }
                    break;

                case Resource.Id.action_signin:
                    if (BlahguaAPIObject.Current.CurrentUser == null)
                    {
                        StartActivity(typeof(LoginActivity));
                    }
                    break;

                case 16908332:// the back button apparently...
                    {
                        Finish();
                    }
                    break;
                
            }
            return base.OnOptionsItemSelected(item);
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
			//_gestureDetector.OnTouchEvent (ev);
			return base.DispatchTouchEvent(ev);
		}

      

        protected override void OnResume()
        {
            base.OnResume();

            InvalidateOptionsMenu();
        }

        private void HandleSharePost()
        {

        }

        private void HandleReportPost()
        {
            Intent emailIntent = new Intent(Intent.ActionSendto,
                        Android.Net.Uri.FromParts("mailto", App.EmailReportBug, null));
            emailIntent.PutExtra(Intent.ExtraSubject, GetString(Resource.String.viewpost_report_email_subject));
            emailIntent.PutExtra(Intent.ExtraText, GetString(Resource.String.viewpost_report_infringement_body));
            StartActivity(Intent.CreateChooser(emailIntent, GetString(Resource.String.signin_infringe_chooser_title)));
        }

        private void HandleAddComment()
        {
            if (ActionBar.SelectedTab != commentTab)
            {
                isFromCommentBtn = true;
                ActionBar.SelectTab(commentTab);
            }
            else
                commentsFragment.triggerCreateBlock();
        }

       
        #region TabBar buttons
        private void btn_stats_Click(object sender, EventArgs e)
        {
            bool firstTime = false;

            if (statsFragment == null)
            {
                statsFragment = ViewPostStatsFragment.NewInstance();

                firstTime = true;
            }

            SetCurrentFragment(statsFragment, firstTime);
                
            
        }

        private void btn_summary_Click(object sender, EventArgs e)
        {
            bool firstTime = false;

            if (summaryFragment == null)
            {
                summaryFragment = ViewPostSummaryFragment.NewInstance();
                firstTime = true;
            }

            SetCurrentFragment(summaryFragment, firstTime);
        }

        private void btn_comments_Click(object sender, EventArgs e)
        {

            bool firstTime = false;
            
            if (commentsFragment == null)
            {
                commentsFragment = ViewPostCommentsFragment.NewInstance();
                firstTime = true;
                if (isFromCommentBtn)
                {
                    commentsFragment.shouldCreateComment = true;
                }
            }

            SetCurrentFragment(commentsFragment, firstTime);
        }


        void SetCurrentFragment(Fragment newFrag, bool firstTime, string theTitle = null)
        {
            if (curFragment != newFrag)
            {
                var fragmentManager = this.FragmentManager;
                var ft = fragmentManager.BeginTransaction();

                if (curFragment != null)
                    ft.Hide(curFragment);

                curFragment = newFrag;

                if (newFrag != null)
                {
                    if (firstTime)
                        ft.Add(Resource.Id.content_fragment, curFragment);
                    else
                        ft.Show(curFragment);
                    ft.Commit();
                }
            }

            if (!String.IsNullOrEmpty(theTitle))
                Title = theTitle;
            if (isFromCommentBtn)
            {
                isFromCommentBtn = false;
                if (firstTime)
                    commentsFragment.shouldCreateComment = true;
                else
                    commentsFragment.triggerCreateBlock();
            }

        }

        #endregion

        #region Handles

        private void HandlePromoteBlah()
        {
            BlahguaAPIObject.Current.SetBlahVote(1, (newVote) =>
            {
                UpdateSummaryButtons();
				HomeActivity.analytics.PostBlahVote(1);
            });
            
        }

        private void HandleDemoteBlah()
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
            InvalidateOptionsMenu();
        }
    }
}


