using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.CodeDom.Compiler;
using BlahguaMobile.BlahguaCore;
using System.Collections.Generic;

namespace BlahguaMobile.IOS
{
	partial class BGSignUpPage02 : UIViewController
	{
		public BGSignUpPage02 (IntPtr handle) : base (handle)
		{
		}

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            string typeId = BlahguaAPIObject.Current.CurrentChannelTypeList.Find(i => i.N == "Industry")._id;
            List<Channel> newList = BlahguaAPIObject.Current.CurrentChannelList.FindAll(i => i.Y == typeId);
            IndustryTable.Source = new ChannelTableSource(newList, true, (BGSignOnPageViewController)ParentViewController);

            typeId = BlahguaAPIObject.Current.CurrentChannelTypeList.Find(i => i.N == "Publishers")._id;
            newList = BlahguaAPIObject.Current.CurrentChannelList.FindAll(i => i.Y == typeId);
            PublishersTable.Source = new ChannelTableSource(newList, false, (BGSignOnPageViewController)ParentViewController);

            this.publicBtn.TouchUpInside += (object sender, EventArgs e) => 
                {
                    SetDefaultChannel(BlahguaAPIObject.Current.GetDefaultChannel());
                    // we are done - dismiss it
                    ((BGSignOnPageViewController)ParentViewController).Finish();
                };


        }

        public static void SetDefaultChannel(Channel theChannel)
        {
            BlahguaAPIObject.Current.SavedChannel = theChannel.ChannelName;
            BlahguaAPIObject.Current.SafeSaveSetting("SavedChannel", theChannel.ChannelName);
            BlahguaAPIObject.Current.CurrentChannel = theChannel;
        }
            
        private class ChannelTableSource : UITableViewSource
        {
            private  List<Channel> channels;
            private bool shouldBadgeUser;
            private string cellID = "A";
            private BGSignOnPageViewController signUpController;

            public ChannelTableSource(List<Channel> channelList, bool badgeUser, BGSignOnPageViewController cont)
            {
                channels = channelList;
                shouldBadgeUser = badgeUser;
                signUpController = cont;
            }

            public override int RowsInSection(UITableView tableview, int section)
            {
                return channels.Count;
            }

            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                UITableViewCell cell = tableView.DequeueReusableCell(cellID);
                if (cell == null)
                    cell = new UITableViewCell(UITableViewCellStyle.Subtitle, cellID);
                cell.TextLabel.Text = channels[indexPath.Row].ChannelName;
                cell.DetailTextLabel.Text = channels[indexPath.Row].D;
                return cell;
            }

            public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
            {
                Channel targetChannel = channels[indexPath.Row];
                tableView.DeselectRow(indexPath, true);
                SetDefaultChannel(targetChannel);
                if (shouldBadgeUser)
                {
                    NSUserDefaults.StandardUserDefaults.SetInt(3, "signupStage");
                    NSUserDefaults.StandardUserDefaults.Synchronize();
                    signUpController.GoToNext();
                }
                else
                    signUpController.Finish();

            }

        }
	}
}
