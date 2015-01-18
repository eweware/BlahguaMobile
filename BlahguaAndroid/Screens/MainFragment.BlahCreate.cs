using System;
using System.Linq;

using Android.Content;
using Android.Views;
using Android.Widget;
using Android.Animation;
using Android.OS;

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

namespace BlahguaMobile.AndroidClient.Screens
{
    public partial class MainFragment : IUrlImageViewCallback, View.IOnTouchListener
    {
        enum MyBlahType
        {
            Asks, Leaks, Polls, Predicts, Says
        }

        private MyBlahType currentType = MyBlahType.Says;

        private readonly int SELECTIMAGE_REQUEST = 777;
        private View create_post_block, additionalfields_layout;
        private EditText newPostTitle, newPostText;
        private EditText editPrediction, editPoll1, editPoll2, editPoll3, editPoll4, editPoll5, editPoll6, editPoll7, editPoll8, editPoll9, editPoll10;
        private Button btnAddOption;
        private Button btn_create_done;

        private FrameLayout imageCreateBlahLayout;
        private ImageView imageCreateBlah;
        private ImageView imageSay, imagePredict, imagePoll, imageAsk, imageLeak;
        private ProgressBar progressBarImageLoading;
        private ImageView currentSpeechAct;

        private Uri fileUri;
        public static int MEDIA_TYPE_IMAGE = 1;
        public static int MEDIA_TYPE_VIDEO = 2;


        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            if (requestCode == SELECTIMAGE_REQUEST && resultCode == (int)Android.App.Result.Ok)
            {
                progressBarImageLoading.Visibility = ViewStates.Visible;
                imageCreateBlahLayout.Visibility = ViewStates.Visible;
                imageCreateBlah.SetImageDrawable(null);

                System.IO.Stream fileStream = StreamHelper.GetStreamFromFileUri(this.Activity, data.Data);
                String fileName = StreamHelper.GetFileName(this.Activity, data.Data);
                if (fileStream != null)
                {
                    BlahguaAPIObject.Current.UploadPhoto(fileStream, fileName, (photoString) =>
                    {
                        this.Activity.RunOnUiThread(() =>
                        {
                            if ((photoString != null) && (photoString.Length > 0))
                            {
                                string photoURL = BlahguaAPIObject.Current.GetImageURL(photoString, "B");
                                imageCreateBlah.SetUrlDrawable(photoURL, this);
                                BlahguaAPIObject.Current.CreateRecord.M = new List<string>();
                                BlahguaAPIObject.Current.CreateRecord.M.Add(photoString);

                            }
                            else
                            {
                                ClearImages();
                                //App.analytics.PostSessionError("blahimageuploadfailed");
                            }
                        });
                    }
                    );
                }
            }
            base.OnActivityResult(requestCode, resultCode, data);
        }

        

