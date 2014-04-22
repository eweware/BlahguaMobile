using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media.Imaging;
using Telerik.Windows.Controls;
using Telerik.Charting;
using System.Windows.Data;
using System.Reflection;
using System.ComponentModel;
using Microsoft.Phone.Tasks;
using BlahguaMobile.BlahguaCore;

namespace BlahguaMobile.Winphone
{
    public partial class BlahDetails : PhoneApplicationPage
    {
        ApplicationBarIconButton signInBtn;
        ApplicationBarIconButton promoteBtn;
        ApplicationBarIconButton demoteBtn;
        ApplicationBarIconButton shareBtn;
        ApplicationBarIconButton commentBtn;
        ApplicationBarMenuItem reportItem;

        SortMenu sortByDateAsc;
        SortMenu sortByDateDesc;
        SortMenu sortByUpVote;
        SortMenu sortByDownVote;
        SortMenu sortByStrength;


        string currentPage;
        bool commentsLoaded = false;
        bool statsLoaded = false;
        private string curCommentSort = "byDateDesc";
        private CollectionViewSource commentDataView;
        string defaultImg;

        public BlahDetails()
        {
            commentsLoaded = false;
            statsLoaded = false;
            currentPage = "summary";
            InitializeComponent();

            this.DataContext = null;// BlahguaAPIObject.Current;
            this.ApplicationBar = new ApplicationBar();
            SetBlahBackground();
            ApplicationBar.IsMenuEnabled = true;
            ApplicationBar.IsVisible = true;
            ApplicationBar.Opacity = .8;

            promoteBtn = new ApplicationBarIconButton(new Uri("/Images/Icons/white_promote.png", UriKind.Relative));
            promoteBtn.Text = "promote";
            promoteBtn.Click += HandlePromoteBlah;

            demoteBtn = new ApplicationBarIconButton(new Uri("/Images/Icons/white_demote.png", UriKind.Relative));
            demoteBtn.Text = "demote";
            demoteBtn.Click += HandleDemoteBlah;


            shareBtn = new ApplicationBarIconButton(new Uri("/Images/Icons/share.png", UriKind.Relative));
            shareBtn.Text = "share";
            shareBtn.Click += HandleShareBlah;


            commentBtn = new ApplicationBarIconButton(new Uri("/Images/Icons/white_comment.png", UriKind.Relative));
            commentBtn.Text = "comment";
            commentBtn.Click += HandleAddComment;

            signInBtn = new ApplicationBarIconButton(new Uri("/Images/Icons/signin.png", UriKind.Relative));
            signInBtn.Text = "sign in";
            signInBtn.Click += HandleSignin;

            reportItem = new ApplicationBarMenuItem("flag post as inappropriate");
            reportItem.Click += HandleReportItem;

            sortByDateAsc = new SortMenu("oldest first", "byDateAsc");
            sortByDateAsc.Click += sortMenuItem_Click;

            sortByDateDesc = new SortMenu("newest first", "byDateDesc");
            sortByDateDesc.Click += sortMenuItem_Click;

            sortByUpVote = new SortMenu("most promoted first", "byPositive");
            sortByUpVote.Click += sortMenuItem_Click;

            sortByDownVote = new SortMenu("most demoted first", "byNegative");
            sortByDownVote.Click += sortMenuItem_Click;

            sortByStrength = new SortMenu("most popular first", "byPopular");
            sortByStrength.Click += sortMenuItem_Click;


            StatsArea.DataContext = null;
        }

        void sortMenuItem_Click(object sender, EventArgs e)
        {
            string newSort = ((SortMenu)sender).SortKey;

            if (currentPage == "comments")
            {
                if (curCommentSort != newSort)
                {
                    curCommentSort = newSort;
                    SortAndFilterComments();
                }
            }
        }


