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
        private static LoginActivity instance;

        private EditText login, password, passwordConfirm;
        private ProgressBar progress;
        private Button buttonDone;

        private CheckBox check_create_acc, check_remember_me;
        private ProgressDialog dialog;

        private bool createNewAccount = false;

		protected override void OnCreate (Bundle bundle)
		{
            base.OnCreate(bundle);
            MainActivity.analytics.PostPageView("/signup");

            instance = this;

            RequestWindowFeature(WindowFeatures.NoTitle);

			// Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_login);

            // HEADER
            TextView loginTitle = FindViewById<TextView>(Resource.Id.login_title);
            Button buttonCancel = FindViewById<Button>(Resource.Id.btn_cancel);
            buttonDone = FindViewById<Button>(Resource.Id.btn_done);
            buttonDone.Enabled = false;
            buttonDone.SetTypeface(MainActivity.merriweatherFont, TypefaceStyle.Normal);
            buttonCancel.SetTypeface(MainActivity.merriweatherFont, TypefaceStyle.Normal);
            loginTitle.SetTypeface(MainActivity.merriweatherFont, Android.Graphics.TypefaceStyle.Normal);

            // BODY
            progress = FindViewById<ProgressBar>(Resource.Id.progressBar1);
            login = FindViewById<EditText>(Resource.Id.login);
            password = FindViewById<EditText>(Resource.Id.password);
            passwordConfirm = FindViewById<EditText>(Resource.Id.password_confirm);
            login.TextChanged += edit_TextChanged;
            password.TextChanged += edit_TextChanged;
            passwordConfirm.TextChanged += edit_TextChanged;
            login.SetTypeface(MainActivity.gothamFont, TypefaceStyle.Normal);
            password.SetTypeface(MainActivity.gothamFont, TypefaceStyle.Normal);
            passwordConfirm.SetTypeface(MainActivity.gothamFont, TypefaceStyle.Normal);

            check_create_acc = FindViewById<CheckBox>(Resource.Id.check_create_acc);
            check_create_acc.SetTypeface(MainActivity.gothamFont, TypefaceStyle.Normal);

            passwordConfirm.Visibility = ViewStates.Gone;
            progress.Visibility = ViewStates.Invisible;

            buttonCancel.Click += delegate
            {
                Finish();
            };
            buttonDone.Click += delegate
            {
                InputMethodManager imm = (InputMethodManager)GetSystemService(
                        Context.InputMethodService);
                imm.HideSoftInputFromWindow(passwordConfirm.WindowToken, 0);

                if (check_create_acc.Checked)
                {
                    DoCreateAccount();
                }
                else
                {
                    DoSignIn();
                }
            };

            check_create_acc.CheckedChange += delegate
            {
                createNewAccount = check_create_acc.Checked;
                if (check_create_acc.Checked)
                {
                    passwordConfirm.Visibility = ViewStates.Visible;
                    passwordConfirm.Text = "";
                    buttonDone.Enabled = false;
                }
                else
                {
                    passwordConfirm.Visibility = ViewStates.Gone;
                }
            };

            // yes and no checkboxes

            check_remember_me = FindViewById<CheckBox>(Resource.Id.check_remember_me);
            check_remember_me.SetTypeface(MainActivity.gothamFont, TypefaceStyle.Normal);

            dialog = new ProgressDialog(this);
            dialog.SetMessage(GetString(Resource.String.signin_message_signing_in));
            dialog.SetCancelable(false);

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
            btn_help.SetTypeface(MainActivity.gothamFont, TypefaceStyle.Normal);
            btn_about.SetTypeface(MainActivity.gothamFont, TypefaceStyle.Normal);
            btn_report.SetTypeface(MainActivity.gothamFont, TypefaceStyle.Normal);
		}

        private void edit_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            if (login.Text.Length == 0 || password.Text.Length == 0 ||
                (check_create_acc.Checked && passwordConfirm.Text.Length == 0))
            {
                buttonDone.Enabled = false;
            }
            else
            {
                buttonDone.Enabled = true;
            }
        }

        private void DoSignIn()
        {
			login.Text = login.Text.Trim ();
			password.Text = password.Text.Trim ();

            if (BlahguaAPIObject.Current.UserName != login.Text)
                BlahguaAPIObject.Current.UserName = login.Text;

            if (BlahguaAPIObject.Current.UserPassword != password.Text)
                BlahguaAPIObject.Current.UserPassword = password.Text;

            BlahguaAPIObject.Current.AutoLogin = check_remember_me.Checked;

            progress.Visibility = ViewStates.Visible;
            dialog.Show();
            BlahguaAPIObject.Current.SignIn(BlahguaAPIObject.Current.UserName,
                BlahguaAPIObject.Current.UserPassword,
                BlahguaAPIObject.Current.AutoLogin,
                (errMsg) =>
                {
                    //dialog.Hide();
                    if (errMsg == null)
                        HandleUserSignIn();
                    else
                    {
                        MainActivity.analytics.PostSessionError("signinfailed-" + errMsg);
                        RunOnUiThread(() =>
                        {
                            progress.Visibility = ViewStates.Invisible;
                            //Toast.MakeText(this, "could not register: " + errMsg, ToastLength.Short).Show();
                            Toast.MakeText(this, GetString(Resource.String.signin_message_error_signing_in), ToastLength.Short).Show();
							dialog.Hide();
                        });
                    }
                }
            );
        }

        private void HandleUserSignIn()
        {
            // remember or not?
            if (BlahguaAPIObject.Current.AutoLogin)
            {
                ISharedPreferences _sharedPref = PreferenceManager.GetDefaultSharedPreferences(this);
                _sharedPref.Edit().PutString("username", BlahguaAPIObject.Current.UserName).Commit();
                _sharedPref.Edit().PutString("password", BlahguaAPIObject.Current.UserPassword).Commit();
            }

            // do the rest
            MainActivity.analytics.PostLogin();
            RunOnUiThread(() =>
            {
                progress.Visibility = ViewStates.Invisible;
                Finish();
            });
        }

        private void DoCreateAccount()
        {
			login.Text = login.Text.Trim ();
			password.Text = password.Text.Trim ();

            if (login.Text.Length == 0)
            {
                RunOnUiThread(() =>
                {
                    Toast.MakeText(this, GetString(Resource.String.signin_message_enter_login), ToastLength.Short).Show();
                });
                return;
            }
            if (password.Text.Length == 0)
            {
                RunOnUiThread(() =>
                {
                    Toast.MakeText(this, GetString(Resource.String.signin_message_enter_pass), ToastLength.Short).Show();
                });
                return;
            }
            if (BlahguaAPIObject.Current.UserName != login.Text)
                BlahguaAPIObject.Current.UserName = login.Text;

            if (BlahguaAPIObject.Current.UserPassword != password.Text)
                BlahguaAPIObject.Current.UserPassword = password.Text;

            if (BlahguaAPIObject.Current.UserPassword2 != passwordConfirm.Text)
                BlahguaAPIObject.Current.UserPassword2 = passwordConfirm.Text;

            BlahguaAPIObject.Current.AutoLogin = check_remember_me.Checked;

            if (BlahguaAPIObject.Current.UserPassword != BlahguaAPIObject.Current.UserPassword2)
            {
                RunOnUiThread(() => {
                    Toast.MakeText(this, GetString(Resource.String.signin_message_passwords_must_match), ToastLength.Short).Show();
                });
            }
            else
            {
                progress.Visibility = ViewStates.Visible;
                dialog.Show();
                BlahguaAPIObject.Current.Register(BlahguaAPIObject.Current.UserName, BlahguaAPIObject.Current.UserPassword,
                    BlahguaAPIObject.Current.AutoLogin, (errMsg) =>
                {
                    if (errMsg == null)
                    {
                        MainActivity.analytics.PostRegisterUser();
                        HandleUserSignIn();
                    }
                    else
                    {
                        MainActivity.analytics.PostSessionError("registerfailed-" + errMsg);
                        RunOnUiThread(() =>
                        {
                            progress.Visibility = ViewStates.Invisible;
                            Toast.MakeText(this, "could not register: " + errMsg, ToastLength.Short).Show();
							dialog.Hide();
                        });
                    }
                }
                );
            }
        }
	}
}
