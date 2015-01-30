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
using Android.Graphics;

namespace BlahguaMobile.AndroidClient
{
	public class UserProfileFragment : Fragment
    {
        private Button btn_right;
        //private TextView title;

        private UserProfileProfileFragment profileFragment;
        private UserProfileDemographicsFragment demographicsFragment;
        private UserProfileBadgesFragment badgesFragment;
        private UserProfileStatsFragment statsFragment;

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView (inflater, container, savedInstanceState);

			HomeActivity.analytics.PostPageView("/self");

			View fragment = inflater.Inflate (Resource.Layout.fragment_userprofile, null);
			// Set our view from the "main" layout resource
		
            Button btn_back = fragment.FindViewById<Button>(Resource.Id.btn_back);
            btn_back.Click += delegate
            {
				if(FragmentManager.BackStackEntryCount >0)
					FragmentManager.PopBackStack();
			};

			btn_right = fragment.FindViewById<Button>(Resource.Id.btn_right);

            //title.SetTypeface(HomeActivity.merriweatherFont, Android.Graphics.TypefaceStyle.Normal);
            btn_right.SetTypeface(HomeActivity.merriweatherFont, TypefaceStyle.Normal);

            btn_right.Click += btn_right_Click;

            //title.Visibility = ViewStates.Gone;
            //btn_right.Visibility = ViewStates.Gone;
			int page = this.Arguments.GetInt ("Page");
			//int page = Activity.Intent.GetIntExtra("Page", 1);
			/*
            switch (page)
            {
                case 1:
                    {
                        profileFragment = null;
                        demographicsFragment = null;
                        statsFragment = null;

                        //title.Text = "Badges";
                        btn_right.Visibility = ViewStates.Visible;
                        btn_right.Text = "New";

                        badgesFragment = UserProfileBadgesFragment.NewInstance();
                        var fragmentTransaction = FragmentManager.BeginTransaction();

                        fragmentTransaction.Replace(Resource.Id.content_fragment, badgesFragment);
                        fragmentTransaction.Commit();
                    }
                    break;
                case 2:
                    {
                        badgesFragment = null;
                        profileFragment = null;
                        statsFragment = null;

                        //title.Text = "Demographics";
                        btn_right.Visibility = ViewStates.Visible;
                        btn_right.Text = "Done";

                        demographicsFragment = UserProfileDemographicsFragment.NewInstance();
                        var fragmentTransaction = FragmentManager.BeginTransaction();
                        fragmentTransaction.Replace(Resource.Id.content_fragment, demographicsFragment);
                        fragmentTransaction.Commit();
                    }
                    break;
                case 4:
                    {
                        badgesFragment = null;
                        profileFragment = null;
                        demographicsFragment = null;

                        //title.Text = "Statistics";
                        btn_right.Visibility = ViewStates.Gone;

                        statsFragment = UserProfileStatsFragment.NewInstance();
                        var fragmentTransaction = FragmentManager.BeginTransaction();
                        fragmentTransaction.Replace(Resource.Id.content_fragment, statsFragment);
                        fragmentTransaction.Commit();
                    }
                    break;
                default:
                    {
                        badgesFragment = null;
                        demographicsFragment = null;
                        statsFragment = null;

                        profileFragment = UserProfileProfileFragment.NewInstance();
                        var fragmentTransaction = FragmentManager.BeginTransaction();
                        fragmentTransaction.Replace(Resource.Id.content_fragment, profileFragment);
                        fragmentTransaction.Commit();
                    }
                    break;
            }
            */
			return fragment;
        }

        private void btn_right_Click(object sender, EventArgs e)
        {
            if (profileFragment != null) // that means it is active
            {
				if(FragmentManager.BackStackEntryCount >0)
					FragmentManager.PopBackStack();
                //commentsFragment.triggerCreateBlock();
            }
            if (badgesFragment != null)
            {
                badgesFragment.triggerNewBlock();
            }
            if (demographicsFragment != null) // that means it is active
            {
				if(FragmentManager.BackStackEntryCount >0)
					FragmentManager.PopBackStack();
                //commentsFragment.triggerCreateBlock();
            }
        }
    }
}