		private void initCreateBlahUi()
		{
			// speech act btns
			imageSay = fragment.FindViewById<View>(Resource.Id.btn_speechact_say) as ImageView;
			imagePredict = fragment.FindViewById<View>(Resource.Id.btn_speechact_predict) as ImageView;
			imagePoll = fragment.FindViewById<View>(Resource.Id.btn_speechact_poll) as ImageView;
			imageAsk = fragment.FindViewById<View>(Resource.Id.btn_speechact_ask) as ImageView;
			imageLeak = fragment.FindViewById<View>(Resource.Id.btn_speechact_leak) as ImageView;

			imageSay.Click += SpeechActBtn_Click;
			imagePredict.Click += SpeechActBtn_Click;
			imagePoll.Click += SpeechActBtn_Click;
			imageAsk.Click += SpeechActBtn_Click;
			imageLeak.Click += SpeechActBtn_Click;
			currentSpeechAct = imageSay;

			blayGrayed = fragment.FindViewById<View>(Resource.Id.BlahGrayed);
			blayGrayed.Visibility = ViewStates.Gone;
			blayGrayed.SetOnTouchListener(this);
			additionalfields_layout = fragment.FindViewById<View>(Resource.Id.additionalfields_layout);
			create_post_block = fragment.FindViewById<View>(Resource.Id.create_post_block);
			/*
			btn_newpost.Click += (sender, args) =>
			{
				triggerCreateBlock();
			};
			*/
			Button btn_select_image = create_post_block.FindViewById<Button>(Resource.Id.btn_image);
			btn_select_image.Click += (sender, args) =>
			{
				var imageIntent = new Intent();
                imageIntent = new Intent(Intent.ActionGetContent, Android.Provider.MediaStore.Images.Media.ExternalContentUri);
                imageIntent.SetType("image/*");
                //fileUri = getOutputMediaFileUri(MEDIA_TYPE_IMAGE);
                StartActivityForResult(Intent.CreateChooser(imageIntent, "Select Image"), 777);
                /*
				imageIntent.SetType("image/*");
				imageIntent.SetAction(Intent,A);

				StartActivityForResult(
					Intent.CreateChooser(imageIntent, "Select image"), SELECTIMAGE_REQUEST);
                 */
			};
			Button btn_signature = create_post_block.FindViewById<Button>(Resource.Id.btn_signature);
			btn_signature.Click += (sender, args) => {
				//initiateSignaturePopUp();
			};
			btn_create_done = create_post_block.FindViewById<Button>(Resource.Id.btn_done);
			btn_create_done.Click += (sender, args) =>
			{
				if (DoCreateClick())
				{
					triggerCreateBlock();
				}
			};
			btn_create_done.Enabled = false;
			newPostTitle = create_post_block.FindViewById<EditText>(Resource.Id.title);
			newPostTitle.TextChanged += editCreate_TextChanged;
			newPostText = create_post_block.FindViewById<EditText>(Resource.Id.text);

			imageCreateBlah = create_post_block.FindViewById<ImageView>(Resource.Id.createblah_image);
			imageCreateBlahLayout = create_post_block.FindViewById<FrameLayout>(Resource.Id.createblah_image_layout);
			progressBarImageLoading = create_post_block.FindViewById<ProgressBar>(Resource.Id.progress_image_loading);
			progressBarImageLoading.Visibility = ViewStates.Gone;

			editPrediction = create_post_block.FindViewById<EditText>(Resource.Id.prediction);
			editPrediction.TextChanged += editCreate_TextChanged;
			editPrediction.Focusable = false;
			setDatePicker(editPrediction);

			editPoll1 = create_post_block.FindViewById<EditText>(Resource.Id.poll1);
			editPoll1.TextChanged += editCreate_TextChanged;
			editPoll2 = create_post_block.FindViewById<EditText>(Resource.Id.poll2);
			editPoll2.TextChanged += editCreate_TextChanged;
			editPoll3 = create_post_block.FindViewById<EditText>(Resource.Id.poll3);
			editPoll4 = create_post_block.FindViewById<EditText>(Resource.Id.poll4);
			editPoll5 = create_post_block.FindViewById<EditText>(Resource.Id.poll5);
			editPoll6 = create_post_block.FindViewById<EditText>(Resource.Id.poll6);
			editPoll7 = create_post_block.FindViewById<EditText>(Resource.Id.poll7);
			editPoll8 = create_post_block.FindViewById<EditText>(Resource.Id.poll8);
			editPoll9 = create_post_block.FindViewById<EditText>(Resource.Id.poll9);
			editPoll10 = create_post_block.FindViewById<EditText>(Resource.Id.poll10);
			btnAddOption = create_post_block.FindViewById<Button>(Resource.Id.btn_add_poll_option);
			btnAddOption.Click += DoAddPollChoice;

			setAsksCreateBlahType();
		}

      


