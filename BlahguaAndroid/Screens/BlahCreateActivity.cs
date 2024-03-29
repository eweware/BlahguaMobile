using System;
using System.Linq;

using Android.Content;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Animation;
using Android.Content.PM;

using BlahguaMobile.BlahguaCore;
using SlidingMenuSharp;
using System.IO;
using Android.Database;
using System.Collections.Generic;
using BlahguaMobile.AndroidClient.ThirdParty.UrlImageViewHelper;
using Java.Util;
using Android.App;
using Java.Text;
using BlahguaMobile.AndroidClient.HelpingClasses;

using Android.Preferences;
using BlahguaMobile.AndroidClient.Adapters;
using System.IO.IsolatedStorage;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Provider;

using File = Java.IO.File;
using Uri = Android.Net.Uri;

namespace BlahguaMobile.AndroidClient.Screens
{
	[Activity(ScreenOrientation = ScreenOrientation.Portrait, Theme = "@android:style/Theme.Holo.Light.Dialog.NoActionBar")]
	public class BlahCreateActivity : Activity, IUrlImageViewCallback, View.IOnTouchListener
    {
        enum MyBlahType
        {
            Asks, Leaks, Polls, Predicts, Says
        }

        private MyBlahType currentType = MyBlahType.Says;

		private const int SELECTIMAGE_REQUEST = 777, MultiChoiceDialog = 0x03;
        private View additionalfields_layout;
        private EditText newPostTitle, newPostText;
        private EditText editPrediction, editPoll1, editPoll2, editPoll3, editPoll4, editPoll5, editPoll6, editPoll7, editPoll8, editPoll9, editPoll10;
        private Button btnAddOption;
        private Button btn_create_done;

        private FrameLayout imageCreateBlahLayout;
        private ImageView imageCreateBlah;
        private ImageView imageSay, imagePredict, imagePoll, imageAsk, imageLeak;
        private ProgressBar progressBarImageLoading;
        private ImageView currentSpeechAct;
		private string[] badgeItemNames = null;
		private bool[]	badgeItemBools = null;

        protected override void OnActivityResult(int requestCode, Android.App.Result resultCode, Intent data)
        {
            if ((requestCode == SELECTIMAGE_REQUEST || requestCode == HomeActivity.PHOTO_CAPTURE_EVENT)
                && resultCode == Android.App.Result.Ok)
            {
                progressBarImageLoading.Visibility = ViewStates.Visible;
                imageCreateBlahLayout.Visibility = ViewStates.Visible;
                imageCreateBlah.SetImageDrawable(null);
                System.IO.Stream fileStream;
                String fileName;

                if (requestCode == SELECTIMAGE_REQUEST)
                {
                    fileStream = StreamHelper.GetStreamFromFileUri(this, data.Data);
                    fileName = StreamHelper.GetFileName(this, data.Data);
                }
                else
                {
                    Bitmap scaledBitmap = BitmapHelper.LoadAndResizeBitmap (HomeActivity._file.AbsolutePath, HomeActivity.MAX_IMAGE_SIZE);
                    fileStream = new System.IO.MemoryStream ();
                    scaledBitmap.Compress (Bitmap.CompressFormat.Jpeg, 90, fileStream);
				    fileStream.Flush ();
                    fileName = HomeActivity._file.Name;
                }

                if (fileStream != null)
                {
                    BlahguaAPIObject.Current.UploadPhoto(fileStream, fileName, (photoString) =>
                        {
                            RunOnUiThread(() =>
                            {
                                progressBarImageLoading.Visibility = ViewStates.Gone;
                                if ((photoString != null) && (photoString.Length > 0))
                                {
                                    string photoURL = BlahguaAPIObject.Current.GetImageURL(photoString, "B");
                                    imageCreateBlah.SetUrlDrawable(photoURL, this);
                                    BlahguaAPIObject.Current.CreateRecord.M = new List<MediaRecordObject>();
                                    MediaRecordObject newRec = new MediaRecordObject();
                                    newRec.type = 1;
                                    newRec.url = photoString;
                                    BlahguaAPIObject.Current.CreateRecord.M.Add(newRec);
                                    HomeActivity.analytics.PostUploadBlahImage();
                                }
                                else
                                {
                                    ClearImages();
                                    HomeActivity.analytics.PostSessionError("blahimageuploadfailed");
                                }

								UpdateDoneBtn();
                            });
                        }
                    );
                    fileStream.Close();
                }
                else
                {
                    RunOnUiThread(() =>
                       {
                           var toast = Toast.MakeText(this, "Cannot upload this type of image", ToastLength.Long);
                           toast.Show();
                           progressBarImageLoading.Visibility = ViewStates.Gone;
                           ClearImages();
                       });
                }
            }
            
            base.OnActivityResult(requestCode, resultCode, data);
        }

