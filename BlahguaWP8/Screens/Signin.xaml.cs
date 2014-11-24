using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.ComponentModel;
using Microsoft.Phone.Tasks;
using Telerik.Windows.Controls;
using BlahguaMobile.BlahguaCore;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace BlahguaMobile.Winphone
{
    public partial class Signin : PhoneApplicationPage
    {
        bool createNewAccount = false;
        private string currentPage;

        public Signin()
        {
            currentPage = "sign in";
            InitializeComponent();
            DataContext = BlahguaAPIObject.Current;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            createNewAccount = (bool)NewAccountBox.IsChecked;

            if (createNewAccount)
            {
                AdditionalInfoPanel.Visibility = Visibility.Visible;
                CreateNewAccountBtn.Visibility = Visibility.Visible;
                SignInBtn.Visibility = Visibility.Collapsed;
            }
            else
            {
                AdditionalInfoPanel.Visibility = Visibility.Collapsed;
                CreateNewAccountBtn.Visibility = Visibility.Collapsed;
                SignInBtn.Visibility = Visibility.Visible;
            }
        }


        private void DoSignIn(object sender, EventArgs e)
        {
            if (BlahguaAPIObject.Current.UserName != UserNameField.Text)
                BlahguaAPIObject.Current.UserName = UserNameField.Text;

            if (BlahguaAPIObject.Current.UserPassword != PasswordField.Password)
                BlahguaAPIObject.Current.UserPassword = PasswordField.Password;

            SignInProgress.Visibility = Visibility.Visible;
            BlahguaAPIObject.Current.SignIn(BlahguaAPIObject.Current.UserName, BlahguaAPIObject.Current.UserPassword, BlahguaAPIObject.Current.AutoLogin, (errMsg) =>
                {
                    SignInProgress.Visibility = Visibility.Collapsed;
                    if (errMsg == null)
                        HandleUserSignIn();
                    else
                    {
                        App.analytics.PostSessionError("signinfailed-" + errMsg);
                        MessageBox.Show("could not register: " + errMsg);
                    }
                }
            );
        }

        private void DoCreateAccount(object sender, EventArgs e)
        {
            if (BlahguaAPIObject.Current.UserPassword != BlahguaAPIObject.Current.UserPassword2)
                MessageBox.Show("Passwords must match");
            else
            {
                SignInProgress.Visibility = Visibility.Visible;
                BlahguaAPIObject.Current.Register(BlahguaAPIObject.Current.UserName, BlahguaAPIObject.Current.UserPassword, BlahguaAPIObject.Current.AutoLogin, (errMsg) =>
                    {
                        SignInProgress.Visibility = Visibility.Collapsed;
                        if (errMsg == null)
                        {
                            App.analytics.PostRegisterUser();
                            HandleUserSignIn();
                        }
                        else
                        {
                            App.analytics.PostSessionError("registerfailed-" + errMsg);
                            MessageBox.Show("could not register: " + errMsg);
                        }
                    }
                );
            }
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            App.analytics.PostPageView("/signup");
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



        private void HandleUserSignIn()
        {
            NavigationService.GoBack();
        }

        private void HyperlinkButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            EmailComposeTask emailComposeTask = new EmailComposeTask();

            emailComposeTask.Subject = "feedback on Heard";
            emailComposeTask.Body = "";
            emailComposeTask.To = "admin@goheard.com";

            emailComposeTask.Show();    
        }

#if WP8
        private async void Recover_Tap(object sender, System.Windows.Input.GestureEventArgs e)
#else
        private void Recover_Tap(object sender, System.Windows.Input.GestureEventArgs e)
#endif
        {
            InputPromptSettings settings = new InputPromptSettings();
            settings.Field1Mode = InputMode.Text;
            settings.Field2Mode = InputMode.Text;
#if WP8
            InputPromptClosedEventArgs args = await RadInputPrompt.ShowAsync(settings, "Please enter username and email address");

            BlahguaAPIObject.Current.UpdatePassword(args.Text, (theResult) =>
                {
                    MessageBox.Show("Check the email for a recovery link.");
                }
            );

#else
            RadInputPrompt.Show(settings, "Please enter username and email address", closedHandler: (args) =>
                {
                    BlahguaAPIObject.Current.UpdatePassword(args.Text, (theResult) =>
                        {
                            MessageBox.Show("Check the email for a recovery link.");
                        }
                    );
                }
            );
#endif
            
        }

        private void RateReview_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            MarketplaceReviewTask rev = new MarketplaceReviewTask();
            rev.Show();
            
        }

        private void HandlePivotLoaded(object sender, PivotItemEventArgs e)
        {
            currentPage = e.Item.Header.ToString();
        }

        private void OnPivotLoading(object sender, PivotItemEventArgs e)
        {
            string newItem = e.Item.Header.ToString();
            App.analytics.PostPageView("/signup/" + newItem);


            // backgrounds
            if ((newItem == "sign in") && (currentPage == "help"))
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
            else if ((newItem == "help") && (currentPage == "sign in"))
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

                if (SignInPivot.Items.Count() > 1)
                    offset = maxScroll / (SignInPivot.Items.Count() - 1);
                else
                    offset = 0;
                ExponentialEase ease = new ExponentialEase();
                ease.Exponent = 5;
                ease.EasingMode = EasingMode.EaseIn;

                targetVal = offset * SignInPivot.Items.IndexOf(e.Item);
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