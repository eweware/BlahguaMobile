// This file has been autogenerated from a class added in the UI designer.

using System;
using CoreGraphics;

using BlahguaMobile.BlahguaCore;

using Foundation;
using MonoTouch.Dialog.Utilities;
using UIKit;
using CoreAnimation;

namespace BlahguaMobile.IOS
{
	public partial class BGRollViewCell : UICollectionViewCell, IImageUpdated
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
		private CAKeyFrameAnimation activityAnimation;
		private CAKeyFrameAnimation fadeInOutAnimation;

		public InboxBlah Blah 
		{
			get
			{
				return blah;
			}
		}

		public BGRollViewCell (IntPtr handle) : base (handle)
		{
			this.ContentMode = UIViewContentMode.Left;
			CGSize theSize = new CGSize(ContentView.Frame.Size.Width, ContentView.Frame.Size.Width);
			imageView = new UIImageView (new CGRect (new CGPoint (0, 0), theSize));
			imageView.ContentMode = UIViewContentMode.ScaleAspectFill;
			imageView.Hidden = false;
			ContentView.Add (imageView);

			textView = new UIView (new CGRect (new CGPoint (0, 0), ContentView.Frame.Size));
			textView.BackgroundColor = new UIColor (1, 1, 1, .9f);
			textView.ContentMode = UIViewContentMode.Left;
			label = new UILabel(new CGRect(new CGPoint(8, 7), new CGSize( ContentView.Frame.Size.Width - 24.0f, ContentView.Frame.Size.Height - 33.0f)));
			label.Lines = 0;
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

        public void UpdatedImage(Uri uri)
        {
            imageView.Image = ImageLoader.DefaultRequestImage(uri, this);

        }


		public void SetCellProperties(InboxBlah blah, string reusableId, CGSize size, NSIndexPath path)
		{
			this.blah = blah;
			this.path = path;
			if (!String.IsNullOrEmpty (blah.ImageURL)) {
				try {
					imageView.Frame = new CGRect (new CGPoint (0, 0), size);
					imageView.Image = null;
					imageView.Image = ImageLoader.DefaultRequestImage(new Uri(blah.ImageURL), this);
					imageView.Hidden = false;
				}
				catch (Exception exp) {
					imageView.Hidden = true;
				}
			}
			else
			{
				imageView.Hidden = true;
			}

            if (!String.IsNullOrEmpty(blah.I))
                BlahguaAPIObject.Current.AddImpression(blah.I);

			if(!String.IsNullOrEmpty(blah.T))
			{
				label.Frame = new CGRect(new CGPoint(8, 7), new CGSize(size.Width - 24.0f, size.Height - 33.0f));
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
				//label.SizeToFit ();
				textView.Hidden = false;
                textView.Frame = new CGRect(0, 0, size.Width, size.Height);
			} 
			else
				textView.Hidden = true;


			// add the various icons
			float iconSize = 16;
			float iconOffset = 2;


			// speech act
			if (BlahguaAPIObject.Current.CurrentChannel.SSA == false)
				speechActItem.Hidden = true;
			else {
				speechActItem.Hidden = false;
				string speechActImageStr = "";
				switch (blah.TypeName)
				{
				case "says":
					speechActImageStr = "say_icon";
					break;
				case "asks":
					speechActImageStr = "ask_icon";
					break;
				case "leaks":
					speechActImageStr = "leak_icon";
					break;
				case "polls":
					speechActImageStr = "poll_icon";
					break;
				case "predicts":
					speechActImageStr = "predict_icon";
					break;
				}
				speechActItem.Image = UIImage.FromBundle (speechActImageStr);
				speechActItem.Frame = new CGRect (size.Width - (iconSize + iconOffset), size.Height - (iconSize + iconOffset), iconSize, iconSize);
				speechActItem.ContentMode = UIViewContentMode.ScaleAspectFit;
			}



			// current user's own blah?
			if ((BlahguaAPIObject.Current.CurrentUser != null) &&
			    (BlahguaAPIObject.Current.CurrentUser._id == blah.A)) {
				ownBlahIcon.Frame = new CGRect (size.Width - ((iconSize + iconOffset) * 2), size.Height - (iconSize + iconOffset), iconSize, iconSize);
				ownBlahIcon.Hidden = false;
			} else
				ownBlahIcon.Hidden = true;

			// remaining icons
			float curLeft = iconOffset;

			// badged
			if (!String.IsNullOrEmpty (blah.B)) {
				badgeIcon.Frame = new CGRect (curLeft, size.Height - (iconSize + iconOffset), iconSize, iconSize);
				badgeIcon.Hidden = false;
				curLeft += iconSize + iconOffset;
			} else
				badgeIcon.Hidden = true;

			// hot
			if (blah.RR) {
				hotIcon.Frame = new CGRect (curLeft, size.Height - (iconSize + iconOffset), iconSize, iconSize);
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
				newIcon.Frame = new CGRect (curLeft, size.Height - (iconSize + iconOffset), iconSize, iconSize);
				newIcon.Hidden = false;
				curLeft += iconSize + iconOffset;
			} else
				newIcon.Hidden = true;

			SetUpAnimation ();
		}

		public void ShowActivity()
		{
			if (activityAnimation != null) {
				hotIcon.Layer.RemoveAllAnimations ();
				activityAnimation = null;
			}
			hotIcon.Hidden = false;
			hotIcon.Layer.Opacity = 1.0f;

			activityAnimation = (CAKeyFrameAnimation)CAKeyFrameAnimation.FromKeyPath ("opacity");
			activityAnimation.KeyTimes = new NSNumber[] { 
				NSNumber.FromFloat(0f),
				NSNumber.FromFloat(1f )
			};
			activityAnimation.Values = new NSNumber[] {
				NSNumber.FromFloat(1f),
				NSNumber.FromFloat(0f)
			};
			activityAnimation.RepeatCount = 1;
			activityAnimation.Duration = 2;
			activityAnimation.TimingFunction = CAMediaTimingFunction.FromName(CAMediaTimingFunction.EaseOut);
			activityAnimation.AnimationStopped += (object sender, CAAnimationStateEventArgs e) => 
			{
				this.hotIcon.Hidden = true;
			};


			hotIcon.Layer.AddAnimation (activityAnimation, "fadeOut");
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


		private int RandomInterval(nint index)
		{
			nint i = index % 5;
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
