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

using BlahguaMobile.AndroidClient.ThirdParty.UrlImageViewHelper;
using Android.Animation;
using BlahguaMobile.AndroidClient.Adapters;
using BlahguaMobile.AndroidClient.HelpingClasses;
using Android.Graphics;
using FortySevenDeg.SwipeListView;
using Android.Support.V4.App;

namespace BlahguaMobile.AndroidClient.Screens
{
    public class HistoryCommentsFragment : Android.Support.V4.App.Fragment
    {
        public static HistoryCommentsFragment NewInstance()
        {
            return new HistoryCommentsFragment { Arguments = new Bundle() };
        }

        private TextView comments_total_count;

		private SwipeListView list;
        private LinearLayout no_comments;
        private ProgressDialog progressDlg;

        private HistoryCommentsAdapter adapter;
		public static CommentList UserCommentList = null;
		public bool UserCommentsLoaded = false;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            progressDlg = new ProgressDialog(this.Activity);
            progressDlg.SetProgressStyle(ProgressDialogStyle.Spinner);

			HomeActivity.analytics.PostPageView("/self/comments");
            View fragment = inflater.Inflate(Resource.Layout.fragment_history_comments, null);

            comments_total_count = fragment.FindViewById<TextView>(Resource.Id.comments_total_count);
            comments_total_count.Text = "loading comments...";
            UiHelper.SetGothamTypeface(TypefaceStyle.Normal, comments_total_count);

			list = fragment.FindViewById<SwipeListView>(Resource.Id.list);
            list.ChoiceMode = ListView.ChoiceModeNone;
            no_comments = fragment.FindViewById<LinearLayout>(Resource.Id.no_comments);
			list.FrontViewClicked += HandleFrontViewClicked;
			list.BackViewClicked += HandleBackViewClicked;
			list.Dismissed += HandleDismissed;
           // LoadUserComments();

            return fragment;
        }

		void HandleFrontViewClicked (object sender, SwipeListViewClickedEventArgs e)
		{
			Activity.RunOnUiThread(() => list.OpenAnimate(e.Position));
		}

		void HandleBackViewClicked (object sender, SwipeListViewClickedEventArgs e)
		{
			Activity.RunOnUiThread(() => list.CloseAnimate(e.Position));
		}

		void HandleDismissed (object sender, SwipeListViewDismissedEventArgs e)
		{
			foreach (var i in e.ReverseSortedPositions)
			{
				adapter.RemoveView(i);
			}
		}

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);
        }

        public void LoadUserComments()
        {
			if (!UserCommentsLoaded) {
				Activity.RunOnUiThread (() => 
					{
						comments_total_count.Text = "loading comments";
						progressDlg.SetMessage ("loading comments...");
						progressDlg.Show ();

						BlahguaAPIObject.Current.LoadUserComments ((theComments) => 
							{
								UserCommentList = theComments;
								UserCommentsLoaded = true;
								Activity.RunOnUiThread (() => 
									{
										progressDlg.Hide ();
										DrawUserComments ();
									});
							});
					});
			}
        }

		public void DrawUserComments()
		{
			if (!UserCommentsLoaded)
				LoadUserComments ();
			else
				Activity.RunOnUiThread(() =>
					{
						if ((UserCommentList != null) && (UserCommentList.Count > 0))
						{
							comments_total_count.Text = "Your Comments (" + UserCommentList.Count + ")";
							no_comments.Visibility = ViewStates.Gone;
							list.Visibility = ViewStates.Visible;
							adapter = new HistoryCommentsAdapter(this, UserCommentList);
							list.Adapter = adapter;
						}
						else
						{
							comments_total_count.Text = "Your Comments (0)";
							no_comments.Visibility = ViewStates.Visible;
							list.Visibility = ViewStates.Gone;
							list.Adapter = adapter = null;
						}
					});
		}
    }
}