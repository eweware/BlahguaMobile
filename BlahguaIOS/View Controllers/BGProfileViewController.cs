using System;
using System.IO;
using System.CodeDom.Compiler;
using System.Drawing;

using BlahguaMobile.BlahguaCore;

using MonoTouch.Foundation;
using MonoTouch.Dialog.Utilities;
using MonoTouch.UIKit;
using MonoTouch.MessageUI;

namespace BlahguaMobile.IOS
{
	partial class BGProfileViewController : UIViewController, IImageUpdated
	{

		#region Fields

		const string chooseFromText = "Choose From";
		const string cancelText = "Cancel";
		const string fromCameraText = "From Camera";
		const string fromGalleryText = "From Gallery";
		const string deleteCurrentPhotoText = "Delete Current Photo";

		private const float iPhone4Padding = 35.0f;
		private NSObject keyboardShowObserver;
		private NSObject keyboardHideObserver;
		UIImage profile_image;
        private MFMailComposeViewController mailComposer;
		private bool keyboardHidden;

		private UIActivityIndicatorView progressIndicator;

		#endregion

		#region Properties

		public bool IsEditMode { get; set; }

		#endregion

		public BGProfileViewController (IntPtr handle) : base (handle)
		{
		}

		#region View Controller Overridden Methods

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			keyboardShowObserver = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillShowNotification, notification => {
				keyboardHidden = false;
				CalibrateViewPosition();
			});

