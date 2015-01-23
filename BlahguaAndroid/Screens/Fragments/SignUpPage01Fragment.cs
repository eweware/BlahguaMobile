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
    public class SignUpPage01Fragment : Android.Support.V4.App.Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here

        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            ViewGroup rootView = (ViewGroup)inflater.Inflate(Resource.Layout.SignUpPage01, container, false);

			rootView.FindViewById<TextView> (Resource.Id.textView1).SetTypeface (MainActivity.bodyFont, TypefaceStyle.Normal);
			rootView.FindViewById<EditText> (Resource.Id.usernameField).SetTypeface (MainActivity.bodyFont, TypefaceStyle.Normal);
			rootView.FindViewById<EditText> (Resource.Id.password).SetTypeface (MainActivity.bodyFont, TypefaceStyle.Normal);
			rootView.FindViewById<EditText> (Resource.Id.password2).SetTypeface (MainActivity.bodyFont, TypefaceStyle.Normal);
			rootView.FindViewById<TextView> (Resource.Id.textView2).SetTypeface (MainActivity.bodyFont, TypefaceStyle.Normal);
			rootView.FindViewById<EditText> (Resource.Id.emailAddrField).SetTypeface (MainActivity.bodyFont, TypefaceStyle.Normal);
			rootView.FindViewById<Button> (Resource.Id.createBtn).SetTypeface (MainActivity.bodyFont, TypefaceStyle.Normal);
			rootView.FindViewById<Button> (Resource.Id.skipBtn).SetTypeface (MainActivity.bodyFont, TypefaceStyle.Normal);
			rootView.FindViewById<TextView> (Resource.Id.loginBtn).SetTypeface (MainActivity.bodyFont, TypefaceStyle.Normal);


            return rootView;
        }
    }
}