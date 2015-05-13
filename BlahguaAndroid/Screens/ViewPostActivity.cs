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
using System.Collections.Generic;
using Android.Support.V7.Widget;
using Android.Support.V7.App;
using Android.Support.V4.View;
using Android.Support.V4.App;
using Android.Support.V4.Widget;
using com.refractored;
using Android.Provider;
using File = Java.IO.File;
using Uri = Android.Net.Uri;

namespace BlahguaMobile.AndroidClient
{
    [Activity(Theme = "@style/AppSubTheme", ScreenOrientation = ScreenOrientation.Portrait)]
	public class ViewPostActivity : Android.Support.V7.App.AppCompatActivity, ViewPager.IOnPageChangeListener
    {
        private BGActionBarDrawerToggle drawerToggle;
        private bool isFromCommentBtn = false;

		private string[] badgeItemNames = null;
		private bool[]	badgeItemBools = null;
		private IMenuItem reportIcon = null;
		private Android.Support.V7.Widget.Toolbar toolbar = null;
        public static ViewPostSummaryFragment SummaryView;
        public static ViewPostCommentsFragment CommentsView;
        public static ViewPostStatsFragment StatsView;
        PagerSlidingTabStrip tabs;

        private DrawerLayout drawerLayout;
        private ListView drawerListView;
        private ViewPager pager;
        public static readonly int SELECTIMAGE_REQUEST = 777;
		private Android.Support.V7.Widget.ShareActionProvider actionProvider = null;

        public class PostPageAdapter : FragmentPagerAdapter, ICustomTabProvider
        {
            private string[] Titles = { "Summary", "Comments", "Stats" };
            private ImageView[] imageIcons = { null, null, null };
            private int[] Icons = { Resource.Drawable.btn_summary_normal, Resource.Drawable.btn_comments_normal, Resource.Drawable.btn_stats_normal };
            private int[] SelectedIcons = { Resource.Drawable.btn_summary_pressed, Resource.Drawable.btn_comments_pressed, Resource.Drawable.btn_stats_pressed };
			Android.Support.V7.App.AppCompatActivity activity;

			public PostPageAdapter(Android.Support.V4.App.FragmentManager fm, Android.Support.V7.App.AppCompatActivity theActivity)
                : base(fm)
            {
                activity = theActivity;
            }

            public override Java.Lang.ICharSequence GetPageTitleFormatted(int position)
            {
                return new Java.Lang.String(Titles[position]);
            }

            public void UpdateIcons(int curSel)
            {
                switch (curSel)
                {
                    case 0:
                        imageIcons[0].SetImageResource(SelectedIcons[0]);
                        imageIcons[1].SetImageResource(Icons[1]);
                        imageIcons[2].SetImageResource(Icons[2]);
                        break;
                    case 1:
                        imageIcons[0].SetImageResource(Icons[0]);
                        imageIcons[1].SetImageResource(SelectedIcons[1]);
                        imageIcons[2].SetImageResource(Icons[2]);
                        break;

                    case 2:
                        imageIcons[0].SetImageResource(Icons[0]);
                        imageIcons[1].SetImageResource(Icons[1]);
                        imageIcons[2].SetImageResource(SelectedIcons[2]);
                        break;
                }
            }
            public View GetCustomTabView(ViewGroup parent, int position)
            {
                ImageView newView = new ImageView(parent.Context);
                newView.SetImageResource(Icons[position]);
                imageIcons[position] = newView;
                return newView;
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
                        theItem = ViewPostActivity.SummaryView;
                        break;

                    case 1:
                        theItem = ViewPostActivity.CommentsView;
                        break;

                    case 2:
                        theItem = ViewPostActivity.StatsView;
                        break;

                }
                return theItem;
            }
        }

