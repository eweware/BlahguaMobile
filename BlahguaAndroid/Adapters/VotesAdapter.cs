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
using Android.Util;
using BlahguaMobile.AndroidClient.HelpingClasses;
using Android.Graphics;

namespace BlahguaMobile.AndroidClient.Adapters
{
    class VotesAdapter : BaseAdapter
    {
        Activity _activity;
        PollItemList _list;
        EventHandler<CompoundButton.CheckedChangeEventArgs> _onCheckHandler;

        int screenWidth;
        bool _isVotable = false;

        public VotesAdapter(Activity activity, PollItemList list, bool isVotable, EventHandler<CompoundButton.CheckedChangeEventArgs> onCheckHandler)
        {
            _activity = activity;
            _list = list;
            _isVotable = isVotable;
            _onCheckHandler = onCheckHandler;

            screenWidth = activity.Resources.DisplayMetrics.WidthPixels;
        }

        public PollItemList List { get { return _list; } }

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
            bool isNewView = convertView == null;
            var view = convertView ?? _activity.LayoutInflater.Inflate(
                                 Resource.Layout.listitem_poll, parent, false);
            var check = view.FindViewById<CheckBox>(Resource.Id.check);
            var percent_bar = view.FindViewById<View>(Resource.Id.percent_bar);
            var percent_string = view.FindViewById<TextView>(Resource.Id.percent_string);
            var vote_text = view.FindViewById<TextView>(Resource.Id.vote_text);
            UiHelper.SetGothamTypeface(TypefaceStyle.Normal, percent_string, vote_text);

            PollItem p = _list[position];
            if (BlahguaAPIObject.Current.CurrentUser == null)
            {
                check.Visibility = ViewStates.Gone;
            }
            else
            {
                if (p.IsUserVote)
                {
                    check.Checked = true;
                }
                if (!_isVotable)
                {
                    check.Enabled = false;
                }
            }

            if (isNewView && _onCheckHandler != null)
                check.CheckedChange += _onCheckHandler;

            check.Tag = position;

            int fullWidth = screenWidth - 60;
            int percentWidth = (int)((p.Votes * 1.0f / p.TotalVotes) * fullWidth);
            percent_string.Text = p.VotePercent;
            int height = (int) TypedValue.ApplyDimension(ComplexUnitType.Dip, 20, _activity.Resources.DisplayMetrics);
            percent_bar.LayoutParameters = new FrameLayout.LayoutParams(percentWidth, height);

            vote_text.Text = p.G;

            return view;
        }
    }
}