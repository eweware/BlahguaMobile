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
using Android.Graphics;
using BlahguaMobile.AndroidClient.HelpingClasses;

namespace BlahguaMobile.AndroidClient.Screens
{
    class WhatsNewDialog : DialogFragment
    {
        private WhatsNewInfo newInfo;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.dialog_whatsnew, null);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            view.Click += (sender, args) =>
            {
                this.DismissAllowingStateLoss();
            };

            var title = view.FindViewById<TextView>(Resource.Id.title);
            var comments = view.FindViewById<TextView>(Resource.Id.comments);
            var upvotes = view.FindViewById<TextView>(Resource.Id.upvotes);
            var downvotes = view.FindViewById<TextView>(Resource.Id.downvotes);
            var messages = view.FindViewById<TextView>(Resource.Id.messages);
            var opens = view.FindViewById<TextView>(Resource.Id.opens);

            UiHelper.SetGothamTypeface(TypefaceStyle.Normal, title, comments, upvotes, downvotes, messages, opens);

            if ((newInfo.message != null) && (newInfo.message != ""))
            {
                title.Text = newInfo.message;
                title.Visibility = ViewStates.Visible;
            }
            else
                title.Visibility = ViewStates.Gone;

            if (newInfo.newComments > 0)
            {
                comments.Text = newInfo.newComments.ToString() + " new comments";
                comments.Visibility = ViewStates.Visible;
            }
            else
                comments.Visibility = ViewStates.Gone;

            if (newInfo.newOpens > 0)
            {
                opens.Text = newInfo.newOpens.ToString() + " new opens";
                opens.Visibility = ViewStates.Visible;
            }
            else
                opens.Visibility = ViewStates.Gone;

            if (newInfo.newUpVotes > 0)
            {
                upvotes.Text = newInfo.newUpVotes.ToString() + " new upvotes";
                upvotes.Visibility = ViewStates.Visible;
            }
            else
                upvotes.Visibility = ViewStates.Gone;

            if (newInfo.newDownVotes > 0)
            {
                downvotes.Text = newInfo.newDownVotes.ToString() + " new downvotes";
                downvotes.Visibility = ViewStates.Visible;
            }
            else
                downvotes.Visibility = ViewStates.Gone;

            if (newInfo.newMessages > 0)
            {
                messages.Text = newInfo.newMessages.ToString() + " new messages";
                messages.Visibility = ViewStates.Visible;
            }
            else
                messages.Visibility = ViewStates.Gone;
        }

        public static WhatsNewDialog ShowDialog(FragmentManager manager, WhatsNewInfo newInfo)
        {
            WhatsNewDialog f = new WhatsNewDialog();
            f.SetStyle(DialogFragmentStyle.NoTitle, 0);
            f.newInfo = newInfo;
            try
            {
                f.Show(manager, "WhatsNewDialog");
            }
            catch (Exception)
            {
            }

            return f;
        }
    }
}