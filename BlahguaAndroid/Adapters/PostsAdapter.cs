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
using BlahguaMobile.AndroidClient.Screens;
using System.Reflection;
using System.ComponentModel;
using Android.Graphics;

namespace BlahguaMobile.AndroidClient.Adapters
{
    class PostsAdapter : BaseAdapter
    {
        private EventHandler openHandler = (sender, args) =>
        {
            var btn = (Button)sender;
            string id = (string)btn.Tag;
            App.BlahIdToOpen = id;
            btn.Context.StartActivity(typeof(ViewPostActivity));
        };

        private EventHandler deleteHandler = (sender, args) =>
        {
            var btn = (Button)sender;
            string id = (string)btn.Tag;
            BlahguaAPIObject.Current.DeleteBlah(id, (theString) =>
            {
                if (theString == "ok")
                {
                    _fragment.LoadUserPosts();
                }
            });
        };

        private static HistoryPostsFragment _fragment;
        private Activity _activity;
        private List<Blah> _list;

        public PostsAdapter(HistoryPostsFragment fragment, BlahList list)
        {
            _activity = fragment.Activity;
            _fragment = fragment;
            _list = list.OrderByDescending(b => b.CreationDate).ToList();
        }

        public void setEntries(BlahList list)
        {
            _list = list.OrderByDescending(b => b.CreationDate).ToList();
        }


        public override int Count
        {
            get { return _list.Count; }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            // could wrap a Contact in a Java.Lang.Object
            // to return it here if needed
            return null;
        }

        public override long GetItemId(int position)
        {
            return position;// _list[position]._id;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view;

            if (convertView != null)
            {
                view = convertView;
                view.FindViewById<TextView>(Resource.Id.left_layout).Visibility = ViewStates.Gone;
                view.FindViewById<TextView>(Resource.Id.right_layout).Visibility = ViewStates.Gone;
            }
            else
            {
                view = _activity.LayoutInflater.Inflate(Resource.Layout.listitem_history_blah, parent, false);

            }
            var text = view.FindViewById<TextView>(Resource.Id.text);
            var image = view.FindViewById<ImageView>(Resource.Id.image);
            var type_mark = view.FindViewById<ImageView>(Resource.Id.blahtype);
            var time_ago = view.FindViewById<TextView>(Resource.Id.time_ago);
            var upvoted = view.FindViewById<TextView>(Resource.Id.upvoted);
            var downvoted = view.FindViewById<TextView>(Resource.Id.downvoted);
            var convert = view.FindViewById<TextView>(Resource.Id.conversion);
            var comments = view.FindViewById<TextView>(Resource.Id.comments);

            // set fonts
            UiHelper.SetGothamTypeface(TypefaceStyle.Normal, time_ago, upvoted, downvoted, convert, comments);
            UiHelper.SetGothamTypeface(TypefaceStyle.Bold, text);

            Blah b = _list[position];
            if (String.IsNullOrEmpty(b.ImageURL))
            {
                image.SetUrlDrawable(b.ImageURL);
                image.Visibility = ViewStates.Visible;
            }
            else
                image.Visibility = ViewStates.Gone;

            text.Text = b.T;

            if (!(b.u == null && b.cdate == null))
            {
                time_ago.Text = StringHelper.ConstructTimeAgo(b.CreationDate);
            }

            switch (b.TypeName)
            {
                case "says":
                    type_mark.SetBackgroundResource(Resource.Drawable.say_icon);
                    break;
                case "asks":
                    type_mark.SetBackgroundResource(Resource.Drawable.ask_icon);
                    break;
                case "leaks":
                    type_mark.SetBackgroundResource(Resource.Drawable.leak_icon);
                    break;
                case "polls":
                    type_mark.SetBackgroundResource(Resource.Drawable.poll_icon);
                    break;
                case "predicts":
                    type_mark.SetBackgroundResource(Resource.Drawable.predict_icon);
                    break;
            }

            upvoted.Text = b.uv.ToString();
            downvoted.Text = b.uv.ToString();
            convert.Text = b.ConversionString;
            comments.Text = b.C.ToString();

            var btnOpenPost = view.FindViewById<Button>(Resource.Id.btn_open);
            btnOpenPost.Tag = b._id;

            var btnDelete = view.FindViewById<Button>(Resource.Id.btn_delete);
            btnDelete.Tag = b._id;

            if (convertView == null)
            {
                // add events to new item
                btnDelete.Click += deleteHandler;
                btnOpenPost.Click += openHandler;
            }

            return view;
        }
    }
}