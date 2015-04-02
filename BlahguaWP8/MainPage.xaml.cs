﻿using System;
using System.Collections;
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
using System.Windows.Threading;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using BlahguaMobile.BlahguaCore;
using Microsoft.Phone.Tasks;
using Microsoft.Advertising.Mobile.UI;

namespace BlahguaMobile.Winphone
{
    public partial class MainPage : PhoneApplicationPage
    {
        Inbox   blahList;
        DispatcherTimer scrollTimer = new DispatcherTimer();
        int inboxCounter = 0;
        private readonly String rowSequence = "ABEAFADCADEACDAFAEBADADCAFABEAEBAFACDAEA";

        int screenMargin = 12;
        int blahMargin = 12;
        double smallBlahSize, mediumBlahSize, largeBlahSize;
        bool AtScrollEnd = false;
        int FramesPerSecond = 60;
        BlahRollItem targetBlah = null;
        DispatcherTimer BlahAnimateTimer = new DispatcherTimer();
        DispatcherTimer WelcomeTimer = new DispatcherTimer();
        Dictionary<string, int> ImpressionMap = new Dictionary<string, int>();
        int maxScroll = 0;
        Storyboard sb = null;
        WhatsNewInfo _savedNewInfo = null;
        DispatcherTimer loadTimer = new DispatcherTimer();
        bool isFrontMost = false;

        public static bool ReturningFromTutorial { get; set; }
     
        // Constructor
        public MainPage()
        {
            ReturningFromTutorial = false;
            Loaded += new RoutedEventHandler(MainPage_Loaded); 
            InitializeComponent();
            this.DataContext = null;
            BlahAnimateTimer.Tick += BlahAnimateTimer_Tick;
            BlahAnimateTimer.Interval = new TimeSpan(0,0,2);

            WelcomeTimer.Tick += WelcomeTimer_Tick;



            
        }


        protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            isFrontMost = false;
            StopTimers();
            FlushImpressionList();
 	        base.OnNavigatingFrom(e);
        }
       


        void BlahScroller_MouseMove(object sender, MouseEventArgs e)
        {
            DetectScrollAtEnd();
        }

        void DetectScrollAtEnd()
        {
            maxScroll = Math.Max((int)BlahScroller.VerticalOffset, maxScroll);
            if ((BlahScroller.VerticalOffset == BlahScroller.ScrollableHeight) && (BlahContainer.Children.Count > 10))
            {
                if (!AtScrollEnd)
                {
                    AtScrollEnd = true;
                    FetchNextBlahList();
                }

            }
           
        }

        private void InitService()
        {
            LoadingMessageBox.Text = "looking for server...";
            loadTimer.Stop();
            loadTimer.Interval = TimeSpan.FromSeconds(10);
            loadTimer.Tick += (theObj, theArgs) =>
            {
                LoadingMessageBox.Text = "still looking...";
            };
            loadTimer.Start();

            BlahguaAPIObject.Current.Initialize(null, DoServiceInited); 

            
        }


        private void FetchInitialBlahList(bool secondTry = false)
        {
            LoadingMessageBox.Text = "loading...";
            loadTimer.Stop();
            loadTimer.Interval = TimeSpan.FromSeconds(10);
            loadTimer.Tick += (theObj, theArgs) =>
            {
                LoadingMessageBox.Text = "still loading...";
            };
            loadTimer.Start();

            BlahguaAPIObject.Current.GetInbox((newBlahList) =>
                {
                    loadTimer.Stop();
                    if (newBlahList == null)
                        newBlahList = new Inbox();
                    blahList = newBlahList;
                    blahList.PrepareBlahs();
                    if (blahList.Count == 100)
                    {
                        RenderInitialBlahs();
                        StartTimers();
                        LoadingBox.Visibility = Visibility.Collapsed;
                    }
                    else if (!secondTry)
                    {
                        MessageBox.Show("We had a problem loading.  Press OK and we will try it again.");
                        LoadingMessageBox.Text = "retrying load...";
                        FetchInitialBlahList(true);
                    }
                    else
                    {
                        MessageBox.Show("Well, thanks for trying.  Looks like there is a server issue.  Please go ahead and leave the app and try again later.");
                        LoadingBox.Visibility = Visibility.Collapsed;
                        ConnectFailure.Visibility = Visibility.Visible;
                    }

                   
                });
        }

        void StartTimers()
        {
            if (isFrontMost)
            {
                targetBlah = null;
                scrollTimer.Start();
                MaybeAnimateElement();
            }
        }

        void StopTimers()
        {
            scrollTimer.Stop();
            AnimateTextFadeIn.Stop();
            AnimateTextFadeOut.Stop();
            targetBlah = null;
        }

