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
using Android.Animation;
using BlahguaMobile.AndroidClient.Screens;
using Android.Graphics;

namespace BlahguaMobile.AndroidClient.HelpingClasses
{
    class UiHelper
    {
        public static void SetGothamTypeface(TypefaceStyle style, params TextView[] views)
        {
            foreach (TextView view in views)
            {
                view.SetTypeface(HomeActivity.gothamFont, style);
            }
        }

        public static void SetListViewHeightBasedOnChildren(ListView listView) {
            IListAdapter listAdapter = listView.Adapter; 
            if (listAdapter == null) {
                // pre-condition
                return;
            }

            int totalHeight = 0;
            for (int i = 0; i < listAdapter.Count; i++) {
                View listItem = listAdapter.GetView(i, null, listView);
                listItem.Measure(0, 0);
                totalHeight += listItem.MeasuredHeight;
            }

            ViewGroup.LayoutParams prms = listView.LayoutParameters;
            prms.Height = totalHeight + (listView.DividerHeight * (listAdapter.Count - 1));
            listView.LayoutParameters = prms;
            listView.RequestLayout();
        }    

        public static View GetViewByPosition(int position, ListView listView)
        {
            int firstListItemPosition = listView.FirstVisiblePosition;
            int lastListItemPosition = firstListItemPosition + listView.ChildCount - 1;

            if (position < firstListItemPosition || position > lastListItemPosition)
            {
                return listView.Adapter.GetView(position, null, listView);
            }
            else
            {
                int childIndex = position - firstListItemPosition;
                return listView.GetChildAt(childIndex);
            }
        }

        public static void ManageSwipe(View listItem, bool left, bool right)
        {
            View leftView = listItem.FindViewById<View>(Resource.Id.left_layout);
            View rightView = listItem.FindViewById<View>(Resource.Id.right_layout);

            bool leftNeedsToBeHidden = left == false && right == true && leftView.Visibility == ViewStates.Visible;
            bool rightNeedsToBeHidden = left == true && right == false && rightView.Visibility == ViewStates.Visible;

            if (!rightNeedsToBeHidden)
            {
                if (left)
                {
                    if (leftView.Visibility == ViewStates.Gone)
                    {
                        // show left view
                        leftView.Visibility = ViewStates.Visible;
                        int widthSpec = View.MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified);
                        int heightSpec = View.MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified);
                        leftView.Measure(widthSpec, heightSpec);

                        ValueAnimator mAnimator = slideAnimator(leftView, 0, leftView.MeasuredWidth, true);
                        mAnimator.Start();

                        EventHandler ev = (s, args) =>
                        {
                            //collapse();
                            int finalWidth = leftView.Width;

                            ValueAnimator Animator = slideAnimator(leftView, finalWidth, 0, true);
                            Animator.Start();
                            Animator.AnimationEnd += (object IntentSender, EventArgs arg) =>
                            {
                                leftView.Visibility = ViewStates.Gone;
                            };
                        };
                        ((Button)leftView.FindViewById<Button>(Resource.Id.btn_open)).Click += ev;
                    }
                    else
                    {
                        // ignore
                    }
                }
                else
                {
                    if (leftView.Visibility == ViewStates.Gone)
                    {
                        // ignore
                    }
                    else
                    {
                        // hide left view
                        int finalWidth = leftView.Width;

                        ValueAnimator Animator = slideAnimator(leftView, finalWidth, 0, true);
                        Animator.Start();
                        Animator.AnimationEnd += (object IntentSender, EventArgs arg) =>
                        {
                            leftView.Visibility = ViewStates.Gone;
                        };
                    }
                }
            }

            if (!leftNeedsToBeHidden)
            {
                if (right)
                {
                    if (rightView.Visibility == ViewStates.Gone)
                    {
                        // show right view
                        rightView.Visibility = ViewStates.Visible;
                        int widthSpec = View.MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified);
                        int heightSpec = View.MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified);
                        rightView.Measure(widthSpec, heightSpec);

                        ValueAnimator mAnimator = slideAnimator(rightView, 0, rightView.MeasuredWidth, true);
                        mAnimator.Start();

                        EventHandler ev = (s, args) =>
                        {
                            //collapse();
                            int finalWidth = rightView.Width;

                            ValueAnimator Animator = slideAnimator(rightView, finalWidth, 0, true);
                            Animator.Start();
                            Animator.AnimationEnd += (object IntentSender, EventArgs arg) =>
                            {
                                rightView.Visibility = ViewStates.Gone;
                            };
                        };
                        ((Button)rightView.FindViewById<Button>(Resource.Id.btn_delete)).Click += ev;
                    }
                    else
                    {
                        // ignore
                    }
                }
                else
                {
                    if (rightView.Visibility == ViewStates.Gone)
                    {
                        // ignore
                    }
                    else
                    {
                        // hide right view
                        int finalWidth = rightView.Width;

                        ValueAnimator Animator = slideAnimator(rightView, finalWidth, 0, true);
                        Animator.Start();
                        Animator.AnimationEnd += (object IntentSender, EventArgs arg) =>
                        {
                            rightView.Visibility = ViewStates.Gone;
                        };
                    }
                }
            }

        }

        /////////////////////////////

        private static ValueAnimator slideAnimator(View layout, int start, int end, bool animatingWidth)
        {
            ValueAnimator animator = ValueAnimator.OfInt(start, end);
            animator.Update +=
                (object sender, ValueAnimator.AnimatorUpdateEventArgs e) =>
                {
                    var value = (int)animator.AnimatedValue;
                    ViewGroup.LayoutParams layoutParams = layout.LayoutParameters;
                    if (animatingWidth)
                        layoutParams.Width = value;
                    else
                        layoutParams.Height = value;
                    layout.LayoutParameters = layoutParams;

                };

            return animator;
        }
    }
}