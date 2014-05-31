using System;
using System.Timers;
using System.ComponentModel;

using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using Android.OS;

using SlidingMenuSharp;
using SlidingMenuSharp.App;

using BlahguaMobile.BlahguaCore;
using BlahguaMobile.AndroidClient.ThirdParty.UrlImageViewHelper;

namespace BlahguaMobile.AndroidClient.Screens
{
	[Activity (MainLauncher = true)]
    public partial class MainActivity : SlidingFragmentActivity
    {
        Inbox blahList;
        Timer loadTimer = new Timer();
        Timer scrollTimer = new Timer();
        int inboxCounter = 0;

        int FramesPerSecond = 60;

        int screenMargin = 0;//24;
        int blahMargin = 0;//12;
        double smallBlahSize, mediumBlahSize, largeBlahSize;

        private readonly String sequence = "ABEAFADCADEACDAFAEBADADCAFABEAEBAFACDAEA";

        private RelativeLayout BlahContainer = null;
        private ScrollView BlahScroller = null;

        protected ListFragment Frag;
        private Button btn_login, btn_newpost;
        private ImageView avatarBar, avatar;
        private LinearLayout registered_layout;

		protected override void OnCreate (Bundle bundle)
		{
            base.OnCreate(bundle);

            RequestWindowFeature(WindowFeatures.NoTitle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.activity_main);

            BlahContainer = FindViewById<RelativeLayout>(Resource.Id.BlahContainer);
            BlahScroller = FindViewById<ScrollView>(Resource.Id.BlahScroller);
			//AddBlahs();

			// Get our button from the layout resource,
			// and attach an event to it

            registered_layout = FindViewById<LinearLayout>(Resource.Id.registered_layout);
            btn_newpost = FindViewById<Button>(Resource.Id.btn_newpost);
            avatarBar = FindViewById<ImageView>(Resource.Id.avatar);
            btn_login = FindViewById<Button>(Resource.Id.btn_login);
            btn_login.Click += delegate
            {
				//button.Text = string.Format ("{0} clicks!", count2++);
                StartActivity(typeof(LoginActivity));
			};


            scrollTimer.Interval = 1000 / FramesPerSecond;
            scrollTimer.Elapsed += ScrollBlahRoll;

            initSlidingMenu();

            initCreateBlahUi();
        }
        void StartTimers()
        {
            //targetBlah = null;
            scrollTimer.Start();
            //MaybeAnimateElement();
        }

        void StopTimers()
        {
            scrollTimer.Stop();
            //AnimateTextFadeIn.Stop();
            //AnimateTextFadeOut.Stop();
            //targetBlah = null;
        }
        private void ScrollBlahRoll(object sender, EventArgs e)
        {

            double curOffset = BlahScroller.ScrollY;
            curOffset += 1.0;
            BlahScroller.ScrollTo(0, (int)curOffset);

            DetectScrollAtEnd();

        }

        bool AtScrollEnd = false;
        void DetectScrollAtEnd()
        {
            int bottom = BlahScroller.GetChildAt(BlahScroller.ChildCount - 1).Bottom - BlahScroller.MeasuredHeight;
            if (BlahScroller.ScrollY == bottom)
            {
                if (!AtScrollEnd)
                {
                    AtScrollEnd = true;
                    FetchNextBlahList();
                }
            }
            
        }

        protected override void OnStart()
        {
            base.OnStart();
            
            BlahguaAPIObject.Current.PropertyChanged += new PropertyChangedEventHandler(On_API_PropertyChanged);
            InitService();

            StartTimers();
            
            //BlahguaAPIObject.Current.GetUserProfile((profile) =>
            //{
            //    int x = 0;
            //    x++;
            //});
        }

