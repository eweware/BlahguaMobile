using System;

using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;

namespace BlahguaMobile.IOS
{
	public class TutorialSwipeViewController:UIView
	{
		UIScrollView scrollView;
		UIPageControl pageControl;

		int totalPages;
		int curPage;

		private NSMutableArray _curViews;

		private RectangleF m_Frame;

		public TutorialSwipeViewController (RectangleF frame) :base(frame)
		{
			m_Frame = frame;
	
			scrollView = new UIScrollView (m_Frame);
			//scrollView.Delegate = this;

			scrollView.ContentSize = new SizeF (m_Frame.Width * 3, m_Frame.Height);
			scrollView.ShowsHorizontalScrollIndicator = false;
			scrollView.ContentOffset = new PointF (m_Frame.Width, 0);

			this.AddSubview (scrollView);

			RectangleF rect = m_Frame;
			rect.Y = rect.Height - 30;
			rect.Height = 30;

			pageControl = new UIPageControl (rect);
			pageControl.UserInteractionEnabled = false;

			this.AddSubview (pageControl);
			curPage = 0;
		}

	}
}

