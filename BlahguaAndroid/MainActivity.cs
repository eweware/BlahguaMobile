using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Util;
using Android.Content.PM;
using Android.Support.V4.Widget;
using BlahguaMobile.BlahguaCore;
using System.Timers;
using System.ComponentModel;
using Android.Support.V4.App;
using Android.Graphics.Drawables;
using System;
using BlahguaMobile.AndroidClient.ThirdParty.UrlImageViewHelper;
using SlidingMenuSharp.App;
using SlidingMenuSharp;

namespace BlahguaMobile.AndroidClient
{
	[Activity (MainLauncher = true)]
    public class MainActivity : SlidingFragmentActivity
    {
        Inbox blahList;
        Timer loadTimer = new Timer();
        Timer scrollTimer = new Timer();
        int inboxCounter = 0;

        int FramesPerSecond = 240;

        int screenMargin = 0;//24;
        int blahMargin = 0;//12;
        double smallBlahSize, mediumBlahSize, largeBlahSize;
        int[] rowSequence = new int[] { 4, 32, 31, 4, 1, 33, 4, 2, 4, 32, 1, 4, 31, 32, 33, 31, 4, 33, 1, 31, 4, 32, 33, 1, 4, 2 };

		int count = 1;
		int count2 = 1;

        private RelativeLayout BlahContainer = null;
        private ScrollView BlahScroller = null;

        protected ListFragment Frag;

        private DrawerLayout m_Drawer;
        private ListView m_DrawerList;

		protected override void OnCreate (Bundle bundle)
		{
            base.OnCreate(bundle);

            RequestWindowFeature(WindowFeatures.NoTitle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.MainScreen);

            BlahContainer = FindViewById<RelativeLayout>(Resource.Id.BlahContainer);
            BlahScroller = FindViewById<ScrollView>(Resource.Id.BlahScroller);
			//AddBlahs();

			// Get our button from the layout resource,
			// and attach an event to it
            Button btn_menu = FindViewById<Button>(Resource.Id.btn_menu);
            btn_menu.Click += delegate
            {
				//button.Text = string.Format ("{0} clicks!", count++);
                SlidingMenu.ShowMenu(true);
			};

            Button btn_login = FindViewById<Button>(Resource.Id.btn_login);
            btn_login.Click += delegate
            {
				//button.Text = string.Format ("{0} clicks!", count2++);
                StartActivity(typeof(LoginActivity));
			};


            ////// SLIDING MENU

            SetBehindContentView(Resource.Layout.menu_frame);
            SlidingMenu.SetSecondaryMenu(Resource.Layout.menu_frame_two);
            SlidingMenu.ShadowWidthRes = Resource.Dimension.shadow_width;
            SlidingMenu.BehindOffsetRes = Resource.Dimension.slidingmenu_offset;
            SlidingMenu.FadeDegree = 0.25f;
            SlidingMenu.TouchModeAbove = TouchMode.Fullscreen;
            SlidingMenu.Mode = MenuMode.LeftRight;
            if (null == bundle)
            {
                var t = SupportFragmentManager.BeginTransaction();
                Frag = new SampleListFragment();
                t.Replace(Resource.Id.menu_frame, Frag);
                t.Commit();
            }
            else
                Frag =
                    (ListFragment)
                    SupportFragmentManager.FindFragmentById(Resource.Id.menu_frame);
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
            //DetectScrollAtEnd();

        }

