// This file has been autogenerated from a class added in the UI designer.

using System;
using System.Drawing;

using BlahguaMobile.BlahguaCore;

using MonoTouch.Foundation;
using MonoTouch.Dialog.Utilities;
using MonoTouch.UIKit;

namespace BlahguaMobile.IOS
{
	public partial class BGRollViewCell : UICollectionViewCell, IImageUpdated
	{
		private InboxBlah blah;
		private UIImageView imageView;
		private UILabel label;
		private UIView textView;
		private NSIndexPath path;

		public InboxBlah Blah 
		{
			get
			{
				return blah;
			}
		}

		public BGRollViewCell (IntPtr handle) : base (handle)
		{
			this.ContentMode = UIViewContentMode.TopLeft;
			SizeF theSize = new SizeF(ContentView.Frame.Size.Width, ContentView.Frame.Size.Width);
			imageView = new UIImageView (new RectangleF (new PointF (0, 0), theSize));
			imageView.ContentMode = UIViewContentMode.ScaleAspectFill;
			imageView.Hidden = false;
			ContentView.Add (imageView);

			textView = new UIView (new RectangleF (new PointF (0, 0), ContentView.Frame.Size));
			textView.BackgroundColor = new UIColor (1, 1, 1, .9f);

			label = new UILabel(new RectangleF(new PointF(8, 7), new SizeF( ContentView.Frame.Size.Width - 24.0f, ContentView.Frame.Size.Height - 33.0f)));
			label.Lines = 6;
			//	UIFont font = reusableId == BGBlahCellSizesConstants.TinyReusableId ? 
			//		UIFont.FromName (BGAppearanceConstants.FontName, 7.0f) : UIFont.FromName (BGAppearanceConstants.FontName, 14.0f);
			//	label.AttributedText = new NSAttributedString (inboxBlah.T, font, UIColor.Black);
			label.Hidden = false;
			textView.Add (label);

			ContentView.Add (textView);
		}

		public void SetCellProperties(InboxBlah blah, string reusableId, SizeF size, NSIndexPath path)
		{
			this.blah = blah;
			if (!String.IsNullOrEmpty (blah.ImageURL)) {
				imageView.Frame = new RectangleF (new PointF (0, 0), size);
				imageView.Image = ImageLoader.DefaultRequestImage (new Uri (blah.ImageURL), this);
				imageView.Hidden = false;
			} else 
				imageView.Hidden = true;


			if (!String.IsNullOrEmpty (blah.T)) {
				label.Frame = new RectangleF(new PointF(8, 7), new SizeF(size.Width - 24.0f, size.Height - 33.0f));
				label.ContentMode = UIViewContentMode.TopLeft;
				label.TextAlignment = UITextAlignment.Left;
				string fontName = BGAppearanceConstants.FontName;
				float fontSize = 10;

				switch (reusableId)
				{
					case BGBlahCellSizesConstants.TinyReusableId:
						fontSize = 14f;
						break;
					case BGBlahCellSizesConstants.SmallReusableId:
						fontSize = 18f;
						break;
					case BGBlahCellSizesConstants.MediumReusableId:
						fontSize = 24;
						break;
					case BGBlahCellSizesConstants.LargeReusableId:
						fontSize = 32f;
						break;
				}



				UIFont font = UIFont.FromName(fontName, fontSize);
				label.AttributedText = new NSAttributedString (blah.T, font, UIColor.Black);
				label.SizeToFit ();
				textView.Hidden = false;
			} else
				textView.Hidden = true;


			// add the various icons
			float iconSize = 16;
			float iconOffset = 2;


			// speech act
			string speechActImageStr = "";
			switch (blah.TypeName)
			{
			case "says":
				speechActImageStr = "say_icon.png";
				break;
			case "asks":
				speechActImageStr = "ask_icon.png";
				break;
			case "leaks":
				speechActImageStr = "leak_icon.png";
				break;
			case "polls":
				speechActImageStr = "poll_icon.png";
				break;
			case "predicts":
				speechActImageStr = "predict_icon.png";
				break;
			}
			UIImageView	speechActItem = new UIImageView (UIImage.FromBundle (speechActImageStr));
			speechActItem.Frame = new RectangleF (size.Width - (iconSize + iconOffset), size.Height - (iconSize + iconOffset), iconSize, iconSize);
			speechActItem.ContentMode = UIViewContentMode.ScaleAspectFit;
			ContentView.Add (speechActItem);

			// current user's own blah?
			if ((BlahguaAPIObject.Current.CurrentUser != null) &&
				(BlahguaAPIObject.Current.CurrentUser._id == blah.A)) {
				UIImageView	ownBlahIcon = new UIImageView (UIImage.FromBundle ("pen_bw.png"));
				ownBlahIcon.Frame = new RectangleF (size.Width - ((iconSize + iconOffset) * 2), size.Height - (iconSize + iconOffset), iconSize, iconSize);
				ownBlahIcon.ContentMode = UIViewContentMode.ScaleAspectFit;
				ContentView.Add (ownBlahIcon);
			}

			// remaining icons
			float curLeft = iconOffset;

			// badged
			if (!String.IsNullOrEmpty (blah.B)) {
				UIImageView	badgeIcon = new UIImageView (UIImage.FromBundle ("badge_icon.png"));
				badgeIcon.Frame = new RectangleF (curLeft, size.Height - (iconSize + iconOffset), iconSize, iconSize);
				badgeIcon.ContentMode = UIViewContentMode.ScaleAspectFit;
				ContentView.Add (badgeIcon);
				curLeft += iconSize + iconOffset;
			}

			// hot
			if (blah.RR) {
				UIImageView	hotIcon = new UIImageView (UIImage.FromBundle ("ico_hot.png"));
				hotIcon.Frame = new RectangleF (curLeft, size.Height - (iconSize + iconOffset), iconSize, iconSize);
				hotIcon.ContentMode = UIViewContentMode.ScaleAspectFit;
				ContentView.Add (hotIcon);
				curLeft += iconSize + iconOffset;
			}

			// new
			double currentUtc = DateTime.Now.ToUniversalTime().Subtract(
				new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
			).TotalMilliseconds;
			double	newMilliseconds = 24 * 60 * 60 * 1000;


			if (currentUtc - blah.c < newMilliseconds) {
				UIImageView	newIcon = new UIImageView (UIImage.FromBundle ("ico_new.png"));
				newIcon.Frame = new RectangleF (curLeft, size.Height - (iconSize + iconOffset), iconSize, iconSize);
				newIcon.ContentMode = UIViewContentMode.ScaleAspectFit;
				ContentView.Add (newIcon);
				curLeft += iconSize + iconOffset;
			}



		}

		#region IImageUpdated implementation

		public void UpdatedImage (Uri uri)
		{
			if(blah.ImageURL != null) 
			{
				var image = ImageLoader.DefaultRequestImage (new Uri (blah.ImageURL), this);
				imageView.Image = image == null ? new UIImage() : image;
			}
			LayoutSubviews ();
		}

		#endregion
	}
}
