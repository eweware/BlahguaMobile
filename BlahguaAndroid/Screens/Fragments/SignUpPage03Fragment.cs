using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Support.V4.App;
using Android.Graphics;

namespace BlahguaMobile.AndroidClient.Screens
{
    public class SignUpPage03Fragment : Android.Support.V4.App.Fragment
    {
		private LinearLayout enterEmailView, verificationView;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            ViewGroup rootView = (ViewGroup)inflater.Inflate(Resource.Layout.SignUpPage03, container, false);

			rootView.FindViewById<TextView> (Resource.Id.textView1).SetTypeface (MainActivity.bodyFont, TypefaceStyle.Normal);
			rootView.FindViewById<TextView> (Resource.Id.textView2).SetTypeface (MainActivity.bodyFont, TypefaceStyle.Normal);

			rootView.FindViewById<TextView> (Resource.Id.titleText).SetTypeface (MainActivity.bodyFont, TypefaceStyle.Normal);
			rootView.FindViewById<EditText> (Resource.Id.emailAddrField).SetTypeface (MainActivity.bodyFont, TypefaceStyle.Normal);
			rootView.FindViewById<Button> (Resource.Id.checkbtn).SetTypeface (MainActivity.bodyFont, TypefaceStyle.Normal);
			rootView.FindViewById<Button> (Resource.Id.skipBtn).SetTypeface (MainActivity.bodyFont, TypefaceStyle.Normal);

			rootView.FindViewById<TextView> (Resource.Id.titleText).SetTypeface (MainActivity.bodyFont, TypefaceStyle.Normal);
			rootView.FindViewById<EditText> (Resource.Id.verifyField).SetTypeface (MainActivity.bodyFont, TypefaceStyle.Normal);
			rootView.FindViewById<Button> (Resource.Id.verifyBtn).SetTypeface (MainActivity.bodyFont, TypefaceStyle.Normal);
			rootView.FindViewById<Button> (Resource.Id.tryAgainBtn).SetTypeface (MainActivity.bodyFont, TypefaceStyle.Normal);

			enterEmailView = rootView.FindViewById<LinearLayout> (Resource.Id.enterEmailView);
			verificationView = rootView.FindViewById<LinearLayout> (Resource.Id.verifyView);
 


            return rootView;
        }
    }
}