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
using Android.Preferences;
using BlahguaMobile.AndroidClient.Adapters;
using System.IO.IsolatedStorage;
using Android.Graphics;
using Android.Animation;
using System.Collections.Generic;

namespace BlahguaMobile.AndroidClient.Screens
{
	[Activity]
    public partial class MainActivity : SlidingFragmentActivity
    {
        Inbox blahList;
        Timer loadTimer = new Timer();
        Timer scrollTimer = new Timer();
        Timer BlahAnimateTimer = new Timer();
        int inboxCounter = 0;

        int FramesPerSecond = 60;

		int screenMargin = 24;
		int blahMargin = 12;
        double smallBlahSize, mediumBlahSize, largeBlahSize;

        private readonly String sequence = "ABEAFADCADEACDAFAEBADADCAFABEAEBAFACDAEA";

        private LinearLayout BlahContainerLayout = null;
        private ScrollView BlahScroller = null;
        private BlahFrameLayout CurrentBlahContainer = null;

        protected ListFragment Frag;
        private Button btn_login;
        private ImageView btn_newpost;
        private ImageView avatarBar, avatar;
        private LinearLayout registered_layout;

        private ProgressBar progress_actionbar;

        private TextView main_title;

        public static GoogleAnalytics analytics = null;
        private static Typeface titleFont = null;


		protected override void OnCreate (Bundle bundle)
		{
            base.OnCreate(bundle);

            RequestWindowFeature(WindowFeatures.NoTitle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.activity_main);

            BlahContainerLayout = FindViewById<LinearLayout>(Resource.Id.BlahContainer);
            BlahScroller = FindViewById<ScrollView>(Resource.Id.BlahScroller);
            TextView mainTitle = FindViewById<TextView>(Resource.Id.main_title);
            if (titleFont == null)
                titleFont = Typeface.CreateFromAsset(this.ApplicationContext.Assets, "fonts/Merriweather.otf");
            mainTitle.SetTypeface(titleFont, TypefaceStyle.Normal);
            FindViewById<TextView>(Resource.Id.btn_login).SetTypeface(titleFont, TypefaceStyle.Normal);

			// Get our button from the layout resource,
			// and attach an event to it

            main_title = FindViewById<TextView>(Resource.Id.main_title);

            registered_layout = FindViewById<LinearLayout>(Resource.Id.registered_layout);
            btn_newpost = FindViewById<ImageView>(Resource.Id.btn_newpost);
            avatarBar = FindViewById<ImageView>(Resource.Id.avatar);
            btn_login = FindViewById<Button>(Resource.Id.btn_login);
            btn_login.Click += delegate
            {
				//button.Text = string.Format ("{0} clicks!", count2++);
                StartActivity(typeof(LoginActivity));
			};
            progress_actionbar = FindViewById<ProgressBar>(Resource.Id.progress_actionbar);

            scrollTimer.Interval = 1000 / FramesPerSecond;
            scrollTimer.Elapsed += ScrollBlahRoll;

            BlahAnimateTimer.Interval = 1000;
            BlahAnimateTimer.Elapsed += BlahAnimateTimer_Elapsed;

            initSlidingMenu();    // move this until after the service is inited

            SlidingMenu.Opened += (sender, args) =>
            {
                StopTimers();
            };
            SlidingMenu.Closed += (sender, args) =>
            {
                if(create_post_block.Visibility != ViewStates.Visible)
                    StartTimers();
            };

            initCreateBlahUi();

            BlahguaAPIObject.Current.PropertyChanged += new PropertyChangedEventHandler(On_API_PropertyChanged);
            InitAnalytics();
            InitService();
        }

        public override void OnBackPressed()
        {
            if (create_post_block.Visibility.Equals(ViewStates.Visible))
            {
                triggerCreateBlock();
            }
            else
            {
                base.OnBackPressed();
            }
        }

        private void InitAnalytics()
        {
            string uniqueId;

            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            if (settings.Contains("uniqueId"))
                uniqueId = settings["uniqueId"].ToString();
            else
            {
                uniqueId = Guid.NewGuid().ToString();
                settings.Add("uniqueId", uniqueId);
                settings.Save();

            }

            string maker = Build.Manufacturer;
            string model = Build.Model;
            string version = ApplicationContext.PackageManager.GetPackageInfo(ApplicationContext.PackageName, 0).VersionName;
            string platform = "ANDROID";
            string userAgent = "Mozilla/5.0 (Lonux; Android; Mobile) ";

            analytics = new GoogleAnalytics(userAgent, maker, model, version, platform, uniqueId);
            analytics.StartSession();
        }