        public void OnLoaded(ImageView imageView, Android.Graphics.Drawables.Drawable loadedDrawable, string url, bool loadedFromCache)
        {
            if (imageCreateBlah == imageView)
            {
				this.Activity.RunOnUiThread(() =>
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
        }

        public bool OnTouch(View v, MotionEvent ev)
        {
            if (ev.Action == MotionEventActions.Up)
            {
                if (create_post_block.Visibility == ViewStates.Visible)
                {
                    triggerCreateBlock();
                }
            }
            return true;
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

                triggerExpand();
            }
        }

        private void DisableSpeechActBtn(ImageView someBtn)
        {
            if (someBtn == imageSay)
            {
				TextView    theText = fragment.FindViewById(Resource.Id.text_speechact_say) as TextView;
                imageSay.SetImageResource(Resource.Drawable.icon_speechact_say);
                theText.SetTextColor(new Android.Graphics.Color(63,43,47));
            }
            else if (someBtn == imagePredict)
            {
				TextView    theText = fragment.FindViewById(Resource.Id.text_speechact_predict) as TextView;
                imagePredict.SetImageResource(Resource.Drawable.icon_speechact_predict);
                theText.SetTextColor(new Android.Graphics.Color(63, 43, 47));
            }
            else if (someBtn == imagePoll)
            {
				TextView    theText = fragment.FindViewById(Resource.Id.text_speechact_poll) as TextView;
                imagePoll.SetImageResource(Resource.Drawable.icon_speechact_poll);
                theText.SetTextColor(new Android.Graphics.Color(63, 43, 47));
            }
            else if (someBtn == imageAsk)
            {
				TextView    theText = fragment.FindViewById(Resource.Id.text_speechact_ask) as TextView;
                imageAsk.SetImageResource(Resource.Drawable.icon_speechact_ask);
                theText.SetTextColor(new Android.Graphics.Color(63, 43, 47));
            }
            else if (someBtn == imageLeak)
            {
				TextView    theText = fragment.FindViewById(Resource.Id.text_speechact_leak) as TextView;
                imageLeak.SetImageResource(Resource.Drawable.icon_speechact_leak);
                theText.SetTextColor(new Android.Graphics.Color(63, 43, 47));
            }
            
        }

        private void EnableSpeechActBtn(ImageView someBtn)
        {
            if (someBtn == imageSay)
            {
				TextView theText = fragment.FindViewById(Resource.Id.text_speechact_say) as TextView;
                imageSay.SetImageResource(Resource.Drawable.icon_speechact_say_teal);
                theText.SetTextColor(new Android.Graphics.Color(96, 191, 164));
                BlahguaAPIObject.Current.CreateRecord.BlahType =
                    BlahguaAPIObject.Current.CurrentBlahTypes.First<BlahType>(n => n.N == "says");
                currentType = MyBlahType.Says;
                setAsksCreateBlahType();
            }
            else if (someBtn == imagePredict)
            {
				TextView theText =fragment. FindViewById(Resource.Id.text_speechact_predict) as TextView;
                imagePredict.SetImageResource(Resource.Drawable.icon_speechact_predict_teal);
                theText.SetTextColor(new Android.Graphics.Color(96, 191, 164));
                BlahguaAPIObject.Current.CreateRecord.BlahType =
                    BlahguaAPIObject.Current.CurrentBlahTypes.First<BlahType>(n => n.N == "predicts");
                currentType = MyBlahType.Predicts;
                setPredictCreateBlahType();
            }
            else if (someBtn == imagePoll)
            {
				TextView theText = fragment.FindViewById(Resource.Id.text_speechact_poll) as TextView;
                imagePoll.SetImageResource(Resource.Drawable.icon_speechact_poll_teal);
                theText.SetTextColor(new Android.Graphics.Color(96, 191, 164));
                BlahguaAPIObject.Current.CreateRecord.BlahType =
                    BlahguaAPIObject.Current.CurrentBlahTypes.First<BlahType>(n => n.N == "polls");
                currentType = MyBlahType.Polls;
                setPollCreateBlahType();
            }
            else if (someBtn == imageAsk)
            {
				TextView theText = fragment.FindViewById(Resource.Id.text_speechact_ask) as TextView;
                imageAsk.SetImageResource(Resource.Drawable.icon_speechact_ask_teal);
                theText.SetTextColor(new Android.Graphics.Color(96, 191, 164));
                BlahguaAPIObject.Current.CreateRecord.BlahType =
                    BlahguaAPIObject.Current.CurrentBlahTypes.First<BlahType>(n => n.N == "asks");
                currentType = MyBlahType.Asks;
                setAsksCreateBlahType();
            }
            else if (someBtn == imageLeak)
            {
				TextView theText = fragment.FindViewById(Resource.Id.text_speechact_leak) as TextView;
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
            if (newPostTitle.Visibility == ViewStates.Visible && newPostTitle.Text.Length == 0)
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

        private void initBlahCreationSlidingMenu()
        {
            //SetBehindContentView(Resource.Layout.sidemenu_blahcreate);

            String[] types = new String[] {
                                GetString(Resource.String.sidemenu_blahcreate_asks),
                                GetString(Resource.String.sidemenu_blahcreate_leaks),
                                GetString(Resource.String.sidemenu_blahcreate_polls),
                                GetString(Resource.String.sidemenu_blahcreate_predicts),
                                GetString(Resource.String.sidemenu_blahcreate_says) };
            
			homeActivity.LeftMenu.Adapter = new ArrayAdapter(homeActivity, Resource.Layout.listitem_check, types);
			homeActivity.LeftMenu.SetItemChecked(0, true);

			homeActivity.LeftMenu.ItemClick += listChannels_ItemClick;
        }

        private void listChannels_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            if (e.Position == 0) // asks
            {
                BlahguaAPIObject.Current.CreateRecord.BlahType =
                    BlahguaAPIObject.Current.CurrentBlahTypes.First<BlahType>(n => n.N == "asks");
                currentType = MyBlahType.Asks;
                setAsksCreateBlahType();
            }
            else if (e.Position == 1) // leaks
            {
                BlahguaAPIObject.Current.CreateRecord.BlahType =
                    BlahguaAPIObject.Current.CurrentBlahTypes.First<BlahType>(n => n.N == "leaks");
                currentType = MyBlahType.Leaks;
                setAsksCreateBlahType();
            }
            else if (e.Position == 2) // polls
            {
                BlahguaAPIObject.Current.CreateRecord.BlahType =
                    BlahguaAPIObject.Current.CurrentBlahTypes.First<BlahType>(n => n.N == "polls");
                currentType = MyBlahType.Polls;
                setPollCreateBlahType();
            }
            else if (e.Position == 3) // predicts
            {
                BlahguaAPIObject.Current.CreateRecord.BlahType =
                    BlahguaAPIObject.Current.CurrentBlahTypes.First<BlahType>(n => n.N == "predicts");
                currentType = MyBlahType.Predicts;
                setPredictCreateBlahType();
            }
            else if (e.Position == 4) // says
            {
                BlahguaAPIObject.Current.CreateRecord.BlahType =
                    BlahguaAPIObject.Current.CurrentBlahTypes.First<BlahType>(n => n.N == "says");
                currentType = MyBlahType.Says;
                setAsksCreateBlahType();
            }
            triggerExpand();
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
        ///////// init

        private int lastCreateBlockHeight = 0;
        private void triggerExpand()
        {
            //set Visible
            create_post_block.Visibility = ViewStates.Visible;
            int widthSpec = View.MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified);
            int heightSpec = View.MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified);
            ViewGroup.LayoutParams layoutParams = create_post_block.LayoutParameters;
            layoutParams.Height = ViewGroup.LayoutParams.WrapContent ;
            create_post_block.LayoutParameters = layoutParams;
            create_post_block.Measure(widthSpec, heightSpec);
            int newHeight = create_post_block.MeasuredHeight;// +(create_post_block.Width > 800 ? 0 : 50);

            if (newHeight < Resources.DisplayMetrics.HeightPixels)
            {
                ValueAnimator mAnimator = slideAnimator(create_post_block, lastCreateBlockHeight, newHeight, false);
                lastCreateBlockHeight = create_post_block.MeasuredHeight;
                mAnimator.Start();
            }
            else
            {
                layoutParams.Height = ViewGroup.LayoutParams.MatchParent;
                create_post_block.LayoutParameters = layoutParams;
            }

            setTitleHint();

            StopTimers();
        }

        private View blayGrayed;
        public void triggerCreateBlock()
        {
            if (create_post_block.Visibility.Equals(ViewStates.Gone))
            {
                //SlidingMenu.TouchModeAbove = TouchMode.None;
                blayGrayed.Visibility = ViewStates.Visible;
                BlahguaAPIObject.Current.CreateRecord = new BlahCreateRecord();
                // reset fields state
                newPostTitle.Text = "";
                newPostText.Text = "";
                ClearImages();

                triggerExpand();
                //SlidingMenu.Enabled = false;

                //initBlahCreationSlidingMenu();
            }
            else
            {
                // inser the blah into the stream
                if (BlahguaAPIObject.Current.NewBlahToInsert != null)
                {
                    InsertBlahInStream(BlahguaAPIObject.Current.NewBlahToInsert);
                    BlahguaAPIObject.Current.NewBlahToInsert = null;

                }

                //SlidingMenu.TouchModeAbove = TouchMode.Margin;
                //collapse
                blayGrayed.Visibility = ViewStates.Gone;
                int finalHeight = create_post_block.Height;

                ValueAnimator mAnimator = slideAnimator(create_post_block, finalHeight, 0, false);
                mAnimator.Start();
                mAnimator.AnimationEnd += (object IntentSender, EventArgs arg) =>
                {
                    create_post_block.Visibility = ViewStates.Gone;

                    // reset views
                    if (imageSay != currentSpeechAct)
                    {
                        DisableSpeechActBtn(currentSpeechAct);
                        EnableSpeechActBtn(imageSay);
                        currentSpeechAct = imageSay;
                        setTitleHint();
                    }
                    imageCreateBlahLayout.Visibility = ViewStates.Gone;
                    editPrediction.Text = newPostTitle.Text = newPostText.Text =
                    editPoll1.Text = editPoll2.Text = editPoll3.Text = editPoll4.Text =
                    editPoll5.Text = editPoll6.Text = editPoll7.Text = editPoll8.Text =
                    editPoll9.Text = editPoll10.Text = "";
                    editPoll3.Visibility = editPoll4.Visibility = editPoll5.Visibility = editPoll6.Visibility =
                    editPoll7.Visibility = editPoll8.Visibility = editPoll9.Visibility = editPoll10.Visibility =
                    ViewStates.Gone;
                };

                //initSlidingMenu();
                if (BlahguaAPIObject.Current.CurrentUser != null)
                {
                    //SlidingMenu.Mode = MenuMode.LeftRight;
                }

                lastCreateBlockHeight = 0;
                StartTimers();

				if (BlahguaAPIObject.Current.CurrentChannelList != null) {
					String[] channels = new String[BlahguaAPIObject.Current.CurrentChannelList.Count];
					int channelIndex = 0;

					foreach (Channel curChannel in BlahguaAPIObject.Current.CurrentChannelList) {
						channels [channelIndex++] = curChannel.ChannelName;
					}
					//Sections = channels;
					//Create Adapter for drawer List
					homeActivity.LeftMenu.Adapter = new ArrayAdapter<string> (homeActivity, Resource.Layout.item_menu, channels);
				}
            }
        }

        void InsertBlahInStream(Blah theBlah)
        {
            View control = null;
            double scrollTop = BlahScroller.ScrollY + BlahScroller.Height;

            for (int i = 0; i < CurrentBlahContainer.ChildCount; i++)
            {
                control = CurrentBlahContainer.GetChildAt(i);
                if ((control.Top > scrollTop) &&
                    (control.Height == mediumBlahSize) &&
                    (control.Width == mediumBlahSize))
                    break;
            }

            if (control != null)
            {
                // conform to view
                var title = control.FindViewById<TextView>(Resource.Id.title);

                if (String.IsNullOrEmpty(theBlah.T))
                    control.FindViewById<LinearLayout>(Resource.Id.textLayout).Visibility = ViewStates.Invisible;
                else
                {
                    double width = control.Width;
                    double height = control.Height;
                    control.FindViewById<LinearLayout>(Resource.Id.textLayout).Visibility = ViewStates.Visible;

                    title.Text = theBlah.T;

                    if (width == smallBlahSize && height == smallBlahSize)
                    {
                        title.SetTextSize(Android.Util.ComplexUnitType.Sp, 14);
                    }
                    else if (width == mediumBlahSize && height == smallBlahSize)
                    {
                        title.SetTextSize(Android.Util.ComplexUnitType.Sp, 18);
                    }
                    else if (width == mediumBlahSize && height == mediumBlahSize)
                    {
                        title.SetTextSize(Android.Util.ComplexUnitType.Sp, 24);
                    }
                    else if (width == largeBlahSize && height == mediumBlahSize)
                    {
                        title.SetTextSize(Android.Util.ComplexUnitType.Sp, 32);
                    }
                }

                ImageView image = control.FindViewById<ImageView>(Resource.Id.image);
                image.Tag = null;
                if (theBlah.M != null)
                {
                    image.Visibility = ViewStates.Visible;
                    string imageBase = theBlah.M[0];
                    string imageSize = "B";
                    string imageURL = BlahguaAPIObject.Current.GetImageURL(imageBase, imageSize);
                    homeActivity.RunOnUiThread(() =>
                    {
                        image.SetUrlDrawable(imageURL);
                        if (!String.IsNullOrEmpty(theBlah.T))
                        {
                            image.Tag = true;   // animate this
                            control.FindViewById<LinearLayout>(Resource.Id.textLayout).Alpha = 0.9f;
                        }
                    });
                }
                else
                    image.Visibility = ViewStates.Invisible;


                ///////
				homeActivity.RunOnUiThread(() =>
                {
                    var type_mark = control.FindViewById<View>(Resource.Id.type_mark);
                    var badges_mark = control.FindViewById<View>(Resource.Id.badges_mark);
                    var hot_mark = control.FindViewById<View>(Resource.Id.hot_mark);
                    var new_mark = control.FindViewById<View>(Resource.Id.new_mark);
                    var user_mark = control.FindViewById<View>(Resource.Id.user_mark);

                    switch (theBlah.TypeName)
                    {
                        case "says":
                            type_mark.SetBackgroundResource(Resource.Drawable.say_icon);
                            break;
                        case "asks":
                            type_mark.SetBackgroundResource(Resource.Drawable.ask_icon);
                            break;
                        case "leaks":
                            type_mark.SetBackgroundResource(Resource.Drawable.leak_icon);
                            break;
                        case "polls":
                            type_mark.SetBackgroundResource(Resource.Drawable.poll_icon);
                            break;
                        case "predicts":
                            type_mark.SetBackgroundResource(Resource.Drawable.predict_icon);
                            break;
                    }

                    // icons
                    new_mark.Visibility = ViewStates.Visible;
                    hot_mark.Visibility = ViewStates.Gone;

                    if ((theBlah.B == null) || (theBlah.B.Count == 0))
                        badges_mark.Visibility = ViewStates.Gone;
                    else
                        badges_mark.Visibility = ViewStates.Visible;

                    user_mark.Visibility = ViewStates.Visible;
                });
                InboxBlah inboxItem = new InboxBlah(theBlah);
                control.Click += delegate
                {
                    OpenBlahItem(inboxItem);
                };
            }

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
                triggerExpand();
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
				Toast.MakeText(homeActivity, valStr, ToastLength.Short).Show();
                return false;
            }
        }

