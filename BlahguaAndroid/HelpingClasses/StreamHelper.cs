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
using System.IO;
using Android.Database;

namespace BlahguaMobile.AndroidClient.HelpingClasses
{
    class StreamHelper
    {
        public static Stream GetStreamFromFileUri(Context context, Android.Net.Uri uri)
        {
            Stream result = null;
            if (context is Activity)
            {
                if (uri.Scheme.Equals("content"))
                {
                    //result = context.ContentResolver.OpenInputStream(uri);
                    // old method (osVersion < 19)
                    string imgPath = GetPathToImage((Activity)context, uri);
                    if (imgPath != null)
                    {
                        result = System.IO.File.OpenRead(imgPath);
                    }
                    //int osVersion = (int)Build.VERSION.SdkInt;
                    //if (osVersion >= 19)
                    //{
                    //    // TODO new kit kat API methods are currently
                    //    // not available in the last Xamarin updates
                    //    // So Bug https://eweware.atlassian.net/browse/BM-343 can't be resolved
                    //    // The way to fix it is described at the following link:
                    //    // http://stackoverflow.com/questions/19834842/android-gallery-on-kitkat-returns-different-uri-for-intent-action-get-content
                    //}
                }
                else if (uri.Scheme.Equals("file"))
                {
                    result = System.IO.File.OpenRead(uri.EncodedPath);
                }
            }

            return result;
        }
        public static string GetFileName(Context context, Android.Net.Uri uri)
        {
            String result = null;
            if (uri.Scheme.Equals("content"))
            {
                string imgPath = GetPathToImage((Activity)context, uri);

                result = System.IO.Path.GetFileName(imgPath);
            }
            else if (uri.Scheme.Equals("file"))
            {
                result = System.IO.Path.GetFileName(uri.EncodedPath);
            }

            return result;
        }
        public static string GetPathToImage(Activity activity, Android.Net.Uri uri)
        {
            string path = null;
            // The projection contains the columns we want to return in our query.
            string[] projection = new[] { Android.Provider.MediaStore.Images.Media.InterfaceConsts.Data };
            using (ICursor cursor = activity.ManagedQuery(uri, projection, null, null, null))
            {
                if (cursor != null)
                {
                    cursor.MoveToFirst();
                    int columnIndex = cursor.GetColumnIndexOrThrow(Android.Provider.MediaStore.Images.Media.InterfaceConsts.Data);
                    FieldType theType = cursor.GetType(columnIndex);
                    path = cursor.GetString(columnIndex);
                }
            }
            return path;
        }
    }
}