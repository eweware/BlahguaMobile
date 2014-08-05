using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;

namespace BlahguaMobile.IOS
{
	public class FullScreenView :UIViewController 
	{
		private UIScrollView scrollView;
		private UIImageView imageView;
		private UIImage m_image; 
		public FullScreenView (UIImage image)
		{

			m_image = image;
		}
		public override void LoadView()
		{
			base.LoadView ();
			scrollView = new UIScrollView(new RectangleF(0, 44, View.Bounds.Width,View.Bounds.Height - 44));
			scrollView.BackgroundColor = UIColor.Black;

			scrollView.ShowsHorizontalScrollIndicator = false;
			scrollView.ShowsVerticalScrollIndicator = false;

			scrollView.MaximumZoomScale = 2.0f;

			float newHeight = m_image.Size.Height / m_image.Size.Width * View.Bounds.Width;

			RectangleF rect= new RectangleF(0, 0, View.Bounds.Width, newHeight);
			//RectangleF rect= new RectangleF(0, 44, m_image.Size.Width, m_image.Size.Height );

			imageView = new UIImageView (rect);
			imageView.ContentMode = UIViewContentMode.ScaleAspectFit; //UIViewContentModeScaleAspectFit;

			imageView.Image = m_image;
			scrollView.ContentSize = imageView.Frame.Size;
			scrollView.MinimumZoomScale = scrollView.Frame.Size.Width / imageView.Frame.Size.Width;
			scrollView.SetZoomScale (scrollView.MinimumZoomScale, false);
			scrollView.AddSubview (imageView);
			this.View.AddSubview (scrollView);
		}

		public override void ViewDidLoad()
		{
			scrollView.ViewForZoomingInScrollView += (UIScrollView sv) => { return imageView; };
		}

	}
}

