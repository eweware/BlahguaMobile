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
using Android.Animation;
using BlahguaMobile.AndroidClient.Adapters;
using BlahguaMobile.AndroidClient.HelpingClasses;
using Android.Graphics;

namespace BlahguaMobile.AndroidClient.Screens
{
    class HistoryCommentsFragment : Fragment
    {
        public static HistoryCommentsFragment NewInstance()
        {
            return new HistoryCommentsFragment { Arguments = new Bundle() };
        }

        private TextView comments_total_count;

        private ListView list;
        private LinearLayout no_comments;
        private ProgressDialog progressDlg;

        private HistoryCommentsAdapter adapter;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            progressDlg = new ProgressDialog(this.Activity);
            progressDlg.SetProgressStyle(ProgressDialogStyle.Spinner);

			HomeActivity.analytics.PostPageView("/self/comments");
            View fragment = inflater.Inflate(Resource.Layout.fragment_history_comments, null);

            comments_total_count = fragment.FindViewById<TextView>(Resource.Id.comments_total_count);
            comments_total_count.Text = "loading comments...";
            UiHelper.SetGothamTypeface(TypefaceStyle.Normal, comments_total_count);

            list = fragment.FindViewById<ListView>(Resource.Id.list);
            list.ChoiceMode = ListView.ChoiceModeNone;
            no_comments = fragment.FindViewById<LinearLayout>(Resource.Id.no_comments);

           // LoadUserComments();

            return fragment;
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);
            LoadUserComments();
        }

        public void LoadUserComments()
        {
            Activity.RunOnUiThread(() =>
            {
                comments_total_count.Text = "loading comments";
                progressDlg.SetMessage("loading comments...");
                progressDlg.Show();

                BlahguaAPIObject.Current.LoadUserComments((theComments) =>
                    {
                        Activity.RunOnUiThread(() =>
                        {
                            progressDlg.Hide();
                            if ((theComments != null) && (theComments.Count > 0))
                            {
                                comments_total_count.Text = "Your Comments (" + theComments.Count + ")";
                                no_comments.Visibility = ViewStates.Gone;
                                list.Visibility = ViewStates.Visible;
                                adapter = new HistoryCommentsAdapter(this, theComments);
                                list.Adapter = adapter;
                            }
                            else
                            {
                                comments_total_count.Text = "Your Comments (0)";
                                no_comments.Visibility = ViewStates.Visible;
                                list.Visibility = ViewStates.Gone;
                                list.Adapter = adapter = null;
                            }
                        });
                    }
                );
            });
        }
    }
}