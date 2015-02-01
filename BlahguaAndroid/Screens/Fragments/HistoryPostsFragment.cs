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

namespace BlahguaMobile.AndroidClient.Screens
{
    class HistoryPostsFragment : Fragment
    {
        public static HistoryPostsFragment NewInstance()
        {
            return new HistoryPostsFragment { Arguments = new Bundle() };
        }

        private TextView posts_total_count;

        private ListView list;
        private LinearLayout no_entries;

        private PostsAdapter adapter;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
			HomeActivity.analytics.PostPageView("/self/history");
            View fragment = inflater.Inflate(Resource.Layout.fragment_history_posts, null);

            posts_total_count = fragment.FindViewById<TextView>(Resource.Id.posts_total_count);
            UiHelper.SetGothamTypeface(TypefaceStyle.Normal, posts_total_count);

            list = fragment.FindViewById<ListView>(Resource.Id.list);
            list.ChoiceMode = ListView.ChoiceModeNone;
            list.Visibility = ViewStates.Gone;
            list.ItemClick += (sender, args) => {
                if (adapter != null)
                {
                    View listItem = UiHelper.GetViewByPosition(args.Position, list);
                    UiHelper.ManageSwipe(listItem, true, false);
                }
            };
            //list.SetOnTouchListener(Activity);
            no_entries = fragment.FindViewById<LinearLayout>(Resource.Id.no_entries);
            no_entries.Visibility = ViewStates.Visible;

            LoadUserPosts();

            return fragment;
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);
        }

        public void LoadUserPosts()
        {
            BlahguaAPIObject.Current.LoadUserPosts((theBlahs) =>
                {
                    Activity.RunOnUiThread(() =>
                    {
                        string countMessage = "";
                        if (theBlahs.Count > 0)
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
        }
    }
}