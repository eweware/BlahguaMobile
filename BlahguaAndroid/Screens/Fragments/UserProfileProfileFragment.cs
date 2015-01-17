using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//using Android.App;
using Android.Support.V4.App;
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
using BlahguaMobile.AndroidClient.HelpingClasses;

namespace BlahguaMobile.AndroidClient.Screens
{
    class UserProfileProfileFragment : Fragment, IUrlImageViewCallback
    {
        public static UserProfileProfileFragment NewInstance()
        {
            return new UserProfileProfileFragment { Arguments = new Bundle() };
        }

        private readonly string TAG = "UserProfileProfileFragment";

        private ProgressBar progress;
        private ImageView avatar;
        private EditText nickname;
        private Button btn_avatar;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
			HomeActivity.analytics.PostPageView("/self/profile");
            View fragment = inflater.Inflate(Resource.Layout.fragment_userprofile_profile, null);

            EventHandler click = (sender, args) =>
            {
                var imageIntent = new Intent();
                imageIntent.SetType("image/*");
                imageIntent.SetAction(Intent.ActionGetContent);
                StartActivityForResult(
                    Intent.CreateChooser(imageIntent, "Select photo"), 0);
            };

            avatar = fragment.FindViewById<ImageView>(Resource.Id.avatar);
            nickname = fragment.FindViewById<EditText>(Resource.Id.text);
            progress = fragment.FindViewById<ProgressBar>(Resource.Id.progressBar1);
            btn_avatar = fragment.FindViewById<Button>(Resource.Id.btn_avatar);

            UiHelper.SetGothamTypeface(TypefaceStyle.Normal, nickname, btn_avatar);

            nickname.TextChanged += nickname_TextChanged;
            progress.Visibility = ViewStates.Gone;
            btn_avatar.Click += click;
            avatar.Click += click;

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

        private void UpdateProfile()
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

        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
			if (resultCode == (int)Android.App.Result.Ok)
            {
                progress.Visibility = ViewStates.Visible;
                btn_avatar.Visibility = ViewStates.Gone;
                System.IO.Stream inputStream = StreamHelper.GetStreamFromFileUri(Activity, data.Data);
                String fileName = StreamHelper.GetFileName(Activity, data.Data);
                //String fileName = data.DataString.Substring(data.DataString.LastIndexOf("\\") + 1);
                avatar.SetUrlDrawable(null);
                BlahguaAPIObject.Current.UploadUserImage(inputStream, fileName, (photoString) =>
                {
                    Activity.RunOnUiThread(() =>
                    {
                        if ((photoString != null) && (photoString.Length > 0))
                        {
                            string photoURL = BlahguaAPIObject.Current.GetImageURL(photoString, "B");
									HomeActivity.analytics.PostUploadUserImage();
                            avatar.SetUrlDrawable(photoURL, this);
                        }
                        else
                        {
                            btn_avatar.Visibility = ViewStates.Visible;
                            Toast.MakeText(Activity, "Uploading failed", ToastLength.Short).Show();
									HomeActivity.analytics.PostSessionError("userimageuploadfailed");
                        }
                    });
                }
                );
            }
        }

        public void OnLoaded(ImageView imageView, Android.Graphics.Drawables.Drawable loadedDrawable, string url, bool loadedFromCache)
        {
            if (avatar == imageView)
            {
                Activity.RunOnUiThread(() =>
                {
                    progress.Visibility = ViewStates.Gone;
                });
            }
        }

    }
}