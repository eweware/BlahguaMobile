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
using Android.Graphics;

using Android.Text;
using Android.Text.Style;

using BlahguaMobile.AndroidClient.HelpingClasses;


namespace BlahguaMobile.AndroidClient
{
    [Activity(WindowSoftInputMode=SoftInput.AdjustPan, ScreenOrientation = ScreenOrientation.Portrait)]
    public class UserProfileActivity : Activity, GestureDetector.IOnGestureListener
    {

        private UserProfileProfileFragment profileFragment;
        private UserProfileDemographicsFragment demographicsFragment;
        private UserProfileBadgesFragment badgesFragment;
        private UserProfileStatsFragment statsFragment;
        private HistoryPostsFragment postsFragment;
        private HistoryCommentsFragment commentsFragment;
	    private GestureDetector _gestureDetector;


        private Fragment curFragment;

        private ActionBar.Tab profileTab, badgesTab, demoTab, postsTab, commentsTab, statsTab;

		protected override void OnCreate (Bundle bundle)
		{
            //this.Window.AddFlags(WindowManagerFlags.Fullscreen);
			//this.Window.DecorView.SystemUiVisibility = StatusBarVisibility.Hidden;
            ActionBar.NavigationMode = ActionBarNavigationMode.Tabs;

            base.OnCreate(bundle);
            this.ActionBar.SetDisplayHomeAsUpEnabled(false);
            this.ActionBar.SetHomeButtonEnabled(false);
            this.ActionBar.SetDisplayShowHomeEnabled(false);
			this.ActionBar.SetDisplayShowTitleEnabled (false);
			HomeActivity.analytics.PostPageView("/self");
            //RequestWindowFeature(WindowFeatures.NoTitle);
			SetContentView (Resource.Layout.activity_userprofile);


			_gestureDetector = new GestureDetector(this, this);


            // set up tabs
            profileTab = ActionBar.NewTab();
            profileTab.SetText("Profile");
            profileTab.TabSelected += SelectProfile;
            ActionBar.AddTab(profileTab);

            badgesTab = ActionBar.NewTab();
            badgesTab.SetText("Badges");
            badgesTab.TabSelected += SelectBadges;
            ActionBar.AddTab(badgesTab);

            demoTab = ActionBar.NewTab();
            demoTab.SetText("Demographics");
            demoTab.TabSelected += SelectDemo;
            ActionBar.AddTab(demoTab);

            postsTab = ActionBar.NewTab();
            postsTab.SetText("Posts");
            postsTab.TabSelected += SelectPosts;
            ActionBar.AddTab(postsTab);

            commentsTab = ActionBar.NewTab();
            commentsTab.SetText("Comments");
            commentsTab.TabSelected += SelectComments;
            ActionBar.AddTab(commentsTab);

            statsTab = ActionBar.NewTab();
            statsTab.SetText("Stats");
            statsTab.TabSelected += SelectStats;
            ActionBar.AddTab(statsTab);


            int page = Intent.GetIntExtra("Page", 1);
            this.ActionBar.SetStackedBackgroundDrawable(new Android.Graphics.Drawables.ColorDrawable(Resources.GetColor(Resource.Color.heard_black)));
            
            switch (page)
            {
                case 1:
                    ActionBar.SelectTab(badgesTab);
                    break;
                   
                case 2:
                    ActionBar.SelectTab(demoTab);
                    break;

                case 3:
                    ActionBar.SelectTab(postsTab);
                    break;

                case 4:
                    ActionBar.SelectTab(statsTab);
                    break;

                default:
                    ActionBar.SelectTab(profileTab);
                    break;
            }
        }

        public bool OnScroll(MotionEvent e1, MotionEvent e2, float distanceX, float distanceY)
        {
            return false;
        }

        public bool OnSingleTapUp(MotionEvent e1)
        {
            return false;
        }

        public void OnLongPress(MotionEvent e1)
        {
            //return false;
        }

        public void OnShowPress(MotionEvent e1)
        {
            //return false;
        }

        public bool OnFling(MotionEvent e1, MotionEvent e2, float distanceX, float distanceY)
        {
            float maxRange = e1.Device.MotionRanges[0].Max;

            float minEdge = maxRange * .1f;
            float maxEdge = maxRange * .9f;

            if ((e1.GetX() < minEdge) && (e2.GetX() > maxRange * .3f))
            {
                swipeRightEvent();
                return true;
            }
            else if ((e1.GetX() > maxEdge) && (e2.GetX() < maxRange * .7f))
            {
                swipeLeftEvent();
                return true;
            }
            else
                return false;
        }