		protected override void OnPause ()
		{
			base.OnPause ();
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

            try
            {
                SetContentView(Resource.Layout.activity_viewpost);
            }
            catch (Exception exp)
            {
                System.Console.WriteLine(exp.Message);
            }


			toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.tool_bar);
            SetSupportActionBar(toolbar);

            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);
            SupportActionBar.SetDisplayShowHomeEnabled(false);

			HomeActivity.analytics.PostPageView("/blah");
			SummaryView = new ViewPostSummaryFragment ();
            CommentsView = new ViewPostCommentsFragment();
            CommentsView.baseView = this;

            StatsView = new ViewPostStatsFragment();

            pager = FindViewById<ViewPager>(Resource.Id.post_pager);
            pager.Adapter = new PostPageAdapter(this.SupportFragmentManager, this);

            tabs = FindViewById<PagerSlidingTabStrip>(Resource.Id.tabs);
            tabs.SetViewPager(pager);
            tabs.IndicatorColor = Resources.GetColor(Resource.Color.heard_teal);
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
			((PostPageAdapter)pager.Adapter).UpdateIcons(0);

        }

        public void UserTakePhoto()
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);

            HomeActivity._file = new File(HomeActivity._dir, String.Format("HeardPhoto_{0}.jpg", Guid.NewGuid()));

            intent.PutExtra(MediaStore.ExtraOutput, Uri.FromFile(HomeActivity._file));

            StartActivityForResult(intent, HomeActivity.PHOTO_CAPTURE_EVENT);
        }

        public void UserChoosePhoto()
        {
            var imageIntent = new Intent();
            imageIntent.SetType("image/*");
            imageIntent.SetAction(Intent.ActionGetContent);

            StartActivityForResult(
                Intent.CreateChooser(imageIntent, "Select image"), SELECTIMAGE_REQUEST);
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
                //Sections = channels;
                //Create Adapter for drawer List
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
                    break;

                case 1:
                    CommentsView.LoadComments();
                    break;

                case 2:
                    
                    break;
            }

            // redo the icons
            ((PostPageAdapter)pager.Adapter).UpdateIcons(position);


        }



		private void MultiListClicked(object sender, DialogMultiChoiceClickEventArgs args)
		{
			if (args.Which == 0)
				BlahguaAPIObject.Current.CreateCommentRecord.XX = !args.IsChecked;
			else if (args.Which == 1)
				BlahguaAPIObject.Current.CreateCommentRecord.XXX = args.IsChecked;
			else {
				int whichBadge = args.Which - 2;
				string badgeId = BlahguaAPIObject.Current.CurrentUser.Badges [whichBadge].ID;
				if (args.IsChecked) {
					// add badge
					if (BlahguaAPIObject.Current.CreateCommentRecord.BD == null)
						BlahguaAPIObject.Current.CreateCommentRecord.BD = new List<string> ();
					BlahguaAPIObject.Current.CreateCommentRecord.BD.Add (badgeId);
				} else {
					BlahguaAPIObject.Current.CreateCommentRecord.BD.Remove (badgeId);
				}
			}
		}


		private void BadgeOKClicked(Object sender, EventArgs args)
		{
			//Toast.MakeText(this, "Badge accepted!", ToastLength.Short).Show();
		}

		protected override Dialog OnCreateDialog(int id, Bundle args)
		{
			switch(id)
			{
			case HomeActivity.MultiChoiceDialog: 
				{
					UpdateBadgeInfo ();
					var builder = new Android.App.AlertDialog.Builder (this, Android.App.AlertDialog.ThemeHoloLight);
					builder.SetIcon (Resource.Drawable.ic_launcher);
					builder.SetTitle ("Sign your comment");
					builder.SetMultiChoiceItems (badgeItemNames, badgeItemBools, MultiListClicked);
					builder.SetPositiveButton ("Ok", BadgeOKClicked);

					Android.App.AlertDialog dlg = builder.Create ();

					return dlg;
				}
				break;
			}
			return base.OnCreateDialog(id, args);
		}

		private void UpdateBadgeInfo()
		{
			BadgeList badges = BlahguaAPIObject.Current.CurrentUser.Badges;

			if (badgeItemNames == null) {
				List<string>	badgeNames = new List<string> ();
				badgeNames.Add ("use profile");
				badgeNames.Add ("mature content");

				if (badges != null) {
					foreach (BadgeReference curBadge in badges) {
						badgeNames.Add (curBadge.BadgeName);
					}
				}
				badgeItemNames = badgeNames.ToArray ();
			}

			// now create the bool list
			badgeItemBools = new bool[badgeItemNames.Length];

			badgeItemBools [0] = !BlahguaAPIObject.Current.CreateCommentRecord.XX;
			badgeItemBools [1] = BlahguaAPIObject.Current.CreateCommentRecord.XXX;

			if (badges != null) {
				int i = 2;
				if (BlahguaAPIObject.Current.CreateCommentRecord.BD == null) {
					foreach (BadgeReference curBadge in badges) {
						badgeItemBools [i++] = false;
					}
				} else {
					foreach (BadgeReference curBadge in badges) {
						badgeItemBools [i++] = BlahguaAPIObject.Current.CreateCommentRecord.BD.Contains (curBadge.ID);
					}
				}
			}

		}



        public override bool OnPrepareOptionsMenu(IMenu menu)
        {
            try
            {
                menu.Clear();
                if (BlahguaAPIObject.Current.CurrentUser == null)
                    MenuInflater.Inflate(Resource.Menu.blahmenu_signedout, menu);
                else
                {
                    MenuInflater.Inflate(Resource.Menu.BlahMenu, menu);
                    IMenuItem upVote = menu.FindItem(Resource.Id.action_upvote);

                    if (!BlahguaAPIObject.Current.CanComment)
                        menu.FindItem(Resource.Id.action_comment).SetVisible(false);

                    if (BlahguaAPIObject.Current.CurrentBlah != null)
                    {
                        if (BlahguaAPIObject.Current.CurrentUser._id == BlahguaAPIObject.Current.CurrentBlah.A)
                        {
                            // can't vote on own blah
                            upVote.SetEnabled(false);
                            upVote.SetIcon(Resource.Drawable.ic_thumb_up_grey);
                        }
                        else if (BlahguaAPIObject.Current.CurrentBlah.uv == 0)
                        {
                            // user can still vote
                            upVote.SetEnabled(true);
                            upVote.SetIcon(Resource.Drawable.ic_thumb_up_white);
                        }
                        else if (BlahguaAPIObject.Current.CurrentBlah.uv == 1)
                        {
                            // user promoted it
                            upVote.SetEnabled(false);
                            upVote.SetIcon(Resource.Drawable.ic_thumb_up_black);

                        }
                        else
                        {
                            // user demoted it
                            upVote.SetEnabled(false);
                            upVote.SetIcon(Resource.Drawable.ic_thumb_up_grey);
                        }
                    }
                }

				reportIcon = menu.FindItem(Resource.Id.action_report);
				if (actionProvider == null)
				{
					Blah curBlah = BlahguaAPIObject.Current.CurrentBlah;
					string blahURL = "http://app.goheard.com/?blahId=" + curBlah._id;
					var shareItem = menu.FindItem(Resource.Id.action_share);
					var nativeAction = MenuItemCompat.GetActionProvider(shareItem);
					actionProvider = nativeAction.JavaCast<Android.Support.V7.Widget.ShareActionProvider>();
					var intent = new Intent(Intent.ActionSend);
					intent.SetType("text/plain");
					intent.AddFlags(ActivityFlags.ClearWhenTaskReset);
					intent.PutExtra(Intent.ExtraTitle, "Shared from Heard");
					intent.PutExtra(Intent.ExtraText, blahURL);
					intent.PutExtra(Intent.ExtraSubject, curBlah.T);
					actionProvider.SetShareIntent(intent);

				}


            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.Message);
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
                        item.SetIcon(Resource.Drawable.ic_thumb_up_black);
                        HandlePromoteBlah();
                    }
                    break;

                case Resource.Id.action_report:
                   
                    break;
				case Resource.Id.action_report_infringe:
	                    // send mail
					//if (reportIcon != null)
					//	reportIcon.SetIcon (Resource.Drawable.ic_report_problem_black);
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
			if (pager.CurrentItem != 1)
				pager.CurrentItem = 1;

			CommentsView.triggerCreateBlock ();
            
        }



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

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
		    if ((requestCode == SELECTIMAGE_REQUEST || requestCode == HomeActivity.PHOTO_CAPTURE_EVENT)
			    && resultCode == Android.App.Result.Ok)
		    {

                CommentsView.HandleCommentImage(requestCode, data);
                
            }
            else
                base.OnActivityResult(requestCode, resultCode, data);
        }
    }
}