        void UpdateSummaryButtons()
        {
            promoteBtn.IconUri = new Uri("/Images/Icons/white_promote.png", UriKind.Relative);
            demoteBtn.IconUri = new Uri("/Images/Icons/white_demote.png", UriKind.Relative);
            Blah curBlah = BlahguaAPIObject.Current.CurrentBlah;

            if (BlahguaAPIObject.Current.CurrentUser != null)
            {
                if (curBlah.A == BlahguaAPIObject.Current.CurrentUser._id)
                {
                    promoteBtn.IsEnabled = false;
                    demoteBtn.IsEnabled = false;
                }
                else if (curBlah.uv == 0)
                {
                    promoteBtn.IsEnabled = true;
                    demoteBtn.IsEnabled = true;
                }
                else
                {
                    promoteBtn.IsEnabled = false;
                    demoteBtn.IsEnabled = false;
                    if (curBlah.uv == 1)
                    {
                        promoteBtn.IconUri = new Uri("/Images/Icons/promote_active.png", UriKind.Relative); 
                    }
                    else
                    {
                        demoteBtn.IconUri = new Uri("/Images/Icons/demote_active.png", UriKind.Relative); 
                    }
                }
            }
        }

        private void SortAndFilterComments()
        {
            commentDataView.SortDescriptions.Clear();

            switch (curCommentSort)
            {
                case "byDateAsc":
                    commentDataView.SortDescriptions.Add(new SortDescription("c", ListSortDirection.Ascending));
                    break;
                case "byDateDesc":
                    commentDataView.SortDescriptions.Add(new SortDescription("c", ListSortDirection.Descending));
                    break;
                case "byPositive":
                    commentDataView.SortDescriptions.Add(new SortDescription("U", ListSortDirection.Descending));
                    break;
                case "byNegative":
                    commentDataView.SortDescriptions.Add(new SortDescription("D", ListSortDirection.Descending));
                    break;
                case "byPopular":
                    commentDataView.SortDescriptions.Add(new SortDescription("S", ListSortDirection.Descending));
                    break;
            }
        }


        void UpdateStatsPage()
        {
            if (BlahguaAPIObject.Current.CurrentUser == null)
            {
                DemoCharts.Visibility = Visibility.Collapsed;
                SignInStatPromp.Visibility = Visibility.Visible;
            }
            else
            {
                DemoCharts.Visibility = Visibility.Visible;
                SignInStatPromp.Visibility = Visibility.Collapsed;
            }
        }

        void UpdateCommentButtons()
        {
            promoteBtn.IconUri = new Uri("/Images/Icons/white_promote.png", UriKind.Relative);
            demoteBtn.IconUri = new Uri("/Images/Icons/white_demote.png", UriKind.Relative);
            Blah curBlah = BlahguaAPIObject.Current.CurrentBlah;

            if (curBlah.C > 0)
            {
                AllCommentList.Visibility = Visibility.Visible;
                NoCommentBox.Visibility = Visibility.Collapsed;
            }
            else
            {
                AllCommentList.Visibility = Visibility.Collapsed;
                NoCommentBox.Visibility = Visibility.Visible;
            }


            if (BlahguaAPIObject.Current.CurrentUser == null)
            {
                promoteBtn.IsEnabled = false;
                demoteBtn.IsEnabled = false;
                commentBtn.IsEnabled = false;
            }
            else
            {
                commentBtn.IsEnabled = true;
                Comment curComment = (Comment)AllCommentList.SelectedItem;

                if (curComment != null)
                {
                    // see if the user voted on it already
                    if (curComment.uv == 0)
                    {
                        promoteBtn.IsEnabled = true;
                        demoteBtn.IsEnabled = true;
                    }
                    else
                    {
                        promoteBtn.IsEnabled = false;
                        demoteBtn.IsEnabled = false;
                        if (curComment.uv == 1)
                        {
                            promoteBtn.IconUri = new Uri("/Images/Icons/promote_active.png", UriKind.Relative);
                        }
                        else
                        {
                            demoteBtn.IconUri = new Uri("/Images/Icons/demote_active.png", UriKind.Relative);
                        }
                    }
                }
                else
                {
                    promoteBtn.IsEnabled = false;
                    demoteBtn.IsEnabled = false;
                }
            }
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            App.analytics.PostPageView("/blah");

            if (this.DataContext != null)
            {
                if (currentPage == "comments")
                {
                    AllCommentList.ItemsSource = BlahguaAPIObject.Current.CurrentBlah.Comments;
                }

                // update the text
                UpdateButtonsForPage();
            }
           
          
        }

