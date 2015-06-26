using System;
using CoreGraphics;

using Foundation;
using UIKit;


namespace BlahguaMobile.IOS
{
	public class TutorialSwipeViewController:UIView
	{
		UIScrollView scrollView;
		UIPageControl pageControl;

		int totalPages;
		int curPage;

		private NSMutableArray curViews = null;

		private CGRect m_Frame;

		private TutorialSwipeViewDataSource m_dataSource;

		public TutorialSwipeViewController (CGRect frame) :base(frame)
		{
			m_Frame = frame;
	
			scrollView = new UIScrollView (m_Frame);
			//scrollView.Delegate = this;

			scrollView.ContentSize = new CGSize (m_Frame.Width * 3, m_Frame.Height);
			scrollView.ShowsHorizontalScrollIndicator = false;
			scrollView.ContentOffset = new CGPoint (m_Frame.Width, 0);
			scrollView.WeakDelegate = this;
			this.AddSubview (scrollView);

			CGRect rect = m_Frame;
			rect.Y = rect.Height - 30;
			rect.Height = 30;

			pageControl = new UIPageControl (rect);
			pageControl.UserInteractionEnabled = false;


			this.AddSubview (pageControl);
			curPage = 0;

		
		}

		public void SetDataSource(TutorialSwipeViewDataSource dataSource)
		{
			m_dataSource = dataSource;
			ReloadData ();
		}

		public void ReloadData()
		{
			totalPages = m_dataSource.NumberOfPages();
			if (totalPages == 0)
				return;
			pageControl.Pages = totalPages;

			LoadData ();
		}

		private void LoadData()
		{
			pageControl.CurrentPage = curPage;
			UIView[] subViews = scrollView.Subviews;
			if (subViews.Length != 0) {
				for (int i = 0; i < subViews.Length; i++) {
					UIView v = subViews [i];
					v.RemoveFromSuperview ();
				}
			}
				
			getDisplayImagesWithCurpage (curPage);

			for (nuint i = 0; i < 3; i++) {
				UIView v = curViews.GetItem<UIView>(i);
				v.Frame = new CGRect (v.Frame.X + v.Frame.Width * i, v.Frame.Y, v.Frame.Width, v.Frame.Height);
				scrollView.AddSubview (v);
			}

			scrollView.SetContentOffset (new CGPoint (scrollView.Frame.Size.Width, 0), false);

		}

		public void SetViewContent(UIView view, int atIndex)
		{
			if (atIndex == curPage) {
				curViews.ReplaceObject (1, view);
				for (nuint i = 0; i < 3; i++) {
					UIView v = curViews.GetItem<UIView> (i);
					v.Frame = new CGRect (v.Frame.X + v.Frame.Width*i, v.Frame.Y, v.Frame.Width, v.Frame.Height);
					scrollView.AddSubview (v);
				}
			}
		}

		private void getDisplayImagesWithCurpage(int page)
		{
			int prev = validPageValue(curPage - 1);
			int last = validPageValue (curPage + 1);

			if (curViews == null)
				curViews = new NSMutableArray ();

			curViews.RemoveAllObjects ();

			curViews.Add (m_dataSource.PageAtIndex (prev));
			curViews.Add (m_dataSource.PageAtIndex (page));
			curViews.Add (m_dataSource.PageAtIndex (last));
		}

		private int validPageValue(int value)
		{
			if (value == -1)
				value = totalPages - 1;
			if (value == totalPages)
				value = 0;
			return value;
		}
		[Export("scrollViewDidScroll:")]
		public void Scrolled(UIScrollView aScroll)
		{
			int x =(int) scrollView.ContentOffset.X;

			if(x >= (m_Frame.Width * 2)) {
				curPage = validPageValue(curPage+1);
				LoadData();
			}

			if(x < 0) {
				curPage = validPageValue(curPage-1);
				LoadData();
			}
		}

		[Export ("scrollViewDidEndDecelerating:")]
		public void DecelerationEnded (UIScrollView aScroll)
		{
			scrollView.SetContentOffset(new CGPoint(scrollView.Frame.Width, 0), true);
		}

	}

	public abstract class TutorialSwipeViewDataSource {

	 	 public abstract int NumberOfPages();

		 public abstract UIView PageAtIndex(int index);

	}
}

