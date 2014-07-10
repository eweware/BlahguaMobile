// This file has been autogenerated from a class added in the UI designer.

using System;
using System.Drawing;

using BlahguaMobile.BlahguaCore;

using MonoTouch.Foundation;
using MonoTouch.Dialog.Utilities;
using MonoTouch.UIKit;
using MonoTouch.CoreAnimation;

namespace BlahguaMobile.IOS
{
	public partial class BGRollViewCell : UICollectionViewCell
	{
		private InboxBlah blah;
		private UIImageView imageView;
		private UILabel label;
		public  UIView textView;
		private NSIndexPath path;
		private UIImageView	speechActItem;
		private UIImageView hotIcon;
		private UIImageView ownBlahIcon;
		private UIImageView newIcon;
		private UIImageView badgeIcon;

		private CAKeyFrameAnimation fadeInOutAnimation;
		private CABasicAnimation fadeOutAnimation;

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

			speechActItem = new UIImageView ();
			ContentView.Add (speechActItem);

			badgeIcon = new UIImageView (UIImage.FromBundle ("badge_icon.png"));
			badgeIcon.ContentMode = UIViewContentMode.ScaleAspectFit;
			badgeIcon.Hidden = true;
			ContentView.Add (badgeIcon);

			ownBlahIcon = new UIImageView (UIImage.FromBundle ("pen_bw.png"));
			ownBlahIcon.ContentMode = UIViewContentMode.ScaleAspectFit;
			ownBlahIcon.Hidden = true;
			ContentView.Add (ownBlahIcon);

			hotIcon = new UIImageView (UIImage.FromBundle ("ico_hot.png"));
			hotIcon.ContentMode = UIViewContentMode.ScaleAspectFit;
			hotIcon.Hidden = true;
			ContentView.Add (hotIcon);

			newIcon = new UIImageView (UIImage.FromBundle ("ico_new.png"));
			newIcon.ContentMode = UIViewContentMode.ScaleAspectFit;
			newIcon.Hidden = true;
			ContentView.Add (newIcon);

		}

		public void SetCellProperties(InboxBlah blah, string reusableId, SizeF size, NSIndexPath path)
		{
			this.blah = blah;
			this.path = path;
			if (!String.IsNullOrEmpty (blah.ImageURL)) {
				imageView.Frame = new RectangleF (new PointF (0, 0), size);
				imageView.Image = ImageLoader.DefaultRequestImage (new Uri(blah.ImageURL), new ImageUpdateDelegate(imageView));
				imageView.Hidden = false;
			}
			else
			{
				imageView.Hidden = true;
			}

			if(!String.IsNullOrEmpty(blah.T))
			{
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
			} 
			else
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
			speechActItem.Image = UIImage.FromBundle (speechActImageStr);
			speechActItem.Frame = new RectangleF (size.Width - (iconSize + iconOffset), size.Height - (iconSize + iconOffset), iconSize, iconSize);
			speechActItem.ContentMode = UIViewContentMode.ScaleAspectFit;


			// current user's own blah?
			if ((BlahguaAPIObject.Current.CurrentUser != null) &&
			    (BlahguaAPIObject.Current.CurrentUser._id == blah.A)) {
				ownBlahIcon.Frame = new RectangleF (size.Width - ((iconSize + iconOffset) * 2), size.Height - (iconSize + iconOffset), iconSize, iconSize);
				ownBlahIcon.Hidden = false;
			} else
				ownBlahIcon.Hidden = true;

			// remaining icons
			float curLeft = iconOffset;

			// badged
			if (!String.IsNullOrEmpty (blah.B)) {
				badgeIcon.Frame = new RectangleF (curLeft, size.Height - (iconSize + iconOffset), iconSize, iconSize);
				badgeIcon.Hidden = false;
				curLeft += iconSize + iconOffset;
			} else
				badgeIcon.Hidden = true;

			// hot
			if (blah.RR) {
				hotIcon.Frame = new RectangleF (curLeft, size.Height - (iconSize + iconOffset), iconSize, iconSize);
				hotIcon.Hidden = false;
				curLeft += iconSize + iconOffset;
			} else
				hotIcon.Hidden = true;

			// new
			double currentUtc = DateTime.Now.ToUniversalTime().Subtract(
				new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
			).TotalMilliseconds;
			double	newMilliseconds = 24 * 60 * 60 * 1000;


			if (currentUtc - blah.c < newMilliseconds) {
				newIcon.Frame = new RectangleF (curLeft, size.Height - (iconSize + iconOffset), iconSize, iconSize);
				newIcon.Hidden = false;
				curLeft += iconSize + iconOffset;
			} else
				newIcon.Hidden = true;

			SetUpAnimation ();
		}

		public void SetUpAnimation()
		{
			if (fadeInOutAnimation != null)
			{
				if (String.IsNullOrEmpty(blah.ImageURL))
				{
					textView.Layer.RemoveAllAnimations ();
					fadeInOutAnimation = null;
					return;
				}
			} 
			// if we have an image AND a string, we do the animation
			if ((!String.IsNullOrEmpty(blah.T)) && (!String.IsNullOrEmpty(blah.ImageURL)))
			{
				InvokeAnimation ();
			}
		}

		private void InvokeAnimation()
		{
			fadeInOutAnimation = (CAKeyFrameAnimation)CAKeyFrameAnimation.FromKeyPath ("opacity");
			float animationDuration = (2 + RandomInterval (path.Item)) * 2;
			fadeInOutAnimation.KeyTimes = new NSNumber[] { 
				NSNumber.FromFloat(0f / animationDuration),
				NSNumber.FromFloat(2f / animationDuration),
				NSNumber.FromFloat((RandomInterval(path.Item) + 2f) / animationDuration),
				NSNumber.FromFloat((RandomInterval(path.Item) + 4f) / animationDuration),
				NSNumber.FromFloat(animationDuration / animationDuration),
			};
			fadeInOutAnimation.Values = new NSNumber[] {
				NSNumber.FromFloat(0.9f),
				NSNumber.FromFloat(0f),
				NSNumber.FromFloat(0f),
				NSNumber.FromFloat(0.9f),
				NSNumber.FromFloat(0.9f),
			};
			fadeInOutAnimation.RepeatCount = 1e10f;
			fadeInOutAnimation.Duration = animationDuration;
			fadeInOutAnimation.TimingFunction = CAMediaTimingFunction.FromName(CAMediaTimingFunction.EaseInEaseOut);


			textView.Layer.AddAnimation (fadeInOutAnimation, "fadeInOut");
		}


		private int RandomInterval(int index)
		{
			int i = index % 5;
			switch(i)
			{
			case 0:
				return 2;
			case 1: 
				return 3;
			case 2: 
				return 5;
			case 3: 
				return 7;
			default:
			case 4: 
				return 11;
			}
		}
	}
}
