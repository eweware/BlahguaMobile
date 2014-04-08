using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Util;
using Android.Content.PM;
using Android.Support.V4.Widget;

namespace BlahguaMobile.Android
{
	[Activity (MainLauncher = true)]
	public class MainActivity : Activity
	{
		int count = 1;
		int count2 = 1;

		private RelativeLayout BlahView = null;

        private DrawerLayout m_Drawer;
        private ListView m_DrawerList;

		protected override void OnCreate (Bundle bundle)
		{
            base.OnCreate(bundle);

            RequestWindowFeature(WindowFeatures.NoTitle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.MainScreen);

			BlahView = FindViewById<RelativeLayout> (Resource.Id.BlahContainer);
			AddBlahs();

			// Get our button from the layout resource,
			// and attach an event to it
			Button button = FindViewById<Button> (Resource.Id.channelSelectBtn);
			button.Click += delegate {
				//button.Text = string.Format ("{0} clicks!", count++);
			};

			button = FindViewById<Button> (Resource.Id.btn_login);
			button.Click += delegate {
				//button.Text = string.Format ("{0} clicks!", count2++);
                StartActivity(typeof(LoginActivity));
			};
		}

        int[] elements = new int[]{ 1, 1, 1, 2, 1, 1, 1, 1, 2, 1, 1, 2, 1, 1, 1 };
        String[] strings = new String[] { 
            "This time, Georgia officials...","Miss out on the chance...","Now that the good people of Russia...",
            "East and west invest in...","<no text>",
            "Nokia WP", "<no text>", "An American citizen who is a member...",
            "The Copenhagen Zoo has face worldwide criticism over its...", "One Company Is Trying...",
            "Investigation of russian economy...", "Oil pumps are getting...",
            "<no text>", "Americans are prepairing for...", "<no text>"
        };
		private void AddBlahs()
		{
            int screenWidth = Resources.DisplayMetrics.WidthPixels;

            double curY, curX;
            int curRow = 0;
            double blahWidth = screenWidth / 3;
            double blahHeight = blahWidth;
            curX = 0;
            int rowCount = 0;
            for (int i = 0; i < elements.Length; i++)
            {
                int size = elements[i];
                rowCount += size;
                curY = curRow * blahHeight;

                Button curBtn = new Button(this);
                RelativeLayout.LayoutParams layoutParams = new RelativeLayout.LayoutParams((int)blahWidth * size, (int)blahHeight);
                layoutParams.SetMargins((int)curX, (int)curY, 0, 0);
                curBtn.LayoutParameters = layoutParams;
                curBtn.Text = strings[i];
                BlahView.AddView(curBtn);
                curX += blahWidth * size;


                if (rowCount >= 3)
                {
                    curX = rowCount = 0;
                    curRow++;
                }
            }

            //double margin = 8;
            //double gutter = 0;
            //double parentWidth = screenWidth;
            //double blahWidth = (parentWidth - ((margin * 3) + (gutter * 2))) / 4;
            //double blahHeight = blahWidth;
            //double curY, curX;
            //double rowHeight = blahHeight + margin;

            //for (int curRow = 0; curRow < numRows; curRow++) {
            //    curY = curRow * rowHeight;
            //    curX = gutter;
            //    for (int curCol = 0; curCol < 4; curCol++) {
            //        Button curBtn = new Button (this);
            //        RelativeLayout.LayoutParams layoutParams = new RelativeLayout.LayoutParams ((int)blahWidth, (int)blahHeight);
            //        layoutParams.SetMargins ((int)curX, (int)curY, 0, 0);
            //        curBtn.LayoutParameters = layoutParams;
            //        curBtn.Text = "This is a blah";
            //        BlahView.AddView (curBtn);
            //        curX += (margin + blahWidth);
            //    }

            //}
		}
	}
}


