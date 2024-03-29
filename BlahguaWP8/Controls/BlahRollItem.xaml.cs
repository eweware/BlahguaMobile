﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BlahguaMobile.BlahguaCore;
using System.Windows.Media.Animation;

namespace BlahguaMobile.Winphone
{
    public partial class BlahRollItem : UserControl
    {
        private static float smallText = 16;
        private static float medText = 36;
        private static float largeText = 52;
        private static BitmapImage saysImage = null, asksImage, pollsImage, predictsImage, leaksImage;

        public InboxBlah BlahData {get; set;}

        public bool IsAnimating { get; set; }

        public BlahRollItem()
        {
            InitializeComponent();
            IsAnimating = false;
            BlahData = null;
            BlahImage.Loaded += BlahImage_Loaded;
            if (saysImage == null)
            {
                saysImage = new BitmapImage(new Uri("/Images/Icons/say_icon.png", UriKind.Relative));
                asksImage = new BitmapImage(new Uri("/Images/Icons/ask_icon.png", UriKind.Relative));
                pollsImage = new BitmapImage(new Uri("/Images/Icons/poll_icon.png", UriKind.Relative));
                leaksImage = new BitmapImage(new Uri("/Images/Icons/leak_icon.png", UriKind.Relative));
                predictsImage = new BitmapImage(new Uri("/Images/Icons/predict_icon.png", UriKind.Relative));

            }
            
        }

        public void Initialize(InboxBlah theBlah)
        {
            BlahData = theBlah;
            BitmapImage postTypeImage = null;
            
            switch (theBlah.TypeName)
            {
                case "says":
                    postTypeImage =saysImage;
                    break;
                case "leaks":
                     postTypeImage =leaksImage;
                    break;
                case "polls":
                     postTypeImage = pollsImage;
                    break;
                case "predicts":
                     postTypeImage = predictsImage;
                    break;
                case "asks":
                     postTypeImage = asksImage;
                    break;

            }

            if (BlahguaAPIObject.Current.CurrentChannel.SSA == false)
                BlahTypeIcon.Visibility = System.Windows.Visibility.Collapsed;
            else
            {
                BlahTypeIcon.Visibility = System.Windows.Visibility.Visible;
                BlahTypeIcon.Source = postTypeImage;
            }
                
            

            User curUser = BlahguaAPIObject.Current.CurrentUser;

            if ((curUser != null) && (curUser._id == theBlah.A))
                OwnBlahIcon.Visibility = Visibility.Visible;
            else
                OwnBlahIcon.Visibility = Visibility.Collapsed;

            //BlahBackground.Opacity = .4;
            if (theBlah.B == null)
                BadgeIcon.Visibility = Visibility.Collapsed;
            
            TimeSpan diff = DateTime.Now - theBlah.Created;

            if (diff.TotalHours > 30)
                NewBlahIcon.Visibility = Visibility.Collapsed;
            else
                NewBlahIcon.Visibility = Visibility.Visible;

            if (theBlah.RR)
                ActiveIcon.Opacity = 1;
            else
                ActiveIcon.Opacity = 0;



            TextArea.Text = BlahData.T;
        }

        public void AnimateActivity()
        {
            ActivityAnimate.Begin();
        }


        void BlahImage_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((BlahData.M != null) && (BlahData.M.Count > 0))
                {
                    BlahImage.ImageOpened += Image_loaded;
                    string imageBase = BlahData.M[0];
                    string imageSize = BlahData.ImageSize;
                    string imageURL = BlahguaAPIObject.Current.GetImageURL(imageBase, imageSize);
                    BlahImage.Opacity = 0;
                    BlahImage.Source = new BitmapImage(new Uri(imageURL, UriKind.Absolute));
                } 
                
            }
            catch (Exception exp)
            {
                System.Console.WriteLine(exp.Message);
            }
            
        }

        void Image_loaded(object sender, RoutedEventArgs e)
        {
            BlahImage.Opacity = 1;
            if (!String.IsNullOrEmpty(BlahData.T) )
            {
                BlahBackground.Opacity = .8;
                TextArea.Foreground = (Brush)App.Current.Resources["BrushBlahguaBlack"];
                BlahBackground.Visibility = Visibility.Collapsed;
                TextArea.Visibility = Visibility.Collapsed;
            }
            else
            {
                BlahBackground.Visibility = Visibility.Collapsed;
                TextArea.Visibility = Visibility.Collapsed;
            }
        }

        


        public void ScaleTextToFit()
        {
            switch (BlahData.displaySize)
            {
                case 3:
                    TextArea.FontSize = smallText;
                    break;
                case 2:
                    TextArea.FontSize = medText;
                    break;
                case 1:
                    TextArea.FontSize = largeText;
                    TextArea.FontFamily = new FontFamily("/Resources/GothamRounded-Bold.otf#Gotham Rounded Bold");
                    break;

            }
            TextArea.TextTrimming = TextTrimming.WordEllipsis;
          

        }
    }
}