        void SetBlahBackground()
        {
            Brush newBrush;
            string typeName = "says";

            if (BlahguaAPIObject.Current.CurrentBlah != null)
                typeName = BlahguaAPIObject.Current.CurrentBlah.TypeName;
            else if (BlahguaAPIObject.Current.CurrentInboxBlah != null)
                typeName = BlahguaAPIObject.Current.CurrentInboxBlah.TypeName;

            switch (typeName)
            {
                case "leaks":
                    newBrush = (Brush)App.Current.Resources["BaseBrushLeaks"];
                    defaultImg = "/Images/bkgnds/leaks.jpg";
                    break;
                case "polls":
                    newBrush = (Brush)App.Current.Resources["BaseBrushPolls"];
                    defaultImg = "/Images/bkgnds/polls.jpg";
                    break;
                case "asks":
                    defaultImg = "/Images/bkgnds/asks.jpg";
                    newBrush = (Brush)App.Current.Resources["BaseBrushAsks"]; 
                    break;
                case "predicts":
                    newBrush = (Brush)App.Current.Resources["BaseBrushPredicts"];
                    defaultImg = "/Images/bkgnds/predicts.jpg";
                    break;
                default:
                    newBrush = (Brush)App.Current.Resources["BaseBrushSays"];
                    defaultImg = "/Images/bkgnds/says.jpg";
                    break;
            }

            BackgroundScreen.Fill = newBrush;

            
        }

        private void HandlePollInit()
        {
            BlahguaAPIObject.Current.GetUserPollVote((theVote) =>
                {
                    if ((theVote != null) && (theVote.W > -1))
                    {
                        PollItemList.ItemTemplate = (DataTemplate)Resources["PollVotedTemplate"];
                    }
                    ((Storyboard)Resources["ShowPollAnimation"]).Begin();
                }
            );
        }

        private void HandlePredictInit()
        {
            BlahguaAPIObject.Current.GetUserPredictionVote((theVote) =>
                {
                    Blah curBlah = BlahguaAPIObject.Current.CurrentBlah;
                    if (curBlah.E > DateTime.Now)
                    {
                        // still has time
                        PredictDateBox.Text = "happening by " + curBlah.E.ToShortDateString();
                        PredictElapsedTimeBox.Text = "(" + Utilities.ElapsedDateString(curBlah.E) + ")";
                        WillHappenItems.Visibility = Visibility.Visible;
                        AlreadyHappenedItems.Visibility = Visibility.Collapsed;
                        WillHappenItems.ItemsSource = curBlah.PredictionItems;
                    }
                    else
                    {
                        // expired
                        PredictDateBox.Text = "should have happened on " + curBlah.E.ToShortDateString();
                        PredictElapsedTimeBox.Text = "(" + Utilities.ElapsedDateString(curBlah.E) + ")";
                        WillHappenItems.Visibility = Visibility.Visible;
                        AlreadyHappenedItems.Visibility = Visibility.Collapsed;
                        AlreadyHappenedItems.ItemsSource = curBlah.ExpPredictionItems;
                    }

                    ((Storyboard)Resources["ShowPredictionAnimation"]).Begin();
                }
            );
        }

        private void HandlePromoteBlah(object target, EventArgs theArgs)
        {
            if (currentPage == "summary")
            {
                BlahguaAPIObject.Current.SetBlahVote(1, (newVote) =>
                    {
                        UpdateSummaryButtons();
                        App.analytics.PostBlahVote(1);
                    });
            }
            else
            {
                Comment curComment = (Comment)AllCommentList.SelectedItem;

                if (curComment != null)
                {
                    BlahguaAPIObject.Current.SetCommentVote(curComment, 1, (newVote) =>
                        {
                            UpdateCommentButtons();
                            App.analytics.PostCommentVote(1);
                        }
                    );
                }
            }
        }

        private void HandleSignin(object target, EventArgs theArgs)
        {
            NavigationService.Navigate(new Uri("/Screens/Signin.xaml", UriKind.Relative));    
        }

        private void HandleDemoteBlah(object target, EventArgs theArgs)
        {
            if (currentPage == "summary")
            {
                BlahguaAPIObject.Current.SetBlahVote(-1, (newVote) =>
                {
                    UpdateSummaryButtons();
                    App.analytics.PostBlahVote(-1);
                });
            }
            else
            {
                Comment curComment = (Comment)AllCommentList.SelectedItem;

                if (curComment != null)
                {
                    BlahguaAPIObject.Current.SetCommentVote(curComment, -1, (newVote) =>
                    {
                        UpdateCommentButtons();
                        App.analytics.PostCommentVote(-1);
                    }
                    );
                }
            }
        }

