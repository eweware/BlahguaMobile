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

namespace BlahguaMobile.AndroidClient.HelpingClasses
{
    class StringHelper
    {
        public static string ConstructTimeAgo(DateTime time)
        {
            string result = "some time ago";
            TimeSpan span = DateTime.Now - time;
            if (span.Days > 30)
            {
                // month
                result = span.Days / 30 + " month ago";
            }
            else if (span.Hours > 23)
            {
                // days
                result = span.Days + " days ago";
            }
            else if (span.Minutes > 59)
            {
                // hours
                result = span.Hours + " hours ago";
            }
            else if (span.Seconds > 59)
            {
                // minutes
                result = span.Minutes + " minutes ago";
            }
            else
            {
                // seconds
                result = span.Seconds + " seconds ago";
            }

            return result;
        }
    }
}