        private void StartTimers()
        {
            //targetBlah = null;
            scrollTimer.Start();
             BlahAnimateTimer.Start();
        }

        private static long fadeDuration = 2000;
        private static Random rnd = new Random();
        private View lastAnimatedBlah;

        private void MaybeAnimateElement()
        {
            View targetView = null;
            Rect scrollBounds = new Rect();
            
            List<View> targetList = new List<View>();
            bool isDone = false;

            for (int curContainer = 0; curContainer < BlahContainerLayout.ChildCount; curContainer++)
            {
                BlahScroller.GetHitRect(scrollBounds);
                BlahFrameLayout curContainerView = BlahContainerLayout.GetChildAt(curContainer) as BlahFrameLayout;
                bool isVisible = curContainerView.GetLocalVisibleRect(scrollBounds);

                for (int curBlahCount = 0; curBlahCount < curContainerView.ChildCount; curBlahCount++)
                {

                    View curBlahView = curContainerView.GetChildAt(curBlahCount);
                    var image = curBlahView.FindViewById<ImageView>(Resource.Id.image);
                    if ((curBlahView != lastAnimatedBlah) && (image.Tag != null))
                    {
                        // it wants animation
                        BlahScroller.GetHitRect(scrollBounds);
                        if (curBlahView.GetLocalVisibleRect(scrollBounds))
                        {
                            targetList.Add(curBlahView);
                        }
                        else
                        {
                            if (targetList.Count > 0)
                            {
                                isDone = true;
                                break;
                            }
                        }
                    }
                }

                if (isDone)
                    break;
            }

            if (targetList.Count > 0)
                targetView = targetList[rnd.Next(targetList.Count)];

            if (targetView != null)
            {
                lastAnimatedBlah = targetView;
                var title = targetView.FindViewById<LinearLayout>(Resource.Id.textLayout);
                float targetAlpha = 0f;
                if (title.Alpha == 0f)
                    targetAlpha = 0.9f;


                title.Animate().Alpha(targetAlpha).SetDuration(fadeDuration);
                
            }

        }

        void BlahAnimateTimer_Elapsed(object sender, EventArgs e)
        {
            //BlahAnimateTimer.Stop();
            MaybeAnimateElement();
        }

        private void OnAnimationEnd(object sender, EventArgs e)
        {
            MaybeAnimateElement();
        }


        private void StopTimers()
        {
            scrollTimer.Stop();
            //AnimateTextFadeIn.Stop();
            //AnimateTextFadeOut.Stop();
            //targetBlah = null;
        }

        private void ScrollBlahRoll(object sender, EventArgs e)
        {

            int curOffset = BlahScroller.ScrollY;
            curOffset += 1;
            BlahScroller.ScrollTo(0, curOffset);

            DetectScrollAtEnd();
        }

        bool AtScrollEnd = false;
        private void DetectScrollAtEnd()
        {
            int bottom = BlahContainerLayout.GetChildAt(BlahContainerLayout.ChildCount - 1).Bottom - BlahScroller.MeasuredHeight;
            if (BlahScroller.ScrollY == bottom)
            {
                if (!AtScrollEnd)
                {
                    AtScrollEnd = true;
                    FetchNextBlahList();
                }
            }
            
        }

        protected override void OnResume()
        {
            base.OnResume();
            if (create_post_block.Visibility != ViewStates.Visible)
                StartTimers();
            initLayouts();
        }

        protected override void OnPause()
        {
            base.OnPause();
            StopTimers();
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

            RunOnUiThread(() => {
                main_title.Text = BlahguaAPIObject.Current.CurrentChannel.ChannelName;
            });
        }

        #region Init
        private void InitService()
        {
            progress_actionbar.Visibility = ViewStates.Visible;
            btn_login.Visibility = ViewStates.Gone;

            Toast.MakeText(this, "looking for server...", ToastLength.Short).Show();

            loadTimer.Stop();
            loadTimer.Interval = 10000;
            loadTimer.Elapsed += delegate
            {
                RunOnUiThread(() =>
                {
                    Toast.MakeText(this, "still looking...", ToastLength.Short).Show();
                });
            };
            loadTimer.Enabled = true;

            ISharedPreferences _sharedPref = PreferenceManager.GetDefaultSharedPreferences(this);
            BlahguaAPIObject.Current.UserName = _sharedPref.GetString("username", "");
            BlahguaAPIObject.Current.UserPassword = _sharedPref.GetString("password", "");

            BlahguaAPIObject.Current.Initialize(null, DoServiceInited);
        }

