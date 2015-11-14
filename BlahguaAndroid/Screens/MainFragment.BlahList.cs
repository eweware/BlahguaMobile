using Android.Views;
using Android.Widget;
using Android.Util;

using BlahguaMobile.BlahguaCore;
using BlahguaMobile.AndroidClient.ThirdParty.UrlImageViewHelper;
using System;
using Android.Views.Animations;
using Android.Animation;
using Android.Graphics;
using BlahguaMobile.AndroidClient.Adapters;
using System.Collections.Generic;

namespace BlahguaMobile.AndroidClient.Screens
{
	public partial class MainFragment
	{
		private readonly int BlahSetsAmountToRemove = 3;
		int blahsToAdd = 0;


		private void RenderInitialBlahs()
		{
			CurrentBlahContainer = new BlahFrameLayout(this.Activity);
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
            BlahguaAPIObject.smallTileSize = (int)smallBlahSize;
            BlahguaAPIObject.mediumTileSize = (int)mediumBlahSize;
            BlahguaAPIObject.largeTileSize = (int)largeBlahSize;

            // lets not be crazy...
            if (BlahguaAPIObject.smallTileSize > 256)
            {
                BlahguaAPIObject.smallTileSize /= 2;
                BlahguaAPIObject.mediumTileSize /= 2;
                BlahguaAPIObject.largeTileSize /= 2;
            }

			foreach (char rowType in sequence)
			{
				curTop = InsertRow(rowType, curTop);
				curTop += blahMargin;
			}

			//curTop = InsertAd(curTop);

			AtScrollEnd = false;
			inboxCounter = 1;//++;
		}
		public void FetchInitialBlahs ()
		{
			FetchInitialBlahList ();
		}

		public void ShowBlahActivity(string blahId)
		{
			for (int i = 0; i < BlahContainerLayout.ChildCount; i++) {
				BlahFrameLayout curFrame = BlahContainerLayout.GetChildAt (i) as BlahFrameLayout;

				for (int curItem = 0; curItem < curFrame.ChildCount; curItem++) {
					FrameLayout curBlahItem = curFrame.GetChildAt (curItem) as FrameLayout;
					string curTag = (string)curBlahItem.Tag;

					if (curTag == blahId) {
						AnimateBlahActivity (curBlahItem); 
					}
				}
			}
		}

		private void AnimateBlahActivity(FrameLayout curBlahItem)
		{
            var hot_mark = curBlahItem.FindViewById<View>(Resource.Id.hot_mark);
            this.Activity.RunOnUiThread(() =>
            {
                hot_mark.Visibility = ViewStates.Visible;
                hot_mark.Alpha = 1.0f;
                hot_mark.Animate()
                    .Alpha(0.0f)
                    .SetDuration(fadeOutDuration);
            });
        }


		private void FetchInitialBlahList(bool secondTry = false)
		{
			this.Activity.RunOnUiThread(() =>
				{
					//Toast.MakeText(this, "loading...", ToastLength.Short).Show();
					progress_main.Visibility = ViewStates.Visible;
				});
			//LoadingMessageBox.Text = "loading...";
			loadTimer.Stop();
			loadTimer.Interval = 100000;// TimeSpan.FromSeconds(10);
			loadTimer.Elapsed += (theObj, theArgs) =>
			{
				this.Activity.RunOnUiThread(() =>
					{
						Toast.MakeText(this.Activity, "still loading...", ToastLength.Short).Show();
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
                        this.Activity.RunOnUiThread(() =>
                            {
                                RenderInitialBlahs();
                                StartTimers();
                            });
						//LoadingBox.Visibility = Visibility.Collapsed;
					}
					else if (!secondTry)
					{

						this.Activity.RunOnUiThread(() =>
							{
								Toast.MakeText(this.Activity, "We had a problem loading. Retrying load...", ToastLength.Long).Show();
							});
						FetchInitialBlahList(true);
					}
					else
					{
						progress_main.Visibility = ViewStates.Gone;
						InsertEmptyChannelWarning();
					}


				});
		}

		private void FinishedLoadingCurrentBlahContainer()
		{
			progress_main.Visibility = ViewStates.Gone;

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
			this.Activity.RunOnUiThread(() =>
				{
					Toast.MakeText(this.Activity, "loading...", ToastLength.Short).Show();
				});
			//LoadingMessageBox.Text = "loading...";
			loadTimer.Stop();
			loadTimer.Interval = 100000;// TimeSpan.FromSeconds(10);
			loadTimer.Elapsed += (theObj, theArgs) =>
			{
				this.Activity.RunOnUiThread(() =>
					{
						Toast.MakeText(this.Activity, "still loading...", ToastLength.Short).Show();
					});
				//LoadingMessageBox.Text = "still loading...";
			};
			loadTimer.Start();
			StopTimers();
			BlahguaAPIObject.Current.GetInbox((newBlahList) =>
				{
					loadTimer.Stop();
					if ((newBlahList == null) || (newBlahList.Count == 0))
					{
						InsertEmptyChannelWarning();
					}
					else
					{
						blahList = newBlahList;
						blahList.PrepareBlahs();
						InsertAdditionalBlahs();
						AtScrollEnd = false;
						inboxCounter++;
					}

					HomeActivity.analytics.PostPageView("/channel/" + BlahguaAPIObject.Current.CurrentChannel.ChannelName);

				});
		}

