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

namespace BlahguaMobile.AndroidClient.HelpingClasses
{
    public class ResizableImageView : ImageView
    {
        public ResizableImageView(Context context, IAttributeSet attrs) :
            base(context, attrs)
        {
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            if (Drawable != null)
            {
                // ceil not round - avoid thin vertical gaps along the left/right edges
                int width = MeasureSpec.GetSize(widthMeasureSpec);
                int height = (int)Math.Ceiling((float)width * (float)Drawable.IntrinsicHeight / (float)Drawable.IntrinsicWidth);
                SetMeasuredDimension(width, height);
            }
            else
            {
                base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
            }
        }
    }
}