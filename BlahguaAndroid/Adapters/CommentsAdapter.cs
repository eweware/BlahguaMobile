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
using Android.Graphics;
using Android.Graphics.Drawables;

namespace BlahguaMobile.AndroidClient.Adapters
{
    class CommentsAdapter : BaseAdapter
    {
        ViewPostCommentsFragment _fragment;
        Activity _activity;
        CommentList _list;

        public CommentsAdapter(ViewPostCommentsFragment fragment, CommentList list)
        {
            _fragment = fragment;
            _activity = fragment.Activity;
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

        private EventHandler imageClick = (sender, args) =>
        {
            View v = (View)sender;
            string url = (string)v.Tag;

            var intent = new Intent(v.Context, typeof(ImageViewActivity));
            intent.PutExtra("image", url);
            v.Context.StartActivity(intent);
        };

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
			View view;

			if (convertView != null)
				view = convertView;
			else
				view = _activity.LayoutInflater.Inflate( Resource.Layout.listitem_comment, parent, false);
            var text = view.FindViewById<TextView>(Resource.Id.text);
            var image = view.FindViewById<ImageView>(Resource.Id.image);
            var author = view.FindViewById<TextView>(Resource.Id.author);
            var author_avatar = view.FindViewById<ImageView>(Resource.Id.author_avatar);
            var time_ago = view.FindViewById<TextView>(Resource.Id.time_ago);
            var upvoted = view.FindViewById<TextView>(Resource.Id.upvoted);
            var downvoted = view.FindViewById<TextView>(Resource.Id.downvoted);
            var badgeIcon = view.FindViewById<ImageView>(Resource.Id.comment_badged);
			var detailsView = view.FindViewById<LinearLayout> (Resource.Id.comment_author_details);
			var badgeList = view.FindViewById<ListView> (Resource.Id.comment_author_badgelist);
            TextView authorDesc = view.FindViewById<TextView>(Resource.Id.comment_author_desc);

			detailsView.Visibility = ViewStates.Gone;

            // set fonts
            UiHelper.SetGothamTypeface(TypefaceStyle.Normal, text, upvoted, downvoted);
            UiHelper.SetGothamTypeface(TypefaceStyle.Bold, author);
			UiHelper.SetGothamTypeface(TypefaceStyle.Italic, time_ago, authorDesc);

            Comment c = _list[position];
            if (!String.IsNullOrEmpty(c.ImageURL))
            {
				image.Visibility = ViewStates.Visible;
                image.SetUrlDrawable(c.ImageURL);
                image.Tag = c.ImageURL;
                image.Click -= imageClick;
                image.Click += imageClick;
            }
            else
            {
                image.Visibility = ViewStates.Gone;
            }
			view.Background = new ColorDrawable (Color.White);
            text.Text = c.T;
            author.Text = c.AuthorName;
            author_avatar.SetUrlDrawable(c.AuthorImage);
			author_avatar.Visibility = ViewStates.Visible;
            time_ago.Text = StringHelper.ConstructTimeAgo(c.CreationDate);


            upvoted.Text = c.UpVoteCount.ToString();
            downvoted.Text = c.DownVoteCount.ToString();


            // description
            if (!c.XX)
                authorDesc.Text = c.DescriptionString;
            else
                authorDesc.Text = "an unidentified person";

			if (c.XXX && ((BlahguaAPIObject.Current.CurrentUser == null) ||
			    (BlahguaAPIObject.Current.CurrentUser.XXX == false))) {
				view.Background = new ColorDrawable (Color.Red);
				text.Text = "(mature content)";
				image.Visibility = ViewStates.Gone;
			} 

            
            // badges
            if ((c.BD == null) || (c.BD.Count == 0))
            {
                badgeIcon.Visibility = ViewStates.Gone;
                //authorBadge.Text = "no badges";
				badgeList.Visibility = ViewStates.Gone;
				badgeList.Adapter = null;
            }
            else
            {
                badgeIcon.Visibility = ViewStates.Visible;
				c.AwaitBadgeData ((didIt) => {
					badgeList.Visibility = ViewStates.Visible;
					badgeList.Adapter = new ViewCommentBadgesAdapter(_activity, c);
				});

            }
            view.Tag = position;

            if (convertView == null)
            {
				view.Click += view_Click;
				author_avatar.Click += (object sender, EventArgs e) => {
					if (detailsView.Visibility == ViewStates.Gone)
						detailsView.Visibility = ViewStates.Visible;
					else
						detailsView.Visibility = ViewStates.Gone;
				};
            }
            
            return view;
        }
       

        void evPromoteClick(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            LinearLayout l = btn.Tag as LinearLayout;
            int pos = (int)l.Tag;

            Comment c = _list[pos];
            BlahguaAPIObject.Current.SetCommentVote(c, 1, (newVote) =>
            {
                _fragment.LoadComments();
                UpdateVoteButtons(l, c);
					HomeActivity.analytics.PostCommentVote(1);
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
                _fragment.LoadComments();
                UpdateVoteButtons(l, c);
					HomeActivity.analytics.PostCommentVote(-1);
            });

            collapseComment(l);
        }

        void view_Click(object sender, EventArgs e)
        {
            View item = sender as View;

            int pos = (int)item.Tag;

            if (_list[pos].A == BlahguaAPIObject.Current.CurrentUser._id)
            {
                // this is a users comment - skip showing promote/demote buttons
                return;
            }

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

            UpdateVoteButtons(item, _list[pos]);
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




        public void UpdateVoteButtons(View item, Comment c)
        {
            Button btn_upvote = item.FindViewById<Button>(Resource.Id.btn_upvote);
            Button btn_downvote = item.FindViewById<Button>(Resource.Id.btn_downvote);

            _activity.RunOnUiThread(() =>
            {
                if (c.A == BlahguaAPIObject.Current.CurrentUser._id)
                {
                    btn_upvote.Enabled = false;
                    btn_downvote.Enabled = false;
                }
                else if (c.uv == 0)
                {
                    btn_upvote.Enabled = true;
                    btn_downvote.Enabled = true;
                }
                else
                {
                    btn_upvote.Enabled = false;
                    btn_downvote.Enabled = false;
                    if (c.uv == 1)
                    {
                        btn_upvote.SetBackgroundResource(Resource.Drawable.btn_promote_active);
                    }
                    else
                    {
                        btn_downvote.SetBackgroundResource(Resource.Drawable.btn_demote_active);
                    }
                }
            });
        }
    }
}