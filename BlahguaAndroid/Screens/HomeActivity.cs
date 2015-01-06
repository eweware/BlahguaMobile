using System;
using System.IO.IsolatedStorage;
using System.ComponentModel;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Content.Res;
using Android.Preferences;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;

using BlahguaMobile.AndroidClient;
using BlahguaMobile.AndroidClient.HelpingClasses;
using BlahguaMobile.AndroidClient.Screens;
using BlahguaMobile.AndroidClient.ThirdParty.UrlImageViewHelper;
using BlahguaMobile.BlahguaCore;

namespace BlahguaMobile.AndroidClient.Screens
{
	[Activity (Label = "Home", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, Icon = "@drawable/ic_launcher")]
	public class HomeActivity : FragmentActivity
	{

		private BGActionBarDrawerToggle drawerToggle;
		private string drawerTitle;
		private string title;
		public static GoogleAnalytics analytics = null;
		private IMenu optionsMenu; 
		MainFragment mainFragment = null;

		private DrawerLayout drawerLayout;
		private ListView drawerListView;
		//private  string[] Sections;
		private static readonly string[] Sections = new[] {
			"Browse", "Friends", "Profile"
		};
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.page_home_view);

			this.title = this.drawerTitle = this.Title;

			this.drawerLayout = this.FindViewById<DrawerLayout> (Resource.Id.drawer_layout);
			this.drawerListView = this.FindViewById<ListView> (Resource.Id.left_drawer);

			//Set click handler when item is selected
			this.drawerListView.ItemClick += (sender, args) => ListItemClicked (args.Position);

			//Set Drawer Shadow
			//this.drawerLayout.SetDrawerShadow (Resource.Drawable.drawer_shadow_dark, (int)GravityFlags.Start);
			BlahguaAPIObject.Current.PropertyChanged += new PropertyChangedEventHandler(On_API_PropertyChanged);


			populateChannelMenu ();


			//DrawerToggle is the animation that happens with the indicator next to the actionbar
			this.drawerToggle = new BGActionBarDrawerToggle (this, this.drawerLayout,
				Resource.Drawable.btn_menu_normal,
				Resource.String.app_name,
				Resource.String.app_name);

			//Display the current fragments title and update the options menu
			this.drawerToggle.DrawerClosed += (o, args) => {
				this.ActionBar.Title = this.title;
				this.InvalidateOptionsMenu ();
			};

			//Display the drawer title and update the options menu
			this.drawerToggle.DrawerOpened += (o, args) => {
				this.ActionBar.Title = this.drawerTitle;
				this.InvalidateOptionsMenu ();
			};

			//Set the drawer lister to be the toggle.
			this.drawerLayout.SetDrawerListener (this.drawerToggle);

			//if first time you will want to go ahead and click first item.
			if (savedInstanceState == null) {
				ListItemClicked (0);
			}


