using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using BlahguaMobile.BlahguaCore;

using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Threading.Tasks;

namespace BlahguaMobile.Winphone.Screens
{
    public partial class OnboardingScreen : PhoneApplicationPage
    {
        private string currentPage;
        private string currentTicket;
        private BadgeAuthority emailAuthority = null;

        public OnboardingScreen()
        {
            currentPage = "sign up";
            InitializeComponent();
            DataContext = BlahguaAPIObject.Current;
        }

        private void HaveAccountBtn_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            AdditionalInfoPanel.Visibility = Visibility.Collapsed;
            CreateAccountArea.Visibility = Visibility.Collapsed;
            SignInPanel.Visibility = Visibility.Visible;
        }

        private void NextPage()
        {
            OnboardPivot.IsLocked = false;
            OnboardPivot.SelectedIndex++;
        }

        private void PrevPage()
        {
            OnboardPivot.SelectedIndex++;
        }

        private void Finish()
        {
            BlahguaAPIObject.Current.SafeSaveSetting("seentutorial", true);
            MainPage.ReturningFromTutorial = true;
            NavigationService.GoBack();
        }

        private void DoSkipAccount(object sender, EventArgs e)
        {
            Finish();
        }

        private void DoSignIn(object sender, EventArgs e)
        {
            SignInBtn.IsEnabled = false;
            if (BlahguaAPIObject.Current.UserName != UserNameField.Text)
                BlahguaAPIObject.Current.UserName = UserNameField.Text;

            if (BlahguaAPIObject.Current.UserPassword != PasswordField.Password)
                BlahguaAPIObject.Current.UserPassword = PasswordField.Password;

            SignInProgress.Visibility = Visibility.Visible;
            BlahguaAPIObject.Current.SignIn(BlahguaAPIObject.Current.UserName, BlahguaAPIObject.Current.UserPassword, true, (errMsg) =>
                {
                    SignInProgress.Visibility = Visibility.Collapsed;
                    if (errMsg == null)
                        Finish();
                    else
                    {
                        App.analytics.PostSessionError("signinfailed-" + errMsg);
                        MessageBox.Show("could not register: " + errMsg);
                    }
                }
            );
        }

        private void StartBadgeBtn_Tap(object sender, EventArgs e)
        {
            StartBadgeBtn.IsEnabled = false;
            SkipBadgeBtn.IsEnabled = false;
            string authId = emailAuthority._id;
            BlahguaAPIObject.Current.GetEmailBadgeForUser(authId, BlahguaAPIObject.Current.UserEmailAddress, (ticket) =>
            {
                if (ticket == String.Empty)
                {
                    MessageBox.Show("No badges are available for your domain.  You can try again later from your profile page.");
                    Finish();

                }
                else if (ticket == "existing")
                {
                    App.analytics.PostBadgeNoEmail(BlahguaAPIObject.Current.UserEmailAddress);
                    MessageBox.Show("A badge has previously been issued to this email address.");
                    Finish();
                }
                else if (ticket == "invalid")
                {
                    App.analytics.PostBadgeNoEmail(BlahguaAPIObject.Current.UserEmailAddress);
                    MessageBox.Show("We were not able to send mail to that email address.");
                    DoReturnToBadge(null,null);
                }
                else
                {
                    App.analytics.PostRequestBadge(authId);
                    currentTicket = ticket;
                    MessageBox.Show("Badges are available for your email address.");
                    EnableCodeSubmit();
                }
            });
        }

        private void EnableCodeSubmit()
        {
            CodeBox.Text = "";
            VerifyBadgeArea.Visibility = Visibility.Collapsed;
            StartBadgingArea.Visibility = Visibility.Visible;
            SubmitCodeBtn.IsEnabled = true;
            ReturnToBadgeScreen.IsEnabled = true;
        }

        private void DoSubmitCode(object sender, EventArgs e)
        {
            SubmitCodeBtn.IsEnabled = false;
            ReturnToBadgeScreen.IsEnabled = false;
            BlahguaAPIObject.Current.VerifyEmailBadge(CodeBox.Text, currentTicket, (result) =>
            {
                if (result == "fail")
                {
                    App.analytics.PostBadgeValidateFailed();
                    MessageBox.Show("That validation code was not valid.  Please retry your badging attempt.");
                    CodeBox.SelectAll();
                    EnableCodeSubmit();
                }
                else
                {
                    App.analytics.PostGotBadge();
                    BlahguaAPIObject.Current.RefreshUserBadges(null);
                    MessageBox.Show("You are ready to use badges in Heard.");
                    Finish();
                }
            });
        }

        private void DoReturnToBadge(object sender, EventArgs e)
        {
            CodeBox.Text = "";
            VerifyBadgeArea.Visibility = Visibility.Collapsed;
            StartBadgingArea.Visibility = Visibility.Visible;
            StartBadgeBtn.IsEnabled = true;
            SkipBadgeBtn.IsEnabled = true;
        }



