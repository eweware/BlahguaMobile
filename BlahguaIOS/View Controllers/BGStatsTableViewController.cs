// This file has been autogenerated from a class added in the UI designer.

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections;
using System.Collections.Generic;

using BlahguaMobile.BlahguaCore;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Drawing;

namespace BlahguaMobile.IOS
{
    public partial class BGStatsTableViewController : UIViewController
    {
        private UIViewController parentViewController;

        private Blah CurrentBlah
        {
            get
            {
                return ((AppDelegate)UIApplication.SharedApplication.Delegate).CurrentBlah;
            }
        }

        public BGStatsTableViewController(IntPtr handle)
            : base(handle)
        {

        }

        public override void ViewDidLoad()
        {

            try
            {
                base.ViewDidLoad();

                //Synsoft on 9 July 2014 added title
                this.Title = "Stats";
                NavigationItem.LeftBarButtonItem = new UIBarButtonItem("Back", UIBarButtonItemStyle.Plain, BlahHandler);
                //Synsoft on 11 July 2014 for active color  #1FBBD1
                NavigationItem.LeftBarButtonItem.TintColor = UIColor.FromRGB(31, 187, 209);

                //Synsoft on 14 July 2014 for swipping between screens                
                UISwipeGestureRecognizer objUISwipeGestureRecognizer = new UISwipeGestureRecognizer(SwipeToCommentsController);
                objUISwipeGestureRecognizer.Direction = UISwipeGestureRecognizerDirection.Right;
                this.View.AddGestureRecognizer(objUISwipeGestureRecognizer);


                //Synsoft on 10 June 2014 
                // scrollView.ContentSize = new SizeF(scrollView.Frame.Width, scrollView.Frame.Height);
                scrollView.ContentSize = new SizeF(scrollView.Frame.Width, 600);
                scrollView.ScrollEnabled = true;
                Dictionary<string, string> source = new Dictionary<string, string>();

                //Synsoft on 10 June 2014 
                if (CurrentBlah != null)
                {
                    lblConversionRatio.Text = CurrentBlah.ConversionString.ToString();
                    lblOpen.Text = CurrentBlah.O.ToString();
                    lblDemotes.Text = CurrentBlah.D.ToString();
                    lblPromotes.Text = CurrentBlah.P.ToString();
                    lblComment.Text = CurrentBlah.C.ToString();
                    lblImpression.Text = CurrentBlah.V.ToString();
                    lblHeardRatio.Text = CurrentBlah.S.ToString("0.00") + "%";
                    lblOpenedImpression.Text = CurrentBlah.O.ToString() + "/" + CurrentBlah.V.ToString();
                }
                //commented by Synsoft on 10 June 2014 -- old code using table source
                //if (CurrentBlah != null)
                //{
                //    source.Add("Opened: ", CurrentBlah.O.ToString());
                //    source.Add("Viewed: ", CurrentBlah.V.ToString());
                //    source.Add("Promotes: ", CurrentBlah.P.ToString());
                //    source.Add("Demotes: ", CurrentBlah.D.ToString());
                //    source.Add("Comments: ", CurrentBlah.C.ToString());
                //}
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
                //commented by Synsoft on 10 June 2014 -- old code using table source
                //TableView.Source = new BGStatsTableSource (source);
                //new BGStatsTableSource(source);
            }
            catch (Exception e)
            {
                e.Message.ToString();

            }

        }

        //Synsoft on 14 July 2014
        private void SwipeToCommentsController()
        {
            this.NavigationController.PopViewControllerAnimated(true);
        }

        //Synsoft on 11 July 2014 for swipping between screens
        private void BlahHandler(object sender, EventArgs args)
        {
            DismissViewController(true, null);
        }

        public void SetParentViewController(UIViewController parentViewController)
        {
            this.parentViewController = parentViewController;
        }
    }

    //commented by Synsoft on 10 June 2014 -- old code using table source

    /*public class BGStatsTableSource : UITableViewSource
    {
        private Dictionary<string, string> source;

        public BGStatsTableSource(Dictionary<string, string> source) : base()
        {
            this.source = source;
        }

        public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
        {
            var cell = (BGStatsTableCell)tableView.DequeueReusableCell ("cell");
            var element = source.ElementAt (indexPath.Row);
            cell.SetKeyValue (element.Key, element.Value);
            return cell;
        }

        public override int RowsInSection (UITableView tableview, int section)
        {
            return source.Count;
        }

        public override int NumberOfSections (UITableView tableView)
        {
            return 1;
        }
    }*/
}