        public void OnLoaded(ImageView imageView, Android.Graphics.Drawables.Drawable loadedDrawable, string url, bool loadedFromCache)
        {
            if (imageCreateBlah == imageView)
            {
                RunOnUiThread(() =>
                {
                    progressBarImageLoading.Visibility = ViewStates.Gone;
                });
            }
        }



        private void ClearImages()
        {
            progressBarImageLoading.Visibility = ViewStates.Gone;
            BlahguaAPIObject.Current.CreateRecord.M = null;
            imageCreateBlah.SetImageDrawable(null);
            imageCreateBlahLayout.Visibility = ViewStates.Gone;
			UpdateDoneBtn ();
        }

        public bool OnTouch(View v, MotionEvent ev)
        {
            if (ev.Action == MotionEventActions.Up)
            {

            }
            return true;
        }

        ///////// init
		protected override void OnCreate (Bundle bundle)
		{
			RequestWindowFeature(WindowFeatures.NoTitle);
			//this.Window.AddFlags(WindowManagerFlags.Fullscreen);
			//this.Window.DecorView.SystemUiVisibility = StatusBarVisibility.Hidden;
			base.OnCreate(bundle);


			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.activity_create_post);
            // speech act btns
            imageSay = FindViewById<View>(Resource.Id.btn_speechact_say) as ImageView;
            imagePredict = FindViewById<View>(Resource.Id.btn_speechact_predict) as ImageView;
            imagePoll = FindViewById<View>(Resource.Id.btn_speechact_poll) as ImageView;
            imageAsk = FindViewById<View>(Resource.Id.btn_speechact_ask) as ImageView;
            imageLeak = FindViewById<View>(Resource.Id.btn_speechact_leak) as ImageView;

            imageSay.Click += SpeechActBtn_Click;
            imagePredict.Click += SpeechActBtn_Click;
            imagePoll.Click += SpeechActBtn_Click;
            imageAsk.Click += SpeechActBtn_Click;
            imageLeak.Click += SpeechActBtn_Click;
            currentSpeechAct = imageSay;



            additionalfields_layout = FindViewById<View>(Resource.Id.additionalfields_layout);

