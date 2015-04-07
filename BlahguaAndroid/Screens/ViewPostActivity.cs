using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Util;
using Android.Content.PM;
using BlahguaMobile.BlahguaCore;
using System.Timers;
using System.ComponentModel;
using Android.Graphics.Drawables;
using System;
using SlidingMenuSharp.App;
using SlidingMenuSharp;
using BlahguaMobile.AndroidClient.Screens;
using Android.App;
using BlahguaMobile.AndroidClient.HelpingClasses;
using Android.Graphics;
using Android.Text;
using Android.Text.Style;
using System.Collections.Generic;

namespace BlahguaMobile.AndroidClient
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait, UiOptions = Android.Content.PM.UiOptions.SplitActionBarWhenNarrow)]
    public class ViewPostActivity : Activity, GestureDetector.IOnGestureListener
    {
        private ViewPostCommentsFragment commentsFragment;
        private ViewPostSummaryFragment summaryFragment;
        private ViewPostStatsFragment statsFragment;
        private bool isFromCommentBtn = false;
        private Fragment curFragment = null;

		private string[] badgeItemNames = null;
		private bool[]	badgeItemBools = null;

        private GestureDetector _gestureDetector;


        private ActionBar.Tab summaryTab, commentTab, statsTab;



		protected override void OnCreate (Bundle bundle)
		{
            this.Window.SetUiOptions(UiOptions.SplitActionBarWhenNarrow);
			this.Window.DecorView.SystemUiVisibility = StatusBarVisibility.Hidden;
            this.Window.AddFlags(WindowManagerFlags.Fullscreen);
            ActionBar.NavigationMode = ActionBarNavigationMode.Tabs;
            base.OnCreate(bundle);
			this.ActionBar.SetDisplayHomeAsUpEnabled(false);
			this.ActionBar.SetHomeButtonEnabled(false);
			this.ActionBar.SetDisplayShowHomeEnabled(false);
			this.ActionBar.SetDisplayShowTitleEnabled (false);
			SetContentView (Resource.Layout.activity_viewpost);


            _gestureDetector = new GestureDetector(this, this);
            

            this.Title = "";


            btn_summary_Click(null, null);
			HomeActivity.analytics.PostPageView("/blah");

            // set up tabs
            summaryTab = ActionBar.NewTab();
            summaryTab.SetIcon(Resource.Drawable.btn_summary);
            summaryTab.TabSelected += btn_summary_Click;
            ActionBar.AddTab(summaryTab);

            commentTab = ActionBar.NewTab();
            commentTab.SetIcon(Resource.Drawable.btn_comments);
            commentTab.TabSelected += btn_comments_Click;
            ActionBar.AddTab(commentTab);

            statsTab = ActionBar.NewTab();
            statsTab.SetIcon(Resource.Drawable.btn_stats);
            statsTab.TabSelected += btn_stats_Click;
            ActionBar.AddTab(statsTab);
			this.ActionBar.SetStackedBackgroundDrawable(new Android.Graphics.Drawables.ColorDrawable(Resources.GetColor(Resource.Color.heard_black)));
			this.ActionBar.SetSplitBackgroundDrawable(new Android.Graphics.Drawables.ColorDrawable(Resources.GetColor(Resource.Color.heard_teal)));
			this.ActionBar.SetBackgroundDrawable(new Android.Graphics.Drawables.ColorDrawable(Resources.GetColor(Resource.Color.heard_teal)));
           
        }

        public bool OnScroll(MotionEvent e1, MotionEvent e2, float distanceX, float distanceY)
        {
            return false;
        }

        public bool OnSingleTapUp(MotionEvent e1)
        {
            return false;
        }

        public void OnLongPress(MotionEvent e1)
        {
            //return false;
        }

        public void OnShowPress(MotionEvent e1)
        {
            //return false;
        }

        public bool OnFling(MotionEvent e1, MotionEvent e2, float distanceX, float distanceY)
        {
            float maxRange = e1.Device.MotionRanges[0].Max;

            float minEdge = maxRange * .1f;
            float maxEdge = maxRange * .9f;

            if ((e1.GetX() < minEdge) && (e2.GetX() > maxRange * .3f))
            {
                swipeRightEvent();
                return true;
            }
            else if ((e1.GetX() > maxEdge) && (e2.GetX() < maxRange * .7f))
            {
                swipeLeftEvent();
                return true;
            }
            else
                return false;
        }

        public bool OnDown(MotionEvent e1)
        {
            return false;
        }


		private void MultiListClicked(object sender, DialogMultiChoiceClickEventArgs args)
		{
			if (args.Which == 0)
				BlahguaAPIObject.Current.CreateCommentRecord.XX = !args.IsChecked;
			else if (args.Which == 1)
				BlahguaAPIObject.Current.CreateCommentRecord.XXX = args.IsChecked;
			else {
				int whichBadge = args.Which - 2;
				string badgeId = BlahguaAPIObject.Current.CurrentUser.Badges [whichBadge].ID;
				if (args.IsChecked) {
					// add badge
					if (BlahguaAPIObject.Current.CreateCommentRecord.BD == null)
						BlahguaAPIObject.Current.CreateCommentRecord.BD = new List<string> ();
					BlahguaAPIObject.Current.CreateCommentRecord.BD.Add (badgeId);
				} else {
					BlahguaAPIObject.Current.CreateCommentRecord.BD.Remove (badgeId);
				}
			}
		}


		private void BadgeOKClicked(Object sender, EventArgs args)
		{
			//Toast.MakeText(this, "Badge accepted!", ToastLength.Short).Show();
		}

		protected override Dialog OnCreateDialog(int id, Bundle args)
		{
			switch(id)
			{
			case HomeActivity.MultiChoiceDialog: 
				{
					UpdateBadgeInfo ();
					var builder = new AlertDialog.Builder (this, Android.App.AlertDialog.ThemeHoloLight);
					builder.SetIcon (Resource.Drawable.ic_launcher);
					builder.SetTitle ("Sign your comment");
					builder.SetMultiChoiceItems (badgeItemNames, badgeItemBools, MultiListClicked);
					builder.SetPositiveButton ("Ok", BadgeOKClicked);

					AlertDialog dlg = builder.Create ();

					return dlg;
				}
				break;
			}
			return base.OnCreateDialog(id, args);
		}

		private void UpdateBadgeInfo()
		{
			BadgeList badges = BlahguaAPIObject.Current.CurrentUser.Badges;

			if (badgeItemNames == null) {
				List<string>	badgeNames = new List<string> ();
				badgeNames.Add ("use profile");
				badgeNames.Add ("mature content");

				if (badges != null) {
					foreach (BadgeReference curBadge in badges) {
						badgeNames.Add (curBadge.BadgeName);
					}
				}
				badgeItemNames = badgeNames.ToArray ();
			}

			// now create the bool list
			badgeItemBools = new bool[badgeItemNames.Length];

			badgeItemBools [0] = !BlahguaAPIObject.Current.CreateCommentRecord.XX;
			badgeItemBools [1] = BlahguaAPIObject.Current.CreateCommentRecord.XXX;

			if (badges != null) {
				int i = 2;
				if (BlahguaAPIObject.Current.CreateCommentRecord.BD == null) {
					foreach (BadgeReference curBadge in badges) {
						badgeItemBools [i++] = false;
					}
				} else {
					foreach (BadgeReference curBadge in badges) {
						badgeItemBools [i++] = BlahguaAPIObject.Current.CreateCommentRecord.BD.Contains (curBadge.ID);
					}
				}
			}

		}

        public override Android.Content.Res.Resources Resources
        {
            get
            {
                return new ResourceFix(base.Resources);
            }
        }

        private class ResourceFix : Android.Content.Res.Resources
        {
            private int targetId = 0;

            public ResourceFix(Android.Content.Res.Resources resources)
                : base(resources.Assets, resources.DisplayMetrics, resources.Configuration)
            {
                
                targetId = Android.Content.Res.Resources.System.GetIdentifier("split_action_bar_is_narrow", "bool", "android");

            }

            public override bool GetBoolean(int id)
            {
                return targetId == id || base.GetBoolean(id);
            }

        }


        public override bool OnPrepareOptionsMenu(IMenu menu)
        {
            try
            {
                menu.Clear();
                if (BlahguaAPIObject.Current.CurrentUser == null)
                    MenuInflater.Inflate(Resource.Menu.blahmenu_signedout, menu);
                else
                {
                    MenuInflater.Inflate(Resource.Menu.BlahMenu, menu);
                    IMenuItem upVote = menu.FindItem(Resource.Id.action_upvote);

                    if (!BlahguaAPIObject.Current.CanComment)
                        menu.FindItem(Resource.Id.action_comment).SetVisible(false);

                    if (BlahguaAPIObject.Current.CurrentBlah != null)
                    {
                        if (BlahguaAPIObject.Current.CurrentUser._id == BlahguaAPIObject.Current.CurrentBlah.A)
                        {
                            // can't vote on own blah
                            upVote.SetEnabled(false);
                            upVote.SetIcon(Resource.Drawable.ic_thumb_up_grey);
                        }
                        else if (BlahguaAPIObject.Current.CurrentBlah.uv == 0)
                        {
                            // user can still vote
                            upVote.SetEnabled(true);
                            upVote.SetIcon(Resource.Drawable.ic_thumb_up_white);
                        }
                        else if (BlahguaAPIObject.Current.CurrentBlah.uv == 1)
                        {
                            // user promoted it
                            upVote.SetEnabled(false);
                            upVote.SetIcon(Resource.Drawable.ic_thumb_up_black);

                        }
                        else
                        {
                            // user demoted it
                            upVote.SetEnabled(false);
                            upVote.SetIcon(Resource.Drawable.ic_thumb_up_grey);
                        }
                    }
                }

            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.Message);
            }
           
            

            return base.OnPrepareOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.action_upvote:
                    if (BlahguaAPIObject.Current.CurrentUser != null)
                    {
                        item.SetIcon(Resource.Drawable.ic_thumb_up_black);
                        HandlePromoteBlah();
                    }
                    break;

                case Resource.Id.action_report:
                   
                    break;
                case Resource.Id.action_report_infringe:
                    // send mail
                    HandleReportPost();
                    break;

                case Resource.Id.action_report_spam:
                    BlahguaAPIObject.Current.ReportPost(2);
                    RunOnUiThread(() =>
                    {
                        Toast.MakeText(this, "Spam reported.", ToastLength.Short).Show();
                    });
                    break;
                case Resource.Id.action_report_offensive:
                    BlahguaAPIObject.Current.ReportPost(1);
                    RunOnUiThread(() =>
                    {
                        Toast.MakeText(this, "Post reported.", ToastLength.Short).Show();
                    });
                    break;
                case Resource.Id.action_share:
                    HandleSharePost();
                    break;
                case Resource.Id.action_comment:
                    if (BlahguaAPIObject.Current.CurrentUser != null)
                    {
                        HandleAddComment();
                    }
                    break;

                case Resource.Id.action_signin:
                    if (BlahguaAPIObject.Current.CurrentUser == null)
                    {
                        StartActivity(typeof(LoginActivity));
                    }
                    break;

                case 16908332:// the back button apparently...
                    {
                        Finish();
                    }
                    break;
                
            }
            return base.OnOptionsItemSelected(item);
        }


        private void swipeLeftEvent()
        {
            if (curFragment == summaryFragment)
                ActionBar.SelectTab(commentTab);
            else if (curFragment == commentsFragment)
                ActionBar.SelectTab(statsTab);
            else
                ActionBar.SelectTab(summaryTab);
           
        }
        private void swipeRightEvent()
        {
            if (curFragment == statsFragment)
                ActionBar.SelectTab(commentTab);
            else if (curFragment == commentsFragment)
                ActionBar.SelectTab(summaryTab);
            else
                ActionBar.SelectTab(statsTab);
        }

		public override bool DispatchTouchEvent(MotionEvent ev)
		{
			bool didIt = _gestureDetector.OnTouchEvent (ev);
            if (!didIt)
                return base.DispatchTouchEvent(ev);
            else
                return true;
		}

      

        protected override void OnResume()
        {
            base.OnResume();

            InvalidateOptionsMenu();
        }

        private void HandleSharePost()
        {

        }

        private void HandleReportPost()
        {
            Intent emailIntent = new Intent(Intent.ActionSendto,
                        Android.Net.Uri.FromParts("mailto", App.EmailReportBug, null));
            emailIntent.PutExtra(Intent.ExtraSubject, GetString(Resource.String.viewpost_report_email_subject));
            emailIntent.PutExtra(Intent.ExtraText, GetString(Resource.String.viewpost_report_infringement_body));
            StartActivity(Intent.CreateChooser(emailIntent, GetString(Resource.String.signin_infringe_chooser_title)));
        }

        private void HandleAddComment()
        {
            if (ActionBar.SelectedTab != commentTab)
            {
                isFromCommentBtn = true;
                ActionBar.SelectTab(commentTab);
            }
            else
                commentsFragment.triggerCreateBlock();
        }

       
        #region TabBar buttons
        private void btn_stats_Click(object sender, EventArgs e)
        {
            bool firstTime = false;

            if (statsFragment == null)
            {
                statsFragment = ViewPostStatsFragment.NewInstance();

                firstTime = true;
            }

            SetCurrentFragment(statsFragment, firstTime);
                
            
        }

        private void btn_summary_Click(object sender, EventArgs e)
        {
            bool firstTime = false;

            if (summaryFragment == null)
            {
                summaryFragment = ViewPostSummaryFragment.NewInstance();
                firstTime = true;
            }

            SetCurrentFragment(summaryFragment, firstTime);
        }

        private void btn_comments_Click(object sender, EventArgs e)
        {

            bool firstTime = false;
            
            if (commentsFragment == null)
            {
                commentsFragment = ViewPostCommentsFragment.NewInstance();
                firstTime = true;
                if (isFromCommentBtn)
                {
                    commentsFragment.shouldCreateComment = true;
                }
            }

            SetCurrentFragment(commentsFragment, firstTime);
        }


        void SetCurrentFragment(Fragment newFrag, bool firstTime, string theTitle = null)
        {
            if (curFragment != newFrag)
            {
                var fragmentManager = this.FragmentManager;
                var ft = fragmentManager.BeginTransaction();

                if (curFragment != null)
                    ft.Hide(curFragment);

                curFragment = newFrag;

                if (newFrag != null)
                {
                    if (firstTime)
                        ft.Add(Resource.Id.content_fragment, curFragment);
                    else
                        ft.Show(curFragment);
                    ft.Commit();
                }
            }

            if (!String.IsNullOrEmpty(theTitle))
                Title = theTitle;
            if (isFromCommentBtn)
            {
                isFromCommentBtn = false;
                if (firstTime)
                    commentsFragment.shouldCreateComment = true;
                else
                    commentsFragment.triggerCreateBlock();
            }

        }

        #endregion

        #region Handles

        private void HandlePromoteBlah()
        {
            BlahguaAPIObject.Current.SetBlahVote(1, (newVote) =>
            {
                UpdateSummaryButtons();
				HomeActivity.analytics.PostBlahVote(1);
            });
            
        }

        private void HandleDemoteBlah()
        {
            BlahguaAPIObject.Current.SetBlahVote(-1, (newVote) =>
            {
                UpdateSummaryButtons();
				HomeActivity.analytics.PostBlahVote(-1);
            });
            
        }
        #endregion

        public void UpdateSummaryButtons()
        {
            InvalidateOptionsMenu();
        }
    }
}


