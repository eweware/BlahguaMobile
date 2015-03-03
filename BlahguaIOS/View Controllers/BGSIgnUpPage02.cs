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

        private UIActionSheet communitySheet = null;
        private UIActionSheet publisherSheet = null;

		public BGSignUpPage02 (IntPtr handle) : base (handle)
		{
		}

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

			CommunityChannelBtn.TouchUpInside += (object sender, EventArgs e) => {
                if (communitySheet == null)
                    communitySheet = PrepCommunitySelect ();

				communitySheet.ShowInView (View.Window);
			};
			PublisherChannelBtn.TouchUpInside += (object sender, EventArgs e) => {
                if (publisherSheet == null)
                    publisherSheet = PrepPublisherSelect();

				publisherSheet.ShowInView (View.Window);
			};
        }
			
		private UIActionSheet PrepCommunitySelect()
		{
			UIActionSheet actionSheet = new UIActionSheet("Select a Community Channel");
			Channel defaultChannel = BlahguaAPIObject.Current.GetDefaultChannel ();
			actionSheet.AddButton (defaultChannel.N);
			string typeId = BlahguaAPIObject.Current.CurrentChannelTypeList.Find(i => i.N == "Industry")._id;
            List<Channel> newList = null;

            try 
            {
                newList = BlahguaAPIObject.Current.CurrentChannelList.FindAll(i => i.Y == typeId);
            }
            catch (Exception e)
            {

            }

            if ((newList == null) || (newList.Count == 0))
            {
                newList = new List<Channel>();
                newList.Add(BlahguaAPIObject.Current.CurrentChannelList.ChannelFromName("Tech Industry"));
                newList.Add(BlahguaAPIObject.Current.CurrentChannelList.ChannelFromName("Entertainment Industry"));
            }



			foreach (Channel curChannel in newList) {
				actionSheet.AddButton (curChannel.N);
			}

			actionSheet.AddButton ("Cancel");
			actionSheet.CancelButtonIndex = 1 + newList.Count;  // cancel

			actionSheet.Clicked += delegate(object a, UIButtonEventArgs b) 
			{
				if (b.ButtonIndex == 0) {
					SetDefaultChannel(BlahguaAPIObject.Current.GetDefaultChannel());
					((BGSignOnPageViewController)ParentViewController).Finish();
				}
				else if (b.ButtonIndex != actionSheet.CancelButtonIndex)
				{
					Channel targetChannel = newList[b.ButtonIndex - 1];
					SetDefaultChannel(targetChannel);
					NSUserDefaults.StandardUserDefaults.SetInt(3, "signupStage");
					NSUserDefaults.StandardUserDefaults.Synchronize();
					((BGSignOnPageViewController)ParentViewController).GoToNext();
				}

			};

			return actionSheet;
		}

		private UIActionSheet PrepPublisherSelect()
        {
            UIActionSheet actionSheet = new UIActionSheet("Select a Publisher Channel");
            string typeId = BlahguaAPIObject.Current.CurrentChannelTypeList.Find(i => i.N == "Publishers")._id;
            List<Channel> newList = null;

            try
            {
                newList = BlahguaAPIObject.Current.CurrentChannelList.FindAll(i => i.Y == typeId);
            }
            catch (Exception e)
            {
               
            }

            if ((newList == null) || (newList.Count == 0))
            {
                newList = new List<Channel>();
                newList.Add(BlahguaAPIObject.Current.CurrentChannelList.ChannelFromName("Lifestyle"));
            }

			foreach (Channel curChannel in newList) {
				actionSheet.AddButton (curChannel.N);
			}

			actionSheet.AddButton ("Cancel");
			actionSheet.CancelButtonIndex = newList.Count;  // cancel

			actionSheet.Clicked += delegate(object a, UIButtonEventArgs b) 
			{
				if (b.ButtonIndex != actionSheet.CancelButtonIndex) {
					Channel targetChannel = newList[b.ButtonIndex];
					SetDefaultChannel(targetChannel);
					((BGSignOnPageViewController)ParentViewController).Finish();
				}
			};

			return actionSheet;
		}

        public static void SetDefaultChannel(Channel theChannel)
        {
            BlahguaAPIObject.Current.SavedChannel = theChannel.ChannelName;
            BlahguaAPIObject.Current.SafeSaveSetting("SavedChannel", theChannel.ChannelName);
            BlahguaAPIObject.Current.CurrentChannel = theChannel;
        }
	}
}
