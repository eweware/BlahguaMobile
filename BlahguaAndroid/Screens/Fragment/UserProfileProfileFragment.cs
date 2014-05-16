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
using BlahguaMobile.AndroidClient.Adapters;
using Android.Animation;
using Android.Graphics;
using Java.IO;
using BlahguaMobile.AndroidClient.ThirdParty.UrlImageViewHelper;

namespace BlahguaMobile.AndroidClient.Screens
{
    class UserProfileProfileFragment : Fragment
    {
        ProgressBar progress;
        ImageView avatar;
        Button btn_avatar;
        public static UserProfileProfileFragment NewInstance()
        {
            return new UserProfileProfileFragment { Arguments = new Bundle() };
        }

        private readonly string TAG = "UserProfileProfileFragment";

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View fragment = inflater.Inflate(Resource.Layout.fragment_userprofile_profile, null);
            //if (container == null)
            //{
            //    Log.Debug(TAG, "Dialer Fragment is in a view without container");
            //    return null;
            //}
            avatar = fragment.FindViewById<ImageView>(Resource.Id.avatar);
            progress = fragment.FindViewById<ProgressBar>(Resource.Id.progressBar1);
            progress.Visibility = ViewStates.Gone;

            btn_avatar = fragment.FindViewById<Button>(Resource.Id.btn_avatar);
            btn_avatar.Click += (sender, args) =>
            {
                var imageIntent = new Intent();
                imageIntent.SetType("image/*");
                imageIntent.SetAction(Intent.ActionGetContent);
                StartActivityForResult(
                    Intent.CreateChooser(imageIntent, "Select photo"), 0);
            };

            return fragment;
        }

        public override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (resultCode == Result.Ok)
            {
                progress.Visibility = ViewStates.Visible;
                btn_avatar.Visibility = ViewStates.Gone;
                System.IO.Stream inputStream = Activity.ContentResolver.OpenInputStream(data.Data);
                BlahguaAPIObject.Current.UploadUserImage(inputStream, data.DataString.Substring(data.DataString.LastIndexOf("\\") + 1), (photoString) =>
                {
                    Activity.RunOnUiThread(() =>
                    {
                        progress.Visibility = ViewStates.Gone;
                        if ((photoString != null) && (photoString.Length > 0))
                        {
                            //App.analytics.PostUploadUserImage();
                            avatar.SetUrlDrawable(photoString);
                        }
                        else
                        {
                            btn_avatar.Visibility = ViewStates.Visible;
                            Toast.MakeText(Activity, "Uploading failed", ToastLength.Short).Show();
                            //App.analytics.PostSessionError("userimageuploadfailed");
                        }
                    });
                }
                );

                //var imageView =
                //    FindViewById<ImageView>(Resource.Id.myImageView);
                //imageView.SetImageURI(data.Data);
            }
        }

        //////////////////////////////////////////////////

        //private void ChangeImage_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        //{
        //    PhotoChooserTask photoChooserTask;
        //    photoChooserTask = new PhotoChooserTask();
        //    photoChooserTask.ShowCamera = true;
        //    photoChooserTask.Completed += new EventHandler<PhotoResult>(photoChooserTask_Completed);
        //    photoChooserTask.Show();
        //}

        //void photoChooserTask_Completed(object sender, PhotoResult e)
        //{
        //    if (e.TaskResult == TaskResult.OK)
        //    {
        //        UploadImageProgress.Visibility = Visibility.Visible;
        //        BlahguaAPIObject.Current.UploadUserImage(e.ChosenPhoto, e.OriginalFileName.Substring(e.OriginalFileName.LastIndexOf("\\") + 1), (photoString) =>
        //        {
        //            UploadImageProgress.Visibility = Visibility.Collapsed;
        //            if ((photoString != null) && (photoString.Length > 0))
        //            {
        //                App.analytics.PostUploadUserImage();
        //            }
        //            else
        //            {
        //                App.analytics.PostSessionError("userimageuploadfailed");
        //            }
        //        }
        //        );
        //    }
        //}
    }
}