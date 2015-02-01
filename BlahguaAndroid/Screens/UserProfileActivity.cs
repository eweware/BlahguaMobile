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


namespace BlahguaMobile.AndroidClient
{
    [Activity(WindowSoftInputMode=SoftInput.AdjustPan, ScreenOrientation = ScreenOrientation.Portrait)]
    public class UserProfileActivity : Activity
    {

        private UserProfileProfileFragment profileFragment;
        private UserProfileDemographicsFragment demographicsFragment;
        private UserProfileBadgesFragment badgesFragment;
        private UserProfileStatsFragment statsFragment;
        private HistoryPostsFragment postsFragment;
        private HistoryCommentsFragment commentsFragment;

        private Fragment curFragment;

        private ActionBar.Tab profileTab, badgesTab, demoTab, postsTab, commentsTab, statsTab;

		protected override void OnCreate (Bundle bundle)
		{
            this.Window.AddFlags(WindowManagerFlags.Fullscreen);
            this.Window.ClearFlags(WindowManagerFlags.Fullscreen);
            ActionBar.NavigationMode = ActionBarNavigationMode.Tabs;
            base.OnCreate(bundle);
            this.ActionBar.SetDisplayHomeAsUpEnabled(true);
            this.ActionBar.SetHomeButtonEnabled(false);
            this.ActionBar.SetDisplayShowHomeEnabled(false);
			HomeActivity.analytics.PostPageView("/self");
            //RequestWindowFeature(WindowFeatures.NoTitle);
			SetContentView (Resource.Layout.activity_userprofile);

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
            this.ActionBar.SetBackgroundDrawable(new Android.Graphics.Drawables.ColorDrawable(Resources.GetColor(Resource.Color.heard_black)));
            this.ActionBar.SetStackedBackgroundDrawable(new Android.Graphics.Drawables.ColorDrawable(Resources.GetColor(Resource.Color.heard_teal)));
            this.ActionBar.SetSplitBackgroundDrawable(new Android.Graphics.Drawables.ColorDrawable(Resources.GetColor(Resource.Color.heard_black)));

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


