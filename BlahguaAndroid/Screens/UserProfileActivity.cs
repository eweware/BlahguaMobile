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

using Android.Support.V7.Widget;
using Android.Support.V7.App;
using Android.Support.V4.View;
using Android.Support.V4.App;
using Android.Support.V4.Widget;
using com.refractored;

using BlahguaMobile.AndroidClient.HelpingClasses;


namespace BlahguaMobile.AndroidClient
{
	[Activity(Theme = "@style/AppSubTheme", ScreenOrientation = ScreenOrientation.Portrait)]
	public class UserProfileActivity : Android.Support.V7.App.ActionBarActivity, ViewPager.IOnPageChangeListener
    {
		private BGActionBarDrawerToggle drawerToggle;
		private Toolbar toolbar = null;
        public static UserProfileProfileFragment profileFragment;
        public static UserProfileDemographicsFragment demographicsFragment;
        public static UserProfileBadgesFragment badgesFragment;
        public static UserProfileStatsFragment statsFragment;
        public static HistoryPostsFragment postsFragment;
        public static HistoryCommentsFragment commentsFragment;
		private PagerSlidingTabStrip tabs;


		private DrawerLayout drawerLayout;
		private ListView drawerListView;
		private ViewPager pager;

		public class ProfilePageAdapter : FragmentPagerAdapter
		{
			private string[] Titles = { "Account", "Demographics", "Badges", "Posts", "Comments", "Stats" };
			Android.Support.V7.App.ActionBarActivity activity;

			public ProfilePageAdapter(Android.Support.V4.App.FragmentManager fm, Android.Support.V7.App.ActionBarActivity theActivity)
				: base(fm)
			{
				activity = theActivity;
			}

			public override Java.Lang.ICharSequence GetPageTitleFormatted(int position)
			{
				return new Java.Lang.String(Titles[position]);
			}

			public override int Count
			{
				get
				{
					return Titles.Length;
				}
			}

			public override Android.Support.V4.App.Fragment GetItem(int position)
			{
				Android.Support.V4.App.Fragment theItem = null;
				switch (position)
				{
				case 0:
					theItem = UserProfileActivity.profileFragment;
					break;

				case 1:
					theItem = UserProfileActivity.demographicsFragment;
					break;

				case 2:
					theItem = UserProfileActivity.badgesFragment;
					break;

				case 3:
					theItem = UserProfileActivity.postsFragment;
					break;

				case 4:
					theItem = UserProfileActivity.commentsFragment;
					break;

				case 5:
					theItem = UserProfileActivity.statsFragment;
					break;
				}
				return theItem;
			}
		}

		class DrawerItemAdapter<T> : ArrayAdapter<T>
		{
			T[] _items;
			Activity _context;

			public DrawerItemAdapter(Context context, int textViewResourceId, T[] objects) :
			base(context, textViewResourceId, objects)
			{
				_items = objects;
				_context = (Activity)context;
			}

			public override View GetView(int position, View convertView, ViewGroup parent)
			{
				View mView = convertView;
				if (mView == null)
				{
					mView = _context.LayoutInflater.Inflate(Android.Resource.Layout.SimpleListItemActivated1, parent, false);

				}

				TextView text = mView.FindViewById<TextView>(Android.Resource.Id.Text1);

				if (_items[position] != null)
				{
					text.Text = _items[position].ToString();
					//text.SetTextColor(_context.Resources.GetColor(Resource.Color.heard_teal));
					text.SetTypeface(HomeActivity.gothamFont, TypefaceStyle.Normal);
					if (position == BlahguaAPIObject.Current.CurrentChannelList.IndexOf(BlahguaAPIObject.Current.CurrentChannel))
					{
						text.SetTextColor(_context.Resources.GetColor(Resource.Color.heard_black));
						text.SetBackgroundColor(Color.White);
					}
					else
					{
						text.SetTextColor(Color.White);
						text.SetBackgroundColor(_context.Resources.GetColor(Resource.Color.heard_blue));
					}

				}

				return mView;
			}
		}

		protected override void OnCreate (Bundle bundle)
		{
             base.OnCreate(bundle);
			HomeActivity.analytics.PostPageView("/self");
            //RequestWindowFeature(WindowFeatures.NoTitle);
			SetContentView (Resource.Layout.activity_userprofile);

            toolbar = FindViewById<Toolbar>(Resource.Id.tool_bar);
            SetSupportActionBar(toolbar);

            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);
            SupportActionBar.SetDisplayShowHomeEnabled(false);
			
            this.Title = "profile info";


            profileFragment = new UserProfileProfileFragment();
            demographicsFragment = new UserProfileDemographicsFragment();
            badgesFragment = new UserProfileBadgesFragment();
            postsFragment = new HistoryPostsFragment();
            commentsFragment = new HistoryCommentsFragment();
            statsFragment = new UserProfileStatsFragment();

