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

namespace BlahguaMobile.AndroidClient
{
    class App
    {
        public static string BlahIdToOpen;

        public static readonly string EmailHelp = "info@goheard.com";
        public static readonly string EmailReportBug = "admin@goheard.com";
        public static readonly string WebsiteAbout = "http://www.goheard.com/";

        public static long WhatsNewDialogCloseTimeMs = 5000;
    }
}