        private void HandleShareBlah(object target, EventArgs theArgs)
        {
            ShareLinkTask shareLinkTask = new ShareLinkTask();
            Blah curBlah = BlahguaAPIObject.Current.CurrentBlah;
            string blahURL = BlahguaAPIObject.Current.GetCurrentShareURL();
            string msgStr = curBlah.T;
            if ((msgStr == null) || (msgStr == ""))
            {
                msgStr = curBlah.F;
                if ((msgStr == null) || (msgStr == ""))
                {
                    msgStr = "a post from the " + curBlah.ChannelName + " channel";
                }
            }
            
            shareLinkTask.Title = "Shared from Blahgua";
            shareLinkTask.LinkUri = new Uri(blahURL, UriKind.Absolute);
            shareLinkTask.Message = msgStr; ;

            shareLinkTask.Show();
        }

        private void HandleAddComment(object target, EventArgs theArgs)
        {
            if (BlahguaAPIObject.Current.CreateCommentRecord == null)
            {
                BlahguaAPIObject.Current.CreateCommentRecord = new CommentCreateRecord();
                BlahguaAPIObject.Current.CreateCommentRecord.UseProfile = false;
            }

            if (currentPage == "summary")
            {
                BlahguaAPIObject.Current.CreateCommentRecord.CID = null;
                NavigationService.Navigate(new Uri("/Screens/CreateComment.xaml", UriKind.Relative));
            }
            else
            {
                // on the comment page
                Comment curComment = (Comment)AllCommentList.SelectedItem;
                if (curComment != null)
                    BlahguaAPIObject.Current.CreateCommentRecord.CID = curComment._id;
                else
                    BlahguaAPIObject.Current.CreateCommentRecord.CID = null;

                NavigationService.Navigate(new Uri("/Screens/CreateComment.xaml", UriKind.Relative));
            }
        }

        private void HandleReportItem(object target, EventArgs theArgs)
        {

        }

        private void HandleDeleteItem(object target, EventArgs theArgs)
        {

        }

        private void HandleImageOpened(object sender, RoutedEventArgs e)
        {
            BlahImage.MaxHeight = ((BitmapImage)BlahImage.Source).PixelHeight;
            BlahImage.MaxWidth = ((BitmapImage)BlahImage.Source).PixelWidth;
        }

        private void LoadComments()
        {
            BlahguaAPIObject.Current.LoadBlahComments((theList) =>
                {
                    commentDataView = new CollectionViewSource();
                    commentDataView.Source = theList;
                    SortAndFilterComments();
                    commentsLoaded = true;
                    AllCommentList.ItemsSource = commentDataView.View;
                    NoCommentBox.Visibility = Visibility.Collapsed;
                });
            
        }

        private void LoadStats()
        {
            BlahguaAPIObject.Current.LoadBlahStats((theStats) =>
            {
                statsLoaded = true;
                NoStatsBox.Visibility = Visibility.Collapsed;
                StatsArea.Visibility = Visibility.Visible;
                StatsArea.DataContext = BlahguaAPIObject.Current.CurrentBlah;
                DrawBlahStats();
            });

        }

