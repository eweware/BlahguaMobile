using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.CodeDom.Compiler;

namespace BlahguaMobile.IOS
{
	partial class BGProfileViewController : UIViewController
	{
		public BGProfileViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			View.BackgroundColor = UIColor.FromPatternImage (UIImage.FromFile ("grayBack.png"));

			NavigationItem.LeftBarButtonItem = new UIBarButtonItem ("Cancel", UIBarButtonItemStyle.Plain, CancelHandler);
			NavigationItem.RightBarButtonItem = new UIBarButtonItem ("Done", UIBarButtonItemStyle.Plain, DoneHandler);

			profileView.BackgroundColor = UIColor.FromPatternImage ("profileInfoBack.png");


		}

		#region Methods

		private void DoneHandler(object sender, EventArgs args)
		{
		}

		private void CancelHandler(object sender, EventArgs args)
		{

		}

		#endregion
	}
}
