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

namespace BlahguaMobile.AndroidClient.Screens
{
    class HistoryPostsFragment : Fragment, Android.Views.View.IOnTouchListener
    {
        public static HistoryPostsFragment NewInstance()
        {
            return new HistoryPostsFragment { Arguments = new Bundle() };
        }

        private readonly string TAG = "HistoryPostsFragment";

        private GestureDetector _gestureDetector;
        private GestureListener _gestureListener;

        private ImageView image;
        private TextView titleView, posts_total_count;

        private ListView list;
        private LinearLayout no_entries;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _gestureListener = new GestureListener();
            _gestureListener.SwipeLeftEvent += GestureLeft;
            _gestureListener.SwipeRightEvent += GestureRight;
            _gestureDetector = new GestureDetector(Activity, _gestureListener);

            View fragment = inflater.Inflate(Resource.Layout.fragment_history_posts, null);
            //if (container == null)
            //{
            //    Log.Debug(TAG, "Dialer Fragment is in a view without container");
            //    return null;
            //}

            TextView text = fragment.FindViewById<TextView>(Resource.Id.text);
            //do some stuff like assigning event handlers, etc.


            posts_total_count = fragment.FindViewById<TextView>(Resource.Id.posts_total_count);
            list = fragment.FindViewById<ListView>(Resource.Id.list);
            list.ChoiceMode = ListView.ChoiceModeNone;
            list.Visibility = ViewStates.Gone;
            list.SetOnTouchListener(this);
            no_entries = fragment.FindViewById<LinearLayout>(Resource.Id.no_entries);
            no_entries.Visibility = ViewStates.Visible;

            LoadUserPosts();

            return fragment;
        }

        void GestureLeft(MotionEvent first, MotionEvent second)
        {
            int position = list.PointToPosition((int)first.GetX(), (int)first.GetY());
            if (adapter != null)
            {
                View listItem = HistoryUiHelper.getViewByPosition(position, list);
                HistoryUiHelper.manageSwipe(listItem, false, true);
                //listItem.FindViewById<TextView>(Resource.Id.text).SetText("You swipe left on the " + position, Android.Widget.TextView.BufferType.Normal);
            }
        }

        void GestureRight(MotionEvent first, MotionEvent second)
        {
            int position = list.PointToPosition((int)first.GetX(), (int)first.GetY());
            if (adapter != null)
            {
                View listItem = HistoryUiHelper.getViewByPosition(position, list);
                HistoryUiHelper.manageSwipe(listItem, true, false);
                //listItem.FindViewById<TextView>(Resource.Id.text).SetText("You swipe right on the " + position, Android.Widget.TextView.BufferType.Normal);
            }
        }

        public bool OnTouch(View v, MotionEvent ev)
        {
            return _gestureDetector.OnTouchEvent(ev);
        }

        /////////////////
        PostsAdapter adapter;
        private void LoadUserPosts()
        {
            BlahguaAPIObject.Current.LoadUserPosts((theBlahs) =>
                {
                    Activity.RunOnUiThread(() =>
                    {
                        posts_total_count.Text = "Your Posts (" + theBlahs.Count + ")";

                        if (theBlahs.Count > 0)
                        {
                            list.Visibility = ViewStates.Visible;
                            no_entries.Visibility = ViewStates.Gone;
                            adapter = new PostsAdapter(Activity, theBlahs);
                            list.Adapter = adapter;
                        }
                        else
                        {
                            list.Visibility = ViewStates.Gone;
                            no_entries.Visibility = ViewStates.Visible;
                            list.Adapter = adapter = null;
                        }
                    });
                    //list.ItemClick += list_ItemClick;

                    //blahDataView = new CollectionViewSource();
                    //blahDataView.Source = theBlahs.Where(blah => blah.S > 0);
                    //postsLoaded = true;
                    //SortAndFilterBlahs();
                    //UserPostList.ItemsSource = blahDataView.View;
                    //NoPostsBox.Visibility = Visibility.Collapsed;
                } );
        }
    }
}