using System;
using System.Timers;
using System.ComponentModel;

//using Android.App;
using Android.Support.V4.App;
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
using Android.Content.PM;
using BlahguaMobile.AndroidClient.HelpingClasses;

namespace BlahguaMobile.AndroidClient.Screens
{
	public partial class MainFragment : Fragment
    {
        Inbox blahList;
        Timer loadTimer = new Timer();
        Timer scrollTimer = new Timer();
        Timer BlahAnimateTimer = new Timer();
        int inboxCounter = 0;
        public static Typeface gothamFont = null;
        public static Typeface merriweatherFont = null;

        int FramesPerSecond = 60;

		int screenMargin = 24;
		int blahMargin = 12;
        double smallBlahSize, mediumBlahSize, largeBlahSize;

        private readonly String sequence = "ABEAFADCADEACDAFAEBADADCAFABEAEBAFACDAEA";

        private LinearLayout BlahContainerLayout = null;
        private ScrollView BlahScroller = null;
        private BlahFrameLayout CurrentBlahContainer = null;

        protected ListFragment Frag;
        private TextView userName;	
        private ProgressBar progress_main;

        public static GoogleAnalytics analytics = null;
        private static Typeface titleFont = null;

		private HomeActivity homeActivity;
		private View fragment;

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView (inflater, container, savedInstanceState);

			fragment = inflater.Inflate (Resource.Layout.fragment_main, null);
			// Set our view from the "main" layout resource

			BlahContainerLayout = fragment.FindViewById<LinearLayout> (Resource.Id.BlahContainer);
			BlahScroller = fragment.FindViewById<ScrollView> (Resource.Id.BlahScroller);

			progress_main = fragment.FindViewById<ProgressBar> (Resource.Id.loader_main);

			scrollTimer.Interval = 1000 / FramesPerSecond;
			scrollTimer.Elapsed += ScrollBlahRoll;

			BlahAnimateTimer.Interval = 1000;
			BlahAnimateTimer.Elapsed += BlahAnimateTimer_Elapsed;

			//this.Activity.initCreateBlahUi ();

			BlahguaAPIObject.Current.PropertyChanged += new PropertyChangedEventHandler (On_API_PropertyChanged);

			homeActivity = (HomeActivity)this.Activity;
			// create the fonts
			//FetchInitialBlahList();

			initCreateBlahUi();

			return fragment;
		}

        private void StartTimers()
        {
            //targetBlah = null;
            scrollTimer.Start();
            BlahAnimateTimer.Start();
        }


        private void StopTimers()
        {
            scrollTimer.Stop();
            BlahAnimateTimer.Stop();
            //AnimateTextFadeIn.Stop();
            //AnimateTextFadeOut.Stop();
            //targetBlah = null;
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
                    //FetchNextBlahList();
                }
            }
            
        }

		public override void OnResume()
        {
            base.OnResume();
            //if (create_post_block.Visibility != ViewStates.Visible)
                StartTimers();
            initLayouts();
        }

		public override void OnPause()
        {
            base.OnPause();
            StopTimers();
        }

        private void On_API_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "CurrentChannel":
                    //OnChannelChanged();
                    break;
            }
        }

        bool firstInit = true;
		public void InitLayouts()
		{
			initLayouts ();
		}
        private void initLayouts()
        {
			if (homeActivity.GetRefreshActionButtonState() == false)
            {

                if (BlahguaAPIObject.Current.CurrentUser != null)
                {
					HomeActivity.analytics.PostAutoLogin();
                    //UserInfoBtn.Visibility = Visibility.Visible;
                    //NewBlahBtn.Visibility = Visibility.Visible;
                    //SignInBtn.Visibility = Visibility.Collapsed;

                    //userName.Text = BlahguaAPIObject.Current.CurrentUser.UserName;

					homeActivity.SetLoginButtonActionView (Resource.Layout.action_login_button);
					homeActivity.SetCreateButtonVisible (true);
                	firstInit = false;
                    
                }
                else
                {

                }
            }

			//avatar.SetUrlDrawable(BlahguaAPIObject.Current.CurrentUser.UserImage, avatar.Drawable);
        }

        private void DoServiceInited(bool didIt)
        {
			this.Activity.RunOnUiThread(() =>
            {
					homeActivity.setRefreshActionButtonState(false);// progress_actionbar.Visibility = ViewStates.Gone;
            });
            loadTimer.Stop();
            if (didIt)
            {
				this.Activity.RunOnUiThread(() =>
                {
                    initLayouts();
                });
                if (BlahguaAPIObject.Current.CurrentUser != null)
                {
                    //this.DataContext = BlahguaAPIObject.Current;
                    BlahguaAPIObject.Current.GetWhatsNew((whatsNew) =>
                    {
                        if ((whatsNew != null) && (whatsNew.message != ""))
                        {
                            ShowNewsFloater(whatsNew);
                        }
                    });
                }
            }
            else
            {
				this.Activity.RunOnUiThread(() =>
                {
						Toast.MakeText(this.Activity, "server connection failure", ToastLength.Short).Show();
                });
                //LoadingBox.Visibility = Visibility.Collapsed;
                //ConnectFailure.Visibility = Visibility.Visible;
            }
        }
        private DateTime whatsNewTimestamp = DateTime.MinValue;
        private void ShowNewsFloater(WhatsNewInfo newInfo)
        {
            if (whatsNewTimestamp == DateTime.MinValue ||
                DateTime.Now - whatsNewTimestamp > TimeSpan.FromSeconds(5))
            {
                whatsNewTimestamp = DateTime.Now;
                //var dialogToClose = WhatsNewDialog.ShowDialog(FragmentManager, newInfo);
                //new Handler(Looper.MainLooper).PostDelayed(() => { dialogToClose.DismissAllowingStateLoss(); }, App.WhatsNewDialogCloseTimeMs);
            }
        }


        private bool secondaryMenuInitiated = false;

    }

}


