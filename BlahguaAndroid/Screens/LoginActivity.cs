using System;
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
using Android.Views.InputMethods;
using Android.Preferences;
using BlahguaMobile.AndroidClient.HelpingClasses;
using Android.Graphics;
using BlahguaMobile.AndroidClient.Screens;

namespace BlahguaMobile.AndroidClient
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait)]
	public class LoginActivity : Activity
	{
        private EditText usernameField;
        private EditText passwordField;
        private EditText confirmPassword;
        private EditText emailField;
        private Button signInBtn;
        private Button createAccountBtn;
        private TextView prepSignIn;
        private TextView emailPrompt;
        private ProgressDialog progressDlg;


		protected override void OnCreate (Bundle bundle)
		{
            RequestWindowFeature(WindowFeatures.NoTitle);
            this.Window.AddFlags(WindowManagerFlags.Fullscreen);

			this.Window.DecorView.SystemUiVisibility = StatusBarVisibility.Hidden;
            base.OnCreate(bundle);
			HomeActivity.analytics.PostPageView("/signup");


			// Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_login);

            progressDlg = new ProgressDialog(this);
            progressDlg.SetProgressStyle(ProgressDialogStyle.Spinner);

            FindViewById<TextView>(Resource.Id.textView1).SetTypeface(HomeActivity.gothamFont, TypefaceStyle.Normal);
            usernameField = FindViewById<EditText>(Resource.Id.usernameField);
            usernameField.SetTypeface(HomeActivity.gothamFont, TypefaceStyle.Normal);
            usernameField.AfterTextChanged += HandleTextValueChanged;

            passwordField = FindViewById<EditText>(Resource.Id.password);
            passwordField.SetTypeface(HomeActivity.gothamFont, TypefaceStyle.Normal);
            passwordField.AfterTextChanged += HandleTextValueChanged;

            confirmPassword = FindViewById<EditText>(Resource.Id.password2);
            confirmPassword.SetTypeface(HomeActivity.gothamFont, TypefaceStyle.Normal);
            confirmPassword.AfterTextChanged += HandleTextValueChanged;

            emailPrompt = FindViewById<TextView>(Resource.Id.emailPrompt);
            emailPrompt.SetTypeface(HomeActivity.gothamFont, TypefaceStyle.Normal);

            emailField = FindViewById<EditText>(Resource.Id.emailAddrField);
            emailField.SetTypeface(HomeActivity.gothamFont, TypefaceStyle.Normal);

            createAccountBtn = FindViewById<Button>(Resource.Id.createBtn);
            createAccountBtn.SetTypeface(HomeActivity.gothamFont, TypefaceStyle.Normal);

            createAccountBtn.Click += (snder, e) =>
            {
                progressDlg.SetMessage("signing in...");
                progressDlg.Show();
                string userName = usernameField.Text.Trim();
                string password = passwordField.Text;
                signInBtn.Enabled = false;
                createAccountBtn.Enabled = false;


                // sign in
                BlahguaAPIObject.Current.Register(userName, password, true, CreateAccountResultCallback);

            };


            prepSignIn = FindViewById<TextView>(Resource.Id.prepSignIn);
            prepSignIn.SetTypeface(HomeActivity.gothamFont, TypefaceStyle.Normal);

            prepSignIn.Click += (object sender, EventArgs e) =>
            {
                PrepForSignIn();


            };

            signInBtn = FindViewById<Button>(Resource.Id.signInBtn);
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

                // sign in
                BlahguaAPIObject.Current.SignIn(userName, password, true, SiginInResultCallback);
            };

            createAccountBtn.Enabled = false;
            signInBtn.Enabled = false;


            Button btn_help = FindViewById<Button>(Resource.Id.btn_help);
            btn_help.Click += (sender, args) =>
            {
                Intent emailIntent = new Intent(Intent.ActionSendto,
                    Android.Net.Uri.FromParts("mailto", App.EmailHelp, null));
                emailIntent.PutExtra(Intent.ExtraSubject, GetString(Resource.String.signin_help_email_subject));
                StartActivity(Intent.CreateChooser(emailIntent, GetString(Resource.String.signin_help_chooser_title)));
            };
            Button btn_about = FindViewById<Button>(Resource.Id.btn_about);
            btn_about.Click += (sender, args) =>
            {
                StartActivity(new Intent(Intent.ActionView, Android.Net.Uri.Parse(App.WebsiteAbout)));
            };
            Button btn_report = FindViewById<Button>(Resource.Id.btn_report);
            btn_report.Click += (sender, args) =>
            {
                Intent emailIntent = new Intent(Intent.ActionSendto,
                    Android.Net.Uri.FromParts("mailto", App.EmailReportBug, null));
                emailIntent.PutExtra(Intent.ExtraSubject, GetString(Resource.String.signin_report_email_subject));
                StartActivity(Intent.CreateChooser(emailIntent, GetString(Resource.String.signin_report_chooser_title)));
            };
            btn_help.SetTypeface(HomeActivity.gothamFont, TypefaceStyle.Normal);
            btn_about.SetTypeface(HomeActivity.gothamFont, TypefaceStyle.Normal);
            btn_report.SetTypeface(HomeActivity.gothamFont, TypefaceStyle.Normal);
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
                RunOnUiThread(() =>
                {
                    progressDlg.Hide();
                    Finish();
                });
            }
            else
            {
                HomeActivity.analytics.PostSessionError("signinfailed-" + result);

                DisplayAlert(result, "Unable to sign in.  Check username and password");
                RunOnUiThread(() =>
                {
                    progressDlg.Hide();
                    signInBtn.Enabled = true;
                    createAccountBtn.Enabled = true;
                    HandleTextValueChanged(null, null);
                });
            }

        }

        private void CreateAccountResultCallback(string result)
        {
            if (result == null)
            {
                HomeActivity.analytics.PostRegisterUser();
                RunOnUiThread(() =>
                {
                    progressDlg.Hide();
                    FirstRunActivity.emailAddress = emailField.Text.Trim();

                    if (!String.IsNullOrEmpty(FirstRunActivity.emailAddress))
                    {
                        BlahguaAPIObject.Current.SetRecoveryEmail(FirstRunActivity.emailAddress, (resultStr) =>
                        {
                            Finish();
                        });
                    }
                    else
                        Finish();
                });
            }
            else
            {
                HomeActivity.analytics.PostSessionError("registerfailed-" + result);

                DisplayAlert(result, "Unable to create account.  Check username");
                RunOnUiThread(() =>
                {
                    progressDlg.Hide();
                    signInBtn.Enabled = true;
                    createAccountBtn.Enabled = true;
                    HandleTextValueChanged(null, null);
                });
            }
        }

        public void DisplayAlert(string titleString, string descString)
        {
            RunOnUiThread(() =>
            {
                AlertDialog alert = new AlertDialog.Builder(this).Create();
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
