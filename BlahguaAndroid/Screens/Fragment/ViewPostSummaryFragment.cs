using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using BlahguaMobile.BlahguaCore;

using BlahguaMobile.AndroidClient.ThirdParty.UrlImageViewHelper;
using BlahguaMobile.AndroidClient.Adapters;
using BlahguaMobile.AndroidClient.HelpingClasses;
using Android.Graphics.Drawables;
using Android.Graphics;

namespace BlahguaMobile.AndroidClient.Screens
{
    class ViewPostSummaryFragment : Fragment
    {
        public static ViewPostSummaryFragment NewInstance()
        {
            return new ViewPostSummaryFragment { Arguments = new Bundle() };
        }

        private readonly string TAG = "ViewPostSummaryFragment";

        private Activity parent = null;

        // main block
        private ImageView image;
        private TextView titleView, textView;
        private ProgressDialog dialog;

        // author block
        private TextView author;
        private ImageView authorAvatar;
        private LinearLayout authorBadgesArea;

        // predicts layout
        private ListView predictsVotes;
        private TextView predictsDatebox;
        private TextView predictsElapsedtime;
        private LinearLayout predictsLayout;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            parent = (Activity)inflater.Context;
            View fragment = inflater.Inflate(Resource.Layout.fragment_viewpost_summary, null);

            dialog = new ProgressDialog(parent);
            dialog.SetMessage("Please wait...");
            dialog.SetCancelable(false);

            textView = fragment.FindViewById<TextView>(Resource.Id.text);
            titleView = fragment.FindViewById<TextView>(Resource.Id.title);
			image = fragment.FindViewById<ImageView>(Resource.Id.blah_image);
            image.Click += (sender, args) => {

                var intent = new Intent(Activity, typeof(ImageViewActivity));
                intent.PutExtra("image", BlahguaAPIObject.Current.CurrentBlah.ImageURL);
                StartActivity(intent);
            };

            author = fragment.FindViewById<TextView>(Resource.Id.author);
            authorAvatar = fragment.FindViewById<ImageView>(Resource.Id.author_avatar);
            authorBadgesArea = fragment.FindViewById<LinearLayout>(Resource.Id.badges_block);

            predictsVotes = fragment.FindViewById<ListView>(Resource.Id.predicts_votes);
            predictsDatebox = fragment.FindViewById<TextView>(Resource.Id.predicts_datebox);
            predictsElapsedtime = fragment.FindViewById<TextView>(Resource.Id.predicts_elapsedtime);
            predictsLayout = fragment.FindViewById<LinearLayout>(Resource.Id.predicts_layout);

            return fragment;
        }

        public override void OnStart()
        {
            base.OnStart();

            dialog.Show();
            BlahguaAPIObject.Current.SetCurrentBlahFromId(App.BlahIdToOpen, (theBlah) =>
            {
                parent.RunOnUiThread(() =>
                {
                    dialog.Hide();
                });

                //this.DataContext = BlahguaAPIObject.Current;
                if (BlahguaAPIObject.Current.CurrentBlah != null)
                {
                    if (theBlah.ImageURL != null && image != null)
                    {
						//image.Visibility = ViewStates.Visible;
                        //parent.RunOnUiThread(() =>
                        //{
                        //    image.SetUrlDrawable(theBlah.ImageURL);
                        //});

                        ImageLoader.Instance.DownloadAsync(theBlah.ImageURL,
                            image, (b) =>
                            {
                                parent.RunOnUiThread(() =>
                                {
                                    image.Visibility = ViewStates.Visible;
                                    image.SetImageBitmap(b);
                                });
                                return true;
                            });
                    }
					else 
                    {
						image.Visibility = ViewStates.Gone;
					}

                    parent.RunOnUiThread(() =>
                    {
                        titleView.SetText(theBlah.T, TextView.BufferType.Normal);
                        textView.SetText(theBlah.F, TextView.BufferType.Normal);
                    });

                    // author
                    parent.RunOnUiThread(() =>
                    {
                        author.Text = theBlah.UserName;
                        authorAvatar.SetUrlDrawable(theBlah.UserImage);
                        if (theBlah.Badges != null)
                        {
                            foreach (BadgeReference b in theBlah.Badges)
                            {
                                var view = Activity.LayoutInflater.Inflate(
                                                     Resource.Layout.uiitem_badge_small, authorBadgesArea, false);
                                var badgeImage = view.FindViewById<ImageView>(Resource.Id.image);
                                badgeImage.SetUrlDrawable(b.BadgeImage);
                            }
                        }
                    });

                    //BlahSummaryArea.Visibility = Visibility.Visible;
                    //UpdateButtonsForPage();
                    string toastMessage = String.Empty;
                    switch (BlahguaAPIObject.Current.CurrentBlah.TypeName)
                    {
                        case "polls":
                            toastMessage = "This is a Poll";
                            HandlePollInit();
                            break;
                        case "predicts":
                            toastMessage = "This is a Prediction";
                            HandlePredictInit();
                            break;
                        case "says":
                            toastMessage = "This is a Says";
                            break;
                        case "asks":
                            toastMessage = "This is an Asks";
                            break;
                        case "leaks":
                            toastMessage = "This is a Leak";
                            break;
                    }
                    parent.RunOnUiThread(() =>
                    {
                        Toast.MakeText(Activity, toastMessage, ToastLength.Short).Show();
                    });
                }
                else
                {
                    Toast.MakeText(parent, "unable to load blah.  Sorry!", ToastLength.Long).Show();
                    MainActivity.analytics.PostSessionError("loadblahfailed");
                    // Finish();
                }
            });
        }


