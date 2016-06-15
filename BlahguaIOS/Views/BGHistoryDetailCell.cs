// This file has been autogenerated from a class added in the UI designer.

using System;
using CoreGraphics;

using BlahguaMobile.BlahguaCore;
using SDWebImage;
using Foundation;
using UIKit;
using MonoTouch.Dialog.Utilities;

namespace BlahguaMobile.IOS
{
	public partial class BGHistoryDetailCell : UITableViewCell
	{
		private const float baseXStart = 20f;
		private const float space = 8f;

		private CGSize baseSizeForFitting = new CGSize (240, 21);

		private nfloat yCoordStart;
		private nfloat labelXCoordStart;

		public UIImageView commentImageView;

		public BGHistoryDetailCell (IntPtr handle) : base (handle)
		{
		}

		public void SetUp(Blah blah)
		{
			SetUp (blah.ImageURL, blah.T, blah.P.ToString () + "/" + blah.D.ToString (),blah.ChannelName,blah.ElapsedTimeString);
		}

		public void SetUp(Comment comment)
		{
	
			SetUp (comment.ImageURL, comment.T, comment.UpVoteCount.ToString () + "/" + comment.DownVoteCount.ToString (),comment.AuthorName,comment.ElapsedTimeString);
		}

		private void SetUp(string imageUrl, string textStr, string upAndDownVotesText,string userNameString,string timeString)
		{
            nfloat width = Frame.Width, height = Frame.Height;
			yCoordStart = space;
			labelXCoordStart = baseXStart;
			ContentView.AddSubview(commentImageView);
			if(!String.IsNullOrEmpty(imageUrl))
			{
				commentImageView = new UIImageView ();
				commentImageView.SetImage(new NSUrl(imageUrl), (image, error, cacheType, loadedUrl) =>
				{
					// todo:  don't we need to resize this??
				});

				commentImageView.Frame = new CGRect (0, yCoordStart, width, 161f);
				yCoordStart += commentImageView.Frame.Height + space;
			}

			this.text.RemoveFromSuperview ();
			if(!String.IsNullOrEmpty(textStr))
			{
				this.text.AttributedText = new NSAttributedString (textStr, UIFont.FromName (BGAppearanceConstants.FontName, 14), UIColor.Black);

				var newSize = this.text.SizeThatFits (new CGSize (width - baseXStart * 2, height));
				ContentView.AddSubview (this.text);
				this.text.Frame = new CGRect (baseXStart, yCoordStart, width - baseXStart * 2, newSize.Height);
				yCoordStart += this.text.Frame.Height + space;
			}

			upAndDownVotes.AttributedText = new NSAttributedString (
				upAndDownVotesText,
				UIFont.FromName (BGAppearanceConstants.BoldFontName, 14), 
				UIColor.Black
			);

			SetLabelSize (upAndDownVotes);

			ContentView.Frame = new CGRect (0, 0, width, yCoordStart + upAndDownVotes.Frame.Height + space);
		}

		private void SetLabelSize(UILabel label)
		{
			label.RemoveFromSuperview ();
			var newSize = label.SizeThatFits(baseSizeForFitting);

			label.Frame = new CGRect (labelXCoordStart, yCoordStart,newSize.Width, newSize.Height);
			ContentView.AddSubview (label);
			labelXCoordStart += newSize.Width + space;
		}
	}
}
