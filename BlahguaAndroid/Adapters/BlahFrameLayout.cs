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
        private Handler handler;

        public BlahFrameLayout(Context context)
            : base(context)
        {
            this.SetWillNotDraw(false);
            handler = new Handler(Looper.MainLooper);
        }
        public BlahFrameLayout(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
            this.SetWillNotDraw(false);
            handler = new Handler();
        }

        private void postHandler()
        {
            
            if (Parent != null)
            {
                for (int i = 0; i < ChildCount; i++)
                {
                    View blah = GetChildAt(i);
                    
                    Rect scrollBounds = new Rect();
                    if (Parent.Parent is ScrollView)
                    {
                        var title = blah.FindViewById<LinearLayout>(Resource.Id.textLayout);
                        var image = blah.FindViewById<ImageView>(Resource.Id.image);
                        (Parent.Parent as ScrollView).GetHitRect(scrollBounds);
                        if (blah.GetLocalVisibleRect(scrollBounds))
                        {
                            if ((image.Tag != null) && (title.Tag == null))
                            {
                                fadein(title, null);
                            }
                        }
                        else
                        {
                            // stop animation
                            if ((image.Tag != null) && (title.Tag != null))
                            {
                                Animator animation = (Animator)title.Tag;
                                animation.Cancel();
                            }

                        }
                    }
                }

            }
           handler.PostDelayed(postHandler, 2000);
        }

        bool handlerPosted = false;

        protected override void OnDraw(Android.Graphics.Canvas canvas)
        {
            base.OnDraw(canvas);

            if (!handlerPosted)
            {
                //postHandler();
                handlerPosted = true;
            }
        }

        //////////////////////

        private static long fadeOutDuration = 2000;
        private static long fadeInDuration = 500;
        private static Random rnd = new Random();

        private static void fadeout(View v1, FadeListener listener)
        {
            //v1.Alpha = 0.9f;
            v1.Visibility = ViewStates.Visible;

            
            if (listener == null)
            {
                listener = new FadeListener();
                listener.v1 = v1;
            }
            long startDelay = 500 + rnd.Next(1000);
            v1.Animate().SetStartDelay(startDelay)
                .Alpha(0.0f)
                .SetDuration(fadeOutDuration)
                .SetListener(listener);
        }

        private static void fadein(View v1, FadeListener listener)
        {
            //v1.Alpha = 0.0f;
            v1.Visibility = ViewStates.Visible;


            if (listener == null)
            {
                listener = new FadeListener();
                listener.v1 = v1;
            }
            long startDelay = 2000 + rnd.Next(1000);
            v1.Animate().SetStartDelay(startDelay)
                .Alpha(0.9f)
                .SetDuration(fadeInDuration / 2)
                .SetListener(listener);
        }


        class FadeListener : Java.Lang.Object, Android.Animation.Animator.IAnimatorListener
        {
            bool flag;
            public View v1;

            public void OnAnimationCancel(Animator animation)
            {
                v1.Tag = null;
                //v1.Alpha = 0.0f;
            }

            public void OnAnimationEnd(Animator animation)
            {
                if (v1.Tag != null)
                {
                    if (flag)
                    {
                        //v1.Visibility = ViewStates.Gone;
                        fadein(v1, this);
                    }
                    else
                    {
                        //v2.Visibility = ViewStates.Gone;
                        fadeout(v1, this);
                    }
                    flag = !flag;
                }

            }

            public void OnAnimationRepeat(Animator animation)
            {
            }

            public void OnAnimationStart(Animator animation)
            {
                v1.Tag = animation;
            }
        }



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
                .SetDuration(fadeInDuration)
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
                .SetDuration(fadeOutDuration)
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
                        crossfade(v1, v2, rootView, 2000, this);
                    }
                    else
                    {
                        //v2.Visibility = ViewStates.Gone;
                        crossfade(v2, v1, rootView, 2000, this);
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