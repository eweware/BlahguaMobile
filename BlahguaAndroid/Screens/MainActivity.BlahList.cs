using Android.Views;
using Android.Widget;

using BlahguaMobile.BlahguaCore;
using BlahguaMobile.AndroidClient.ThirdParty.UrlImageViewHelper;
using System;
using Android.Views.Animations;
using Android.Animation;
using Android.Graphics;
using BlahguaMobile.AndroidClient.Adapters;

namespace BlahguaMobile.AndroidClient.Screens
{
    public partial class MainActivity
    {
        private readonly int BlahSetsAmountToRemove = 3;
        int blahsToAdd = 0;
		private Typeface blahRollFont = null;

        private void RenderInitialBlahs()
        {
            CurrentBlahContainer = new BlahFrameLayout(this);
            FrameLayout.LayoutParams layoutparams =
                new FrameLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent,
                    ViewGroup.LayoutParams.WrapContent);
            CurrentBlahContainer.LayoutParameters = layoutparams;

            blahsToAdd = 100;

            int screenWidth = Resources.DisplayMetrics.WidthPixels - screenMargin * 2;

            double curTop = screenMargin;
			smallBlahSize = (screenWidth - (blahMargin * 2)) / 3;// - blahMargin; // (480 - ((screenMargin * 2) + (blahMargin * 3))) / 4;
			mediumBlahSize = (smallBlahSize * 2) + blahMargin;
			largeBlahSize = (smallBlahSize * 3) + (blahMargin * 2);

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
                    RunOnUiThread(() =>
                    {
                        Toast.MakeText(this, "Well, thanks for trying.  Looks like there is a server issue.  Please go ahead and leave the app and try again later.", ToastLength.Long).Show();
                        //MessageBox.Show("Well, thanks for trying.  Looks like there is a server issue.  Please go ahead and leave the app and try again later.");
                        //LoadingBox.Visibility = Visibility.Collapsed;
                        //ConnectFailure.Visibility = Visibility.Visible;
                    });
                }

                StartTimers();
            });
        }

        private void FinishedLoadingCurrentBlahContainer()
        {
            BlahContainerLayout.AddView(CurrentBlahContainer);
            if (inboxCounter > BlahSetsAmountToRemove)
            {
                int heightToShift = BlahContainerLayout.GetChildAt(0).MeasuredHeight;
                BlahContainerLayout.RemoveViewAt(0);
                BlahScroller.ScrollTo(0, BlahScroller.ScrollY - heightToShift);

                inboxCounter--;
            }
            StartTimers();
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
            StopTimers();
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
                }
                MainActivity.analytics.PostPageView("/channel/" + BlahguaAPIObject.Current.CurrentChannel.ChannelName);

            });
        }

        private void ClearBlahs()
        {
            RunOnUiThread(() =>
            {
                inboxCounter = 0;
                BlahContainerLayout.RemoveAllViews();
                BlahScroller.ScrollTo(0, 0);
            });
        }

        private void OpenBlahItem(InboxBlah curBlah)
        {
            StopTimers();
            BlahguaAPIObject.Current.CurrentInboxBlah = curBlah;
            App.BlahIdToOpen = curBlah.I;
            StartActivity(typeof(ViewPostActivity));

        }

        #region InsertRows
        private void InsertAdditionalBlahs()
        {
            CurrentBlahContainer = new BlahFrameLayout(this);
            FrameLayout.LayoutParams layoutparams =
                new FrameLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent,
                    ViewGroup.LayoutParams.WrapContent);
	
            CurrentBlahContainer.LayoutParameters = layoutparams;

            blahsToAdd = 100;

			double curTop = blahMargin;// BlahScroller.ScrollY + BlahScroller.MeasuredHeight;// + Resources.DisplayMetrics.HeightPixels;

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
            Console.WriteLine("xLoc:" + xLoc.ToString() + ", yLoc:" + yLoc.ToString() + ", width:" + width.ToString() + ", height:" + height.ToString());
            RelativeLayout.LayoutParams layoutParams = new RelativeLayout.LayoutParams((int)width, (int)height);
            layoutParams.SetMargins((int)xLoc, (int)yLoc, 0, 0);
            // TODO visual interpretatuion of a blah

            var control = LayoutInflater.Inflate(Resource.Layout.uiitem_blah, null);
            var title = control.FindViewById<TextView>(Resource.Id.title);

            control.LayoutParameters = layoutParams;

            if (String.IsNullOrEmpty(theBlah.T))
                control.FindViewById<LinearLayout>(Resource.Id.textLayout).Visibility = ViewStates.Invisible;
            else
            {
                control.FindViewById<LinearLayout>(Resource.Id.textLayout).Visibility = ViewStates.Visible ;
                if (blahRollFont == null)
				blahRollFont = Typeface.CreateFromAsset (this.ApplicationContext.Assets, "fonts/GothamRounded-Book.otf");
			    title.SetTypeface (blahRollFont, TypefaceStyle.Normal);

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

            /////// image loading ///////
            ImageView image = control.FindViewById<ImageView>(Resource.Id.image);
            image.Tag = null;
            if (theBlah.M != null)
            {
                image.Visibility = ViewStates.Visible;
                string imageBase = theBlah.M[0];
                string imageSize = theBlah.ImageSize;
                string imageURL = BlahguaAPIObject.Current.GetImageURL(imageBase, imageSize);
                RunOnUiThread(() =>
                {
                    image.SetUrlDrawable(imageURL);
                    //image.SetScaleType(ImageView.ScaleType.FitStart);
                    if (!String.IsNullOrEmpty(theBlah.T))
                    {
                        image.Tag = true;   // animate this
                        control.FindViewById<LinearLayout>(Resource.Id.textLayout).Alpha = 0.9f;
                    }
                });
            }
            else
                image.Visibility = ViewStates.Invisible;


            ///////
            RunOnUiThread(() =>
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
                double currentUtc = DateTime.Now.ToUniversalTime().Subtract(
                                new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                                ).TotalMilliseconds;
                if (currentUtc - theBlah.c < 86400000)
                    new_mark.Visibility = ViewStates.Visible;
                else
                    new_mark.Visibility = ViewStates.Gone;

                if (theBlah.RR)
                    hot_mark.Visibility = ViewStates.Visible;
                else
                    hot_mark.Visibility = ViewStates.Gone;

                if (!String.IsNullOrEmpty(theBlah.B))
                    badges_mark.Visibility = ViewStates.Visible;
                else
                    badges_mark.Visibility = ViewStates.Gone;

                if ((BlahguaAPIObject.Current.CurrentUser != null) &&
                    (BlahguaAPIObject.Current.CurrentUser._id == theBlah.A))
                    user_mark.Visibility = ViewStates.Visible;
                else
                    user_mark.Visibility = ViewStates.Gone;
            });

            control.Click += delegate
            {
                OpenBlahItem(theBlah);
            };

            RunOnUiThread(() =>
            {
                CurrentBlahContainer.AddView(control);

                blahsToAdd--;

                if (blahsToAdd == 0)
                {
                    FinishedLoadingCurrentBlahContainer();
                }
            });
        }
        #endregion
    }
}