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
        EditText nickname;
        Button btn_avatar;
        public static UserProfileProfileFragment NewInstance()
        {
            return new UserProfileProfileFragment { Arguments = new Bundle() };
        }

        private readonly string TAG = "UserProfileProfileFragment";

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            MainActivity.analytics.PostPageView("/self/profile");
            View fragment = inflater.Inflate(Resource.Layout.fragment_userprofile_profile, null);
            //if (container == null)
            //{
            //    Log.Debug(TAG, "Dialer Fragment is in a view without container");
            //    return null;
            //}
            avatar = fragment.FindViewById<ImageView>(Resource.Id.avatar);
            nickname = fragment.FindViewById<EditText>(Resource.Id.text);
            nickname.TextChanged += nickname_TextChanged;
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

            initUi();
            return fragment;
        }

        private void nickname_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            if (BlahguaAPIObject.Current.CurrentUser.Profile != null)
            {
                string newVal = nickname.Text;
                if (newVal != BlahguaAPIObject.Current.CurrentUser.Profile.City)
                {
                    BlahguaAPIObject.Current.CurrentUser.Profile.Nickname = newVal;
                    UpdateProfile();
                }
            }
        }

        void UpdateProfile()
        {
            // the profile has changed, save and reload the description...
            BlahguaAPIObject.Current.UpdateUserProfile((theString) =>
            {
                BlahguaAPIObject.Current.GetUserDescription((theDesc) =>
                {
                    // to do - see if we need to rebind or...
                }
                );
            }
            );
        }

        private void initUi()
        {
            nickname.Text = BlahguaAPIObject.Current.CurrentUser.Profile.Nickname;
            if (!String.IsNullOrEmpty(BlahguaAPIObject.Current.CurrentUser.UserImage))
            {
                btn_avatar.Visibility = ViewStates.Gone;
                avatar.SetUrlDrawable(BlahguaAPIObject.Current.CurrentUser.UserImage, avatar.Drawable);
            }
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
                            MainActivity.analytics.PostUploadUserImage();
                            avatar.SetUrlDrawable(photoString);
                        }
                        else
                        {
                            btn_avatar.Visibility = ViewStates.Visible;
                            Toast.MakeText(Activity, "Uploading failed", ToastLength.Short).Show();
                            MainActivity.analytics.PostSessionError("userimageuploadfailed");
                        }
                    });
                }
                );
            }
        }

        
    }
}