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
using BlahguaMobile.AndroidClient.HelpingClasses;
using BlahguaMobile.AndroidClient.Adapters;
using Android.Animation;
using Android.Graphics;
using FortySevenDeg.SwipeListView;
using Android.Support.V4.App;

namespace BlahguaMobile.AndroidClient.Screens
{
    public class HistoryPostsFragment : Android.Support.V4.App.Fragment
    {
        private TextView posts_total_count;
		private SwipeListView list;
        private LinearLayout no_entries;
        private PostsAdapter adapter;
        private ProgressDialog progressDlg;
		public static BlahList UserBlahList = null;
		public bool UserBlahsLoaded = false;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
			HomeActivity.analytics.PostPageView("/self/history");
            View fragment = inflater.Inflate(Resource.Layout.fragment_history_posts, null);
            progressDlg = new ProgressDialog(this.Activity);
            progressDlg.SetProgressStyle(ProgressDialogStyle.Spinner);
            posts_total_count = fragment.FindViewById<TextView>(Resource.Id.posts_total_count);
            UiHelper.SetGothamTypeface(TypefaceStyle.Normal, posts_total_count);

			list = fragment.FindViewById<SwipeListView>(Resource.Id.list);
            list.ChoiceMode = ListView.ChoiceModeNone;
            list.Visibility = ViewStates.Gone;
    
            //list.SetOnTouchListener(Activity);
            no_entries = fragment.FindViewById<LinearLayout>(Resource.Id.no_entries);
            no_entries.Visibility = ViewStates.Visible;
            posts_total_count.Text = "loading post history";
			list.FrontViewClicked += HandleFrontViewClicked;
			list.BackViewClicked += HandleBackViewClicked;
			list.Dismissed += HandleDismissed;
          

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
            //DrawUserPosts();
        }

        public void LoadUserPosts()
        {
			if (!UserBlahsLoaded) {
				Activity.RunOnUiThread (() => {
                    
					progressDlg.SetMessage ("loading posts...");
					progressDlg.Show ();


					BlahguaAPIObject.Current.LoadUserPosts ((theBlahs) => {
						UserBlahList = theBlahs;
						UserBlahsLoaded = true;
						Activity.RunOnUiThread (() => {
							progressDlg.Hide ();
							DrawUserPosts();
                                
						});
					}
					);
				});
			}
        }

		public void DrawUserPosts()
		{
			if (!UserBlahsLoaded)
				LoadUserPosts ();
			else
				Activity.RunOnUiThread(() =>
					{
						string countMessage = "";
						if ((UserBlahList != null) && (UserBlahList.Count > 0))
						{
							list.Visibility = ViewStates.Visible;
							no_entries.Visibility = ViewStates.Gone;
							foreach (Blah b in UserBlahList.ToArray())
							{
								if (b.S < 0)
									UserBlahList.Remove(b);
							}
							adapter = new PostsAdapter(this, UserBlahList);
							list.Adapter = adapter;

							countMessage = "Your Posts (" + UserBlahList.Count + ")";
						}
						else
						{
							list.Visibility = ViewStates.Gone;
							no_entries.Visibility = ViewStates.Visible;
							list.Adapter = adapter = null;

							countMessage = "No Posts yet";
						}
						posts_total_count.Text = countMessage;
					});
		}
    }
}