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
using FortySevenDeg.SwipeListView;

namespace BlahguaMobile.AndroidClient.Adapters
{
    class PostsAdapter : BaseAdapter
    {
 
        private static HistoryPostsFragment _fragment;
        private Activity _activity;
        private List<Blah> _list;

        public PostsAdapter(HistoryPostsFragment fragment, BlahList list)
        {
            _activity = fragment.Activity;
            _fragment = fragment;
            _list = list.OrderByDescending(b => b.cdate).ToList();
        }

        public void setEntries(BlahList list)
        {
            _list = list.OrderByDescending(b => b.cdate).ToList();
        }

		public void RemoveView(int position)
		{
			long id = _list[position]._id;

			BlahguaAPIObject.Current.DeleteBlah(id, (theString) =>
				{
					this._activity.RunOnUiThread(() => {
						Toast.MakeText(_activity, "post deleted", ToastLength.Short).Show();
					});
				});

			_list.RemoveAt(position);
			NotifyDataSetChanged();
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
            }
            else
            {
                view = _activity.LayoutInflater.Inflate(Resource.Layout.listitem_history_blah, null);

            }

			((SwipeListView)parent).Recycle(view, position);

            var text = view.FindViewById<TextView>(Resource.Id.text);
            var image = view.FindViewById<ImageView>(Resource.Id.image);
            var type_mark = view.FindViewById<ImageView>(Resource.Id.blahtype);
            var time_ago = view.FindViewById<TextView>(Resource.Id.time_ago);
            var upvoted = view.FindViewById<TextView>(Resource.Id.upvoted);
            var downvoted = view.FindViewById<TextView>(Resource.Id.downvoted);
            var convert = view.FindViewById<TextView>(Resource.Id.conversion);
            var comments = view.FindViewById<TextView>(Resource.Id.comments);

            // set fonts
            UiHelper.SetGothamTypeface(TypefaceStyle.Normal, upvoted, downvoted, convert, comments);
            UiHelper.SetGothamTypeface(TypefaceStyle.Bold, text);
			UiHelper.SetGothamTypeface(TypefaceStyle.Italic, time_ago);

            Blah b = _list[position];

            if (!String.IsNullOrEmpty(b.ImageURL))
            {
                image.SetUrlDrawable(b.ImageURL);
                image.Visibility = ViewStates.Visible;
            }
            else
                image.Visibility = ViewStates.Gone;

            text.Text = b.T;

            if (b.cdate != null)
            {
                time_ago.Text = StringHelper.ConstructTimeAgo(b.cdate);
            }

            switch (b.TypeName)
            {
                case "says":
                    type_mark.SetImageResource(Resource.Drawable.icon_speechact_say);
                    break;
                case "asks":
					type_mark.SetImageResource(Resource.Drawable.icon_speechact_ask);
                    break;
                case "leaks":
					type_mark.SetImageResource(Resource.Drawable.icon_speechact_leak);
                    break;
                case "polls":
					type_mark.SetImageResource(Resource.Drawable.icon_speechact_poll);
                    break;
                case "predicts":
					type_mark.SetImageResource(Resource.Drawable.icon_speechact_predict);
                    break;
            }

            upvoted.Text = b.uv.ToString();
            downvoted.Text = b.uv.ToString();
            convert.Text = b.ConversionString;
            comments.Text = b.C.ToString();

            var btnOpenPost = view.FindViewById<Button>(Resource.Id.btn_open);
            btnOpenPost.Tag = b._id;
			var btnDeletePost = view.FindViewById<Button>(Resource.Id.btn_delete);
			btnDeletePost.Tag = position;
             if (convertView == null)
            {
                // add events to new item
				btnOpenPost.Click += (s, e) => {
					var btn = (Button)s;
					long id = (long)btn.Tag;
					App.BlahIdToOpen = id;
					_activity.StartActivity(typeof(ViewPostActivity));
				};
				btnDeletePost.Click += (s, e) =>
				{
					var btn = (Button)s;
					int pos = (int)btn.Tag;
					RemoveView(pos);
				};
            }

            return view;
        }
    }
}