        private void OnCreateBlahOK(Blah newBlah)
        {
            if (newBlah != null)
            {
                BlahguaAPIObject.Current.NewBlahToInsert = newBlah;
				HomeActivity.analytics.PostCreateBlah(newBlah.Y);

				homeActivity.RunOnUiThread(() =>
                {
						Toast.MakeText(homeActivity, "Blah posted", ToastLength.Short).Show();
                    triggerCreateBlock();
                });
                //NavigationService.GoBack();
            }
            else
            {
				homeActivity.RunOnUiThread(() =>
                {
						Toast.MakeText(homeActivity, "Unable to create the blah.  Please try again.  If the problem persists, please try at a different time.", ToastLength.Short).Show();
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
                    if ((curBlah.ExpirationDate == null) || (curBlah.ExpirationDate <= DateTime.Now.AddDays(1)))
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
                    myCalendar.Set(Calendar.Year, arg.Year);
                    myCalendar.Set(Calendar.Month, arg.MonthOfYear);
                    myCalendar.Set(Calendar.DayOfMonth, arg.DayOfMonth);
                    updateLabel(edit);
                };
				new DatePickerDialog(homeActivity, handle, myCalendar
                        .Get(Calendar.Year), myCalendar.Get(Calendar.Month),
                        myCalendar.Get(Calendar.DayOfMonth)).Show();
            };
        }

        private void updateLabel(EditText edit)
        {
            String myFormat = "MM/dd/yy"; //In which you need put here
            SimpleDateFormat sdf = new SimpleDateFormat(myFormat, Locale.Us);

            edit.Text = sdf.Format(myCalendar.Time);
        }
        #endregion
    }
}