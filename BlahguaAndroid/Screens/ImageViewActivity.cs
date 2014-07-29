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
using BlahguaMobile.AndroidClient.HelpingClasses;
using Android.Provider;
using BlahguaMobile.BlahguaCore;
using Android.Content.PM;

namespace BlahguaMobile.AndroidClient.Screens
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait)]
    class ImageViewActivity : Activity
    {
        TouchImageView imageView;
        string imgPath;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            MainActivity.analytics.PostPageView("/ImageViewer");

            RequestWindowFeature(WindowFeatures.NoTitle);
            SetContentView(Resource.Layout.activity_imageview);

            imgPath = Intent.GetStringExtra("image");
            imageView = FindViewById<TouchImageView>(Resource.Id.image);

            ImageLoader.Instance.DownloadAsync(imgPath,
                imageView, (b) =>
                {
                    RunOnUiThread(() =>
                    {
                        imageView.Visibility = ViewStates.Visible;
                        imageView.SetImageBitmap(b);
                    });
                    return true;
                });
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            menu.Add("Save image");
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            try
            {
                string localPath = ImageLoader.GetLocalFilepath(imgPath);
                MediaStore.Images.Media.
                    InsertImage(ContentResolver, localPath, BlahguaAPIObject.Current.CurrentBlah.T, String.Empty);
                Toast.MakeText(this, "image saved", ToastLength.Short).Show();
            }
            catch (Exception e)
            {
                Toast.MakeText(this, "saving failed. check your sd card is accessible", ToastLength.Short).Show();
            }
            return base.OnOptionsItemSelected(item);
        }
    }
}