
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
				UIFont.FromName(BGAppearanceConstants.FontName, 16),
				UIColor.Black
			);

			if(isVoted)
			{
				SelectionStyle = UITableViewCellSelectionStyle.None;
				UserInteractionEnabled = false;
                float percent;

                if (item.MaxVotes > 0)
                    percent = (float)item.Votes / (float)item.MaxVotes;
                else
                    percent = 0;

                progressView.Progress = percent;
              
                if (item.IsUserVote)
                {
                    progressView.ProgressTintColor = new UIColor(.38f, .75f, .64f, 1f);
                }
                else
                {
                    progressView.ProgressTintColor = new UIColor(12f, .73f, .82f, 1f);
                }

				percentage.AttributedText = new NSAttributedString(
					item.VotePercent, 
					UIFont.FromName(BGAppearanceConstants.FontName, 14), 
					UIColor.Black);
			}
			else
			{
				UserInteractionEnabled = true;
				progressView.Progress = .5f;
				percentage.AttributedText = new NSAttributedString (
					"??",
					UIFont.FromName(BGAppearanceConstants.FontName, 16),
					UIColor.Black
				);
			}
		}
	}
}

