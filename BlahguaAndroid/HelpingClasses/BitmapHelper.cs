﻿using System;

using Android.Graphics;
using Android.Media;


namespace BlahguaMobile.AndroidClient.HelpingClasses
{
	public static class BitmapHelper
	{
		public static Bitmap LoadAndResizeBitmap(this string fileName, int maxSize)
		{
			// First we get the the dimensions of the file on disk
			BitmapFactory.Options options = new BitmapFactory.Options { InJustDecodeBounds = true };
			BitmapFactory.DecodeFile(fileName, options);

			// Next we calculate the ratio that we need to resize the image by
			// in order to fit the requested dimensions.
			int outHeight = options.OutHeight;
			int outWidth = options.OutWidth;
			int inSampleSize = 1;

			if (outHeight > outWidth) {
				inSampleSize = outHeight / maxSize;
			} else {
				inSampleSize = outWidth / maxSize;
			}

			// Now we will load the image and have BitmapFactory resize it for us.
			options.InSampleSize = inSampleSize;
			options.InJustDecodeBounds = false;

			Bitmap resizedBitmap = BitmapFactory.DecodeFile(fileName, options);

			// check for orientation issues...
			ExifInterface exif = new ExifInterface (fileName);
			Orientation rotation = (Orientation)exif.GetAttributeInt (ExifInterface.TagOrientation, (int)Orientation.Normal);

			int angle = 0;

			if (rotation == Orientation.Rotate90) {// ExifInterface.ORIENTATION_ROTATE_90) {
				angle = 90;
			} 
			else if (rotation == Orientation.Rotate180) {//ExifInterface.ORIENTATION_ROTATE_180) {
				angle = 180;
			} 
			else if (rotation == Orientation.Rotate270) {// ExifInterface.ORIENTATION_ROTATE_270) {
				angle = 270;
			}
				
			if (angle != 0) {
				Matrix mat = new Matrix ();
				mat.PostRotate (angle);
				        
				Bitmap correctBmp = Bitmap.CreateBitmap (resizedBitmap, 0, 0, resizedBitmap.Width, resizedBitmap.Height, mat, true);
				return correctBmp;
			} else
				return resizedBitmap;
		}
	}
}

