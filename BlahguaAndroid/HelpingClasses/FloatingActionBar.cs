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
using Android.Graphics;

using BlahguaMobile.AndroidClient.HelpingClasses;

namespace BlahguaMobile.AndroidClient.HelpingClasses
{
    class FloatingActionBar : FrameLayout
    {
        private ImageView image;
        public FloatingActionBar(Context context)
            : base(context)
        {
            SharedConstructing(context);
        }

        public FloatingActionBar(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
            SharedConstructing(context);
        }

        public FloatingActionBar(Context context, IAttributeSet attrs, int defStyle)
            : base(context, attrs, defStyle)
        {
            SharedConstructing(context);
        }

        private void SharedConstructing(Context context)
        {
            Bitmap bm = BitmapFactory.DecodeResource(null, Resource.Drawable.ic_launcher);
            CircleDrawable myCircle = new CircleDrawable(bm);

            image = new ImageView(context);
            image.SetImageDrawable(myCircle);

            AddView(image, ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);

        }

       
    }
}
