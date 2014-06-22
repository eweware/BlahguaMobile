// This file has been autogenerated from a class added in the UI designer.

using System;
using System.ComponentModel;
using System.Drawing;

using BlahguaMobile.BlahguaCore;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace BlahguaMobile.IOS
{
	public partial class BGNewPostViewController : UIViewController
	{
		const string chooseFromText = "Choose From";
		const string cancelText = "Cancel";
		const string fromCameraText = "From Camera";
		const string fromGalleryText = "From Gallery";
		const string deleteCurrentPhotoText = "Delete Current Photo";
		const string userProfileText = "User Profile";
		const string deleteSignatare = "Delete Signature";

		private float space = 8f;

		private bool isProfileSignature;

		private UITextField expirationDateInput;
		private UITextField ExpirationDateInput
		{
			get
			{
				if(expirationDateInput == null)
				{
					expirationDateInput = new BGTextField (new RectangleF (pollItemsTableView.Frame.Location, 
																			new Size (305, 40)));

					expirationDateInput.AttributedPlaceholder = new NSAttributedString(
						"Type prediction expiration date (mm/dd/yyyy)",
						UIFont.FromName(BGAppearanceConstants.FontName, 12),
						UIColor.Black
					);
					expirationDateInput.AttributedText = new NSAttributedString(
						String.Empty, 
						UIFont.FromName(BGAppearanceConstants.FontName, 12),
						UIColor.Black
					);

					expirationDateInput.Background = UIImage.FromFile ("input_back.png");

					expirationDateInput.ReturnKeyType = UIReturnKeyType.Default;
					expirationDateInput.ShouldReturn = delegate {
						DateTime expDateTime;
						if (DateTime.TryParse (expirationDateInput.Text, out expDateTime)) 
						{
							return true;
						} 
						else 
						{
							return false;
						}
					};
					View.AddSubview (expirationDateInput);
					expirationDateInput.Hidden = true;
				}
				return expirationDateInput;
			}
		}

		public BGCommentsViewController ParentViewController { get; set; }

		private BlahCreateRecord NewPost 
		{
			get 
			{
				if (BlahguaAPIObject.Current.CreateRecord == null)
					BlahguaAPIObject.Current.CreateRecord = new BlahCreateRecord ();
				return BlahguaAPIObject.Current.CreateRecord;
			}
		}

		public BGNewPostViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			pollItemsTableView.Hidden = true;

			var buttonsTextAttributes = new UIStringAttributes {
				Font = UIFont.FromName (BGAppearanceConstants.BoldFontName, 14),
				ForegroundColor = UIColor.Black
			};

			((UIScrollView)View).ScrollEnabled = true;
			View.BackgroundColor = UIColor.FromPatternImage(UIImage.FromFile("grayBack.png"));

			selectSignature.SetAttributedTitle (new NSAttributedString ("  Signature", buttonsTextAttributes), UIControlState.Normal);
			selectSignature.SetImage (UIImage.FromFile ("signature_ico.png"), UIControlState.Normal);
			selectSignature.TouchUpInside += ChooseSignature;

			selectImageButton.SetAttributedTitle (new NSAttributedString ("  Select Image", buttonsTextAttributes), UIControlState.Normal);
			selectImageButton.SetImage (UIImage.FromFile ("image_select.png"), UIControlState.Normal);
			selectImageButton.TouchUpInside += ActionForImage;

			titleInput.ReturnKeyType = UIReturnKeyType.Next;
			titleInput.ShouldReturn = delegate {
				bodyInput.BecomeFirstResponder ();
				return false;
			};

			bodyInput.ReturnKeyType = UIReturnKeyType.Next;
			bodyInput.ShouldReturn = delegate {
				bodyInput.ResignFirstResponder();
				return true;
			};

			done.TouchUpInside += (object sender, EventArgs e) => {
				Done();
			};

			pollItemsTableView.AllowsMultipleSelection = false;
			pollItemsTableView.Source = new BGNewPostPollTableSource (this);
			pollItemsTableView.BackgroundColor = UIColor.Clear;

			pollItemsTableView.TableFooterView = new UIView ();
			pollItemsTableView.ScrollEnabled = false;

			BlahguaAPIObject.Current.PropertyChanged += (object sender, PropertyChangedEventArgs e) => 
			{
				if(e.PropertyName == "CreateRecord")
				{
					var typeName = BlahguaAPIObject.Current.CreateRecord.BlahType.N;
					if(typeName == "polls")
					{
						InvokeOnMainThread(() => {
							pollItemsTableView.Hidden = false;
							ExpirationDateInput.Hidden = true;
							PreparePollMode();
						});
					}
					else if(typeName == "predicts")
					{
						InvokeOnMainThread(() => {
							ExpirationDateInput.AttributedText = new NSAttributedString(
								NewPost.E ?? String.Empty, 
								UIFont.FromName(BGAppearanceConstants.FontName, 14),
								UIColor.Black
							);
							pollItemsTableView.Hidden = true;
							ExpirationDateInput.Hidden = false;
						});
					}
					else
					{
						pollItemsTableView.Hidden = true;
						ExpirationDateInput.Hidden = true;
					}

				}
			};
		}

		public void Done()
		{
			if(!String.IsNullOrEmpty(titleInput.Text))
			{
				NewPost.F = titleInput.Text;
				NewPost.T = bodyInput.Text;
				NewPost.UseProfile = isProfileSignature;
				NewPost.Badges = BlahguaAPIObject.Current.CurrentUser.Badges;
				DateTime expDate;
				if (DateTime.TryParse (expirationDateInput.Text, out expDate))
					NewPost.ExpirationDate = expDate;
				if(NewPost.I != null && NewPost.I.Count > 0)
				{
					foreach(var pi in NewPost.I)
					{
						if(String.IsNullOrEmpty(pi.T))
						{
							NewPost.I.Remove (pi);
						}
					}
				}
				BlahguaAPIObject.Current.CreateBlah (PostCreated);
			}
		}



		private void ActionForImage(object sender, EventArgs e)
		{
			UIActionSheet sheet;
			if(NewPost.M == null || NewPost.M.Count == 0)
				sheet = new UIActionSheet (chooseFromText, null, cancelText, null, new string[] {
					fromCameraText,
					fromGalleryText
				});
			else
				sheet = new UIActionSheet(chooseFromText, null, cancelText, null, new string[] {
					fromCameraText, 
					fromGalleryText, 
					deleteCurrentPhotoText
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
				ParentViewController.PresentViewController(filePicker, true, null);
			} else if (eventArgs.ButtonIndex == 0) {
				filePicker.SourceType = UIImagePickerControllerSourceType.Camera;
				ParentViewController.PresentViewController (filePicker, true, 
					() => UIApplication.SharedApplication.SetStatusBarHidden(true, UIStatusBarAnimation.Slide));
			} else if (eventArgs.ButtonIndex == 2) {
				NewPost.M.Clear();
			}
		}

		private void FileChooseFinished(object sender, UIImagePickerMediaPickedEventArgs eventArgs)
		{
			UIImage image = eventArgs.OriginalImage;
			DateTime now = DateTime.Now;
			string imageName = String.Format ("{0}_{1}.png", now.ToLongDateString(), BlahguaAPIObject.Current.CurrentUser.UserName);
			BlahguaAPIObject.Current.UploadPhoto (image.AsPNG ().AsStream (), imageName, ImageUploaded);
			((BGImagePickerController) sender).DismissViewController(true, 
				() => UIApplication.SharedApplication.SetStatusBarHidden (false, UIStatusBarAnimation.Slide));
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

		private void PreparePollMode()
		{
			NewPost.I.Clear ();
			NewPost.I.Add (new PollItem (""));
			NewPost.I.Add (new PollItem (""));

			pollItemsTableView.ReloadData ();
			AdjustTableViewSize ();
		}

		public void AdjustTableViewSize ()
		{
			var newSize = new SizeF (pollItemsTableView.Frame.Width, pollItemsTableView.NumberOfRowsInSection (0) * pollItemsTableView.RowHeight);
			pollItemsTableView.Frame = new RectangleF (pollItemsTableView.Frame.Location, newSize);
			((UIScrollView)View).ContentSize = new SizeF (320, pollItemsTableView.Frame.Bottom);
		}

		private void PostCreated(Blah NewPost)
		{
			Console.WriteLine ("Post pushed");
		}

		private void ImageUploaded(string s)
		{
			NewPost.M.Add (s);
			Console.WriteLine (s);
		}
	}

	public class BGNewPostPollTableSource : UITableViewSource
	{
		private const float rowHeight = 40f;

		private BGNewPostViewController vc;

		public BGNewPostPollTableSource(BGNewPostViewController vc) : base()
		{
			this.vc = vc;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = (BGNewPostPollItemCell)tableView.DequeueReusableCell ("newPostPollCell");

			string backImageName = String.Empty;
			if (indexPath.Row == 0)
			{
				backImageName = "input_field_top.png";
				cell.SetUpWithPollItem (BlahguaAPIObject.Current.CreateRecord.I [indexPath.Row]);
			} 
			else if(BlahguaAPIObject.Current.CreateRecord.I.Count  == indexPath.Row)
			{
				cell.SetUp();
				backImageName = "input_field_bottom.png";

			}
			else
			{
				backImageName = "input_field_middle.png";
				cell.SetUpWithPollItem (BlahguaAPIObject.Current.CreateRecord.I [indexPath.Row]);
			}
			vc.AdjustTableViewSize ();
			cell.BackgroundView = new UIImageView(UIImage.FromFile(backImageName));

			return cell;
		}

		public override float GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
		{
			return rowHeight;
		}

		public override int NumberOfSections (UITableView tableView)
		{
			return 1;
		}

		public override int RowsInSection (UITableView tableview, int section)
		{
			if (BlahguaAPIObject.Current.CreateRecord == null)
				BlahguaAPIObject.Current.CreateRecord = new BlahCreateRecord ();
			return BlahguaAPIObject.Current.CreateRecord.I.Count + 1;
		}

		public override NSIndexPath WillSelectRow (UITableView tableView, NSIndexPath indexPath)
		{
			if(indexPath.Row == BlahguaAPIObject.Current.CreateRecord.I.Count)
			{

				BlahguaAPIObject.Current.CreateRecord.I.Add (new PollItem (""));
				tableView.BeginUpdates ();
				tableView.InsertRows (new NSIndexPath[] { indexPath }, UITableViewRowAnimation.Left);
				tableView.EndUpdates ();

			}
			return indexPath;
		}
			
	}
}
