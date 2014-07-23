using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using BlahguaMobile.BlahguaCore;
using System.Collections.Generic;

namespace BlahguaMobile.IOS.View_Controllers
{
    public partial class ProfileToStatsViewController : UIViewController
    {
        private UIViewController parentViewController;

        private Blah CurrentBlah
        {
            get
            {
                return ((AppDelegate)UIApplication.SharedApplication.Delegate).CurrentBlah;
            }
        }

        public ProfileToStatsViewController(IntPtr handle)
            : base(handle)
        {

        }

        public override void ViewDidLoad()
        {
            try
            {
                base.ViewDidLoad();

                //Synsoft on 17 July 2014 added title
                this.Title = "Stats";

                this.NavigationController.SetNavigationBarHidden(false, true);
              
                NavigationItem.LeftBarButtonItem = new UIBarButtonItem("Back", UIBarButtonItemStyle.Plain, BlahHandler);
                //Synsoft on 17 July 2014 for active color  #1FBBD1
                NavigationItem.LeftBarButtonItem.TintColor = UIColor.FromRGB(31, 187, 209);

                //Synsoft on 17 July 2014 
                // scrollView.ContentSize = new SizeF(scrollView.Frame.Width, scrollView.Frame.Height);

                scrollView.ContentSize = new SizeF(scrollView.Frame.Width, 600);
                scrollView.ScrollEnabled = true;

                Dictionary<string, string> source = new Dictionary<string, string>();

                //Synsoft on 17 July 2014 
                if (CurrentBlah != null)
                {
                    //lblReputation.Text = ;   
                    //lblControversy.Text=;
                    lblOpen.Text = CurrentBlah.O.ToString();
                    //lblOpen1.Text = CurrentBlah.O.ToString();
                    lblDemots.Text = CurrentBlah.D.ToString();
                    lblPromots.Text = CurrentBlah.P.ToString();
                    lblComment.Text = CurrentBlah.C.ToString();
                    //lblComments1.Text = CurrentBlah.C.ToString();
                    lblImpression.Text = CurrentBlah.V.ToString();
                    //lblimpression1.Text = CurrentBlah.V.ToString();
                }

                else if (BlahguaAPIObject.Current.CurrentUser != null && BlahguaAPIObject.Current.CurrentUser.UserInfo != null)
                {
                    var userinfo = BlahguaAPIObject.Current.CurrentUser.UserInfo;
                    int userviews, opens, creates, comments, views;
                    userviews = opens = creates = comments = views = 0;

                    for (int i = 0; i < userinfo.DayCount; i++)
                    {
                        userviews += userinfo.UserViews(i);
                        opens += userinfo.Opens(i);
                        creates += userinfo.UserCreates(i);
                        comments += userinfo.UserComments(i);
                        views += userinfo.Views(i);
                    }

                    lblOpen.Text = opens.ToString();
                    lblComment.Text = comments.ToString();
                    lblImpression.Text = views.ToString();
                }
            }
            catch (Exception e)
            {
                e.Message.ToString();

            }

        }

        //Synsoft on 14 July 2014
        private void SwipeToRollController()
        {
            this.NavigationController.PopViewControllerAnimated(true);
        }

        
        private void BlahHandler(object sender, EventArgs args)
        {
            this.NavigationController.PopViewControllerAnimated(true);
        }

        public void SetParentViewController(UIViewController parentViewController)
        {
            this.parentViewController = parentViewController;
        }
    }
}