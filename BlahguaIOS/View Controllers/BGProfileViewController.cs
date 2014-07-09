using System;
using System.IO;
using System.CodeDom.Compiler;
using System.Drawing;

using BlahguaMobile.BlahguaCore;

using MonoTouch.Foundation;
using MonoTouch.Dialog.Utilities;
using MonoTouch.UIKit;

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

		private bool keyboardHidden;

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
		}

		public override void ViewDidLoad ()
		{
			View.BackgroundColor = UIColor.FromPatternImage (UIImage.FromBundle ("grayBack"));

			NavigationItem.LeftBarButtonItem = new UIBarButtonItem ("Cancel", UIBarButtonItemStyle.Plain, CancelHandler);
            //Synsoft on 9 July 2014 for active color  #1FBBD1
            NavigationItem.LeftBarButtonItem.TintColor = UIColor.FromRGB(31, 187, 209);
            NavigationItem.RightBarButtonItem = new UIBarButtonItem("Done", UIBarButtonItemStyle.Plain, DoneHandler);
            //Synsoft on 9 July 2014 for active color  #1FBBD1
            NavigationItem.RightBarButtonItem.TintColor = UIColor.FromRGB(31, 187, 209);

			profileView.BackgroundColor = UIColor.FromPatternImage (UIImage.FromBundle("profileInfoBack"));

			nicknameTextField.Text = BlahguaAPIObject.Current.CurrentUser.UserName;
			nicknameTextField.SetNeedsDisplay ();


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
			if(!String.IsNullOrEmpty(url))
			{
				selectImage.Hidden = true;
				UIImage profileImage = UIImageHelper.ImageFromUrl(url);

				UIImageView profileImageView = new UIImageView (profileImage);
				profileImageView.Frame = new RectangleF (89, 24, 128, 128);

				var button = new UIButton (profileImageView.Frame);
				button.TouchUpInside += ActionForImage;

				profileView.Add (profileImageView);

				profileView.SendSubviewToBack (profileImageView);
			}
			else
			{
				selectImage.Hidden = false;
			}
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
			NavigationController.PopToRootViewController (true);
		}

		private void CancelHandler(object sender, EventArgs args)
		{
			NavigationController.PopViewControllerAnimated (true);
		}

		private void NicknameUpdateCallback(string result)
		{
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
			UIImage image = eventArgs.OriginalImage;
			DateTime now = DateTime.Now;
			string imageName = String.Format ("{0}_{1}.png", now.ToLongDateString(), BlahguaAPIObject.Current.CurrentUser.UserName);
			BlahguaAPIObject.Current.UploadUserImage (image.AsPNG ().AsStream (), 
													  imageName,
													  ProfileImageUploadCallback);
			((BGImagePickerController) sender).DismissViewController(true, 
				() => UIApplication.SharedApplication.SetStatusBarHidden (false, UIStatusBarAnimation.Slide));
		}

		private void ProfileImageUploadCallback(string result)
		{
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
			throw new NotImplementedException ();
		}

		#endregion
	}

	[Register("BGImagePickerController")]
	public class BGImagePickerController : UIImagePickerController {

		public BGImagePickerController() : base() 
		{
			NavigationBar.SetTitleTextAttributes(new UITextAttributes { TextColor = UIColor.White });
		}
	}
}
