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
    public class UserProfileActivity : Activity
    {
        private Button btn_right;
        private TextView title;

        private UserProfileProfileFragment profileFragment;
        private UserProfileDemographicsFragment demographicsFragment;

		protected override void OnCreate (Bundle bundle)
		{
            base.OnCreate(bundle);

            RequestWindowFeature(WindowFeatures.NoTitle);
			SetContentView (Resource.Layout.activity_userprofile);

            Button btn_back = FindViewById<Button>(Resource.Id.btn_back);
            btn_back.Click += delegate
            {
                Finish();
			};

            title = FindViewById<TextView>(Resource.Id.title);
            btn_right = FindViewById<Button>(Resource.Id.btn_right);

            btn_right.Click += btn_right_Click;

            //title.Visibility = ViewStates.Gone;
            //btn_right.Visibility = ViewStates.Gone;

            int page = Intent.GetIntExtra("Page", 1);

            switch (page)
            {
                case 2:
                    {
                        title.Text = "Demographics";
                        btn_right.Visibility = ViewStates.Gone;

                        demographicsFragment = UserProfileDemographicsFragment.NewInstance();
                        var fragmentTransaction = FragmentManager.BeginTransaction();
                        fragmentTransaction.Replace(Resource.Id.content_fragment, demographicsFragment);
                        fragmentTransaction.Commit();
                    }
                    break;
                default:
                    {
                        profileFragment = UserProfileProfileFragment.NewInstance();
                        var fragmentTransaction = FragmentManager.BeginTransaction();
                        fragmentTransaction.Replace(Resource.Id.content_fragment, profileFragment);
                        fragmentTransaction.Commit();
                    }
                    break;
            }
        }

        private void btn_right_Click(object sender, EventArgs e)
        {
            if (profileFragment != null) // that means it is active
            {
                Finish();
                //commentsFragment.triggerCreateBlock();
            }
        }
    }
}


