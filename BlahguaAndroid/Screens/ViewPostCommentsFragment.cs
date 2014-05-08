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
using BlahguaMobile.AndroidClient.Adapters;

namespace BlahguaMobile.AndroidClient.Screens
{
    class ViewPostCommentsFragment : Fragment
    {
        public static ViewPostCommentsFragment NewInstance()
        {
            return new ViewPostCommentsFragment { Arguments = new Bundle() };
        }

        private readonly string TAG = "ViewPostCommentsFragment";

        ListView list;
        LinearLayout no_comments;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View fragment = inflater.Inflate(Resource.Layout.fragment_viewpost_comments, null);
            //if (container == null)
            //{
            //    Log.Debug(TAG, "Dialer Fragment is in a view without container");
            //    return null;
            //}

            list = fragment.FindViewById<ListView>(Resource.Id.list);
            no_comments = fragment.FindViewById<LinearLayout>(Resource.Id.no_comments);

            LoadComments();

            return fragment;
        }

        private void LoadComments()
        {
            BlahguaAPIObject.Current.LoadBlahComments((theList) =>
            {
                if (theList.Count > 0)
                {
                    no_comments.Visibility = ViewStates.Gone;
                    list.Visibility = ViewStates.Visible;
                    list.Adapter = new CommentsAdapter(Activity, theList);
                }
                else
                {
                    no_comments.Visibility = ViewStates.Visible;
                    list.Visibility = ViewStates.Gone;
                    list.Adapter = null;
                }
            });
        }
    }
}