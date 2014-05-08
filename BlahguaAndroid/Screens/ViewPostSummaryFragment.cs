using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using BlahguaMobile.BlahguaCore;

using BlahguaMobile.AndroidClient.ThirdParty.UrlImageViewHelper;

namespace BlahguaMobile.AndroidClient.Screens
{
    class ViewPostSummaryFragment : Fragment
    {
        public static ViewPostSummaryFragment NewInstance()
        {
            return new ViewPostSummaryFragment { Arguments = new Bundle() };
        }

        private readonly string TAG = "ViewPostSummaryFragment";

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        private Activity parent = null;

        private ImageView image;
        private TextView titleView, textView;
        private ProgressDialog dialog;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            parent = (Activity)inflater.Context;
            View fragment = inflater.Inflate(Resource.Layout.fragment_viewpost_summary, null);
            //if (container == null)
            //{
            //    Log.Debug(TAG, "Dialer Fragment is in a view without container");
            //    return null;
            //}

            TextView text = fragment.FindViewById<TextView>(Resource.Id.text);
            //do some stuff like assigning event handlers, etc.

            dialog = new ProgressDialog(parent);
            dialog.SetMessage("Please wait...");
            dialog.SetCancelable(false);

            textView = fragment.FindViewById<TextView>(Resource.Id.text);
            titleView = fragment.FindViewById<TextView>(Resource.Id.title);
            image = fragment.FindViewById<ImageView>(Resource.Id.image);

            return fragment;// base.OnCreateView(inflater, container, savedInstanceState);
        }

        public override void OnStart()
        {
            base.OnStart();

            dialog.Show();
            BlahguaAPIObject.Current.SetCurrentBlahFromId(App.BlahIdToOpen, (theBlah) =>
            {

                dialog.Hide();

                //this.DataContext = BlahguaAPIObject.Current;
                if (BlahguaAPIObject.Current.CurrentBlah != null)
                {
                    if (theBlah.ImageURL != null && image != null)
                    {
                        //ImageSource defaultSrc = new BitmapImage(new Uri(defaultImg, UriKind.Relative));
                        //BackgroundImage.Source = defaultSrc;
                        //BackgroundImage2.Source = defaultSrc;


                        parent.RunOnUiThread(() =>
                        {
                            image.SetUrlDrawable(theBlah.ImageURL);
                            //image.SetImageURI(Android.Net.Uri.Parse(imageURL));
                        });
                    }

                    parent.RunOnUiThread(() =>
                    {
                        titleView.SetText(theBlah.T, TextView.BufferType.Normal);
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
                    Toast.MakeText(parent, "unable to load blah.  Sorry!", ToastLength.Long).Show();
                    //App.analytics.PostSessionError("loadblahfailed");
                    // Finish();
                }
            });
        }
    }
}