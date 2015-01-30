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
    public class SignUpPage03Fragment : Android.Support.V4.App.Fragment
    {
		private LinearLayout enterEmailView, verificationView;
        private Button checkBtn;
        private Button skipBtn;
        private Button verifyBtn;
        private Button tryAgainBtn;
        private EditText emailAddrField;
        private EditText verifyField;
        private ProgressDialog progressDlg;
        private string currentTicket;
        private BadgeAuthority emailAuthority = null;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            progressDlg = new ProgressDialog(this.Activity);
            progressDlg.SetProgressStyle(ProgressDialogStyle.Spinner);

            ViewGroup rootView = (ViewGroup)inflater.Inflate(Resource.Layout.SignUpPage03, container, false);

			rootView.FindViewById<TextView> (Resource.Id.textView1).SetTypeface (HomeActivity.gothamFont, TypefaceStyle.Normal);
			rootView.FindViewById<TextView> (Resource.Id.textView2).SetTypeface (HomeActivity.gothamFont, TypefaceStyle.Normal);

			rootView.FindViewById<TextView> (Resource.Id.titleText).SetTypeface (HomeActivity.gothamFont, TypefaceStyle.Normal);
            emailAddrField = rootView.FindViewById<EditText>(Resource.Id.emailAddrField);
			emailAddrField.SetTypeface(HomeActivity.gothamFont, TypefaceStyle.Normal);
            emailAddrField.AfterTextChanged += HandleTextValueChanged;


            checkBtn = rootView.FindViewById<Button>(Resource.Id.checkbtn);
			checkBtn.SetTypeface(HomeActivity.gothamFont, TypefaceStyle.Normal);
            checkBtn.Click += (s, e) =>
                {
                    {
                        progressDlg.SetTitle("contacting badge authority...");
                        progressDlg.Show();
                        // submit email
                        this.checkBtn.Enabled = false;
                        this.skipBtn.Enabled = false;
                        FirstRunActivity.emailAddress = this.emailAddrField.Text.Trim();
                        string authId = emailAuthority._id;

                        // make formal request
                        BlahguaAPIObject.Current.GetEmailBadgeForUser(authId, FirstRunActivity.emailAddress, (ticket) =>
                        {
                            Activity.RunOnUiThread(() =>
                            {
                                progressDlg.Hide();
                                if (ticket == String.Empty)
                                {
                                    DisplayAlert("No Result", "No badges are available for your domain.  You can try again later from your profile page.", "got it", () =>
                                    {
                                        ((FirstRunActivity)this.Activity).Finish();
                                    });

                                }
                                else if (ticket == "existing")
                                {
                                    HomeActivity.analytics.PostBadgeNoEmail(FirstRunActivity.emailAddress);
                                    DisplayAlert("Issued", "A badge has previously been issued to this email address.", "got it", () =>
                                    {
                                        ((FirstRunActivity)this.Activity).Finish();
                                    });
                                }
                                else if (ticket == "invalid")
                                {
                                    HomeActivity.analytics.PostBadgeNoEmail(FirstRunActivity.emailAddress);
                                    DisplayAlert("Invalid email", "We were not able to send mail to that email address.", "try again", () =>
                                    {
                                        PrepPhaseOne();
                                        skipBtn.Enabled = true;
                                    });
                                }
                                else
                                {
                                    HomeActivity.analytics.PostRequestBadge(authId);
                                    currentTicket = ticket;
                                    DisplayAlert("Badges Success", "Badges are available for your email address.", "next", () =>
                                    {
                                        PrepPhaseTwo();
                                    });
                                }
                            });
                        });
                    };
                };

            skipBtn = rootView.FindViewById<Button>(Resource.Id.skipBtn);
			skipBtn.SetTypeface(HomeActivity.gothamFont, TypefaceStyle.Normal);
            skipBtn.Click += (s, e) =>
            {
                ((FirstRunActivity)this.Activity).Finish();
            };

			rootView.FindViewById<TextView> (Resource.Id.titleText).SetTypeface (HomeActivity.gothamFont, TypefaceStyle.Normal);
            verifyField = rootView.FindViewById<EditText>(Resource.Id.verifyField);
			verifyField.SetTypeface(HomeActivity.gothamFont, TypefaceStyle.Normal);

            verifyField.AfterTextChanged += HandleTextValueChanged;

            verifyBtn = rootView.FindViewById<Button>(Resource.Id.verifyBtn);
			verifyBtn.SetTypeface(HomeActivity.gothamFont, TypefaceStyle.Normal);
            verifyBtn.Click += (s, e) =>
            {
                BlahguaAPIObject.Current.VerifyEmailBadge(verifyField.Text, currentTicket, (result) =>
                {
                    Activity.RunOnUiThread(() =>
                    {
                        if (result == "fail")
                        {
                            HomeActivity.analytics.PostBadgeValidateFailed();
                            DisplayAlert("Verification Failure", "That validation code was not valid.  Please retry your badging attempt.", "retry", () =>
                            {
                                verifyField.SelectAll();
                            });
                        }
                        else
                        {
                            HomeActivity.analytics.PostGotBadge();
                            BlahguaAPIObject.Current.RefreshUserBadges(null);
                            DisplayAlert("Verified", "You are ready to use badges in Heard.", "let's go", () =>
                            {
                                ((FirstRunActivity)this.Activity).Finish();
                            });
                        }
                    });
                });
            };

            tryAgainBtn = rootView.FindViewById<Button>(Resource.Id.tryAgainBtn);
			tryAgainBtn.SetTypeface(HomeActivity.gothamFont, TypefaceStyle.Normal);
            tryAgainBtn.Click += (s, e) =>
            {
                verifyField.Text = "";
                skipBtn.Enabled = true;
                PrepPhaseOne();
            };

			enterEmailView = rootView.FindViewById<LinearLayout> (Resource.Id.enterEmailView);
			verificationView = rootView.FindViewById<LinearLayout> (Resource.Id.verifyView);

            verificationView.Visibility = ViewStates.Gone;
            checkBtn.Enabled = false;
            skipBtn.Enabled = false;

            enterEmailView.Visibility = ViewStates.Visible;
            verificationView.Visibility = ViewStates.Gone;
            emailAddrField.Text = FirstRunActivity.emailAddress;
            InitBadgeAuthorities();

            return rootView;
        }

        private void InitBadgeAuthorities()
        {
            BlahguaAPIObject.Current.GetBadgeAuthorities((authorities) =>
            {
                Activity.RunOnUiThread(() =>
                {
                    if ((authorities == null) || (authorities.Count == 0))
                    {
                        Console.WriteLine("Error:  no badge authories available");

                        ((FirstRunActivity)this.Activity).Finish();
                    }
                    else
                    {
                        emailAuthority = authorities[0];
                        skipBtn.Enabled = true;
                    }
                });
            });
        }

        private void PrepPhaseOne()
        {
            Activity.RunOnUiThread(() =>
            {
                enterEmailView.Visibility = ViewStates.Visible;
                verificationView.Visibility = ViewStates.Gone;
            });
        }

        private void PrepPhaseTwo()
        {
            Activity.RunOnUiThread(() =>
            {
                verifyField.Text = "";
                enterEmailView.Visibility = ViewStates.Gone;
                verificationView.Visibility = ViewStates.Visible;
            });
        }

        void HandleTextValueChanged(object sender, EventArgs e)
        {
            string emailText = emailAddrField.Text;
            string verifyText = verifyField.Text;

            if (String.IsNullOrEmpty(emailText))
                checkBtn.Enabled = false;
            else
                checkBtn.Enabled = true;

            if (String.IsNullOrEmpty(verifyText))
                verifyBtn.Enabled = false;
            else
                verifyBtn.Enabled = true;
        }

        public void DisplayAlert(string titleString, string descString, string btnName = "ok", Action action = null)
        {
            Activity.RunOnUiThread(() =>
            {
                AlertDialog alert = new AlertDialog.Builder(this.Activity).Create();
                alert.SetTitle(titleString);
                alert.SetMessage(descString);
                alert.SetButton("ok", (sender, args) =>
                {
                    if (action != null)
                        action.Invoke();
                });
                alert.Show();
            });

        }

    }
}