        private void DoCreateAccount(object sender, EventArgs e)
        {
            if (BlahguaAPIObject.Current.UserPassword != BlahguaAPIObject.Current.UserPassword2)
                MessageBox.Show("Passwords must match");
            else
            {
                SignInProgress.Visibility = Visibility.Visible;
                CreateNewAccountBtn.IsEnabled = false;
                BlahguaAPIObject.Current.Register(BlahguaAPIObject.Current.UserName, BlahguaAPIObject.Current.UserPassword, true, (errMsg) =>
                {
                    if (errMsg == null)
                    {
                        App.analytics.PostRegisterUser();
                        if (!String.IsNullOrEmpty(BlahguaAPIObject.Current.UserEmailAddress))
                        {
                            BlahguaAPIObject.Current.SetRecoveryEmail(BlahguaAPIObject.Current.UserEmailAddress, (resultStr) =>
                            {
                                SignInProgress.Visibility = Visibility.Collapsed;
                                NextPage();
                            });
                        }
                        else
                        {
                            SignInProgress.Visibility = Visibility.Collapsed;
                            NextPage();
                        }
                    }
                    else
                    {
                        SignInProgress.Visibility = Visibility.Collapsed;
                        App.analytics.PostSessionError("registerfailed-" + errMsg);
                        MessageBox.Show("could not register: " + errMsg);
                    }
                }
                );
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

        private void HandlePivotLoaded(object sender, PivotItemEventArgs e)
        {
            currentPage = e.Item.Header.ToString();
            
            switch(currentPage)
            {
                case "sign up":
                    PrepSignUp();
                    break;

                case "channel":
                    PrepChannelList();
                    break;

                case  "badges":
                    PrepBadgePage();
                    break;
            }
        }

        private void ResetHeader()
        {
            Task.Delay(TimeSpan.FromMilliseconds(2000)).ContinueWith((theTask) =>
            {
                OnboardPivot.Dispatcher.BeginInvoke(() =>
                {
                    OnboardPivot.IsLocked = true;
                });
            });
        }

        private void PrepSignUp()
        {
            ResetHeader();
        }

        private void PrepBadgePage()
        {
            BlahguaAPIObject.Current.GetBadgeAuthorities((authorities) =>
            {
                if ((authorities == null) || (authorities.Count == 0))
                {
                    Console.WriteLine("Error:  no badge authories available");

                    Finish();
                }
                else
                {
                    emailAuthority = authorities[0];
                    SkipBadgeBtn.IsEnabled = true;
                    StartBadgeBtn.IsEnabled = true;
                    ResetHeader();
                }
            });
        }


        private void PrepChannelList()
        {
            Channel defChannel = BlahguaAPIObject.Current.GetDefaultChannel();
            
            List<Channel> commList = new List<Channel>();
            List<Channel> pubList = new List<Channel>();
            Channel commHeader = new Channel();
            Channel pubHeader = new Channel();

            commHeader.N = "Select a Community";
            commHeader.D = "";
            pubHeader.N = "Select a Publisher";
            pubHeader.D = "";

            try
            {
                string typeId = BlahguaAPIObject.Current.CurrentChannelTypeList.Find(i => i.N == "Industry")._id;
                List<Channel> newList = BlahguaAPIObject.Current.CurrentChannelList.FindAll(i => i.Y == typeId);
                commList.Add(commHeader);
                commList.Add(defChannel);
                commList.AddRange(newList);
                CommunityList.ItemsSource = commList;

                typeId = BlahguaAPIObject.Current.CurrentChannelTypeList.Find(i => i.N == "Publishers")._id;
                newList = BlahguaAPIObject.Current.CurrentChannelList.FindAll(i => i.Y == typeId);
                pubList.Add(pubHeader);
                pubList.AddRange(newList);
                PublisherList.ItemsSource = pubList;

                CommunityList.SelectionChanged += CommunityList_SelectionChanged;
                PublisherList.SelectionChanged += PublisherList_SelectionChanged;

            }
            catch (Exception e)
            {

            }

            ResetHeader();

        }

        void CommunityList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CommunityList.SelectedIndex != 0)
            {
                SetDefaultChannel((Channel)CommunityList.SelectedItem);
                if (PublisherList.SelectedIndex == 1)
                    Finish();
                else
                    NextPage();

            }
        }

        void PublisherList_SelectionChanged (object sender, SelectionChangedEventArgs e)
        {
            if (PublisherList.SelectedIndex != 0)
            {
                SetDefaultChannel((Channel)PublisherList.SelectedItem);
                Finish();
            }
        }


        private void SetDefaultChannel(Channel theChannel)
        {
            BlahguaAPIObject.Current.SavedChannel = theChannel.ChannelName;
            BlahguaAPIObject.Current.SafeSaveSetting("SavedChannel", theChannel.ChannelName);
            BlahguaAPIObject.Current.CurrentChannel = theChannel;
        }


        private void ResetHeader(object theItem)
        {
            Console.WriteLine("Pivot Page: " + theItem.ToString());
        }

        private void OnPivotLoading(object sender, PivotItemEventArgs e)
        {
            string newItem = e.Item.Header.ToString();
            App.analytics.PostPageView("/tutorial/" + newItem);
         

            // backgrounds
            if ((newItem == "sign up") && (currentPage == "badges"))
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
            else if ((newItem == "badges") && (currentPage == "sign up"))
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

                if (OnboardPivot.Items.Count() > 1)
                    offset = maxScroll / (OnboardPivot.Items.Count() - 1);
                else
                    offset = 0;
                ExponentialEase ease = new ExponentialEase();
                ease.Exponent = 5;
                ease.EasingMode = EasingMode.EaseIn;

                targetVal = offset * OnboardPivot.Items.IndexOf(e.Item);
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


    }
}