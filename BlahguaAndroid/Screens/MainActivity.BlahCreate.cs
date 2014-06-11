using System;
using System.Linq;

using Android.Content;
using Android.Views;
using Android.Widget;
using Android.Animation;

using BlahguaMobile.BlahguaCore;
using SlidingMenuSharp;

namespace BlahguaMobile.AndroidClient.Screens
{
    public partial class MainActivity
    {
        private View create_post_block, additionalfields_layout;
        private EditText newPostTitle, newPostText;
        private EditText editPrediction, editPoll1, editPoll2, editPoll3, editPoll4, editPoll5, editPoll6, editPoll7, editPoll8, editPoll9, editPoll10;
        private Button btnAddOption;
        ///////// init
        private void initCreateBlahUi()
        {
            additionalfields_layout = FindViewById<View>(Resource.Id.additionalfields_layout);
            create_post_block = FindViewById<View>(Resource.Id.create_post_block);
            btn_newpost.Click += (sender, args) =>
            {
                triggerCreateBlock();
            };
            Button btn_select_image = create_post_block.FindViewById<Button>(Resource.Id.btn_image);
            btn_select_image.Click += (sender, args) =>
            {
                var imageIntent = new Intent();
                imageIntent.SetType("image/*");
                imageIntent.SetAction(Intent.ActionGetContent);
                StartActivityForResult(
                    Intent.CreateChooser(imageIntent, "Select image"), 0);
            };
            Button btn_done = create_post_block.FindViewById<Button>(Resource.Id.btn_done);
            btn_done.Click += (sender, args) =>
            {
                if (DoCreateClick())
                {
                    triggerCreateBlock();
                }
            };
            newPostTitle = create_post_block.FindViewById<EditText>(Resource.Id.title);
            newPostText = create_post_block.FindViewById<EditText>(Resource.Id.text);

            editPrediction = create_post_block.FindViewById<EditText>(Resource.Id.prediction);
            editPoll1 = create_post_block.FindViewById<EditText>(Resource.Id.poll1);
            editPoll2 = create_post_block.FindViewById<EditText>(Resource.Id.poll2);
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

        private void initBlahCreationSlidingMenu()
        {
            SetBehindContentView(Resource.Layout.sidemenu_blahcreate);
            View leftMenu = SlidingMenu.GetMenu();

            String[] types = new String[] {
                                GetString(Resource.String.sidemenu_blahcreate_asks),
                                GetString(Resource.String.sidemenu_blahcreate_leaks),
                                GetString(Resource.String.sidemenu_blahcreate_polls),
                                GetString(Resource.String.sidemenu_blahcreate_predicts),
                                GetString(Resource.String.sidemenu_blahcreate_says) };
            ListView listChannels = leftMenu.FindViewById<ListView>(Resource.Id.list);
            listChannels.ChoiceMode = ChoiceMode.Single;
            listChannels.Adapter = new ArrayAdapter(this, Resource.Layout.listitem_check, types);
            listChannels.SetItemChecked(0, true);

            listChannels.ItemClick += listChannels_ItemClick;
        }

        enum MyBlahType
        {
            Asks, Leaks, Polls, Predicts, Says
        }
        MyBlahType currentType = MyBlahType.Asks;
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

        public void triggerCreateBlock()
        {
            if (create_post_block.Visibility.Equals(ViewStates.Gone))
            {
                //set Visible
                create_post_block.Visibility = ViewStates.Visible;
                int widthSpec = View.MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified);
                int heightSpec = View.MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified);
                create_post_block.Measure(widthSpec, heightSpec);

                BlahguaAPIObject.Current.CreateRecord = new BlahCreateRecord();

                ValueAnimator mAnimator = slideAnimator(create_post_block, 0, create_post_block.MeasuredHeight, false);
                mAnimator.Start();

                initBlahCreationSlidingMenu();

                StopTimers();
            }
            else
            {
                //collapse();
                int finalHeight = create_post_block.Height;

                ValueAnimator mAnimator = slideAnimator(create_post_block, finalHeight, 0, false);
                mAnimator.Start();
                mAnimator.AnimationEnd += (object IntentSender, EventArgs arg) =>
                {
                    create_post_block.Visibility = ViewStates.Gone;
                };

                initSlidingMenu();
                if (BlahguaAPIObject.Current.CurrentUser != null)
                {
                    SlidingMenu.Mode = MenuMode.LeftRight;
                }

                StartTimers();
            }
        }

        private ValueAnimator slideAnimator(View layout, int start, int end, bool animatingWidth)
        {
            ValueAnimator animator = ValueAnimator.OfInt(start, end);
            //ValueAnimator animator2 = ValueAnimator.OfInt(start, end);
            //  animator.AddUpdateListener (new ValueAnimator.IAnimatorUpdateListener{
            animator.Update +=
                (object sender, ValueAnimator.AnimatorUpdateEventArgs e) =>
                {
                    //  int newValue = (int)
                    //e.Animation.AnimatedValue; // Apply this new value to the object being animated.
                    //  myObj.SomeIntegerValue = newValue; 
                    var value = (int)animator.AnimatedValue;
                    ViewGroup.LayoutParams layoutParams = layout.LayoutParameters;
                    if (animatingWidth)
                        layoutParams.Width = value;
                    else
                        layoutParams.Height = value;
                    layout.LayoutParameters = layoutParams;

                };


            //      });
            return animator;
        }

        private void DoAddPollChoice(object sender, EventArgs e)
        {
            int count = BlahguaAPIObject.Current.CreateRecord.I.Count;

            if (count < 10)
            {
                MaybeEnableAddPollBtns();
                BlahguaAPIObject.Current.CreateRecord.I.Add(new PollItem("choice " + (count + 1)));
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
                case 9:
                    editPoll10.Visibility = ViewStates.Visible;
                    break;
                case 8:
                    editPoll9.Visibility = ViewStates.Visible;
                    break;
                case 7:
                    editPoll8.Visibility = ViewStates.Visible;
                    break;
                case 6:
                    editPoll7.Visibility = ViewStates.Visible;
                    break;
                case 5:
                    editPoll6.Visibility = ViewStates.Visible;
                    break;
                case 4:
                    editPoll5.Visibility = ViewStates.Visible;
                    break;
                case 3:
                    editPoll4.Visibility = ViewStates.Visible;
                    break;
                case 2:
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
                //App.analytics.PostCreateBlah(newBlah.Y);

                Toast.MakeText(this, "Blah posted", ToastLength.Short).Show();
                triggerCreateBlock();
                //NavigationService.GoBack();
            }
            else
            {
                Toast.MakeText(this, "Unable to create the blah.  Please try again.  If the problem persists, please try at a different time.", ToastLength.Short).Show();
                //MessageBox.Show("Unable to create the blah.  Please try again.  If the problem persists, please try at a different time.");
                //App.analytics.PostFormatError("blah create failed");

            }
        }

        private string IsBlahValid()
        {
            BlahCreateRecord curBlah = BlahguaAPIObject.Current.CreateRecord;

            bool hasImage = ((curBlah.M != null) && (curBlah.M.Count > 0));

            if (curBlah.T == null)
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
                        return "Asks must contain a question mark.";
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
    }
}