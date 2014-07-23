// This file has been autogenerated from a class added in the UI designer.

using System;
using System.Linq;

using System.ComponentModel;
using System.Drawing;
using System.Collections.Generic;

using BlahguaMobile.BlahguaCore;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.ActionSheetDatePicker;

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

		private UIActivityIndicatorView progressIndicator;
		private UIImage imageForUploading;

		private bool isProfileSignature;
        private UIButton curTypeView = null;

		private RectangleF doneFrame;

		private ActionSheetDatePicker datePicker = null;

		private UITextField expirationDateInput;
		private UITextField ExpirationDateInput
		{
			get
			{
				if(expirationDateInput == null)
				{
					expirationDateInput = new BGTextField (new RectangleF (doneFrame.X, selectImageButton.Frame.Y + 42, 
																			305, 40));

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

                    expirationDateInput.Background = UIImage.FromBundle ("input_back");

					expirationDateInput.ReturnKeyType = UIReturnKeyType.Default;
					expirationDateInput.ShouldBeginEditing = delegate {
						datePicker = new ActionSheetDatePicker (this.View);
						datePicker.Title = "Choose Date";
						datePicker.DatePicker.Mode = UIDatePickerMode.Date;
						NSDateFormatter dateFormatter = new NSDateFormatter();
						dateFormatter.DateFormat = "MM/dd/yyyy";
					
						datePicker.DatePicker.ValueChanged += (s, e) => {

							NSDate dateValue = (s as UIDatePicker).Date;
							expirationDateInput.Text = new NSString(dateFormatter.ToString(dateValue));
						};

						datePicker.Show ();

						return false;
					};
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

		public BGRollViewController ParentViewController { get; set; }

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
        public string s, y;
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			pollItemsTableView.Hidden = true;

			doneFrame = done.Frame;

			var buttonsTextAttributes = new UIStringAttributes {
				Font = UIFont.FromName (BGAppearanceConstants.BoldFontName, 14),
				ForegroundColor = UIColor.Black
			};

			((UIScrollView)View).ScrollEnabled = true;
            View.BackgroundColor = UIColor.FromPatternImage(UIImage.FromBundle("grayBack"));

			selectSignature.SetAttributedTitle (new NSAttributedString ("  Signature", buttonsTextAttributes), UIControlState.Normal);
            //selectSignature.SetImage (UIImage.FromBundle ("signature_ico"), UIControlState.Normal);
			//selectSignature.TouchUpInside += ChooseSignature;

			selectImageButton.SetAttributedTitle (new NSAttributedString ("  Select Image", buttonsTextAttributes), UIControlState.Normal);
            //selectImageButton.SetImage (UIImage.FromBundle ("image_select"), UIControlState.Normal);
			selectImageButton.TouchUpInside += ActionForImage;

			titleInput.ReturnKeyType = UIReturnKeyType.Next;
             y = titleInput.Text;
            titleInput.ShouldReturn = delegate {
				bodyInput.BecomeFirstResponder ();
				return false;
			};

			bodyInput.ReturnKeyType = UIReturnKeyType.Next;
             s = bodyInput.Text;
			bodyInput.ShouldReturn = delegate {
				bodyInput.ResignFirstResponder();
				return true;
			};

            done.SetBackgroundImage (UIImage.FromBundle ("long_button"), UIControlState.Normal);
            done.SetBackgroundImage (UIImage.FromBundle ("long_button_gray"), UIControlState.Disabled);
			done.SetAttributedTitle (new NSAttributedString (
				"Done", 
				UIFont.FromName(BGAppearanceConstants.MediumFontName, 17), 
				BGAppearanceConstants.buttonTitleInactiveColor), UIControlState.Disabled);

			done.SetAttributedTitle (new NSAttributedString (
				"Done", 
				UIFont.FromName(BGAppearanceConstants.MediumFontName, 17), 
				UIColor.White), UIControlState.Normal);
           
            HandleTitleChanged(null);

            // Synsoft on 18 July 2014 
            HandleBodyChanged(null);
                    
			done.TouchUpInside += (object sender, EventArgs e) => {
				Done();
			};

			titleInput.Placeholder = "HEADLINE: Says are general posts, no requirements.";
			bodyInput.Placeholder = "Says is used for general sharing";
          
            SayBtn.TouchUpInside += (object sender, EventArgs e) =>
            {
                    SetBlahType(SayBtn, BlahguaAPIObject.Current.CurrentBlahTypes. First<BlahType>(n => n.N == "says"));
                    SayBtn.Highlighted = true;
            };
            curTypeView = SayBtn;
            EnableTypeBtn(SayBtn);

            PredictBtn.TouchUpInside += (object sender, EventArgs e) =>
                {
                    SetBlahType(PredictBtn, BlahguaAPIObject.Current.CurrentBlahTypes.First<BlahType>(n => n.N == "predicts"));

                };
            PollBtn.TouchUpInside += (object sender, EventArgs e) =>
                {
                    SetBlahType(PollBtn, BlahguaAPIObject.Current.CurrentBlahTypes.First<BlahType>(n => n.N == "polls"));

                };
            AskBtn.TouchUpInside += (object sender, EventArgs e) =>
                {
                    SetBlahType(AskBtn, BlahguaAPIObject.Current.CurrentBlahTypes.First<BlahType>(n => n.N == "asks"));

                };
            LeakBtn.TouchUpInside += (object sender, EventArgs e) =>
                {
                    SetBlahType(LeakBtn, BlahguaAPIObject.Current.CurrentBlahTypes.First<BlahType>(n => n.N == "leaks"));
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
                        UpdateForNewBlahType();

				}
			};
		}
        public bool isTitle;
        public bool isbody = false;
        public bool isFirst = false;

        partial void HandleTitleChanged(UITextField sender)
        {
            if (!string.IsNullOrEmpty(titleInput.Text) && isbody)
            {
              // Synsoft on 18 July 2014
            //    done.Enabled = true;

                isTitle = true;
                done.Enabled = true;
                //isbody = true;
            }
            else if (string.IsNullOrEmpty(titleInput.Text))
            {
                isTitle = false;
                done.Enabled = false;
            }
            else
            {
              // Synsoft on 18 July 2014
              done.Enabled = false;
                // Synsoft on 18 July 2014
                done.Enabled = false;
                isTitle = true;
            }
        }

        // Synsoft on 18 July 2014
        partial void HandleBodyChanged(UITextField sender)
        {
            if (!string.IsNullOrEmpty(bodyInput.Text) && isTitle)
            {
                done.Enabled = true;
                isbody = true;
            }
            else if (string.IsNullOrEmpty(bodyInput.Text))
            {
                isbody = false;
                done.Enabled = false;
            }
            else
            {
                    isbody = true;
                    isFirst = true;
           }

        }

       

        private void EnableTypeBtn(UIButton theType)
        {
            if (theType == SayBtn)
            {
                SayBtn.SetImage(UIImage.FromBundle("icon_speechact_say_teal"), UIControlState.Normal);
                SayBtnText.TextColor = UIColor.FromRGB(96, 191, 164);
            }
            else if (theType == PredictBtn)
            {
                PredictBtn.SetImage(UIImage.FromBundle("icon_speechact_predict_teal"), UIControlState.Normal);
                PredictBtnText.TextColor = UIColor.FromRGB(96, 191, 164);
            }
            else if (theType == PollBtn)
            {
                PollBtn.SetImage(UIImage.FromBundle("icon_speechact_poll_teal"), UIControlState.Normal);
                PollBtnText.TextColor = UIColor.FromRGB(96, 191, 164);
            }
            else if (theType == AskBtn)
            {
                AskBtn.SetImage(UIImage.FromBundle("icon_speechact_ask_teal"), UIControlState.Normal);
                AskBtnText.TextColor = UIColor.FromRGB(96, 191, 164);
            }
            else if (theType == LeakBtn)
            {
                LeakBtn.SetImage(UIImage.FromBundle("icon_speechact_leak_teal"), UIControlState.Normal);
                LeakBtnText.TextColor = UIColor.FromRGB(96, 191, 164);
            }
        }

        private void DisableTypeBtn(UIButton theType)
        {
            if (theType == SayBtn)
            {
                SayBtn.SetImage(UIImage.FromBundle("icon_speechact_say"), UIControlState.Normal);
                SayBtnText.TextColor = UIColor.FromRGB(63, 43, 47);
            }
            else if (theType == PredictBtn)
            {
                PredictBtn.SetImage(UIImage.FromBundle("icon_speechact_predict"), UIControlState.Normal);
                PredictBtnText.TextColor = UIColor.FromRGB(63, 43, 47);
            }
            else if (theType == PollBtn)
            {
                PollBtn.SetImage(UIImage.FromBundle("icon_speechact_poll"), UIControlState.Normal);
                PollBtnText.TextColor = UIColor.FromRGB(63, 43, 47);
            }
            else if (theType == AskBtn)
            {
                AskBtn.SetImage(UIImage.FromBundle("icon_speechact_ask"), UIControlState.Normal);
                AskBtnText.TextColor = UIColor.FromRGB(63, 43, 47);
            }   
        else if (theType == LeakBtn)
            {
                LeakBtn.SetImage(UIImage.FromBundle("icon_speechact_leak"), UIControlState.Normal);
                LeakBtnText.TextColor = UIColor.FromRGB(63, 43, 47);
            }
        }



        private void UpdateForNewBlahType()
        {
            var typeName = BlahguaAPIObject.Current.CreateRecord.BlahType.N;
            switch (typeName)
            {
			case "polls":
				InvokeOnMainThread (() => {
					pollItemsTableView.Hidden = false;

					pollItemsTableView.Frame = new RectangleF (pollItemsTableView.Frame.X, doneFrame.Y, pollItemsTableView.Frame.Width, pollItemsTableView.Frame.Height);


					ExpirationDateInput.Hidden = true;

					PreparePollMode ();
					bodyInput.Placeholder = "Polls must have at least two choices";

					done.RemoveFromSuperview ();
					done.Frame = new RectangleF (doneFrame.X, pollItemsTableView.Frame.Y + pollItemsTableView.Frame.Height + 8, doneFrame.Width, doneFrame.Height);
					View.AddSubview(done);
				});

                    break;

                case "predicts":
                    InvokeOnMainThread(() =>
                        {
                            ExpirationDateInput.AttributedText = new NSAttributedString(
                                NewPost.E ?? String.Empty, 
                                UIFont.FromName(BGAppearanceConstants.FontName, 14),
                                UIColor.Black
                            );
						done.RemoveFromSuperview ();
						done.Frame = new RectangleF(doneFrame.X, doneFrame.Y + 48, doneFrame.Width, doneFrame.Height) ;
						View.AddSubview(done);
                            pollItemsTableView.Hidden = true;
                            ExpirationDateInput.Hidden = false;


						titleInput.Placeholder = "HEADLINE: Predictions detail outcomes expected to occure.";
                            bodyInput.Placeholder = "Predictions require you to set a date";
                        });

                    break;

			case "says":
				pollItemsTableView.Hidden = true;
				ExpirationDateInput.Hidden = true;
				done.RemoveFromSuperview ();
				done.Frame = doneFrame ;
				View.AddSubview(done);
				titleInput.Placeholder = "HEADLINE: Says are general posts, no requirements.";
                    bodyInput.Placeholder = "Says is used for general sharing";
                    break;

                case "asks":
                    pollItemsTableView.Hidden = true;
                    ExpirationDateInput.Hidden = true;
				done.RemoveFromSuperview ();
				done.Frame = doneFrame ;
				View.AddSubview(done);
				titleInput.Placeholder = "HEADLINE: Asks are open-ended questions. Must include a '?'";
                    bodyInput.Placeholder = "Asks must be in the form a a question";
                    break;

                case "leaks":
                    pollItemsTableView.Hidden = true;
                    ExpirationDateInput.Hidden = true;
				done.RemoveFromSuperview ();
				done.Frame = doneFrame ;
				View.AddSubview(done);
				titleInput.Placeholder = "HEADLINE: Leaks require that a badge to be attached.";
                    bodyInput.Placeholder = "You must be badged to leak something";
                    break;

                default:
                    pollItemsTableView.Hidden = true;
                    ExpirationDateInput.Hidden = true;
				titleInput.Placeholder = "HEADLINE: Polls allow user to vote on pre-defined responses.";
                    break;
            }
        }

        private void SetBlahType(UIButton theView, BlahType newType)
        {
            if (theView != curTypeView)
            {
                if (curTypeView != null)
                    DisableTypeBtn(curTypeView);
                curTypeView = theView;
                if (curTypeView != null)
                {
                    EnableTypeBtn(curTypeView);
                    NewPost.BlahType = newType;
                    UpdateForNewBlahType();
                }
            }
        }

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			base.PrepareForSegue (segue, sender);
			if (segue.Identifier == "fromNewPostToSignatures")
				((BGSignaturesTableViewController)segue.DestinationViewController).ParentViewController = this;
		}

		public void Done()
		{
			if(!String.IsNullOrEmpty(titleInput.Text))
            {
				NewPost.F = titleInput.Text;
				NewPost.T = bodyInput.Text;

                switch (NewPost.BlahType.N)
                {
                    case "predicts":
                        DateTime expDate;
                        if (DateTime.TryParse(expirationDateInput.Text, out expDate))
                            NewPost.ExpirationDate = expDate;
                        break;

                    case "polls":
                        if (NewPost.I != null && NewPost.I.Count > 0)
                        {
                            foreach (var pi in NewPost.I)
                            {
                                if (String.IsNullOrEmpty(pi.T))
                                {
                                    NewPost.I.Remove(pi);
                                }
                            }
                        }
                        break;
                }
                System.Console.WriteLine("About to Submit!");
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
				NewPost.M.Clear();
			}
		}

		private void FileChooseFinished(object sender, UIImagePickerMediaPickedEventArgs eventArgs)
		{
			UIImage image = imageForUploading = eventArgs.OriginalImage;
			DateTime now = DateTime.Now;
			string imageName = String.Format ("{0}_{1}.png", now.ToLongDateString(), BlahguaAPIObject.Current.CurrentUser.UserName);
			BlahguaAPIObject.Current.UploadPhoto (image.AsPNG ().AsStream (), imageName, ImageUploaded);
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

			((UIScrollView)View).ContentSize = new SizeF (320, pollItemsTableView.Frame.Bottom + 60);
			if (pollItemsTableView.Hidden == false) {
				done.RemoveFromSuperview ();

				if ((pollItemsTableView.Frame.Y + pollItemsTableView.Frame.Height) >= (View.Bounds.Height - 48)) {
					done.Frame = new RectangleF (doneFrame.X, View.Bounds.Height - 48, doneFrame.Width, doneFrame.Height);
				} else
					done.Frame = new RectangleF (doneFrame.X, pollItemsTableView.Frame.Y + pollItemsTableView.Frame.Height + 8, doneFrame.Width, doneFrame.Height);

				View.AddSubview (done);
			}
		}

		private void PostCreated(Blah NewPost)
		{
			Console.WriteLine ("Post pushed");
		}

		private void ImageUploaded(string s)
		{
			InvokeOnMainThread (() => {
				progressIndicator.StopAnimating ();
				selectImageButton.Hidden = false;
				selectImageButton.SetImage (imageForUploading, UIControlState.Normal);
				selectImageButton.ImageEdgeInsets = new UIEdgeInsets(0, 56, 0, 56);
				if(NewPost.M == null || NewPost.M.Count == 0)
				{
					NewPost.M = new List<string>();
				}
				NewPost.M.Add (s);
			});
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
				backImageName = "input_field_top";
				cell.SetUpWithPollItem (BlahguaAPIObject.Current.CreateRecord.I [indexPath.Row]);
			} 
			else if(BlahguaAPIObject.Current.CreateRecord.I.Count  == indexPath.Row)
			{
				cell.SetUp();
				backImageName = "input_field_bottom";

			}
			else
			{
				backImageName = "input_field_middle";
				cell.SetUpWithPollItem (BlahguaAPIObject.Current.CreateRecord.I [indexPath.Row]);
			}
			vc.AdjustTableViewSize ();
            cell.BackgroundView = new UIImageView(UIImage.FromBundle(backImageName));

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
