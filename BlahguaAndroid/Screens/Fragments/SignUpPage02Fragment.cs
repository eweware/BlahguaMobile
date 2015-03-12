using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

using Android.Support.V4.App;

using Android.Graphics;
using BlahguaMobile.BlahguaCore;

namespace BlahguaMobile.AndroidClient.Screens
{
    public class SignUpPage02Fragment : Android.Support.V4.App.Fragment
    {
        private Button communityBtn;
        private Button publisherBtn;
        private PopupMenu communityMenu;
        private PopupMenu publisherMenu;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            ViewGroup rootView = (ViewGroup)inflater.Inflate(Resource.Layout.SignUpPage02, container, false);

			rootView.FindViewById<TextView> (Resource.Id.textView1).SetTypeface (HomeActivity.gothamFont, TypefaceStyle.Normal);
			rootView.FindViewById<TextView> (Resource.Id.textView2).SetTypeface (HomeActivity.gothamFont, TypefaceStyle.Normal);
			rootView.FindViewById<TextView> (Resource.Id.titleText).SetTypeface (HomeActivity.gothamFont, TypefaceStyle.Normal);

            communityBtn = rootView.FindViewById<Button>(Resource.Id.CommunityBtn);
            communityBtn.SetTypeface(HomeActivity.gothamFont, TypefaceStyle.Normal);
            PrepCommunityMenu();
            communityBtn.Click += (object sender, EventArgs e) =>
                {
                    communityMenu.Show();
                    
                };


            publisherBtn = rootView.FindViewById<Button>(Resource.Id.PublisherBtn);
            publisherBtn.SetTypeface(HomeActivity.gothamFont, TypefaceStyle.Normal);
            PrepPublisherMenu();
            publisherBtn.Click += (object sender, EventArgs e) =>
            {
                publisherMenu.Show();
            };


            return rootView;
        }

        private void PrepCommunityMenu()
        {
            communityMenu = new PopupMenu(this.Activity, communityBtn);
            Channel defChannel = BlahguaAPIObject.Current.GetDefaultChannel();
            int itemCount = 0;

            communityMenu.Menu.Add(0, itemCount++, 0, defChannel.N); 
            
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

            foreach (Channel curChannel in newList)
            {
                communityMenu.Menu.Add(0, itemCount++, 0, curChannel.N); 
            }

            communityMenu.MenuItemClick += (s, e) =>
                {
                    int itemIndex = e.Item.ItemId;

                    if (itemIndex == 0)
                    {
                        SetDefaultChannel(BlahguaAPIObject.Current.GetDefaultChannel());
                        ((FirstRunActivity)this.Activity).FinishSignin();
                    }
                    else
                    {
                        Channel targetChannel = newList[itemIndex - 1];
                        SetDefaultChannel(targetChannel);
                        ((FirstRunActivity)this.Activity).GoToNext();
                    }
                };
        }

        private void PrepPublisherMenu()
        {
            publisherMenu = new PopupMenu(this.Activity, publisherBtn);

            string typeId = BlahguaAPIObject.Current.CurrentChannelTypeList.Find(i => i.N == "Publishers")._id;
            List<Channel> newList = null;
            int itemCount = 0;

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

            foreach (Channel curChannel in newList)
            {
                IMenuItem curItem = publisherMenu.Menu.Add(0, itemCount++, 0, curChannel.N);
            }

            publisherMenu.MenuItemClick += (s, e) =>
                {
                    int itemIndex = e.Item.ItemId;

                    Channel targetChannel = newList[itemIndex];
                    SetDefaultChannel(targetChannel);
                    ((FirstRunActivity)this.Activity).FinishSignin();
                };

        }


        private void SetDefaultChannel(Channel theChannel)
        {
            BlahguaAPIObject.Current.SavedChannel = theChannel.ChannelName;
            BlahguaAPIObject.Current.SafeSaveSetting("SavedChannel", theChannel.ChannelName);
            BlahguaAPIObject.Current.CurrentChannel = theChannel;
        }
    }
}