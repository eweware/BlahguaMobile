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

using BlahguaMobile.BlahguaCore;


namespace BlahguaMobile.AndroidClient.Screens
{
    public class SignUpPage01Fragment : Android.Support.V4.App.Fragment
    {
        private EditText usernameField;
        private EditText passwordField;
        private EditText confirmPassword;
        private EditText emailField;
        private Button signInBtn;
        private Button createAccountBtn;
        private Button skipButton;
        private TextView prepSignIn;
        private TextView emailPrompt;
        private ProgressDialog progressDlg;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here

        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            ViewGroup rootView = (ViewGroup)inflater.Inflate(Resource.Layout.SignUpPage01, container, false);

            progressDlg = new ProgressDialog(this.Activity);
            progressDlg.SetProgressStyle(ProgressDialogStyle.Spinner);

			rootView.FindViewById<TextView> (Resource.Id.textView1).SetTypeface (HomeActivity.gothamFont, TypefaceStyle.Normal);
			usernameField = rootView.FindViewById<EditText> (Resource.Id.usernameField);
			usernameField.SetTypeface (HomeActivity.gothamFont, TypefaceStyle.Normal);
            usernameField.AfterTextChanged += HandleTextValueChanged;

            passwordField = rootView.FindViewById<EditText>(Resource.Id.password);
			passwordField.SetTypeface(HomeActivity.gothamFont, TypefaceStyle.Normal);
            passwordField.AfterTextChanged += HandleTextValueChanged;

            confirmPassword = rootView.FindViewById<EditText>(Resource.Id.password2);
			confirmPassword.SetTypeface(HomeActivity.gothamFont, TypefaceStyle.Normal);
            confirmPassword.AfterTextChanged += HandleTextValueChanged;

            emailPrompt = rootView.FindViewById<TextView>(Resource.Id.emailPrompt);
			emailPrompt.SetTypeface(HomeActivity.gothamFont, TypefaceStyle.Normal);

            emailField = rootView.FindViewById<EditText>(Resource.Id.emailAddrField);
			emailField.SetTypeface(HomeActivity.gothamFont, TypefaceStyle.Normal);

            createAccountBtn =rootView.FindViewById<Button>(Resource.Id.createBtn);
			createAccountBtn.SetTypeface(HomeActivity.gothamFont, TypefaceStyle.Normal);

            createAccountBtn.Click += (snder, e) =>
                {
                    progressDlg.SetMessage("signing in...");
                    progressDlg.Show();
                    string userName = usernameField.Text.Trim();
                    string password = passwordField.Text;
                    signInBtn.Enabled = false;
                    createAccountBtn.Enabled = false;
                    skipButton.Enabled = false;


                    // sign in
                    BlahguaAPIObject.Current.Register(userName, password, true, CreateAccountResultCallback);

                };

            skipButton = rootView.FindViewById<Button>(Resource.Id.skipBtn);
			skipButton.SetTypeface(HomeActivity.gothamFont, TypefaceStyle.Normal);

            skipButton.Click += (object sender, EventArgs e) =>
            {
                ((FirstRunActivity)this.Activity).FinishSignin();


            };
            
            prepSignIn = rootView.FindViewById<TextView>(Resource.Id.prepSignIn);
			prepSignIn.SetTypeface(HomeActivity.gothamFont, TypefaceStyle.Normal);

            prepSignIn.Click += (object sender, EventArgs e) =>
            {
                PrepForSignIn();


            };

            signInBtn = rootView.FindViewById<Button>(Resource.Id.signInBtn);
			signInBtn.SetTypeface(HomeActivity.gothamFont, TypefaceStyle.Normal);
            signInBtn.Visibility = ViewStates.Gone;