            Button btn_select_image =  FindViewById<Button>(Resource.Id.btn_image);
            btn_select_image.Click += (sender, args) =>
            {
                PopupMenu imageMenu = new PopupMenu(this, btn_select_image);
                imageMenu.Inflate(Resource.Menu.cameramenu);

                if ((BlahguaAPIObject.Current.CreateRecord.M == null) ||
                    (BlahguaAPIObject.Current.CreateRecord.M.Count == 0))
                    imageMenu.Menu.RemoveItem(Resource.Id.action_removephoto);

                imageMenu.MenuItemClick += (s1, arg1) =>
                    {
                        if (arg1.Item.ItemId == Resource.Id.action_takephoto)
                        {
                            Intent intent = new Intent(MediaStore.ActionImageCapture);

                            HomeActivity._file = new File(HomeActivity._dir, String.Format("PhotoTossPhoto_{0}.jpg", Guid.NewGuid()));

                            intent.PutExtra(MediaStore.ExtraOutput, Uri.FromFile(HomeActivity._file));

                            StartActivityForResult(intent, HomeActivity.PHOTO_CAPTURE_EVENT);
                        }
                        else if (arg1.Item.ItemId == Resource.Id.action_selectphoto)
                        {
                            // select a photo
                            var imageIntent = new Intent();
                            imageIntent.SetType("image/*");
                            imageIntent.SetAction(Intent.ActionGetContent);

                            StartActivityForResult(
                                Intent.CreateChooser(imageIntent, "Select image"), SELECTIMAGE_REQUEST);
                        }
                        else if (arg1.Item.ItemId == Resource.Id.action_removephoto)
                        {
                            // remove a photo
                            ClearImages();
                        }
                    };

                imageMenu.Show();
            };
            Button btn_signature =  FindViewById<Button>(Resource.Id.btn_signature);
            btn_signature.Click += (sender, args) => {
				ShowDialog(MultiChoiceDialog);
            };
            btn_create_done =  FindViewById<Button>(Resource.Id.btn_done);
            btn_create_done.Click += (sender, args) =>
            {
				DoCreateClick();
            };
            btn_create_done.Enabled = false;
            newPostTitle =  FindViewById<EditText>(Resource.Id.title);
            newPostTitle.TextChanged += editCreate_TextChanged;
            newPostText =  FindViewById<EditText>(Resource.Id.text);
			newPostText.TextChanged += editCreate_TextChanged;

            imageCreateBlah =  FindViewById<ImageView>(Resource.Id.createblah_image);
            imageCreateBlahLayout =  FindViewById<FrameLayout>(Resource.Id.createblah_image_layout);
            progressBarImageLoading =  FindViewById<ProgressBar>(Resource.Id.progress_image_loading);
            progressBarImageLoading.Visibility = ViewStates.Gone;

            editPrediction =  FindViewById<EditText>(Resource.Id.prediction);
            editPrediction.TextChanged += editCreate_TextChanged;
            editPrediction.Focusable = false;
            setDatePicker(editPrediction);

            editPoll1 =  FindViewById<EditText>(Resource.Id.poll1);
            editPoll1.TextChanged += editCreate_TextChanged;
            editPoll2 =  FindViewById<EditText>(Resource.Id.poll2);
            editPoll2.TextChanged += editCreate_TextChanged;
            editPoll3 =  FindViewById<EditText>(Resource.Id.poll3);
            editPoll4 =  FindViewById<EditText>(Resource.Id.poll4);
            editPoll5 =  FindViewById<EditText>(Resource.Id.poll5);
            editPoll6 =  FindViewById<EditText>(Resource.Id.poll6);
            editPoll7 =  FindViewById<EditText>(Resource.Id.poll7);
            editPoll8 =  FindViewById<EditText>(Resource.Id.poll8);
            editPoll9 =  FindViewById<EditText>(Resource.Id.poll9);
            editPoll10 =  FindViewById<EditText>(Resource.Id.poll10);
            btnAddOption =  FindViewById<Button>(Resource.Id.btn_add_poll_option);
            btnAddOption.Click += DoAddPollChoice;

            setAsksCreateBlahType();

			triggerCreateBlock ();
        }

        void SpeechActBtn_Click(object sender, EventArgs e)
        {
            ImageView   newBtn = sender as ImageView;

            if (newBtn != currentSpeechAct)
            {
                DisableSpeechActBtn(currentSpeechAct);
                EnableSpeechActBtn(newBtn);
                currentSpeechAct = newBtn;
                setTitleHint();

                //triggerExpand();
            }
        }

