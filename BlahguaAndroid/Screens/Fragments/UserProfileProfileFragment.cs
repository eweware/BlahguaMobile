using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
//using Android.Support.V4.App;
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
        private TextView accountName, headingPrompt;
        private ImageView avatar;
        private EditText nickname, recoveryEmail;
        private Button btn_avatar, btn_save;
        private CheckBox useMatureChk;
        private string oldUserName, oldEmail;
        private bool oldMatureSetting;
        

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
            nickname = fragment.FindViewById<EditText>(Resource.Id.nickname);
            progress = fragment.FindViewById<ProgressBar>(Resource.Id.progressBar1);
            btn_avatar = fragment.FindViewById<Button>(Resource.Id.btn_avatar);
            recoveryEmail = fragment.FindViewById<EditText>(Resource.Id.recoveryEmail);
            btn_save = fragment.FindViewById<Button>(Resource.Id.btn_save);
            useMatureChk = fragment.FindViewById<CheckBox>(Resource.Id.matureContentCheck);
            accountName = fragment.FindViewById<TextView>(Resource.Id.accountName);
            headingPrompt = fragment.FindViewById<TextView>(Resource.Id.headingPrompt);

            UiHelper.SetGothamTypeface(TypefaceStyle.Normal, headingPrompt, useMatureChk, nickname, btn_avatar, btn_save, recoveryEmail);
            UiHelper.SetGothamTypeface(TypefaceStyle.Bold, accountName);

            accountName.Text = "Account name:  " + BlahguaMobile.BlahguaCore.BlahguaAPIObject.Current.GetSavedUserInfo().UserName;
            progress.Visibility = ViewStates.Gone;
            btn_avatar.Click += click;
            avatar.Click += click;
            btn_save.Click += btn_save_Click;

            initUi();
            return fragment;
        }

        void btn_save_Click(object sender, EventArgs e)
        {
            // save user name, email, and profile
            string newProfileName = nickname.Text;
            bool newMatureSetting = useMatureChk.Checked;
            string newEmailAddr = recoveryEmail.Text;

            if (newMatureSetting != oldMatureSetting)
            {
                BlahguaAPIObject.Current.UpdateMatureFlag(newMatureSetting, (theResult) =>
                    {
                        oldMatureSetting = newMatureSetting;
                    });
            }

            if (newProfileName != oldUserName)
            {
                BlahguaAPIObject.Current.CurrentUser.Profile.Nickname = newProfileName;
                BlahguaAPIObject.Current.UpdateUserProfile((theString) =>
                {
                    oldUserName = newProfileName;
                    BlahguaAPIObject.Current.GetUserDescription((theDesc) =>
                    {
                        // to do - see if we need to rebind or...
                    }
                    );
                }
            );
            }

            if (oldEmail != newEmailAddr)
            {
                BlahguaAPIObject.Current.SetRecoveryEmail(newEmailAddr, (theResult) =>
                {
                    oldEmail = newEmailAddr;
                });
            }

        }

     

        private void initUi()
        {
            nickname.Text = BlahguaAPIObject.Current.CurrentUser.Profile.Nickname;
            if (!String.IsNullOrEmpty(BlahguaAPIObject.Current.CurrentUser.UserImage))
            {
                btn_avatar.Visibility = ViewStates.Gone;
                avatar.SetUrlDrawable(BlahguaAPIObject.Current.CurrentUser.UserImage, avatar.Drawable);
            }
            else
            {
                // set a placeholder image
                
            }
            oldUserName = nickname.Text;
            oldMatureSetting = BlahguaAPIObject.Current.CurrentUser.XXX;
            BlahguaAPIObject.Current.GetRecoveryEmail((theMail) =>
                {
                    oldEmail = theMail;
                    recoveryEmail.Text = theMail;
                });
        }

		public override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
			if (resultCode == Android.App.Result.Ok)
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