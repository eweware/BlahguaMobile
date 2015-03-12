using System;
using System.Timers;
using System.ComponentModel;

//using Android.App;
using Android.Support.V4.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Util;

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
        private ProgressBar progress_main;
        public static GoogleAnalytics analytics = null;
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

			homeActivity = (HomeActivity)this.Activity;
			// create the fonts
			return fragment;
		}

		public void StartTimers()
        {
            if (!scrollTimer.Enabled)
            	scrollTimer.Start();
            BlahAnimateTimer.Start();
        }


		public void StopTimers()
        {
            scrollTimer.Stop();
            BlahAnimateTimer.Stop();
        }

        private static long fadeInDuration = 500;
        private static long fadeOutDuration = 2000;
        private static Random rnd = new Random();
        private View lastAnimatedBlah;

        private void MaybeAnimateElement()
        {
            View targetView = null;
            Rect scrollBounds = new Rect();
            
            List<View> targetList = new List<View>();
            BlahScroller.GetHitRect(scrollBounds);

            for (int curContainer = 0; curContainer < BlahContainerLayout.ChildCount; curContainer++)
            {
                BlahFrameLayout curContainerView = BlahContainerLayout.GetChildAt(curContainer) as BlahFrameLayout;
 
                for (int curBlahCount = 0; curBlahCount < curContainerView.ChildCount; curBlahCount++)
                {
                    View curBlahView = curContainerView.GetChildAt(curBlahCount);
                    var image = curBlahView.FindViewById<ImageView>(Resource.Id.image);

                    if (curBlahView.GetLocalVisibleRect(scrollBounds) && 
                        (curBlahView != lastAnimatedBlah) &&
                        (image.Tag != null))
                    {
                        targetList.Add(curBlahView);
                    }
                }
            }

            if (targetList.Count > 0)
                targetView = targetList[rnd.Next(targetList.Count)];

			if (targetView != null) {
				lastAnimatedBlah = targetView;
				var title = targetView.FindViewById<LinearLayout> (Resource.Id.textLayout);
				float targetAlpha = 0f;
                long fadeDuration = fadeOutDuration;
				if (title.Alpha == 0f)
                {
                    targetAlpha = 0.8f;
                    fadeDuration = fadeInDuration;
                }

				title.Animate ().Alpha (targetAlpha).SetDuration (fadeDuration);
			}

			BlahAnimateTimer.Start ();

        }

        void BlahAnimateTimer_Elapsed(object sender, EventArgs e)
        {
            BlahAnimateTimer.Stop();
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
                    FetchNextBlahList();
                }
            }
            
        }

		public override void OnResume()
        {
            base.OnResume();
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

					homeActivity.SetCreateButtonVisible (true);
                }
                else
                {

                }
            }
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
            }
        }
        private DateTime whatsNewTimestamp = DateTime.MinValue;
        private void ShowNewsFloater(WhatsNewInfo newInfo)
        {
            if (whatsNewTimestamp == DateTime.MinValue ||
                DateTime.Now - whatsNewTimestamp > TimeSpan.FromSeconds(5))
            {
                whatsNewTimestamp = DateTime.Now;
                var dialogToClose = WhatsNewDialog.ShowDialog(FragmentManager, newInfo);
                new Handler(Looper.MainLooper).PostDelayed(() => { dialogToClose.DismissAllowingStateLoss(); }, App.WhatsNewDialogCloseTimeMs);
            }
        }

		public void InsertBlahInStream(Blah theBlah)
		{
			View control = null;
			double scrollTop = BlahScroller.ScrollY + BlahScroller.Height;

			for (int i = 0; i < CurrentBlahContainer.ChildCount; i++)
			{
				control = CurrentBlahContainer.GetChildAt(i);
				if ((control.Top > scrollTop) &&
					(control.Height == mediumBlahSize) &&
					(control.Width == mediumBlahSize))
					break;
			}

			if (control != null)
			{
				// conform to view
				var title = control.FindViewById<TextView>(Resource.Id.title);

				if (String.IsNullOrEmpty(theBlah.T))
					control.FindViewById<LinearLayout>(Resource.Id.textLayout).Visibility = ViewStates.Invisible;
				else
				{
					double width = control.Width;
					double height = control.Height;
					control.FindViewById<LinearLayout>(Resource.Id.textLayout).Visibility = ViewStates.Visible;

					title.Text = theBlah.T;

					if (width == smallBlahSize && height == smallBlahSize)
					{
						title.SetTextSize(Android.Util.ComplexUnitType.Sp, 14);
					}
					else if (width == mediumBlahSize && height == smallBlahSize)
					{
						title.SetTextSize(Android.Util.ComplexUnitType.Sp, 18);
					}
					else if (width == mediumBlahSize && height == mediumBlahSize)
					{
						title.SetTextSize(Android.Util.ComplexUnitType.Sp, 24);
					}
					else if (width == largeBlahSize && height == mediumBlahSize)
					{
						title.SetTextSize(Android.Util.ComplexUnitType.Sp, 32);
					}
				}

				ImageView image = control.FindViewById<ImageView>(Resource.Id.image);
				image.Tag = null;
				if (theBlah.M != null)
				{
					image.Visibility = ViewStates.Visible;
					string imageBase = theBlah.M[0];
					string imageSize = "B";
					string imageURL = BlahguaAPIObject.Current.GetImageURL(imageBase, imageSize);
					homeActivity.RunOnUiThread(() =>
						{
							image.SetUrlDrawable(imageURL);
							if (!String.IsNullOrEmpty(theBlah.T))
							{
								image.Tag = true;   // animate this
								control.FindViewById<LinearLayout>(Resource.Id.textLayout).Alpha = 0.9f;
							}
						});
				}
				else
					image.Visibility = ViewStates.Invisible;

				homeActivity.RunOnUiThread(() =>
					{
						var type_mark = control.FindViewById<View>(Resource.Id.type_mark);
						var badges_mark = control.FindViewById<View>(Resource.Id.badges_mark);
						var hot_mark = control.FindViewById<View>(Resource.Id.hot_mark);
						var new_mark = control.FindViewById<View>(Resource.Id.new_mark);
						var user_mark = control.FindViewById<View>(Resource.Id.user_mark);

						switch (theBlah.TypeName)
						{
						case "says":
							type_mark.SetBackgroundResource(Resource.Drawable.say_icon);
							break;
						case "asks":
							type_mark.SetBackgroundResource(Resource.Drawable.ask_icon);
							break;
						case "leaks":
							type_mark.SetBackgroundResource(Resource.Drawable.leak_icon);
							break;
						case "polls":
							type_mark.SetBackgroundResource(Resource.Drawable.poll_icon);
							break;
						case "predicts":
							type_mark.SetBackgroundResource(Resource.Drawable.predict_icon);
							break;
						}

						// icons
						new_mark.Visibility = ViewStates.Visible;
						hot_mark.Visibility = ViewStates.Gone;

						if ((theBlah.B == null) || (theBlah.B.Count == 0))
							badges_mark.Visibility = ViewStates.Gone;
						else
							badges_mark.Visibility = ViewStates.Visible;

						user_mark.Visibility = ViewStates.Visible;
					});
				InboxBlah inboxItem = new InboxBlah(theBlah);
				control.Click += delegate
				{
					OpenBlahItem(inboxItem);
				};
			}

		}

    }

}


