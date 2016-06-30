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
using Android.Text;
using BlahguaMobile.AndroidClient.HelpingClasses;
using Android.Support.V4.App;

namespace BlahguaMobile.AndroidClient.Screens
{
    public class UserProfileBadgesFragment : Android.Support.V4.App.Fragment
    {
        public static UserProfileBadgesFragment NewInstance()
        {
            return new UserProfileBadgesFragment { Arguments = new Bundle() };
        }

        private LinearLayout list, no_badges;
        private ScrollView list_container;

        private View new_block;
        private Button btn_submit, btn_verify, btn_request;
        private EditText emailField, codeField;
        private ProgressBar progressBar1;
        private LinearLayout submitSection, verifySection, requestSection;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
			HomeActivity.analytics.PostPageView("/self/badges");
            View fragment = inflater.Inflate(Resource.Layout.fragment_userprofile_badges, null);
           
            list = fragment.FindViewById<LinearLayout>(Resource.Id.list);
            submitSection = fragment.FindViewById<LinearLayout>(Resource.Id.section_badge_submit);
            verifySection = fragment.FindViewById<LinearLayout>(Resource.Id.section_badge_confirm);
            requestSection = fragment.FindViewById<LinearLayout>(Resource.Id.section_badge_request);
            no_badges = fragment.FindViewById<LinearLayout>(Resource.Id.no_badges);
            list_container = fragment.FindViewById<ScrollView>(Resource.Id.list_container);

            submitSection.Visibility = ViewStates.Visible;
            verifySection.Visibility = ViewStates.Gone;
            requestSection.Visibility = ViewStates.Gone;

            new_block = fragment.FindViewById<View>(Resource.Id.new_badge);

            btn_submit = new_block.FindViewById<Button>(Resource.Id.btn_done);
            btn_submit.Enabled = false;
            btn_submit.Click += btn_submit_Click;

            btn_verify = new_block.FindViewById<Button>(Resource.Id.btn_verify_done);
            btn_verify.Enabled = false;
            btn_verify.Click += btn_verify_Click;


            btn_request = new_block.FindViewById<Button>(Resource.Id.btn_request);
            btn_request.Enabled = true;
            btn_request.Click += btn_request_Click;


            emailField = new_block.FindViewById<EditText>(Resource.Id.edit);
            emailField.TextChanged += edit_TextChanged;

            codeField = new_block.FindViewById<EditText>(Resource.Id.badge_codeField);
            codeField.TextChanged += badge_codeField_TextChanged;

            TextView privacy = new_block.FindViewById<TextView>(Resource.Id.text_privacy_state);
            privacy.TextFormatted = Html.FromHtml("<b>" + GetString(Resource.String.new_badge_title_privacy_title) + "</b> " + GetString(Resource.String.new_badge_title_privacy_statement));

            //TextView text_new_badge_title = new_block.FindViewById<TextView>(Resource.Id.text_new_badge_title);

            UiHelper.SetGothamTypeface(TypefaceStyle.Normal, btn_submit, btn_request, btn_verify);//, edit, privacy, text_new_badge_title);

            progressBar1 = new_block.FindViewById<ProgressBar>(Resource.Id.progressBar1);

            initDimensions();

            UpdateBadgeArea();

            return fragment;
        }

