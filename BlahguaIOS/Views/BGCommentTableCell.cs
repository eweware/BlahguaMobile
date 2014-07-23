// This file has been autogenerated from a class added in the UI designer.

using System;
using System.Drawing;

using BlahguaMobile.BlahguaCore;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.Dialog.Utilities;
using MonoTouch.UIKit;

namespace BlahguaMobile.IOS
{
    public partial class BGCommentTableCell : UITableViewCell
    {
        private UIPanGestureRecognizer panRecognizer;
		private UITapGestureRecognizer tapRecognizer;

        private PointF panStartPoint;
        private float startingLayoutRight = 0;
        private Comment comment;

        public BGCommentTableCell(IntPtr handle)
            : base(handle)
        {
        }

        public void SetUp(Comment comment)
        {
            this.comment = comment;
            panRecognizer = new UIPanGestureRecognizer(PanThisCell);
            panRecognizer.Delegate = new PanGestureRecognizerDelegate();
            containerView.TranslatesAutoresizingMaskIntoConstraints = false;
            containerView.AddGestureRecognizer(panRecognizer);

			NSAction action = () => {
				if (containerView.Frame.Width != this.Frame.Width) {
					rightPosition.Constant = 0;
				} else {
					rightPosition.Constant = voteView.Frame.Width;
				}
			};

			tapRecognizer = new UITapGestureRecognizer (action );

			tapRecognizer.Delegate = new PanGestureRecognizerDelegate ();
			containerView.TranslatesAutoresizingMaskIntoConstraints = false;
			tapRecognizer.NumberOfTapsRequired = 1;
			containerView.AddGestureRecognizer (tapRecognizer);


            if (!String.IsNullOrEmpty(comment.ImageURL))
            {
                commentImageView.Image = ImageLoader.DefaultRequestImage(new Uri(comment.ImageURL), new ImageUpdateDelegate(commentImageView));
            }

            if (!String.IsNullOrEmpty(comment.T))
            {
                text.AttributedText = new NSAttributedString(comment.T, UIFont.FromName(BGAppearanceConstants.FontName, 14), UIColor.Black);
                text.ScrollEnabled = false;
            }

            author.AttributedText = new NSAttributedString(
                comment.AuthorName,
                UIFont.FromName(BGAppearanceConstants.BoldFontName, 14),
                UIColor.Black
            );


            timespan.AttributedText = new NSAttributedString(
                comment.ElapsedTimeString,
                UIFont.FromName(BGAppearanceConstants.BoldFontName, 14),
                UIColor.Black
            );


            upAndDownVotes.AttributedText = new NSAttributedString(
                comment.UpVoteCount.ToString() + "/" + comment.DownVoteCount.ToString(),
                UIFont.FromName(BGAppearanceConstants.BoldFontName, 14),
                UIColor.Black
            );

            voteView.BackgroundColor = BGAppearanceConstants.TealGreen;
            if (comment.uv == -1)
            {
                downVoteButton.SetImage(UIImage.FromFile("arrow_down_dark.png"), UIControlState.Normal);
                upVoteButton.SetImage(UIImage.FromFile("arrow_up.png"), UIControlState.Normal);
            }
            else if (comment.uv == 1)
            {
                downVoteButton.SetImage(UIImage.FromFile("arrow_down.png"), UIControlState.Normal);
                upVoteButton.SetImage(UIImage.FromFile("arrow_up_dark.png"), UIControlState.Normal);
            }

            downVoteButton.TouchUpInside += (sender, e) =>
            {
                BlahguaAPIObject.Current.SetCommentVote(this.comment, -1, (v) => Console.WriteLine(v));
                downVoteButton.SetImage(UIImage.FromFile("arrow_down_dark.png"), UIControlState.Normal);
                upVoteButton.SetImage(UIImage.FromFile("arrow_up.png"), UIControlState.Normal);
            };

            upVoteButton.TouchUpInside += (sender, e) =>
            {
                BlahguaAPIObject.Current.SetCommentVote(this.comment, 1, (v) => Console.WriteLine(v));
                downVoteButton.SetImage(UIImage.FromFile("arrow_down.png"), UIControlState.Normal);
                upVoteButton.SetImage(UIImage.FromFile("arrow_up_dark.png"), UIControlState.Normal);
            };
        }

