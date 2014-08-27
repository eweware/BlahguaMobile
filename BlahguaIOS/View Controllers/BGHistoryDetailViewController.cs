// This file has been autogenerated from a class added in the UI designer.

using System;
using System.Drawing;

using BlahguaMobile.BlahguaCore;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.Dialog.Utilities;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.SlideMenu;

namespace BlahguaMobile.IOS
{
	public partial class BGHistoryDetailViewController : UITableViewController
	{
		public string imageUrl;
		public string postTitle;
		public string upDownCount;
		public string timeAgo;

		public new BGHistoryViewController ParentViewController
		{
			get;
			set;
		}

		public BGHistoryDetailViewController (IntPtr handle) : base (handle)
		{

		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
            View.BackgroundColor = UIColor.FromPatternImage(UIImage.FromBundle("grayBack"));
			Title = "History";

			TableView.BackgroundColor = UIColor.White;
			//TableView.BackgroundColor = UIColor.FromPatternImage(UIImage.FromBundle("grayBack"));
			TableView.TableFooterView = new UIView ();
			TableView.AllowsSelection = false;
			TableView.TableHeaderView = new UIView ();
			TableView.SeparatorInset = new UIEdgeInsets (0, 0, 0, 0);
			TableView.ReloadData ();
			TableView.Source = new BGHistoryDetailTableSource(this);
			//TableView.Source = new Source (this);
            NavigationItem.LeftBarButtonItem = new UIBarButtonItem("Back", UIBarButtonItemStyle.Plain, (s, e)=> 
                {
                    this.NavigationController.PopViewControllerAnimated(true);
                });
            NavigationItem.LeftBarButtonItem.SetTitleTextAttributes(new UITextAttributes
                { 
                    TextColor = BGAppearanceConstants.TealGreen, 
                    TextShadowColor = UIColor.Clear, 
                    Font = UIFont.FromName("Merriweather", 16) 
                }, UIControlState.Normal);



		 }
		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			base.PrepareForSegue (segue, sender);
			if (segue.Identifier.Equals ("historyDetailToCommentDetail")) {
			}

		}
	
	}

	public class BGHistoryDetailTableSource : UITableViewSource
	{
		private BGHistoryDetailViewController vc;
		private float yCoordStart; 
		private float labelXCoordStart;
		private const float baseXStart = 30f;
		private const float space = 8f;
		private SizeF baseSizeForFitting = new SizeF (240, 21);





		public BGHistoryDetailTableSource(BGHistoryDetailViewController vc) : base()
		{
			this.vc = vc;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
		
			var cell = tableView.DequeueReusableCell ("C") as SWTableViewCell;

			float CellHeight;

				var leftView = new UIButton ();
				leftView.Frame = new RectangleF (0, 0, 120, tableView.RowHeight); 
                leftView.BackgroundColor = UIColor.FromRGB(31,187, 209);
				leftView.SetTitle ("OPEN POST", UIControlState.Normal);
				leftView.TouchUpInside += (object sender, EventArgs e) => 
                    {
                        string blahID;

                        if (vc.ParentViewController.isBlahs)
                        {
                            int blahIndex = indexPath.Row;
                            
                            blahID = vc.ParentViewController.UserBlahs[blahIndex]._id;
                        }
                        else
                        {
                            int commentIndex = indexPath.Row;
                            blahID = vc.ParentViewController.UserComments[commentIndex].B;
                        }
                        

    				    BlahguaAPIObject.Current.SetCurrentBlahFromId (blahID, (blah) => 
                        {
        					InvokeOnMainThread(() => 
                                {
            						((AppDelegate)UIApplication.SharedApplication.Delegate).CurrentBlah = BlahguaAPIObject.Current.CurrentBlah;

            						var myStoryboard = ((AppDelegate)UIApplication.SharedApplication.Delegate).MainStoryboard;
            						BGBlahViewController objBGBlahViewController = myStoryboard.InstantiateViewController("BGBlahViewController") as BGBlahViewController;
            						BGCommentsViewController commentView = myStoryboard.InstantiateViewController("BGCommentsViewController") as BGCommentsViewController;
            						BGStatsTableViewController statsView = myStoryboard.InstantiateViewController("BGStatsTableViewController") as BGStatsTableViewController;

            						((AppDelegate)UIApplication.SharedApplication.Delegate).CurrentBlah = BlahguaAPIObject.Current.CurrentBlah;
            						SwipeViewController swipeView = new SwipeViewController(objBGBlahViewController, commentView, statsView);
            						((AppDelegate)UIApplication.SharedApplication.Delegate).swipeView = swipeView;
            						((AppDelegate)UIApplication.SharedApplication.Delegate).SlideMenu.NavigationController.PushViewController(swipeView, false);

            					});
    				    });

                };

					
            var buttons = new System.Collections.Generic.List<UIButton> ();

            if (vc.ParentViewController.isBlahs) 
            {       
                UIButton deleteBtn = buttons.AddUtilityButton ("Delete", UIColor.FromRGB(231, 61, 80));

            }

				//buttons.AddUtilityButton ("Edit", UIColor.Blue);


            if (vc.ParentViewController.isBlahs) 
            {
                CellHeight = 0f;
                CellHeight= getHeight (vc.ParentViewController.UserBlahs[indexPath.Row].T);

            } 
            else 
            {

					CellHeight = 0f;
                CellHeight= getHeight (vc.ParentViewController.UserComments[indexPath.Row].T);
					
				}
			
			    cell = new SWTableViewCell (UITableViewCellStyle.Subtitle, CellHeight, "C", tableView, buttons, leftView);
//				cell = new SWTableViewCell (UITableViewCellStyle.Subtitle, "C", tableView, buttons, leftView);
				cell.Scrolling += OnScrolling;
				cell.UtilityButtonPressed += OnButtonPressed;

				
			if (cell.ContentView.Subviews!=null) {

				foreach(UIView sub in cell.ContentView.Subviews)
				{
					sub.RemoveFromSuperview();
				}
			}

			cell.HideSwipedContent (false);//reset cell state
			cell.SetNeedsDisplay ();

			int commentCountVal;
			if (vc.ParentViewController.isBlahs) 
            {
                Blah userBlah = vc.ParentViewController.UserBlahs [indexPath.Row];
				commentCountVal = userBlah.C;
				string historyType = "Blahs";
				SetUp (cell,historyType, userBlah.TypeName,userBlah.T, userBlah.P.ToString (),userBlah.D.ToString (),userBlah.ElapsedTimeString,
					userBlah.ConversionString,commentCountVal);

			} 
            else 
            {
                Comment userComment = vc.ParentViewController.UserComments[indexPath.Row];
				string historyType = "Comments";
				commentCountVal = -1;
				SetUp (cell,historyType,null,userComment.T, userComment.UpVoteCount.ToString (),userComment.DownVoteCount.ToString (),
					userComment.ElapsedTimeString,null,commentCountVal);

			}	
				
			return cell;
		}
			
		private void SetUp(UITableViewCell cell,string historyType,string type,string text, string upVotesText,string downVotesText,string timeString,string conversionStirng,int commentsCount)
		{

			var textView = new UITextView ();
			var upVotesLbl = new UILabel ();
			var downVotesLbl = new UILabel ();
			//var userNameLbl = new UILabel ();
			var daysAgoLbl = new UILabel ();
			var conversionRatioLbl = new UILabel ();
			var commentsCountLbl = new UILabel ();
			var upVoteImageView = new UIImageView ();
			var downVoteImageView = new UIImageView ();
			var conversionImagView = new UIImageView ();
			var commentIconImageView = new UIImageView ();

			yCoordStart = space;
			labelXCoordStart = baseXStart;

			textView.RemoveFromSuperview ();
			if(!String.IsNullOrEmpty(text)) {

				textView.AttributedText = new NSAttributedString (text, UIFont.FromName (BGAppearanceConstants.FontName, 14), UIColor.Black);
				textView.Editable = false;
                textView.TextAlignment = UITextAlignment.Left;
				var newSize = textView.SizeThatFits (new SizeF (320 - baseXStart * 2, 568));
				textView.Frame = new RectangleF (baseXStart-6, yCoordStart, 320 - baseXStart * 2, newSize.Height);
			    cell.ContentView.AddSubview (textView);
				yCoordStart += textView.Frame.Height + space;
			}

			if(!String.IsNullOrEmpty(type)) {

				var commentImageView = new UIImageView ();

				if (type.Equals("says")) {
					commentImageView.Image = UIImage.FromBundle ("icon_speechact_say");
				} else if(type.Equals("predicts")){
					commentImageView.Image = UIImage.FromBundle ("icon_speechact_predict");
				} else if(type.Equals("polls")) {
					commentImageView.Image = UIImage.FromBundle ("icon_speechact_poll");
				} else if(type.Equals("asks")) {
					commentImageView.Image = UIImage.FromBundle ("icon_speechact_ask");
				} else {
					commentImageView.Image = UIImage.FromBundle ("icon_speechact_leak");
				}

				cell.ContentView.AddSubview (commentImageView);
				commentImageView.Frame = new RectangleF (baseXStart, yCoordStart-15, 20f, 20f);
			   
			}

			if (historyType.Equals ("Blahs")) {
                upVoteImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
				upVoteImageView.Image = UIImage.FromBundle ("arrow_up_dark");
                cell.ContentView.AddSubview (upVoteImageView);
				upVoteImageView.Frame = new RectangleF (baseXStart + 30, yCoordStart - 15, 10f, 20f);
				

				labelXCoordStart += 40; 
				upVotesLbl.AttributedText = new NSAttributedString (upVotesText, UIFont.FromName (BGAppearanceConstants.BoldFontName, 14), UIColor.Black);
				SetLabelSize (upVotesLbl, cell);

				downVoteImageView.Image = UIImage.FromBundle ("arrow_down_dark");
                downVoteImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
                downVoteImageView.Frame = new RectangleF (baseXStart + 65, yCoordStart - 15, 10f, 20f);
				cell.ContentView.AddSubview (downVoteImageView);

				labelXCoordStart += 20; 
				downVotesLbl.AttributedText = new NSAttributedString (downVotesText, UIFont.FromName (BGAppearanceConstants.BoldFontName, 14), UIColor.Black); 
				SetLabelSize (downVotesLbl, cell);

			} 

			
		    if (!String.IsNullOrEmpty (conversionStirng)) {

			    conversionImagView.Image = UIImage.FromBundle ("conversion");
                conversionImagView.ContentMode = UIViewContentMode.ScaleAspectFit;
			    conversionImagView.Frame = new RectangleF (baseXStart+100, yCoordStart-15, 20f, 20f);
			    cell.ContentView.AddSubview (conversionImagView);
			    labelXCoordStart += 30; 	

				conversionRatioLbl.AttributedText = new NSAttributedString (conversionStirng, UIFont.FromName (BGAppearanceConstants.MediumFontName, 14), UIColor.Black);
				SetLabelSize (conversionRatioLbl, cell);

		    }

			
			if (commentsCount >= 0) {

				commentIconImageView.Image = UIImage.FromBundle ("comments_dark");
                commentIconImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
				commentIconImageView.Frame = new RectangleF (baseXStart+205, yCoordStart-10, 22f, 19f);
				cell.ContentView.AddSubview (commentIconImageView);
			    labelXCoordStart += 45; 	

				commentsCountLbl.AttributedText = new NSAttributedString (commentsCount.ToString (), UIFont.FromName (BGAppearanceConstants.MediumFontName, 14), UIColor.Black);
				SetLabelSize (commentsCountLbl, cell);
			}

		    yCoordStart += downVotesLbl.Frame.Height + space;
		    labelXCoordStart = baseXStart;
		    daysAgoLbl.AttributedText = new NSAttributedString (timeString, UIFont.FromName (BGAppearanceConstants.MediumItalicFontName, 10), UIColor.Black);
		    SetLabelSize (daysAgoLbl,cell);
		    
		    if(historyType.Equals ("Comments")) {

				labelXCoordStart = 260;
				upVotesLbl.AttributedText = new NSAttributedString (upVotesText, UIFont.FromName (BGAppearanceConstants.BoldFontName, 14),UIColor. FromRGB(115/255.0f,195/255.0f,173/255.0f));
				SetLabelSize (upVotesLbl, cell);

				downVotesLbl.AttributedText = new NSAttributedString ("/"+downVotesText, UIFont.FromName (BGAppearanceConstants.BoldFontName, 14), UIColor.Black); 
				SetLabelSize (downVotesLbl, cell);
		    }

		    //cell.ContentView.Frame = new RectangleF (0, 0, 320, 50);
		}

		private void SetLabelSize(UILabel label,UITableViewCell cell)
		{
			label.RemoveFromSuperview ();
			var newSize = label.SizeThatFits(baseSizeForFitting);
			label.Frame = new RectangleF (labelXCoordStart, yCoordStart-10,newSize.Width, newSize.Height);
			cell.ContentView.AddSubview (label);
			labelXCoordStart += newSize.Width + space;
		}

		void OnScrolling (object sender, ScrollingEventArgs e)
		{
			//uncomment to close any other cells that are open when another cell is swiped

				if (e.CellState != SWCellState.Center) {
				var paths = this.vc.TableView.IndexPathsForVisibleRows;
					foreach (var path in paths) {
						if(path.Equals(e.IndexPath))
						   continue;
					var cell = (SWTableViewCell)this.vc.TableView.CellAt (path);
						if (cell.State != SWCellState.Center) {
							cell.HideSwipedContent (true);
						}
					}
				}
				
		}


		void OnButtonPressed (object sender, CellUtilityButtonClickedEventArgs e)
		{
			if (e.UtilityButtonIndex ==  1) {

				new UIAlertView("Pressed", "You pressed the edit button!", null, null, new[] {"OK"}).Show();
			}
			else if(e.UtilityButtonIndex == 0) 
            {
                var blah = vc.ParentViewController.UserBlahs[e.IndexPath.Row];
				vc.ParentViewController.UserBlahs.Remove (blah);
                BlahguaAPIObject.Current.DeleteBlah (blah._id, BlahDeleted);
				this.vc.TableView.ReloadData ();
			}
		}
			
		public override int RowsInSection (UITableView tableview, int section)
		{
			if(vc.ParentViewController.isBlahs)
			{
				return vc.ParentViewController.UserBlahs.Count;
			}
			else
			{
				return vc.ParentViewController.UserComments.Count;
			}
		}
			
		public override int NumberOfSections (UITableView tableView)
		{
			return 1;
		}

		public override float GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
		{
            float height = 0f;

			if (vc.ParentViewController.isBlahs)
                height = getHeight (vc.ParentViewController.UserBlahs [indexPath.Row].T);
            else
                height = getHeight (vc.ParentViewController.UserComments [indexPath.Row].T);
				
            return height;

		}

		public float getHeight(string textViewString)
		{ 
			float height=0f;
			float xStart=30f;
			float yStart = 8f;
		
			if(!String.IsNullOrEmpty(textViewString)) {
				var obj_textView = new UITextView ();
				obj_textView.RemoveFromSuperview ();
				obj_textView.AttributedText = new NSAttributedString (textViewString, UIFont.FromName (BGAppearanceConstants.FontName, 14), UIColor.Black);
				var newSize = obj_textView.SizeThatFits (new SizeF (320 - xStart * 2, 568));
				obj_textView.Frame = new RectangleF (xStart, yStart, 320 - xStart * 2, newSize.Height);
				height = obj_textView.Frame.Height + 60;
				return height;
			}
			return height;

		}


		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			//cell.Scrolling += OnScrolling;
		}

		public override float GetHeightForHeader (UITableView tableView, int section)
		{
			return 45f;
		}

		public override UIView GetViewForHeader (UITableView tableView, int section)
		{
			UIView headerView = new UIView (new RectangleF (0, 0, 320, 40));
			headerView.BackgroundColor = UIColor.LightGray;
			UILabel label = new UILabel (new RectangleF (20, 7, 280, 21));
			string labelText = String.Empty;
			if(vc.ParentViewController.isBlahs)
			{
				labelText = "Posts" + "(" + vc.ParentViewController.UserBlahs.Count + ")";

			} else {

				labelText = "Comments" + "(" + vc.ParentViewController.UserComments.Count + ")";
			}
			label.AttributedText = new NSAttributedString(
				labelText,
				UIFont.FromName(BGAppearanceConstants.FontName, 17),
				UIColor.Black
			);
			headerView.AddSubview (label);
            return headerView;
			
		}

		public override bool CanEditRow (UITableView tableView, NSIndexPath indexPath)
		{
			if (vc.ParentViewController.isBlahs)
				return true;
			else
				return false;
		}

		

		public void BlahDeleted (string status)
		{
			Console.WriteLine (status);
		}
	}
}