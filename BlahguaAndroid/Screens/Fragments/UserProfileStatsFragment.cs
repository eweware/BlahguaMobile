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
    class UserProfileStatsFragment : Fragment
    {
        public static UserProfileStatsFragment NewInstance()
        {
            return new UserProfileStatsFragment { Arguments = new Bundle() };
        }

        private readonly string TAG = "UserProfileStatsFragment";

        TextView views, opens, comments, userviews, useropens, usercreates, usercomments;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View fragment = inflater.Inflate(Resource.Layout.fragment_userprofile_stats, null);
            views = fragment.FindViewById<TextView>(Resource.Id.views);
            opens = fragment.FindViewById<TextView>(Resource.Id.opens);
            comments = fragment.FindViewById<TextView>(Resource.Id.comments);
            userviews = fragment.FindViewById<TextView>(Resource.Id.userviews);
            useropens = fragment.FindViewById<TextView>(Resource.Id.useropens);
            usercreates = fragment.FindViewById<TextView>(Resource.Id.usercreates);
            usercomments = fragment.FindViewById<TextView>(Resource.Id.usercomments);

            TextView conversion = fragment.FindViewById<TextView>(Resource.Id.conversion);
            TextView impression = fragment.FindViewById<TextView>(Resource.Id.impression);
            TextView score = fragment.FindViewById<TextView>(Resource.Id.score);
            TextView promotes = fragment.FindViewById<TextView>(Resource.Id.promotes);
            TextView demotes = fragment.FindViewById<TextView>(Resource.Id.demotes);

            User curUser = BlahguaAPIObject.Current.CurrentUser;

            double scoreAmount = curUser.K;

            score.Text = (Math.Floor(scoreAmount * 10000) / 100).ToString() + "%";

            LoadStats();

            return fragment;
        }

        private void LoadStats()
        {
            BlahguaAPIObject.Current.LoadUserStatsInfo((userInfo) =>
            {
                ///// user /////
                int userViewsCount = 0;
                for (int i = 0; i < userInfo.DayCount; ++i)
                {
                    userViewsCount += userInfo.UserViews(i);
                }
                int userOpensCount = 0;
                for (int i = 0; i < userInfo.DayCount; ++i)
                {
                    userOpensCount += userInfo.UserOpens(i);
                }
                int userCreatesCount = 0;
                for (int i = 0; i < userInfo.DayCount; ++i)
                {
                    userCreatesCount += userInfo.UserCreates(i);
                }
                int userCommentsCount = 0;
                for (int i = 0; i < userInfo.DayCount; ++i)
                {
                    userCommentsCount += userInfo.UserComments(i);
                }
                ///// other /////
                int viewsCount = 0;
                for (int i = 0; i < userInfo.DayCount; ++i)
                {
                    viewsCount += userInfo.Views(i);
                }
                int opensCount = 0;
                for (int i = 0; i < userInfo.DayCount; ++i)
                {
                    opensCount += userInfo.Opens(i);
                }
                int commentsCount = 0;
                for (int i = 0; i < userInfo.DayCount; ++i)
                {
                    commentsCount += userInfo.Comments(i);
                }

                Activity.RunOnUiThread(() =>
                {
                    views.Text = viewsCount.ToString();
                    opens.Text = opensCount.ToString();
                    comments.Text = commentsCount.ToString();
                    userviews.Text = userViewsCount.ToString();
                    useropens.Text = userOpensCount.ToString();
                    usercreates.Text = userCreatesCount.ToString();
                    usercomments.Text = userCommentsCount.ToString();
                });
            });

        }
    }
}