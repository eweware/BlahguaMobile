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
using Java.Util;
using BlahguaMobile.BlahguaCore;
using BlahguaMobile.AndroidClient.ThirdParty.UrlImageViewHelper;
using Android.Graphics;
using BlahguaMobile.AndroidClient.HelpingClasses;

namespace BlahguaMobile.AndroidClient.Adapters
{
	class ViewCommentBadgesAdapter : BaseAdapter
	{
        List<BadgeRecord> _badges;

		LayoutInflater inflater;
		public ViewCommentBadgesAdapter(Activity activity, Comment comment)
		{
			inflater = activity.LayoutInflater;
			_badges = comment.BD;
		}

		public override int Count {
			get
			{
				return _badges.Count;
			}
		}

		public override Java.Lang.Object GetItem(int arg0) {
			return null;
		}

		public override long GetItemId(int arg0) {
			return 0;
		}

		public override View GetView(int position, View convertView, ViewGroup parent) {

			if (convertView == null) {
				convertView = inflater.Inflate(Resource.Layout.listitem_viewpost_summary_badge, parent, false);
			}

			BadgeRecord b = _badges[position];

			//var badgeImage = convertView.FindViewById<ImageView>(Resource.Id.image);
			var badgeName = convertView.FindViewById<TextView>(Resource.Id.text);
			var verifiedText = convertView.FindViewById<TextView>(Resource.Id.verified_text);
			// TO DO:  For some reason this does not load the image correctly
			//badgeImage.SetUrlDrawable(b.BadgeImage);
			badgeName.Text = b.N;
			UiHelper.SetGothamTypeface(TypefaceStyle.Bold, badgeName);
			UiHelper.SetGothamTypeface(TypefaceStyle.Normal, verifiedText);
			return convertView;
		}
	}
}