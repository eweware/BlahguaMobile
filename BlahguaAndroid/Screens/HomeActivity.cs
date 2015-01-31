using System;
using System.IO.IsolatedStorage;
using System.ComponentModel;
using System.Timers;
using System.Collections.Generic;

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
using Android.Graphics;
using Android.Util;
using Android.Text;
using Android.Text.Style;
using Android.Provider;

using BlahguaMobile.AndroidClient;
using BlahguaMobile.AndroidClient.HelpingClasses;
using BlahguaMobile.AndroidClient.Screens;
using BlahguaMobile.AndroidClient.ThirdParty.UrlImageViewHelper;
using BlahguaMobile.BlahguaCore;
using BlahguaMobile.AndroidClient.Adapters;

namespace BlahguaMobile.AndroidClient.Screens
{
    [Activity(MainLauncher = true, ScreenOrientation = ScreenOrientation.Portrait)]
	public partial class  HomeActivity : FragmentActivity
	{

		private BGActionBarDrawerToggle drawerToggle;
		private string drawerTitle;
		//private string title;
		public static GoogleAnalytics analytics = null;
		private IMenu optionsMenu; 
		MainFragment mainFragment = null;

		public static Typeface gothamFont = null;
		public static Typeface merriweatherFont = null;

		private DrawerLayout drawerLayout;
		private ListView drawerListView;

		private string _actionBarTitle;
		//private  string[] Sections;

		private String[] profile_items;


		

		class DrawerItemAdapter<T> : ArrayAdapter<T>
		{
			T[] _items;
			Activity _context;

			public DrawerItemAdapter(Context context, int textViewResourceId, T[] objects) :
			base(context, textViewResourceId, objects)
			{
				_items = objects;
				_context = (Activity)context;
			}

			public override View GetView(int position, View convertView, ViewGroup parent)
			{
				View mView = convertView;
				if (mView == null)
				{
					mView = _context.LayoutInflater.Inflate(Resource.Layout.DrawerListItem, parent, false);

				}

				TextView text = mView.FindViewById<TextView>(Resource.Id.ItemName);

				if (_items[position] != null)
				{
					text.Text = _items[position].ToString();
					//text.SetTextColor(_context.Resources.GetColor(Resource.Color.heard_teal));
					text.SetTypeface(HomeActivity.gothamFont, TypefaceStyle.Normal);
				}

				return mView;
			}
		}


		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.page_home_view);

			this.drawerTitle = this.Title;

