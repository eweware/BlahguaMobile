// This file has been autogenerated from a class added in the UI designer.

using System;
using System.Drawing;
using System.Collections.Generic;

using BlahguaMobile.BlahguaCore;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.Dialog.Utilities;

namespace BlahguaMobile.IOS
{
	public partial class BGNewCommentViewController : UIViewController
	{
		const string chooseFromText = "Choose From";
		const string cancelText = "Cancel";
		const string fromCameraText = "From Camera";
		const string fromGalleryText = "From Gallery";
		const string deleteCurrentPhotoText = "Delete Current Photo";
		const string userProfileText = "Use Profile";
		const string deleteSignatare = "Delete Signature";

		private UIActivityIndicatorView progressIndicator;
		private UIImage imageForUploading;

		private bool isProfileSignature;


		public new BGCommentsViewController ParentViewController { get; set; }

		private CommentCreateRecord NewComment
		{
			get
			{
				if(BlahguaAPIObject.Current.CreateCommentRecord == null)
					BlahguaAPIObject.Current.CreateCommentRecord = new CommentCreateRecord();
				return BlahguaAPIObject.Current.CreateCommentRecord;
			}
		}

		public BGNewCommentViewController (IntPtr handle) : base (handle)
		{

		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			if(BlahguaAPIObject.Current.CreateCommentRecord == null)
				BlahguaAPIObject.Current.CreateCommentRecord = new CommentCreateRecord();
			var buttonsTextAttributes = new UIStringAttributes {
				Font = UIFont.FromName (BGAppearanceConstants.BoldFontName, 14),
				ForegroundColor = UIColor.Black
			};

			selectSignature.SetAttributedTitle (new NSAttributedString ("Signature", buttonsTextAttributes), UIControlState.Normal);
            //selectSignature.SetImage (UIImage.FromBundle ("signature_ico"), UIControlState.Normal);
			//selectSignature.TouchUpInside += ChooseSignature;

			selectImageButton.SetAttributedTitle (new NSAttributedString ("Select Image", buttonsTextAttributes), UIControlState.Normal);
            //selectImageButton.SetImage (UIImage.FromBundle ("image_select"), UIControlState.Normal);
			selectImageButton.TouchUpInside += ActionForImage;

			input.ReturnKeyType = UIReturnKeyType.Default;
            input.Changed += (object sender, EventArgs e) => {
				if(String.IsNullOrEmpty(input.Text))
					done.Enabled = false;
				else
					done.Enabled = true;
			};

            //done.SetBackgroundImage (UIImage.FromBundle ("long_button"), UIControlState.Normal);
            done.SetBackgroundImage (UIImage.FromBundle ("long_button_gray"), UIControlState.Disabled);
			done.SetAttributedTitle (new NSAttributedString (
				"Done", 
				UIFont.FromName(BGAppearanceConstants.MediumFontName, 17), 
				BGAppearanceConstants.buttonTitleInactiveColor), UIControlState.Disabled);

			done.SetAttributedTitle (new NSAttributedString (
				"Done", 
				UIFont.FromName(BGAppearanceConstants.MediumFontName, 17), 
				UIColor.White), UIControlState.Normal);
			if(!String.IsNullOrEmpty(input.Text))
			{
				done.Enabled = true;
			}
			else
			{
				done.Enabled = false;
			}
			done.TouchUpInside += (object sender, EventArgs e) => {
				Done();
				
			};

			cancel.TouchUpInside += (object sender, EventArgs e) => {
				this.View.RemoveFromSuperview();
			};
		}

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			base.PrepareForSegue (segue, sender);
			if (segue.Identifier == "fromNewCommentToSignatures")
				((BGSignaturesTableViewController)segue.DestinationViewController).ParentViewController = this;
		}

		public void Done()
		{
			if(!String.IsNullOrEmpty(input.Text))
			{
				NewComment.Text = input.Text;
				BlahguaAPIObject.Current.CreateComment (CommentCreated);
			}
		}



		private void ActionForImage(object sender, EventArgs e)
		{
			UIActionSheet sheet;
			if(NewComment.M == null || NewComment.M.Count == 0)
				sheet = new UIActionSheet (chooseFromText, null, cancelText, null, new string[] {
					fromCameraText,
					fromGalleryText,
					deleteCurrentPhotoText
				});
			else
				sheet = new UIActionSheet(chooseFromText, null, cancelText, null, new string[] {
					fromCameraText, fromGalleryText
				});
			sheet.ShowInView(View);
			sheet.Clicked += FileChooseActionSheetClicked;
		}

