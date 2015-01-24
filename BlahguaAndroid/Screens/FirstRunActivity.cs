using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V4.App;
using Android.Support.V4.View;
using BlahguaMobile.AndroidClient.Screens;

namespace BlahguaMobile.AndroidClient
{
    [Activity(Label = "FirstRunActivity")]
    public class FirstRunActivity : FragmentActivity
    {
        private NonSwipeViewPager mPager;
        private PagerAdapter mPagerAdapter;



        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Create your application here
            SetContentView(Resource.Layout.ViewPager);
			mPager = FindViewById<NonSwipeViewPager>(Resource.Id.pager);
            mPagerAdapter = new ScreenSlidePageAdapter(SupportFragmentManager);
            mPager.Adapter = mPagerAdapter;
            mPager.TouchEnabled = false;
           
        }

        public override void OnBackPressed()
        {
            if (mPager.CurrentItem == 0)
            {
                base.OnBackPressed();
            }
            else
            {
                mPager.CurrentItem--;
            }
        }

        public void GoToNext()
        {
            mPager.CurrentItem++;
        }

        public void FinishSignin()
        {
            this.Finish();
        }


        private class ScreenSlidePageAdapter : FragmentStatePagerAdapter
        {
            private SignUpPage01Fragment page1 = null;
            private SignUpPage02Fragment page2 = null;
            private SignUpPage03Fragment page3 = null;

            public ScreenSlidePageAdapter(Android.Support.V4.App.FragmentManager mgr)
                : base(mgr)
            {
                // do nothing
            }

            public override int Count
            {
                get { return 3; }
            }

            public override Android.Support.V4.App.Fragment GetItem(int position)
            {
                Android.Support.V4.App.Fragment newFragment = null;

                switch (position)
                {
                    case 0:
                        // first page
                        if (page1 == null)
                        {
                            page1 = new SignUpPage01Fragment();
                        }
                        
                        newFragment = page1;

                        break;

                    case 1:
                        if (page2 == null)
                        {
                            page2 = new SignUpPage02Fragment();
                        }
                        newFragment = page2;
                        break;

                    case 2:
                        if (page3 == null)
                        {
                            page3 = new SignUpPage03Fragment();
                        }
                        newFragment = page3;
                        break;
                }

                return newFragment;
            }
        }
    }
}