			profile_items= new String[]{
				GetString(Resource.String.profilemenu_profile),
				GetString(Resource.String.profilemenu_badges),
				GetString(Resource.String.profilemenu_demographics),
				GetString(Resource.String.profilemenu_history),
				GetString(Resource.String.profilemenu_stats) };

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
				this.Title = this.Title;
				this.InvalidateOptionsMenu ();
			};

			//Display the drawer title and update the options menu
			this.drawerToggle.DrawerOpened += (o, args) => {
				this.ActionBar.Title = "choose channel";
				StopScrolling();
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

			this.ActionBar.SetBackgroundDrawable(new Android.Graphics.Drawables.ColorDrawable( Resources.GetColor(Resource.Color.heard_black)));

			gothamFont = Typeface.CreateFromAsset(this.ApplicationContext.Assets, "fonts/GothamRounded-Book.otf");
			merriweatherFont = Typeface.CreateFromAsset(this.ApplicationContext.Assets, "fonts/Merriweather.otf");


		}

		public int GetContentPositionY()
		{
			var contentFrame = FindViewById<FrameLayout>(Resource.Id.content_frame);
			return contentFrame.Top;
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
				this.drawerListView.Adapter = new DrawerItemAdapter<String> (this, Resource.Layout.item_menu, channels);
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
				Android.Support.V4.App.FragmentTransaction transaction = SupportFragmentManager.BeginTransaction ();
				transaction.Add (Resource.Id.main_frame, mainFragment);
				transaction.AddToBackStack (null);
				transaction.Commit ();
				var count = SupportFragmentManager.BackStackEntryCount;
				this.drawerListView.SetItemChecked (position, true);
				if(BlahguaAPIObject.Current != null && BlahguaAPIObject.Current.CurrentChannelList != null)
					this.Title = BlahguaAPIObject.Current.CurrentChannelList[position].ChannelName;
				this.drawerLayout.CloseDrawers ();
			}

			if (BlahguaAPIObject.Current.CurrentChannelList != null) {
				Channel newChannel = BlahguaAPIObject.Current.CurrentChannelList [position];
				if (newChannel != BlahguaAPIObject.Current.CurrentChannel) {
					this.Title = BlahguaAPIObject.Current.CurrentChannelList [position].ChannelName;
					BlahguaAPIObject.Current.CurrentChannel = BlahguaAPIObject.Current.CurrentChannelList [position];
				} else {
					// start the blah roll going?
					ResumeScrolling ();
				}
				this.drawerLayout.CloseDrawers ();
			}

			switch (position) {
			case 0:

				break;
			}
		}

		protected override void OnTitleChanged (Java.Lang.ICharSequence title, Color color)
		{
			SpannableString s = new SpannableString(title);
			s.SetSpan(new TypefaceSpan(this, "Merriweather.otf"), 0, s.Length(), SpanTypes.ExclusiveExclusive);
			s.SetSpan(new ForegroundColorSpan(Resources.GetColor(Resource.Color.heard_teal)), 0, s.Length(), SpanTypes.ExclusiveExclusive);

			this.ActionBar.TitleFormatted = s;

		}

		public override bool OnPrepareOptionsMenu (IMenu menu)
		{
			this.optionsMenu = menu;
			if (BlahguaAPIObject.Current.CurrentUser == null) {
				this.MenuInflater.Inflate (Resource.Menu.login_menu, menu);
			} else
				this.MenuInflater.Inflate (Resource.Menu.loggedin_menu, menu);

			var drawerOpen = this.drawerLayout.IsDrawerOpen((int)GravityFlags.Left);

			//when open don't show anything

			for (int i = 0; i < menu.Size (); i++) {
				IMenuItem curItem = menu.GetItem (i);
				curItem.SetVisible (!drawerOpen);
				if (BlahguaAPIObject.Current.CurrentUser == null) {
					if (curItem.ItemId == Resource.Id.action_newpost) {
						curItem.SetVisible (false);
					}
				}
				else if (curItem.ItemId == Resource.Id.action_avatar) {
					curItem.SetVisible (false);
				}
			}
			InitAnalytics ();
			InitService ();

			return base.OnPrepareOptionsMenu (menu);
		}

		public bool IsMenuOpened
		{
			get{
				return this.drawerLayout.IsDrawerOpen (this.drawerListView);
			}
		}
		// Pass the event to ActionBarDrawerToggle, if it returns
		// true, then it has handled the app icon touch event
		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			if (this.drawerToggle.OnOptionsItemSelected (item)) 
            {
                /*
			    if (this.drawerLayout.IsDrawerOpen (this.drawerListView))
				    ResumeScrolling ();
			    else
				    StopScrolling ();
                        */
			    return true;
		    }
			
			switch(item.ItemId )
			{
			case Resource.Id.action_login:
				if (BlahguaAPIObject.Current.CurrentUser == null) {
					var intent = new Intent(this, typeof(LoginActivity));
					StartActivity(intent);

				} 

				break;
			case Resource.Id.action_newpost:
				if (IsMenuOpened == false && mainFragment != null)
					//triggerCreateBlock ();
 {					//mainFragment.triggerCreateBlock ();
					var create_intent = new Intent (this, typeof(BlahCreateActivity));
					StartActivity (create_intent);
				}
				break;
			case Resource.Id.action_profile:

				var intent_profile = new Intent(this, typeof(UserProfileActivity));
				intent_profile.PutExtra("Page", 0);
				StartActivity(intent_profile);
				break;
			case Resource.Id.action_badges:

				intent_profile = new Intent(this, typeof(UserProfileActivity));
				intent_profile.PutExtra("Page", 1);
				StartActivity(intent_profile);
				break;
			case Resource.Id.action_demographics:

				intent_profile = new Intent(this, typeof(UserProfileActivity));
				intent_profile.PutExtra("Page",2 );
				StartActivity(intent_profile);
				break;
			case Resource.Id.action_history:
				var intent_history = new Intent (this, typeof(HistoryActivity));
				StartActivity (intent_history);
				break;
			case Resource.Id.action_stats:
				intent_profile = new Intent(this, typeof(UserProfileActivity));
				intent_profile.PutExtra("Page", 4);
				StartActivity(intent_profile);
				break;
			case Resource.Id.action_logout:
				ProgressDialog dialog = new ProgressDialog (this);
				dialog.SetMessage (GetString (Resource.String.roll_message_signing_out));
				dialog.SetCancelable (false);
				dialog.Show ();

				BlahguaAPIObject.Current.SignOut (null, (theStr) => {
					if (theStr) {
						RunOnUiThread (() => {
							ISharedPreferences _sharedPref = PreferenceManager.GetDefaultSharedPreferences (this);
							_sharedPref.Edit ().Remove ("username").Commit ();
							_sharedPref.Edit ().Remove ("password").Commit ();

							populateChannelMenu ();
							dialog.Cancel ();

							optionsMenu.Clear();
							this.MenuInflater.Inflate (Resource.Menu.login_menu, optionsMenu);
						});
					}

					//NavigationService.GoBack();
				});
				break;
			}
			return base.OnOptionsItemSelected (item);
		}

		public void StopScrolling()
		{
			if (mainFragment != null)
				mainFragment.StopTimers ();
		}

		public void ResumeScrolling()
		{
			if (mainFragment != null)
				mainFragment.StartTimers ();
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

				if (refreshItem==null || refreshItem.ActionView == null)
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
			//analytics.StartSession();
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
						//if(mainFragment!= null)
						//	mainFragment.InitLayouts();
						InitLayouts();
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
			if (mainFragment != null) {
				BlahguaAPIObject.Current.FlushImpressionList ();
				mainFragment.StopTimers ();
				mainFragment.ClearBlahs ();
				mainFragment.FetchInitialBlahs ();
			}
				
			analytics.PostPageView("/channel/" + BlahguaAPIObject.Current.CurrentChannel.ChannelName);

			RunOnUiThread(() => {
				this.Title = BlahguaAPIObject.Current.CurrentChannel.ChannelName;
			});
		}

		protected override void OnResume()
		{
			base.OnResume();
			//if (create_post_block.Visibility != ViewStates.Visible)
			//StartTimers();
			initLayouts();
		}

		protected override void OnPause()
		{
			base.OnPause();
		}

		bool firstInit = true;
		public void InitLayouts()
		{
			initLayouts ();
		}
		private void initLayouts()
		{
			if (GetRefreshActionButtonState() == false)
			{

				if (BlahguaAPIObject.Current.CurrentUser != null)
				{
					HomeActivity.analytics.PostAutoLogin();
					//UserInfoBtn.Visibility = Visibility.Visible;
					//NewBlahBtn.Visibility = Visibility.Visible;
					//SignInBtn.Visibility = Visibility.Collapsed;

					//userName.Text = BlahguaAPIObject.Current.CurrentUser.UserName;
					this.optionsMenu.Clear ();
					this.MenuInflater.Inflate (Resource.Menu.loggedin_menu, this.optionsMenu);
					IMenuItem avatarItem = optionsMenu.FindItem (Resource.Id.action_avatar);
					if (avatarItem != null) {
						avatarItem.SetActionView (Resource.Layout.action_login_button);
					}

					SetCreateButtonVisible (true);
					firstInit = false;

				}
				else
				{

				}
			}

			//avatar.SetUrlDrawable(BlahguaAPIObject.Current.CurrentUser.UserImage, avatar.Drawable);
		}

		private DateTime whatsNewTimestamp = DateTime.MinValue;
		private void ShowNewsFloater(WhatsNewInfo newInfo)
		{
			if (whatsNewTimestamp == DateTime.MinValue ||
				DateTime.Now - whatsNewTimestamp > TimeSpan.FromSeconds(5))
			{
				whatsNewTimestamp = DateTime.Now;
				//var dialogToClose = WhatsNewDialog.ShowDialog(FragmentManager, newInfo);
				//new Handler(Looper.MainLooper).PostDelayed(() => { dialogToClose.DismissAllowingStateLoss(); }, App.WhatsNewDialogCloseTimeMs);
			}
		}


		private bool secondaryMenuInitiated = false;

	}
}