        private void FetchNextBlahList()
        {
            LoadingMessageBox.Text = "loading...";
            loadTimer.Stop();
            loadTimer.Interval = TimeSpan.FromSeconds(10);
            loadTimer.Tick += (theObj, theArgs) =>
            {
                LoadingMessageBox.Text = "still loading...";
            };
            loadTimer.Start();
            BlahguaAPIObject.Current.GetInbox((newBlahList) =>
            {
                loadTimer.Stop();
                if (newBlahList != null)
                { 
                    blahList = newBlahList;
                    blahList.PrepareBlahs();
                    InsertAdditionalBlahs();
                    AtScrollEnd = false;
                    inboxCounter++;
                    if (inboxCounter >= 5)
                    {
                        UIElement curItem;
                        double bottom = 0;
                        // remove some blahs...
                        for (int i = 0; i < 101; i++)
                        {
                            curItem = BlahContainer.Children[0];
                            if (curItem is BlahRollItem)
                            {
                                BlahRollItem curBlah = (BlahRollItem)curItem;
                                AddImpression(curBlah.BlahData.I);
                            }

                            BlahContainer.Children.Remove(curItem);
                        }

                        bottom = Canvas.GetTop(BlahContainer.Children[0]);

                        // now shift everything up
                        foreach (UIElement theBlah in BlahContainer.Children)
                        {
                            Canvas.SetTop(theBlah, Canvas.GetTop(theBlah) - bottom);
                        }
                        BlahScroller.ScrollToVerticalOffset(BlahScroller.VerticalOffset - bottom);
                        BlahContainer.Height -= bottom;
                        maxScroll -= (int)bottom;
                        inboxCounter--;
                    }

                }
                App.analytics.PostPageView("/channel/" + BlahguaAPIObject.Current.CurrentChannel.ChannelName);

            });
        }

        private void AddImpression(string blahId)
        {
            if (!ImpressionMap.ContainsKey(blahId))
            {
                ImpressionMap[blahId] = 1;
            }
            else
                ImpressionMap[blahId]++;
        }

        private void FlushImpressionList()
        {
            foreach (UIElement curItem in BlahContainer.Children)
            {
                if (curItem is BlahRollItem)
                {
                    if (Canvas.GetTop(curItem) < maxScroll)
                        AddImpression(((BlahRollItem)curItem).BlahData.I);
                }

            }

            BlahguaAPIObject.Current.RecordImpressions(ImpressionMap);

            ImpressionMap.Clear();
        }

        private void ScrollBlahRoll(object sender, EventArgs e)
        {
            
            double curOffset = BlahScroller.VerticalOffset;
            curOffset += 1.0;
            BlahScroller.ScrollToVerticalOffset(curOffset);
            DetectScrollAtEnd();
            
        }

        
        

        private void RenderInitialBlahs()
        {
            double curTop = screenMargin;
            smallBlahSize = (480 - ((screenMargin * 2) + (blahMargin * 2))) / 3;
            mediumBlahSize = smallBlahSize + smallBlahSize + blahMargin;
            largeBlahSize = 456; // mediumBlahSize + mediumBlahSize + blahMargin;
            BlahguaAPIObject.smallTileSize = (int)smallBlahSize;
            BlahguaAPIObject.mediumTileSize = (int)mediumBlahSize;
            BlahguaAPIObject.largeTileSize = (int)largeBlahSize;


            foreach (char rowType in rowSequence)
            {
                curTop = InsertRow(rowType, curTop);
                curTop += blahMargin;
            }


           
            curTop = InsertAd(curTop);
            

            BlahContainer.Height = curTop + screenMargin;
            inboxCounter++;
        }

        private double InsertAd(double curTop)
        {
            if (false) // true
            {
                // insert an add
                AdControl theAd = new AdControl();
                theAd.IsAutoCollapseEnabled = false;
                theAd.IsAutoRefreshEnabled = true;
                theAd.AdUnitId = "Image480_80";
                theAd.ApplicationId = "test_client";
                theAd.IsEnabled = true;
                theAd.Width = 480;
                theAd.Height = 80;
                Canvas.SetLeft(theAd, 0);
                Canvas.SetTop(theAd, curTop);
                BlahContainer.Children.Add(theAd);

                return curTop + 80;
            }
            else
                return curTop;
            
        }

        private void ClearBlahs()
        {
            inboxCounter = 0;
            BlahContainer.Children.Clear();
            BlahScroller.ScrollToVerticalOffset(0);
        }

        private void InsertAdditionalBlahs()
        {
            double curTop = BlahContainer.Height;
            foreach (char rowType in rowSequence)
            {
                curTop = InsertRow(rowType, curTop);
                curTop += blahMargin;
            }

            curTop = InsertAd(curTop);

            BlahContainer.Height = curTop + blahMargin;
        }

