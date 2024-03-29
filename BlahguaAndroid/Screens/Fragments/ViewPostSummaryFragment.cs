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
using Android.Support.V4.App;
using Android.Support.V4.View;


namespace BlahguaMobile.AndroidClient.Screens
{
    public class ViewPostSummaryFragment : Android.Support.V4.App.Fragment
    {
        public static ViewPostSummaryFragment NewInstance()
        {
            return new ViewPostSummaryFragment { Arguments = new Bundle() };
        }

        private Activity parent = null;

        // main block
        private ImageView image;
        private TextView titleView, messageView;
        private ProgressDialog dialog;
        private ProgressBar progress_image;

        // author block
        private TextView author, timeago;
        private TextView authorDescription;
        private ImageView authorAvatar, badgesIcon;
		private ListView authorBadgeList;

        // predicts layout
        private ListView predictsVotes;
        private TextView predictsDatebox;
        private TextView predictsElapsedtime;
		private LinearLayout predictsLayout, authorDetailsArea;

        // predicts layout
        private ListView pollsVotes;
        private LinearLayout pollsLayout;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
			HomeActivity.analytics.PostPageView("/blah/summary");
            parent = (Activity)inflater.Context;
            View fragment = inflater.Inflate(Resource.Layout.fragment_viewpost_summary, null);

            dialog = new ProgressDialog(parent);
            dialog.SetMessage("Please wait...");
            dialog.SetCancelable(false);

            messageView = fragment.FindViewById<TextView>(Resource.Id.text);
            titleView = fragment.FindViewById<TextView>(Resource.Id.title);
			image = fragment.FindViewById<ImageView>(Resource.Id.blah_image);
            image.Click += (sender, args) => {

                var intent = new Intent(Activity, typeof(ImageViewActivity));
                intent.PutExtra("image", BlahguaAPIObject.Current.CurrentBlah.ImageURL);
                StartActivity(intent);
            };

            progress_image = fragment.FindViewById<ProgressBar>(Resource.Id.loader_image);

            author = fragment.FindViewById<TextView>(Resource.Id.author);
            timeago = fragment.FindViewById<TextView>(Resource.Id.timeago);
            authorDescription = fragment.FindViewById<TextView>(Resource.Id.author_description);
            authorAvatar = fragment.FindViewById<ImageView>(Resource.Id.author_avatar);
            badgesIcon = fragment.FindViewById<ImageView>(Resource.Id.badges_icon);
			authorDetailsArea = fragment.FindViewById<LinearLayout>(Resource.Id.details_block);
			authorBadgeList = fragment.FindViewById<ListView>(Resource.Id.badges_list);
			authorAvatar.Click += (sender, args) =>
            {
				if (authorDetailsArea.Visibility == ViewStates.Visible)
					authorDetailsArea.Visibility = ViewStates.Gone;
                else
					authorDetailsArea.Visibility = ViewStates.Visible;
            };

            predictsVotes = fragment.FindViewById<ListView>(Resource.Id.predicts_votes);
            predictsDatebox = fragment.FindViewById<TextView>(Resource.Id.predicts_datebox);
            predictsElapsedtime = fragment.FindViewById<TextView>(Resource.Id.predicts_elapsedtime);
            predictsLayout = fragment.FindViewById<LinearLayout>(Resource.Id.predicts_layout);

            pollsLayout = fragment.FindViewById<LinearLayout>(Resource.Id.polls_layout);
            pollsVotes = fragment.FindViewById<ListView>(Resource.Id.polls_votes);

            UiHelper.SetGothamTypeface(TypefaceStyle.Normal, messageView, authorDescription, predictsElapsedtime);
			UiHelper.SetGothamTypeface (TypefaceStyle.Bold, author, titleView);
			UiHelper.SetGothamTypeface (TypefaceStyle.Italic, timeago);
            initBlahPost();

            return fragment;
        }


        private Blah loadedBlah;
        private void initBlahPost()
        {
            if (loadedBlah == null)
            {
                //dialog.Show();
                BlahguaAPIObject.Current.SetCurrentBlahFromId(App.BlahIdToOpen, (theBlah) =>
                {
                    loadedBlah = theBlah;
                    parent.RunOnUiThread(() =>
                    {
                        //dialog.Hide();
                        Activity.InvalidateOptionsMenu();
                    });
                    populateFragment();
                });
            }
            else
            {
                populateFragment();
            }
        }

