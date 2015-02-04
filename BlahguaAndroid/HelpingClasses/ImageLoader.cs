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

        public static string GetLocalFilepath(string url)
        {
            string documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            string localFilename = url.Replace(":", "_").Replace("\\", "_").Replace("/", "_");
            string localPath = System.IO.Path.Combine(documentsPath, localFilename);

            return localPath;
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
                catch (Exception e)
                {
                    Console.WriteLine("Image " + imageUrl + " loading task failure! :(");
                    return;
                }
                // downloaded

                //Save the Image using writeAsync
                FileStream fs = new FileStream(localPath, FileMode.OpenOrCreate);
                await fs.WriteAsync(bytes, 0, bytes.Length);

                Console.WriteLine("Loaded image local path :" + localPath);
                fs.Close();
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