        public bool OnDown(MotionEvent e1)
        {
            return false;
        }


        protected void SelectStats(object sender, EventArgs e)
        {
            bool firstTime = false;

            if (statsFragment == null)
            {
                statsFragment = UserProfileStatsFragment.NewInstance();
                firstTime = true;
            }

            SetCurrentFragment(statsFragment, firstTime, "Statistics");

        }
		private void swipeLeftEvent()
		{

            if (curFragment == profileFragment)
                ActionBar.SelectTab(badgesTab);
            else if (curFragment == badgesFragment)
                ActionBar.SelectTab(demoTab);
            else if (curFragment == demographicsFragment)
                ActionBar.SelectTab(postsTab);
            else if (curFragment == postsFragment)
                ActionBar.SelectTab(commentsTab);
            else if (curFragment == commentsFragment)
                ActionBar.SelectTab(statsTab);
            else
                ActionBar.SelectTab(profileTab);

		}
		private void swipeRightEvent()
		{

            if (curFragment == statsFragment)
                ActionBar.SelectTab(commentsTab);
            else if (curFragment == commentsFragment)
                ActionBar.SelectTab(postsTab);
            else if (curFragment == postsFragment)
                ActionBar.SelectTab(demoTab);
            else if (curFragment == demographicsFragment)
                ActionBar.SelectTab(badgesTab);
            else if (curFragment == badgesFragment)
                ActionBar.SelectTab(profileTab);
            else
                ActionBar.SelectTab(statsTab);
	}

        public override bool DispatchTouchEvent(MotionEvent ev)
        {
            bool didIt = _gestureDetector.OnTouchEvent(ev);
            if (!didIt)
                return base.DispatchTouchEvent(ev);
            else
                return true;
        }

        protected void SelectBadges(object sender, EventArgs e)
        {
            bool firstTime = false;

            if (badgesFragment == null)
            {
                badgesFragment = UserProfileBadgesFragment.NewInstance();
                firstTime = true;
            }

            SetCurrentFragment(badgesFragment, firstTime, "Badges");

        }

        protected void SelectDemo(object sender, EventArgs e)
        {
            bool firstTime = false;

            if (demographicsFragment == null)
            {
                demographicsFragment = UserProfileDemographicsFragment.NewInstance();
                firstTime = true;
            }

            SetCurrentFragment(demographicsFragment, firstTime, "Demographics");

        }

        protected void SelectProfile(object sender, EventArgs e)
        {
            bool firstTime = false;

            if (profileFragment == null)
            {
                profileFragment = UserProfileProfileFragment.NewInstance();
                firstTime = true;
            }

            SetCurrentFragment(profileFragment, firstTime, "Profile");

        }

        protected void SelectComments(object sender, EventArgs e)
        {
            bool firstTime = false;

            if (commentsFragment == null)
            {
                commentsFragment = HistoryCommentsFragment.NewInstance();
                firstTime = true;
            }

            SetCurrentFragment(commentsFragment, firstTime, "Comment History");

        }

        protected void SelectPosts(object sender, EventArgs e)
        {
            bool firstTime = false;

            if (postsFragment == null)
            {
                postsFragment = HistoryPostsFragment.NewInstance();
                firstTime = true;
            }

            SetCurrentFragment(postsFragment, firstTime, "Post History");
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




        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case 16908332:// the back button apparently...
                    {
                        Finish();
                    }
                    break;

            }
            return base.OnOptionsItemSelected(item);
        }

        protected override void OnTitleChanged(Java.Lang.ICharSequence title, Color color)
        {
            SpannableString s = new SpannableString(title);
            s.SetSpan(new TypefaceSpan(this, "Merriweather.otf"), 0, s.Length(), SpanTypes.ExclusiveExclusive);
            s.SetSpan(new ForegroundColorSpan(Resources.GetColor(Resource.Color.heard_teal)), 0, s.Length(), SpanTypes.ExclusiveExclusive);

            this.ActionBar.TitleFormatted = s;

        }

        private void btn_right_Click(object sender, EventArgs e)
        {
            if (profileFragment != null) // that means it is active
            {
                Finish();
                //commentsFragment.triggerCreateBlock();
            }
            if (badgesFragment != null)
            {
                badgesFragment.triggerNewBlock();
            }
            if (demographicsFragment != null) // that means it is active
            {
                Finish();
                //commentsFragment.triggerCreateBlock();
            }
        }
    }
}