        bool firstInit = true;
        private void initLayouts()
        {
            if (progress_actionbar.Visibility == ViewStates.Gone)
            {
                if(create_post_block.Visibility == ViewStates.Gone)
                    populateChannelMenu();
                if (BlahguaAPIObject.Current.CurrentUser != null)
                {
                    MainActivity.analytics.PostAutoLogin();
                    //UserInfoBtn.Visibility = Visibility.Visible;
                    //NewBlahBtn.Visibility = Visibility.Visible;
                    //SignInBtn.Visibility = Visibility.Collapsed;
                    initSecondaryMenu();

                    btn_login.Visibility = ViewStates.Gone;
                    registered_layout.Visibility = ViewStates.Visible;
                    avatarBar.SetUrlDrawable(BlahguaAPIObject.Current.CurrentUser.UserImage, avatarBar.Drawable);
                    avatar.SetUrlDrawable(BlahguaAPIObject.Current.CurrentUser.UserImage, avatar.Drawable);

                    if (firstInit)
                    {
                        avatar.Click += (sender, args) =>
                        {
                            var intent = new Intent(this, typeof(UserProfileActivity));
                            intent.PutExtra("Page", 0);
                            StartActivity(intent);
                            SlidingMenu.Toggle();
                        };
                        avatarBar.Click += (sender, args) =>
                        {
                            SlidingMenu.ShowSecondaryMenu();
                        };
                        firstInit = false;
                    }
                }
                else
                {
                    //UserInfoBtn.Visibility = Visibility.Collapsed;
                    //NewBlahBtn.Visibility = Visibility.Collapsed;
                    //SignInBtn.Visibility = Visibility.Visible;
                    btn_login.Visibility = ViewStates.Visible;
                    registered_layout.Visibility = ViewStates.Gone;
                }
            }
        }

        private void DoServiceInited(bool didIt)
        {
            RunOnUiThread(() =>
            {
                progress_actionbar.Visibility = ViewStates.Gone;
            });
            loadTimer.Stop();
            if (didIt)
            {
                RunOnUiThread(() =>
                {
                    initLayouts();
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
                RunOnUiThread(() =>
                {
                    Toast.MakeText(this, "server connection failure", ToastLength.Short).Show();
                });
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
        }

        private void populateChannelMenu()
        {
            View leftMenu = SlidingMenu.GetMenu();

            String[] channels = new String[BlahguaAPIObject.Current.CurrentChannelList.Count];
            int channelIndex = 0;

            foreach (Channel curChannel in BlahguaAPIObject.Current.CurrentChannelList)
            {
                channels[channelIndex++] = curChannel.ChannelName;
            }

            listChannels = leftMenu.FindViewById<ListView>(Resource.Id.listChannels);
            listChannels.ChoiceMode = ChoiceMode.Single;
            listChannels.Adapter = new ArrayAdapter(this, Resource.Layout.listitem_check, channels);
            listChannels.SetItemChecked(0, true);


            listChannels.ItemClick += listChannel_Click;
        }


        private ListView listChannels;

        private bool secondaryMenuInitiated = false;
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
                    if (theStr)
                    {
                        RunOnUiThread(() =>
                        {
                            btn_login.Visibility = ViewStates.Visible;
                            registered_layout.Visibility = ViewStates.Gone;

                            ISharedPreferences _sharedPref = PreferenceManager.GetDefaultSharedPreferences(this);
                            _sharedPref.Edit().Remove("username").Commit();
                            _sharedPref.Edit().Remove("password").Commit();

                            SlidingMenu.Mode = MenuMode.LeftRight;
                            OnChannelChanged();
                            dialog.Cancel();
                        });
                    }

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

        private void listChannel_Click(object sender, EventArgs e)
        {
            if (BlahguaAPIObject.Current.CurrentChannelList != null)
            {
                int checkedId = (int)listChannels.GetCheckItemIds()[0];
                BlahguaAPIObject.Current.CurrentChannel =
                    BlahguaAPIObject.Current.CurrentChannelList[checkedId];
            }

            SlidingMenu.Toggle();
        }
        #endregion

        #endregion

    }

    class ToggleFadeListener : Java.Lang.Object, Android.Animation.Animator.IAnimatorListener
    {
        public EventHandler EndHandler = null;

        public void OnAnimationEnd(Animator animation)
        {

            if (EndHandler != null)
                EndHandler(this, new EventArgs());

        }

        public void OnAnimationRepeat(Animator animation)
        {
        }

        public void OnAnimationCancel(Animator animation)
        {
        }


        public void OnAnimationStart(Animator animation)
        {
           
        }
    }


}


