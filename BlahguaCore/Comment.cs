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
                    return "/Images/unknown-user.png";
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
                    return "An anonymous person";
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

    [DataContract]
    public class Comment : INotifyPropertyChanged
    {
        [DataMember]
        public string _id { get; set; }
        [DataMember]
        public string B { get; set; }
        [DataMember]
        public string T { get; set; }
        [DataMember]
        public string A { get; set; }
        [DataMember]
        public double S { get; set; }
        public DateTime c { get; set; }
        [DataMember]
        public List<string> BD { get; set; }
        [DataMember]
        public string CID { get; set; }
        [DataMember]
        public bool XX { get; set; }
        [DataMember]
        public int U { get; set; }
        [DataMember]
        public int D { get; set; }
        [DataMember]
        public DemographicRecord _d { get; set; }
        public DateTime u { get; set; }
        [DataMember]
        public List<string> M { get; set; }
        [DataMember]
        public CommentList subComments = null;
        [DataMember]
        public string K { get; set; }
        [DataMember]
        public string d { get; set; }
        [DataMember]
        public List<string> _m { get; set; }
        [DataMember]
        public int uv { get; set; }
        private int _indentLevel;

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
                    // do nothing for now...
                }
            }
        }

        public bool HasComments
        {
            get { return ((subComments != null) && (subComments.Count > 0)); }
        }

        public Comment()
        {
            U = 0;
            D = 0;
            uv = 0;
            _indentLevel = 0;
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
                    return "/Images/unknown-user.png";
            }
        }

        public int UpVoteCount
        {
            get {return U;}
            set
            {
                U = value;
                OnPropertyChanged("UpVoteCount");
            }
           
        }

        public int DownVoteCount
        {
            get { return D; }
            set
            {
                D = value;
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
                    return "An anonymous user";
            }
        }

        public string ImageURL
        {
            get
            {
                if (M != null)
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
                return Utilities.ElapsedDateString(new DateTime());//c);
            }
        }

    }



    public class CommentList : ObservableCollection<Comment>
    {

    }
}