            pager = FindViewById<ViewPager>(Resource.Id.post_pager);
            pager.Adapter = new ProfilePageAdapter(this.SupportFragmentManager, this);

            tabs = FindViewById<PagerSlidingTabStrip>(Resource.Id.tabs);
            tabs.SetViewPager(pager);
            tabs.IndicatorColor = Resources.GetColor(Resource.Color.heard_teal);
            //tabs.TabTextColor = Resources.GetColorStateList(Resource.Color.tabtextcolor);
            tabs.TabTextColorSelected = Resources.GetColorStateList(Resource.Color.tabtextcolor);
            tabs.IndicatorHeight = Resources.GetDimensionPixelSize(Resource.Dimension.tab_indicator_height);
            tabs.UnderlineColor = Resources.GetColor(Resource.Color.heard_red);
            tabs.TabPaddingLeftRight = Resources.GetDimensionPixelSize(Resource.Dimension.tab_padding);
            tabs.OnPageChangeListener = this;
            //tabs.ShouldExpand = true;

            tabs.SetTabTextColor(Color.White);

            this.drawerLayout = this.FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            this.drawerListView = this.FindViewById<ListView>(Resource.Id.left_drawer);
            var headerView = LayoutInflater.Inflate(Resource.Layout.channelheader, null);
            drawerListView.AddHeaderView(headerView);


            //Set click handler when item is selected
            this.drawerListView.ItemClick += (sender, args) => ListItemClicked(args.Position);
            this.drawerListView.Divider = new ColorDrawable(Resources.GetColor(Resource.Color.heard_white));
            this.drawerListView.DividerHeight = 1;

            //DrawerToggle is the animation that happens with the indicator next to the actionbar
            this.drawerToggle = new BGActionBarDrawerToggle(this, this.drawerLayout,
                toolbar,
                Resource.String.app_name,
                Resource.String.app_name);

            //Display the current fragments title and update the options menu
            this.drawerToggle.DrawerClosed += (o, args) =>
            {

            };
                    

            //Display the drawer title and update the options menu
            this.drawerToggle.DrawerOpened += (o, args) =>
            {

            };

            //Set the drawer lister to be the toggle.
            this.drawerLayout.SetDrawerListener(this.drawerToggle);
            drawerLayout.SetStatusBarBackgroundColor(Resource.Color.heard_red);
            drawerLayout.SetScrimColor(Resource.Color.heard_red);
            drawerLayout.SetDrawerShadow(Resource.Drawable.draweredgeshadow, (int)GravityFlags.Left);

            populateChannelMenu();

            int page = Intent.GetIntExtra("Page", 1);

            pager.CurrentItem = page;

        }

        private void populateChannelMenu()
        {
            if (BlahguaAPIObject.Current.CurrentChannelList != null)
            {
                String[] channels = new String[BlahguaAPIObject.Current.CurrentChannelList.Count];
                int channelIndex = 0;

                foreach (Channel curChannel in BlahguaAPIObject.Current.CurrentChannelList)
                {
                    channels[channelIndex++] = curChannel.ChannelName;
                }
                this.drawerListView.Adapter = new DrawerItemAdapter<String>(this, Resource.Layout.item_menu, channels);
            }
        }

        private void ListItemClicked(int position)
        {
            if (position > 0)
            {
                position--;

                if (BlahguaAPIObject.Current.CurrentChannelList != null)
                {
                    Channel newChannel = BlahguaAPIObject.Current.CurrentChannelList[position];
                    if (newChannel != BlahguaAPIObject.Current.CurrentChannel)
                    {
                        this.Finish();
                        BlahguaAPIObject.Current.CurrentChannel = newChannel;
                    }
                    else
                    {
                        this.Finish();
                    }
                    this.drawerLayout.CloseDrawers();
                }
            }
        }


        public void OnPageScrolled(int position, float positionOffset, int positionOffsetPixels)
        {

        }

        public void OnPageScrollStateChanged(int state)
        {

        }

        public void OnPageSelected(int position)
        {
            switch (position)
            {
                case 0:
                    // Profile
                    break;

                case 1:
                    //demographics
                    break;

                case 2:
                    // badges
                    break;

                case 3:
                    // posts
					postsFragment.DrawUserPosts();
                    break;

                case 4:
                    // comments
					commentsFragment.DrawUserComments();
                    break;

                case 5:
                    // stats
                    break;
            }
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
            s.SetSpan(new ForegroundColorSpan(Resources.GetColor(Resource.Color.heard_white)), 0, s.Length(), SpanTypes.ExclusiveExclusive);

            toolbar.TitleFormatted = s;

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


