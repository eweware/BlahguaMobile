
using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using BlahguaMobile.BlahguaCore;


namespace BlahguaMobile.IOS
{
    public partial class BGBlahHistoryCell : SWTableViewCell.SWTableViewCell
    {
        public static readonly UINib Nib = UINib.FromName("BGBlahHistoryCell", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("BGBlahHistoryCell");

       
        public BGBlahHistoryCell(IntPtr handle)
            : base(handle)
        {

        }

        public BGBlahHistoryCell(UITableViewCellStyle style, string reuseIdentifier) 
            : base(style, reuseIdentifier)
        {
           
        }

        public static BGBlahHistoryCell Create()
        {
            BGBlahHistoryCell newCell = (BGBlahHistoryCell)Nib.Instantiate(null, null)[0];
            newCell.InitCell();

            return newCell;
        }

        public override string ReuseIdentifier
        {
            get
            {
                return "B";
            }
        }

        public void InitCell()
        {
            // here we put anything that doesn't work in the xib file...

        }

        public void SetupBlah(Blah userBlah)
        {
            string text = userBlah.T;

            string upVotesText = userBlah.P.ToString();
            string downVotesText = userBlah.D.ToString();
            string conversionString = userBlah.ConversionString;
            string timeString = userBlah.ElapsedTimeString;

            if (!String.IsNullOrEmpty(text))
                this.BlahTitleLabel.AttributedText = new NSAttributedString(text, UIFont.FromName(BGAppearanceConstants.FontName, 14), UIColor.Black);
            else
                this.BlahTitleLabel.AttributedText = new NSAttributedString("untitled post", UIFont.FromName(BGAppearanceConstants.MediumItalicFontName, 14), UIColor.Gray);
                

            string typeBundle = "";

            switch (userBlah.TypeName)
            {
                case "says":
                    typeBundle = "icon_speechact_predict";
                    break;
                case "predicts":
                    typeBundle = "icon_speechact_say";
                    break;
                case "polls":
                    typeBundle = "icon_speechact_poll";
                    break;
                case "asks":
                    typeBundle = "icon_speechact_ask";
                    break;
                case "leaks":
                    typeBundle = "icon_speechact_leakß";
                    break;
            }
            this.BlahTypeImage.Image = UIImage.FromBundle(typeBundle);

            this.BlahUpVotesLabel.AttributedText = new NSAttributedString (upVotesText, UIFont.FromName (BGAppearanceConstants.BoldFontName, 14), UIColor.Black);
            this.BlahDownVotesLabel.AttributedText = new NSAttributedString (downVotesText, UIFont.FromName (BGAppearanceConstants.BoldFontName, 14), UIColor.Black); 
            this.BlahConversionLabel.AttributedText = new NSAttributedString (conversionString, UIFont.FromName (BGAppearanceConstants.MediumFontName, 14), UIColor.Black);
            this.BlahCommentsLabel.AttributedText = new NSAttributedString (userBlah.C.ToString (), UIFont.FromName (BGAppearanceConstants.MediumFontName, 14), UIColor.Black);
            this.BlahTimeLabel.AttributedText = new NSAttributedString (timeString, UIFont.FromName (BGAppearanceConstants.MediumItalicFontName, 10), UIColor.Black);
        }

    }
}

