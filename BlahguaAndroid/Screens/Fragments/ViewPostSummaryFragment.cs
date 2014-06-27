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
        private TextView author, timeago;
        private TextView authorDescription;
        private ImageView authorAvatar, badgesIcon;
        private ListView authorBadgesArea;

        // predicts layout
        private ListView predictsVotes;
        private TextView predictsDatebox;
        private TextView predictsElapsedtime;
        private LinearLayout predictsLayout;

        // predicts layout
        private ListView pollsVotes;
        private LinearLayout pollsLayout;

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
            timeago = fragment.FindViewById<TextView>(Resource.Id.timeago);
            authorDescription = fragment.FindViewById<TextView>(Resource.Id.author_description);
            authorAvatar = fragment.FindViewById<ImageView>(Resource.Id.author_avatar);
            badgesIcon = fragment.FindViewById<ImageView>(Resource.Id.badges_icon);
            authorBadgesArea = fragment.FindViewById<ListView>(Resource.Id.badges_block);
            badgesIcon.Click += (sender, args) =>
            {
                if (authorBadgesArea.Visibility == ViewStates.Visible)
                    authorBadgesArea.Visibility = ViewStates.Gone;
                else
                    authorBadgesArea.Visibility = ViewStates.Visible;
            };

            predictsVotes = fragment.FindViewById<ListView>(Resource.Id.predicts_votes);
            predictsDatebox = fragment.FindViewById<TextView>(Resource.Id.predicts_datebox);
            predictsElapsedtime = fragment.FindViewById<TextView>(Resource.Id.predicts_elapsedtime);
            predictsLayout = fragment.FindViewById<LinearLayout>(Resource.Id.predicts_layout);

            pollsLayout = fragment.FindViewById<LinearLayout>(Resource.Id.polls_layout);
            pollsVotes = fragment.FindViewById<ListView>(Resource.Id.polls_votes);

            return fragment;
        }


        public override void OnPause()
        {
            dialog.Dismiss();
            base.OnPause();
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
                if (theBlah != null)
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
                        if (!(theBlah.u == null && theBlah.c == null))
                        {
                            timeago.Text = StringHelper.ConstructTimeAgo(theBlah.u == null ? theBlah.CreationDate : theBlah.UpdateDate);
                        }
                        authorDescription.Text = theBlah.Description.d;
                        authorAvatar.SetUrlDrawable(theBlah.UserImage);
                        if (theBlah.Badges != null)
                        {
                            badgesIcon.Visibility = ViewStates.Visible;
                            authorBadgesArea.Visibility = ViewStates.Visible;
                            authorBadgesArea.Adapter = new ViewPostBadgesAdapter(Activity);
                        }
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
                    MainActivity.analytics.PostSessionError("loadblahfailed");
                    // Finish();
                }

                ((ViewPostActivity)Activity).UpdateSummaryButtons();
            });
        }


        #region Handles

        private void HandlePollInit()
        {
            parent.RunOnUiThread(() =>
            {
                pollsLayout.Visibility = ViewStates.Visible;
            });
            BlahguaAPIObject.Current.GetUserPollVote((theVote) =>
            {
                Blah curBlah = BlahguaAPIObject.Current.CurrentBlah;
                bool isVoted = ((theVote != null) && (theVote.W > -1));
                parent.RunOnUiThread(() =>
                {
                    pollsVotes.Adapter = new VotesAdapter(Activity, curBlah.I, !isVoted, pollsTap);
                    //((Storyboard)Resources["ShowPollAnimation"]).Begin();
                    HistoryUiHelper.setListViewHeightBasedOnChildren(pollsVotes);
                });
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
                            bool isVoted = theVote != null && !String.IsNullOrEmpty(theVote.D);
                            predictsVotes.Adapter = new VotesAdapter(Activity, curBlah.PredictionItems, !isVoted, predictsTap);
								    predictsDatebox.Text = "happening by " + curBlah.ExpireDate.ToShortDateString();
								    predictsElapsedtime.Text = "(" + Utilities.ElapsedDateString(curBlah.ExpireDate) + ")";
                        }
                        else
                        {
                            // expired
                            bool isVoted = theVote != null && !String.IsNullOrEmpty(theVote.Z);
                            predictsVotes.Adapter = new VotesAdapter(Activity, curBlah.ExpPredictionItems, !isVoted, predictsTap);
								    predictsDatebox.Text = "should have happened on " + curBlah.ExpireDate.ToShortDateString();
								    predictsElapsedtime.Text = "(" + Utilities.ElapsedDateString(curBlah.ExpireDate) + ")";
                        }

                        HistoryUiHelper.setListViewHeightBasedOnChildren(predictsVotes);
                    });
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
            }
            );
        }

        bool pollsTapped = false;
        private void pollsTap(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            if (pollsTapped)
                return;
            VotesAdapter adapter = (VotesAdapter)pollsVotes.Adapter;
            int pos = (int)((CheckBox)sender).Tag;
            PollItem newVote = adapter.List[pos];
            pollsTapped = true;
            BlahguaAPIObject.Current.SetPollVote(newVote, (resultStr) =>
            {
                pollsTapped = newVote != null;
                HandlePollInit();
            }
            );
        }

        #endregion
    }
}