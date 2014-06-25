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

namespace BlahguaMobile.AndroidClient.Adapters
{
    class PostsAdapter : BaseAdapter
    {

        EventHandler openHandler = (sender, args) =>
        {
            var btn = (Button)sender;
            string id = (string)btn.Tag;
            App.BlahIdToOpen = id;
            btn.Context.StartActivity(typeof(ViewPostActivity));
        };
        EventHandler deleteHandler = (sender, args) =>
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

        static HistoryPostsFragment _fragment;
        Activity _activity;
        BlahList _list;

        public PostsAdapter(HistoryPostsFragment fragment, BlahList list)
        {
            _activity = fragment.Activity;
            _fragment = fragment;
            _list = list;
        }

        public void setEntries(BlahList list)
        {
            _list = list;
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
            var view = convertView ?? _activity.LayoutInflater.Inflate(
                Resource.Layout.listitem_history_blah, parent, false);
            var text = view.FindViewById<TextView>(Resource.Id.text);
            var image = view.FindViewById<ImageView>(Resource.Id.image);
            var author = view.FindViewById<TextView>(Resource.Id.author);
            var author_avatar = view.FindViewById<ImageView>(Resource.Id.author_avatar);
            var time_ago = view.FindViewById<TextView>(Resource.Id.time_ago);
            var upvoted = view.FindViewById<TextView>(Resource.Id.upvoted);
            var downvoted = view.FindViewById<TextView>(Resource.Id.downvoted);

            Blah b = _list[position];
            if (String.IsNullOrEmpty(b.ImageURL))
            {
                image.SetUrlDrawable(b.ImageURL);
            }

            text.Text = b.T;
            author.Text = b.UserName;
            author_avatar.SetUrlDrawable(b.UserImage);
            if (!(b.u == null && b.c == null))
            {
                time_ago.Text = StringHelper.ConstructTimeAgo(b.u == null ? b.CreationDate : b.UpdateDate);
            }

            upvoted.Text = b.uv.ToString();
            downvoted.Text = b.uv.ToString();

            var btnOpenPost = view.FindViewById<Button>(Resource.Id.btn_open);
            btnOpenPost.Tag = b._id;
            btnOpenPost.Click -= openHandler;
            btnOpenPost.Click += openHandler;
            var btnDelete = view.FindViewById<Button>(Resource.Id.btn_delete);
            btnDelete.Tag = b._id;
            btnDelete.Click -= deleteHandler;
            btnDelete.Click += deleteHandler;

            return view;
        }
    }
}