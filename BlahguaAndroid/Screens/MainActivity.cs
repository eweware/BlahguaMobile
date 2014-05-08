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
            Button btn_menu = FindViewById<Button>(Resource.Id.btn_menu);
            btn_menu.Click += delegate
            {
				//button.Text = string.Format ("{0} clicks!", count++);
                SlidingMenu.ShowMenu(true);
			};

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

            ////// SLIDING MENU

            SetBehindContentView(Resource.Layout.sidemenu_sorting);
            SlidingMenu.SetSecondaryMenu(Resource.Layout.sidemenu_profile);
            SlidingMenu.ShadowWidthRes = Resource.Dimension.shadow_width;
            SlidingMenu.BehindOffsetRes = Resource.Dimension.slidingmenu_offset;
            SlidingMenu.FadeDegree = 0.25f;
            SlidingMenu.TouchModeAbove = TouchMode.Fullscreen;
            SlidingMenu.Mode = MenuMode.LeftRight;

            View leftMenu = SlidingMenu.GetMenu();
            View rightMenu = SlidingMenu.GetSecondaryMenu();

            avatar = rightMenu.FindViewById<ImageView>(Resource.Id.avatar);

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


            //if (null == bundle)
            //{
            //    var t = SupportFragmentManager.BeginTransaction();
            //    Frag = new SampleListFragment();
            //    t.Replace(Resource.Id.menu_frame, Frag);
            //    t.Commit();
            //}
            //else
            //    Frag =
            //        (ListFragment)
            //        SupportFragmentManager.FindFragmentById(Resource.Id.menu_frame);
        }

        private void list_Click(object sender, EventArgs e)
        {
            OnChannelChanged();
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
            smallBlahSize = screenWidth / 3 - blahMargin; // (480 - ((screenMargin * 2) + (blahMargin * 3))) / 4;
            mediumBlahSize = smallBlahSize + smallBlahSize + blahMargin;
            largeBlahSize = smallBlahSize + smallBlahSize + smallBlahSize + blahMargin;

            //foreach (int rowType in rowSequence)
            //{
            //    curTop = InsertRow(rowType, curTop);
            //    curTop += blahMargin;
            //}

            foreach (char rowType in sequence)
            {
                curTop = InsertRow(rowType, curTop);
                curTop += blahMargin;
            }



            //curTop = InsertAd(curTop);


            //BlahContainer.Height = curTop + screenMargin;
            inboxCounter = 1;//++;
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
        private void FetchNextBlahList()
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
                if (newBlahList != null)
                {
                    blahList = newBlahList;
                    blahList.PrepareBlahs();
                    InsertAdditionalBlahs();
                    AtScrollEnd = false;
                    inboxCounter++;
                    if (inboxCounter >= 5)
                    {
                        //UIElement curItem;
                        double bottom = 0;
                        // remove some blahs...
                        //for (int i = 0; i < 101; i++)
                        //{
                        //    curItem = BlahContainer.Children[0];
                        //    if (curItem is BlahRollItem)
                        //    {
                        //        BlahRollItem curBlah = (BlahRollItem)curItem;
                        //        AddImpression(curBlah.BlahData.I);
                        //    }

                        //    BlahContainer.Children.Remove(curItem);
                        //}
                        RunOnUiThread(() =>
                        {
                            BlahContainer.RemoveAllViews();
                        });

                        bottom = BlahContainer.Height;
                        //bottom = Canvas.GetTop(BlahContainer.Children[0]);

                        // now shift everything up
                        //foreach (UIElement theBlah in BlahContainer.Children)
                        //{
                        //    Canvas.SetTop(theBlah, Canvas.GetTop(theBlah) - bottom);
                        //}
                        //BlahScroller.ScrollToVerticalOffset(BlahScroller.VerticalOffset - bottom);
                        //BlahContainer.Height -= bottom;
                        //maxScroll -= (int)bottom;
                        inboxCounter--;
                    }

                }
                //App.analytics.PostPageView("/channel/" + BlahguaAPIObject.Current.CurrentChannel.ChannelName);

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

        private void InsertAdditionalBlahs()
        {
            double curTop = BlahScroller.ScrollY;

            foreach (char rowType in sequence)
            {
                curTop = InsertRow(rowType, curTop);
                curTop += blahMargin;
            }

            //curTop = InsertAd(curTop);

            //BlahContainer.Height = curTop + blahMargin;
        }

        private double InsertRow(char rowType, double topLoc)
        {
            double newTop = topLoc;
            switch (rowType)
            {//ABEAFADCADEACDAFAEBADADCAFABEAEBAFACDAEA
                case 'A':
                    newTop = InsertRowTypeA(topLoc);
                    break;
                case 'B':
                    newTop = InsertRowTypeB(topLoc);
                    break;
                case 'C':
                    newTop = InsertRowTypeC(topLoc);
                    break;
                case 'D':
                    newTop = InsertRowTypeD(topLoc);
                    break;
                case 'E':
                    newTop = InsertRowTypeE(topLoc);
                    break;
                case 'F':
                    newTop = InsertRowTypeF(topLoc);
                    break;
            }

            return newTop;
        }

        private double InsertRowTypeA(double topLoc)
        {
            double newTop = topLoc;
            double curLeft = screenMargin;
            InboxBlah nextBlah;

            for (int i = 0; i < 3; i++)
            {
                nextBlah = blahList.PopBlah(4);
                InsertElementForBlah(nextBlah, curLeft, topLoc, smallBlahSize, smallBlahSize);
                curLeft += smallBlahSize + blahMargin;
            }
            newTop += smallBlahSize;

            return newTop;
        }

        private double InsertRowTypeB(double topLoc)
        {
            double newTop = topLoc;
            double curLeft = screenMargin;
            InboxBlah nextBlah = blahList.PopBlah(4);
            InsertElementForBlah(nextBlah, curLeft, topLoc, smallBlahSize, smallBlahSize);
            nextBlah = blahList.PopBlah(4);
            InsertElementForBlah(nextBlah, curLeft, topLoc + smallBlahSize + blahMargin, smallBlahSize, smallBlahSize);
            nextBlah = blahList.PopBlah(2);
            curLeft += smallBlahSize + blahMargin;
            InsertElementForBlah(nextBlah, curLeft, topLoc, mediumBlahSize, mediumBlahSize);

            newTop += mediumBlahSize;

            return newTop;
        }

        private double InsertRowTypeC(double topLoc)
        {
            double newTop = topLoc;
            double curLeft = screenMargin;
            InboxBlah nextBlah = blahList.PopBlah(2);
            InsertElementForBlah(nextBlah, curLeft, topLoc, mediumBlahSize, mediumBlahSize);
            curLeft += mediumBlahSize + blahMargin;
            nextBlah = blahList.PopBlah(4);
            InsertElementForBlah(nextBlah, curLeft, topLoc, smallBlahSize, smallBlahSize);
            nextBlah = blahList.PopBlah(4);
            InsertElementForBlah(nextBlah, curLeft, topLoc + smallBlahSize + blahMargin, smallBlahSize, smallBlahSize);

            newTop += mediumBlahSize;

            return newTop;
        }

        private double InsertRowTypeD(double topLoc)
        {
            double newTop = topLoc;
            double curLeft = screenMargin;
            InboxBlah nextBlah = blahList.PopBlah(4);
            InsertElementForBlah(nextBlah, curLeft, topLoc, smallBlahSize, smallBlahSize);
            curLeft += smallBlahSize + blahMargin;
            nextBlah = blahList.PopBlah(3);
            InsertElementForBlah(nextBlah, curLeft, topLoc, mediumBlahSize, smallBlahSize);

            newTop += smallBlahSize;

            return newTop;
        }

        private double InsertRowTypeE(double topLoc)
        {
            double newTop = topLoc;
            double curLeft = screenMargin;
            InboxBlah nextBlah = blahList.PopBlah(3);
            InsertElementForBlah(nextBlah, curLeft, topLoc, mediumBlahSize, smallBlahSize);
            curLeft += mediumBlahSize + blahMargin;
            nextBlah = blahList.PopBlah(4);
            InsertElementForBlah(nextBlah, curLeft, topLoc, smallBlahSize, smallBlahSize);

            newTop += smallBlahSize;

            return newTop;
        }

        private double InsertRowTypeF(double topLoc)
        {
            double newTop = topLoc;
            double curLeft = screenMargin;
            InboxBlah nextBlah = blahList.PopBlah(1);
            InsertElementForBlah(nextBlah, curLeft, topLoc, largeBlahSize, mediumBlahSize);

            newTop += mediumBlahSize;

            return newTop;
        }

        private void InsertElementForBlah(InboxBlah theBlah, double xLoc, double yLoc, double width, double height)
        {
            RelativeLayout.LayoutParams layoutParams = new RelativeLayout.LayoutParams((int)width, (int)height);
            layoutParams.SetMargins((int)xLoc, (int)yLoc, 0, 0);
            // TODO visual interpretatuion of a blah

            var control = LayoutInflater.Inflate(Resource.Layout.uiitem_blah, null);
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

            control.Click += delegate
            {
                OpenBlahItem(theBlah);
            };

            RunOnUiThread(() =>
            {
                BlahContainer.AddView(control);
            });

        }

        void OpenBlahItem(InboxBlah curBlah)
        {
            StopTimers();
            BlahguaAPIObject.Current.CurrentInboxBlah = curBlah;
            App.BlahIdToOpen = curBlah.I;
            StartActivity(typeof(ViewPostActivity));

        }

        #endregion
    }
}


