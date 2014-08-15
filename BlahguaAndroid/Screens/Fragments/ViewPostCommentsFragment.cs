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
using BlahguaMobile.AndroidClient.Adapters;
using Android.Animation;
using Android.Graphics;
using BlahguaMobile.AndroidClient.ThirdParty.UrlImageViewHelper;
using Android.Database;
using Android.Graphics.Drawables;
using BlahguaMobile.AndroidClient.HelpingClasses;

namespace BlahguaMobile.AndroidClient.Screens
{
    class ViewPostCommentsFragment : Fragment, IUrlImageViewCallback
    {
        private readonly int SELECTIMAGE_REQUEST = 777;

        public static ViewPostCommentsFragment NewInstance()
        {
            return new ViewPostCommentsFragment { Arguments = new Bundle() };
        }

        private readonly string TAG = "ViewPostCommentsFragment";

        private TextView comments_total_count;
        private ListView list;
        private LinearLayout no_comments, create_comment_block;

        private Button btn_done;
        private EditText text;

        private CommentsAdapter adapter;

        #region ImageUpload

        private FrameLayout imageCreateCommentLayout;
        private ImageView imageCreateComment;
        private ProgressBar progressBarImageLoading;

        private string GetPathToImage(Android.Net.Uri uri)
        {
            string path = null;
            // The projection contains the columns we want to return in our query.
            string[] projection = new[] { Android.Provider.MediaStore.Images.Media.InterfaceConsts.Data };
            using (ICursor cursor = Activity.ManagedQuery(uri, projection, null, null, null))
            {
                if (cursor != null)
                {
                    int columnIndex = cursor.GetColumnIndexOrThrow(Android.Provider.MediaStore.Images.Media.InterfaceConsts.Data);
                    cursor.MoveToFirst();
                    path = cursor.GetString(columnIndex);
                }
            }
            return path;
        }

        public override void OnActivityResult(int requestCode, Android.App.Result resultCode, Intent data)
        {
            if (requestCode == SELECTIMAGE_REQUEST && resultCode == Android.App.Result.Ok)
            {
                progressBarImageLoading.Visibility = ViewStates.Visible;
                imageCreateCommentLayout.Visibility = ViewStates.Visible;
                imageCreateComment.SetImageDrawable(null);
                System.IO.Stream fileStream = StreamHelper.GetStreamFromFileUri(this.Activity, data.Data, 1024);
                String fileName = StreamHelper.GetFileName(this.Activity, data.Data);

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
                            MainActivity.analytics.PostUploadCommentImage();
                        }
                        else
                        {
                            progressBarImageLoading.Visibility = ViewStates.Gone;
                            ClearImages();
                            MainActivity.analytics.PostSessionError("commentimageuploadfailed");
                        }
                    });
                }
                );
            }
            base.OnActivityResult(requestCode, resultCode, data);
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
            BlahguaAPIObject.Current.CreateRecord.M = null;
            imageCreateComment.SetImageDrawable(null);
            imageCreateCommentLayout.Visibility = ViewStates.Gone;
        }
        #endregion

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            MainActivity.analytics.PostPageView("/blah/comments");
            View fragment = inflater.Inflate(Resource.Layout.fragment_viewpost_comments, null);

            create_comment_block = fragment.FindViewById<LinearLayout>(Resource.Id.create_comment_block);
            text = create_comment_block.FindViewById<EditText>(Resource.Id.text);
            text.TextChanged += edit_TextChanged;
            Button btn_select_image = create_comment_block.FindViewById<Button>(Resource.Id.btn_image);
            btn_select_image.Click += (sender, args) => {
                var imageIntent = new Intent();
                imageIntent.SetType("image/*");
                imageIntent.SetAction(Intent.ActionGetContent);
                StartActivityForResult(
                    Intent.CreateChooser(imageIntent, "Select image"), SELECTIMAGE_REQUEST);
            };
            Button btn_signature = create_comment_block.FindViewById<Button>(Resource.Id.btn_signature);
            btn_signature.Click += (sender, args) =>
            {
                initiateSignaturePopUp();
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

            LoadComments();

            return fragment;
        }

        private void edit_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            btn_done.Enabled = (text.Text.Length > 1);
        }

        #region Signature
        private PopupWindow signaturePopup;
        /*
         * Function to set up the pop-up window which acts as drop-down list
         * */
        private void initiateSignaturePopUp()
        {
            LayoutInflater inflater = (LayoutInflater)Activity.GetSystemService(Context.LayoutInflaterService);

            //get the pop-up window i.e.  drop-down layout
            LinearLayout layout = (LinearLayout)inflater.Inflate(Resource.Layout.popup_signature, (ViewGroup)Activity.FindViewById(Resource.Id.popUpView));

            //get the view to which drop-down layout is to be anchored
            Button layout1 = (Button)Activity.FindViewById(Resource.Id.btn_signature);

            signaturePopup = new PopupWindow(layout, (int)(Resources.DisplayMetrics.Density * 200), ViewGroup.LayoutParams.WrapContent, true);

            //Pop-up window background cannot be null if we want the pop-up to listen touch events outside its window
            signaturePopup.SetBackgroundDrawable(new BitmapDrawable());
            signaturePopup.Touchable = true;

            //let pop-up be informed about touch events outside its window. This  should be done before setting the content of pop-up
            signaturePopup.OutsideTouchable = true;
            signaturePopup.Height = ViewGroup.LayoutParams.WrapContent;

            //dismiss the pop-up i.e. drop-down when touched anywhere outside the pop-up
            //pw.setTouchInterceptor(new OnTouchListener() {

            //    public bool onTouch(View v, MotionEvent ev) {
            //        // TODO Auto-generated method stub
            //        if (ev.Action == MotionEventActions.Outside) {
            //            pw.Dismiss();
            //            return true;    				
            //        }
            //        return false;
            //    }
            //});

            //provide the source layout for drop-down
            signaturePopup.ContentView = layout;

            //anchor the drop-down to bottom-left corner of 'layout1'
            signaturePopup.ShowAsDropDown(layout1);

            //populate the drop-down list
            ListView list = (ListView)layout.FindViewById(Resource.Id.dropDownList);
            list.Adapter = new CreateCommentSignatureAdapter(Activity, BlahguaAPIObject.Current.CurrentUser.Badges);
        }
        #endregion

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
                MainActivity.analytics.PostCreateComment();
                // might want to resort the comments...
                //NavigationService.GoBack();
                Activity.RunOnUiThread(() =>
                {
                    triggerCreateBlock();
                    LoadComments();
                });
            }
            else
            {
                MainActivity.analytics.PostSessionError("commentcreatefailed");
                // handle create comment failed
                Toast.MakeText(Activity, "Your comment was not created.  Please try again or come back another time.", ToastLength.Short).Show();
                btn_done.Enabled = true;
                text.Enabled = true;
            }

        }

        public void LoadComments()
        {
            BlahguaAPIObject.Current.LoadBlahComments((theList) =>
            {
                Activity.RunOnUiThread(() =>
                {
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
        }

        #region CreateCommentUI

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
        #endregion
    }
}