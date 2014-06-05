using Android.Views;
using Android.Widget;

using BlahguaMobile.BlahguaCore;
using BlahguaMobile.AndroidClient.ThirdParty.UrlImageViewHelper;
using System;

namespace BlahguaMobile.AndroidClient.Screens
{
    public partial class MainActivity
    {
        private void RenderInitialBlahs()
        {
            int screenWidth = Resources.DisplayMetrics.WidthPixels - screenMargin * 2;

            double curTop = screenMargin;
            smallBlahSize = screenWidth / 3 - blahMargin; // (480 - ((screenMargin * 2) + (blahMargin * 3))) / 4;
            mediumBlahSize = smallBlahSize + smallBlahSize + blahMargin;
            largeBlahSize = smallBlahSize + smallBlahSize + smallBlahSize + blahMargin;

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

            ///////

            if (theBlah.M != null)
            {
                title.Visibility = ViewStates.Invisible;

                ImageView image = control.FindViewById<ImageView>(Resource.Id.image);
                string imageBase = theBlah.M[0];
                string imageSize = theBlah.ImageSize;
                string imageURL = BlahguaAPIObject.Current.GetImageURL(imageBase, imageSize);
                
                var type_mark = control.FindViewById<View>(Resource.Id.type_mark);
                var badges_mark = control.FindViewById<View>(Resource.Id.badges_mark);
                RunOnUiThread(() =>
                {
                    image.SetUrlDrawable(imageURL);
                    //image.SetImageURI(Android.Net.Uri.Parse(imageURL));
                    switch (theBlah.TypeName)
                    {
                        case "says":
                            type_mark.SetBackgroundColor(
                                new Android.Graphics.Color(207, 196, 182));
                            break;
                        case "asks":
                            type_mark.SetBackgroundColor(
                                new Android.Graphics.Color(255, 194, 84));
                            break;
                        case "leaks":
                            type_mark.SetBackgroundColor(
                                new Android.Graphics.Color(56, 76, 120));
                            break;
                        case "polls":
                            type_mark.SetBackgroundColor(Android.Graphics.Color.Red);
                            break;
                        case "predicts":
                            type_mark.SetBackgroundColor(Android.Graphics.Color.Purple);
                            break;
                    }

                    if (!String.IsNullOrEmpty(theBlah.B))
                    {
                        badges_mark.Visibility = ViewStates.Visible;
                    }
                    else
                    {
                        badges_mark.Visibility = ViewStates.Gone;
                    }
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
        #endregion
    }
}