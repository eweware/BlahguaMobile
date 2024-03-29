﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Data;
using System.IO.IsolatedStorage;
using BlahguaMobile.BlahguaCore;
using HockeyApp;


namespace BlahguaMobile.Winphone
{
    class CustomUriMapper : UriMapperBase
    {
       
        public override Uri MapUri(Uri uri)
        {
            string tempUri = uri.ToString();
            string mappedUri;

            // Launch from the photo share picker.
            // Incoming URI example: /MainPage.xaml?Action=ShareContent&FileId=%7BA3D54E2D-7977-4E2B-B92D-3EB126E5D168%7D
            if ((tempUri.Contains("ShareContent")) && (tempUri.Contains("FileId")))
            {
                // Redirect to PhotoShare.xaml.
                mappedUri = tempUri.Replace("MainPage", "Screens/PhotoShare");
                return new Uri(mappedUri, UriKind.Relative);
            }

            // Otherwise perform normal launch.
            return uri;
        }
    }


    public sealed class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo language)
        {
            return (value is bool && (bool)value) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo language)
        {
            return value is Visibility && (Visibility)value == Visibility.Visible;
        }
    }


    public partial class App : Application
    {
        /// <summary>
        /// Provides easy access to the root frame of the Phone Application.
        /// </summary>
        /// <returns>The root frame of the Phone Application.</returns>
        public TransitionFrame RootFrame { get; private set; }
        
        private BlahguaAPIObject BlahguaAPI = null;
        public static GoogleAnalytics analytics = null;
        public static string BlahIdToOpen { get; set; }



        /// <summary>
        /// Constructor for the Application object.
        /// </summary>
        public App()
        {
            // Global handler for uncaught exceptions. 
            UnhandledException += Application_UnhandledException;

            // Standard Silverlight initialization
            InitializeComponent();

            // Phone-specific initialization
            InitializePhoneApplication();

            // Show graphics profiling information while debugging.
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // Display the current frame rate counters.
                Application.Current.Host.Settings.EnableFrameRateCounter = true;
                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            }


            BlahguaAPI = BlahguaAPIObject.Current;
            HockeyClient.Current.Configure("310cb7910c3711f97632b3df79fe41f8");
            

        }


        // Code to execute when the application is launching (eg, from Start)
        // This code will not execute when the application is reactivated
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
            string uniqueId;

            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            if (settings.Contains("uniqueId"))
                uniqueId = settings["uniqueId"].ToString();
            else
            {
                uniqueId = Guid.NewGuid().ToString();
                settings.Add("uniqueId", uniqueId);
                settings.Save();
                
            }

            ManifestAppInfo am = new ManifestAppInfo(); // gets appmanifest as per link above
            string maker = Microsoft.Phone.Info.DeviceStatus.DeviceManufacturer;
            string model = Microsoft.Phone.Info.DeviceStatus.DeviceName;
            string version = am.Version;
            string platform = "WP8";
            string userAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows Phone 8.0; Trident/6.0; IEMobile/10.0; ARM; Touch; ";

            analytics = new GoogleAnalytics(userAgent, maker, model, version, platform, uniqueId);
            analytics.StartSession();

            HockeyClient.Current.SendCrashesAsync();
            HockeyClient.Current.CheckForUpdates();

        }



        

        // Code to execute when the application is activated (brought to foreground)
        // This code will not execute when the application is first launched
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
            BlahguaAPI.EnsureSignin();
            BlahguaAPI.StartSigninTimer();
        }

        // Code to execute when the application is deactivated (sent to background)
        // This code will not execute when the application is closing
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
            BlahguaAPI.StopSigninTimer();
        }

        // Code to execute when the application is closing (eg, user hit Back)
        // This code will not execute when the application is deactivated
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
            BlahguaAPI.SignOut(null, (theObj) =>
                {
                }
            );
            analytics.EndSession();
        }

        // Code to execute if a navigation fails
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // A navigation has failed; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        // Code to execute on Unhandled Exceptions
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            try
            {
                analytics.PostCrash(e.ExceptionObject.Message);
            }
            catch (Exception exp)
            {
            }

            if (System.Diagnostics.Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        #region Phone application initialization

        // Avoid double-initialization
        private bool phoneApplicationInitialized = false;

        // Do not add any additional code to this method
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // Create the frame but don't set it as RootVisual yet; this allows the splash
            // screen to remain active until the application is ready to render.
            RootFrame = new TransitionFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            RootFrame.UriMapper = new CustomUriMapper();


            // Handle navigation failures
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Ensure we don't initialize again
            phoneApplicationInitialized = true;
        }

        // Do not add any additional code to this method
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Set the root visual to allow the application to render
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Remove this handler since it is no longer needed
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        #endregion
    }
}