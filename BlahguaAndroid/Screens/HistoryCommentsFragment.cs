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

namespace BlahguaMobile.AndroidClient.Screens
{
    class HistoryCommentsFragment : Fragment
    {
        public static HistoryCommentsFragment NewInstance()
        {
            return new HistoryCommentsFragment { Arguments = new Bundle() };
        }

        private readonly string TAG = "HistoryCommentsFragment";

        private GestureDetector _gestureDetector;
        private GestureListener _gestureListener;

        private TextView comments_total_count;

        private ListView list;
        private LinearLayout no_comments;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _gestureListener = new GestureListener();
            _gestureListener.SwipeLeftEvent += GestureLeft;
            _gestureListener.SwipeRightEvent += GestureRight;
            _gestureDetector = new GestureDetector(Activity, _gestureListener);

            View fragment = inflater.Inflate(Resource.Layout.fragment_history_comments, null);
            //if (container == null)
            //{
            //    Log.Debug(TAG, "Dialer Fragment is in a view without container");
            //    return null;
            //}

            comments_total_count = fragment.FindViewById<TextView>(Resource.Id.comments_total_count);
            list = fragment.FindViewById<ListView>(Resource.Id.list);
            list.ChoiceMode = ListView.ChoiceModeNone;
            no_comments = fragment.FindViewById<LinearLayout>(Resource.Id.no_comments);

            LoadUserComments();

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

        CommentsAdapter adapter;
        private void LoadUserComments()
        {
            BlahguaAPIObject.Current.LoadUserComments((theComments) =>
            {
                Activity.RunOnUiThread(() =>
                {
                    if (theComments.Count > 0)
                    {
                        comments_total_count.Text = "Your Comments (" + theComments.Count + ")";
                        no_comments.Visibility = ViewStates.Gone;
                        list.Visibility = ViewStates.Visible;
                        adapter = new CommentsAdapter(Activity, theComments);
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
                //commentDataView = new CollectionViewSource();
                //commentDataView.Source = theComments;
                //SortAndFilterComments();
                //commentsLoaded = true;
                //UserCommentList.ItemsSource = commentDataView.View;
                //NoCommentsBox.Visibility = Visibility.Collapsed;

            }
            );

        }
        //private void LoadComments()
        //{
        //    BlahguaAPIObject.Current.LoadBlahComments((theList) =>
        //    {
        //        Activity.RunOnUiThread(() =>
        //        {
        //            if (theList.Count > 0)
        //            {
        //                comments_total_count.Text = "Your Comments (" + theList.Count + ")";
        //                no_comments.Visibility = ViewStates.Gone;
        //                list.Visibility = ViewStates.Visible;
        //                adapter = new CommentsAdapter(Activity, theList);
        //                list.Adapter = adapter;
        //            }
        //            else
        //            {
        //                comments_total_count.Text = "There is no comments";
        //                no_comments.Visibility = ViewStates.Visible;
        //                list.Visibility = ViewStates.Gone;
        //                list.Adapter = adapter = null;
        //            }
        //        });
        //    });
        //}
    }
}