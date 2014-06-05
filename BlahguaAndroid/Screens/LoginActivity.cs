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

        private CheckBox check_remember_me;
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
            CheckBox check_create_acc = FindViewById<CheckBox>(Resource.Id.check_create_acc);
            Button buttonCancel = FindViewById<Button>(Resource.Id.btn_cancel);
            Button buttonDone = FindViewById<Button>(Resource.Id.btn_done);

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
                        //App.analytics.PostSessionError("signinfailed-" + errMsg);
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
            if (check_remember_me.Checked)
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
                        //App.analytics.PostRegisterUser();
                        HandleUserSignIn();
                    }
                    else
                    {
                        //App.analytics.PostSessionError("registerfailed-" + errMsg);
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