        private void On_API_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "CurrentChannel":
                    OnChannelChanged();
                    break;
            }
        }

        private void OnChannelChanged()
        {
            //FlushImpressionList();
            //LoadingBox.Visibility = Visibility.Visible;
            StopTimers();
            ClearBlahs();
            FetchInitialBlahList();
            //App.analytics.PostPageView("/channel/" + BlahguaAPIObject.Current.CurrentChannel.ChannelName);


        }

        #region Init
        private void InitService()
        {
            RunOnUiThread(() =>
            {
                Toast.MakeText(this, "looking for server...", ToastLength.Short).Show();
                //LoadingMessageBox.Text = "looking for server...";
            });

            loadTimer.Stop();
            loadTimer.Interval = 10000;
            loadTimer.Elapsed += delegate
            {
                RunOnUiThread(() =>
                {
                    Toast.MakeText(this, "still looking...", ToastLength.Short).Show();
                    //LoadingMessageBox.Text = "still looking...";
                });
            };
            loadTimer.Enabled = true;

            BlahguaAPIObject.Current.Initialize(null, DoServiceInited);

        }

        private void DoServiceInited(bool didIt)
        {
            loadTimer.Stop();
            if (didIt)
            {
                RunOnUiThread(() =>
                {
                    if (BlahguaAPIObject.Current.CurrentUser != null)
                    {
                        //App.analytics.PostAutoLogin();
                        //UserInfoBtn.Visibility = Visibility.Visible;
                        //NewBlahBtn.Visibility = Visibility.Visible;
                        //SignInBtn.Visibility = Visibility.Collapsed;
                        initSecondaryMenu();

                        btn_login.Visibility = ViewStates.Gone;
                        registered_layout.Visibility = ViewStates.Visible;
                        avatarBar.SetUrlDrawable(BlahguaAPIObject.Current.CurrentUser.UserImage);
                        avatar.SetUrlDrawable(BlahguaAPIObject.Current.CurrentUser.UserImage);
                    }
                    else
                    {
                        //UserInfoBtn.Visibility = Visibility.Collapsed;
                        //NewBlahBtn.Visibility = Visibility.Collapsed;
                        //SignInBtn.Visibility = Visibility.Visible;
                        btn_login.Visibility = ViewStates.Visible;
                        registered_layout.Visibility = ViewStates.Gone;
                    }
                });
                //this.DataContext = BlahguaAPIObject.Current;
                BlahguaAPIObject.Current.GetWhatsNew((whatsNew) =>
                {
                    if ((whatsNew != null) && (whatsNew.message != ""))
                    {
                        //ShowNewsFloater(whatsNew);
                    }
                });
            }
            else
            {
                //LoadingBox.Visibility = Visibility.Collapsed;
                //ConnectFailure.Visibility = Visibility.Visible;
            }
        }

        #region SlidingMenu
        private void initSlidingMenu()
        {
            Button btn_menu = FindViewById<Button>(Resource.Id.btn_menu);
            btn_menu.Click += delegate
            {
                SlidingMenu.ShowMenu(true);
            };

            SetBehindContentView(Resource.Layout.sidemenu_sorting);
            SlidingMenu.ShadowWidthRes = Resource.Dimension.shadow_width;
            SlidingMenu.BehindOffsetRes = Resource.Dimension.slidingmenu_offset;
            SlidingMenu.FadeDegree = 0.25f;
            SlidingMenu.TouchModeAbove = TouchMode.Fullscreen;
            SlidingMenu.Mode = MenuMode.Left;

            View leftMenu = SlidingMenu.GetMenu();

            String[] channels = new String[] {
                                GetString(Resource.String.sidemenu_ch_public),
                                GetString(Resource.String.sidemenu_ch_tech),
                                GetString(Resource.String.sidemenu_ch_entertainment),
                                GetString(Resource.String.sidemenu_ch_feedback) };
            String[] views = new String[] {
                                GetString(Resource.String.sidemenu_v_newest),
                                GetString(Resource.String.sidemenu_v_oldest),
                                GetString(Resource.String.sidemenu_v_most_popular),
                                GetString(Resource.String.sidemenu_v_most_promoted),
                                GetString(Resource.String.sidemenu_v_most_demoted) };

            ListView listChannels = leftMenu.FindViewById<ListView>(Resource.Id.listChannels);
            listChannels.ChoiceMode = ChoiceMode.Single;
            listChannels.Adapter = new ArrayAdapter(this, Resource.Layout.listitem_check, channels);
            listChannels.SetItemChecked(0, true);

            ListView listViews = leftMenu.FindViewById<ListView>(Resource.Id.listViews);
            listViews.ChoiceMode = ChoiceMode.Single;
            listViews.Adapter = new ArrayAdapter(this, Resource.Layout.listitem_check, views);
            listViews.SetItemChecked(0, true);

            listChannels.ItemClick += list_Click;
            listViews.ItemClick += list_Click;
        }

        bool secondaryMenuInitiated = false;
        private void initSecondaryMenu()
        {
            if (!secondaryMenuInitiated)
            {
                secondaryMenuInitiated = true;
            }
            else
            {
                return;
            }
            SlidingMenu.Mode = MenuMode.LeftRight;
            SlidingMenu.SetSecondaryMenu(Resource.Layout.sidemenu_profile);
            View rightMenu = SlidingMenu.GetSecondaryMenu();

            avatar = rightMenu.FindViewById<ImageView>(Resource.Id.avatar);

            //View demog = rightMenu.FindViewById<View>(Resource.Id.menu_demographics);
            //demog.Click += (sender, args) =>
            //{
            //    var intent = new Intent(this, typeof(UserProfileActivity));
            //    intent.PutExtra("Page", 2);
            //    StartActivity(intent);
            //};
            //View prof = rightMenu.FindViewById<View>(Resource.Id.menu_profile);
            //prof.Click += (sender, args) =>
            //{
            //    var intent = new Intent(this, typeof(UserProfileActivity));
            //    intent.PutExtra("Page", 1);
            //    StartActivity(intent);
            //};

            String[] profile = new String[] {
                                GetString(Resource.String.profilemenu_profile),
                                GetString(Resource.String.profilemenu_badges),
                                GetString(Resource.String.profilemenu_demographics),
                                GetString(Resource.String.profilemenu_history),
                                GetString(Resource.String.profilemenu_stats) };
            ListView listProfileMenu = rightMenu.FindViewById<ListView>(Resource.Id.listProfileMenu);
            listProfileMenu.ChoiceMode = ChoiceMode.None;
            listProfileMenu.Adapter = new ArrayAdapter(this, Resource.Layout.listitem_profile, profile);
            listProfileMenu.ItemClick += (sender, args) =>
            {
                SlidingMenu.Toggle();
                if (args.Position == 0) // Profile
                {
                    var intent = new Intent(this, typeof(UserProfileActivity));
                    intent.PutExtra("Page", 0);
                    StartActivity(intent);
                }
                else if (args.Position == 1) // Badges
                {
                    var intent = new Intent(this, typeof(UserProfileActivity));
                    intent.PutExtra("Page", 1);
                    StartActivity(intent);
                }
                else if (args.Position == 2) // Demographics
                {
                    var intent = new Intent(this, typeof(UserProfileActivity));
                    intent.PutExtra("Page", 2);
                    StartActivity(intent);
                }
                else if (args.Position == 3) // History
                {
                    var intent = new Intent(this, typeof(HistoryActivity));
                    StartActivity(intent);
                }
                else if (args.Position == 4) // Stats
                {
                    var intent = new Intent(this, typeof(UserProfileActivity));
                    intent.PutExtra("Page", 4);
                    StartActivity(intent);
                }
            };

            (rightMenu.FindViewById<Button>(Resource.Id.btn_logout)).Click += (sender, args) =>
            {
                ProgressDialog dialog = new ProgressDialog(this);
                dialog.SetMessage("Signing out...");
                dialog.SetCancelable(false);
                dialog.Show();

                SlidingMenu.Toggle();
                BlahguaAPIObject.Current.SignOut(null, (theStr) =>
                {
                    RunOnUiThread(() =>
                    {
                        btn_login.Visibility = ViewStates.Visible;
                        registered_layout.Visibility = ViewStates.Gone;

                        SlidingMenu.Mode = MenuMode.LeftRight;
                        OnChannelChanged();
                        dialog.Cancel();
                    });
                    //NavigationService.GoBack();
                }
                );
            };
        }

        private void list_Click(object sender, EventArgs e)
        {
            SlidingMenu.Toggle();
            OnChannelChanged();
        }
        #endregion

        #endregion

    }
}


