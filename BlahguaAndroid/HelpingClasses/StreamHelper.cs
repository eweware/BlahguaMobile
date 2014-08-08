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
using Android.Graphics;

namespace BlahguaMobile.AndroidClient.HelpingClasses
{
    class StreamHelper
    {
        public static Stream GetStreamFromFileUri(Context context, Android.Net.Uri uri, int maxSize = 0)
        {
            Stream result = null;
            if (context is Activity)
            {
                string filePath = null;

                if (uri.Scheme.Equals("content"))
                {
                    // old method (osVersion < 19)
                    filePath = GetPathToImage((Activity)context, uri);
                   
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
                    filePath = uri.EncodedPath;
                }

                if (!String.IsNullOrEmpty(filePath))
                {
                    if (maxSize > 0)
                    {
                        // TO DO:  Limit image size to 1024
                        Bitmap scaledImage = ResizeImageFile(filePath, maxSize);
                        MemoryStream stream = new MemoryStream();
                        scaledImage.Compress(Bitmap.CompressFormat.Jpeg, 90, stream);
                        result = stream;
                    }
                    else
                        result = System.IO.File.OpenRead(filePath);

                }
            }

            return result;
        }

        private static Bitmap ResizeImageFile(string filePath, int maxSize)
        {
            BitmapFactory.Options opt = new BitmapFactory.Options();

            opt.InJustDecodeBounds = true;
            Bitmap bp = BitmapFactory.DecodeFile(filePath, opt);
            int originalWidth = opt.OutWidth;
            int originalHeight = opt.OutHeight;

            double resizeScale = 1;

             if ( originalWidth > maxSize || originalHeight > maxSize ) 
             {
                double heightRatio = Math.Round((double)originalHeight / (double)maxSize);
                double widthRatio = Math.Round((double)originalWidth / (double)maxSize);
                resizeScale = heightRatio < widthRatio ? heightRatio : widthRatio;
            }
            //put the scale instruction (1 -> scale to (1/1); 8-> scale to 1/8)
            opt.InSampleSize = (int)resizeScale;
            opt.InJustDecodeBounds = false;

            bp = BitmapFactory.DecodeFile(filePath, opt);


            return bp;

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