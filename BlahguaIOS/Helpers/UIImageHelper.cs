using System;

using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace BlahguaMobile.IOS
{
	public static class UIImageHelper
	{
		public static UIImage ImageFromUrl(string uri)
		{
			using(var url = new NSUrl(uri))
			{
				using(var data = NSData.FromUrl(url))
				{
					return UIImage.LoadFromData(data);
				}
			}
		}
	}
}

