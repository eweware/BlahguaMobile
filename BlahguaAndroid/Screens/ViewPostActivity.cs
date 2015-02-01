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

        private Fragment curFragment = null;


        private GestureDetector _gestureDetector;
        private GestureListener _gestureListener;

        private ActionBar.Tab summaryTab, commentTab, statsTab;

		protected override void OnCreate (Bundle bundle)
		{
            ActionBar.NavigationMode = ActionBarNavigationMode.Tabs;
            ActionBar.SetDisplayShowTitleEnabled(true);
            //ActionBar.SetDisplayShowHomeEnabled(false);
            base.OnCreate(bundle);
            this.ActionBar.SetDisplayHomeAsUpEnabled(true);
            this.ActionBar.SetHomeButtonEnabled(true);
            this.ActionBar.SetDisplayShowHomeEnabled(true);
			SetContentView (Resource.Layout.activity_viewpost);

            _gestureListener = new GestureListener();
            _gestureDetector = new GestureDetector(this, _gestureListener);
            _gestureListener.SwipeLeftEvent += swipeLeftEvent;
            _gestureListener.SwipeRightEvent += swipeRightEvent;

            this.Title = "   back to Heard";


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
            this.ActionBar.SetStackedBackgroundDrawable(new Android.Graphics.Drawables.ColorDrawable(Resources.GetColor(Resource.Color.heard_teal)));
            this.ActionBar.SetSplitBackgroundDrawable(new Android.Graphics.Drawables.ColorDrawable(Resources.GetColor(Resource.Color.heard_black)));
            this.ActionBar.SetBackgroundDrawable(new Android.Graphics.Drawables.ColorDrawable(Resources.GetColor(Resource.Color.heard_black)));
           
        }

        protected override void OnTitleChanged(Java.Lang.ICharSequence title, Color color)
        {
            SpannableString s = new SpannableString(title);
            s.SetSpan(new TypefaceSpan(this, "Merriweather.otf"), 0, s.Length(), SpanTypes.ExclusiveExclusive);
            s.SetSpan(new ForegroundColorSpan(Resources.GetColor(Resource.Color.heard_teal)), 0, s.Length(), SpanTypes.ExclusiveExclusive);

            this.ActionBar.TitleFormatted = s;

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
                    HandleReportPost();
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
            base.DispatchTouchEvent(ev);
            return _gestureDetector.OnTouchEvent(ev);
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
        
        }

        private void HandleAddComment()
        {
            if (ActionBar.SelectedTab != commentTab)
            {
                if (commentsFragment == null)
                    commentsFragment = ViewPostCommentsFragment.NewInstance();
                commentsFragment.shouldCreateComment = true;
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


