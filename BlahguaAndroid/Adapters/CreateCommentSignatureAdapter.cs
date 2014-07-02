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

namespace BlahguaMobile.AndroidClient.Adapters
{
    class CreateCommentSignatureAdapter : BaseAdapter
    {
	    private class ViewHolder : Java.Lang.Object {
		    public TextView tv;
		    public CheckBox chkbox;
	    }

        private BadgeList mListItems;
	    private LayoutInflater mInflater;
	    private ViewHolder holder;

        public CreateCommentSignatureAdapter(Context context, BadgeList items)
        {
            if (items == null)
                mListItems = new BadgeList();
            else
                mListItems = items;
		    mInflater = LayoutInflater.From(context);
	    }

	    public override int Count {
            get
            {
                return mListItems.Count + 1;
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
                holder.chkbox = convertView.FindViewById<CheckBox>(Resource.Id.check);
                holder.chkbox.Tag = position;
			    convertView.Tag = holder;
		    } else {
			    holder = (ViewHolder) convertView.Tag;
		    }

            if (position == 0)
            {
                // userprofile option
                holder.tv.Text = "User Profile";
                holder.chkbox.Checked = !BlahguaAPIObject.Current.CreateCommentRecord.XX;
            }
            else
            {
                BadgeReference badge = (BadgeReference)mListItems[position - 1];
                holder.tv.Text = badge.BadgeName;
                holder.chkbox.Checked = BlahguaAPIObject.Current.CreateCommentRecord.B.Contains(badge.ID);
            }

            holder.chkbox.Click += (sender, args) =>
            {
                CheckBox cb = (CheckBox)sender;
                int pos = (int)cb.Tag;
                if (pos == 0)
                {
                    BlahguaAPIObject.Current.CreateCommentRecord.XX = !cb.Checked;
                }
                else
                {
                    if (mListItems != null)
                    {
                        if (BlahguaAPIObject.Current.CreateCommentRecord.BD == null)
                        {
                            BlahguaAPIObject.Current.CreateCommentRecord.BD = new List<string>();
                        }
                        string badgeId = mListItems[pos].ID;
                        if (cb.Checked && !BlahguaAPIObject.Current.CreateCommentRecord.BD.Contains(badgeId))
                        {
                            BlahguaAPIObject.Current.CreateCommentRecord.BD.Add(badgeId);
                        }
                        else if (!cb.Checked && BlahguaAPIObject.Current.CreateCommentRecord.BD.Contains(badgeId))
                        {
                            BlahguaAPIObject.Current.CreateCommentRecord.BD.Remove(badgeId);
                        }
                    }
                }

            };

		    return convertView;
	    }
    }
}