﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Xna.Framework.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Microsoft.Phone.Tasks;
using System.Windows.Data;
using System.IO;
using BlahguaMobile.BlahguaCore;


namespace BlahguaMobile.Winphone
{
    public partial class PhotoShare : PhoneApplicationPage
    {
        bool isInited = false;
        Picture photoFromLibrary = null;

        public PhotoShare()
        {
            InitializeComponent();
           
        }

        private void InitService()
        {
            BlahguaAPIObject.Current.Initialize(null, DoServiceInited);


        }

        void DoServiceInited(bool didIt)
        {
            isInited = true;
            if (didIt)
            {
                if (BlahguaAPIObject.Current.CurrentUser == null)
                {
                    NavigationService.Navigate(new Uri("/Screens/Signin.xaml", UriKind.Relative)); 
                }
                else
                {
                    PrepareForUser();
                }
            }
            else
            {
                LoadingBox.Visibility = Visibility.Collapsed;
                MessageBox.Show("Heard failed to initialize.  Please try again later.");
                NavigationService.GoBack(); 
            }
        }

        void PrepareForUser()
        {
            BlahguaAPIObject.Current.EnsureUserDescription((desc) =>
                {
                    if (BlahguaAPIObject.Current.CreateRecord == null)
                        BlahguaAPIObject.Current.CreateRecord = new BlahCreateRecord();

                    SelectedBadgesList.SummaryForSelectedItemsDelegate = SummarizeItems;
                    if ((BlahguaAPIObject.Current.CurrentUser.Badges != null) && (BlahguaAPIObject.Current.CurrentUser.Badges.Count > 0))
                        SelectedBadgesList.Visibility = Visibility.Visible;
                    else
                        SelectedBadgesList.Visibility = Visibility.Collapsed;
                    this.DataContext = BlahguaAPIObject.Current;

                    LoadingBox.Visibility = Visibility.Collapsed;
                }
            );
        }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            App.analytics.PostPageView("/PhotoShare");
            // Get a dictionary of query string keys and values.
            IDictionary<string, string> queryStrings = this.NavigationContext.QueryString;

            // Ensure that there is at least one key in the query string, and check whether the "FileId" key is present.
            if (!isInited)
            {
                if (queryStrings.ContainsKey("FileId"))
                {
                    // Retrieve the photo from the media library using the FileID passed to the app.
                    MediaLibrary library = new MediaLibrary();
                    photoFromLibrary = library.GetPictureFromToken(queryStrings["FileId"]);

                    // Create a BitmapImage object and add set it as the image control source.
                    // To retrieve a full-resolution image, use the GetImage() method instead.
                    BitmapImage bitmapFromPhoto = new BitmapImage();
                    bitmapFromPhoto.SetSource(photoFromLibrary.GetImage());
                    PreviewImage.Source = bitmapFromPhoto;

                    InitService();

                }
            }
            else
            {
                if (BlahguaAPIObject.Current.CurrentUser == null)
                {
                    MessageBox.Show("Heard was unable to sign in.   Please sign in or create an account to post.");
                    NavigationService.GoBack();
                }
                else
                {
                    PrepareForUser();
                }
            }

        }


        private void OnCreateBlahOK(Blah newBlah)
        {
            if (newBlah != null)
            {

                BlahguaAPIObject.Current.NewBlahToInsert = newBlah;
                App.analytics.PostCreateBlah("photoshare");
                NavigationService.GoBack();
            }
            else
            {
                MessageBox.Show("Unable to create the post.  Please try again.  If the problem persists, go ahead and try at a different time.");

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
                        return "Polls require at least two chouices.";

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



        private string SummarizeItems(System.Collections.IList theItems)
        {
            if ((theItems == null) || (theItems.Count == 0))
            {
                return "no badges selected";
            }
            else
            {
                string badgeNames = "";
                foreach (BadgeReference curBadge in theItems)
                {
                    if (badgeNames != "")
                        badgeNames += ", ";
                    badgeNames += curBadge.BadgeName;

                }

                return badgeNames;
            }
        }


        private void BlahChannelChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BlahguaAPIObject.Current.CreateRecord != null)
            {

            }
        }

