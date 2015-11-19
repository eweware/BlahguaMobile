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
using Android.Provider;
using BlahguaMobile.BlahguaCore;
using BlahguaMobile.AndroidClient.Adapters;
using Android.Animation;
using Android.Graphics;
using BlahguaMobile.AndroidClient.ThirdParty.UrlImageViewHelper;
using Android.Database;
using Android.Graphics.Drawables;
using BlahguaMobile.AndroidClient.HelpingClasses;
using Android.Support.V4.App;
using Android.Support.V4.View;


using File = Java.IO.File;
using Uri = Android.Net.Uri;

namespace BlahguaMobile.AndroidClient.Screens
{
    public class ViewPostCommentsFragment : Android.Support.V4.App.Fragment, IUrlImageViewCallback
    {


        public static ViewPostCommentsFragment NewInstance()
        {
            return new ViewPostCommentsFragment { Arguments = new Bundle() };
        }

        private TextView comments_total_count;
        private ListView list;
        private LinearLayout no_comments, create_comment_block;
		private const int MultiChoiceDialog = 0x03;

		private Button btn_done;
		Button btn_signature;
        private EditText text;

        private CommentsAdapter adapter;
        private ProgressDialog progressDlg;

        public bool shouldCreateComment = false;
        public ViewPostActivity baseView = null;

        #region ImageUpload

        public FrameLayout imageCreateCommentLayout;
        public ImageView imageCreateComment;
        public ProgressBar progressBarImageLoading;
        public bool commentsAreLoaded = false;


        public void HandleCommentImage(int requestCode, Intent data)
        {
            progressBarImageLoading.Visibility = ViewStates.Visible;
            imageCreateCommentLayout.Visibility = ViewStates.Visible;
            imageCreateComment.SetImageDrawable(null);
            System.IO.Stream fileStream;
            String fileName;

            if (requestCode == ViewPostActivity.SELECTIMAGE_REQUEST)
            {
                fileStream = StreamHelper.GetStreamFromFileUri(this.Activity, data.Data);
                fileName = StreamHelper.GetFileName(this.Activity, data.Data);
            }
            else
            {
                Bitmap scaledBitmap = BitmapHelper.LoadAndResizeBitmap(HomeActivity._file.AbsolutePath, HomeActivity.MAX_IMAGE_SIZE);
                fileStream = new System.IO.MemoryStream();
                scaledBitmap.Compress(Bitmap.CompressFormat.Jpeg, 90, fileStream);
                fileStream.Flush();
                fileName = HomeActivity._file.Name;
            }

            if (fileStream != null)
            {
                BlahguaAPIObject.Current.UploadPhoto(fileStream, fileName, (photoString) =>
                {
                    Activity.RunOnUiThread(() =>
                    {
                        if ((photoString != null) && (photoString.Length > 0))
                        {
                            string photoURL = BlahguaAPIObject.Current.GetImageURL(photoString, "B");
                            imageCreateComment.SetUrlDrawable(photoURL, this);
                            BlahguaAPIObject.Current.CreateCommentRecord.M = new List<string>();
                            BlahguaAPIObject.Current.CreateCommentRecord.M.Add(photoString);
                            HomeActivity.analytics.PostUploadBlahImage();
                        }
                        else
                        {
                            progressBarImageLoading.Visibility = ViewStates.Gone;
                            ClearImages();
                            HomeActivity.analytics.PostSessionError("commentimageuploadfailed");
                        }
                    });
                });
            }
            else
            {
                Activity.RunOnUiThread(() =>
                {
                    var toast = Toast.MakeText(Activity, "Cannot upload this type of image", ToastLength.Long);
                    toast.Show();
                    progressBarImageLoading.Visibility = ViewStates.Gone;
                    ClearImages();
                });
            }
        }
		

        public void OnLoaded(ImageView imageView, Android.Graphics.Drawables.Drawable loadedDrawable, string url, bool loadedFromCache)
        {
            if (imageCreateComment == imageView)
            {
                Activity.RunOnUiThread(() =>
                {
                    progressBarImageLoading.Visibility = ViewStates.Gone;
                });
            }
        }

        private void ClearImages()
        {
            BlahguaAPIObject.Current.CreateCommentRecord.M = null;
            imageCreateComment.SetImageDrawable(null);
            imageCreateCommentLayout.Visibility = ViewStates.Gone;
        }
        #endregion