        private void DrawBlahStats()
        {
            Blah curBlah = BlahguaAPIObject.Current.CurrentBlah;
            Stats stats = curBlah.L;
            int maxVal;

            // blahgua score

            CategoricalSeries newSeries;
            newSeries = new BarSeries();
            CategoricalDataPoint newPoint = new CategoricalDataPoint();
            newPoint.Value = curBlah.S;
            newPoint.Category = "";

            
            newSeries.DataPoints.Add(newPoint);
            ScoreChart.Series.Add(newSeries);   

            // votes

            if ((curBlah.P > 0) || (curBlah.D > 0))
            {
                maxVal = 2 + Math.Max(curBlah.P, curBlah.D);
                if (maxVal < 5)
                    maxVal = 5;

                newSeries = new BarSeries();
                newSeries.PointTemplates.Add((DataTemplate)Resources["GreenPalette"]);
                newSeries.PointTemplates.Add((DataTemplate)Resources["RedPalette"]);
                newPoint = new CategoricalDataPoint();
                newPoint.Value = curBlah.P;
                newPoint.Category = "promotes";
                newSeries.DataPoints.Add(newPoint);
                newPoint = new CategoricalDataPoint();
                newPoint.Value = curBlah.D;
                newPoint.Category = "demotes";
                newSeries.DataPoints.Add(newPoint);
                ((LinearAxis)VoteChart.HorizontalAxis).Maximum = (int)maxVal;

                double step = (double)maxVal / 5;
                ((LinearAxis)VoteChart.HorizontalAxis).MajorStep = (int)Math.Round(step);
                VoteChart.Series.Add(newSeries);
            }
            

            // views
            if (stats.HasViews)
            {
                maxVal = 0;
                newSeries = new SplineAreaSeries();
                for (int i = 0; i < stats.Count; i++)
                {
                    newPoint = new CategoricalDataPoint();
                    newPoint.Value = stats.Impressions[i];
                    if (newPoint.Value > maxVal)
                        maxVal = (int)newPoint.Value;
                    newPoint.Category = stats[i].StatDate;
                    newSeries.DataPoints.Add(newPoint);
                }

                ViewChart.Series.Add(newSeries);
                maxVal += 2;
                if (maxVal < 5)
                    maxVal = 5;
                double step = (double)maxVal / 5;
                ((LinearAxis)ViewChart.VerticalAxis).MajorStep = (int)Math.Round(step);
                ((LinearAxis)ViewChart.VerticalAxis).Maximum = (int)maxVal;
            }

            // opens
            if (stats.HasOpens)
            {
                maxVal = 0;
                newSeries = new SplineAreaSeries();
                for (int i = 0; i < stats.Count; i++)
                {
                    newPoint = new CategoricalDataPoint();
                    newPoint.Value = stats.Opens[i];
                    if (newPoint.Value > maxVal)
                        maxVal = (int)newPoint.Value;
                    newPoint.Category = stats[i].StatDate;
                    newSeries.DataPoints.Add(newPoint);
                }
                OpenChart.Series.Add(newSeries);
                maxVal += 2;
                if (maxVal < 5)
                    maxVal = 5;
                double step = (double)maxVal / 5;
                ((LinearAxis)OpenChart.VerticalAxis).MajorStep = (int)Math.Round(step);
                ((LinearAxis)OpenChart.VerticalAxis).Maximum = (int)maxVal;
            }
 

            // comments
            if (stats.HasComments)
            {
                maxVal = 0;
                newSeries = new SplineAreaSeries();
                for (int i = 0; i < stats.Count; i++)
                {
                    newPoint = new CategoricalDataPoint();
                    newPoint.Value = stats.Comments[i];
                    if (newPoint.Value > maxVal)
                        maxVal = (int)newPoint.Value;
                    newPoint.Category = stats[i].StatDate;
                    newSeries.DataPoints.Add(newPoint);
                }
                CommentChart.Series.Add(newSeries);
                maxVal += 2;
                if (maxVal < 5)
                    maxVal = 5;
                double step = (double)maxVal / 5;
                ((LinearAxis)CommentChart.VerticalAxis).MajorStep = (int)Math.Round(step);
                ((LinearAxis)CommentChart.VerticalAxis).Maximum = (int)maxVal;
            }

            // gender
            if (BlahguaAPIObject.Current.CurrentUser != null)
            {
                UserProfile profile = BlahguaAPIObject.Current.CurrentUser.Profile;

                if (profile != null)
                {
                    CreateDemoChart(GenderChart, "B");

                    // age
                    CreateDemoChart(AgeChart, "C");


                    // race
                    CreateDemoChart(RaceChart, "D");


                    // income
                    CreateDemoChart(IncomeChart, "E");

                    // country
                    CreateDemoChart(CountryChart, "J");
                }
                
            }

        }