        protected override void OnStart()
        {
            base.OnStart();

            scrollTimer.Interval = (TimeSpan.TicksPerSecond / FramesPerSecond) / 1000;
            scrollTimer.Elapsed += ScrollBlahRoll;

            BlahguaAPIObject.Current.PropertyChanged += new PropertyChangedEventHandler(On_API_PropertyChanged);
            InitService();

            StartTimers();
        }
        void On_API_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "CurrentChannel":
                    OnChannelChanged();
                    break;
            }
        }
        void OnChannelChanged()
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

        void DoServiceInited(bool didIt)
        {
            loadTimer.Stop();
            if (didIt)
            {
                if (BlahguaAPIObject.Current.CurrentUser != null)
                {
                    //App.analytics.PostAutoLogin();
                    //UserInfoBtn.Visibility = Visibility.Visible;
                    //NewBlahBtn.Visibility = Visibility.Visible;
                    //SignInBtn.Visibility = Visibility.Collapsed;
                }
                else
                {
                    //UserInfoBtn.Visibility = Visibility.Collapsed;
                    //NewBlahBtn.Visibility = Visibility.Collapsed;
                    //SignInBtn.Visibility = Visibility.Visible;
                }
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
        #endregion

        private void RenderInitialBlahs()
        {
            int screenWidth = Resources.DisplayMetrics.WidthPixels - screenMargin * 2;

            double curTop = screenMargin;
            smallBlahSize = screenWidth / 4 - blahMargin; // (480 - ((screenMargin * 2) + (blahMargin * 3))) / 4;
            mediumBlahSize = smallBlahSize + smallBlahSize + blahMargin;
            largeBlahSize = mediumBlahSize + mediumBlahSize + blahMargin;

            foreach (int rowType in rowSequence)
            {
                curTop = InsertRow(rowType, curTop);
                curTop += blahMargin;
            }



            //curTop = InsertAd(curTop);


            //BlahContainer.Height = curTop + screenMargin;
            inboxCounter++;
        }

        private void FetchInitialBlahList(bool secondTry = false)
        {
            RunOnUiThread(() =>
            {
                Toast.MakeText(this, "loading...", ToastLength.Short).Show();
            });
            //LoadingMessageBox.Text = "loading...";
            loadTimer.Stop();
            loadTimer.Interval = 100000;// TimeSpan.FromSeconds(10);
            loadTimer.Elapsed += (theObj, theArgs) =>
            {
                RunOnUiThread(() =>
                {
                    Toast.MakeText(this, "still loading...", ToastLength.Short).Show();
                });
                //LoadingMessageBox.Text = "still loading...";
            };
            loadTimer.Start();

            BlahguaAPIObject.Current.GetInbox((newBlahList) =>
            {
                loadTimer.Stop();
                if (newBlahList == null)
                    newBlahList = new Inbox();
                blahList = newBlahList;
                blahList.PrepareBlahs();
                if (blahList.Count == 100)
                {
                    RenderInitialBlahs();
                    //StartTimers();
                    //LoadingBox.Visibility = Visibility.Collapsed;
                }
                else if (!secondTry)
                {

                    RunOnUiThread(() =>
                    {
                        //MessageBox.Show("We had a problem loading.  Press OK and we will try it again.");
                        Toast.MakeText(this, "We had a problem loading. Retrying load...", ToastLength.Long).Show();
                        //LoadingMessageBox.Text = "retrying load...";
                    });
                    FetchInitialBlahList(true);
                }
                else
                {
                    //MessageBox.Show("Well, thanks for trying.  Looks like there is a server issue.  Please go ahead and leave the app and try again later.");
                    //LoadingBox.Visibility = Visibility.Collapsed;
                    //ConnectFailure.Visibility = Visibility.Visible;
                }

                StartTimers();
            });
        }

        private void ClearBlahs()
        {
            RunOnUiThread(() =>
            {
                inboxCounter = 0;
                BlahContainer.RemoveAllViews();
                BlahScroller.ScrollTo(0, 0);
            });
        }

        #region InsertRows

        private double InsertRow(int rowType, double topLoc)
        {
            double newTop = topLoc;
            switch (rowType)
            {
                case 1:
                    newTop = InsertRowType1(topLoc);
                    break;
                case 2:
                    newTop = InsertRowType2(topLoc);
                    break;
                case 31:
                    newTop = InsertRowType31(topLoc);
                    break;
                case 32:
                    newTop = InsertRowType32(topLoc);
                    break;
                case 33:
                    newTop = InsertRowType33(topLoc);
                    break;
                case 4:
                    newTop = InsertRowType4(topLoc);
                    break;
            }

            return newTop;
        }

        private double InsertRowType1(double topLoc)
        {
            double newTop = topLoc;
            InboxBlah nextBlah = blahList.PopBlah(1);
            InsertElementForBlah(nextBlah, screenMargin, topLoc, largeBlahSize, mediumBlahSize);
            newTop += mediumBlahSize;


            return newTop;
        }

        private double InsertRowType2(double topLoc)
        {
            double newTop = topLoc;
            double curLeft = screenMargin;
            InboxBlah nextBlah = blahList.PopBlah(2);
            InsertElementForBlah(nextBlah, curLeft, topLoc, mediumBlahSize, mediumBlahSize);
            curLeft += mediumBlahSize + blahMargin;
            nextBlah = blahList.PopBlah(2);
            InsertElementForBlah(nextBlah, curLeft, topLoc, mediumBlahSize, mediumBlahSize);
            newTop += mediumBlahSize;

            return newTop;
        }


        private double InsertRowType31(double topLoc)
        {
            double newTop = topLoc;
            double curLeft = screenMargin;
            InboxBlah nextBlah = blahList.PopBlah(2);
            InsertElementForBlah(nextBlah, curLeft, topLoc, mediumBlahSize, mediumBlahSize);
            curLeft += mediumBlahSize + blahMargin;
            nextBlah = blahList.PopBlah(3);
            InsertElementForBlah(nextBlah, curLeft, topLoc, smallBlahSize, smallBlahSize);
            nextBlah = blahList.PopBlah(3);
            InsertElementForBlah(nextBlah, curLeft, topLoc + smallBlahSize + blahMargin, smallBlahSize, smallBlahSize);
            curLeft += smallBlahSize + blahMargin;
            nextBlah = blahList.PopBlah(3);
            InsertElementForBlah(nextBlah, curLeft, topLoc, smallBlahSize, smallBlahSize);
            nextBlah = blahList.PopBlah(3);
            InsertElementForBlah(nextBlah, curLeft, topLoc + smallBlahSize + blahMargin, smallBlahSize, smallBlahSize);

            newTop += mediumBlahSize;

            return newTop;
        }


        private double InsertRowType32(double topLoc)
        {
            double newTop = topLoc;
            double curLeft = screenMargin;
            InboxBlah nextBlah;
            nextBlah = blahList.PopBlah(3);
            InsertElementForBlah(nextBlah, curLeft, topLoc, smallBlahSize, smallBlahSize);
            nextBlah = blahList.PopBlah(3);
            InsertElementForBlah(nextBlah, curLeft, topLoc + smallBlahSize + blahMargin, smallBlahSize, smallBlahSize);
            curLeft += smallBlahSize + blahMargin;

            nextBlah = blahList.PopBlah(2);
            InsertElementForBlah(nextBlah, curLeft, topLoc, mediumBlahSize, mediumBlahSize);
            curLeft += mediumBlahSize + blahMargin;

            nextBlah = blahList.PopBlah(3);
            InsertElementForBlah(nextBlah, curLeft, topLoc, smallBlahSize, smallBlahSize);
            nextBlah = blahList.PopBlah(3);
            InsertElementForBlah(nextBlah, curLeft, topLoc + smallBlahSize + blahMargin, smallBlahSize, smallBlahSize);


            newTop += mediumBlahSize;

            return newTop;
        }


        private double InsertRowType33(double topLoc)
        {
            double newTop = topLoc;
            double curLeft = screenMargin;
            InboxBlah nextBlah;
            nextBlah = blahList.PopBlah(3);
            InsertElementForBlah(nextBlah, curLeft, topLoc, smallBlahSize, smallBlahSize);
            nextBlah = blahList.PopBlah(3);
            InsertElementForBlah(nextBlah, curLeft, topLoc + smallBlahSize + blahMargin, smallBlahSize, smallBlahSize);
            curLeft += smallBlahSize + blahMargin;

            nextBlah = blahList.PopBlah(3);
            InsertElementForBlah(nextBlah, curLeft, topLoc, smallBlahSize, smallBlahSize);
            nextBlah = blahList.PopBlah(3);
            InsertElementForBlah(nextBlah, curLeft, topLoc + smallBlahSize + blahMargin, smallBlahSize, smallBlahSize);
            curLeft += smallBlahSize + blahMargin;

            nextBlah = blahList.PopBlah(2);
            InsertElementForBlah(nextBlah, curLeft, topLoc, mediumBlahSize, mediumBlahSize);
            curLeft += mediumBlahSize + blahMargin;

            newTop += mediumBlahSize;
            return newTop;
        }


        private double InsertRowType4(double topLoc)
        {
            double newTop = topLoc;
            double curLeft = screenMargin;
            InboxBlah nextBlah;

            for (int i = 0; i < 4; i++)
            {

                nextBlah = blahList.PopBlah(3);
                InsertElementForBlah(nextBlah, curLeft, topLoc, smallBlahSize, smallBlahSize);
                curLeft += smallBlahSize + blahMargin;
            }
            newTop += smallBlahSize;

            return newTop;
        }

        private void InsertElementForBlah(InboxBlah theBlah, double xLoc, double yLoc, double width, double height)
        {
            RelativeLayout.LayoutParams layoutParams = new RelativeLayout.LayoutParams((int)width, (int)height);
            layoutParams.SetMargins((int)xLoc, (int)yLoc, 0, 0);
            // TODO visual interpretatuion of a blah

            var control = LayoutInflater.Inflate(Resource.Layout.ControlBlahRollItem, null);
            var title = control.FindViewById<TextView>(Resource.Id.title);
            
            //control.SetBackgroundColor(new global::Android.Graphics.Color(100, 100, 100));
            control.LayoutParameters = layoutParams;

            title.Text = theBlah.T;

            ///////

            if (theBlah.M != null)
            {
                title.Visibility = ViewStates.Invisible;

                ImageView image = control.FindViewById<ImageView>(Resource.Id.image);
                string imageBase = theBlah.M[0];
                string imageSize = theBlah.ImageSize;
                string imageURL = BlahguaAPIObject.Current.GetImageURL(imageBase, imageSize);

                RunOnUiThread(() =>
                {
                    image.SetUrlDrawable(imageURL);
                    //image.SetImageURI(Android.Net.Uri.Parse(imageURL));
                });
            }
            ///////


            RunOnUiThread(() =>
            {
                BlahContainer.AddView(control);
            });

        }

        #endregion
    }
}


