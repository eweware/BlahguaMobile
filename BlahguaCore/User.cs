using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace BlahguaMobile.BlahguaCore
{

    public class WhatsNewInfo
    {
        public string message { get; set; }
        public int newComments { get; set; }
        public int newOpens { get; set; }
        public int newUpVotes { get; set; }
        public int newDownVotes { get; set; }
        public int newMessages { get; set; }
        public int newViews { get; set; }
        public int newPosts { get; set; }
        public int newCommentUpVotes { get; set; }
        public int newCommentDownVotes { get; set; }

        public string SummaryString
        {
            get
            {
                string theStr = "";

                if ((newViews  > 0) || (newOpens > 0))
                {
                    bool didIt = false;

                    if (newViews > 0)
                    {
                        theStr += "Your posts have been viewed " + newViews.ToString() + " time";
                        if (newViews > 1)
                            theStr += "s";
                        didIt = true;
                    }

                    if (newOpens > 0)
                    {
                        if (!didIt)
                            theStr += "Your posts have been opened ";
                        else
                            theStr += " and opened ";
                        theStr += newOpens.ToString() + " time";
                        if (newOpens > 1)
                            theStr += "s";
                    }
                    theStr += ".\n";
                }

                if (newComments > 0)
                {
                    theStr += "Your posts have received " + newComments.ToString() + " new comment";
                    if (newComments > 1)
                        theStr += "s";
                    theStr += ".\n";

                }

                if ((newUpVotes > 0) || (newDownVotes > 0))
                {
                    bool didIt = false;
                    theStr += "Your posts have been ";
                    if (newUpVotes > 0)
                    {
                        theStr += "promoted ";
                        theStr += newUpVotes.ToString() + " time";
                        if (newUpVotes > 1)
                            theStr += "s";
                        didIt = true;
                    }

                    if (newDownVotes > 0)
                    {
                        if (didIt)
                            theStr += " and ";
                        theStr += "demoted ";
                        theStr += newDownVotes.ToString() + " time";
                        if (newUpVotes > 1)
                            theStr += "s";
                    }

                    theStr += ".\n";
                }


                if ((newCommentUpVotes > 0) || (newCommentDownVotes > 0))
                {
                    bool didIt = false;
                    theStr += "Your comments have been ";
                    if (newUpVotes > 0)
                    {
                        theStr += "promoted ";
                        theStr += newUpVotes.ToString() + " time";
                        if (newUpVotes > 1)
                            theStr += "s";
                        didIt = true;
                    }

                    if (newDownVotes > 0)
                    {
                        if (didIt)
                            theStr += " and ";
                        theStr += "demoted ";
                        theStr += newDownVotes.ToString() + " time";
                        if (newUpVotes > 1)
                            theStr += "s";
                    }

                    theStr += ".  ";
                }
            

                return theStr;
                
            }
        }
    }


    public class User : INotifyPropertyChanged
    {
        public List<string> M { get; set; }
        public List<string> B { get; set; }
        public string N { get; set; }
        public double S { get; set; }
        public double K { get; set; }
        public string _id { get; set; }
        public bool XXX { get; set; }

		public string c { get; set; }
		private DateTime _createDate = DateTime.MinValue;
		public string u { get; set; }
		private DateTime _updateDate = DateTime.MinValue;

        private CommentList _commentHistory;
        private BlahList _postHistory;
        private UserInfoObject _info = null;
        private UserProfile _theProfile = null;
        private string _recoveryEmail = null;
        private BadgeList _intBadgeList = null;
        public event PropertyChangedEventHandler PropertyChanged;


        public User()
        {
            _theProfile = null;
            XXX = false;
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                try
                {
                    handler(this, new PropertyChangedEventArgs(name));
                }
                catch (Exception)
                {
                    // do nothing for now...
                }
            }
        }



        public void DescriptionUpdated()
        {
            OnPropertyChanged("DescriptionString");
        }

        public void RefreshUserImage(string newImage)
        {
            if (newImage == "")
                M = null;
            else
            {
                M = new List<string>();
                M.Add(newImage);
            }
            OnPropertyChanged("UserImage");
        }


        public UserProfile Profile
        {
            get { return _theProfile; }
            set
            {
                _theProfile = value;
                OnPropertyChanged("Profile");
            }
        }

        public UserInfoObject UserInfo
        {
            get { return _info; }
            set 
            { 
                _info = value;
                OnPropertyChanged("UserInfo");
            }
        }

        public string AccountName
        {
            get
            {
                return N;
            }
            set
            {
                N = value;
            }
        }

        public string UserName
        {
            get
            {
                string theName = "Someone";
                if ((BlahguaAPIObject.Current.CurrentUserDescription != null) && (BlahguaAPIObject.Current.CurrentUserDescription.K != null))
                    theName = BlahguaAPIObject.Current.CurrentUserDescription.K;

                return theName;
            }

            set
            {
                if (BlahguaAPIObject.Current.CurrentUserDescription != null)
                    BlahguaAPIObject.Current.CurrentUserDescription.K = value;
                OnPropertyChanged("UserName");
            }
        }

        public bool WantsMatureContent
        {
            get { return XXX; }
            set 
            { 
                XXX = value;
                OnPropertyChanged("WantsMatureContent");
            }
        }

        public string RecoveryEmail
        {
            get
            {
                return _recoveryEmail;
            }
            set
            {
                _recoveryEmail = value;
                OnPropertyChanged("RecoveryEmail");
            }
        }

        public string UserImage
        {
            get
            {
                if ((M != null) && (M.Count > 0)) 
                    return BlahguaAPIObject.Current.GetImageURL(M[0], "A");
                else
                    return "https://s3-us-west-2.amazonaws.com/app.goheard.com/images/unknown-user.png";
            }
        }

        public string DescriptionString
        {
            get
            {
                if (BlahguaAPIObject.Current.CurrentUserDescription != null)
                    return BlahguaAPIObject.Current.CurrentUserDescription.d;
                else
                    return null;
            }
        }

        public CommentList CommentHistory
        {
            get  {  return _commentHistory; }
            set { _commentHistory = value; }
        }

        public BlahList PostHistory
        {
            get { return _postHistory; }
            set { _postHistory = value; }
        }

        public BadgeList Badges
        {
            get {
				return _intBadgeList;
			}
        }

		public void AwaitBadgeData(bool_callback callback)
		{
			if (B == null)
			{
				callback(true);
			}
			else
			{
				_intBadgeList = new BadgeList();
				List<string> idList = new List<string>(B);
				FetchBadgeSerially(idList, callback);

			}
		}


		protected void FetchBadgeSerially(List<string> badgeIdList, bool_callback callback)
		{
			BadgeReference newBadge = new BadgeReference();
			newBadge.UpdateBadgeForId(badgeIdList[0], (didIt) =>
				{
					if (didIt)
					{
						if (_intBadgeList.Contains(newBadge))
						{
							System.Diagnostics.Debug.WriteLine("Duplicate call!");
							return;
						}

						_intBadgeList.Add(newBadge);
					}

					if (badgeIdList.Count > 0)
						badgeIdList.RemoveAt(0);

					if (badgeIdList.Count > 0)
						FetchBadgeSerially(badgeIdList, callback);
					else
						callback(true);
				}
			);
		}
    }



    public class UserDescription
    {
        public string K { get; set; }
        public string i { get; set; }
        public string d { get; set; }
        public string m { get; set; }
    }

    public class CommentAuthorDescription
    {
        public string K { get; set; }
        public string d { get; set; }
        public string i { get; set; }
        public List<string> _m { get; set; }
    }

    public class CommentAuthorDescriptionList : List<CommentAuthorDescription>
    {
    }
}