		private void FileChooseActionSheetClicked(object sender, UIButtonEventArgs eventArgs)
		{
			var filePicker = new BGImagePickerController();
			filePicker.FinishedPickingMedia += FileChooseFinished;
			filePicker.Canceled += (sender1, eventArguments) => {

				filePicker.DismissViewController(true, 
					() => {});
			};
			if (eventArgs.ButtonIndex == 1) {
				filePicker.SourceType = UIImagePickerControllerSourceType.PhotoLibrary;
				ParentViewController.PresentViewController(filePicker, true, null);
			} else if (eventArgs.ButtonIndex == 0) {
				filePicker.SourceType = UIImagePickerControllerSourceType.Camera;
				ParentViewController.PresentViewController (filePicker, true, 
					() => UIApplication.SharedApplication.SetStatusBarHidden(true, UIStatusBarAnimation.Slide));
			} else if (eventArgs.ButtonIndex == 2) {
				if (NewComment.M != null)
					NewComment.M.Clear();
			}
		}

		private void FileChooseFinished(object sender, UIImagePickerMediaPickedEventArgs eventArgs)
		{
            UIImage image = imageForUploading =  UIImageHelper.ScaleAndRotateImage(eventArgs.OriginalImage);
			DateTime now = DateTime.Now;
			string imageName = String.Format ("{0}_{1}.jpg", now.ToLongDateString(), BlahguaAPIObject.Current.CurrentUser.UserName);
			BlahguaAPIObject.Current.UploadPhoto (image.AsJPEG ().AsStream (), imageName, ImageUploaded);
			progressIndicator = new UIActivityIndicatorView (UIActivityIndicatorViewStyle.Gray);
			progressIndicator.TranslatesAutoresizingMaskIntoConstraints = false;
			var constraintWidth = NSLayoutConstraint.Create (progressIndicator, NSLayoutAttribute.Width, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 1, 40);
			var constraintHeight = NSLayoutConstraint.Create (progressIndicator, NSLayoutAttribute.Height, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 1, 40);
			progressIndicator.AddConstraints (new NSLayoutConstraint[] { constraintHeight, constraintWidth });
			progressIndicator.HidesWhenStopped = true;
			selectImageButton.Hidden = true;
			View.AddSubview (progressIndicator);
			View.BringSubviewToFront (progressIndicator);
			var constraintX = NSLayoutConstraint.Create (progressIndicator, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, selectImageButton, NSLayoutAttribute.CenterX, 1, 0);
			var constraintY = NSLayoutConstraint.Create (progressIndicator, NSLayoutAttribute.CenterY, NSLayoutRelation.Equal, selectImageButton, NSLayoutAttribute.CenterY, 1, 0);
			View.AddConstraints (new NSLayoutConstraint[] { constraintX, constraintY });
			progressIndicator.StartAnimating ();
			((BGImagePickerController) sender).DismissViewController(true, 
				() => {});
		}

		private void ChooseSignature (object sender, EventArgs e)
		{
			UIActionSheet sheet;

			if(isProfileSignature)
			{
				sheet = new UIActionSheet (chooseFromText, null, cancelText, null, new string[] {
					userProfileText,
					deleteSignatare
				});
			}
			else
			{
				sheet = new UIActionSheet (chooseFromText, null, cancelText, null, new string[] {
					userProfileText
				});
			}

			sheet.ShowInView(View);
			sheet.Clicked += SignatureChoosedClicked;
		}

		private void SignatureChoosedClicked(object sender, UIButtonEventArgs eventArgs)
		{
			if (eventArgs.ButtonIndex == 1) {
				isProfileSignature = true;
			} else if (eventArgs.ButtonIndex == 0) {
				isProfileSignature = false;
			}
		}

		private void CommentCreated(Comment newComment)
		{
            if (newComment != null)
            {
                //ParentViewController.CurrentBlah.Comments.Insert(0, newComment);

                AppDelegate.analytics.PostCreateComment();
                NewComment.Text = "";
                InvokeOnMainThread(() =>
                    {
                        this.View.RemoveFromSuperview();
                        input.Text = "";
                        ParentViewController.SwitchNewCommentMode();
                    }
                );

                //BGCommentsViewController vc = (BGCommentsViewController)ParentViewController;
                //vc.ReloadComments();
            }
            else
            {
                AppDelegate.analytics.PostSessionError("commentcreatefailed");
                Console.WriteLine("Comment post failed");
            }
		}

		private void ImageUploaded(string s)
		{
            if (!String.IsNullOrEmpty(s))
            {
                AppDelegate.analytics.PostUploadCommentImage();
                InvokeOnMainThread(() =>
                    {
                        progressIndicator.StopAnimating();
                        selectImageButton.Hidden = false;
                        selectImageButton.SetImage(imageForUploading, UIControlState.Normal);
                        selectImageButton.ImageEdgeInsets = new UIEdgeInsets(0, 56, 0, 56);
                        if (NewComment.M == null || NewComment.M.Count == 0)
                        {
                            NewComment.M = new List<string>();
                        }
                        NewComment.M.Add(s);
                    });
            }
            else
            {
                AppDelegate.analytics.PostSessionError("commentimageuploadfailed");
                InvokeOnMainThread(() =>
                    {
                        progressIndicator.StopAnimating();
                        selectImageButton.Hidden = false;

                    });
            }
		}
	}
}