        #region Handles

        private void HandlePollInit()
        {
            BlahguaAPIObject.Current.GetUserPollVote((theVote) =>
            {
                if ((theVote != null) && (theVote.W > -1))
                {
                    //PollItemList.ItemTemplate = (DataTemplate)Resources["PollVotedTemplate"];
                }
                //((Storyboard)Resources["ShowPollAnimation"]).Begin();
            }
            );
        }

        private void HandlePredictInit()
        {
            parent.RunOnUiThread(() =>
            {
                predictsLayout.Visibility = ViewStates.Visible;
            });
            BlahguaAPIObject.Current.GetUserPredictionVote((theVote) =>
            {
                Blah curBlah = BlahguaAPIObject.Current.CurrentBlah;
                parent.RunOnUiThread(() =>
                {
                    if (curBlah.ExpireDate > DateTime.Now)
                    {
                        // still has time
                        //PredictDateBox.Text = "happening by " + curBlah.E.ToShortDateString();
                        //PredictElapsedTimeBox.Text = "(" + Utilities.ElapsedDateString(curBlah.E) + ")";
                        bool isVoted = theVote != null && !String.IsNullOrEmpty(theVote.D);
                        predictsVotes.Adapter = new VotesAdapter(Activity, curBlah.PredictionItems, !isVoted, predictsTap);
								predictsDatebox.Text = "happening by " + curBlah.ExpireDate.ToShortDateString();
								predictsElapsedtime.Text = "(" + Utilities.ElapsedDateString(curBlah.ExpireDate) + ")";

                        //WillHappenItems.Visibility = Visibility.Visible;
                        //AlreadyHappenedItems.Visibility = Visibility.Collapsed;
                        //WillHappenItems.ItemsSource = curBlah.PredictionItems;
                    }
                    else
                    {
                        // expired
                        //PredictDateBox.Text = "should have happened on " + curBlah.E.ToShortDateString();
                        //PredictElapsedTimeBox.Text = "(" + Utilities.ElapsedDateString(curBlah.E) + ")";
                        bool isVoted = theVote != null && !String.IsNullOrEmpty(theVote.Z);
                        predictsVotes.Adapter = new VotesAdapter(Activity, curBlah.ExpPredictionItems, !isVoted, predictsTap);
								predictsDatebox.Text = "should have happened on " + curBlah.ExpireDate.ToShortDateString();
								predictsElapsedtime.Text = "(" + Utilities.ElapsedDateString(curBlah.ExpireDate) + ")";

                        //WillHappenItems.Visibility = Visibility.Visible;
                        //AlreadyHappenedItems.Visibility = Visibility.Collapsed;
                        //AlreadyHappenedItems.ItemsSource = curBlah.ExpPredictionItems;
                    }

                    HistoryUiHelper.setListViewHeightBasedOnChildren(predictsVotes);
                });

                //((Storyboard)Resources["ShowPredictionAnimation"]).Begin();
            }
            );
        }
        #endregion

        #region PollsTap
        bool predictsTapped = false;
        private void predictsTap(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            if (predictsTapped)
                return;
            VotesAdapter adapter = (VotesAdapter)predictsVotes.Adapter;
            int pos = (int)((CheckBox)sender).Tag;
            PollItem newVote = adapter.List[pos];
            predictsTapped = true;
            BlahguaAPIObject.Current.SetPredictionVote(newVote, (resultStr) =>
            {
                predictsTapped = newVote != null;
                HandlePredictInit();
                //adapter.NotifyDataSetChanged();
                //if (BlahguaAPIObject.Current.CurrentBlah.IsPredictionExpired)
                //{
                //    AlreadyHappenedItems.ItemsSource = BlahguaAPIObject.Current.CurrentBlah.ExpPredictionItems;
                //    AlreadyHappenedItems.ItemTemplate = (DataTemplate)Resources["PredictVotedTemplate"];
                //}
                //else
                //{
                //    WillHappenItems.ItemsSource = BlahguaAPIObject.Current.CurrentBlah.PredictionItems;
                //    WillHappenItems.ItemTemplate = (DataTemplate)Resources["PredictVotedTemplate"];
                //}

            }
            );
        }

        private void PollVote_Tap(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            VotesAdapter adapter = (VotesAdapter)predictsVotes.Adapter;
            int pos = (int)((CheckBox)sender).Tag;
            PollItem newVote = adapter.List[pos];

            BlahguaAPIObject.Current.SetPollVote(newVote, (resultStr) =>
            {
                adapter.NotifyDataSetChanged();
                //PollItemList.ItemTemplate = (DataTemplate)Resources["PollVotedTemplate"];
            }
            );
        }

        #endregion
    }
}