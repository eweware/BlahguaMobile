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
using BlahguaMobile.AndroidClient.HelpingClasses;
using Android.Animation;
using BlahguaMobile.AndroidClient.Screens;

namespace BlahguaMobile.AndroidClient.Adapters
{
    class CommentsAdapter : BaseAdapter
    {
        Activity _activity;
        CommentList _list;

        public CommentsAdapter(Activity activity, CommentList list)
        {
            _activity = activity;
            _list = list;
        }

        public void setComments(CommentList list)
        {
            _list = list;
        }


        public override int Count
        {
            get { return _list.Count; }
        }

        public Comment GetComment(int position)
        {
            return _list[position];
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
            var view = convertView ?? _activity.LayoutInflater.Inflate(
                                 Resource.Layout.listitem_comment, parent, false);
            var text = view.FindViewById<TextView>(Resource.Id.text);
            var image = view.FindViewById<ImageView>(Resource.Id.image);
            var author = view.FindViewById<TextView>(Resource.Id.author);
            var author_avatar = view.FindViewById<ImageView>(Resource.Id.author_avatar);
            var time_ago = view.FindViewById<TextView>(Resource.Id.time_ago);
            var upvoted = view.FindViewById<TextView>(Resource.Id.upvoted);
            var downvoted = view.FindViewById<TextView>(Resource.Id.downvoted);
            var badgeIcon = view.FindViewById<ImageView>(Resource.Id.comment_badged);
            //LinearLayout authorDetailArea = view.FindViewById<LinearLayout>(Resource.Id.comment_author_details);
            TextView authorDesc = view.FindViewById<TextView>(Resource.Id.comment_author_desc);
            TextView authorBadge = view.FindViewById<TextView>(Resource.Id.comment_author_badge);

            Comment c = _list[position];
            if (!String.IsNullOrEmpty(c.ImageURL))
            {
                image.SetUrlDrawable(c.ImageURL);
                
            }
            else
            {
                image.Visibility = ViewStates.Gone;
            }

            text.SetText(c.T, Android.Widget.TextView.BufferType.Normal);
            author.Text = c.AuthorName;
            author_avatar.SetUrlDrawable(c.AuthorImage);
            time_ago.Text = StringHelper.ConstructTimeAgo(c.CreationDate);

            upvoted.Text = c.UpVoteCount.ToString();
            downvoted.Text = c.DownVoteCount.ToString();


            // description

            authorDesc.Text = c.DescriptionString;
            
            // badges
            if ((c.BD == null) || (c.BD.Count == 0))
            {
                badgeIcon.Visibility = ViewStates.Gone;
                authorBadge.Text = "no badges";
            }
            else
            {
                badgeIcon.Visibility = ViewStates.Visible;
                authorBadge.Text = c.BD.Count.ToString() + " badge(s)";
            }
            view.Tag = position;
            view.Click -= view_Click;
            view.Click += view_Click;            return view;
        }
        
        void evPromoteClick(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            LinearLayout l = btn.Tag as LinearLayout;
            int pos = (int)l.Tag;

            Comment c = _list[pos];
            BlahguaAPIObject.Current.SetCommentVote(c, 1, (newVote) =>
            {
                //UpdateVoteButtons();
                MainActivity.analytics.PostCommentVote(1);
            });

            collapseComment(l);
        }
        
        void evDemoteClick(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            LinearLayout l = btn.Tag as LinearLayout;
            int pos = (int)l.Tag;

            Comment c = _list[pos];
            BlahguaAPIObject.Current.SetCommentVote(c, -1, (newVote) =>
            {
                //UpdateVoteButtons();
                MainActivity.analytics.PostCommentVote(-1);
            });

            collapseComment(l);
        }

