using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
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

namespace BlahguaMobile.AndroidClient.Screens
{
    class UserProfileBadgesFragment : Fragment
    {
        public static UserProfileBadgesFragment NewInstance()
        {
            return new UserProfileBadgesFragment { Arguments = new Bundle() };
        }

        private readonly string TAG = "UserProfileBadgesFragment";

        private LinearLayout list, no_badges;
        private ScrollView list_container;

        private View new_block;
        private Button btn_done;
        private EditText edit;
        private ProgressBar progressBar1;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View fragment = inflater.Inflate(Resource.Layout.fragment_userprofile_badges, null);
            //if (container == null)
            //{
            //    Log.Debug(TAG, "Dialer Fragment is in a view without container");
            //    return null;
            //}
            list = fragment.FindViewById<LinearLayout>(Resource.Id.list);
            no_badges = fragment.FindViewById<LinearLayout>(Resource.Id.no_badges);
            list_container = fragment.FindViewById<ScrollView>(Resource.Id.list_container);

            new_block = fragment.FindViewById<LinearLayout>(Resource.Id.new_badge);
            btn_done = new_block.FindViewById<Button>(Resource.Id.btn_done);
            edit = new_block.FindViewById<EditText>(Resource.Id.edit);
            progressBar1 = new_block.FindViewById<ProgressBar>(Resource.Id.progressBar1);

            btn_done.Click += btn_done_Click;

            initDimensions();

            UpdateBadgeArea();

            return fragment;
        }


        public override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (resultCode == Result.Ok)
            {

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

                foreach (BadgeReference b in BlahguaAPIObject.Current.CurrentUser.Badges)
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

        private void InsertElementForBadge(BadgeReference badge, LinearLayout rowLayout, int column)
        {
            String imageUrl = badge.BadgeImage;
            String badgeTitle = badge.BadgeName;

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
            edit.Enabled = false;
            btn_done.Visibility = ViewStates.Gone;
            progressBar1.Visibility = ViewStates.Visible;
            BlahguaAPIObject.Current.GetBadgeAuthorities((authList) =>
            {
                string badgeId = authList[0]._id;
                string emailAddr = edit.Text;

                BlahguaAPIObject.Current.GetEmailBadgeForUser(badgeId, emailAddr, (ticket) =>
                {
                    Activity.RunOnUiThread(() =>
                    {
                        progressBar1.Visibility = ViewStates.Gone;
                        if (ticket == "")
                        {
                            // fail
                            Toast.MakeText(Activity, "The authority currently has no badges for that email address.  Please try again in the future.", ToastLength.Short).Show();
                            edit.Enabled = true;
                            btn_done.Visibility = ViewStates.Visible;
                            //App.analytics.PostRequestBadge(badgeId);
                            //App.analytics.PostBadgeNoEmail(emailAddr);
                        }
                        else
                        {
                            // success
                            edit.Text = "";
                            ticketStr = ticket;
                            newBadgeStage = 1;
                            btn_done.Text = "DONE";
                            edit.Hint = "Type confirm code";
                            edit.Enabled = true;
                            btn_done.Visibility = ViewStates.Visible;
                            //App.analytics.PostRequestBadge(badgeId);
                        }
                    });
                }
                );

            }
            );
        }

        private void DoValidate()
        {
            edit.Enabled = false;
            btn_done.Visibility = ViewStates.Gone;
            progressBar1.Visibility = ViewStates.Visible;

            string valString = edit.Text;

            BlahguaAPIObject.Current.VerifyEmailBadge(valString, ticketStr, (resultStr) =>
            {
                if (resultStr == "")
                {
                    // fail
                    Activity.RunOnUiThread(() =>
                    {
                        progressBar1.Visibility = ViewStates.Gone;
                        Toast.MakeText(Activity, "That validation code was not valid.  Please retry your badging attempt.", ToastLength.Short).Show();
                        edit.Enabled = true;
                        btn_done.Visibility = ViewStates.Visible;
                    });
                    //App.analytics.PostBadgeValidateFailed();
                }
                else
                {
                    // success

                    //App.analytics.PostGotBadge();
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

        private void btn_done_Click(object sender, EventArgs e)
        {
            if (newBadgeStage == 0)
            {
                if (IsValidEmail(edit.Text))
                {
                    DoSubmitEmail();
                }
                else
                {
                    Toast.MakeText(Activity, "Email not valid!", ToastLength.Short).Show();
                }
            }
            else
            {
                if (!String.IsNullOrEmpty(edit.Text))
                {
                    DoValidate();
                }
                else
                {
                    Toast.MakeText(Activity, "Type confirm code first", ToastLength.Short).Show();
                }
            }
        }
        int newBadgeStage = 0;
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

                newBadgeStage = 0;
                btn_done.Text = "NEXT";
                edit.Hint = "Type email address";

                ValueAnimator mAnimator = slideAnimator(new_block, 0, new_block.MeasuredHeight, false);
                mAnimator.Start();

                //initBlahCreationSlidingMenu();
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

                //initSlidingMenu();
                //if (BlahguaAPIObject.Current.CurrentUser != null)
                //{
                //    SlidingMenu.Mode = MenuMode.LeftRight;
                //}
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