        private void InsertElementForBlah(InboxBlah theBlah, double xLoc, double yLoc, double width, double height)
        {
            BlahRollItem newBlahItem = new BlahRollItem();
            newBlahItem.Initialize(theBlah);
            newBlahItem.Width = width;
            newBlahItem.Height = height;
            Canvas.SetLeft(newBlahItem, xLoc);
            Canvas.SetTop(newBlahItem, yLoc);

            BlahContainer.Children.Add(newBlahItem);
            newBlahItem.ScaleTextToFit();
        }

        private void HandleScrollStart(object sender, ManipulationStartedEventArgs e)
        {
            scrollTimer.Stop();
        }

        bool alreadyHookedScrollEvents = false;

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            isFrontMost = true;

            if (alreadyHookedScrollEvents)
                return;
            TiltEffect.TiltableItems.Add(typeof(BlahRollItem));
            alreadyHookedScrollEvents = true;
            // Visual States are always on the first child of the control template 
            FrameworkElement element = VisualTreeHelper.GetChild(BlahScroller, 0) as FrameworkElement;
            if (element != null)
            {
                VisualStateGroup group = FindVisualState(element, "ScrollStates");
                if (group != null)
                {
                    group.CurrentStateChanging += ScrollStateHandler;
                }
            }

            scrollTimer.Interval = new TimeSpan(TimeSpan.TicksPerSecond / FramesPerSecond);
            scrollTimer.Tick += ScrollBlahRoll;

            BlahScroller.MouseMove += BlahScroller_MouseMove;

            BlahContainer.Tap += BlahContainer_Tap;
            BlahContainer.ManipulationCompleted += BlahContainer_ManipulationCompleted;
            BlahguaAPIObject.Current.PropertyChanged += new PropertyChangedEventHandler(On_API_PropertyChanged);