			keyboardHideObserver = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification,  (notification) => { 
				keyboardHidden = true;
				CalibrateViewPosition();
			});

            // change header font
			this.NavigationController.NavigationBar.TitleTextAttributes = new UIStringAttributes() { 
                Font = UIFont.FromName ("Merriweather", 20),
                ForegroundColor = UIColor.FromRGB (96, 191, 164)
            };
		}

		public override void ViewDidLoad ()
		{
            AppDelegate.analytics.PostPageView("/self/profile");
			View.BackgroundColor = UIColor.FromPatternImage (UIImage.FromBundle ("grayBack"));
			this.NavigationController.SetNavigationBarHidden (false, true);
			NavigationItem.LeftBarButtonItem = new UIBarButtonItem ("Cancel", UIBarButtonItemStyle.Plain, CancelHandler);
            NavigationItem.LeftBarButtonItem.SetTitleTextAttributes(new UITextAttributes
                { 
                    TextColor = BGAppearanceConstants.TealGreen, 
                    TextShadowColor = UIColor.Clear, 
                    Font = UIFont.FromName("Merriweather", 16) 
                }, UIControlState.Normal  );

            NavigationItem.RightBarButtonItem = new UIBarButtonItem("Done", UIBarButtonItemStyle.Plain, DoneHandler);
            NavigationItem.RightBarButtonItem.SetTitleTextAttributes(new UITextAttributes
                { 
                    TextColor = BGAppearanceConstants.TealGreen, 
                    TextShadowColor = UIColor.Clear, 
                    Font = UIFont.FromName("Merriweather", 16) 
                }, UIControlState.Normal  );

			profileView.BackgroundColor = UIColor.FromPatternImage (UIImage.FromBundle("profileInfoBack"));

			nicknameTextField.Text = BlahguaAPIObject.Current.CurrentUser.UserName;
			nicknameTextField.SetNeedsDisplay ();

            showMatureBtn.Enabled = true;
            showMatureBtn.On = BlahguaAPIObject.Current.CurrentUser.WantsMatureContent;

            ReportBugButton.TouchUpInside +=(object sender, EventArgs e) => {
                if(MFMailComposeViewController.CanSendMail)
                {
                    mailComposer = new MFMailComposeViewController();
                    mailComposer.SetToRecipients(new string[] { "admin@goheard.com" });
                    mailComposer.SetSubject("Report issue on Heard for iOS");
                    mailComposer.Finished += (s, ev) => {
                        ev.Controller.DismissViewController(true, null);
                    };
                    PresentViewController(mailComposer, true, null);
                }
                else
                {
                    var alert = new UIAlertView("Information", "There are no email accounts on this iPhone. Please add email account and try again.", null, "OK");
                    alert.Show();
                }
            };

			if(!IsEditMode)
			{
				nicknameTextField.Enabled = false;
				selectImage.Hidden = true;
			}
			else
			{
				nicknameTextField.ReturnKeyType = UIReturnKeyType.Done;
				nicknameTextField.ShouldReturn = delegate {
					BlahguaCore.BlahguaAPIObject.Current.UpdateUserName(nicknameTextField.Text, NicknameUpdateCallback);
					nicknameTextField.ResignFirstResponder();
					return true;
				};
					
				nicknameTextField.Enabled = true;
				selectImage.Hidden = false;
				selectImage.TouchUpInside += ActionForImage;
			}

			var url = BlahguaAPIObject.Current.CurrentUser.UserImage;
			if (url != null)
				selectImage.Hidden = true;



			if(!String.IsNullOrEmpty(url))
			{
				selectImage.Hidden = true;
				if (url != null) {
					//UIImage profileImage = UIImageHelper.ImageFromUrl (url);

					//UIImageView profileImageView = new UIImageView (profileImage);
					Uri imageToLoad = new Uri (url);

					profileImageView.Image = ImageLoader.DefaultRequestImage  (imageToLoad, this);

					//profileImageView.Frame = new RectangleF (89, 24, 128, 128);

					var button = new UIButton (profileImageView.Frame);
					button.TouchUpInside += ActionForImage;

					profileView.Add (button);

					//profileView.SendSubviewToBack (profileImageView);
				}
			}
			else
			{
				selectImage.Hidden = false;
			}
		}
		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear (animated);
			UIApplication.SharedApplication.SetStatusBarHidden (true, false);
		}
		public override void ViewWillDisappear(bool animated)
		{
			base.ViewWillDisappear(animated);
			if(keyboardHideObserver != null)
			{
				NSNotificationCenter.DefaultCenter.RemoveObserver(keyboardHideObserver);
			}
			if(keyboardShowObserver != null)
			{
				NSNotificationCenter.DefaultCenter.RemoveObserver(keyboardShowObserver);
			}
		}

		#endregion

		#region Methods

		private void DoneHandler(object sender, EventArgs args)
		{
			BlahguaCore.BlahguaAPIObject.Current.UpdateUserName(nicknameTextField.Text, NicknameUpdateCallback);
            BlahguaAPIObject.Current.UpdateMatureFlag(showMatureBtn.On, null);


			NavigationController.PopToRootViewController (true);
		}

		private void CancelHandler(object sender, EventArgs args)
		{
			NavigationController.PopViewControllerAnimated (true);
		}

		private void NicknameUpdateCallback(string result)
		{
			InvokeOnMainThread (() => {
				BlahguaCore.BlahguaAPIObject.Current.CurrentUser.UserName = nicknameTextField.Text;
                BlahguaAPIObject.Current.CurrentUser.WantsMatureContent = showMatureBtn.On;
				((AppDelegate)UIApplication.SharedApplication.Delegate).SlideMenu.UpdateProfileImage ();
			});
			Console.WriteLine (result);
		}

		private void ActionForImage(object sender, EventArgs e)
		{
			UIActionSheet sheet;
			if(!String.IsNullOrEmpty(BlahguaAPIObject.Current.CurrentUser.UserImage))
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
					() => UIApplication.SharedApplication.SetStatusBarHidden (false, UIStatusBarAnimation.Slide));
			};
			if (eventArgs.ButtonIndex == 1) {
				filePicker.SourceType = UIImagePickerControllerSourceType.PhotoLibrary;
				PresentViewController(filePicker, true, null);
			} else if (eventArgs.ButtonIndex == 0) {
				filePicker.SourceType = UIImagePickerControllerSourceType.Camera;
				PresentViewController (filePicker, true, 
					() => UIApplication.SharedApplication.SetStatusBarHidden(true, UIStatusBarAnimation.Slide));
			} else if (eventArgs.ButtonIndex == 2) {
				BlahguaAPIObject.Current.DeleteUserImage (DeleteUserImageCallback);
			}
		}

		private void FileChooseFinished(object sender, UIImagePickerMediaPickedEventArgs eventArgs)
		{
            UIImage image =  UIImageHelper.ScaleAndRotateImage(eventArgs.OriginalImage);
			profile_image = image;
			DateTime now = DateTime.Now;
			string imageName = String.Format ("{0}_{1}.jpg", now.ToLongDateString(), BlahguaAPIObject.Current.CurrentUser.UserName);

			BlahguaAPIObject.Current.UploadUserImage (image.AsJPEG ().AsStream (), 
													  imageName,
													  ProfileImageUploadCallback);
				
			progressIndicator = new UIActivityIndicatorView (UIActivityIndicatorViewStyle.Gray);
			progressIndicator.TranslatesAutoresizingMaskIntoConstraints = false;
			var constraintWidth = NSLayoutConstraint.Create (progressIndicator, NSLayoutAttribute.Width, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 1, 40);
			var constraintHeight = NSLayoutConstraint.Create (progressIndicator, NSLayoutAttribute.Height, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 1, 40);
			progressIndicator.AddConstraints (new NSLayoutConstraint[] { constraintHeight, constraintWidth });
			progressIndicator.HidesWhenStopped = true;
			profileImageView.Hidden = true;
			View.AddSubview (progressIndicator);
			View.BringSubviewToFront (progressIndicator);
			var constraintX = NSLayoutConstraint.Create (progressIndicator, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, profileImageView, NSLayoutAttribute.CenterX, 1, 0);
			var constraintY = NSLayoutConstraint.Create (progressIndicator, NSLayoutAttribute.CenterY, NSLayoutRelation.Equal, profileImageView, NSLayoutAttribute.CenterY, 1, 0);
			View.AddConstraints (new NSLayoutConstraint[] { constraintX, constraintY });
			progressIndicator.StartAnimating ();

			((BGImagePickerController) sender).DismissViewController(true, 
				() => UIApplication.SharedApplication.SetStatusBarHidden (false, UIStatusBarAnimation.Slide));

		}

		private void ProfileImageUploadCallback(string result)
		{
            if (!String.IsNullOrEmpty(result))
            {
                AppDelegate.analytics.PostUploadUserImage();
                InvokeOnMainThread(() =>
                    {
                        progressIndicator.StopAnimating();
                        profileImageView.Hidden = false;
                        profileImageView.Image = profile_image;
                    });
            }
            else
            {
                AppDelegate.analytics.PostSessionError("userimageuploadfailed");
                InvokeOnMainThread(() =>
                    {
                        progressIndicator.StopAnimating();
                        profileImageView.Hidden = false;
                        profileImageView.Image = null;
                    });
            }
			Console.WriteLine (result);
		}

		private void DeleteUserImageCallback(string result)
		{
			Console.WriteLine (result);
		}

		private void CalibrateViewPosition ()
		{
			if(BGAppearanceHelper.DeviceType == DeviceType.iPhone4)
			{
				if(!keyboardHidden)
				{
					profileView.Frame = new RectangleF (profileView.Frame.X, 
						profileView.Frame.Y - iPhone4Padding, 
						profileView.Frame.Width, 
						profileView.Frame.Height);


				}
				else
				{
					profileView.Frame = new RectangleF (profileView.Frame.X, 
						profileView.Frame.Y + iPhone4Padding, 
						profileView.Frame.Width, 
						profileView.Frame.Height);
				}
			}
		}

		#endregion

		#region IImageUpdated implementation

		public void UpdatedImage (Uri uri)
		{
			profileImageView.Image = ImageLoader.DefaultRequestImage (uri, this);
		}

		#endregion
	}

	[Register("BGImagePickerController")]
	public class BGImagePickerController : UIImagePickerController {

		public BGImagePickerController() : base() 
		{
			NavigationBar.TitleTextAttributes = new UIStringAttributes { ForegroundColor = UIColor.White };
		}
	}
}
