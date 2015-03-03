using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace BlahguaMobile.BlahguaCore
{

    public class CommentCreateRecord : INotifyPropertyChanged
    {
        public string B { get; set; }
        public string T { get; set; }
        public List<string> BD { get; set; }
        public bool XX { get; set; }
        public List<string> M { get; set; }
        public string CID { get; set; }
        public bool XXX { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public CommentCreateRecord()
        {
            XX = true;
            B = null;
            BD = null;
            M = null;
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

        public string Text
        {
            get { return T; }
            set
            {
                T = value;
                OnPropertyChanged("Text");
            }
        }

        public bool UseProfile
        {
            get { return !XX; }
            set
            {
                XX = (!value);
                OnPropertyChanged("UseProfile");
            }
        }

        public bool IsMature
        {
            get { return XXX; }
            set
            {
                XXX = (value);
                OnPropertyChanged("IsMature");
            }
        }

        public string UserName
        {
            get
            {
                if (XX)
                {
                    return "Someone";
                }
                else
                {
                    return BlahguaAPIObject.Current.CurrentUser.N;
                }
            }
        }
        public string UserImage
        {
            get
            {
                if (XX)
                {
                    return "https://s3-us-west-2.amazonaws.com/app.goheard.com/images/unknown-user.png";
                }
                else
                {
                    return BlahguaAPIObject.Current.CurrentUser.UserImage;
                }
            }
        }

        public string DescriptionString
        {
            get
            {
                if (XX)
                {
                    return "An unidentified person";
                }
                else
                {
                    return BlahguaAPIObject.Current.CurrentUser.DescriptionString;
                }
            }
        }

        public BadgeList Badges
        {
            get
            {
                if (BD == null)
                {
                    return null;
                }
                else
                {
                    BadgeList badges = new BadgeList();
                    foreach (string badgeId in BD)
                    {
                        badges.Add(new BadgeReference(badgeId));
                    }

                    return badges;
                }
            }
            set
            {
                if ((value == null) || (value.Count == 0))
                {
                    BD = null;
                }
                else
                {
                    BD = new List<string>();
                    foreach (BadgeReference curBadge in value)
                    {
                        BD.Add(curBadge.ID);
                    }
                }
                OnPropertyChanged("Badges");
            }
        }

    }
		
    public class Comment : INotifyPropertyChanged
    {

        public string _id { get; set; }

        public string B { get; set; }

        public string T { get; set; }
   
        public string A { get; set; }

        public double S { get; set; }
		public string c { get; set; }
		private DateTime _createDate = DateTime.MinValue;
		public string u { get; set; }
		private DateTime _updateDate = DateTime.MinValue;
 
        public List<string> BD { get; set; }
 
        public string CID { get; set; }

        public bool XX { get; set; }

        public bool XXX { get; set; }

        public int Upvotes { get; set; }

        public int Downvotes { get; set; }

        public DemographicRecord _d { get; set; }


        public List<string> M { get; set; }

        public CommentList subComments = null;

        public string K { get; set; }

        public string d { get; set; }

        public List<string> _m { get; set; }

        public int uv { get; set; }
        private int _indentLevel;
		private BadgeList _badgeList = null;

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
                catch (Exception)
                {
                    // do nothing for now...
                }
            }
        }

        public bool HasComments
        {
            get { return ((subComments != null) && (subComments.Count > 0)); }
        }

        public bool IsMature
        {
            get { return XXX; }
            set
            {
                XXX = (value);
                OnPropertyChanged("IsMature");
            }
        }


        public Comment()
        {
            Upvotes = 0;
            Downvotes = 0;
            uv = 0;
            _indentLevel = 0;
            XXX = false;
        }

		public DateTime CreationDate {
			get {
				if (_createDate == DateTime.MinValue)
					_createDate = DateTime.Parse (c);
				return _createDate;
			}
		}

		public DateTime UpdateDate {
			get {
				if (_updateDate == DateTime.MinValue)
					_updateDate = DateTime.Parse (u);
				return _updateDate;
			}
		}


        public double IndentWidth
        {
            get { return _indentLevel * 8; }
        }

        public int IndentLevel
        {
            get { return _indentLevel; }
            set 
            {  
                _indentLevel = value;
                OnPropertyChanged("IndentLevel");
                OnPropertyChanged("IndentWidth");
            }
        }



        public string AuthorName
        {
            get
            {
                if ((XX == false) && (K != null))
                    return K;
                else
                    return "Someone";
            }
        }

        public string AuthorImage
        {
            get
            {
                if ((XX == false) && (_m != null))
                {
                    string imageName = _m[0];
                    return BlahguaAPIObject.Current.GetImageURL(_m[0], "A");
                }
                else
					return "https://s3-us-west-2.amazonaws.com/app.goheard.com/images/unknown-user.png";
            }
        }

        public int UpVoteCount
        {
            get {return Upvotes;}
            set
            {
                Upvotes = value;
                OnPropertyChanged("UpVoteCount");
            }
           
        }

        public int DownVoteCount
        {
            get { return Downvotes; }
            set
            {
                Downvotes = value;
                OnPropertyChanged("DownVoteCount");
            }
        }

        public int UserVote
        {
            get { return uv; }
            set
            {
                uv = value;
                OnPropertyChanged("UserVote");
                if (uv == 1)
                {
                    UpVoteCount++;
                }
                else
                {
                    DownVoteCount++;
                }
            }
        }

        public string DescriptionString
        {
            get
            {
                if ((XX == false) && (d != null))
                    return d;
                else
                    return "An unidentified person";
            }
        }

        public string ImageURL
        {
            get
            {
                if ((M != null) && (M.Count > 0)) 
                {
                    string imageName = M[0];
                    return BlahguaAPIObject.Current.GetImageURL(M[0], "D");
                }
                else
                    return null;
            }
        }

        public string ElapsedTimeString
        {
            get
            {
                return Utilities.ElapsedDateString(CreationDate);//c);
            }
        }

        public BadgeList Badges
        {
			get
			{
				return _badgeList;
			}
			set
			{
				if ((value == null) || (value.Count == 0))
				{
					BD = null;
				}
				else
				{
					BD = new List<string>();
					foreach (BadgeReference curBadge in value)
					{
						BD.Add(curBadge.ID);
					}
				}
			}
        }

		public void AwaitBadgeData(bool_callback callback)
		{
			if (BD == null)
			{
				callback(true);
			}
			else
			{
				_badgeList = new BadgeList();
				List<string> idList = new List<string>(BD);
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
						if (_badgeList.Contains(newBadge))
						{
							System.Diagnostics.Debug.WriteLine("Duplicate call!");
							return;
						}

						_badgeList.Add(newBadge);
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





    public class CommentList : ObservableCollection<Comment>
    {

    }
}
