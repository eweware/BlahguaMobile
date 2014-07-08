
using System;
using System.Drawing;

using BlahguaMobile.BlahguaCore;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace BlahguaMobile.IOS
{
	public partial class BGPollTableViewCell : UITableViewCell
	{
		public static readonly UINib Nib = UINib.FromName ("BGPollTableViewCell", NSBundle.MainBundle);
		public static readonly NSString Key = new NSString ("BGPollTableViewCell");

		public BGPollTableViewCell (IntPtr handle) : base (handle)
		{
		}

		public static BGPollTableViewCell Create ()
		{
			return (BGPollTableViewCell)Nib.Instantiate (null, null) [0];
		}

		public void SetUp(PollItem item, bool isVoted)
		{
			ContentView.BackgroundColor = UIColor.Clear;
			name.AttributedText = new NSAttributedString (
				item.G,
				UIFont.FromName(BGAppearanceConstants.FontName, 17),
				UIColor.Black
			);

			if(isVoted)
			{
				SelectionStyle = UITableViewCellSelectionStyle.None;
				UserInteractionEnabled = false;
				if(progressView.Progress <= 0)
				{
					noVotesLabel.Hidden = false;
					noVotesLabel.AttributedText = new NSAttributedString(
						"no votes", 
						UIFont.FromName(BGAppearanceConstants.FontName, 13), 
						UIColor.Gray
					);
				}
				else
				{
					noVotesLabel.Hidden = true;
				}
				percentage.AttributedText = new NSAttributedString(
					item.VotePercent, 
					UIFont.FromName(BGAppearanceConstants.FontName, 17), 
					UIColor.Black
				);
			}
			else
			{

				UserInteractionEnabled = true;
				noVotesLabel.Hidden = true;
				progressView.Progress = 1;
				percentage.AttributedText = new NSAttributedString (
					"?",
					UIFont.FromName(BGAppearanceConstants.FontName, 17),
					UIColor.Black
				);
			}
		}
	}
}

