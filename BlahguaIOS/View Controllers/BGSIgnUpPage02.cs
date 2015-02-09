using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.CodeDom.Compiler;
using BlahguaMobile.BlahguaCore;

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

            this.techBtn.TouchUpInside += (object sender, EventArgs e) => 
                {
                    // we are done - dismiss it
                    SetDefaultChannel("Tech Industry");
                    ((BGSignOnPageViewController)ParentViewController).GoToNext();
                };

            this.entertainmentBtn.TouchUpInside += (object sender, EventArgs e) => 
                {
                    // we are done - dismiss it
                    SetDefaultChannel("Entertainment Industry");
                    ((BGSignOnPageViewController)ParentViewController).GoToNext();
                };


            this.publicBtn.TouchUpInside += (object sender, EventArgs e) => 
                {
                    SetDefaultChannel("Public");
                    // we are done - dismiss it
                    ((BGSignOnPageViewController)ParentViewController).Finish();
                };


        }

        private void SetDefaultChannel(string channelName)
        {
            BlahguaAPIObject.Current.SavedChannel = channelName;
            BlahguaAPIObject.Current.SafeSaveSetting("SavedChannel", channelName);
			BlahguaAPIObject.Current.CurrentChannel = BlahguaAPIObject.Current.CurrentChannelList.ChannelFromName (channelName);
        }
	}
}
