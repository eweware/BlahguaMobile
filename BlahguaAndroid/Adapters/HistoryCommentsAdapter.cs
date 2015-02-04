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
using BlahguaMobile.AndroidClient.Screens;
using BlahguaMobile.AndroidClient.HelpingClasses;
using Android.Graphics;

namespace BlahguaMobile.AndroidClient.Adapters
{
    class HistoryCommentsAdapter : BaseAdapter
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

        };

        private static HistoryCommentsFragment _fragment;
        Activity _activity;
        List<Comment> _list;

        public HistoryCommentsAdapter(HistoryCommentsFragment fragment, CommentList list)
        {
            _activity = fragment.Activity;
            _fragment = fragment;
            _list = list.OrderByDescending(b => b.CreationDate).ToList();
        }

        public void setComments(CommentList list)
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

			if (convertView == null)
				view = _activity.LayoutInflater.Inflate (Resource.Layout.listitem_history_comment, parent, false);
			else
				view = convertView;

            var text = view.FindViewById<TextView>(Resource.Id.text);
            var image = view.FindViewById<ImageView>(Resource.Id.image);
            var time_ago = view.FindViewById<TextView>(Resource.Id.time_ago);
            var upvoted = view.FindViewById<TextView>(Resource.Id.upvoted);
            var downvoted = view.FindViewById<TextView>(Resource.Id.downvoted);
            

            // set fonts
            UiHelper.SetGothamTypeface(TypefaceStyle.Normal, text, time_ago, upvoted, downvoted);

            Comment c = _list[position];
            if (String.IsNullOrEmpty(c.ImageURL))
            {
                image.SetUrlDrawable(c.ImageURL);
            }

            text.SetText(c.T, Android.Widget.TextView.BufferType.Normal);

            if (!(c.u == null && c.c == null))
            {
                time_ago.Text = StringHelper.ConstructTimeAgo(c.CreationDate);
            }

            upvoted.Text = c.UpVoteCount.ToString();
            downvoted.Text = c.DownVoteCount.ToString();

            var btnOpenPost = view.FindViewById<Button>(Resource.Id.btn_open);
            btnOpenPost.Tag = c.B;
            btnOpenPost.Click -= openHandler;
            btnOpenPost.Click += openHandler;

            return view;
        }
    }
}