        private void populateFragment()
        {
            if (loadedBlah != null)
            {
                if (loadedBlah.ImageURL != null && image != null)
                {
                    parent.RunOnUiThread(() =>
                    {
                        progress_image.Visibility = ViewStates.Visible;
                    });
                    ImageLoader.Instance.DownloadAsync(loadedBlah.ImageURL,(b) =>
                        {
                            parent.RunOnUiThread(() =>
                            {
                                progress_image.Visibility = ViewStates.Gone;
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
                    if (String.IsNullOrEmpty(loadedBlah.T))
                        titleView.Visibility = ViewStates.Gone;
                    else 
                    {
                        titleView.Visibility = ViewStates.Visible;
                        titleView.SetText(loadedBlah.T, TextView.BufferType.Normal);
                    }

                    if (String.IsNullOrEmpty(loadedBlah.F))
                        messageView.Visibility = ViewStates.Gone;
                    else 
                    {
                        messageView.Visibility = ViewStates.Visible;
                        messageView.SetText(loadedBlah.F, TextView.BufferType.Normal);
                    }
                    
                });

                // author
                parent.RunOnUiThread(() =>
                {
                    author.Text = loadedBlah.UserName;
                    if ( loadedBlah.cdate != null)
                    {
                        timeago.Text = StringHelper.ConstructTimeAgo(loadedBlah.cdate);
                    }

                    if (BlahguaAPIObject.Current.CurrentChannel.SAD == false)
                    {
                        authorDescription.Visibility = ViewStates.Invisible;
                    }
                    else
                    {
                        authorDescription.Visibility = ViewStates.Visible;
                        if (!loadedBlah.XX)
                        {
                            authorDescription.Text = loadedBlah.Description.d;

                        }
                        else
                        {
                            authorDescription.Text = "an unidentified person";
                        }
                    }


                    authorAvatar.SetUrlDrawable(loadedBlah.UserImage);

                    if (loadedBlah.B != null)
                    {
                        badgesIcon.Visibility = ViewStates.Visible;
						authorBadgeList.Visibility = ViewStates.Visible;
						authorBadgeList.Adapter = new ViewPostBadgesAdapter(Activity);
                    }
					else
					{
						badgesIcon.Visibility = ViewStates.Gone;
						authorBadgeList.Visibility = ViewStates.Gone;
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
                Activity.RunOnUiThread(() =>
                {
                    Toast.MakeText(parent, "unable to load blah.  Sorry!", ToastLength.Long).Show();
                });
				HomeActivity.analytics.PostSessionError("loadblahfailed");
                // Finish();
            }

            //((ViewPostActivity)Activity).UpdateSummaryButtons();

        }

		public override void OnPause ()
		{
			if (dialog.IsShowing)
				dialog.Hide ();
			base.OnPause ();
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
                bool isVoted = pollsTapped || theVote != 0;
                parent.RunOnUiThread(() =>
                {
                    pollsVotes.Adapter = new VotesAdapter(Activity, curBlah.I, !isVoted, pollsTap);
                    //((Storyboard)Resources["ShowPollAnimation"]).Begin();
                    UiHelper.SetListViewHeightBasedOnChildren(pollsVotes);
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
                        if (curBlah.E > DateTime.Now)
                        {
                            // still has time
                            bool isVoted = predictsTapped || (theVote != null && !String.IsNullOrEmpty(theVote.D));
                            predictsVotes.Adapter = new VotesAdapter(Activity, curBlah.PredictionItems, !isVoted, predictsTap);
								    predictsDatebox.Text = "happening by " + curBlah.E.ToShortDateString();
								    predictsElapsedtime.Text = "(" + Utilities.ElapsedDateString(curBlah.E) + ")";
                        }
                        else
                        {
                            // expired
                            bool isVoted = predictsTapped || (theVote != null && !String.IsNullOrEmpty(theVote.Z));
                            predictsVotes.Adapter = new VotesAdapter(Activity, curBlah.ExpPredictionItems, !isVoted, predictsTap);
								    predictsDatebox.Text = "should have happened on " + curBlah.E.ToShortDateString();
								    predictsElapsedtime.Text = "(" + Utilities.ElapsedDateString(curBlah.E) + ")";
                        }

                        UiHelper.SetListViewHeightBasedOnChildren(predictsVotes);
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