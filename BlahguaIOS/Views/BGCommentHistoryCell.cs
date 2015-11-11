
using System;
using CoreGraphics;

using Foundation;
using UIKit;
using BlahguaMobile.BlahguaCore;


namespace BlahguaMobile.IOS
{
    public partial class BGCommentHistoryCell : SWTableViewCell.SWTableViewCell
    {
        public static readonly UINib Nib = UINib.FromName("BGCommentHistoryCell", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("BGCommentHistoryCell");

        public BGCommentHistoryCell(IntPtr handle)
            : base(handle)
        {
        }

        public BGCommentHistoryCell(UITableViewCellStyle style, string reuseIdentifier) 
            : base(style, reuseIdentifier)
        {

        }

        public static BGCommentHistoryCell Create()
        {
            BGCommentHistoryCell newCell = (BGCommentHistoryCell)Nib.Instantiate(null, null)[0];
            newCell.InitCell();

            return newCell;
        }

        public override Foundation.NSString ReuseIdentifier
        {
            get
            {
                return (Foundation.NSString)"C";
            }
        }

        public void InitCell()
        {
            // here we put anything that doesn't work in the xib file...

        }

        public void SetupComment(Comment userComment)
        {
            string text = userComment.T;
            string timeString = userComment.ElapsedTimeString;
            string upVotesText = userComment.UpVoteCount.ToString();
            string downVotesText = userComment.DownVoteCount.ToString();


            if (!String.IsNullOrEmpty(text))
                this.CommentText.AttributedText = new NSAttributedString(text, UIFont.FromName(BGAppearanceConstants.FontName, 14), UIColor.Black);
            else
                this.CommentText.AttributedText = new NSAttributedString("comment has no text", UIFont.FromName(BGAppearanceConstants.MediumItalicFontName, 14), UIColor.Gray);
                

            this.CommentElapsedTimeLabel.AttributedText = new NSAttributedString (timeString, UIFont.FromName (BGAppearanceConstants.MediumItalicFontName, 10), UIColor.Black);
			this.CommentUpVotesLabel.AttributedText = new NSAttributedString (upVotesText, UIFont.FromName (BGAppearanceConstants.BoldFontName, 14), UIColor.Black);
            this.CommentDownVotesLabel.AttributedText = new NSAttributedString (downVotesText, UIFont.FromName (BGAppearanceConstants.BoldFontName, 14), UIColor.Black); 

        }





    }
}