            signInBtn.Click += (object sender, EventArgs e) =>
            {
                progressDlg.SetMessage("signing in...");
                progressDlg.Show();
                string userName = usernameField.Text.Trim();
                string password = passwordField.Text;
                signInBtn.Enabled = false;
                createAccountBtn.Enabled = false;
                skipButton.Enabled = false;

                // sign in
                BlahguaAPIObject.Current.SignIn(userName, password, true, SiginInResultCallback);
            };

            createAccountBtn.Enabled = false;
            signInBtn.Enabled = false;

            return rootView;
        }

        void HandleTextValueChanged(object sender, Android.Text.AfterTextChangedEventArgs e)
        {
            string usernameText = usernameField.Text;
            string passwordText = passwordField.Text;
            string confirmText = confirmPassword.Text;
            string emailText = emailField.Text;

            if (String.IsNullOrEmpty(usernameText) || String.IsNullOrEmpty(passwordText) ||
                (usernameText.Length < 3) || (passwordText.Length < 3))
            {
                signInBtn.Enabled = false;
                createAccountBtn.Enabled = false;

            }
            else
            {
                signInBtn.Enabled = true;

                if (passwordText == confirmText)
                    createAccountBtn.Enabled = true;
                else
                    createAccountBtn.Enabled = false;
            }
        }

        private void SiginInResultCallback(string result)
        {

            if (result == null)
            {
                HomeActivity.analytics.PostLogin();
                Activity.RunOnUiThread(() =>
                    {
                        progressDlg.Hide();
                        ((FirstRunActivity)this.Activity).FinishSignin();
                    });
            }
            else
            {
                HomeActivity.analytics.PostSessionError("signinfailed-" + result);

                DisplayAlert(result, "Unable to sign in.  Check username and password");
                Activity.RunOnUiThread(() =>
                {
                    progressDlg.Hide();
                    signInBtn.Enabled = true;
                    createAccountBtn.Enabled = true;
                    skipButton.Enabled = true;
                    HandleTextValueChanged(null, null);
                });
            }

        }

        private void CreateAccountResultCallback(string result)
        {
            if (result == null)
            {
                HomeActivity.analytics.PostRegisterUser();
                Activity.RunOnUiThread(() =>
                {
                    progressDlg.Hide();
                    FirstRunActivity.emailAddress = emailField.Text.Trim();

                    if (!String.IsNullOrEmpty(FirstRunActivity.emailAddress))
                    {
                        BlahguaAPIObject.Current.SetRecoveryEmail(FirstRunActivity.emailAddress, (resultStr) =>
                        {

                            Activity.RunOnUiThread(() =>
                                {
                                    ((FirstRunActivity)this.Activity).FinishSignin();
                                });
                        });
                    }
                    else
                        ((FirstRunActivity)this.Activity).FinishSignin();
                });
            }
            else
            {
                HomeActivity.analytics.PostSessionError("registerfailed-" + result);

                DisplayAlert(result, "Unable to create account.  Check username");
                Activity.RunOnUiThread(() =>
                {
                    progressDlg.Hide();
                    signInBtn.Enabled = true;
                    createAccountBtn.Enabled = true;
                    skipButton.Enabled = true;
                    HandleTextValueChanged(null, null);
                });
            }
        }

        public void DisplayAlert(string titleString, string descString)
        {
            Activity.RunOnUiThread(() =>
            {
                AlertDialog alert = new AlertDialog.Builder(this.Activity).Create() ;
                alert.SetTitle(titleString);
                alert.SetMessage(descString);
                alert.SetButton("ok", (sender, args) =>
                    {
                        alert.Dismiss();
                    });
                alert.Show();
            });

        }

        void PrepForSignIn()
        {
            signInBtn.Visibility = ViewStates.Visible;
            confirmPassword.Visibility = ViewStates.Gone;
            emailPrompt.Visibility = ViewStates.Gone;
            emailField.Visibility = ViewStates.Gone;
            prepSignIn.Visibility = ViewStates.Gone;
            createAccountBtn.Visibility = ViewStates.Gone;
        }

    }
}