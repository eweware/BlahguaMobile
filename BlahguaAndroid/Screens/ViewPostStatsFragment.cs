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
using Android.Support.V4.App;
using Android.Util;

namespace BlahguaMobile.AndroidClient.Screens
{
    class ViewPostStatsFragment : Fragment
    {
        private readonly string TAG = "ViewPostStatsFragment";

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View fragment = inflater.Inflate(Resource.Layout.fragment_viewpost_stats, null);
            //if (container == null)
            //{
            //    Log.Debug(TAG, "Dialer Fragment is in a view without container");
            //    return null;
            //}

            TextView text = fragment.FindViewById<TextView>(Resource.Id.text);
            //do some stuff like assigning event handlers, etc.

            return fragment;// base.OnCreateView(inflater, container, savedInstanceState);
        }
    }
}