        public void PanThisCell(UIPanGestureRecognizer recognizer)
        {
            switch (recognizer.State)
            {

                case UIGestureRecognizerState.Began:
                    panStartPoint = recognizer.TranslationInView(containerView);
                    break;
                case UIGestureRecognizerState.Changed:
                    PointF currentPoint = recognizer.TranslationInView(containerView);
                    float deltaX = currentPoint.X - panStartPoint.X;
                    bool panningLeft = false;
                    if (currentPoint.X < panStartPoint.X)
                    {
                        panningLeft = true;
                    }

                    if (startingLayoutRight == 0)
                    {
                        if (!panningLeft)
                        {
                            float constant = Math.Max(-deltaX, 0);
                            if (constant == 0)
                            {
                                ResetToStartPosition(true);
                            }
                            else
                            {
                                rightPosition.Constant = constant;
                            }
                        }
                        else
                        {
                            float constant = Math.Min(-deltaX, ButtonTotalWidth());
                            if (constant == ButtonTotalWidth())
                            {
                                SetFinalContainerViewPosition(true);
                            }
                            else
                            {
                                rightPosition.Constant = constant;
                            }
                        }
                    }
                    else
                    {
                        float adjustment = startingLayoutRight - deltaX;
                        if (!panningLeft)
                        {
                            float constant = Math.Max(adjustment, 0);
                            if (constant == 0)
                            {
                                ResetToStartPosition(true);
                            }
                            else
                            {
                                rightPosition.Constant = constant;
                            }
                        }
                        else
                        {
                            float constant = Math.Min(adjustment, ButtonTotalWidth());
                            if (constant == ButtonTotalWidth())
                            {
                                SetFinalContainerViewPosition(true);
                            }
                            else
                            {
                                rightPosition.Constant = constant;
                            }
                        }
                    }
                    break;
                case UIGestureRecognizerState.Ended:
                    if (startingLayoutRight == 0)
                    {
                        float position = ButtonTotalWidth() / 2 - 1;
                        if (rightPosition.Constant >= position)
                        {
                            SetFinalContainerViewPosition(true);
                        }
                        else
                        {
                            ResetToStartPosition(true);
                        }
                    }
                    else
                    {
                        float position = ButtonTotalWidth() / 2;
                        if (rightPosition.Constant >= position)
                        {
                            SetFinalContainerViewPosition(true);
                        }
                        else
                        {
                            ResetToStartPosition(true);
                        }
                    }
                    break;
                case UIGestureRecognizerState.Cancelled:
                    if (startingLayoutRight == 0)
                    {
                        ResetToStartPosition(true);
                    }
                    else
                    {
                        SetFinalContainerViewPosition(true);
                    }
                    break;
                default:
                    break;
            }
        }

        public void ResetToStartPosition(bool animated)
        {
            if (startingLayoutRight == 0 &&
             rightPosition.Constant == 0)
            {
                //Already all the way closed, no bounce necessary
                return;
            }

            rightPosition.Constant = 0;

            UpdateConstraintsIfNeeded(animated, () =>
            {
                startingLayoutRight = rightPosition.Constant;
            });
        }

        public void SetFinalContainerViewPosition(bool animated)
        {
            if (startingLayoutRight == ButtonTotalWidth() &&
                rightPosition.Constant == ButtonTotalWidth())
            {
                return;
            }

            rightPosition.Constant = ButtonTotalWidth();

            UpdateConstraintsIfNeeded(animated, () =>
            {
                startingLayoutRight = rightPosition.Constant;
            });
        }

        private void UpdateConstraintsIfNeeded(bool animated, NSAction completionHandler)
        {
            float duration = 0;
            if (animated)
            {
                duration = 0.1f;
            }

            UIView.Animate(duration, () =>
            {
                LayoutIfNeeded();
            }, completionHandler);
        }

        private float ButtonTotalWidth()
        {
            return voteView.Frame.Width;
        }

        public override void PrepareForReuse()
        {
            base.PrepareForReuse();
            ResetToStartPosition(false);
        }
    }

    public class PanGestureRecognizerDelegate : UIGestureRecognizerDelegate
    {
        public override bool ShouldRecognizeSimultaneously(UIGestureRecognizer gestureRecognizer, UIGestureRecognizer otherGestureRecognizer)
        {
			return false;
        }

        public override bool ShouldReceiveTouch(UIGestureRecognizer recognizer, UITouch touch)
        {
            //			if(touch.View is UIButton)
            //			{
            //				return false;
            //			}
            return true;
        }
    }

}