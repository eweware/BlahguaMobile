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
using Android.Graphics;
using BlahguaMobile.AndroidClient.HelpingClasses;


namespace BlahguaMobile.AndroidClient.Screens
{
    public class TutorialDialog : DialogFragment
    {
        private int curScreen = 1;
        private ImageView tutorialImageView = null;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View fragment = inflater.Inflate(Resource.Layout.dialog_tutorial, null);

            tutorialImageView = fragment.FindViewById<ImageView>(Resource.Id.tutorialImageView);
            return fragment;
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            view.Click += (sender, args) =>
            {
                curScreen++;
                if (curScreen > 5)
                {

                    this.DismissAllowingStateLoss();
                }
                else
                    UpdateImage();
            };

            
        }

        private void UpdateImage()
        {
            int imageRes = Resource.Drawable.tutorial_android_screen_1;

            switch (curScreen)
            {
                case 1:
                    imageRes = Resource.Drawable.tutorial_android_screen_1;
                    break;
                case 2:
                    imageRes = Resource.Drawable.tutorial_android_screen_2;
                    break;
                case 3:
                    imageRes = Resource.Drawable.tutorial_android_screen_3;
                    break;
                case 4:
                    imageRes = Resource.Drawable.tutorial_android_screen_4;
                    break;
                case 5:
                    imageRes = Resource.Drawable.tutorial_android_screen_5;
                    break;

            }
            tutorialImageView.SetImageResource(imageRes);
        }

        public static TutorialDialog ShowDialog(FragmentManager manager)
        {
            TutorialDialog f = new TutorialDialog();
            f.SetStyle(DialogFragmentStyle.NoTitle, 0);
            f.curScreen = 1;
            try
            {
                f.Show(manager, "TutorialDialog");
            }
            catch (Exception)
            {
            }

            return f;
        }
    }
}