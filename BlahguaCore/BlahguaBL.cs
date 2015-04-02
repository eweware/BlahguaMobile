﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.IO.IsolatedStorage;
using RestSharp;
using System.Threading;

namespace BlahguaMobile.BlahguaCore
{


    public class BlahguaAPIObject : INotifyPropertyChanged
    {
        public class SavedUserInfo
        {
            public string UserName { get; set; }
            public string Password { get; set; }
        }

        private static BlahguaAPIObject _instance = null;

        ChannelList curChannelList = null;
        ChannelTypeList curChannelTypes = null;
        BlahTypeList blahTypeList = null;
        BlahguaRESTservice BlahguaRest = null;
        Blah currentBlah = null;
        User currentUser = null;
        Channel currentChannel = null;
        BlahCreateRecord createRec = null;
        CommentCreateRecord createCommentRec = null;
        private UserDescription _userDescription = null;
        Timer signinTimer;
        private string _recoveryEmail;
        Dictionary<string, int> ImpressionMap = new Dictionary<string, int>();
        string badgeEndpoint;



        public bool AutoLogin { set; get; }
        public string UserName { get; set; }
        public string UserPassword { get; set; }
        public string UserPassword2 { get; set; }
        public string UserEmailAddress { get; set; }
        public bool NewAccount { get; set; }
		public bool CanPost {get; set;}
		public bool CanComment { get; set; }

		public static int smallTileSize {get; set; }
		public static int mediumTileSize {get; set; }
		public static int largeTileSize {get; set; }


        bool inited = false;
        public Blah NewBlahToInsert { get; set; }
        private Dictionary<string, BadgeRecord> intBadgeMap = new Dictionary<string, BadgeRecord>();
        private ProfileSchema _profileSchema = null;
        private string _savedChannel = "";
        private bool _filterProfanity = true;
        private bool _filterFlaggedContent = true;
        public InboxBlah CurrentInboxBlah { get; set; }

        public string RecoveryEmail
        {
            get { return _recoveryEmail; }
            set
            {
                if (value != _recoveryEmail)
                {
                    _recoveryEmail = value;
                    BlahguaRest.SetRecoveryEmail(value, (theString) =>
                        {
                            OnPropertyChanged("RecoveryEmail");
                        }
                    );
                    
                }
            }
        }


        public bool FilterProfanity
        {
            get { return _filterProfanity; }
            set
            {
                _filterProfanity = value;
                OnPropertyChanged("FilterProfanity");
            }
        }

        public string SavedChannel
        {
            get { return _savedChannel; }
            set
            {
                _savedChannel = value;
                OnPropertyChanged("SavedChannel");
            }
        }


        public string GetCurrentShareURL()
        {
            return BlahguaRest.BaseShareURL + "?blahId=" + CurrentBlah._id;
        }

        public string GetBaseShareURL()
        {
            return BlahguaRest.BaseShareURL;
        }






