// This file has been autogenerated from a class added in the UI designer.

using System;
using CoreGraphics;

using BlahguaMobile.BlahguaCore;

using Foundation;
using UIKit;

namespace BlahguaMobile.IOS
{
	public partial class BGNewPostPollItemCell : UITableViewCell
	{
		private PollItem pollItem;

		public BGNewPostPollItemCell (IntPtr handle) : base (handle)
		{
		}

		public void SetUpWithPollItem(PollItem pollItem)
		{
			this.pollItem = pollItem;
			SetUp (pollItem.G ?? String.Empty, UIFont.FromName (BGAppearanceConstants.MediumFontName, 14));
		}

		public void SetUp()
		{
            SetUp ("+ Add more options", UIFont.FromName (BGAppearanceConstants.MediumFontName, 14));
			pollItemText.Enabled = false;
		}

		public UITextField PollItemTextField
		{
			get{
				return pollItemText;
			}
			set{
				pollItemText = value;
			}
		}

		private void SetUp(string text, UIFont fontForText)
		{
			pollItemText.AttributedPlaceholder = new NSAttributedString (
				"Type poll option", 
				UIFont.FromName(BGAppearanceConstants.MediumItalicFontName, 14), 
				UIColor.DarkGray
			);
			pollItemText.Enabled = true;

			pollItemText.AttributedText = new NSAttributedString (
				text,
				fontForText,
				UIColor.Black
			);

			pollItemText.EditingDidEndOnExit += (object sender, EventArgs e) => {
				pollItem.G = pollItemText.Text;
			};

			pollItemText.EditingDidEnd += (object sender, EventArgs e) => {
				pollItem.G = pollItemText.Text;
			};

			pollItemText.ReturnKeyType = UIReturnKeyType.Done;
			//pollItemText.ShouldReturn = delegate {
			//	return true;
			//};
		}
	}
}
