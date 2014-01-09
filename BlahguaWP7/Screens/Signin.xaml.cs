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

namespace BlahguaMobile.Winphone
{
    public partial class Signin : PhoneApplicationPage
    {
        bool createNewAccount = false;

        public Signin()
        {
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



        private void HandleUserSignIn()
        {
            NavigationService.GoBack();
        }

        private void HyperlinkButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            EmailComposeTask emailComposeTask = new EmailComposeTask();

            emailComposeTask.Subject = "feedback on blahgua";
            emailComposeTask.Body = "";
            emailComposeTask.To = "admin@blahgua.com";

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
    }
}