        private void DisableSpeechActBtn(ImageView someBtn)
        {
            if (someBtn == imageSay)
            {
                TextView    theText = FindViewById(Resource.Id.text_speechact_say) as TextView;
                imageSay.SetImageResource(Resource.Drawable.icon_speechact_say);
                theText.SetTextColor(new Android.Graphics.Color(63,43,47));
            }
            else if (someBtn == imagePredict)
            {
                TextView    theText = FindViewById(Resource.Id.text_speechact_predict) as TextView;
                imagePredict.SetImageResource(Resource.Drawable.icon_speechact_predict);
                theText.SetTextColor(new Android.Graphics.Color(63, 43, 47));
            }
            else if (someBtn == imagePoll)
            {
                TextView    theText = FindViewById(Resource.Id.text_speechact_poll) as TextView;
                imagePoll.SetImageResource(Resource.Drawable.icon_speechact_poll);
                theText.SetTextColor(new Android.Graphics.Color(63, 43, 47));
            }
            else if (someBtn == imageAsk)
            {
                TextView    theText = FindViewById(Resource.Id.text_speechact_ask) as TextView;
                imageAsk.SetImageResource(Resource.Drawable.icon_speechact_ask);
                theText.SetTextColor(new Android.Graphics.Color(63, 43, 47));
            }
            else if (someBtn == imageLeak)
            {
                TextView    theText = FindViewById(Resource.Id.text_speechact_leak) as TextView;
                imageLeak.SetImageResource(Resource.Drawable.icon_speechact_leak);
                theText.SetTextColor(new Android.Graphics.Color(63, 43, 47));
            }
            
        }

        private void EnableSpeechActBtn(ImageView someBtn)
        {
            if (someBtn == imageSay)
            {
                TextView theText = FindViewById(Resource.Id.text_speechact_say) as TextView;
                imageSay.SetImageResource(Resource.Drawable.icon_speechact_say_teal);
                theText.SetTextColor(new Android.Graphics.Color(96, 191, 164));
                BlahguaAPIObject.Current.CreateRecord.BlahType =
                    BlahguaAPIObject.Current.CurrentBlahTypes.First<BlahType>(n => n.N == "says");
                currentType = MyBlahType.Says;
                setAsksCreateBlahType();
            }
            else if (someBtn == imagePredict)
            {
                TextView theText = FindViewById(Resource.Id.text_speechact_predict) as TextView;
                imagePredict.SetImageResource(Resource.Drawable.icon_speechact_predict_teal);
                theText.SetTextColor(new Android.Graphics.Color(96, 191, 164));
                BlahguaAPIObject.Current.CreateRecord.BlahType =
                    BlahguaAPIObject.Current.CurrentBlahTypes.First<BlahType>(n => n.N == "predicts");
                currentType = MyBlahType.Predicts;
                setPredictCreateBlahType();
            }
            else if (someBtn == imagePoll)
            {
                TextView theText = FindViewById(Resource.Id.text_speechact_poll) as TextView;
                imagePoll.SetImageResource(Resource.Drawable.icon_speechact_poll_teal);
                theText.SetTextColor(new Android.Graphics.Color(96, 191, 164));
                BlahguaAPIObject.Current.CreateRecord.BlahType =
                    BlahguaAPIObject.Current.CurrentBlahTypes.First<BlahType>(n => n.N == "polls");
                currentType = MyBlahType.Polls;
                setPollCreateBlahType();
            }
            else if (someBtn == imageAsk)
            {
                TextView theText = FindViewById(Resource.Id.text_speechact_ask) as TextView;
                imageAsk.SetImageResource(Resource.Drawable.icon_speechact_ask_teal);
                theText.SetTextColor(new Android.Graphics.Color(96, 191, 164));
                BlahguaAPIObject.Current.CreateRecord.BlahType =
                    BlahguaAPIObject.Current.CurrentBlahTypes.First<BlahType>(n => n.N == "asks");
                currentType = MyBlahType.Asks;
                setAsksCreateBlahType();
            }
            else if (someBtn == imageLeak)
            {
                TextView theText = FindViewById(Resource.Id.text_speechact_leak) as TextView;
                imageLeak.SetImageResource(Resource.Drawable.icon_speechact_leak_teal);
                theText.SetTextColor(new Android.Graphics.Color(96, 191, 164));
                BlahguaAPIObject.Current.CreateRecord.BlahType =
                    BlahguaAPIObject.Current.CurrentBlahTypes.First<BlahType>(n => n.N == "leaks");
                currentType = MyBlahType.Leaks;
                setAsksCreateBlahType();
            }
        }

