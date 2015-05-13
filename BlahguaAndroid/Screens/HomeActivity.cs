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
using Android.Graphics.Drawables;
using Android.Util;
using Android.Text;
using Android.Text.Style;
using Android.Provider;
using Java.IO;
using Android.Support.V7.App;
using System.Threading.Tasks;

using BlahguaMobile.AndroidClient;
using BlahguaMobile.AndroidClient.HelpingClasses;
using BlahguaMobile.AndroidClient.Screens;
using BlahguaMobile.AndroidClient.ThirdParty.UrlImageViewHelper;
using BlahguaMobile.BlahguaCore;
using BlahguaMobile.AndroidClient.Adapters;
using Android.Support.V7.Widget;

using File = Java.IO.File;
using Uri = Android.Net.Uri;

namespace BlahguaMobile.AndroidClient.Screens
{
    [Activity(MainLauncher = true, Theme = "@style/AppTheme", ScreenOrientation = ScreenOrientation.Portrait)]
	public partial class  HomeActivity : Android.Support.V7.App.AppCompatActivity
	{

		private BGActionBarDrawerToggle drawerToggle;
		//private string title;
		public static GoogleAnalytics analytics = null;
		private IMenu optionsMenu; 
		MainFragment mainFragment = null;

		public static Typeface gothamFont = null;
		public static Typeface merriweatherFont = null;

		private DrawerLayout drawerLayout;
		private ListView drawerListView;

		public static bool forceFirstTime = false;
        public static int FIRST_RUN_RESULT = 0x1111, PHOTO_CAPTURE_EVENT = 0x2222;
        public static int LOGIN_RESULT = 0x4444, CREATE_BLAH_CODE = 0x3333;
		public const int MultiChoiceDialog = 0x03;
        
        public static File _dir;
        public static File _file;
        public static int MAX_IMAGE_SIZE = 1024;

        private static bool needsInit = true;
        private static bool isFirstLoad = true;
		private Android.Support.V7.Widget.Toolbar toolbar = null;

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
					mView = _context.LayoutInflater.Inflate(Android.Resource.Layout.SimpleListItemActivated1, parent, false);

				}

				TextView text = mView.FindViewById<TextView>(Android.Resource.Id.Text1);

				if (_items[position] != null)
				{
					text.Text = _items[position].ToString();
					//text.SetTextColor(_context.Resources.GetColor(Resource.Color.heard_teal));
					text.SetTypeface(HomeActivity.gothamFont, TypefaceStyle.Normal);
                    if (position == BlahguaAPIObject.Current.CurrentChannelList.IndexOf(BlahguaAPIObject.Current.CurrentChannel))
                    {
                        text.SetTextColor(_context.Resources.GetColor(Resource.Color.heard_black));
                        text.SetBackgroundColor(Color.White);
                    }
                    else
                    {
                        text.SetTextColor(Color.White);
                        text.SetBackgroundColor(_context.Resources.GetColor(Resource.Color.heard_blue));
                    }

				}

