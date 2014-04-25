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
    [Activity]
    public class ViewPostActivity : FragmentActivity
    {

		protected override void OnCreate (Bundle bundle)
		{
            base.OnCreate(bundle);

            RequestWindowFeature(WindowFeatures.NoTitle);
			SetContentView (Resource.Layout.activity_viewpost);

            dialog = new ProgressDialog(this);
            dialog.SetMessage("Please wait...");
            dialog.SetCancelable(false);

            textView = FindViewById<TextView>(Resource.Id.text);
            image = FindViewById<ImageView>(Resource.Id.image);

            Button btn_back = FindViewById<Button>(Resource.Id.btn_back);
            btn_back.Click += delegate
            {
                Finish();
			};
        }

        private ImageView image;
        private TextView textView;
        private ProgressDialog dialog;
        protected override void OnStart()
        {
            base.OnStart();

            dialog.Show();
            BlahguaAPIObject.Current.SetCurrentBlahFromId(App.BlahIdToOpen, (theBlah) =>
            {

                dialog.Hide();

                //this.DataContext = BlahguaAPIObject.Current;
                if (BlahguaAPIObject.Current.CurrentBlah != null)
                {
                    if (theBlah.ImageURL != null)
                    {
                        //ImageSource defaultSrc = new BitmapImage(new Uri(defaultImg, UriKind.Relative));
                        //BackgroundImage.Source = defaultSrc;
                        //BackgroundImage2.Source = defaultSrc;


                        RunOnUiThread(() =>
                        {
                            image.SetUrlDrawable(theBlah.ImageURL);
                            //image.SetImageURI(Android.Net.Uri.Parse(imageURL));
                        });
                    }

                    RunOnUiThread(() =>
                    {
                        textView.SetText(theBlah.F, TextView.BufferType.Normal);
                    });

                    //BlahSummaryArea.Visibility = Visibility.Visible;
                    //UpdateButtonsForPage();
                    //switch (BlahguaAPIObject.Current.CurrentBlah.TypeName)
                    //{
                    //    case "polls":
                    //        HandlePollInit();
                    //        break;
                    //    case "predicts":
                    //        HandlePredictInit();
                    //        break;
                    //}
                }
                else
                {
                    Toast.MakeText(this, "unable to load blah.  Sorry!", ToastLength.Long).Show();
                    //App.analytics.PostSessionError("loadblahfailed");
                    Finish();
                }
            } );
        }
    }
}


