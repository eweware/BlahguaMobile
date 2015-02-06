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

namespace BlahguaMobile.AndroidClient.Screens
{
    class HistoryPostsFragment : Fragment
    {
        public static HistoryPostsFragment NewInstance()
        {
            return new HistoryPostsFragment { Arguments = new Bundle() };
        }

        private TextView posts_total_count;

		private SwipeListView list;
        private LinearLayout no_entries;

        private PostsAdapter adapter;
        private ProgressDialog progressDlg;

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
            LoadUserPosts();
        }

        public void LoadUserPosts()
        {
            Activity.RunOnUiThread(() =>
                {
                    
                    progressDlg.SetMessage("loading posts...");
                    progressDlg.Show();


                    BlahguaAPIObject.Current.LoadUserPosts((theBlahs) =>
                        {
                            Activity.RunOnUiThread(() =>
                            {
                                progressDlg.Hide();
                                string countMessage = "";
                                if ((theBlahs != null) && (theBlahs.Count > 0))
                                {
                                    list.Visibility = ViewStates.Visible;
                                    no_entries.Visibility = ViewStates.Gone;
                                    foreach (Blah b in theBlahs.ToArray())
                                    {
                                        if (!b.XX || b.S <= 0)
                                            theBlahs.Remove(b);
                                    }
                                    adapter = new PostsAdapter(this, theBlahs);
                                    list.Adapter = adapter;

                                    countMessage = "Your Posts (" + theBlahs.Count + ")";
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
                    );
                });
        }
    }
}