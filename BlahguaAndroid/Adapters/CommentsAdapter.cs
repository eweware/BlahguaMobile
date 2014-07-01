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

namespace BlahguaMobile.AndroidClient.Adapters
{
    class CommentsAdapter : BaseAdapter
    {
        Activity _activity;
        CommentList _list;

        public CommentsAdapter(Activity activity, CommentList list)
        {
            _activity = activity;
            _list = list;
        }

        public void setComments(CommentList list)
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
                                 Resource.Layout.listitem_comment, parent, false);
            var text = view.FindViewById<TextView>(Resource.Id.text);
            var image = view.FindViewById<ImageView>(Resource.Id.image);
            var author = view.FindViewById<TextView>(Resource.Id.author);
            var author_avatar = view.FindViewById<ImageView>(Resource.Id.author_avatar);
            var time_ago = view.FindViewById<TextView>(Resource.Id.time_ago);
            var upvoted = view.FindViewById<TextView>(Resource.Id.upvoted);
            var downvoted = view.FindViewById<TextView>(Resource.Id.downvoted);

            Comment c = _list[position];
            if (!String.IsNullOrEmpty(c.ImageURL))
            {
                image.SetUrlDrawable(c.ImageURL);
            }

            text.SetText(c.T, Android.Widget.TextView.BufferType.Normal);
            author.Text = c.AuthorName;
            author_avatar.SetUrlDrawable(c.AuthorImage);
            time_ago.Text = StringHelper.ConstructTimeAgo(c.CreationDate);

            upvoted.Text = c.UpVoteCount.ToString();
            downvoted.Text = c.DownVoteCount.ToString();

            return view;
        }
    }
}