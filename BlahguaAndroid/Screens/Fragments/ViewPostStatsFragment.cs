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
using Android.Util;
using BlahguaMobile.BlahguaCore;

namespace BlahguaMobile.AndroidClient.Screens
{
    class ViewPostStatsFragment : Fragment
    {
        public static ViewPostStatsFragment NewInstance()
        {
            return new ViewPostStatsFragment { Arguments = new Bundle() };
        }

        private readonly string TAG = "ViewPostStatsFragment";

        TextView views, opens, comments;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            MainActivity.analytics.PostPageView("/blah/stats");
            View fragment = inflater.Inflate(Resource.Layout.fragment_viewpost_stats, null);
            views = fragment.FindViewById<TextView>(Resource.Id.views);
            opens = fragment.FindViewById<TextView>(Resource.Id.opens);
            comments = fragment.FindViewById<TextView>(Resource.Id.comments);

            TextView conversion = fragment.FindViewById<TextView>(Resource.Id.conversion);
            TextView impression = fragment.FindViewById<TextView>(Resource.Id.impression);
            TextView score = fragment.FindViewById<TextView>(Resource.Id.score);
            TextView promotes = fragment.FindViewById<TextView>(Resource.Id.promotes);
            TextView demotes = fragment.FindViewById<TextView>(Resource.Id.demotes);

            Blah curBlah = BlahguaAPIObject.Current.CurrentBlah;

            double scoreAmount = curBlah.S;
            int promotesAmount = curBlah.P;
            int demotesAmount = curBlah.D;

            conversion.Text = curBlah.ConversionString;
            impression.Text = curBlah.ImpressionString;

            score.Text = (Math.Floor(scoreAmount * 10000) / 100).ToString() + "%";
            promotes.Text = promotesAmount.ToString();
            demotes.Text = demotesAmount.ToString();

            LoadStats();

            return fragment;// base.OnCreateView(inflater, container, savedInstanceState);
        }

        private void LoadStats()
        {
            BlahguaAPIObject.Current.LoadBlahStats((stats) =>
            {
                int viewsCount = 0;
                foreach (int v in stats.Impressions)
                    viewsCount += v;

                int opensCount = 0;
                foreach (int o in stats.Opens)
                    opensCount += o;

                int commentsCount = 0;
                foreach (int c in stats.Comments)
                    commentsCount += c;

                Activity.RunOnUiThread(() =>
                {
                    views.Text = viewsCount.ToString();
                    opens.Text = opensCount.ToString();
                    comments.Text = commentsCount.ToString();
                });
            });

        }
    }
}