			this.ActionBar.SetDisplayHomeAsUpEnabled (true);
			this.ActionBar.SetHomeButtonEnabled (true);
			this.ActionBar.SetDisplayShowHomeEnabled (false);

		}
		private void populateChannelMenu()
		{
			if (BlahguaAPIObject.Current.CurrentChannelList != null) {
				String[] channels = new String[BlahguaAPIObject.Current.CurrentChannelList.Count];
				int channelIndex = 0;

				foreach (Channel curChannel in BlahguaAPIObject.Current.CurrentChannelList) {
					channels [channelIndex++] = curChannel.ChannelName;
				}
				//Sections = channels;
				//Create Adapter for drawer List
				this.drawerListView.Adapter = new ArrayAdapter<string> (this, Resource.Layout.item_menu, channels);
			}
		}
		public ListView LeftMenu {
			get{
				return this.drawerListView;
			}
		}
		private void ListItemClicked (int position)
		{
			if (mainFragment == null) {
				mainFragment = new MainFragment ();
				SupportFragmentManager.BeginTransaction ()
					.Replace (Resource.Id.content_frame, mainFragment)
					.Commit ();

				this.drawerListView.SetItemChecked (position, true);
				if(BlahguaAPIObject.Current != null && BlahguaAPIObject.Current.CurrentChannelList != null)
					ActionBar.Title = this.title = BlahguaAPIObject.Current.CurrentChannelList[position].ChannelName;
				this.drawerLayout.CloseDrawers ();
			}
			if (BlahguaAPIObject.Current.CurrentChannelList != null) {
				ActionBar.Title = this.title = BlahguaAPIObject.Current.CurrentChannelList [position].ChannelName;
				BlahguaAPIObject.Current.CurrentChannel = BlahguaAPIObject.Current.CurrentChannelList [position];
				this.drawerLayout.CloseDrawers ();
			}

			switch (position) {
			case 0:

				break;
			}
		}

		public override bool OnPrepareOptionsMenu (IMenu menu)
		{
			this.optionsMenu = menu;
			this.MenuInflater.Inflate (Resource.Menu.home_menu_actions, menu);
			var drawerOpen = this.drawerLayout.IsDrawerOpen((int)GravityFlags.Left);
			//when open don't show anything

			for (int i = 0; i < menu.Size (); i++) {
				menu.GetItem (i).SetVisible (!drawerOpen);
				if (menu.GetItem (i).ItemId == Resource.Id.action_newpost) {
					if (BlahguaAPIObject.Current.CurrentUser== null || BlahguaAPIObject.Current.CurrentUser.UserName.Equals("")) {
						menu.GetItem (i).SetVisible (false);
					}
				}
			}
			InitAnalytics ();
			InitService ();

			return base.OnPrepareOptionsMenu (menu);
		}

		// Pass the event to ActionBarDrawerToggle, if it returns
		// true, then it has handled the app icon touch event
		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			if (this.drawerToggle.OnOptionsItemSelected (item))
				return true;
			switch(item.ItemId )
			{
			case Resource.Id.action_login:
				Android.App.FragmentTransaction fragmentTx = this.FragmentManager.BeginTransaction ();
				LoginFragment loginFragment = new LoginFragment ();

				fragmentTx.Replace (Resource.Id.content_frame, loginFragment);
				fragmentTx.AddToBackStack (null);
				fragmentTx.Commit ();
				break;
			case Resource.Id.action_newpost:
				mainFragment.triggerCreateBlock ();
				break;
				return true;

			}
			return base.OnOptionsItemSelected (item);
		}
		public void setRefreshActionButtonState(bool refreshing){
			if (optionsMenu != null) {
				IMenuItem refreshItem = optionsMenu.FindItem (Resource.Id.action_login);
				if (refreshItem != null) {
					if (refreshing) {
						refreshItem.SetActionView (Resource.Layout.actionbar_progress);
					} else {
						refreshItem.SetActionView (null);
					}
				}
			}
		}
		public void SetLoginButtonActionView(int resID)
		{
			if (optionsMenu != null) {
				IMenuItem refreshItem = optionsMenu.FindItem (Resource.Id.action_login);
				if (refreshItem != null) {
					refreshItem.SetActionView (resID);
				}
			}
			if (BlahguaAPIObject.Current.CurrentUser.UserName != null) {
				ImageView loginImv = (ImageView)FindViewById (Resource.Id.action_login_button);

				loginImv.SetUrlDrawable(BlahguaAPIObject.Current.CurrentUser.UserImage, loginImv.Drawable);

				if (loginImv != null) {
					loginImv.Click += (object sender, EventArgs e) => {
						Android.App.FragmentTransaction fragmentTx = this.FragmentManager.BeginTransaction ();
						UserProfileFragment profileFragment = new UserProfileFragment ();
						var intent = new Intent (this, typeof(UserProfileFragment));
						intent.PutExtra ("Page", 0);
						profileFragment.Arguments = intent.Extras;
						fragmentTx.Replace (Resource.Id.content_frame, profileFragment);
						fragmentTx.AddToBackStack (null);
						fragmentTx.Commit ();
					};
				}
			}
		}

		public void SetCreateButtonVisible(bool visible)
		{
			for (int i = 0; i < this.optionsMenu.Size (); i++) {
				if (this.optionsMenu.GetItem (i).ItemId == Resource.Id.action_newpost) {
					this.optionsMenu.GetItem (i).SetVisible (visible);
				}
			}
		}
		public bool GetRefreshActionButtonState()
		{
			if (optionsMenu != null) {
				IMenuItem refreshItem = optionsMenu.FindItem (Resource.Id.action_login);
				if (refreshItem.ActionView == null)
					return false;
				else
					return true;
			} else
				return false;
		}
		protected override void OnPostCreate (Bundle savedInstanceState)
		{
			base.OnPostCreate (savedInstanceState);
			this.drawerToggle.SyncState ();
		}

		public override void OnConfigurationChanged (Configuration newConfig)
		{
			base.OnConfigurationChanged (newConfig);
			this.drawerToggle.OnConfigurationChanged (newConfig);
		}

		private void InitAnalytics()
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

			string maker = Build.Manufacturer;
			string model = Build.Model;
			string version = ApplicationContext.PackageManager.GetPackageInfo(ApplicationContext.PackageName, 0).VersionName;
			string platform = "ANDROID";
			string userAgent = "Mozilla/5.0 (Lonux; Android; Mobile) ";

			analytics = new GoogleAnalytics(userAgent, maker, model, version, platform, uniqueId);
			analytics.StartSession();
		}

		private void InitService()
		{
			setRefreshActionButtonState (true);
			ISharedPreferences _sharedPref = PreferenceManager.GetDefaultSharedPreferences(this);
			BlahguaAPIObject.Current.UserName = _sharedPref.GetString("username", "");
			BlahguaAPIObject.Current.UserPassword = _sharedPref.GetString("password", "");

			BlahguaAPIObject.Current.Initialize(null, DoServiceInited);
		}

		private void DoServiceInited(bool didIt)
		{
			RunOnUiThread(() =>
				{
					setRefreshActionButtonState(false);
				});
			//loadTimer.Stop();
			if (didIt)
			{
				RunOnUiThread(() =>
					{
						if(mainFragment!= null)
							mainFragment.InitLayouts();
						populateChannelMenu();
					});
				if (BlahguaAPIObject.Current.CurrentUser != null)
				{

					BlahguaAPIObject.Current.GetWhatsNew((whatsNew) =>
						{
							if ((whatsNew != null) && (whatsNew.message != ""))
							{
								//ShowNewsFloater(whatsNew);
							}
						});
				}
			}
			else
			{
				RunOnUiThread(() =>
					{
						Toast.MakeText(this, "server connection failure", ToastLength.Short).Show();
					});
				//LoadingBox.Visibility = Visibility.Collapsed;
				//ConnectFailure.Visibility = Visibility.Visible;
			}
		}

		private void On_API_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
			case "CurrentChannel":
				OnChannelChanged();
				break;
			}
		}

		private void OnChannelChanged()
		{
			//FlushImpressionList();
			//LoadingBox.Visibility = Visibility.Visible;
			//StopTimers();
			//ClearBlahs();
			//FetchInitialBlahList();
			if (mainFragment != null)
				mainFragment.FetchInitialBlahs();
			HomeActivity.analytics.PostPageView("/channel/" + BlahguaAPIObject.Current.CurrentChannel.ChannelName);

			RunOnUiThread(() => {
				//main_title.Text = BlahguaAPIObject.Current.CurrentChannel.ChannelName;
			});
		}
	}
}

