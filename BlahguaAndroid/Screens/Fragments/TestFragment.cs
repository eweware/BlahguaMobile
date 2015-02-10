using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
	class TestFragment : Fragment, IUrlImageViewCallback
    {
        public static UserProfileProfileFragment NewInstance()
        {
            return new UserProfileProfileFragment { Arguments = new Bundle() };
        }

        private ProgressBar progress;
        private ImageView avatar;
        private EditText nickname;
        private Button btn_avatar;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
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