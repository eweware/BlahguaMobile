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

namespace BlahguaMobile.Android
{
	[Activity]
	public class LoginActivity : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
            base.OnCreate(bundle);

            RequestWindowFeature(WindowFeatures.NoTitle);

			// Set our view from the "main" layout resource
            SetContentView(Resource.Layout.LoginScreen);
            EditText password_confirm = FindViewById<EditText>(Resource.Id.password_confirm);
            password_confirm.Visibility = ViewStates.Gone;

            Button button = FindViewById<Button>(Resource.Id.btn_cancel);
            button.Click += delegate
            {
                Finish();
            };
            CheckBox check_create_acc = FindViewById<CheckBox>(Resource.Id.check_create_acc);
            check_create_acc.CheckedChange += delegate
            {
                if (check_create_acc.Checked)
                {
                    password_confirm.Visibility = ViewStates.Visible;
                }
                else
                {
                    password_confirm.Visibility = ViewStates.Gone;
                }
            };

            // yes and no checkboxes

            CheckBox check_yes = FindViewById<CheckBox>(Resource.Id.check_yes);
            CheckBox check_no = FindViewById<CheckBox>(Resource.Id.check_no);
            check_yes.CheckedChange += delegate
            {
                if (check_yes.Checked)
                {
                    check_no.Checked = false;
                }
            };
            check_no.CheckedChange += delegate
            {
                if (check_no.Checked)
                {
                    check_yes.Checked = false;
                }
            };
		}

	}
}


