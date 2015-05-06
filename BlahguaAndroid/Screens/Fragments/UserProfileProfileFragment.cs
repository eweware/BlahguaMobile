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
using Android.Support.V4.App;

namespace BlahguaMobile.AndroidClient.Screens
{
    public class UserProfileProfileFragment : Android.Support.V4.App.Fragment, IUrlImageViewCallback
    {
        public static UserProfileProfileFragment NewInstance()
        {
            return new UserProfileProfileFragment { Arguments = new Bundle() };
        }

        private ProgressBar progress;
        private TextView accountName, headingPrompt;
        private ImageView avatar;
        private EditText nickname, recoveryEmail;
        private Button btn_avatar, btn_save;
        private CheckBox useMatureChk;
        private string oldUserName, oldEmail;
        private bool oldMatureSetting;
        private readonly int SELECTIMAGE_REQUEST = 777;
        

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
                    Intent.CreateChooser(imageIntent, "Select photo"), SELECTIMAGE_REQUEST);
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
            useMatureChk.SetBackgroundColor(Resources.GetColor(Resource.Color.heard_red));
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
            useMatureChk.Checked = oldMatureSetting;
            BlahguaAPIObject.Current.GetRecoveryEmail((theMail) =>
                {
                    oldEmail = theMail;
                    recoveryEmail.Text = theMail;
                });

            
        }

		public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == SELECTIMAGE_REQUEST && resultCode == (int)Android.App.Result.Ok)
            {
                progress.Visibility = ViewStates.Visible;
                btn_avatar.Visibility = ViewStates.Gone;
                System.IO.Stream inputStream = StreamHelper.GetStreamFromFileUri(Activity, data.Data);
                String fileName = StreamHelper.GetFileName(Activity, data.Data);
                avatar.SetUrlDrawable(null);
                if (inputStream != null)
                {
                    BlahguaAPIObject.Current.UploadUserImage(inputStream, fileName, (photoString) =>
                        {
                            Activity.RunOnUiThread(() =>
                            {
                                if ((photoString != null) && (photoString.Length > 0))
                                {
                                    string photoURL = BlahguaAPIObject.Current.GetImageURL(photoString, "C");
									HomeActivity.analytics.PostUploadUserImage();
                                    avatar.SetUrlDrawable(photoURL, this);
                                }
                                else
                                {
                                    progress.Visibility = ViewStates.Gone;
                                    btn_avatar.Visibility = ViewStates.Visible;
                                    Toast.MakeText(Activity, "Uploading failed", ToastLength.Short).Show();
									        HomeActivity.analytics.PostSessionError("userimageuploadfailed");
                                }
                            });
                        });
                }
                else
                {
                    Activity.RunOnUiThread(() =>
                    {
                        var toast = Toast.MakeText(Activity, "Cannot upload this type of image", ToastLength.Long);
                        toast.Show();
                        progress.Visibility = ViewStates.Gone;
                        btn_avatar.Visibility = ViewStates.Visible;
                    });
                }
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