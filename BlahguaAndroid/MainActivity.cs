using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Util;

namespace BlahguaMobile.Android
{
	[Activity (Label = "blahgua", MainLauncher = true)]
	public class MainActivity : Activity
	{
		int count = 1;
		int count2 = 1;

		private RelativeLayout BlahView = null;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.MainScreen);
			BlahView = FindViewById<RelativeLayout> (Resource.Id.BlahContainer);
			AddBlahs ();

			// Get our button from the layout resource,
			// and attach an event to it
			Button button = FindViewById<Button> (Resource.Id.channelSelectBtn);
			
			button.Click += delegate {
				button.Text = string.Format ("{0} clicks!", count++);
			};

			button = FindViewById<Button> (Resource.Id.signInBtn);

			button.Click += delegate {
				button.Text = string.Format ("{0} clicks!", count2++);
			};
		}

		private void AddBlahs()
		{
			int numRows = 20;
			DisplayMetrics metrics = Resources.DisplayMetrics;


			int screenWidth = metrics.WidthPixels;

			double margin = 8;
			double gutter = 0;
			double parentWidth = screenWidth;
			double blahWidth = (parentWidth - ((margin * 3) + (gutter * 2))) / 4;
			double blahHeight = blahWidth;
			double curY, curX;
			double rowHeight = blahHeight + margin;

			for (int curRow = 0; curRow < numRows; curRow++) {
				curY = curRow * rowHeight;
				curX = gutter;
				for (int curCol = 0; curCol < 4; curCol++) {
					Button curBtn = new Button (this);
					RelativeLayout.LayoutParams layoutParams = new RelativeLayout.LayoutParams ((int)blahWidth, (int)blahHeight);
					layoutParams.SetMargins ((int)curX, (int)curY, 0, 0);
					curBtn.LayoutParameters = layoutParams;
					curBtn.Text = "This is a blah";
					BlahView.AddView (curBtn);
					curX += (margin + blahWidth);
				}

			}
		}
	}
}


