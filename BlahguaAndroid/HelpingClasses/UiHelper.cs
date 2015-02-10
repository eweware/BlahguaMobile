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