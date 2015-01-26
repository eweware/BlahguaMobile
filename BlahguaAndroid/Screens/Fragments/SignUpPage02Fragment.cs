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
    public class SignUpPage02Fragment : Android.Support.V4.App.Fragment
    {
        private Button techBtn;
        private Button entBtn;
        private Button publicBtn;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            ViewGroup rootView = (ViewGroup)inflater.Inflate(Resource.Layout.SignUpPage02, container, false);

			rootView.FindViewById<TextView> (Resource.Id.textView1).SetTypeface (MainActivity.bodyFont, TypefaceStyle.Normal);
			rootView.FindViewById<TextView> (Resource.Id.textView2).SetTypeface (MainActivity.bodyFont, TypefaceStyle.Normal);
			rootView.FindViewById<TextView> (Resource.Id.textView3).SetTypeface (MainActivity.bodyFont, TypefaceStyle.Normal);
			rootView.FindViewById<TextView> (Resource.Id.titleText).SetTypeface (MainActivity.bodyFont, TypefaceStyle.Normal);

            techBtn = rootView.FindViewById<Button>(Resource.Id.TechBtn);
            techBtn.SetTypeface(MainActivity.bodyFont, TypefaceStyle.Normal);
            techBtn.Click += (object sender, EventArgs e) =>
                {
                    SetDefaultChannel("Tech Industry");
                    ((FirstRunActivity)this.Activity).GoToNext();
                };

            entBtn = rootView.FindViewById<Button>(Resource.Id.EntBtn);
            entBtn.SetTypeface(MainActivity.bodyFont, TypefaceStyle.Normal);
            entBtn.Click += (object sender, EventArgs e) =>
            {
                SetDefaultChannel("Entertainment Industry");
                ((FirstRunActivity)this.Activity).GoToNext();
            };

            publicBtn = rootView.FindViewById<Button>(Resource.Id.PublicBtn);
            publicBtn.SetTypeface(MainActivity.bodyFont, TypefaceStyle.Normal);
            publicBtn.Click += (object sender, EventArgs e) =>
            {
                SetDefaultChannel("Public");
                ((FirstRunActivity)this.Activity).Finish();
            };

            return rootView;
        }

        private void SetDefaultChannel(string channelName)
        {
            BlahguaCore.BlahguaAPIObject.Current.SavedChannel = channelName;
            BlahguaCore.BlahguaAPIObject.Current.SafeSaveSetting("SavedChannel", channelName);
        }
    }
}