        private void CreateDemoChart(RadCartesianChart theChart, string demoProp)
        {
            bool visible = false;

            if ((BlahguaAPIObject.Current.CurrentUser != null) &&
                (BlahguaAPIObject.Current.CurrentUser.Profile != null))
            {
                UserProfile  theProfile = BlahguaAPIObject.Current.CurrentUser.Profile;
                PropertyInfo theProp = theProfile.GetType().GetProperty(demoProp);
                if (theProp != null)
                {
                    string theVal = (string)theProp.GetValue(theProfile, null);
                    if (theVal != "-1")
                        visible = true;
                }
            }

            if (visible)
            {
                Dictionary<string, string> curDict = BlahguaAPIObject.Current.UserProfileSchema.GetTypesForProperty(demoProp);

                CategoricalDataPoint promotePoint, demotePoint;
                CategoricalSeries promoteSeries = new BarSeries();
                CategoricalSeries demoteSeries = new BarSeries();
                Blah curBlah = BlahguaAPIObject.Current.CurrentBlah;
                DemoProfileSummaryRecord upVotes = curBlah._d._u;
                DemoProfileSummaryRecord downVotes = curBlah._d._d;

                promoteSeries.CombineMode = ChartSeriesCombineMode.Stack;
                demoteSeries.CombineMode = ChartSeriesCombineMode.Stack;
                int maxVal = 0;
                foreach (string curVal in curDict.Keys)
                {
                    promotePoint = new CategoricalDataPoint();
                    promotePoint.Category = curDict[curVal];
                    promotePoint.Value = upVotes.GetPropertyValue(demoProp, curVal);// curBlah._d._u.B.GetValue(curVal);
                    promoteSeries.DataPoints.Add(promotePoint);


                    demotePoint = new CategoricalDataPoint();
                    demotePoint.Category = curDict[curVal];
                    demotePoint.Value = downVotes.GetPropertyValue(demoProp, curVal);
                    demoteSeries.DataPoints.Add(demotePoint);

                    if ((promotePoint.Value + demotePoint.Value) > maxVal)
                        maxVal = (int)(promotePoint.Value + demotePoint.Value);
                }
                if (maxVal > 0)
                {
                    maxVal += 2;
                    if (maxVal < 5)
                        maxVal = 5;
                    ((LinearAxis)theChart.VerticalAxis).Maximum = maxVal;
                    if (maxVal < 5)
                        maxVal = 5;
                    double step = (double)maxVal / 5;
                    ((LinearAxis)theChart.VerticalAxis).MajorStep = (int)Math.Round(step);

                    theChart.Series.Add(promoteSeries);
                    theChart.Series.Add(demoteSeries);
                }
            }
            else
            {
                theChart.EmptyContent = "Set this attribute in your profile to see it for others.";
                theChart.EmptyContentTemplate = (DataTemplate)Resources["HiddenChartTemplate"];
            }
        }

