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
        public long B { get; set; }
        public string T { get; set; }
        public List<BadgeRecord> BD { get; set; }
        public bool XX { get; set; }
        public List<MediaRecordObject> M { get; set; }
        public long CID { get; set; }
        public bool XXX { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public CommentCreateRecord()
        {
            XX = true;
            B = 0;
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

        

    }
		
    public class Comment : INotifyPropertyChanged
    {

        public long _id { get; set; }

        public long B { get; set; }

        public string T { get; set; }
   
        public long A { get; set; }

        public double S { get; set; }
		public DateTime c { get; set; }
 
        public List<BadgeRecord> BD { get; set; }
 
        public long CID { get; set; }

        public bool XX { get; set; }

        public bool XXX { get; set; }

        public int Upvotes { get; set; }

        public int Downvotes { get; set; }

        public List<MediaRecordObject> M { get; set; }

        public CommentList subComments = null;

        public string K { get; set; }

        public string d { get; set; }

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
				if ((XX == false) && (M != null) && (M.Count > 0))
                {
					string urlStr = M [0].url;
					if (!string.IsNullOrEmpty (urlStr))
						return BlahguaAPIObject.Current.GetImageURL (urlStr, "A");
					else
						return null;
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
					string urlStr = M [0].url;
					if (!string.IsNullOrEmpty (urlStr))
						return BlahguaAPIObject.Current.GetImageURL (M [0].url, "D");
					else
						return null;
                }
                else
                    return null;
            }
        }

        public string ElapsedTimeString
        {
            get
            {
                return Utilities.ElapsedDateString(c);//c);
            }
        }

       
	}





    public class CommentList : ObservableCollection<Comment>
    {

    }
}
