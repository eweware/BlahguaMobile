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


namespace BlahguaMobile.Winphone
{
    public partial class BadgingPage : PhoneApplicationPage
    {
        string ticketStr = "";
        public BadgingPage()
        {
            InitializeComponent();

           

        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            App.analytics.PostPageView("/AddBadge");
        }

        private void DoSubmitEmail(object sender, System.Windows.Input.GestureEventArgs e)
        {
            SubmitBtn.IsEnabled = false;
            EmailField.IsEnabled = false;
            ProgressBox.Visibility = Visibility.Visible;
            BlahguaAPIObject.Current.GetBadgeAuthorities((authList) =>
                {
                    string badgeId = authList[0]._id;
                    string emailAddr = EmailField.Text;

                    BlahguaAPIObject.Current.GetEmailBadgeForUser(badgeId, emailAddr, (ticket) =>
                        {
                            ProgressBox.Visibility = Visibility.Collapsed;
                            if (ticket == "")
                            {
                                MessageBox.Show("The authority currently has no badges for that email address.  Please try again in the future.");
                                SubmitBtn.IsEnabled = true;
                                EmailField.IsEnabled = true;
                            }
                            else
                            {
                                ValidationField.Text = "";
                                ticketStr = ticket;
                                EmailArea.Visibility = Visibility.Collapsed;
                                ValidationArea.Visibility = Visibility.Visible;
                                ValidateBtn.IsEnabled = true;
                                ValidationField.IsEnabled = true;
                            }
                        }
                    );

                }
            );
        }

        private void DoValidate(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ValidateBtn.IsEnabled = false;
            ValidationField.IsEnabled = false;
            ProgressBox.Visibility = Visibility.Visible;

            string valString = ValidationField.Text;

            BlahguaAPIObject.Current.VerifyEmailBadge(valString, ticketStr, (resultStr) =>
                {
                    if (resultStr == "")
                    {
                        ProgressBox.Visibility = Visibility.Collapsed;
                        MessageBox.Show("That validation code was not valid.  Please retry your badging attempt.");
                        SubmitBtn.IsEnabled = true;
                        EmailField.IsEnabled = true;
                        EmailArea.Visibility = Visibility.Visible;
                        ValidationArea.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        BlahguaAPIObject.Current.RefreshUserBadges((theStr) =>
                            {
                                ProgressBox.Visibility = Visibility.Collapsed;
                                MessageBox.Show("Badging successful!");
                                NavigationService.GoBack();
                            }
                        );
                       
                    }
                }
            );

        }
    }
}