// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace BlahguaMobile.IOS
{
	[Register ("BGPollTableViewCell")]
	partial class BGPollTableViewCell
	{
		[Outlet]
		MonoTouch.UIKit.UILabel name { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel noVotesLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel percentage { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIProgressView progressView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (noVotesLabel != null) {
				noVotesLabel.Dispose ();
				noVotesLabel = null;
			}

			if (name != null) {
				name.Dispose ();
				name = null;
			}

			if (percentage != null) {
				percentage.Dispose ();
				percentage = null;
			}

			if (progressView != null) {
				progressView.Dispose ();
				progressView = null;
			}
		}
	}
}