		private void InsertEmptyChannelWarning()
		{
			FrameLayout.LayoutParams layoutParams =
				new FrameLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent,
					ViewGroup.LayoutParams.WrapContent);

			FrameLayout.LayoutParams layoutParams2 =
				new FrameLayout.LayoutParams(Resources.DisplayMetrics.WidthPixels, Resources.DisplayMetrics.HeightPixels);

			var control = this.Activity.LayoutInflater.Inflate(Resource.Layout.empty_channel_warning, null);
			var title = control.FindViewById<TextView>(Resource.Id.title);
			title.SetTypeface(HomeActivity.gothamFont, TypefaceStyle.Normal);
			control.LayoutParameters = layoutParams;
			BlahContainerLayout.LayoutParameters = layoutParams2;


			this.Activity.RunOnUiThread(() =>
				{
					progress_main.Visibility = ViewStates.Gone;
					inboxCounter = 0;
					BlahContainerLayout.RemoveAllViews();
					BlahContainerLayout.AddView(control);
					BlahScroller.ScrollTo(0, 0);

				});

		}
		public void ClearBlahs()
		{
            if (this.Activity != null)
            {
                this.Activity.RunOnUiThread(() =>
                    {
                        inboxCounter = 0;
                        BlahContainerLayout.RemoveAllViews();
                        BlahScroller.ScrollTo(0, 0);
                    });
            }
            else
            {
                inboxCounter = 0;
                BlahContainerLayout.RemoveAllViews();
                BlahScroller.ScrollTo(0, 0);
            }
    }
		private void OpenBlahItem(InboxBlah curBlah)
		{
		    StopTimers();
			BlahguaAPIObject.Current.CurrentInboxBlah = curBlah;
			App.BlahIdToOpen = curBlah.I;
			this.Activity.StartActivity(typeof(ViewPostActivity));

		}

        public void OpenBlahFromId(string blahId)
        {
            StopTimers();
            BlahguaAPIObject.Current.CurrentInboxBlah = null;
            App.BlahIdToOpen = blahId;
            this.Activity.StartActivity(typeof(ViewPostActivity));

        }


		#region InsertRows
		private void InsertAdditionalBlahs()
		{
			CurrentBlahContainer = new BlahFrameLayout(this.Activity);
			FrameLayout.LayoutParams layoutparams =
				new FrameLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent,
					ViewGroup.LayoutParams.WrapContent);

			CurrentBlahContainer.LayoutParameters = layoutparams;

			blahsToAdd = 100;

			double curTop = blahMargin;

			foreach (char rowType in sequence)
			{
				curTop = InsertRow(rowType, curTop);
				curTop += blahMargin;
			}

		}

		private double InsertRow(char rowType, double topLoc)
		{
			double newTop = topLoc;
			switch (rowType)
			{
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

			var control = this.Activity.LayoutInflater.Inflate(Resource.Layout.uiitem_blah, null);
			var title = control.FindViewById<TextView>(Resource.Id.title);
			control.Tag = theBlah.I;

			control.LayoutParameters = layoutParams;

			if (String.IsNullOrEmpty(theBlah.T))
				control.FindViewById<LinearLayout>(Resource.Id.textLayout).Visibility = ViewStates.Invisible;
			else
			{
				control.FindViewById<LinearLayout>(Resource.Id.textLayout).Visibility = ViewStates.Visible ;
				title.SetTypeface (HomeActivity.gothamFont, TypefaceStyle.Normal);

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
				this.Activity.RunOnUiThread(() =>
					{
						image.SetUrlDrawable(imageURL);
						if (!String.IsNullOrEmpty(theBlah.T))
						{
							image.Tag = true;   // animate this
							control.FindViewById<LinearLayout>(Resource.Id.textLayout).Alpha = .8f;
						}
					});
			}
			else
				image.Visibility = ViewStates.Invisible;


			///////
			this.Activity.RunOnUiThread(() =>
				{
					var type_mark = control.FindViewById<View>(Resource.Id.type_mark);
					var badges_mark = control.FindViewById<View>(Resource.Id.badges_mark);
					var hot_mark = control.FindViewById<View>(Resource.Id.hot_mark);
					var new_mark = control.FindViewById<View>(Resource.Id.new_mark);
					var user_mark = control.FindViewById<View>(Resource.Id.user_mark);

                    if (BlahguaAPIObject.Current.CurrentChannel.SSA == false)
                        type_mark.Visibility = ViewStates.Gone;
                    else
                    {
                        type_mark.Visibility = ViewStates.Visible;
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
                Animation myAnimation = AnimationUtils.LoadAnimation(this.Activity, Resource.Animation.fade_spin);
                control.StartAnimation(myAnimation);
				OpenBlahItem(theBlah);
			};

			this.Activity.RunOnUiThread(() =>
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