        private void OnPivotLoading(object sender, PivotItemEventArgs e)
        {
            string newItem = e.Item.Header.ToString();

            if (newItem == "comments")
            {
                if (BlahguaAPIObject.Current.CurrentBlah.C == 0)
                {
                    NoCommentBox.Visibility = Visibility.Visible;
                    NoCommentProgress.Visibility = Visibility.Collapsed;
                    NoCommentTextBox.Text = "This post has no comments.\nMaybe you can add the first!";
                }
                else
                {
                    if (commentsLoaded)
                        NoCommentBox.Visibility = Visibility.Collapsed;
                    else
                    {
                        NoCommentBox.Visibility = Visibility.Visible;
                        NoCommentTextBox.Text = "loading comments";
                        NoCommentProgress.Visibility = Visibility.Visible;
                        LoadComments();
                    }
                }
            }
            else if (newItem == "stats")
            {
                if (!statsLoaded)
                {
                    NoStatsBox.Visibility = Visibility.Visible;
                    LoadStats();

                }
            }
            
            
            if ((newItem == "summary") && (currentPage == "stats"))
            {
                // wrap around
                BackgroundImage2.Visibility = Visibility.Visible;

                Storyboard sb = new Storyboard();
                DoubleAnimation db1 = new DoubleAnimation();
                DoubleAnimation db2 = new DoubleAnimation();
                ExponentialEase ease = new ExponentialEase();
                ease.Exponent = 5;
                ease.EasingMode = EasingMode.EaseIn;

                db1.EasingFunction = ease;
                db1.BeginTime = TimeSpan.FromSeconds(0);
                db1.Duration = TimeSpan.FromSeconds(.5);
                db1.To = -800;
                Storyboard.SetTarget(db1, BackgroundImage);
                Storyboard.SetTargetProperty(db1, new PropertyPath("(Canvas.Left)"));
                sb.Children.Add(db1);

                db2.EasingFunction = ease;
                db2.BeginTime = TimeSpan.FromSeconds(0);
                db2.Duration = TimeSpan.FromSeconds(.5);
                db2.To = 0;
                Storyboard.SetTarget(db2, BackgroundImage2);
                Storyboard.SetTargetProperty(db2, new PropertyPath("(Canvas.Left)"));
                sb.Children.Add(db2);

                sb.Completed += sbWrap_Completed;
                sb.Begin();
            }
            else if ((newItem == "stats") && (currentPage == "summary"))
            {
                // back up
                // wrap around
                BackgroundImage2.Visibility = Visibility.Visible;

                Storyboard sb = new Storyboard();
                DoubleAnimation db1 = new DoubleAnimation();
                DoubleAnimation db2 = new DoubleAnimation();
                ExponentialEase ease = new ExponentialEase();
                ease.Exponent = 5;
                ease.EasingMode = EasingMode.EaseIn;

                db1.EasingFunction = ease;
                db1.BeginTime = TimeSpan.FromSeconds(0);
                db1.Duration = TimeSpan.FromSeconds(.5);
                db1.From = -800;
                db1.To = -320;
                Storyboard.SetTarget(db1, BackgroundImage);
                Storyboard.SetTargetProperty(db1, new PropertyPath("(Canvas.Left)"));
                sb.Children.Add(db1);

                db2.EasingFunction = ease;
                db2.BeginTime = TimeSpan.FromSeconds(0);
                db2.Duration = TimeSpan.FromSeconds(.5);
                db2.From = 0;
                db2.To = 480;
                Storyboard.SetTarget(db2, BackgroundImage2);
                Storyboard.SetTargetProperty(db2, new PropertyPath("(Canvas.Left)"));
                sb.Children.Add(db2);

                sb.Completed += sbBackWrap_Completed;
                sb.Begin();

            }
            else
            {
                // do the background
                Storyboard sb = new Storyboard();
                DoubleAnimation db = new DoubleAnimation();
                double targetVal = 0;
                double maxScroll = -320;
                double offset;

                if (BlahDetailsPivot.Items.Count() > 1)
                    offset = maxScroll / (BlahDetailsPivot.Items.Count() - 1);
                else
                    offset = 0;
                ExponentialEase ease = new ExponentialEase();
                ease.Exponent = 5;
                ease.EasingMode = EasingMode.EaseIn;

                targetVal = offset * BlahDetailsPivot.Items.IndexOf(e.Item);
                db.EasingFunction = ease;
                db.BeginTime = TimeSpan.FromSeconds(0);
                db.Duration = TimeSpan.FromSeconds(.5);
                db.To = targetVal;
                Storyboard.SetTarget(db, BackgroundImage);
                Storyboard.SetTargetProperty(db, new PropertyPath("(Canvas.Left)"));
                sb.Children.Add(db);
                sb.Begin();
            }
        }

        void sbWrap_Completed(object sender, EventArgs e)
        {
            BackgroundImage2.Visibility = Visibility.Collapsed;
            Canvas.SetLeft(BackgroundImage2, 480);
            Canvas.SetLeft(BackgroundImage, 0);
        }

        void sbBackWrap_Completed(object sender, EventArgs e)
        {
            BackgroundImage2.Visibility = Visibility.Collapsed;
            Canvas.SetLeft(BackgroundImage2, 480);
            Canvas.SetLeft(BackgroundImage, -320);
        }

        private void UpdateButtonsForPage()
        {
            if (this.DataContext == null)
                return;

            ApplicationBar.Buttons.Clear();
            ApplicationBar.MenuItems.Clear();
            App.analytics.PostPageView("/blah/" + currentPage);
            if (BlahguaAPIObject.Current.CurrentUser != null)
            {
                switch (currentPage)
                {
                    case "summary":
                        ApplicationBar.Buttons.Add(promoteBtn);
                        ApplicationBar.Buttons.Add(demoteBtn);
                        ApplicationBar.Buttons.Add(commentBtn);
                        ApplicationBar.Buttons.Add(shareBtn);

                        ApplicationBar.MenuItems.Add(reportItem);
                        ApplicationBar.IsVisible = true;
                        UpdateSummaryButtons();
                        break;

                    case "comments":
                        ApplicationBar.Buttons.Add(promoteBtn);
                        ApplicationBar.Buttons.Add(demoteBtn);
                        ApplicationBar.Buttons.Add(commentBtn);
                        ApplicationBar.MenuItems.Add(sortByDateDesc);
                        ApplicationBar.MenuItems.Add(sortByDateAsc);
                        ApplicationBar.MenuItems.Add(sortByStrength);
                        ApplicationBar.MenuItems.Add(sortByUpVote);
                        ApplicationBar.MenuItems.Add(sortByDownVote);
                        ApplicationBar.MenuItems.Add(reportItem);
                        ApplicationBar.IsVisible = true;
                        UpdateCommentButtons();
                        break;

                    case "stats":
                        ApplicationBar.IsVisible = false;
                        UpdateStatsPage();
                        break;

                    default:


                        break;
                }
            }
            else
            {
                ApplicationBar.Buttons.Add(signInBtn);
                switch (currentPage)
                {
                    case "summary":
                        ApplicationBar.Buttons.Add(shareBtn);
                        break;

                    case "stats":
                        UpdateStatsPage();
                        break;
                }
                    
                ApplicationBar.IsVisible = true;    
            }
        }