        private void BlahTypeChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((e.AddedItems != null) && (e.AddedItems.Count > 0))
            {

                BlahType newType = e.AddedItems[0] as BlahType;
                Storyboard sb;

                switch (newType.N)
                {
                    case "polls":
                        sb = ((Storyboard)Resources["ShowPollAnimation"]);
                        break;
                    case "predicts":
                        sb = ((Storyboard)Resources["ShowPredictionAnimation"]);
                        break;
                    default:
                        sb = ((Storyboard)Resources["HideAllSectionsAnimation"]);
                        break;

                }

                //sb.SpeedRatio = 3;
                sb.Begin();
                SetBlahBackground(newType.N);
            }



        }



        private void UseProfile_Checked(object sender, RoutedEventArgs e)
        {
            AuthorHeader.DataContext = null;
            BlahguaAPIObject.Current.CreateRecord.UseProfile = (bool)((CheckBox)sender).IsChecked;
            AuthorHeader.DataContext = BlahguaAPIObject.Current.CreateRecord;

        }


        private void SelectedBadgesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AuthorHeader.DataContext = null;
            if (BlahguaAPIObject.Current.CreateRecord != null)
            {
                System.Collections.IList curSelection = SelectedBadgesList.SelectedItems;

                if ((curSelection != null) && (curSelection.Count > 0))
                {
                    BadgeList newList = new BadgeList();
                    foreach (object curItem in curSelection)
                    {
                        BadgeReference curBadge = (BadgeReference)curItem;
                        newList.Add(curBadge);
                    }

                    BlahguaAPIObject.Current.CreateRecord.Badges = newList;
                }
                else
                    BlahguaAPIObject.Current.CreateRecord.Badges = null;

                AuthorHeader.DataContext = BlahguaAPIObject.Current.CreateRecord;
            }
        }

        private void PollItem_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (BlahguaAPIObject.Current.CreateRecord.I.Count > 2)
            {
                var curItem = sender;
                var parent = VisualTreeHelper.GetParent((TextBlock)sender);

                while (!(parent is StackPanel))
                {
                    curItem = parent;
                    parent = VisualTreeHelper.GetParent(parent);
                }

                int index = ((StackPanel)parent).Children.IndexOf((UIElement)curItem);
                BlahguaAPIObject.Current.CreateRecord.I.RemoveAt(index);
                MaybeEnableAddPollBtns();

            }
        }


        void SetBlahBackground(string newType)
        {
            Brush newBrush;

            switch (newType)
            {
                case "leaks":
                    newBrush = (Brush)App.Current.Resources["BaseBrushLeaks"];
                    break;
                case "polls":
                    newBrush = (Brush)App.Current.Resources["BaseBrushPolls"]; ;
                    break;
                case "asks":
                    newBrush = (Brush)App.Current.Resources["BaseBrushAsks"]; ;
                    break;
                case "predicts":
                    newBrush = (Brush)App.Current.Resources["BaseBrushPredicts"]; ;
                    break;
                default:
                    newBrush = (Brush)App.Current.Resources["BaseBrushSays"]; ;
                    break;
            }

            BackgroundScreen.Fill = newBrush;

        }


        private void DoAddPollChoice(object sender, System.Windows.Input.GestureEventArgs e)
        {
            int count = BlahguaAPIObject.Current.CreateRecord.I.Count;

            if (count < 10)
            {
                BlahguaAPIObject.Current.CreateRecord.I.Add(new PollItem("choice " + (count + 1)));
                MaybeEnableAddPollBtns();
            }
        }

        private void MaybeEnableAddPollBtns()
        {
            int count = BlahguaAPIObject.Current.CreateRecord.I.Count;

            AddPollChoiceBtn.IsEnabled = (count < 10);

            foreach (object curItem in PollChoiceList.Descendents().OfType<TextBlock>())
            {
                if (count > 2)
                    ((TextBlock)curItem).Opacity = 1;
                else
                    ((TextBlock)curItem).Opacity = .4;
            }
        }

        private void DoCreateClick(object sender, EventArgs e)
        {
            SelectedBadgesList.Focus();
            BlahguaAPIObject.Current.CreateRecord.T = BlahHeadlineBox.Text;
            string valStr = IsBlahValid();
            if (valStr == "")
            {
                LoadingPrompt.Text = "Posting to Heard...";
                LoadingBox.Visibility = Visibility.Visible;
                WriteableBitmap wBitmap = new WriteableBitmap((BitmapSource)PreviewImage.Source);

                MemoryStream ms = new MemoryStream();
                int width, height;

                if (photoFromLibrary.Width > photoFromLibrary.Height)
                {
                    width = Math.Min(photoFromLibrary.Width, 1024);
                    height = (photoFromLibrary.Height * width) / photoFromLibrary.Width;
                }
                else
                {
                    height = Math.Min(photoFromLibrary.Height, 1024);
                    width = (photoFromLibrary.Width * height) / photoFromLibrary.Height;
                }

                wBitmap.SaveJpeg(ms, width, height, 0, 90);
                BlahguaAPIObject.Current.UploadPhoto(ms, photoFromLibrary.Name.Substring(photoFromLibrary.Name.LastIndexOf("\\") + 1), (photoString) =>
                    {
                        if ((photoString != null) && (photoString.Length > 0))
                        {
                            BlahguaAPIObject.Current.CreateRecord.M = new List<string>();
                            BlahguaAPIObject.Current.CreateRecord.M.Add(photoString);
                        }
                        BlahguaAPIObject.Current.CreateBlah(OnCreateBlahOK);

                    }
                );
                
            }
            else
            {
                MessageBox.Show(valStr);
            }
        }
    }

    

}