            InitService();

            
        }

        void BlahContainer_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            Point finalVel = e.FinalVelocities.LinearVelocity;
            Point finalDelta = e.TotalManipulation.Translation;

            if ((Math.Abs(finalDelta.X) > Math.Abs(finalDelta.Y)) &&
                (Math.Abs(finalVel.X) > Math.Abs(finalVel.Y)))
            {
                if (finalDelta.X < 0)
                    BlahguaAPIObject.Current.GoNextChannel();
                else
                    BlahguaAPIObject.Current.GoPrevChannel();

            }
            else
            {
                
            }
        }



        void On_API_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (isFrontMost)
            {
                switch (e.PropertyName)
                {
                    case "CurrentChannel":
                        BlahguaAPIObject.Current.GetCurrentChannelPermission((thePerms) =>
                        {
                            OnChannelChanged();
                        });

                        break;
                }
            }
            
        }

        void BlahContainer_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            bool startScroll = true;

            if (e.OriginalSource != sender)
            {
                FrameworkElement curEl = (FrameworkElement)e.OriginalSource;

                while (curEl.GetType() != typeof(BlahRollItem))
                {
                    curEl = (FrameworkElement)curEl.Parent;
                    if (curEl == null)
                        break;
                }

                if (curEl != null)
                {
                    BlahRollItem curBlah = (BlahRollItem)curEl;
                    OpenBlahItem(curBlah);
                    startScroll = false;
                }

            }
            if (startScroll)
            {
                if (!scrollTimer.IsEnabled)
                    scrollTimer.Start();
            }

        }

        void OpenBlahItem(BlahRollItem curBlah)
        {
            StopTimers();
            BlahguaAPIObject.Current.CurrentInboxBlah = curBlah.BlahData;
            App.BlahIdToOpen = curBlah.BlahData.I;
            NavigationService.Navigate(new Uri("/Screens/BlahDetails.xaml", UriKind.Relative));

        }


        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (BlahguaAPIObject.Current.CurrentUser != null)
            {
                UserInfoBtn.Visibility = Visibility.Visible;
                if (BlahguaAPIObject.Current.CanPost)
                    NewBlahBtn.Visibility = Visibility.Visible;
                else
                    NewBlahBtn.Visibility = Visibility.Collapsed;

                SignInBtn.Visibility = Visibility.Collapsed;
            }
            else
            {
                UserInfoBtn.Visibility = Visibility.Collapsed;
                NewBlahBtn.Visibility = Visibility.Collapsed;
                SignInBtn.Visibility = Visibility.Visible;
            }

            if (BlahguaAPIObject.Current.NewBlahToInsert != null)
            {
                InsertBlahInStream(BlahguaAPIObject.Current.NewBlahToInsert);
                BlahguaAPIObject.Current.NewBlahToInsert = null;
                ShowNewBlahFloater();
            }

            StartTimers();

            if (ReturningFromTutorial)
            {
                OnChannelChanged();
                ReturningFromTutorial = false;
                CheckForWhatsNew();
            }
        }


        void InsertBlahInStream(Blah theBlah)
        {
            BlahRollItem newItem = null;
            double top = BlahScroller.VerticalOffset;
            double bottom = top + 800;

            foreach (UIElement curItem in BlahContainer.Children)
            {
                newItem = (BlahRollItem)curItem;
                if ((Canvas.GetTop(newItem) > bottom) && (newItem.BlahData.displaySize == 2))
                    break;
                else
                    newItem = null;
            }

            if (newItem == null)
            {
                for (int curIndex = BlahContainer.Children.Count - 1; curIndex >= 0; curIndex--)
                {
                    newItem = (BlahRollItem)BlahContainer.Children[curIndex];
                    if (newItem.BlahData.displaySize == 2)
                        break;
                    else
                        newItem = null;
                }
            }

            if (newItem != null)
            {
                newItem.Initialize(new InboxBlah(theBlah));
            }
        }


        void OpenFullBlah(Blah theBlah)
        {
            if (theBlah != null)
            {
                
            }
            else
            {
                MessageBox.Show("Blah failed to load");
            }
        }

        private void CheckForWhatsNew()
        {
            BlahguaAPIObject.Current.GetWhatsNew((whatsNew) =>
            {
                if ((whatsNew != null))
                {
                    ShowNewsFloater(whatsNew);
                }
            });
        }

        void DoServiceInited(bool didIt)
        {
            loadTimer.Stop();
            if (didIt)
            {
                this.DataContext = BlahguaAPIObject.Current;

                if (true)
                {
                    StopTimers();
                    NavigationService.Navigate(new Uri("/Screens/OnboardingScreen.xaml", UriKind.Relative));
                }
                else if (BlahguaAPIObject.Current.CurrentUser != null)
                {
                    App.analytics.PostAutoLogin();
                    UserInfoBtn.Visibility = Visibility.Visible;
                    NewBlahBtn.Visibility = Visibility.Visible;
                    SignInBtn.Visibility = Visibility.Collapsed;
                }
                else
                {
                    UserInfoBtn.Visibility = Visibility.Collapsed;
                    NewBlahBtn.Visibility = Visibility.Collapsed;
                    SignInBtn.Visibility = Visibility.Visible;
                    bool seenTutorial = (bool)BlahguaAPIObject.Current.SafeLoadSetting("seentutorial", false);

                    if (!seenTutorial)
                    {
                        StopTimers();
                        NavigationService.Navigate(new Uri("/Screens/OnboardingScreen.xaml", UriKind.Relative));
                    }
                }
            }
            else
            {
                LoadingBox.Visibility = Visibility.Collapsed;
                ConnectFailure.Visibility = Visibility.Visible;
            }
        }

        private void ShowNewBlahFloater()
        {
            WelcomeMessage.Visibility = Visibility.Visible;
            WelcomeMessage.Opacity = 1;
            WelcomeTextBox.Text = "Your post has been created. Look for it in the stream, and track its performance in your profile stats!";
            UserStatsBox.Visibility = Visibility.Collapsed;
            NewMessageBox.Visibility = Visibility.Collapsed;

            // animate it
            WelcomeMessage.Opacity = 1.0;
            WelcomeTransform.Y = 400;
            sb = new Storyboard();
            DoubleAnimation db1 = new DoubleAnimation();

            ExponentialEase ease = new ExponentialEase();
            ease.Exponent = 5;
            ease.EasingMode = EasingMode.EaseIn;

            db1.EasingFunction = ease;
            db1.BeginTime = TimeSpan.FromSeconds(0);
            db1.Duration = TimeSpan.FromSeconds(3);
            db1.From = 400;
            db1.To = 0;
            Storyboard.SetTarget(db1, WelcomeTransform);
            Storyboard.SetTargetProperty(db1, new PropertyPath("Y"));
            sb.Children.Add(db1);

            sb.Completed += sbWrap_Completed;

            sb.Begin();
        }


        private void ShowNewsFloater(WhatsNewInfo newInfo)
        {
            _savedNewInfo = newInfo;
            if (!String.IsNullOrEmpty(newInfo.message))
            {
                NewMessageBox.Text = newInfo.message;
                NewMessageBox.Visibility = Visibility.Visible;
            }
            else
                NewMessageBox.Visibility = Visibility.Collapsed;

            //bool statShown = false;

            WhatsNewSummaryBox.Text = newInfo.SummaryString;

            if (!String.IsNullOrEmpty(WhatsNewSummaryBox.Text))
                UserStatsBox.Visibility = Visibility.Visible;
            else
                UserStatsBox.Visibility = Visibility.Collapsed;

            // animate it
            WelcomeMessage.Opacity = 1.0;
            WelcomeTransform.Y = 400;
            sb = new Storyboard();
            DoubleAnimation db1 = new DoubleAnimation();

            ExponentialEase ease = new ExponentialEase();
            ease.Exponent = 5;
            ease.EasingMode = EasingMode.EaseIn;

            db1.EasingFunction = ease;
            db1.BeginTime = TimeSpan.FromSeconds(0);
            db1.Duration = TimeSpan.FromSeconds(3);
            db1.From = 400;
            db1.To = 0;
            Storyboard.SetTarget(db1, WelcomeTransform);
            Storyboard.SetTargetProperty(db1, new PropertyPath("Y"));
            sb.Children.Add(db1);

            sb.Completed += sbWrap_Completed;
            
            sb.Begin();

        }

        void sbWrap_Completed(object sender, EventArgs e)
        {
            WelcomeTransform.Y = 0;
            WelcomeTimer.Interval = new TimeSpan(0, 0, 10);
            WelcomeTimer.Start(); 
        }


        void WelcomeTimer_Tick(object sender, EventArgs e)
        {
            WelcomeTimer.Stop();
            HideNewsFloater(true);
        }


        private void HideNewsFloater(bool animate)
        {
            // to do:  animate
            WelcomeTimer.Stop();
            sb.Stop();
            _savedNewInfo = null;

            if (animate)
            {
                Storyboard sb2 = new Storyboard();
                DoubleAnimation db1 = new DoubleAnimation();

                db1.BeginTime = TimeSpan.FromSeconds(0);
                db1.Duration = TimeSpan.FromSeconds(2);
                db1.From = 1.0;
                db1.To = 0.0;
                Storyboard.SetTarget(db1, WelcomeMessage);
                Storyboard.SetTargetProperty(db1, new PropertyPath("Opacity"));
                sb2.Children.Add(db1);

                sb2.Completed += (sender, args) =>
                    {
                        WelcomeMessage.Visibility = Visibility.Collapsed;
                    }
                    ;

                sb2.Begin();
            }
            else
                WelcomeMessage.Visibility = Visibility.Collapsed;

        }

        void OnChannelChanged()
        {
            FlushImpressionList();
            LoadingBox.Visibility = Visibility.Visible;
            StopTimers();
            ClearBlahs();
            FetchInitialBlahList();
            App.analytics.PostPageView("/channel/" + BlahguaAPIObject.Current.CurrentChannel.ChannelName);
            if (!String.IsNullOrEmpty(BlahguaAPIObject.Current.CurrentChannel.HeaderImage))
            {
                BitmapImage imgBkgnd = new BitmapImage(new Uri(BlahguaAPIObject.Current.CurrentChannel.HeaderImage));
                ImageBrush bkgndBrush = new ImageBrush();
                bkgndBrush.ImageSource = imgBkgnd;
                bkgndBrush.Stretch = Stretch.UniformToFill;

                ChannelHeaderGrid.Background = bkgndBrush;
                ChannelHeaderTextArea.Text = "";
            }
            else
            {
                ChannelHeaderGrid.Background = (Brush)App.Current.Resources["BrushHeardBlack"];
                ChannelHeaderTextArea.Text = BlahguaAPIObject.Current.CurrentChannel.ChannelName;
            }

            if (BlahguaAPIObject.Current.CurrentUser != null)
            {
                if (BlahguaAPIObject.Current.CanPost)
                    NewBlahBtn.Visibility = Visibility.Visible;
                else
                    NewBlahBtn.Visibility = Visibility.Collapsed;
            }
        }

        private void ScrollStateHandler(object sender, VisualStateChangedEventArgs args)
        {
            if (args.NewState.Name == "NotScrolling")
                scrollTimer.Start();
            DetectScrollAtEnd();

        }

        VisualStateGroup FindVisualState(FrameworkElement element, string name)
        {
            if (element == null)
                return null;

            IList groups = VisualStateManager.GetVisualStateGroups(element);
            foreach (VisualStateGroup group in groups)
                if (group.Name == name)
                    return group;

            return null;
        }

        T FindSimpleVisualChild<T>(DependencyObject element) where T : class
        {
            while (element != null)
            {

                if (element is T)
                    return element as T;

                element = VisualTreeHelper.GetChild(element, 0);
            }

            return null;
        }

        private double InsertRow(char rowType, double topLoc)
        {
            double newTop = topLoc;
            switch (rowType)
            {//ABEAFADCADEACDAFAEBADADCAFABEAEBAFACDAEA
                case 'A':
                    newTop = InsertRowTypeA(topLoc);
                    break;
                case 'B':
                    newTop = InsertRowTypeB(topLoc);
                    break;
                case 'C':
                    newTop = InsertRowTypeC(topLoc);
                    break;
                case 'D':
                    newTop = InsertRowTypeD(topLoc);
                    break;
                case 'E':
                    newTop = InsertRowTypeE(topLoc);
                    break;
                case 'F':
                    newTop = InsertRowTypeF(topLoc);
                    break;
            }

            return newTop;
        }

        private double InsertRowTypeA(double topLoc)
        {
            double newTop = topLoc;
            double curLeft = screenMargin;
            InboxBlah nextBlah;

            for (int i = 0; i < 3; i++)
            {
                nextBlah = blahList.PopBlah(4);
                InsertElementForBlah(nextBlah, curLeft, topLoc, smallBlahSize, smallBlahSize);
                curLeft += smallBlahSize + blahMargin;
            }
            newTop += smallBlahSize;

            return newTop;
        }

        private double InsertRowTypeB(double topLoc)
        {
            double newTop = topLoc;
            double curLeft = screenMargin;
            InboxBlah nextBlah = blahList.PopBlah(4);
            InsertElementForBlah(nextBlah, curLeft, topLoc, smallBlahSize, smallBlahSize);
            nextBlah = blahList.PopBlah(4);
            InsertElementForBlah(nextBlah, curLeft, topLoc + smallBlahSize + blahMargin, smallBlahSize, smallBlahSize);
            nextBlah = blahList.PopBlah(2);
            curLeft += smallBlahSize + blahMargin;
            InsertElementForBlah(nextBlah, curLeft, topLoc, mediumBlahSize, mediumBlahSize);

            newTop += mediumBlahSize;

            return newTop;
        }

        private double InsertRowTypeC(double topLoc)
        {
            double newTop = topLoc;
            double curLeft = screenMargin;
            InboxBlah nextBlah = blahList.PopBlah(2);
            InsertElementForBlah(nextBlah, curLeft, topLoc, mediumBlahSize, mediumBlahSize);
            curLeft += mediumBlahSize + blahMargin;
            nextBlah = blahList.PopBlah(4);
            InsertElementForBlah(nextBlah, curLeft, topLoc, smallBlahSize, smallBlahSize);
            nextBlah = blahList.PopBlah(4);
            InsertElementForBlah(nextBlah, curLeft, topLoc + smallBlahSize + blahMargin, smallBlahSize, smallBlahSize);

            newTop += mediumBlahSize;

            return newTop;
        }

        private double InsertRowTypeD(double topLoc)
        {
            double newTop = topLoc;
            double curLeft = screenMargin;
            InboxBlah nextBlah = blahList.PopBlah(4);
            InsertElementForBlah(nextBlah, curLeft, topLoc, smallBlahSize, smallBlahSize);
            curLeft += smallBlahSize + blahMargin;
            nextBlah = blahList.PopBlah(3);
            InsertElementForBlah(nextBlah, curLeft, topLoc, mediumBlahSize, smallBlahSize);

            newTop += smallBlahSize;

            return newTop;
        }

        private double InsertRowTypeE(double topLoc)
        {
            double newTop = topLoc;
            double curLeft = screenMargin;
            InboxBlah nextBlah = blahList.PopBlah(3);
            InsertElementForBlah(nextBlah, curLeft, topLoc, mediumBlahSize, smallBlahSize);
            curLeft += mediumBlahSize + blahMargin;
            nextBlah = blahList.PopBlah(4);
            InsertElementForBlah(nextBlah, curLeft, topLoc, smallBlahSize, smallBlahSize);

            newTop += smallBlahSize;

            return newTop;
        }

        private double InsertRowTypeF(double topLoc)
        {
            double newTop = topLoc;
            double curLeft = screenMargin;
            InboxBlah nextBlah = blahList.PopBlah(1);
            InsertElementForBlah(nextBlah, curLeft, topLoc, largeBlahSize, mediumBlahSize);

            newTop += mediumBlahSize;

            return newTop;
        }

        /*
        private double InsertRow(char rowType, double topLoc)
        {
            double newTop = topLoc;
            switch (rowType)
            {
                case 1:
                    newTop = InsertRowType1(topLoc);
                    break;
                case 2:
                    newTop = InsertRowType2(topLoc);
                    break;
                case 31:
                    newTop = InsertRowType31(topLoc);
                    break;
                case 32:
                    newTop = InsertRowType32(topLoc);
                    break;
                case 33:
                    newTop = InsertRowType33(topLoc);
                    break;
                case 4:
                    newTop = InsertRowType4(topLoc);
                    break;
            }

            return newTop;
        }

        private double InsertRowType1(double topLoc)
        {
            double newTop = topLoc;
            InboxBlah nextBlah = blahList.PopBlah(1);
            InsertElementForBlah(nextBlah, screenMargin, topLoc, largeBlahSize, mediumBlahSize);
            newTop += mediumBlahSize;


            return newTop;
        }

        private double InsertRowType2(double topLoc)
        {
            double newTop = topLoc;
            double curLeft = screenMargin;
            InboxBlah nextBlah = blahList.PopBlah(2);
            InsertElementForBlah(nextBlah, curLeft, topLoc, mediumBlahSize, mediumBlahSize);
            curLeft += mediumBlahSize + blahMargin;
            nextBlah = blahList.PopBlah(2);
            InsertElementForBlah(nextBlah, curLeft, topLoc, mediumBlahSize, mediumBlahSize);
            newTop += mediumBlahSize;

            return newTop;
        }


        private double InsertRowType31(double topLoc)
        {
            double newTop = topLoc;
            double curLeft = screenMargin;
            InboxBlah nextBlah = blahList.PopBlah(2);
            InsertElementForBlah(nextBlah, curLeft, topLoc, mediumBlahSize, mediumBlahSize);
            curLeft += mediumBlahSize + blahMargin;
            nextBlah = blahList.PopBlah(3);
            InsertElementForBlah(nextBlah, curLeft, topLoc, smallBlahSize, smallBlahSize);
            nextBlah = blahList.PopBlah(3);
            InsertElementForBlah(nextBlah, curLeft, topLoc + smallBlahSize + blahMargin, smallBlahSize, smallBlahSize);
            curLeft += smallBlahSize + blahMargin;
            nextBlah = blahList.PopBlah(3);
            InsertElementForBlah(nextBlah, curLeft, topLoc, smallBlahSize, smallBlahSize);
            nextBlah = blahList.PopBlah(3);
            InsertElementForBlah(nextBlah, curLeft, topLoc + smallBlahSize + blahMargin, smallBlahSize, smallBlahSize);

            newTop += mediumBlahSize;

            return newTop;
        }


        private double InsertRowType32(double topLoc)
        {
            double newTop = topLoc;
            double curLeft = screenMargin;
            InboxBlah nextBlah;
            nextBlah = blahList.PopBlah(3);
            InsertElementForBlah(nextBlah, curLeft, topLoc, smallBlahSize, smallBlahSize);
            nextBlah = blahList.PopBlah(3);
            InsertElementForBlah(nextBlah, curLeft, topLoc + smallBlahSize + blahMargin, smallBlahSize, smallBlahSize);
            curLeft += smallBlahSize + blahMargin;

            nextBlah = blahList.PopBlah(2);
            InsertElementForBlah(nextBlah, curLeft, topLoc, mediumBlahSize, mediumBlahSize);
            curLeft += mediumBlahSize + blahMargin;

            nextBlah = blahList.PopBlah(3);
            InsertElementForBlah(nextBlah, curLeft, topLoc, smallBlahSize, smallBlahSize);
            nextBlah = blahList.PopBlah(3);
            InsertElementForBlah(nextBlah, curLeft, topLoc + smallBlahSize + blahMargin, smallBlahSize, smallBlahSize);
            
           
            newTop += mediumBlahSize;

            return newTop;
        }


        private double InsertRowType33(double topLoc)
        {
            double newTop = topLoc;
            double curLeft = screenMargin;
            InboxBlah nextBlah;
            nextBlah = blahList.PopBlah(3);
            InsertElementForBlah(nextBlah, curLeft, topLoc, smallBlahSize, smallBlahSize);
            nextBlah = blahList.PopBlah(3);
            InsertElementForBlah(nextBlah, curLeft, topLoc + smallBlahSize + blahMargin, smallBlahSize, smallBlahSize);
            curLeft += smallBlahSize + blahMargin;

            nextBlah = blahList.PopBlah(3);
            InsertElementForBlah(nextBlah, curLeft, topLoc, smallBlahSize, smallBlahSize);
            nextBlah = blahList.PopBlah(3);
            InsertElementForBlah(nextBlah, curLeft, topLoc + smallBlahSize + blahMargin, smallBlahSize, smallBlahSize);
            curLeft += smallBlahSize + blahMargin;

            nextBlah = blahList.PopBlah(2);
            InsertElementForBlah(nextBlah, curLeft, topLoc, mediumBlahSize, mediumBlahSize);
            curLeft += mediumBlahSize + blahMargin;

            newTop += mediumBlahSize;
            return newTop;
        }


        private double InsertRowType4(double topLoc)
        {
            double newTop = topLoc;
            double curLeft = screenMargin;
            InboxBlah nextBlah;

            for (int i = 0; i < 4; i++)
            {

                nextBlah = blahList.PopBlah(3);
                InsertElementForBlah(nextBlah, curLeft, topLoc, smallBlahSize, smallBlahSize);
                curLeft += smallBlahSize + blahMargin;
            }
            newTop += smallBlahSize;

            return newTop;
        }
        */

        private void DoSignIn(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Screens/Signin.xaml", UriKind.Relative));    
        }


        private void UserInfoBtn_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            BlahguaAPIObject.Current.GetUserProfile((theProfile) =>
                {
                    NavigationService.Navigate(new Uri("/Screens/ProfileViewer.xaml", UriKind.Relative));
                }
            );
        }
        
        private void NewBlahBtn_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Screens/CreateBlah.xaml", UriKind.Relative));    
        }

        private void FadeIn_Completed(object sender, EventArgs e)
        {
            ((Storyboard)sender).Stop();
            if (targetBlah != null)
            {
                targetBlah.TextArea.Visibility = Visibility.Visible;
                targetBlah.BlahBackground.Visibility = Visibility.Visible;
            }
            MaybeAnimateElement();   
            
        }

        private void FadeOut_Completed(object sender, EventArgs e)
        {
            ((Storyboard)sender).Stop();
            if (targetBlah != null)
            {
                targetBlah.TextArea.Visibility = Visibility.Collapsed;
                targetBlah.BlahBackground.Visibility = Visibility.Collapsed;
            }
            MaybeAnimateElement();

        }


        void MaybeAnimateElement()
        {
            try
            {
                int blahCount = BlahContainer.Children.Count;

                if (blahCount > 0)
                {
                    Random rnd = new Random();
                    BlahRollItem newItem;
                    double top = BlahScroller.VerticalOffset;
                    double bottom = top + 800;
                    double curTop;
                    int maxTry = 100;
                    int curTry = 0;

                    do
                    {
                        int newindex = rnd.Next(blahCount);
                        newItem = (BlahRollItem)BlahContainer.Children[newindex];
                        curTop = Canvas.GetTop(newItem);
                        curTry++;
                        if (curTry > maxTry)
                        {
                            newItem = null;
                            break;
                        }

                    }
                    while ((newItem.BlahData.M == null) || (String.IsNullOrEmpty(newItem.BlahData.T)) || (curTop < top) || (curTop > bottom) || (targetBlah == newItem));

                    if (newItem != null)
                    {
                        targetBlah = newItem;

                        if (newItem.TextArea.Visibility == Visibility.Collapsed)
                        {
                            Storyboard.SetTarget(AnimateTextFadeIn.Children[0], newItem.TextArea);
                            Storyboard.SetTarget(AnimateTextFadeIn.Children[1], newItem.TextArea);
                            Storyboard.SetTarget(AnimateTextFadeIn.Children[2], newItem.BlahBackground);
                            Storyboard.SetTarget(AnimateTextFadeIn.Children[3], newItem.BlahBackground);
                            AnimateTextFadeIn.Begin();
                        }
                        else
                        {
                            Storyboard.SetTarget(AnimateTextFadeOut.Children[0], newItem.TextArea);
                            Storyboard.SetTarget(AnimateTextFadeOut.Children[1], newItem.TextArea);
                            Storyboard.SetTarget(AnimateTextFadeOut.Children[2], newItem.BlahBackground);
                            Storyboard.SetTarget(AnimateTextFadeOut.Children[3], newItem.BlahBackground);
                            AnimateTextFadeOut.Begin();
                        }
                    }
                    else
                    {
                        BlahAnimateTimer.Start();
                    }
                }
            }
            catch (Exception exp)
            {
                Console.WriteLine("err: " + exp.Message);
            }
        }

        void BlahAnimateTimer_Tick(object sender, EventArgs e) 
        {
            targetBlah = null;
            BlahAnimateTimer.Stop();
            MaybeAnimateElement();
        }

        private void UserImageLoadFailed(object sender, ExceptionRoutedEventArgs e)
        {
            UserInfoBtn.Source = new BitmapImage(new Uri("/Images/unknown-user.png", UriKind.Relative));
        }

        private void WelcomeMessage_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            HideNewsFloater(false);
        }

        private void ShareStats_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ShareLinkTask shareLinkTask = new ShareLinkTask();
            Blah curBlah = BlahguaAPIObject.Current.CurrentBlah;
            string blahURL = BlahguaAPIObject.Current.GetBaseShareURL();
            string msgStr = _savedNewInfo.SummaryString;

            shareLinkTask.Title = "I am being heard on blahgua!";
            shareLinkTask.LinkUri = new Uri(blahURL, UriKind.Absolute);
            shareLinkTask.Message = msgStr; ;

            shareLinkTask.Show();
        }
    }

}

   