        private void HandlePivotLoaded(object sender, PivotItemEventArgs e)
        {
            currentPage = e.Item.Header.ToString();
            UpdateButtonsForPage();
        }

        private void HandlePivotUnloaded(object sender, PivotItemEventArgs e)
        {
            ApplicationBar.Buttons.Clear();
            ApplicationBar.MenuItems.Clear();
        }

        private void NavigationInTransition_EndTransition(object sender, RoutedEventArgs e)
        {
           
        }

        private void AllCommentList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBoxItem curItem;

            foreach (Comment oldComment in e.RemovedItems)
            {
                curItem = (ListBoxItem)AllCommentList.ItemContainerGenerator.ContainerFromItem(oldComment);
                if (curItem != null)
                    curItem.Background = new SolidColorBrush(Colors.Transparent);
            }

            foreach (Comment newComment in e.AddedItems)
            {
                curItem = (ListBoxItem)AllCommentList.ItemContainerGenerator.ContainerFromItem(newComment);
                if (curItem != null)
                    curItem.Background = new SolidColorBrush(Color.FromArgb(128,0,0,0));
            }

            UpdateCommentButtons();
        }

        private void PollVote_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            PollItem newVote = (PollItem)((TextBlock)sender).DataContext;

            BlahguaAPIObject.Current.SetPollVote(newVote, (resultStr) =>
                {
                    PollItemList.ItemTemplate = (DataTemplate)Resources["PollVotedTemplate"];
                }
            );
        }

        private void PredictVote_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            PollItem newVote = (PollItem)((TextBlock)sender).DataContext;

            BlahguaAPIObject.Current.SetPredictionVote(newVote, (resultStr) =>
                {
                     if (BlahguaAPIObject.Current.CurrentBlah.IsPredictionExpired)
                     {
                         AlreadyHappenedItems.ItemsSource = BlahguaAPIObject.Current.CurrentBlah.ExpPredictionItems;
                         AlreadyHappenedItems.ItemTemplate = (DataTemplate)Resources["PredictVotedTemplate"];
                     }
                     else
                     {
                        WillHappenItems.ItemsSource = BlahguaAPIObject.Current.CurrentBlah.PredictionItems;
                        WillHappenItems.ItemTemplate = (DataTemplate)Resources["PredictVotedTemplate"];
                     }

                }
            );
        }

        private void BlahImage_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            string urlString = BlahguaAPIObject.Current.CurrentBlah.ImageURL;
            if (urlString != null)
            {
                App.analytics.PostPageView("/ImageViewer");
                NavigationService.Navigate(new Uri("/Screens/ImageViewer.xaml", UriKind.Relative));    
            }
        }

        internal static class Util
        {
            public delegate void MethodInvoker();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            BlahguaAPIObject.Current.SetCurrentBlahFromId(App.BlahIdToOpen, (theBlah) =>
                {

                    BlahLoadingBox.Visibility = Visibility.Collapsed;

                    this.DataContext = BlahguaAPIObject.Current;
                    if (BlahguaAPIObject.Current.CurrentBlah != null)
                    {
                        if (theBlah.ImageURL == null)
                        {
                            ImageSource defaultSrc = new BitmapImage(new Uri(defaultImg, UriKind.Relative));
                            BackgroundImage.Source = defaultSrc;
                            BackgroundImage2.Source = defaultSrc;
                        }

                        BlahSummaryArea.Visibility = Visibility.Visible;
                        UpdateButtonsForPage();
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
                        MessageBox.Show("unable to load blah.  Sorry!");
                        App.analytics.PostSessionError("loadblahfailed");
                        NavigationService.GoBack();
                    }
                }
                );
        }

        

    }
}