				return mView;
			}
		}



        public const string HOCKEYAPP_APPID = "aadcf778d85a29d59bd4ef87394ee6ad";

		protected override void OnCreate (Bundle savedInstanceState)
		{
            //this.Window.AddFlags(WindowManagerFlags.Fullscreen);
			base.OnCreate (savedInstanceState);

            if (Intent.GetBooleanExtra("EXIT", false))
            {
                // handle app finish
                needsInit = true;
                mainFragment = null;
                BlahguaAPIObject.Delete();
                Finish();
                return;
            }
                

            // init hockey app
            // Register the crash manager before Initializing the trace writer
            HockeyApp.CrashManager.Register (this, HOCKEYAPP_APPID); 

            //Register to with the Update Manager
            HockeyApp.UpdateManager.Register (this, HOCKEYAPP_APPID);

            // Initialize the Trace Writer
            HockeyApp.TraceWriter.Initialize ();

            // Wire up Unhandled Expcetion handler from Android
            Android.Runtime.AndroidEnvironment.UnhandledExceptionRaiser += (sender, args) => 
                {
                    // Use the trace writer to log exceptions so HockeyApp finds them
                    HockeyApp.TraceWriter.WriteTrace(args.Exception);
                    args.Handled = true;
                };

            // Wire up the .NET Unhandled Exception handler
            AppDomain.CurrentDomain.UnhandledException +=
                (sender, args) => HockeyApp.TraceWriter.WriteTrace(args.ExceptionObject);

            // Wire up the unobserved task exception handler
            TaskScheduler.UnobservedTaskException += 
                (sender, args) => HockeyApp.TraceWriter.WriteTrace(args.Exception);

			    //this.Window.DecorView.SystemUiVisibility = StatusBarVisibility.Hidden;
                try
                {
                    SetContentView(Resource.Layout.page_home_view);
                }
                catch (Exception exp)
                {
                    System.Console.WriteLine(exp.Message);
                }
			toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.tool_bar);
               SetSupportActionBar(toolbar);
           

			this.drawerLayout = this.FindViewById<DrawerLayout> (Resource.Id.drawer_layout);
			this.drawerListView = this.FindViewById<ListView> (Resource.Id.left_drawer);
            var headerView = LayoutInflater.Inflate(Resource.Layout.channelheader,null);
            drawerListView.AddHeaderView(headerView);


			//Set click handler when item is selected
			this.drawerListView.ItemClick += (sender, args) => ListItemClicked (args.Position);
			this.drawerListView.Divider = new ColorDrawable (Resources.GetColor(Resource.Color.heard_white));
			this.drawerListView.DividerHeight = 1;

			//DrawerToggle is the animation that happens with the indicator next to the actionbar
			this.drawerToggle = new BGActionBarDrawerToggle (this, this.drawerLayout,
				toolbar,
				Resource.String.app_name,
				Resource.String.app_name);

			//Display the current fragments title and update the options menu
			this.drawerToggle.DrawerClosed += (o, args) => {
				//this.Title = this.Title;
				this.InvalidateOptionsMenu ();
				this.ResumeScrolling();
			};

			//Display the drawer title and update the options menu
			this.drawerToggle.DrawerOpened += (o, args) => {
				//this.ActionBar.Title = "Channel";
				StopScrolling();
				this.InvalidateOptionsMenu ();
			};

			//Set the drawer lister to be the toggle.
			this.drawerLayout.SetDrawerListener (this.drawerToggle);
			drawerToggle.DrawerIndicatorEnabled = true;
			drawerToggle.SyncState ();
            drawerLayout.SetStatusBarBackgroundColor(Resource.Color.heard_red);
            drawerLayout.SetScrimColor(Resource.Color.heard_red);
            drawerLayout.SetDrawerShadow(Resource.Drawable.draweredgeshadow, (int)GravityFlags.Left);
            

			//if first time you will want to go ahead and click first item.
			if (savedInstanceState == null) {
                int curChan = 0;
                if (BlahguaAPIObject.Current.CurrentChannelList != null)
                {
                    curChan = BlahguaAPIObject.Current.CurrentChannelList.IndexOf(BlahguaAPIObject.Current.CurrentChannel);
                    if (curChan < 0)
                        curChan = 0;
                }

                InitChannelMenu (curChan);
			}

			gothamFont = Typeface.CreateFromAsset(this.ApplicationContext.Assets, "fonts/GothamRounded-Book.otf");
			merriweatherFont = Typeface.CreateFromAsset(this.ApplicationContext.Assets, "fonts/Merriweather.otf");



            CreateDirectoryForPictures();


            isFirstLoad = false;
          
		}

        private void CreateDirectoryForPictures()
        {
            _dir = new File(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures), "PhotoTossImages");
            if (!_dir.Exists())
            {
                _dir.Mkdirs();
            }
        }


		public int GetContentPositionY()
		{
			var contentFrame = FindViewById<FrameLayout>(Resource.Id.main_frame);
			return contentFrame.Top;
		}

        private void MaybeShowTutorial()
        {
            ISharedPreferences _sharedPref = PreferenceManager.GetDefaultSharedPreferences(this);
            String seenIt = _sharedPref.GetString("sawtutorial", "");


            if ((String.IsNullOrEmpty(seenIt) || forceFirstTime))
            {
                 _sharedPref.Edit().PutString("sawtutorial", "true").Commit();
                // TutorialDialog.ShowDialog(FragmentManager);
                Intent firstRun = new Intent(this, typeof(FirstRunActivity));
                StopScrolling();
                StartActivityForResult(firstRun, FIRST_RUN_RESULT);
                forceFirstTime = false;
            }
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
                MaybeShowTutorial();
			}
		}
			
		public ListView LeftMenu {
			get{
				return this.drawerListView;
			}
		}
		
        private void InitChannelMenu (int position)
        {
            if (mainFragment == null)
            {

                mainFragment = new MainFragment();
                Android.Support.V4.App.FragmentTransaction transaction = SupportFragmentManager.BeginTransaction();
                transaction.Add(Resource.Id.main_frame, mainFragment);
                transaction.AddToBackStack(null);
                transaction.Commit();
                var count = SupportFragmentManager.BackStackEntryCount;
                this.drawerListView.SetItemChecked(position, true);
                if (BlahguaAPIObject.Current != null && BlahguaAPIObject.Current.CurrentChannelList != null)
                    UpdateChannelTitle(BlahguaAPIObject.Current.CurrentChannelList[position]);
                this.drawerLayout.CloseDrawers();
            }
        }
        
        private void ListItemClicked (int position)
		{
            if (position > 0)
            {
                position--;

                if (BlahguaAPIObject.Current.CurrentChannelList != null)
                {
                    Channel newChannel = BlahguaAPIObject.Current.CurrentChannelList[position];
                    if (newChannel != BlahguaAPIObject.Current.CurrentChannel)
                    {
                        UpdateChannelTitle(newChannel);
                        BlahguaAPIObject.Current.CurrentChannel = newChannel;
                    }
                    else
                    {
                        // start the blah roll going?
                        ResumeScrolling();
                    }
                    this.drawerLayout.CloseDrawers();
                }
            }
		}

        private Channel currentDisplayedChannel = null;
        protected void UpdateChannelTitle(Channel newChan)
        {
            if (newChan == currentDisplayedChannel)
                return;

            currentDisplayedChannel = newChan;
            drawerListView.InvalidateViews();
           
            if (String.IsNullOrEmpty(newChan.HeaderImage))
            {
                this.Title = newChan.ChannelName;
                toolbar.SetBackgroundDrawable(null);
            }
            else
            {
                this.Title = "";
                ImageLoader.Instance.DownloadAsync(newChan.HeaderImage,(b) =>
                        {
                            RunOnUiThread(() =>
                            {
                                if (b != null)
                                {
                                    BitmapDrawable theDrawable;

                                    this.Title = "";
                                    
                                    int headerSize = Resources.GetDimensionPixelOffset(Resource.Dimension.tool_bar_top_padding);
                                    if (headerSize > 0)
                                    {
                                        Bitmap overlay = Bitmap.CreateBitmap(toolbar.Width, toolbar.Height, b.GetConfig());
                                        Canvas canvas = new Canvas(overlay);
                                        Paint backColor = new Paint();
                                        backColor.StrokeWidth = 0;
                                        backColor.SetStyle(Paint.Style.Fill);
                                        float bmScale = (float)(toolbar.Height - headerSize) / (float)b.Height;
                                        Color theCornerColor = new Color(b.GetPixel(0, 0));
                                        backColor.Color = theCornerColor;
                                        canvas.DrawRect(new Rect(0, 0, toolbar.Width, toolbar.Height), backColor);
                                        Matrix offsetMatrix = new Matrix();
                                        offsetMatrix.PostScale(bmScale, bmScale);
                                        offsetMatrix.PostTranslate(0, headerSize);
                                        canvas.DrawBitmap(b, offsetMatrix, null);
                                        theDrawable = new BitmapDrawable(overlay);
                                    }
                                    else
                                        theDrawable = new BitmapDrawable(b);
                                    //this.ActionBar.SetBackgroundDrawable(theDrawable);
                                    toolbar.SetBackgroundDrawable(theDrawable);
                                }
                                else
                                {
                                    this.Title = newChan.ChannelName;
                                    //this.ActionBar.SetBackgroundDrawable(null);
                                    toolbar.SetBackgroundDrawable(null);
                                }

                            });
                            return true;
                        });
            }
        }


		protected override void OnTitleChanged (Java.Lang.ICharSequence title, Color color)
		{
			SpannableString s = new SpannableString(title);
			s.SetSpan(new TypefaceSpan(this, "Merriweather.otf"), 0, s.Length(), SpanTypes.ExclusiveExclusive);
			s.SetSpan(new ForegroundColorSpan(Resources.GetColor(Resource.Color.heard_teal)), 0, s.Length(), SpanTypes.ExclusiveExclusive);

			//this.ActionBar.TitleFormatted = s;
            toolbar.TitleFormatted = s;

		}

		public override bool OnPrepareOptionsMenu (IMenu menu)
		{
			this.optionsMenu = menu;
            menu.Clear();
			if (BlahguaAPIObject.Current.CurrentUser == null) {
				this.MenuInflater.Inflate (Resource.Menu.login_menu, menu);
			} else
            {
                this.MenuInflater.Inflate(Resource.Menu.loggedin_menu, menu);
        
            }
				

			var drawerOpen = this.drawerLayout.IsDrawerOpen((int)GravityFlags.Left);

			//when open don't show anything

			for (int i = 0; i < menu.Size (); i++) 
            {
				IMenuItem curItem = menu.GetItem (i);
				curItem.SetVisible (!drawerOpen);
                if (curItem.ItemId == Resource.Id.action_newpost)
                {
                    if ((BlahguaAPIObject.Current.CurrentUser == null) ||
                        (BlahguaAPIObject.Current.CanPost == false))
                        curItem.SetVisible(false);
                }
                else if (curItem.ItemId == Resource.Id.action_overflow)
                {
                    if (BlahguaAPIObject.Current.CurrentUser != null)
                        SetMenuItemIconToUser(curItem);
                }
			}


			return base.OnPrepareOptionsMenu (menu);
		}

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            return base.OnCreateOptionsMenu(menu);
        }

		private void SetMenuItemIconToUser(IMenuItem curItem)
		{
			if (BlahguaAPIObject.Current.CurrentUser != null) {
				string userImageURL = BlahguaAPIObject.Current.CurrentUser.UserImage;
				ImageLoader.Instance.DownloadAsync (userImageURL, (newBitmap) => {
					if (newBitmap != null) {
						RunOnUiThread(() => {
							BitmapDrawable bd = new BitmapDrawable(Resources, newBitmap);
							curItem.SetIcon(bd);
						});

					}
					return true;
				});
			}
		}

		public override bool OnKeyUp (Keycode keyCode, KeyEvent e)
		{

			switch(keyCode) {
			case Keycode.Menu:
				if (this.optionsMenu != null) {
					optionsMenu.PerformIdentifierAction (Resource.Id.action_overflow, 0);
				}
				break;
			}

			return base.OnKeyUp (keyCode, e);
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

			    return true;
		    }
			
			switch(item.ItemId )
			{
			case Resource.Id.action_login:
				if (BlahguaAPIObject.Current.CurrentUser == null) {
					var intent = new Intent(this, typeof(LoginActivity));
					StartActivityForResult(intent, LOGIN_RESULT);

				} 

				break;
			case Resource.Id.action_newpost:
				if (IsMenuOpened == false && mainFragment != null)
				{					
					var create_intent = new Intent (this, typeof(BlahCreateActivity));
					StartActivityForResult (create_intent, CREATE_BLAH_CODE);
				}
				break;
			case Resource.Id.action_profile:

				var intent_profile = new Intent(this, typeof(UserProfileActivity));
				intent_profile.PutExtra("Page", 0);
				StartActivity(intent_profile);
				break;
			case Resource.Id.action_badges:

				intent_profile = new Intent(this, typeof(UserProfileActivity));
				intent_profile.PutExtra("Page", 2);
				StartActivity(intent_profile);
				break;
			case Resource.Id.action_demographics:

				intent_profile = new Intent(this, typeof(UserProfileActivity));
				intent_profile.PutExtra("Page",1 );
				StartActivity(intent_profile);
				break;
			case Resource.Id.action_history:
                intent_profile = new Intent(this, typeof(UserProfileActivity));
                intent_profile.PutExtra("Page", 3);
                StartActivity(intent_profile);
				break;
			case Resource.Id.action_stats:
				intent_profile = new Intent(this, typeof(UserProfileActivity));
				intent_profile.PutExtra("Page", 5);
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
								ShowNewsFloater(whatsNew);
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
                UpdateChannelTitle(BlahguaAPIObject.Current.CurrentChannel);
                UpdateChannelPermissions();
			});
		}

        protected void UpdateChannelPermissions()
        {
            BlahguaAPIObject.Current.GetChannelPermissionById(BlahguaAPIObject.Current.CurrentChannel._id, (thePerms) =>
                {
                    BlahguaAPIObject.Current.CanPost = thePerms.post;
                    BlahguaAPIObject.Current.CanComment = thePerms.comment;
                    InvalidateOptionsMenu();
                });
        }

		protected override void OnResume()
		{
			base.OnResume();

            if (needsInit)
            {
                BlahguaAPIObject.Current.PropertyChanged += On_API_PropertyChanged;
                InitAnalytics();
                InitService();

                populateChannelMenu ();
                needsInit = false;
            }
                
			initLayouts();
            BlahguaAPIObject.Current.EnsureSignin();
            InvalidateOptionsMenu();
		}

		protected override void OnPause()
		{
			base.OnPause();
		}

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

					if (this.optionsMenu != null) {
						this.optionsMenu.Clear ();
						this.MenuInflater.Inflate (Resource.Menu.loggedin_menu, this.optionsMenu);
						SetMenuItemIconToUser (this.optionsMenu.FindItem (Resource.Id.action_overflow));

						SetCreateButtonVisible (true);
					}

				}
				else
				{

				}
			}

		}

		private DateTime whatsNewTimestamp = DateTime.MinValue;
		private void ShowNewsFloater(WhatsNewInfo newInfo)
		{
			if (whatsNewTimestamp == DateTime.MinValue ||
				DateTime.Now - whatsNewTimestamp > TimeSpan.FromSeconds(5))
			{
				whatsNewTimestamp = DateTime.Now;
                var dialogToClose = WhatsNewDialog.ShowDialog(SupportFragmentManager, newInfo);
				new Handler(Looper.MainLooper).PostDelayed(() => { dialogToClose.DismissAllowingStateLoss(); }, App.WhatsNewDialogCloseTimeMs);
			}
		}


		protected override void OnActivityResult(int requestCode, Android.App.Result resultCode, Intent data)
		{
			if (requestCode == FIRST_RUN_RESULT && resultCode == Android.App.Result.Ok) {
				// to do - change the the correct channel?
				if (!String.IsNullOrEmpty (BlahguaAPIObject.Current.SavedChannel)) {
					Channel newChannel = BlahguaAPIObject.Current.CurrentChannelList.ChannelFromName (BlahguaAPIObject.Current.SavedChannel);
					if (newChannel != null) {
						BlahguaAPIObject.Current.CurrentChannel = newChannel;
					}
                    this.populateChannelMenu();
				}
			} else if (requestCode == CREATE_BLAH_CODE && resultCode == Android.App.Result.Ok) {
				// created a new blah!
				if (BlahguaAPIObject.Current.NewBlahToInsert != null)
				{
					mainFragment.InsertBlahInStream(BlahguaAPIObject.Current.NewBlahToInsert);
					BlahguaAPIObject.Current.NewBlahToInsert = null;

				}
			} 
            else if (requestCode == LOGIN_RESULT && resultCode == Android.App.Result.Ok)
            {
                this.populateChannelMenu();
            }
		}

		private bool alreadyTapped = false;

		public override void OnBackPressed ()
		{
			if (alreadyTapped)
				Finish ();
			else {
				alreadyTapped = true;

				RunOnUiThread(() =>
					{
						Android.App.AlertDialog alert = new Android.App.AlertDialog.Builder(this).Create() ;
                        StopScrolling();
						alert.SetTitle("exit");
						alert.SetMessage("exit from Heard?");
						alert.SetCancelable(true);
						alert.SetButton("ok", (sender, args) =>
							{
								alert.Dismiss();
                                Intent newIntent = new Intent(ApplicationContext, Java.Lang.Class.FromType(typeof(HomeActivity)));
                                newIntent.AddFlags(ActivityFlags.ClearTop);
                                newIntent.PutExtra("EXIT", true);
                                StartActivity(newIntent);
							});
							alert.SetButton2 ("cancel", (sender, args) =>
								{
                                    ResumeScrolling();
									alreadyTapped = false;
									alert.Dismiss();
									
								});
						alert.CancelEvent += (object sender, EventArgs e) => {
							if (alreadyTapped)
								Finish();
						};
						alert.Show();
					});
			}
		}




	}
}

