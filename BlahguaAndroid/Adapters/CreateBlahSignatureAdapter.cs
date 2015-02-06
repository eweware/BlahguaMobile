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
using BlahguaMobile.AndroidClient.HelpingClasses;
using Android.Graphics;

namespace BlahguaMobile.AndroidClient.Adapters
{
    class CreateBlahSignatureAdapter : BaseAdapter
    {
	    private class ViewHolder : Java.Lang.Object {
		    public TextView tv;
		    public CheckBox chkbox;
	    }

        private BadgeList mListItems;
	    private LayoutInflater mInflater;
	    private ViewHolder holder;

	    public CreateBlahSignatureAdapter(Context context, BadgeList items) {
            if (items == null)
                mListItems = new BadgeList();
            else
                mListItems = items;
		    mInflater = LayoutInflater.From(context);
	    }

		public override int Count {
			get
			{
				if (mListItems.Count > 0)
					return mListItems.Count + 3;
				else
					return 3;
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
				convertView = mInflater.Inflate(Resource.Layout.listitem_popup_signature, null);
				holder = new ViewHolder();
				holder.tv = convertView.FindViewById<TextView>(Resource.Id.text);
				UiHelper.SetGothamTypeface(TypefaceStyle.Normal, holder.tv);
				holder.chkbox = convertView.FindViewById<CheckBox>(Resource.Id.check);
				holder.chkbox.Tag = position;
				convertView.Tag = holder;
			} else {
				holder = (ViewHolder) convertView.Tag;
				holder.tv.SetTextColor (Color.Black);
				holder.chkbox.Visibility = ViewStates.Visible;
				UiHelper.SetGothamTypeface(TypefaceStyle.Normal, holder.tv);
			}

			if (position == 0)
			{
				// userprofile option
				holder.tv.Text = "use profile";
				holder.chkbox.Checked = !BlahguaAPIObject.Current.CreateRecord.XX;
			}
			else if (position == 1)
			{
				// userprofile option
				holder.tv.Text = "mature content";
				holder.chkbox.Checked = BlahguaAPIObject.Current.CreateRecord.XXX;
			}
			else if (position == 2)
			{
				string badgesString;
				// userprofile option
				if (mListItems.Count == 0)
					badgesString = "get badges to use them on a post";
				else if (mListItems.Count == 1)
					badgesString = "available badge";
				else
					badgesString = "available badges";

				holder.tv.Text = badgesString;
				UiHelper.SetGothamTypeface(TypefaceStyle.Italic, holder.tv);
				holder.tv.SetTextColor (Color.DarkGray);
				holder.chkbox.Visibility = ViewStates.Gone;
			}
			else
			{
				BadgeReference badge = (BadgeReference)mListItems[position - 3];
				holder.tv.Text = badge.BadgeName;
				if ((BlahguaAPIObject.Current.CreateRecord.B != null) &&
					BlahguaAPIObject.Current.CreateRecord.B.Contains(badge.ID))
					holder.chkbox.Checked = true;
				else
					holder.chkbox.Checked = false;
			}

			holder.chkbox.Click += (sender, args) =>
			{
				CheckBox cb = (CheckBox)sender;
				int pos = (int)cb.Tag;
				if (pos == 0)
				{
					BlahguaAPIObject.Current.CreateRecord.XX = !cb.Checked;
				}
				else if (pos == 1)
				{
					BlahguaAPIObject.Current.CreateRecord.XXX = cb.Checked;
				}
				else 
				{
					pos -= 3;
					if (mListItems != null)
					{
						if (BlahguaAPIObject.Current.CreateRecord.B == null)
						{
							BlahguaAPIObject.Current.CreateRecord.B = new List<string>();
						}
						string badgeId = mListItems[pos].ID;
						if (cb.Checked && !BlahguaAPIObject.Current.CreateRecord.B.Contains(badgeId))
						{
							BlahguaAPIObject.Current.CreateRecord.B.Add(badgeId);
						}
						else if (!cb.Checked && BlahguaAPIObject.Current.CreateRecord.B.Contains(badgeId))
						{
							BlahguaAPIObject.Current.CreateRecord.B.Remove(badgeId);
						}
					}
				}

			};

			return convertView;
	    }
    }
}