        public delegate void BlahguaInit_callback(bool didIt);

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                try
                {
                    handler(this, new PropertyChangedEventArgs(name));
                }
                catch (Exception exp)
                {
                    Console.WriteLine(exp.Message);
                    // do nothing for now...
                }
            }
        }

        public UserDescription CurrentUserDescription
        {
            get { return _userDescription; }
            set
            {
                _userDescription = value;
                OnPropertyChanged("CurrentUserDescription");
                if (CurrentUser != null)
                    CurrentUser.DescriptionUpdated();
            }
        }
        

        private BlahguaAPIObject()
        {
            UserName = "";
            UserPassword = "";

            BlahguaRest = new BlahguaRESTservice();
            NewBlahToInsert = null;
            signinTimer = new Timer(timer_callback, null, System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);

        }

        public static BlahguaAPIObject Current
        {
            get
            {
                if (_instance == null)
                    _instance = new BlahguaAPIObject();
                return _instance;
            }
        }

        public void StartSigninTimer()
        {
            signinTimer.Change(2000, 20000);
        }

        public void StopSigninTimer()
        {
            signinTimer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
        }


        public void EnsureSignin()
        {
            if (CurrentUser != null)
            {
                BlahguaRest.CheckUserSignIn((theResult) =>
                {
                    if (theResult == false)
                    {
                        BlahguaRest.SignIn(UserName, UserPassword, (resultStr) =>
                        {

                        }
                        );
                    }
                }
                );
            }
        }

        public void ReportPost(int reportType)
        {
            BlahguaRest.ReportPost(currentBlah._id, reportType);
        }

        public void ReportComment(string commentId, int reportType)
        {
            BlahguaRest.ReportComment(commentId, reportType);
        }

        void timer_callback(object state)
        {
            EnsureSignin();
        }

        public SavedUserInfo GetSavedUserInfo()
        {
            SavedUserInfo theInfo = new SavedUserInfo();
            theInfo.UserName = (string)SafeLoadSetting("username", UserName);
            theInfo.Password = (string)SafeLoadSetting("password", UserPassword);
			System.Console.WriteLine ("Loaded " + theInfo.UserName + "/" + theInfo.Password);
            return theInfo;
        }

        void SaveUserInfo(string name, string password)
        {
			System.Console.WriteLine ("Saving " + name + "/" + password);
            SafeSaveSetting("username", name);
            SafeSaveSetting("password", password);

        }

        void LoadSettings()
        {
			System.Console.WriteLine ("Loading Settings");
            AutoLogin = true;
            FilterProfanity = (bool)SafeLoadSetting("FilterProfanity", true);
            SavedChannel = (string)SafeLoadSetting("SavedChannel", BlahguaAPIObject.Current.GetDefaultChannel());
        }

        void SaveSettings()
        {
			System.Console.WriteLine ("Saving Settings");
            SafeSaveSetting("FilterProfanity", FilterProfanity);
            SafeSaveSetting("SavedChannel", SavedChannel);
        }

        public object SafeLoadSetting(string setting, object defVal)
        {
            System.IO.IsolatedStorage.IsolatedStorageSettings settings = System.IO.IsolatedStorage.IsolatedStorageSettings.ApplicationSettings;
            if (settings.Contains(setting))
                return settings[setting];
            else
            {
                settings.Add(setting, defVal);
                return defVal;
            }
        }

        public ProfileSchema UserProfileSchema
        {
            get { return _profileSchema; }
        }

        public void SafeSaveSetting(string setting, object val)
        {
            System.IO.IsolatedStorage.IsolatedStorageSettings settings = System.IO.IsolatedStorage.IsolatedStorageSettings.ApplicationSettings;
            if (settings.Contains(setting))
                settings[setting] = val;
            else
            {
                settings.Add(setting, val);

            }
			settings.Save();
        }

        public void SafeClearSetting(string setting)
        {
            System.IO.IsolatedStorage.IsolatedStorageSettings settings = System.IO.IsolatedStorage.IsolatedStorageSettings.ApplicationSettings;
            if (settings.Contains(setting))
            {
                settings.Remove(setting);
                settings.Save();
            }
        }

        public string UnprocessText(string originalString)
        {
            if (originalString == null)
                return null;
            else
            {
                string finalString =  originalString.Replace("[_r;", "\n");
                if (FilterProfanity)
                    finalString = Utilities.MaskProfanity(finalString);

                return finalString;
            }
        }

        public string ProcessText(string originalString)
        {
             if (originalString == null)
                return null;
            else
                 return originalString.Replace("\n", "[_r;").Replace("\r", "[_r;");
        }

        public void LoadUserComments(Comments_callback callback)
        {
            string userId = CurrentUser._id;
            BlahguaRest.GetUserComments(userId, (theList) =>
                {
                    if (theList != null)
                    {
                        foreach (Comment curComment in theList)
                        {
                            curComment.T = UnprocessText(curComment.T);
                        }
                    }
                    callback(theList);
                }
            );
            
        }

        public void LoadUserPosts(Blahs_callback callback)
        {
            BlahguaRest.GetUserBlahs((theList) =>
                {
                    if (theList != null)
                    {
						BlahList	newList = new BlahList();
                        foreach (Blah curBlah in theList)
                        {
							if (curBlah.S >= 0)
							{
	                            curBlah.T = UnprocessText(curBlah.T);
	                            curBlah.F = UnprocessText(curBlah.F);
								newList.Add(curBlah);
							}
                        }
                            

						theList = newList;
                    }
                    callback(theList);
                }
                
            );
        }

        public void GetBadgeAuthorities(BadgeAuthorities_callback callback)
        {
            BlahguaRest.GetBadgeAuthorities(callback);
        }

        public void GetBadgeForUser(string authorityId, string_callback callback)
        {
            BlahguaRest.CreateBadgeForUser(authorityId, "", callback);
        }

        public void GetEmailBadgeForUser(string authorityId, string emailAddr, string_callback callback)
        {
            // construct a call manually to the badge authority
            GetBadgeForUser(authorityId, (newHTML) =>
                {
                    if (newHTML.StartsWith("error"))
                    {
                        callback("");
                    }
                    else
                    {
                        string tkv = GetTicket(newHTML);
                        badgeEndpoint = GetEndPoint(newHTML);

                        RestClient apiClient;
                        apiClient = new RestClient(badgeEndpoint);
                        string query = "?tk=" + tkv + "&e=" + emailAddr;
                        RestRequest request = new RestRequest("/badges/credentials" + query, Method.POST);
                        request.AddHeader("Accept", "*/*");

                        apiClient.ExecuteAsync(request, (response) =>
                            {
                                string step2HTML = response.Content;

                                if (step2HTML.IndexOf("invalid email") != -1)
                                {
                                    callback("invalid");
                                }
                                else if (step2HTML.IndexOf("Request Domain") != -1)
                                {
                                    callback("");
                                }
                                else if (step2HTML.IndexOf("granted in the past") != -1)
                                {
                                    callback("existing");
                                }
                                else
                                {
                                    string tk2 = GetTicket(step2HTML);
                                    callback(tk2);
                                }

                            }
                        );
                    }
                    

                }
            );
        }


        public void VerifyEmailBadge(string verificationCode, string ticket, string_callback callback)
        {
            RestClient apiClient;
            apiClient = new RestClient(badgeEndpoint);
            string query = "?tk=" + ticket + "&c=" + verificationCode;
            RestRequest request = new RestRequest("/badges/verify" + query, Method.POST);
            request.AddHeader("Accept", "*/*");

            apiClient.ExecuteAsync(request, (response) =>
                {
                    string step3HTML = response.Content;
                    int theLoc = step3HTML.IndexOf("Congrat");
                    if (theLoc != -1)
                        callback("success");
                    else
                        callback("fail");
                }
            );

        }

        public void RequestBadgeForDomain(string emailDomain, string domainName, string_callback callback)
        {
            RestClient apiClient;
            apiClient = new RestClient(badgeEndpoint);
            string query = "?e=" + emailDomain + "&d=" + domainName;
            RestRequest request = new RestRequest("/badges/support" + query, Method.POST);
            request.AddHeader("Accept", "*/*");

            apiClient.ExecuteAsync(request, (response) =>
            {
                    callback("ok");
            }
            );

        }



        private string GetTicket(string htmlStr)
        {
            if (String.IsNullOrEmpty(htmlStr))
                return "";
            else
            {
                int startVal = htmlStr.IndexOf("'goheard.com") + 1;
                int endVal = htmlStr.IndexOf("'", startVal + 1);
                string tkv = htmlStr.Substring(startVal, endVal - startVal);

                return tkv;
            }

        }

        private string GetEndPoint(string htmlStr)
        {
            int startVal = htmlStr.IndexOf("'ba_end'");
            startVal = htmlStr.IndexOf("value='", startVal) + 7;
            int endVal = htmlStr.IndexOf("'", startVal);
            string endPoint = htmlStr.Substring(startVal, endVal - startVal);

            return endPoint;
        }


        public void Initialize(string defaultChannel, BlahguaInit_callback callBack)
        {
            LoadSettings();
            if (!inited)
            {
                BlahguaRest.GetChannelTypes((cTypeList) =>
                    {
                        System.Diagnostics.Debug.WriteLine("Loaded Channels");
                        if (cTypeList != null)
                        {
                            System.Diagnostics.Debug.WriteLine("Channels OK");
                            curChannelTypes = cTypeList;

                            BlahguaRest.GetBlahTypes((bTypeList) =>
                                {
                                    System.Diagnostics.Debug.WriteLine("Loaded Blah Types");
                                    blahTypeList = bTypeList;
                                    blahTypeList.Remove(blahTypeList.First(i => i.N == "ad"));

                                    if (AutoLogin)
                                    {
        								System.Console.WriteLine("Doing autologin");
                                        System.Diagnostics.Debug.WriteLine("Doing auto login");
                                        SavedUserInfo info = GetSavedUserInfo();
                                        if (!String.IsNullOrEmpty(SavedChannel))
                                        {
                                            defaultChannel = SavedChannel;
                                            Console.WriteLine("restoring channel " + SavedChannel);
                                        }

                                        if (info.UserName != "")
                                        {
                                            SignIn(info.UserName, info.Password, true, (theErrStr) =>
                                                {
                                                    if (theErrStr == null)
                                                    {
                                                        inited = true;
                                                        callBack(true);
                                                    }
                                                    else
                                                        CompletePublicSignin(defaultChannel, callBack);
                                                }
                                            );
                                        }
                                        else
                                            CompletePublicSignin(defaultChannel, callBack);
                                    }
                                    else
                                        CompletePublicSignin(defaultChannel, callBack);

                                });
                        }
                        else
                        {
                            callBack(false);
                        }
                    });
            }
            else
            {
                callBack(true);
            }
        }

        private void CompletePublicSignin(string defaultChannel, BlahguaInit_callback callback)
        {
            bool bIncludeHidden = false;
           

            BlahguaRest.GetPublicChannels(bIncludeHidden, (chanList) =>
               {
                   curChannelList = chanList;
                   Channel newChan;

                   if (defaultChannel == null)
                        newChan = GetDefaultChannel();
                   else
                   {
                       Channel curChan = curChannelList.ChannelFromName(defaultChannel);
                       if (curChan != null)
                           newChan = curChan;
                       else
							newChan = GetDefaultChannel();
                   }
                   currentChannel = null;
                   CurrentChannel = newChan;
                   inited = true;
                   callback(true);
               });
        }

        

        public void UploadPhoto(System.IO.Stream photo, string fileName, string_callback callback)
        {
            BlahguaRest.UploadPhoto(photo, fileName, (newPhotoId) =>
                {
                    callback(newPhotoId);
                }
            );
        }

        public void UploadUserImage(System.IO.Stream photo, string fileName, string_callback callback)
        {
            BlahguaRest.UploadObjectPhoto(CurrentUser._id, "U", photo, fileName, (newPhotoId) =>
            {
                CurrentUser.RefreshUserImage(newPhotoId);
                callback(newPhotoId);
            }
            );
        }

        public void DeleteUserImage(string_callback callback)
        {
            BlahguaRest.DeleteUserImage((theString) =>
                {
                    CurrentUser.RefreshUserImage("");
                    callback(theString);
                }
            );
        }

        public void DeleteBlah(string blahId, string_callback callback)
        {
            BlahguaRest.DeleteBlah(blahId, (theString) =>
            {
                callback(theString);
            }
            );
        }

        public void GetBadgeInfo(string badgeId, BadgeRecord_callback callback)
        {
            if (intBadgeMap.ContainsKey(badgeId))
                callback(intBadgeMap[badgeId]);
            else
            {
                BlahguaRest.GetBadgeInfo(badgeId, (theBadge) =>
                    {
                        
                        intBadgeMap[badgeId] = theBadge;
                        callback(theBadge);
                    }
                );
            }
        }

        public ChannelList CurrentChannelList
        {
            get { return curChannelList; }
            set
            {
                curChannelList = value;
                OnPropertyChanged("CurrentChannelList");
            }
                
        }

        public BlahCreateRecord CreateRecord
        {
            get { return createRec; }
            set
            {
                createRec = value;
                OnPropertyChanged("CreateRecord");
            }
        }

        public CommentCreateRecord CreateCommentRecord
        {
            get { return createCommentRec; }
            set
            {
                createCommentRec = value;
                OnPropertyChanged("CreateCommentRecord");
            }
        }

        public void CreateBlah(Blah_callback callback)
        {
            CreateRecord.G = CurrentChannel.ChannelId;
            if (CreateRecord.BlahType.N == "polls")
            {
                CreateRecord.H = CreateRecord.I.Count;
            }
            else
                CreateRecord.I = null;

            CreateRecord.T = ProcessText(CreateRecord.T);
            CreateRecord.F = ProcessText(CreateRecord.F);

            BlahguaRest.CreateBlah(CreateRecord, (theBlah) =>
                {
                    if (theBlah != null)
                        CreateRecord = new BlahCreateRecord(); // erase the old record
                    callback(theBlah);
                }
                );

        }

        public void CreateComment(Comment_callback callback)
        {
            CreateCommentRecord.B = CurrentBlah._id;

            CreateCommentRecord.T = ProcessText(CreateCommentRecord.T);

            BlahguaRest.CreateComment(CreateCommentRecord, (theComment) =>
                {
                    if (theComment != null)
                    {
                        CurrentBlah.C++;
                        CreateCommentRecord = new CommentCreateRecord();
                        LoadBlahComments((commentList) =>
                            {
                                callback(theComment);
                            }
                        );
                        
                    }
                    else 
                        callback(theComment);
                }
            );

        }

       

        private void _intGetUserProfile(Profile_callback callback)
        {
            BlahguaRest.GetUserProfile((theProfile) =>
                {
                    theProfile.Nickname_perm = 2;
                    CurrentUser.Profile = theProfile;
                    if (callback != null)
                        callback(theProfile);
                }
            );
        }

        private void GetUserProfileSchema(ProfileSchema_callback callback)
        {
            BlahguaRest.GetProfileSchema((theSchema) =>
                {
                    _profileSchema = theSchema;

                    // add the age
                    AddAgeSchemaInfo();
                    UserProfile.Schema = UserProfileSchema;
                    callback(theSchema);
                }
            );
        }

        public void GetUserProfile(Profile_callback callback)
        {
            if (UserProfileSchema == null)
            {
                GetUserProfileSchema((theProfile) =>
                    {
                        _intGetUserProfile(callback);
                    }
                );
            }
            else
                _intGetUserProfile(callback);
        }
            

        public void UpdateUserProfile(string_callback callback)
        {
            BlahguaRest.UpdateUserProfile(CurrentUser.Profile, callback);
        }

        public void UpdateUserName(string userName, string_callback callback)
        {
            UserProfile theProfile = CurrentUser.Profile;

            if (theProfile == null)
            {
                BlahguaRest.UpdateUserName(userName, callback);
            }
            else
            {
                theProfile.A = userName;
                theProfile.Nickname_perm = 2;

                BlahguaRest.UpdateUserProfile(theProfile, callback);
            }
        }

        public void EnsureUserDescription(string_callback callback)
        {
            if (CurrentUserDescription == null)
            {
                BlahguaRest.GetUserDescription(CurrentUser._id, (theDesc) =>
                    {
                        CurrentUserDescription = theDesc;
                        callback(CurrentUserDescription.d);
                    }
                );
            }
            else
            {
                callback( CurrentUser.DescriptionString);
            }
        }

        public void GetUserDescription(string_callback callback)
        {
            BlahguaRest.GetUserDescription(CurrentUser._id, (theDesc) =>
                {
                    CurrentUserDescription = theDesc;
                    callback(CurrentUserDescription.d);
                }
            );
        }

        public ChannelTypeList CurrentChannelTypeList
        {
            get { return curChannelTypes; }
        }

        public BlahTypeList CurrentBlahTypes
        {
            get { return blahTypeList; }
        }

         public string GetImageURL(string baseURL, string size)
        {
            string fullURL;
			if (baseURL.Contains ("http")) {
				switch (size) {
				case "A":
					fullURL = baseURL + "=s" + BlahguaAPIObject.smallTileSize + "-c";
					break;
				case "B":
					fullURL = baseURL + "=s" + BlahguaAPIObject.mediumTileSize + "-c";
					break;
				case "C":
					fullURL = baseURL + "=s" + BlahguaAPIObject.largeTileSize + "-c";
					break;
				default:
					fullURL = baseURL + "=s2048";
					break;
				}
			}
			else  
				fullURL = BlahguaRest.ImageBaseURL + baseURL + "-" + size + ".jpg";

            return fullURL;
        }
        
        public string GetImageURL(string baseURL)
        {
            string fullURL;
			if (baseURL.Contains ("http"))
				fullURL = baseURL + "=s2048";
			else
				fullURL = BlahguaRest.ImageBaseURL + baseURL + ".jpg";

            return fullURL;
        }

        public Blah CurrentBlah
        {
            get { return currentBlah; }
        }

        public User CurrentUser
        {
            get { return currentUser; }
            set
            {
                currentUser = value;
                OnPropertyChanged("CurrentUser");
            }
        }

        public Channel CurrentChannel
        {
            get { return currentChannel; }
            set
            {
                if (currentChannel != value)
                {
                    FlushImpressionList();
                    currentChannel = value;
                    OnPropertyChanged("CurrentChannel");
                    SavedChannel = value.ChannelName;
                    SafeSaveSetting("SavedChannel", SavedChannel);
                }
            }
        }

        public void GoPrevChannel()
        {
            int curIndex = curChannelList.IndexOf(currentChannel);
            curIndex--;
            if (curIndex < 0)
                curIndex = curChannelList.Count - 1;
            CurrentChannel = curChannelList[curIndex];
        }

        public void GoNextChannel()
        {
            int curIndex = curChannelList.IndexOf(currentChannel);
            curIndex++;
            if (curIndex >= curChannelList.Count)
                curIndex =  0;
            CurrentChannel = curChannelList[curIndex];
        }

        public void AddImpression(string blahId)
        {
            if (!ImpressionMap.ContainsKey(blahId))
            {
                ImpressionMap[blahId] = 1;
            }
            else
                ImpressionMap[blahId]++;
        }

        public void FlushImpressionList()
        {
            if (ImpressionMap.Count > 0)
                RecordImpressions(ImpressionMap);

            ImpressionMap.Clear();
        }


        public void SetCurrentBlahFromId(string blahId, Blah_callback callback)
        {
            BlahguaRest.FetchFullBlah(blahId, (theBlah) =>
            {
                if (theBlah != null)
                {
                    BlahguaRest.GetUserDescription(theBlah.A, (theDesc) =>
                    {
                        BlahguaRest.AddBlahOpen(theBlah._id);
                        theBlah.Description = theDesc;
                        theBlah.T = UnprocessText(theBlah.T);
                        theBlah.F = UnprocessText(theBlah.F);
                        currentBlah = theBlah;
                        theBlah.AwaitBadgeData((didIt) =>
                            {
                                callback(theBlah);
                            });
                    });
                }
                else
                    callback(null);
            });
        }

        public void SetBlahVote(int newVote, int_callback callback)
        {
            BlahguaRest.SetBlahVote(CurrentBlah._id, newVote, (theInt) => 
                {
                    if (theInt == newVote)
                    {
                        CurrentBlah.uv = theInt;
                        callback(theInt);
                    }
                }
            );
        }

        public void SetCommentVote(Comment theComment, int newVote, int_callback callback)
        {
            BlahguaRest.SetCommentVote(theComment._id, newVote, (theInt) => 
                {
                    if (theInt == newVote)
                    {
                        theComment.UserVote = theInt;
                        callback(theInt);
                    }
                }
            );
        }

        public void RecordImpressions(Dictionary<string, int> impressionMap)
        {
            if ((impressionMap != null) && (impressionMap.Count > 0))
            {
                BlahguaRest.RecordImpressions(impressionMap, null);
            }
        }

        public void GetInbox(Inbox_callback callback)
        {
            BlahguaRest.GetInbox(CurrentChannel._id, (theList) =>
                {
                    if (theList != null)
                    {
                        foreach (InboxBlah theBlah in theList)
                        {
                            theBlah.T = UnprocessText(theBlah.T);
                        }
                    }
                    callback(theList);
                }
            );
        }

        public Channel GetDefaultChannel()
        {
            if ((this.CurrentChannelTypeList != null) && (this.CurrentChannelList != null))
            {
                ChannelType heardType = this.CurrentChannelTypeList.First(i => i.N == "Official Heard");
                Channel theChan;

                try {
                    theChan = this.CurrentChannelList.First (i => i.Y == heardType._id);
                }
                catch (Exception exp) {
                    theChan = this.CurrentChannelList[0];
                }

                return theChan;
            }
            else
                return null;
        }

        public void LoadBlahComments(Comments_callback callback)
        {
            BlahguaRest.GetBlahComments(currentBlah._id, (comments) =>
                {
                    if (comments != null)
                    {
                        List<Comment>   sortedList = comments.OrderByDescending(c => c.CreationDate).ToList();
                        comments = new CommentList();

                        foreach (Comment theComment in sortedList)
                        {
                            if ((theComment.XXX == false) || 
                                ((CurrentUser != null) && (CurrentUser.WantsMatureContent == true)))
                            {
                                theComment.T = UnprocessText(theComment.T);
                                comments.Add(theComment);
                            }
                        }

                        List<string> authorIds = GetCommentAuthorIds(comments);
                        BlahguaRest.GetCommentAuthorDescriptions(authorIds, (descList) =>
                            {
                                ApplyAuthorDescriptions(comments, descList);
                                comments = ThreadComments(comments, true);
                                currentBlah.Comments = comments;
                                callback(comments);

                            }
                        );
                    }
                   
                }
            );
        }

        void ApplyAuthorDescriptions(CommentList commList, CommentAuthorDescriptionList descList)
        {
            foreach (Comment curComm in commList)
            {
                foreach (CommentAuthorDescription curDesc in descList)
                {
                    if (curDesc.i == curComm.A)
                    {
                        curComm.K = curDesc.K;
                        curComm.d = curDesc.d;
                        curComm._m = curDesc._m;
                    }
                }
            }
        }

        List<string> GetCommentAuthorIds(CommentList theList)
        {
            List<string> authorList = new List<string>();

            foreach (Comment curComment in theList)
            {
                if (!authorList.Contains(curComment.A))
                {
                    authorList.Add(curComment.A);
                }
            }

            return authorList;
        }


        public void GetUserPollVote(PollVote_callback callback)
        {
            if (!CurrentBlah.IsPollInited)
            {
                if (CurrentUser != null)
                {
                    BlahguaRest.GetUserPollVote(CurrentBlah._id, (theVote) =>
                        {
                            CurrentBlah.UpdateUserPollVote(theVote);
                            callback(theVote);
                        }
                        );
                }
                else
                {
                    CurrentBlah.UpdateUserPollVote(null);
                    callback(null);
                }
            }
            else
                callback(null);
        }

        public void LoadUserStatsInfo(UserInfo_callback callback)
        {
            DateTime endDate = DateTime.Today;
            DateTime startDate = endDate - new TimeSpan(7, 0, 0, 0);

            BlahguaRest.GetUserStatsInfo(startDate, endDate, (theStats) =>
                {
                    theStats.SetDateRange(startDate, endDate);
                    CurrentUser.UserInfo = theStats;
                   

                    callback(CurrentUser.UserInfo);
                }
            );
        }

        public void LoadBlahStats(Stats_callback callback)
        {
            if (CurrentBlah.L != null)
                callback(CurrentBlah.L);
            else
            {
                DateTime endDate = DateTime.Today;
                DateTime startDate = endDate - new TimeSpan(7, 0, 0, 0);
                BlahguaRest.GetBlahWithStats(CurrentBlah._id, startDate, endDate, (blahWithStats) =>
                {
                    int curStatIndex = 0;
                    CurrentBlah.L = new Stats();
                    if (blahWithStats != null)
                    {
                        DateTime curDate = startDate;

                        while (curDate <= endDate)
                        {
                            if (curStatIndex >= blahWithStats.L.Count)
                            {
                                CurrentBlah.L.Add(new StatDayRecord(curDate));
                            }
                            else
                            {
                                if (blahWithStats.L[curStatIndex].StatDate != curDate)
                                {
                                    CurrentBlah.L.Add(new StatDayRecord(curDate));
                                }
                                else
                                {
                                    CurrentBlah.L.Add(blahWithStats.L[curStatIndex]);
                                    curStatIndex++;
                                }
                            }
                            curDate = curDate.AddDays(1);
                        }
                    }
                    else
                        CurrentBlah.L = null;

                    callback(CurrentBlah.L);

                }
                );

            }
        }

        public void GetUserPredictionVote(PredictionVote_callback callback)
        {
            if (!CurrentBlah.IsPredictInited)
            {
                if (CurrentUser != null)
                {
                    BlahguaRest.GetUserPredictionVote(CurrentBlah._id, (theVote) =>
                    {
                        CurrentBlah.UpdateUserPredictionVote(theVote);
                        callback(theVote);
                    }
                        );
                }
                else
                {
                    CurrentBlah.UpdateUserPredictionVote(null);
                    callback(null);
                }
            }
            else
                callback(null);
        }

        public void SetPollVote(PollItem theVote, PollVote_callback callback)
        {
            // first, ensure we really have a poll and we are voting on it...
            if ((CurrentBlah != null) && (CurrentBlah.I != null))
            {
                int index = CurrentBlah.I.IndexOf(theVote);
                BlahguaRest.SetUserPollVote(CurrentBlah._id, index, (thePollVote) =>
                {
                    // need to update everything
                    if (thePollVote != null)
                    {
                        CurrentBlah.J[thePollVote.W]++;
                        CurrentBlah.UpdateUserPollVote(thePollVote);
                    }
                    callback(thePollVote);
                }
                );
            }
        }

        public void SetPredictionVote(PollItem userVote, PredictionVote_callback callback)
        {
            string voteStr = userVote.PredictVoteStr;
            bool isExpired = CurrentBlah.IsPredictionExpired;

            BlahguaRest.SetUserPredictionVote(CurrentBlah._id, voteStr, isExpired, (thePred) =>
                {
                    if (thePred != null)
                    {
                        switch (voteStr)
                        {
                            case "y":
                                if (isExpired)
                                    CurrentBlah._1++;
                                else
                                    CurrentBlah._4++;
                                break;
                            case "n":
                                if (isExpired)
                                    CurrentBlah._2++;
                                else
                                    CurrentBlah._5++;
                                break;
                            case "u":
                                if (isExpired)
                                    CurrentBlah._3++;
                                else
                                    CurrentBlah._6++;
                                break;
                        }

                        CurrentBlah.UpdateUserPredictionVote(thePred);
                    }
                    callback(thePred);
                }
            );
        }

        void AddCommentAndChildren(CommentList flatTree, Comment theComment, int indentLevel)
        {
            theComment.IndentLevel = indentLevel;
            flatTree.Add(theComment);
            if (theComment.subComments != null)
            {
                foreach (Comment curComment in theComment.subComments)
                {
                    AddCommentAndChildren(flatTree, curComment, indentLevel + 1);
                }
            }
        }

        
        CommentList ThreadComments(CommentList comments, bool init)
        {
            CommentList threadedList = new CommentList();
            CommentList flatTree = new CommentList();

            if (init)
            {
                foreach (Comment curComment in comments)
                {
                    if (curComment.CID != null)
                    {
                        Comment parent = comments.First(comment => comment._id == curComment.CID);
                        if (parent.subComments == null)
                            parent.subComments = new CommentList();
                        parent.subComments.Add(curComment);
                    }
                    else
                    {
                        threadedList.Add(curComment);
                    }
                }
            }
            else
            {
                foreach (Comment curComment in comments)
                {
                    threadedList.Add(curComment);
                }
            }  

            foreach (Comment curComment in threadedList)
            {
                AddCommentAndChildren(flatTree, curComment, 0);
            }

            return flatTree;
        }

        public void SignIn(string userName, string password, bool saveIt, string_callback callBack)
        {
            BlahguaRest.SignIn(userName, password, (resultStr) =>
                {
                    if (resultStr == "")
                    {
                        UserName = userName;
                        UserPassword = password;
                        if (saveIt)
                        {
                            AutoLogin = true;
                            SaveUserInfo(userName, password);
                            SaveSettings();
                        }
                        PrepareForNewUser(callBack);
                    }
                    else
                    {
                        callBack(resultStr);
                    }
                }
            );
        }

        public void SignOut(string returnChannel, BlahguaInit_callback callBack)
        {
            BlahguaRest.SignOut((resultStr) =>
                {
                    CurrentUser = null;
                    CurrentUserDescription = null;
                    ClearAutoLogin();
                    UserName = "";
                    UserPassword = "";
                    UserPassword2 = "";
                    CompletePublicSignin(returnChannel, callBack);
                }
            );
        }

        public void ClearAutoLogin()
        {
            AutoLogin = false;
            SafeClearSetting("username");
            SafeClearSetting("password");
            SaveSettings();
        }

        public void Register(string userName, string password, bool saveIt, string_callback callBack)
        {
            BlahguaRest.Register(userName, password, (resultStr) =>
                {
                    if (resultStr == "")
                    {
                        SignIn(userName, password, saveIt, callBack);
                    }
                    else
                    {
                        callBack(resultStr);
                    }
                }
            );
        }

        public void RefreshUserBadges(bool_callback callback)
        {
            BlahguaRest.GetUserInfo((newUser) =>
                {
                    CurrentUser.B = newUser.B;
					CurrentUser.AwaitBadgeData(callback);
                    
                }
            );
        }

        void AddChannelsToUser(ChannelList theList, ChannelList_callback theCallback)
        {
            Channel curChanel = theList[0];
            if (curChanel.R > 0)
            {
                BlahguaRest.AddUserToChannel(curChanel._id, (didItStr) =>
                {
                    theList.RemoveAt(0);
                    if (theList.Count > 0)
                        AddChannelsToUser(theList, theCallback);
                    else
                    {
                        BlahguaRest.GetUserChannels(theCallback);
                    }
                });
            }
            else
            {
                theList.RemoveAt(0);
                if (theList.Count > 0)
                    AddChannelsToUser(theList, theCallback);
                else
                {
                    BlahguaRest.GetUserChannels(theCallback);
                }
            }
            
        }


		public void GetChannelPermissionById(string channelId, ChannelPermissions_callback theCallback)
		{
			BlahguaRest.GetChannelPermissionById(channelId, (thePerms) =>
				{
					theCallback(thePerms);
				}
			);
		}

		public void GetCurrentChannelPermission(ChannelPermissions_callback theCallback)
		{
			CanPost = true;
			CanComment = true;
			BlahguaRest.GetChannelPermissionById(CurrentChannel._id, (thePerms) =>
				{
					if (thePerms != null) {
						CanPost = thePerms.post;
						CanComment = thePerms.comment;
					}
					else {
						thePerms = new ChannelPermissions();
						thePerms.post = true;
						thePerms.comment = true;
					}

					theCallback(thePerms);
				}
			);
		}


        public void GetWhatsNew(WhatsNew_callback theCallback)
        {
            BlahguaRest.GetWhatsNew((newInfo) =>
            {
                theCallback(newInfo);
            }
            );
        }

        void GetOrAddUserChannels(ChannelList_callback callback)
        {
            BlahguaRest.GetUserChannels((chanList) =>
                {
                    if ((chanList == null) || (chanList.Count == 0))
                    {
                        BlahguaRest.GetPublicChannels(true, (userChanList) =>
                            {
                                AddChannelsToUser(userChanList, callback);
                            }
                        );
                    }
                    else
                        callback(chanList);
                }
            );
        }

        void PrepareForNewUser(string_callback callBack)
        {
            GetOrAddUserChannels((chanList) =>
                {
                    CurrentChannelList = chanList;
					if (!String.IsNullOrEmpty(SavedChannel)) {
                        Channel savedChan = curChannelList.ChannelFromName(SavedChannel);
						if (savedChan != null)
							CurrentChannel = savedChan;
						else
							CurrentChannel = GetDefaultChannel();
					}
                    else
						CurrentChannel = GetDefaultChannel();

                    BlahguaRest.GetUserInfo((newUser) =>
                        {
                            CurrentUser = newUser;
                            BlahguaRest.GetProfileSchema((theSchema) =>
                                {
                                    _profileSchema = theSchema;

                                    // add the age
                                    AddAgeSchemaInfo();
                                    UserProfile.Schema = theSchema;

                                    GetUserProfile((theProfile) =>
                                        {
											EnsureUserDescription((theDesc) =>
												{
													if (CurrentUser.B != null)
													{
														CurrentUser.AwaitBadgeData((didIt)=> 
															{
																callBack(null);
															});
													}
													else callBack(null);
												}
											);
                                        }
                                    );
                                }
                            );
                        }
                    );
                }
            );  
        }

        private void AddAgeSchemaInfo()
        {
            Dictionary<string, string> newDict = new Dictionary<string, string>();
            newDict.Add("0", "under 18");
            newDict.Add("1", "18-24");
            newDict.Add("2", "25-34");
            newDict.Add("3", "35-44");
            newDict.Add("4", "45-54");
            newDict.Add("5", "55-64");
            newDict.Add("6", "over 65");
            newDict.Add("-1", "unspecified");

            _profileSchema.C.DT = newDict;

        }

        public void GetRecoveryEmail(string_callback callback)
        {
            BlahguaRest.GetRecoveryEmail((theString) =>
                {
                    if (theString != _recoveryEmail)
                    {
                        _recoveryEmail = theString;
                        OnPropertyChanged("RecoveryEmail");
                    }
                    callback(theString);
                }
            );
        }

        public void SetRecoveryEmail(string emailAddress, string_callback callback)
        {
            BlahguaRest.SetRecoveryEmail(emailAddress, (resultStr) => 
                {
                    CurrentUser.RecoveryEmail = resultStr;
                    callback(resultStr);
                }
            );
        }

        public void UpdatePassword(string newPassword, string_callback callback)
        {
            BlahguaRest.UpdatePassword(newPassword, (resultStr) =>
            {
                callback(resultStr);
            }
           );
        }

        public void UpdateMatureFlag(bool matureFlag, string_callback callback)
        {
            BlahguaRest.UpdateMatureFlag(matureFlag, (resultStr) =>
            {
                callback(resultStr);
            }
           );
        }

        public void RecoverUser(string userName, string email, string_callback callback)
        {
            BlahguaRest.RecoverUser(userName, email, (resultStr) =>
                {
                    callback(resultStr);
                }
           );
        }

        public void EnsureUserHasChannel(string channelName, bool_callback callback)
        {
            Channel theChan = curChannelList.ChannelFromName(channelName);

            if (theChan == null)
            {
                BlahguaRest.GetPublicChannels(true, (chanList) =>
                    {

                        theChan = chanList.ChannelFromName(channelName);
                        if (theChan != null)
                        {
                            BlahguaRest.AddUserToChannel(theChan._id, (didItStr) =>
                                {
                                    callback(true);
                                }
                            );
                        }
                        else
                        {
                            callback(false);
                        }
                    }
                );
            }
            else
                callback(true);
        }



    }
}
