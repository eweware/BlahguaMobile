// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace BlahguaMobile.IOS
{
	[Register ("BGBlahHistoryCell")]
	partial class BGBlahHistoryCell
	{
		[Outlet]
		UIKit.UILabel BlahCommentsLabel { get; set; }

		[Outlet]
		UIKit.UILabel BlahConversionLabel { get; set; }

		[Outlet]
		UIKit.UILabel BlahDownVotesLabel { get; set; }

		[Outlet]
		UIKit.UIImageView BlahImageView { get; set; }

		[Outlet]
		UIKit.UILabel BlahTimeLabel { get; set; }

		[Outlet]
		UIKit.UILabel BlahTitleLabel { get; set; }

		[Outlet]
		UIKit.UIImageView BlahTypeImage { get; set; }

		[Outlet]
		UIKit.UILabel BlahUpVotesLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (BlahCommentsLabel != null) {
				BlahCommentsLabel.Dispose ();
				BlahCommentsLabel = null;
			}

			if (BlahConversionLabel != null) {
				BlahConversionLabel.Dispose ();
				BlahConversionLabel = null;
			}

			if (BlahDownVotesLabel != null) {
				BlahDownVotesLabel.Dispose ();
				BlahDownVotesLabel = null;
			}

			if (BlahImageView != null) {
				BlahImageView.Dispose ();
				BlahImageView = null;
			}

			if (BlahTimeLabel != null) {
				BlahTimeLabel.Dispose ();
				BlahTimeLabel = null;
			}

			if (BlahTitleLabel != null) {
				BlahTitleLabel.Dispose ();
				BlahTitleLabel = null;
			}

			if (BlahUpVotesLabel != null) {
				BlahUpVotesLabel.Dispose ();
				BlahUpVotesLabel = null;
			}

			if (BlahTypeImage != null) {
				BlahTypeImage.Dispose ();
				BlahTypeImage = null;
			}
		}
	}
}