        void view_Click(object sender, EventArgs e)
        {
            View item = sender as View;
            LinearLayout layout = item.FindViewById<LinearLayout>(Resource.Id.votes);
            Button btn_upvote = item.FindViewById<Button>(Resource.Id.btn_upvote);
            Button btn_downvote = item.FindViewById<Button>(Resource.Id.btn_downvote);
            if (layout != null && layout.Visibility.Equals(ViewStates.Gone))
            {
                //set Visible
                layout.Visibility = ViewStates.Visible;
                int widthSpec = View.MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified);
                int heightSpec = View.MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified);
                layout.Measure(widthSpec, heightSpec);

                ValueAnimator mAnimator = slideAnimator(layout, 0, layout.MeasuredWidth, true);
                mAnimator.Start();

                layout.Tag = item.Tag;
                btn_upvote.Tag = layout;
                btn_downvote.Tag = layout;

                btn_upvote.Click -= evPromoteClick;
                btn_downvote.Click -= evDemoteClick;
                btn_upvote.Click += evPromoteClick;
                btn_downvote.Click += evDemoteClick;
            }
            else
            {
                //collapse();
                int finalWidth = layout.Width;

                ValueAnimator mAnimator = slideAnimator(layout, finalWidth, 0, true);
                mAnimator.Start();
                mAnimator.AnimationEnd += (object IntentSender, EventArgs arg) =>
                {
                    layout.Visibility = ViewStates.Gone;
                };//mLinearLayout.Visibility = ViewStates.Gone;

            }
        }

        private static void collapseComment(LinearLayout l)
        {

            //collapse();
            int finalWidth = l.Width;

            ValueAnimator Animator = slideAnimator(l, finalWidth, 0, true);
            Animator.Start();
            Animator.AnimationEnd += (object IntentSender, EventArgs arg) =>
            {
                l.Visibility = ViewStates.Gone;
            };//mLinearLayout.Visibility = ViewStates.Gone;
        }

        private static ValueAnimator slideAnimator(View layout, int start, int end, bool animatingWidth)
        {
            ValueAnimator animator = ValueAnimator.OfInt(start, end);
            //ValueAnimator animator2 = ValueAnimator.OfInt(start, end);
            //  animator.AddUpdateListener (new ValueAnimator.IAnimatorUpdateListener{
            animator.Update +=
                (object sender, ValueAnimator.AnimatorUpdateEventArgs e) =>
                {
                    //  int newValue = (int)
                    //e.Animation.AnimatedValue; // Apply this new value to the object being animated.
                    //  myObj.SomeIntegerValue = newValue; 
                    var value = (int)animator.AnimatedValue;
                    ViewGroup.LayoutParams layoutParams = layout.LayoutParameters;
                    if (animatingWidth)
                        layoutParams.Width = value;
                    else
                        layoutParams.Height = value;
                    layout.LayoutParameters = layoutParams;

                };

            //      });
            return animator;
        }




        public void UpdateVoteButtons()
        {
            //btn_promote.IconUri = new Uri("/Images/Icons/white_promote.png", UriKind.Relative);
            //btn_demote.IconUri = new Uri("/Images/Icons/white_demote.png", UriKind.Relative);
            //Blah curBlah = BlahguaAPIObject.Current.CurrentBlah;

            //if (BlahguaAPIObject.Current.CurrentUser != null)
            //{
            //    RunOnUiThread(() =>
            //    {
            //        if (curBlah.A == BlahguaAPIObject.Current.CurrentUser._id)
            //        {
            //            btn_promote.Enabled = false;
            //            btn_demote.Enabled = false;
            //        }
            //        else if (curBlah.uv == 0)
            //        {
            //            btn_promote.Enabled = true;
            //            btn_demote.Enabled = true;
            //        }
            //        else
            //        {
            //            btn_promote.Enabled = false;
            //            btn_demote.Enabled = false;
            //            if (curBlah.uv == 1)
            //            {
            //                btn_promote.SetBackgroundResource(Resource.Drawable.btn_promote_active);
            //            }
            //            else
            //            {
            //                btn_demote.SetBackgroundResource(Resource.Drawable.btn_demote_active);
            //            }
            //        }
            //    });
            //}
        }
    }
}