        public void ShowComment(PublishAction theAction)
        {
            // load the comment and append it

			if ((BlahguaAPIObject.Current.CurrentUser == null) || (theAction.userid != BlahguaAPIObject.Current.CurrentUser._id))
            {
                BlahguaAPIObject.Current.GetComment(theAction.commentid, (theComment) =>
                {
                    InsertNewComment(theComment);
                });
            }
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
			HomeActivity.analytics.PostPageView("/blah/comments");
            View fragment = inflater.Inflate(Resource.Layout.fragment_viewpost_comments, null);
            progressDlg = new ProgressDialog(this.Activity);
            progressDlg.SetProgressStyle(ProgressDialogStyle.Spinner);

            create_comment_block = fragment.FindViewById<LinearLayout>(Resource.Id.create_comment_block);
            text = create_comment_block.FindViewById<EditText>(Resource.Id.text);
            text.TextChanged += edit_TextChanged;
            Button btn_select_image = create_comment_block.FindViewById<Button>(Resource.Id.btn_image);
			btn_select_image.Click += (sender, args) =>
			{
				PopupMenu imageMenu = new PopupMenu(this.Activity, btn_select_image);
				imageMenu.Inflate(Resource.Menu.cameramenu);

				if ((BlahguaAPIObject.Current.CreateCommentRecord.M == null) ||
					(BlahguaAPIObject.Current.CreateCommentRecord.M.Count == 0))
					imageMenu.Menu.RemoveItem(Resource.Id.action_removephoto);

				imageMenu.MenuItemClick += (s1, arg1) =>
				{
					if (arg1.Item.ItemId == Resource.Id.action_takephoto)
					{
                        baseView.UserTakePhoto();
					}
					else if (arg1.Item.ItemId == Resource.Id.action_selectphoto)
					{
						// select a photo
                        baseView.UserChoosePhoto();
					}
					else if (arg1.Item.ItemId == Resource.Id.action_removephoto)
					{
						// remove a photo
						ClearImages();
					}
				};

				imageMenu.Show();
			};
            btn_signature = create_comment_block.FindViewById<Button>(Resource.Id.btn_signature);
            btn_signature.Click += (sender, args) =>
            {
				Activity.ShowDialog(HomeActivity.MultiChoiceDialog);
            };
            btn_done = create_comment_block.FindViewById<Button>(Resource.Id.btn_done);
            btn_done.Click += btn_done_Click;
            btn_done.Enabled = false;

            imageCreateComment = create_comment_block.FindViewById<ImageView>(Resource.Id.createcomment_image);
            imageCreateCommentLayout = create_comment_block.FindViewById<FrameLayout>(Resource.Id.createcomment_image_layout);
            progressBarImageLoading = create_comment_block.FindViewById<ProgressBar>(Resource.Id.progress_image_loading);
            progressBarImageLoading.Visibility = ViewStates.Gone;

            comments_total_count = fragment.FindViewById<TextView>(Resource.Id.comments_total_count);
            list = fragment.FindViewById<ListView>(Resource.Id.list);
            list.ChoiceMode = ListView.ChoiceModeNone;
            no_comments = fragment.FindViewById<LinearLayout>(Resource.Id.no_comments);

            UiHelper.SetGothamTypeface(TypefaceStyle.Normal, comments_total_count, btn_done, btn_signature, btn_select_image);
            comments_total_count.Text = "loading comments";
                    
            //LoadComments();

            if (shouldCreateComment)
            {
                shouldCreateComment = false;
                this.Activity.RunOnUiThread(() =>
                {
                    triggerCreateBlock();
                });
            }

            return fragment;
        }

        private void edit_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            btn_done.Enabled = (text.Text.Length > 1);
        }

        private void btn_done_Click(object sender, EventArgs e)
        {
            btn_done.Enabled = false;
            text.Enabled = false;
            var comment = BlahguaAPIObject.Current.CreateCommentRecord;
            comment.T = text.Text;
            //SelectedBadgesList.Focus();
            BlahguaAPIObject.Current.CreateComment(OnCreateCommentOK);
        }

        public void PrepareNewComment()
        {
            if (BlahguaAPIObject.Current.CreateCommentRecord == null)
            {
                BlahguaAPIObject.Current.CreateCommentRecord = new CommentCreateRecord();
                BlahguaAPIObject.Current.CreateCommentRecord.UseProfile = false;
            }
        }



