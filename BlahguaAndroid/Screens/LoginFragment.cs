using System;
using Android.App;
using Android.Support.V4.App;
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
    //[Activity(ScreenOrientation = ScreenOrientation.Portrait)]
	public class LoginFragment : Android.Support.V4.App.Fragment
	{
        private EditText login, password, passwordConfirm, recoveryEmail;
        private ProgressBar progress;
        private Button buttonDone;

        private CheckBox check_create_acc, check_remember_me;
        private ProgressDialog dialog;

        private bool createNewAccount = false;
		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView (inflater, container, savedInstanceState);

			HomeActivity.analytics.PostPageView("/signup");

			View fragment = inflater.Inflate(Resource.Layout.fragment_login, null);

			//TextView loginTitle = fragment.FindViewById<TextView>(Resource.Id.login_title);
			Button buttonCancel = fragment.FindViewById<Button>(Resource.Id.btn_cancel);
			buttonDone = fragment.FindViewById<Button>(Resource.Id.btn_done);
			buttonDone.Enabled = false;
			buttonDone.SetTypeface(HomeActivity.merriweatherFont, TypefaceStyle.Normal);
			buttonCancel.SetTypeface(HomeActivity.merriweatherFont, TypefaceStyle.Normal);
			//loginTitle.SetTypeface(HomeActivity.merriweatherFont, Android.Graphics.TypefaceStyle.Normal);

			// BODY
			progress = fragment.FindViewById<ProgressBar>(Resource.Id.progressBar1);
			login = fragment.FindViewById<EditText>(Resource.Id.login);
			password = fragment.FindViewById<EditText>(Resource.Id.password);
			passwordConfirm = fragment.FindViewById<EditText>(Resource.Id.password_confirm);
			recoveryEmail = fragment.FindViewById<EditText>(Resource.Id.email_recovery);
			login.TextChanged += edit_TextChanged;
			password.TextChanged += edit_TextChanged;
			passwordConfirm.TextChanged += edit_TextChanged;
			login.SetTypeface(HomeActivity.gothamFont, TypefaceStyle.Normal);
			password.SetTypeface(HomeActivity.gothamFont, TypefaceStyle.Normal);
			passwordConfirm.SetTypeface(HomeActivity.gothamFont, TypefaceStyle.Normal);
			recoveryEmail.SetTypeface(HomeActivity.gothamFont, TypefaceStyle.Normal);

			check_create_acc = fragment.FindViewById<CheckBox>(Resource.Id.check_create_acc);
			check_create_acc.SetTypeface(HomeActivity.gothamFont, TypefaceStyle.Normal);

			passwordConfirm.Visibility = ViewStates.Gone;
			recoveryEmail.Visibility = ViewStates.Gone;
			progress.Visibility = ViewStates.Invisible;

			buttonCancel.Click += delegate
			{
				//Finish();
			};
			buttonDone.Click += delegate
			{
				InputMethodManager imm = (InputMethodManager) this.Activity.GetSystemService(
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
					recoveryEmail.Visibility = ViewStates.Visible;
					recoveryEmail.Text = "";
					buttonDone.Enabled = false;
				}
				else
				{
					passwordConfirm.Visibility = ViewStates.Gone;
					recoveryEmail.Visibility = ViewStates.Gone;
				}
			};

			// yes and no checkboxes

			check_remember_me = fragment.FindViewById<CheckBox>(Resource.Id.check_remember_me);
			check_remember_me.SetTypeface(HomeActivity.gothamFont, TypefaceStyle.Normal);

			dialog = new ProgressDialog(this.Activity);
			dialog.SetMessage(GetString(Resource.String.signin_message_signing_in));
			dialog.SetCancelable(false);

			Button btn_help = fragment.FindViewById<Button>(Resource.Id.btn_help);
			btn_help.Click += (sender, args) =>
			{
				Intent emailIntent = new Intent(Intent.ActionSendto,
					Android.Net.Uri.FromParts("mailto", App.EmailHelp, null));
				emailIntent.PutExtra(Intent.ExtraSubject, GetString(Resource.String.signin_help_email_subject));
				StartActivity(Intent.CreateChooser(emailIntent, GetString(Resource.String.signin_help_chooser_title)));
			};
			Button btn_about = fragment.FindViewById<Button>(Resource.Id.btn_about);
			btn_about.Click += (sender, args) =>
			{
				StartActivity(new Intent(Intent.ActionView, Android.Net.Uri.Parse(App.WebsiteAbout)));
			};
			Button btn_report = fragment.FindViewById<Button>(Resource.Id.btn_report);
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

			return fragment;
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
						HomeActivity.analytics.PostSessionError("signinfailed-" + errMsg);
						this.Activity.RunOnUiThread(() =>
                        {
                            progress.Visibility = ViewStates.Invisible;
                            //Toast.MakeText(this, "could not register: " + errMsg, ToastLength.Short).Show();
								Toast.MakeText(this.Activity, GetString(Resource.String.signin_message_error_signing_in), ToastLength.Short).Show();
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
				ISharedPreferences _sharedPref = PreferenceManager.GetDefaultSharedPreferences(this.Activity);
                _sharedPref.Edit().PutString("username", BlahguaAPIObject.Current.UserName).Commit();
                _sharedPref.Edit().PutString("password", BlahguaAPIObject.Current.UserPassword).Commit();
            }

            // do the rest
			HomeActivity.analytics.PostLogin();
			this.Activity.RunOnUiThread(() =>
            {
                progress.Visibility = ViewStates.Invisible;
					//Android.Support.V4.App.FragmentManager 
					if(FragmentManager.BackStackEntryCount >0)
					{
						FragmentManager.PopBackStack();
						HomeActivity homeActivity = (HomeActivity)this.Activity;
						homeActivity.RestoreTitle();
						homeActivity.ResumeScrolling();
					}
					dialog.Hide();
				//this.Activity.Finish();
            });
        }

        private void DoCreateAccount()
        {
			login.Text = login.Text.Trim ();
			password.Text = password.Text.Trim ();

            if (login.Text.Length == 0)
            {
				this.Activity.RunOnUiThread(() =>
                {
						Toast.MakeText(this.Activity, GetString(Resource.String.signin_message_enter_login), ToastLength.Short).Show();
                });
                return;
            }
            if (password.Text.Length == 0)
            {
				this.Activity.RunOnUiThread(() =>
                {
						Toast.MakeText(this.Activity, GetString(Resource.String.signin_message_enter_pass), ToastLength.Short).Show();
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
				this.Activity.RunOnUiThread(() => {
					Toast.MakeText(this.Activity, GetString(Resource.String.signin_message_passwords_must_match), ToastLength.Short).Show();
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
							HomeActivity.analytics.PostRegisterUser();
                        HandleUserSignIn();
                    }
                    else
                    {
							HomeActivity.analytics.PostSessionError("registerfailed-" + errMsg);
							this.Activity.RunOnUiThread(() =>
                        {
                            progress.Visibility = ViewStates.Invisible;
									Toast.MakeText(this.Activity, "could not register: " + errMsg, ToastLength.Short).Show();
							dialog.Hide();
                        });
                    }
                }
                );
            }
        }
	}
}