        private void edit_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            if (emailField.Text.Length == 0)
            {
                btn_submit.Enabled = false;
            }
            else
            {
                btn_submit.Enabled = true;
            }
        }

        private void badge_codeField_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            if (codeField.Text.Length == 0)
            {
                btn_verify.Enabled = false;
            }
            else
            {
                btn_verify.Enabled = true;
            }
        }

        #region Badges Init
        private LinearLayout createNewRowLayout()
        {
            LinearLayout rowLayout = new LinearLayout(Activity);
            rowLayout.Orientation = Orientation.Horizontal;
            LinearLayout.LayoutParams layParams = new LinearLayout.LayoutParams(
                LinearLayout.LayoutParams.MatchParent,
                LinearLayout.LayoutParams.WrapContent);
            layParams.SetMargins((int)screenMargin, screenMargin, screenMargin, screenMargin);
            rowLayout.LayoutParameters = layParams;

            return rowLayout;
        }
        private void LoadBadges()
        {
            if (BlahguaAPIObject.Current.CurrentUser.B != null)
            {
                int count = 0;

                LinearLayout rowLayout = createNewRowLayout();

                foreach (BadgeRecord b in BlahguaAPIObject.Current.CurrentUser.B)
                {
                    InsertElementForBadge(b, rowLayout, count);
                    count++;
                    if (count == numOfColumns)
                    {
                        count = 0;
                        InsertRowLayout(rowLayout);
                        rowLayout = createNewRowLayout();
                    }
                }

                if (count != 0)
                {
                    InsertRowLayout(rowLayout);
                }
            }
        }

        private readonly int numOfColumns = 3;
        //private int currentColumn = 1;
        //private int currentYPosition = 0;

        private int badgeSize = 0;
        private int imageSize = 0;
        private int badgeMargin = 10;
        private int screenMargin = 10;

        private void initDimensions()
        {
            int screenWidth = Resources.DisplayMetrics.WidthPixels - screenMargin * 2;

            badgeSize = screenWidth / 3;
            imageSize = badgeSize - badgeMargin * 2;
        }

        private void InsertRowLayout(LinearLayout rowLayout)
        {
            list.AddView(rowLayout);
        }

        private void InsertElementForBadge(BadgeRecord badge, LinearLayout rowLayout, int column)
        {
            String imageUrl = badge.URL;
            String badgeTitle = badge.N;

            //int x = column * badgeSize;

            RelativeLayout.LayoutParams layoutParams =
                new RelativeLayout.LayoutParams(
                    badgeSize,
                    RelativeLayout.LayoutParams.WrapContent);
            LinearLayout.LayoutParams imageParams =
                new LinearLayout.LayoutParams(
                    imageSize,
                    imageSize);
            //layoutParams.SetMargins((int)x, 0, 0, 0);

            var control = Activity.LayoutInflater.Inflate(Resource.Layout.uiitem_badge, null);
            var title = control.FindViewById<TextView>(Resource.Id.title);
			title.SetTypeface(HomeActivity.gothamFont, TypefaceStyle.Normal);
            var image = control.FindViewById<ImageView>(Resource.Id.image);
            image.LayoutParameters = imageParams;

            control.LayoutParameters = layoutParams;

            title.Text = badgeTitle;

            if (imageUrl != null)
            {
                Activity.RunOnUiThread(() =>
                {
                    image.SetUrlDrawable(imageUrl);
                    //image.SetImageURI(Android.Net.Uri.Parse(imageURL));
                });
            }

            //control.Click += delegate
            //{
            //    OpenBadge
            //};

            Activity.RunOnUiThread(() =>
            {
                rowLayout.AddView(control);
            });

        }

        private void UpdateBadgeArea()
        {
            if (BlahguaAPIObject.Current.CurrentUser.B != null)
            {
                list_container.Visibility = ViewStates.Visible;
                no_badges.Visibility = ViewStates.Gone;

                LoadBadges();
                //list_container.RequestLayout();
            }
            else
            {
                list_container.Visibility = ViewStates.Gone;
                no_badges.Visibility = ViewStates.Visible;
            }
        }
        #endregion

        //////////////////////////////////////

        private String ticketStr = String.Empty;
        private void DoSubmitEmail()
        {
            emailField.Enabled = false;
            btn_submit.Visibility = ViewStates.Gone;
            progressBar1.Visibility = ViewStates.Visible;
            BlahguaAPIObject.Current.GetBadgeAuthorities((authList) =>
            {
                long badgeId = authList[0]._id;
                string emailAddr = emailField.Text;

                BlahguaAPIObject.Current.GetEmailBadgeForUser(badgeId, emailAddr, (ticket) =>
                {
                    Activity.RunOnUiThread(() =>
                    {
                        progressBar1.Visibility = ViewStates.Gone;
                        if (ticket == "")
                        {
                            // fail
                            Toast.MakeText(Activity, "The authority currently has no badges for that email address.", ToastLength.Short).Show();
                            emailField.Enabled = true;
                            btn_submit.Visibility = ViewStates.Visible;
										HomeActivity.analytics.PostRequestBadge(badgeId);
										HomeActivity.analytics.PostBadgeNoEmail(emailAddr);
                            submitSection.Visibility = ViewStates.Gone;
                            requestSection.Visibility = ViewStates.Visible;
                        }
                        else
                        {
                            // success
                            emailField.Text = "";
                            ticketStr = ticket;
                            submitSection.Visibility = ViewStates.Gone;
                            verifySection.Visibility = ViewStates.Visible;
                        }
                    });
                }
                );

            }
            );
        }

        private void DoValidate()
        {
            codeField.Enabled = false;
            btn_verify.Visibility = ViewStates.Gone;
            progressBar1.Visibility = ViewStates.Visible;

            string valString = codeField.Text;

            BlahguaAPIObject.Current.VerifyEmailBadge(valString, ticketStr, (resultStr) =>
            {
                if (resultStr == "fail")
                {
                    // fail
                    Activity.RunOnUiThread(() =>
                    {
                        progressBar1.Visibility = ViewStates.Gone;
                        Toast.MakeText(Activity, "That validation code was not valid.  Please retry your badging attempt.", ToastLength.Short).Show();
                        codeField.Enabled = true;
                        codeField.SelectAll();
                        btn_verify.Visibility = ViewStates.Visible;
                    });
						HomeActivity.analytics.PostBadgeValidateFailed();
                }
                else
                {
                    // success

						HomeActivity.analytics.PostGotBadge();
                    BlahguaAPIObject.Current.RefreshUserBadges((theStr) =>
                    {
                        Activity.RunOnUiThread(() =>
                        {
                            progressBar1.Visibility = ViewStates.Gone;
                            Toast.MakeText(Activity, "Badging successful!", ToastLength.Short).Show();

                            triggerNewBlock();

                            UpdateBadgeArea();
                        });
                    }
                    );

                }
            }
            );

        }
        //////////////////////////////////////
        //////////

        private void btn_submit_Click(object sender, EventArgs e)
        {
            if (IsValidEmail(emailField.Text))
            {
                DoSubmitEmail();
            }
            else
            {
                Toast.MakeText(Activity, "Email not valid!", ToastLength.Short).Show();
            }
           
        }

        private void btn_verify_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(codeField.Text))
            {
                DoValidate();
            }
            else
            {
                Toast.MakeText(Activity, "Type confirm code first", ToastLength.Short).Show();
            }
        }

        private void btn_request_Click(object sender, EventArgs e)
        {
            var addr = new System.Net.Mail.MailAddress(emailField.Text);
            string emailAddr = addr.Address;
            string domainName = addr.Host;
            BlahguaAPIObject.Current.RequestBadgeForDomain(emailAddr, domainName, (resultStr) =>
                {
                    if (resultStr == "ok")
                    {
                        Toast.MakeText(Activity, "Domain Requested", ToastLength.Short).Show();
                    }
                });
        }

        bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public void triggerNewBlock()
        {
            if (new_block.Visibility.Equals(ViewStates.Gone))
            {
                //set Visible
                new_block.Visibility = ViewStates.Visible;
                int widthSpec = View.MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified);
                int heightSpec = View.MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified);
                new_block.Measure(widthSpec, heightSpec);


                ValueAnimator mAnimator = slideAnimator(new_block, 0, new_block.MeasuredHeight, false);
                mAnimator.Start();
                mAnimator.AnimationEnd += (object IntentSender, EventArgs arg) =>
                {
                    if (btn_submit.Bottom + new_block.PaddingBottom > new_block.MeasuredHeight)
                    {
                        slideAnimator(new_block,
                            new_block.MeasuredHeight, btn_submit.Bottom + new_block.PaddingBottom * 2,
                            false).Start();
                    }
                };
            }
            else
            {
                //collapse();
                int finalHeight = new_block.Height;

                ValueAnimator mAnimator = slideAnimator(new_block, finalHeight, 0, false);
                mAnimator.Start();
                mAnimator.AnimationEnd += (object IntentSender, EventArgs arg) =>
                {
                    new_block.Visibility = ViewStates.Gone;
                };

            }
        }

        private ValueAnimator slideAnimator(View layout, int start, int end, bool animatingWidth)
        {
            ValueAnimator animator = ValueAnimator.OfInt(start, end);
            //ValueAnimator animator2 = ValueAnimator.OfInt(start, end);
            //  animator.AddUpdateListener (new ValueAnimator.IAnimatorUpdateListener{
            animator.Update +=
                (object sender, ValueAnimator.AnimatorUpdateEventArgs e) =>
                {
                    //  int newValue = (int)
                    //e.Animation.AnimatedValue; // Apply this new value to the object being animated.
                    //  myObj.SomeIntegerValue = newValue; 
                    var value = (int)animator.AnimatedValue;
                    ViewGroup.LayoutParams layoutParams = layout.LayoutParameters;
                    if (animatingWidth)
                        layoutParams.Width = value;
                    else
                        layoutParams.Height = value;
                    layout.LayoutParameters = layoutParams;

                };


            //      });
            return animator;
        }
    }
}