        private void OnCreateCommentOK(Comment newComment)
        {
            if (newComment != null)
            {
				HomeActivity.analytics.PostCreateComment();
                // might want to resort the comments...
                //NavigationService.GoBack();
                Activity.RunOnUiThread(() =>
                {
                    triggerCreateBlock();
					commentsAreLoaded = false;
                    InsertNewComment(newComment);
                    baseView.NotifyNewComment(newComment);
                    HomeActivity.NotifyBlahActivity();
                });
            }
            else
            {
				HomeActivity.analytics.PostSessionError("commentcreatefailed");
                // handle create comment failed
                Toast.MakeText(Activity, "Your comment was not created.  Please try again or come back another time.", ToastLength.Short).Show();
                btn_done.Enabled = true;
                text.Enabled = true;
            }

        }

        private void InsertNewComment(Comment theComment)
        {
            CommentsAdapter adapter = list.Adapter as CommentsAdapter;

            if (adapter != null)
            {
                adapter.InsertComment(theComment);
                Activity.RunOnUiThread(() =>
                {
                    adapter.NotifyDataSetChanged();
                    list.InvalidateViews();
                    list.SmoothScrollToPosition(0);

                    string commentTextStr;
                    if (adapter.Count == 1)
                        commentTextStr = "one comment";
                    else
                        commentTextStr = adapter.Count.ToString() + " comments";
                    comments_total_count.Text = commentTextStr;
                    list.Visibility = ViewStates.Visible;

                    no_comments.Visibility = ViewStates.Gone;

                });
            }
            else
            {
                commentsAreLoaded = false;
                LoadComments(true);
            }
        }

        

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);
            commentsAreLoaded = false;
        }


        public void LoadComments(bool silent = false)
        {
            if (!commentsAreLoaded)
            {
                commentsAreLoaded = true;

                Activity.RunOnUiThread(() =>
                    {
                        if (!silent)
                        {
                            progressDlg.SetMessage("loading comments...");
                            progressDlg.Show();
                        }


                        BlahguaAPIObject.Current.LoadBlahComments((theList) =>
                        {
                            Activity.RunOnUiThread(() =>
                            {
                                if (!silent)
                                    progressDlg.Hide();

                                if (theList.Count > 0)
                                {
                                    string commentTextStr;
                                    if (theList.Count == 1)
                                        commentTextStr = "one comment";
                                    else
                                        commentTextStr = theList.Count.ToString() + " comments";
                                    comments_total_count.Text = commentTextStr;

                                    no_comments.Visibility = ViewStates.Gone;
                                    list.Visibility = ViewStates.Visible;
                                    adapter = new CommentsAdapter(this, theList);
                                    list.Adapter = adapter;
                                    //list.ItemClick += list_ItemClick;
                                }
                                else
                                {
                                    comments_total_count.Text = "No comments yet.  Add one now!";
                                    no_comments.Visibility = ViewStates.Visible;
                                    list.Visibility = ViewStates.Gone;
                                    list.Adapter = adapter = null;
                                }
                            });
                        });
                    });
            }
        }

    

        public void triggerCreateBlock()
        {
            if (create_comment_block.Visibility.Equals(ViewStates.Gone))
            {
                PrepareNewComment();

                //set Visible
                create_comment_block.Visibility = ViewStates.Visible;
                text.Enabled = true;
                int widthSpec = View.MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified);
                int heightSpec = View.MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified);
                create_comment_block.Measure(widthSpec, heightSpec);

                ValueAnimator mAnimator = slideAnimator(create_comment_block, 0, create_comment_block.MeasuredHeight, false);
                mAnimator.Start();
            }
            else
            {
                //collapse();
                int finalHeight = create_comment_block.Height;

                ValueAnimator mAnimator = slideAnimator(create_comment_block, finalHeight, 0, false);
                mAnimator.Start();
                mAnimator.AnimationEnd += (object IntentSender, EventArgs arg) =>
                {
                    create_comment_block.Visibility = ViewStates.Gone;

                    // reset views
                    imageCreateCommentLayout.Visibility = ViewStates.Gone;
                    text.Text = "";
                };

            }
        }

        private ValueAnimator slideAnimator(View layout, int start, int end, bool animatingWidth)
        {
            ValueAnimator animator = ValueAnimator.OfInt(start, end);

            animator.Update +=
                (object sender, ValueAnimator.AnimatorUpdateEventArgs e) =>
                {
                    var value = (int)animator.AnimatedValue;
                    ViewGroup.LayoutParams layoutParams = layout.LayoutParameters;
                    if(animatingWidth)
                        layoutParams.Width = value;
                    else
                        layoutParams.Height = value;
                    layout.LayoutParameters = layoutParams;

                };
            return animator;
        }
    }
}