        private void editCreate_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
			UpdateDoneBtn ();
        }

		private void UpdateDoneBtn()
		{
			bool hasImage = BlahguaAPIObject.Current.CreateRecord.M != null;
			bool noTitle = newPostTitle.Visibility == ViewStates.Visible && String.IsNullOrEmpty (newPostTitle.Text);

			if (noTitle) {
				if (hasImage && newPostText.Text.Length > 3)
					btn_create_done.Enabled = true;
				else
					btn_create_done.Enabled = false;

			} 
			else  if (newPostTitle.Visibility == ViewStates.Visible && newPostTitle.Text.Length == 0 && hasImage == false)
			{
				btn_create_done.Enabled = false;
			}
			else if (editPrediction.Visibility == ViewStates.Visible && editPrediction.Text.Length == 0)
			{
				btn_create_done.Enabled = false;
			}
			else if (editPoll1.Visibility == ViewStates.Visible && (editPoll1.Text.Length == 0 || editPoll2.Text.Length == 0))
			{
				btn_create_done.Enabled = false;
			}
			else
			{
				btn_create_done.Enabled = true;
			}
		}

        private void setTitleHint()
        {
            switch (currentType)
            {
                case MyBlahType.Asks:
                    newPostTitle.Hint = "HEADLINE: Asks are open-ended questions. Must include a '?'";
                    break;
                case MyBlahType.Leaks:
                    newPostTitle.Hint = "HEADLINE: Leaks require that a badge to be attached.";
                    break;
                case MyBlahType.Polls:
                    newPostTitle.Hint = "HEADLINE: Polls allow users to vote on pre-defined responses.";
                    break;
                case MyBlahType.Predicts:
                    newPostTitle.Hint = "HEADLINE: Predictions detail outcomes expected to occur.";
                    break;
                case MyBlahType.Says:
                    newPostTitle.Hint = "HEADLINE: Says are general posts, no requirements.";
                    break;
            }

        }

        private void setAsksCreateBlahType()
        {
            additionalfields_layout.Visibility = ViewStates.Gone;
            editPrediction.Visibility = ViewStates.Gone;
            editPoll1.Visibility = ViewStates.Gone;
            editPoll2.Visibility = ViewStates.Gone;
            editPoll3.Visibility = ViewStates.Gone;
            editPoll4.Visibility = ViewStates.Gone;
            editPoll5.Visibility = ViewStates.Gone;
            editPoll6.Visibility = ViewStates.Gone;
            editPoll7.Visibility = ViewStates.Gone;
            editPoll8.Visibility = ViewStates.Gone;
            editPoll9.Visibility = ViewStates.Gone;
            editPoll10.Visibility = ViewStates.Gone;
            btnAddOption.Visibility = ViewStates.Gone;
        }

        private void setPollCreateBlahType()
        {
            int count = BlahguaAPIObject.Current.CreateRecord.I.Count;

            additionalfields_layout.Visibility = ViewStates.Visible;
            editPrediction.Visibility = ViewStates.Gone;
            editPoll1.Visibility = ViewStates.Visible;
            editPoll2.Visibility = ViewStates.Visible;
            if(count > 2)
                editPoll3.Visibility = ViewStates.Visible;
            else
                editPoll3.Visibility = ViewStates.Gone;
            if (count > 3)
                editPoll4.Visibility = ViewStates.Visible;
            else
                editPoll4.Visibility = ViewStates.Gone;
            if (count > 4)
                editPoll5.Visibility = ViewStates.Visible;
            else
                editPoll5.Visibility = ViewStates.Gone;
            if (count > 5)
                editPoll6.Visibility = ViewStates.Visible;
            else
                editPoll6.Visibility = ViewStates.Gone;
            if (count > 6)
                editPoll7.Visibility = ViewStates.Visible;
            else
                editPoll7.Visibility = ViewStates.Gone;
            if (count > 7)
                editPoll8.Visibility = ViewStates.Visible;
            else
                editPoll8.Visibility = ViewStates.Gone;
            if (count > 8)
                editPoll9.Visibility = ViewStates.Visible;
            else
                editPoll9.Visibility = ViewStates.Gone;
            if (count > 9)
                editPoll10.Visibility = ViewStates.Visible;
            else
                editPoll10.Visibility = ViewStates.Gone;
            btnAddOption.Visibility = ViewStates.Visible;
        }

        private void setPredictCreateBlahType()
        {
            additionalfields_layout.Visibility = ViewStates.Visible;
            editPrediction.Visibility = ViewStates.Visible;
            editPoll1.Visibility = ViewStates.Gone;
            editPoll2.Visibility = ViewStates.Gone;
            editPoll3.Visibility = ViewStates.Gone;
            editPoll4.Visibility = ViewStates.Gone;
            editPoll5.Visibility = ViewStates.Gone;
            editPoll6.Visibility = ViewStates.Gone;
            editPoll7.Visibility = ViewStates.Gone;
            editPoll8.Visibility = ViewStates.Gone;
            editPoll9.Visibility = ViewStates.Gone;
            editPoll10.Visibility = ViewStates.Gone;
            btnAddOption.Visibility = ViewStates.Gone;
        }
        


		private void triggerCreateBlock()
        {
			BlahguaAPIObject.Current.CreateRecord = new BlahCreateRecord();
            // reset fields state
            newPostTitle.Text = "";
            newPostText.Text = "";
            ClearImages();
        }

        


        private ValueAnimator slideAnimator(View layout, int start, int end, bool animatingWidth)
        {
            ValueAnimator animator = ValueAnimator.OfInt(start, end);
            //ValueAnimator animator2 = ValueAnimator.OfInt(start, end);
            //animator.AddUpdateListener (new ValueAnimator.IAnimatorUpdateListener{
            animator.Update +=
                (object sender, ValueAnimator.AnimatorUpdateEventArgs e) =>
                {
                    //int newValue = (int)
                    //e.Animation.AnimatedValue; // Apply this new value to the object being animated.
                    //myObj.SomeIntegerValue = newValue; 
                    var value = (int)animator.AnimatedValue;
                    ViewGroup.LayoutParams layoutParams = layout.LayoutParameters;
                    if (animatingWidth)
                        layoutParams.Width = value;
                    else
                        layoutParams.Height = value;
                    layout.LayoutParameters = layoutParams;

                };
            //});
            return animator;
        }

        private void DoAddPollChoice(object sender, EventArgs e)
        {
            int count = BlahguaAPIObject.Current.CreateRecord.I.Count;

            if (count < 10)
            {
                BlahguaAPIObject.Current.CreateRecord.I.Add(new PollItem("choice " + (count + 1)));
                MaybeEnableAddPollBtns();
                //triggerExpand();
            }
        }

        private void MaybeEnableAddPollBtns()
        {
            int count = BlahguaAPIObject.Current.CreateRecord.I.Count;

            if (count == 10)
            {
                btnAddOption.Visibility = ViewStates.Gone;
            }
            else
            {
                btnAddOption.Visibility = ViewStates.Visible;
            }

            switch (count)
            {
                case 10:
                    editPoll10.Visibility = ViewStates.Visible;
                    break;
                case 9:
                    editPoll9.Visibility = ViewStates.Visible;
                    break;
                case 8:
                    editPoll8.Visibility = ViewStates.Visible;
                    break;
                case 7:
                    editPoll7.Visibility = ViewStates.Visible;
                    break;
                case 6:
                    editPoll6.Visibility = ViewStates.Visible;
                    break;
                case 5:
                    editPoll5.Visibility = ViewStates.Visible;
                    break;
                case 4:
                    editPoll4.Visibility = ViewStates.Visible;
                    break;
                case 3:
                    editPoll3.Visibility = ViewStates.Visible;
                    break;
            }
        }

        private void fillPoll()
        {
            for (int i = 0; i < BlahguaAPIObject.Current.CreateRecord.I.Count; ++i)
            {
                PollItem pi = BlahguaAPIObject.Current.CreateRecord.I[i];

                switch (i)
                {
                    case 0:
                        pi.G = editPoll1.Text;
                        break;
                    case 1:
                        pi.G = editPoll2.Text;
                        break;
                    case 2:
                        pi.G = editPoll3.Text;
                        break;
                    case 3:
                        pi.G = editPoll4.Text;
                        break;
                    case 4:
                        pi.G = editPoll5.Text;
                        break;
                    case 5:
                        pi.G = editPoll6.Text;
                        break;
                    case 6:
                        pi.G = editPoll7.Text;
                        break;
                    case 7:
                        pi.G = editPoll8.Text;
                        break;
                    case 8:
                        pi.G = editPoll9.Text;
                        break;
                    case 9:
                        pi.G = editPoll10.Text;
                        break;
                }
            }
        }

        private bool DoCreateClick()
        {
            //SelectedBadgesList.Focus();
            BlahguaAPIObject.Current.CreateRecord.T = newPostTitle.Text;
            BlahguaAPIObject.Current.CreateRecord.F = newPostText.Text;
            if (currentType == MyBlahType.Polls)
            {
                fillPoll();
            }
            string valStr = IsBlahValid();
            if (valStr == "")
            {
                BlahguaAPIObject.Current.CreateBlah(OnCreateBlahOK);
                return true;
            }
            else
            {
                Toast.MakeText(this, valStr, ToastLength.Short).Show();
                return false;
            }
        }

        private void OnCreateBlahOK(Blah newBlah)
        {
            if (newBlah != null)
            {
                BlahguaAPIObject.Current.NewBlahToInsert = newBlah;
                HomeActivity.analytics.PostCreateBlah(newBlah.Y);

                RunOnUiThread(() =>
	                {
	                    Toast.MakeText(this, "Blah posted", ToastLength.Short).Show();
						SetResult(Result.Ok, new Intent());
						Finish();
	                });
            }
            else
            {
                RunOnUiThread(() =>
                {
                    Toast.MakeText(this, "Unable to create the blah.  Please try again.  If the problem persists, please try at a different time.", ToastLength.Short).Show();
                });
				HomeActivity.analytics.PostFormatError("blah create failed");
            }
        }

        private string IsBlahValid()
        {
            BlahCreateRecord curBlah = BlahguaAPIObject.Current.CreateRecord;

            bool hasImage = ((curBlah.M != null) && (curBlah.M.Count > 0));

            if (String.IsNullOrEmpty(curBlah.T))
            {
                if (!hasImage)
                    return "Headline is too short for a post with no image (< 3 characters)";
            }
            else
            {
                if ((curBlah.T.Length < 3) && (!hasImage))
                    return "Headline is too short for a post with no image (< 3 characters)";

                if (curBlah.T.Length > 64)
                    return "Headline is too long (> 64 characters)";
            }

            if ((curBlah.F != null) && (curBlah.F.Length > 2000))
                return "Body text is too long (> 2000 characters)";

            // type restrictions
            switch (curBlah.BlahType.N)
            {
                case "leaks":
                    if ((curBlah.B == null) || (curBlah.B.Count == 0))
                        return "Leaks must be badged.";
                    break;

                case "asks":
                    if (curBlah.T.IndexOf("?") == -1)
                        return "Asks must contain a question.";
                    break;

                case "polls":
                    if ((curBlah.I == null) || (curBlah.I.Count < 2))
                        return "Polls require at least two choices.";

                    foreach (PollItem curItem in curBlah.I)
                    {
                        if ((curItem.G == null) || (curItem.G.Length == 0))
                            return "Each poll response requires a title.";
                    }
                    break;

                case "predicts":
                    if (curBlah.E <= DateTime.Now.AddDays(1))
                        return "Predictions must be at least a day in the future.";

                    break;
            }

            return "";
        }


        #region Prediction DatePickerCalendar
        
        Calendar myCalendar = Calendar.Instance;
        private void setDatePicker(EditText edit)
        {
            edit.Click += (sender, args) =>
            {
                EventHandler<DatePickerDialog.DateSetEventArgs> handle = (s, arg) =>
                {
                    myCalendar.Set(CalendarField.Year, arg.Year);
					myCalendar.Set(CalendarField.Month, arg.MonthOfYear);
					myCalendar.Set(CalendarField.DayOfMonth, arg.DayOfMonth);
                    updateLabel(edit);
                };
                new DatePickerDialog(this, handle, myCalendar
					.Get(CalendarField.Year), myCalendar.Get(CalendarField.Month),
					myCalendar.Get(CalendarField.DayOfMonth)).Show();
            };
        }

        private void updateLabel(EditText edit)
        {
            String myFormat = "MM/dd/yy"; //In which you need put here
            SimpleDateFormat sdf = new SimpleDateFormat(myFormat, Locale.Us);

            edit.Text = sdf.Format(myCalendar.Time);
        }
			
		private void MultiListClicked(object sender, DialogMultiChoiceClickEventArgs args)
		{
			if (args.Which == 0)
				BlahguaAPIObject.Current.CreateRecord.XX = !args.IsChecked;
			else if (args.Which == 1)
				BlahguaAPIObject.Current.CreateRecord.XXX = args.IsChecked;
			else {
				int whichBadge = args.Which - 2;
				BadgeRecord badgeId = BlahguaAPIObject.Current.CurrentUser.B [whichBadge];
				if (args.IsChecked) {
					// add badge
					if (BlahguaAPIObject.Current.CreateRecord.B == null)
						BlahguaAPIObject.Current.CreateRecord.B = new List<BadgeRecord> ();
					BlahguaAPIObject.Current.CreateRecord.B.Add (badgeId);
				} else {
					BlahguaAPIObject.Current.CreateRecord.B.Remove (badgeId);
				}
			}
		}


		private void BadgeOKClicked(Object sender, EventArgs args)
		{
			//Toast.MakeText(this, "Badge accepted!", ToastLength.Short).Show();
		}

		protected override Dialog OnCreateDialog(int id, Bundle args)
		{
			switch(id)
			{
			case MultiChoiceDialog: 
				{
					UpdateBadgeInfo ();
					var builder = new AlertDialog.Builder (this, Android.App.AlertDialog.ThemeHoloLight);
					builder.SetIcon (Resource.Drawable.ic_launcher);
					builder.SetTitle ("Sign your post");
					builder.SetMultiChoiceItems (badgeItemNames, badgeItemBools, MultiListClicked);
					builder.SetPositiveButton ("Ok", BadgeOKClicked);

					AlertDialog dlg = builder.Create ();

					return dlg;
				}
				break;
			}
			return base.OnCreateDialog(id, args);
		}

		private void UpdateBadgeInfo()
		{
			List<BadgeRecord> badges = BlahguaAPIObject.Current.CurrentUser.B;

			if (badgeItemNames == null) {
				List<string>	badgeNames = new List<string> ();
				badgeNames.Add ("use profile");
				badgeNames.Add ("mature content");

				if (badges != null) {
					foreach (BadgeRecord curBadge in badges) {
						badgeNames.Add (curBadge.N);
					}
				}
				badgeItemNames = badgeNames.ToArray ();
			}

			// now create the bool list
			badgeItemBools = new bool[badgeItemNames.Length];

			badgeItemBools [0] = !BlahguaAPIObject.Current.CreateRecord.XX;
			badgeItemBools [1] = BlahguaAPIObject.Current.CreateRecord.XXX;

			if (badges != null) {
				int i = 2;
				if (BlahguaAPIObject.Current.CreateRecord.B == null) {
					foreach (BadgeRecord curBadge in badges) {
						badgeItemBools [i++] = false;
					}
				} else {
					foreach (BadgeRecord curBadge in badges) {
						badgeItemBools [i++] = BlahguaAPIObject.Current.CreateRecord.B.Contains (curBadge);
					}
				}
			}

		}


        #endregion
    }
}