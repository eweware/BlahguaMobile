using Android.Graphics;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BlahguaMobile.AndroidClient.HelpingClasses
{

	public class ImageLoader
    {
        private static ImageLoader _instance;
        public static ImageLoader Instance { get { return _instance ?? (_instance = new ImageLoader()); } }

        private WebClient webClient;
        private static string documentsPath;
        private static string userFolderPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
        private static string defaultBucketName = "default";
        private static string curBucket = ""
            ;
        private ImageLoader()
        {
            documentsPath = System.IO.Path.Combine(userFolderPath, defaultBucketName);
            new Java.IO.File(documentsPath).Mkdir();
        }

        public static string GetLocalFilepath(string url)
        {
            string localFilename = url.Replace(":", "_").Replace("\\", "_").Replace("/", "_");
            string localPath;

            if (String.IsNullOrEmpty(curBucket))
                localPath = System.IO.Path.Combine(documentsPath, localFilename);
            else
                localPath = System.IO.Path.Combine(documentsPath, curBucket, localFilename);

            return localPath;
        }

        public string CurrentBucket
        {
            get {return curBucket;}
            set
            {
                if (!String.IsNullOrEmpty(curBucket))
                    EmptyBucket(curBucket);
                curBucket = value;
            }
        }

        private void EmptyBucket(string bucketName)
        {
            string bucketPath = System.IO.Path.Combine(documentsPath, bucketName);
            Java.IO.File theDir = new Java.IO.File(bucketPath);

            foreach (Java.IO.File theFile in theDir.ListFiles())
            {
                theFile.Delete();
            }
        }

        public async void DownloadAsync(string imageUrl, Func<Bitmap, bool> result)
        {
            webClient = new WebClient();
            var url = new Uri(imageUrl);
            byte[] bytes = null;

            string localPath = GetLocalFilepath(imageUrl);
            Java.IO.File file = new Java.IO.File(localPath);
            if (!file.Exists())
            {
                // downloading
                try
                {
                    bytes = await webClient.DownloadDataTaskAsync(url);
                }
                catch (TaskCanceledException)
                {
                    Console.WriteLine("Image " + imageUrl + " loading task canceled! :(");
                    return;
                }
                catch (Exception)
                {
                    Console.WriteLine("Image " + imageUrl + " loading task failure! :(");
                    result(null);
                    return;
                }
                // downloaded

                //Save the Image using writeAsync
                file = new Java.IO.File(localPath);
                if (!file.Exists())
                {
                    FileStream fs = new FileStream(localPath, FileMode.OpenOrCreate);
                    await fs.WriteAsync(bytes, 0, bytes.Length);

                    Console.WriteLine("Loaded image local path :" + localPath);
                    fs.Close();
                }
                else
                {
                    Console.WriteLine("Image downloaded while we were waiting :" + localPath);
                }
                
            }
            
            // resizing to fit imageView
            BitmapFactory.Options options = new BitmapFactory.Options();
            options.InJustDecodeBounds = true;
            await BitmapFactory.DecodeFileAsync(localPath, options);

            //options.InSampleSize = options.OutWidth > options.OutHeight ? options.OutHeight / view.Height : options.OutWidth / view.Width;
            options.InJustDecodeBounds = false;

            Bitmap bitmap = await BitmapFactory.DecodeFileAsync(localPath, options);
            
            result(bitmap);
        }
	}
}