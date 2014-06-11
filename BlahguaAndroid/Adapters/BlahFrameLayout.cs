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
using Android.Graphics;
using Android.Animation;
using Android.Util;
using BlahguaMobile.AndroidClient.ThirdParty.UrlImageViewHelper;

namespace BlahguaMobile.AndroidClient.Adapters
{
    class BlahFrameLayout : RelativeLayout
    {

        public BlahFrameLayout(Context context)
            : base(context)
        {
            this.SetWillNotDraw(false);
            r = new Random((int)DateTime.Now.Ticks);
        }
        public BlahFrameLayout(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
            this.SetWillNotDraw(false);
            r = new Random((int)DateTime.Now.Ticks);
        }

        static Random r;
        protected override void OnDraw(Android.Graphics.Canvas canvas)
        {
            base.OnDraw(canvas);

            for (int i = 0; i < ChildCount; i++)
            {
                View blah = GetChildAt(i);
                Rect scrollBounds = new Rect();
                if (Parent.Parent is ScrollView)
                {
                    (Parent.Parent as ScrollView).GetHitRect(scrollBounds);
                    if (blah.GetLocalVisibleRect(scrollBounds))
                    {
                        var title = blah.FindViewById<TextView>(Resource.Id.title);
                        var image = blah.FindViewById<ImageView>(Resource.Id.image);
                        if (image.Tag != null)
                        {
                            string imageUrl = image.Tag.ToString();
                            image.SetUrlDrawable(imageUrl);
                            image.Tag = null;
                        }

                        // do animation
                        if (blah.Tag == null && image.Drawable != null)
                        {
                            blah.Tag = true;
                            crossfade(image, title, blah, r.Next(3000), null);
                        }
                    }
                    else
                    {
                        // stop animation
                        if (blah.Tag != null)
                            blah.Tag = false;
                    }
                }
            }
        }

        //////////////////////

        private static long fadeDuration = 3000;
        private static void crossfade(View v1, View v2, View rootView, long startDelay, CrossfadeListener listener)
        {

            // Set the content view to 0% opacity but visible, so that it is visible
            // (but fully transparent) during the animation.
            v1.Alpha = 0.0f;
            v1.Visibility = ViewStates.Visible;

            v2.Alpha = 1.0f;
            v2.Visibility = ViewStates.Visible;

            // Animate the content view to 100% opacity, and clear any animation
            // listener set on the view.
            v1.Animate().SetStartDelay(startDelay)
                .Alpha(1.0f)
                .SetDuration(fadeDuration)
                .SetListener(null);

            // Animate the loading view to 0% opacity. After the animation ends,
            // set its visibility to GONE as an optimization step (it won't
            // participate in layout passes, etc.)
            if (listener == null)
            {
                listener = new CrossfadeListener();
                listener.v1 = v1;
                listener.v2 = v2;
                listener.rootView = rootView;
            }
            v2.Animate().SetStartDelay(startDelay)
                .Alpha(0.0f)
                .SetDuration(fadeDuration)
                .SetListener(listener);
        }
        class CrossfadeListener : Java.Lang.Object, Android.Animation.Animator.IAnimatorListener
        {
            bool flag;
            public View v1;
            public View v2;
            public View rootView;
            public void OnAnimationCancel(Animator animation)
            {
            }

            public void OnAnimationEnd(Animator animation)
            {
                bool? tag = (Boolean)rootView.Tag;
                if (tag != null && tag.Value)
                {
                    if (flag)
                    {
                        //v1.Visibility = ViewStates.Gone;
                        crossfade(v1, v2, rootView, r.Next(5000), this);
                    }
                    else
                    {
                        //v2.Visibility = ViewStates.Gone;
                        crossfade(v2, v1, rootView, r.Next(5000), this);
                    }
                    flag = !flag;
                }
                else
                {
                    rootView.Tag = null;
                }
            }

            public void OnAnimationRepeat(Animator animation)
            {
            }

            public void OnAnimationStart(Animator animation)
            {
            }
        }
    }
}