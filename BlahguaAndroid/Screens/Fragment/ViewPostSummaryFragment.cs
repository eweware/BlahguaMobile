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
            image = fragment.FindViewById<ImageView>(Resource.Id.image);

            author = fragment.FindViewById<TextView>(Resource.Id.author);
            authorAvatar = fragment.FindViewById<ImageView>(Resource.Id.author_avatar);
            authorBadgesArea = fragment.FindViewById<LinearLayout>(Resource.Id.badges_block);

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
                        //ImageSource defaultSrc = new BitmapImage(new Uri(defaultImg, UriKind.Relative));
                        //BackgroundImage.Source = defaultSrc;
                        //BackgroundImage2.Source = defaultSrc;


                        parent.RunOnUiThread(() =>
                        {
                            image.SetUrlDrawable(theBlah.ImageURL);
                            //image.SetImageURI(Android.Net.Uri.Parse(imageURL));
                        });
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
                    });

                    //BlahSummaryArea.Visibility = Visibility.Visible;
                    //UpdateButtonsForPage();
                    parent.RunOnUiThread(() =>
                    {
                        Toast.MakeText(Activity, "This is " + BlahguaAPIObject.Current.CurrentBlah.TypeName, ToastLength.Short).Show();
                    });
                    switch (BlahguaAPIObject.Current.CurrentBlah.TypeName)
                    {
                        case "polls":
                            HandlePollInit();
                            break;
                        case "predicts":
                            HandlePredictInit();
                            break;
                    }
                }
                else
                {
                    Toast.MakeText(parent, "unable to load blah.  Sorry!", ToastLength.Long).Show();
                    //App.analytics.PostSessionError("loadblahfailed");
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
                    if (curBlah.E > DateTime.Now)
                    {
                        // still has time
                        //PredictDateBox.Text = "happening by " + curBlah.E.ToShortDateString();
                        //PredictElapsedTimeBox.Text = "(" + Utilities.ElapsedDateString(curBlah.E) + ")";

                        predictsDatebox.Text = "happening by " + curBlah.E.ToShortDateString();
                        predictsElapsedtime.Text = "(" + Utilities.ElapsedDateString(curBlah.E) + ")";

                        //WillHappenItems.Visibility = Visibility.Visible;
                        //AlreadyHappenedItems.Visibility = Visibility.Collapsed;
                        //WillHappenItems.ItemsSource = curBlah.PredictionItems;
                    }
                    else
                    {
                        // expired
                        //PredictDateBox.Text = "should have happened on " + curBlah.E.ToShortDateString();
                        //PredictElapsedTimeBox.Text = "(" + Utilities.ElapsedDateString(curBlah.E) + ")";

                        predictsDatebox.Text = "should have happened on " + curBlah.E.ToShortDateString();
                        predictsElapsedtime.Text = "(" + Utilities.ElapsedDateString(curBlah.E) + ")";

                        //WillHappenItems.Visibility = Visibility.Visible;
                        //AlreadyHappenedItems.Visibility = Visibility.Collapsed;
                        //AlreadyHappenedItems.ItemsSource = curBlah.ExpPredictionItems;
                    }
                });

                //((Storyboard)Resources["ShowPredictionAnimation"]).Begin();
            }
            );
        }
        #endregion
    }
}