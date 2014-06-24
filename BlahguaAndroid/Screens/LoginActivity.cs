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

namespace BlahguaMobile.AndroidClient
{
	[Activity]
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

            instance = this;

            RequestWindowFeature(WindowFeatures.NoTitle);

			// Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_login);

            progress = FindViewById<ProgressBar>(Resource.Id.progressBar1);
            login = FindViewById<EditText>(Resource.Id.login);
            password = FindViewById<EditText>(Resource.Id.password);
            passwordConfirm = FindViewById<EditText>(Resource.Id.password_confirm);
            login.TextChanged += edit_TextChanged;
            password.TextChanged += edit_TextChanged;
            passwordConfirm.TextChanged += edit_TextChanged;

            check_create_acc = FindViewById<CheckBox>(Resource.Id.check_create_acc);
            Button buttonCancel = FindViewById<Button>(Resource.Id.btn_cancel);
            buttonDone = FindViewById<Button>(Resource.Id.btn_done);
            buttonDone.Enabled = false;

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

            dialog = new ProgressDialog(this);
            dialog.SetMessage("Signing in...");
            dialog.SetCancelable(false);

            Button btn_help = FindViewById<Button>(Resource.Id.btn_help);
            btn_help.Click += (sender, args) =>
            {
                Intent emailIntent = new Intent(Intent.ActionSendto,
                    Android.Net.Uri.FromParts("mailto", App.EmailHelp, null));
                emailIntent.PutExtra(Intent.ExtraSubject, "Help Me");
                StartActivity(Intent.CreateChooser(emailIntent, "Send email..."));
            };
            Button btn_about = FindViewById<Button>(Resource.Id.btn_about);
            btn_about.Click += (sender, args) =>
            {
                StartActivity(new Intent(Intent.ActionView, Android.Net.Uri.Parse(App.WebsiteAbout)));
            };
		}

        private void edit_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            if (login.Text.Length == 0 || password.Text.Length == 0 || (check_create_acc.Checked && passwordConfirm.Text.Length == 0))
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
                    dialog.Hide();
                    if (errMsg == null)
                        HandleUserSignIn();
                    else
                    {
                        //MainActivity.analytics.PostSessionError("signinfailed-" + errMsg);
                        RunOnUiThread(() =>
                        {
                            progress.Visibility = ViewStates.Invisible;
                            Toast.MakeText(this, "could not register: " + errMsg, ToastLength.Short).Show();
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
            RunOnUiThread(() =>
            {
                progress.Visibility = ViewStates.Invisible;
                Finish();
            });
        }

        private void DoCreateAccount()
        {
            if (login.Text.Length == 0)
            {
                RunOnUiThread(() =>
                {
                    Toast.MakeText(this, "Enter username", ToastLength.Short).Show();
                });
                return;
            }
            if (password.Text.Length == 0)
            {
                RunOnUiThread(() =>
                {
                    Toast.MakeText(this, "Enter password", ToastLength.Short).Show();
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
                    Toast.MakeText(this, "Passwords must match", ToastLength.Short).Show();
                });
            }
            else
            {
                progress.Visibility = ViewStates.Visible;
                dialog.Show();
                BlahguaAPIObject.Current.Register(BlahguaAPIObject.Current.UserName, BlahguaAPIObject.Current.UserPassword, BlahguaAPIObject.Current.AutoLogin, (errMsg) =>
                {
                    dialog.Hide();
                    if (errMsg == null)
                    {
                        //MainActivity.analytics.PostRegisterUser();
                        HandleUserSignIn();
                    }
                    else
                    {
                        //MainActivity.analytics.PostSessionError("registerfailed-" + errMsg);
                        RunOnUiThread(() =>
                        {
                            progress.Visibility = ViewStates.Invisible;
                            Toast.MakeText(this, "could not register: " + errMsg, ToastLength.Short).Show();
                        });
                    }
                }
                );
            }
        }
	}
}


