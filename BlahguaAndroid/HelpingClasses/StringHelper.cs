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
            if (span.TotalDays > 360)
            {
                result = "a long time ago";
            }
            else if (span.TotalDays > 70)
            {
                // month
                result = (int)(span.TotalDays / 30) + " months ago";
            }
            else if (span.TotalHours > 48)
            {
                // days
                result = (int)(span.TotalDays) + " days ago";
            }
            else if (span.TotalMinutes > 120)
            {
                // hours
                result = (int)(span.TotalHours) + " hours ago";
            }
            else if (span.TotalSeconds > 120)
            {
                // minutes
                result = (int)(span.TotalMinutes) + " minutes ago";
            }
            else if (span.TotalSeconds > 15)
            {
                // minutes
                result =(int)(span.TotalSeconds) + " seconds ago";
            }
            else
            {
                // seconds
                result = "just now";
            }

            return result;
        }
    }
}