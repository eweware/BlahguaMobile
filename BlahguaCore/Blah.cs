using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace BlahguaMobile.BlahguaCore
{
    


    public class InboxBlah
    {
        public long I { get; set; }
        public DateTime cdate { get; set; }
        public string T { get; set; }
        public long Y { get; set; }
        public long G { get; set; }
        public long A { get; set; }
        public List<MediaRecordObject> M { get; set; }
        public string B { get; set; }
        public double S { get; set; }
        public int displaySize { get; set; }

        public bool RR { get; set; }

        public bool XXX {get; set;}

        public InboxBlah()
        {
        }

        public InboxBlah(InboxBlah otherBlah)
        {
            I = otherBlah.I;
            cdate = otherBlah.cdate;
            T = otherBlah.T;
            Y = otherBlah.Y;
            G = otherBlah.G;
            A = otherBlah.A;
            M = otherBlah.M;
            B = otherBlah.B;
            S = otherBlah.S;
            RR = otherBlah.RR;
            XXX = otherBlah.XXX;
            displaySize = otherBlah.displaySize;
        }

        public InboxBlah(Blah otherBlah)
        {
			cdate = otherBlah.cdate;
            I = otherBlah._id;
            T = otherBlah.T;
            Y = otherBlah.Y;
            G = otherBlah.G;
            A = otherBlah.A;
            M = otherBlah.M;
            XXX = otherBlah.XXX;
            if ((otherBlah.B != null) && (otherBlah.B.Count > 0))
                B = "B";
            displaySize = 2;
        }
			

        public string ImageSize
        {
            get
            {
                switch (displaySize)
                {
                    case 1:
                        return "C";

                    case 2:
                        return "B";


                    case 3:
                        return "B";
     
     
                    default:
                        return "A";
                      
                }
            }
        }

        public string ImageURL
        {
            get
            {
                if ((M != null) && (M.Count > 0)) 
                {
                    string imageName = M[0].url;
					if (imageName != null)
						return BlahguaAPIObject.Current.GetImageURL (M [0].url, ImageSize);
					else
						return null;
                }
                else
                    return null;
            }

        }

        public string TypeName
        {
            get
            {
                string typeName = BlahguaAPIObject.Current.CurrentBlahTypes.GetTypeName(Y);
                return typeName;
            }
        }

    }

    public class Inbox : List<InboxBlah>
    {
        static private Random _random = new Random();

        public InboxBlah PopBlah(int blahSize)
        {
            foreach (InboxBlah curBlah in this)
            {
                if (curBlah.displaySize == blahSize)
                {
                    this.Remove(curBlah);
                    return curBlah;
                }
            }
            return null;

        }

        private void ComputeSizesOld()
        {
            int numLarge = 4;
            int numMedium = 16;

            this.Sort((obj1, obj2) =>
            {
                return obj1.S.CompareTo(obj2.S);
            });


            int i = 0;
            while (i < numLarge)
            {
                this[i++].displaySize = 1;
            }

            while (i < (numMedium + numLarge))
            {
                this[i++].displaySize = 2;
            }

            while (i < this.Count)
            {
                this[i++].displaySize = 3;
            }
        }

        private void ComputeSizes()
        {
            int numLarge = 4;
            int numMediumLarge = 8;
            int numMedium = 12;

            this.Sort((obj1, obj2) =>
            {
                return obj1.S.CompareTo(obj2.S);
            });


            int i = 0;
            while (i < numLarge)
            {
                this[i++].displaySize = 1;
            }

            while (i < (numMediumLarge + numLarge))
            {
                this[i++].displaySize = 2;
            }

            while (i < (numMediumLarge + numMedium + numLarge))
            {
                this[i++].displaySize = 3;
            }

            while (i < this.Count)
            {
                this[i++].displaySize = 4;
            }
        }

        private void Shuffle()
        {
            var random = _random;
            for (int i = this.Count; i > 1; i--)
            {
                // Pick random element to swap.
                int j = random.Next(i); // 0 <= j <= i-1
                // Swap.
                InboxBlah tmp = this[j];
                this[j] = this[i - 1];
                this[i - 1] = tmp;
            }

        }

        private void EnsureInboxSize()
        {
            int curIndex = 0;

            
            while (this.Count < 100)
            {
                this.Add(new InboxBlah(this[curIndex++]));
            }

            while (this.Count > 100)
            {
                this.RemoveAt(this.Count - 1);
            }
        }

        public void PrepareBlahs()
        {
            if (this.Count > 0)
            {
                EnsureInboxSize();
                ComputeSizes();
                Shuffle();
            }

        }


    }


    public class BlahType
    {
        public long _id { get; set; }
        public string N { get; set; }
    }


    public class BlahTypeList : List<BlahType> 
    {
        public string GetTypeName(long typeId)
        {
			if (typeId != null)
				return this.First (i => i._id == typeId).N;
			else
				return null;
        }

        public long GetTypeId(string typeName)
        {
            if (typeName != null)
                return this.First (i => i.N == typeName)._id;
            else
                return 0;
        }
    }

    [DataContract]
    public class PollItem
    {
        [DataMember]
        public string G {get; set;}
		public string F { get; set; }
		public long _id { get; set; }
		public long blahId { get; set; }

        //[DataMember]
        //public string T {get; set;}

        private int _maxVotes = 0;
        private int _totalVotes = 0;
        private int _itemVotes = 0;
        private bool _isUserVote = false;
        private string expVote = "";


        public PollItem(string theText)
        {
            G = theText;
        }

        public PollItem(string theText, int numVotes, int maxVotes, int totalVotes, bool isUserVote, string eVote = "")
        {
            G = theText;
            _maxVotes = maxVotes;
            _itemVotes = numVotes;
            _isUserVote = isUserVote;
            _totalVotes = totalVotes;
            expVote = eVote;
        }

        public int MaxVotes
        {
            get { return _maxVotes; }
            set { _maxVotes = value; }
        }

        

        public string PredictVoteStr
        {
            get { return expVote; }
        }

        public int TotalVotes
        {
            get { return _totalVotes; }
            set { _totalVotes = value; }
        }

        public double ComputedWidth
        {
            get 
            { 
                double votes = 0;
                if (_maxVotes > 0)
                votes = 360.0 * ((double)_itemVotes / (double)_maxVotes);
                return Math.Max(5, votes); 
            }
        }

        public string VotePercent
        {
            get 
            {
                int percent = 0;

                if (_totalVotes > 0)
                    percent = (int)(((double)_itemVotes / (double)_totalVotes) * 100);

                if (percent > 0)
                    return percent.ToString() + "%";
                else
                    return "no votes"; // no votes
            }
        }

        public string VoteString
        {
            get
            {
                if (_isUserVote)
                    return "\uf046";
                else
                    return "\uf096";
            }
        }


        public int Votes
        {
            get { return _itemVotes; }
            set { _itemVotes = value; }
        }

        public bool IsUserVote
        {
            get { return _isUserVote; }
            set { _isUserVote = value; }
        }
    }

    public class PollItemList : ObservableCollection<PollItem>
    {
    }

    public class BlahCreateRecord
    {
        public List<BadgeRecord> B { get; set; }
        public string F { get; set; }
		public DateTime E { get; set; } // expiration date
        public long G { get; set; } // group ID
        public List<MediaRecordObject> M { get; set; } // image IDs
        public int H { get; set; } // poll option count
        public PollItemList I { get; set; } // poll text
        public string T { get; set; } // blah text
        public long Y { get; set; } // type ID
        public bool XX { get; set; } // wehter or not the blah is private
        public bool XXX { get; set;  } // whether or not the blah is mature
       

        public BlahCreateRecord()
        {
            XX = true;
            XXX = false;
            Y = BlahguaAPIObject.Current.CurrentBlahTypes.First<BlahType>(n => n.N == "says")._id;
            G = BlahguaAPIObject.Current.CurrentChannelList.First(c => c.N == BlahguaAPIObject.Current.GetDefaultChannel().N)._id;
            E = DateTime.Now + new TimeSpan(30, 0, 0, 0);

            I = new PollItemList();
            I.Add(new PollItem("first choice"));
            I.Add(new PollItem("second choice"));
        }


        public bool UseProfile
        {
            get { return !XX; }
            set
            {
                XX = (!value);
            }
        }

        public bool IsMature
        {
            get { return XXX; }
            set
            {
                XXX = value;
                
            }
        }

      
        public BlahType BlahType
        {
            get 
            {
                return BlahguaAPIObject.Current.CurrentBlahTypes.First<BlahType>(n => n._id == Y);
            }
            set
            {
                Y = value._id;
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
                    return BlahguaAPIObject.Current.CurrentUser.UserName;
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

    public class StatDayRecord
    {
        public int C { get; set; }
        public int D { get; set; }
        public int O { get; set; }
        public int P { get; set; }
        public int U { get; set; }
        public int V { get; set; }
        public int X { get; set; }
        public int XX { get; set; }
        public int T { get; set; }
        public int DT { get; set; }
        public string _id { get; set; }

        public StatDayRecord()
        {
            C = D = O = P = U = V = 0;
        }

        public StatDayRecord(DateTime theDate)
        {
            string timeStr = theDate.ToString("yyMMdd");
            _id = timeStr;
            C = D = O = P = U = V = 0;
        }

        public DateTime StatDate
        {
            get
            {
                string datePart = _id.Substring(_id.Length - 6);
                string statYear = datePart.Substring(0, 2);
                string statMonth = datePart.Substring(2, 2);
                string statDay = datePart.Substring(4, 2);
                return new DateTime(2000 + int.Parse(statYear), int.Parse(statMonth), int.Parse(statDay));
            }
        }
    }


    public class UserStatsList : ObservableCollection<StatDayRecord>
    {
        List<int> _openList = null;
        List<int> _commentList = null;
        List<int> _viewList = null;
        List<int> _userOpenList = null;
        List<int> _userCommentList = null;
        List<int> _userViewList = null;
        List<int> _userCreateList = null;

        bool? _hasComments;
        bool? _hasViews;
        bool? _hasOpens;
        bool? _hasUserOpens;
        bool? _hasUserViews;
        bool? _hasUserCreates;
        bool? _hasUserComments;

        public List<int> Opens
        {
            get
            {
                if (_openList == null)
                {
                    bool hasVotes = false;
                    int curVote;
                    _openList = new List<int>();
                    foreach (StatDayRecord dayRec in this)
                    {
                        curVote = dayRec.O;
                        if (curVote > 0)
                            hasVotes = true;
                        _openList.Add(curVote);
                    }

                    _hasOpens = hasVotes;
                }


                return _openList;
            }
        }

        public List<int> Impressions
        {
            get
            {
                if (_viewList == null)
                {
                    bool hasVotes = false;
                    int curVote;
                    _viewList = new List<int>();
                    foreach (StatDayRecord dayRec in this)
                    {
                        curVote = dayRec.V;
                        if (curVote > 0)
                            hasVotes = true;
                        _viewList.Add(curVote);
                    }

                    _hasViews = hasVotes;
                }


                return _viewList;
            }
        }

        public List<int> Comments
        {
            get
            {
                if (_commentList == null)
                {
                    bool hasVotes = false;
                    int curVote;
                    _commentList = new List<int>();
                    foreach (StatDayRecord dayRec in this)
                    {
                        curVote = dayRec.O;
                        if (curVote > 0)
                            hasVotes = true;
                        _commentList.Add(curVote);
                    }

                    _hasComments = hasVotes;
                }


                return _commentList;
            }
        }

        public List<int> UserComments
        {
            get
            {
                if (_userCommentList == null)
                {
                    bool hasVotes = false;
                    int curVote;
                    _userCommentList = new List<int>();
                    foreach (StatDayRecord dayRec in this)
                    {
                        curVote = dayRec.XX;
                        if (curVote > 0)
                            hasVotes = true;
                        _userCommentList.Add(curVote);
                    }

                    _hasUserComments = hasVotes;
                }


                return _userCommentList;
            }
        }

        public List<int> UserOpens
        {
            get
            {
                if (_userOpenList == null)
                {
                    bool hasVotes = false;
                    int curVote;
                    _userOpenList = new List<int>();
                    foreach (StatDayRecord dayRec in this)
                    {
                        curVote = dayRec.O;
                        if (curVote > 0)
                            hasVotes = true;
                        _userOpenList.Add(curVote);
                    }

                    _hasUserOpens = hasVotes;
                }


                return _userOpenList;
            }
        }

        public List<int> UserViews
        {
            get
            {
                if (_userViewList == null)
                {
                    bool hasVotes = false;
                    int curVote;
                    _userViewList = new List<int>();
                    foreach (StatDayRecord dayRec in this)
                    {
                        curVote = dayRec.V;
                        if (curVote > 0)
                            hasVotes = true;
                        _userViewList.Add(curVote);
                    }

                    _hasUserViews = hasVotes;
                }


                return _userViewList;
            }
        }

        public List<int> UserCreates
        {
            get
            {
                if (_userCreateList == null)
                {
                    bool hasVotes = false;
                    int curVote;
                    _userCreateList = new List<int>();
                    foreach (StatDayRecord dayRec in this)
                    {
                        curVote = dayRec.X;
                        if (curVote > 0)
                            hasVotes = true;
                        _userCreateList.Add(curVote);
                    }

                    _hasUserCreates = hasVotes;
                }


                return _userCreateList;
            }
        }

        public bool HasOpens
        {
            get
            {
                if (_hasOpens == null)
                {
                    List<int> theOpens = Opens;
                }
                return (bool)_hasOpens;
            }
        }

        public bool HasViews
        {
            get
            {
                if (_hasViews == null)
                {
                    List<int> views = Impressions;
                }
                return (bool)_hasViews;
            }
        }

        public bool HasComments
        {
            get
            {
                if (_hasComments == null)
                {
                    List<int> theComments = Comments;
                }
                return (bool)_hasComments;
            }
        }

        public bool HasUserComments
        {
            get
            {
                if (_hasUserComments == null)
                {
                    List<int> theComments = UserComments;
                }
                return (bool)_hasUserComments;
            }
        }

        public bool HasUserOpens
        {
            get
            {
                if (_hasUserOpens == null)
                {
                    List<int> theList = UserOpens;
                }
                return (bool)_hasUserOpens;
            }
        }

        public bool HasUserCreates
        {
            get
            {
                if (_hasUserCreates == null)
                {
                    List<int> theList = UserCreates;
                }
                return (bool)_hasUserCreates;
            }
        }

        public bool HasUserViews
        {
            get
            {
                if (_hasUserViews == null)
                {
                    List<int> theList = UserViews;
                }
                return (bool)_hasUserViews;
            }
        }


    }

    public class Stats : ObservableCollection<StatDayRecord>
    {

        List<int> _openList = null;
        List<int> _commentList = null;
        List<int> _viewList = null;

        bool? _hasComments;
        bool? _hasViews;
        bool? _hasOpens;

        public List<int> Opens
        {
            get
            {
                if (_openList == null)
                {
                    bool hasVotes = false;
                    int curVote;
                    _openList = new List<int>();
                    foreach (StatDayRecord dayRec in this)
                    {
                        curVote = dayRec.O;
                        if (curVote > 0)
                            hasVotes = true;
                        _openList.Add(curVote);
                    }

                    _hasOpens = hasVotes;
                }


                return _openList;
            }
        }

        public List<int> Impressions
        {
            get
            {
                if (_viewList == null)
                {
                    bool hasVotes = false;
                    int curVote;
                    _viewList = new List<int>();
                    foreach (StatDayRecord dayRec in this)
                    {
                        curVote = dayRec.V;
                        if (curVote > 0)
                            hasVotes = true;
                        _viewList.Add(curVote);
                    }

                    _hasViews = hasVotes;
                }


                return _viewList;
            }
        }

        public List<int> Comments
        {
            get
            {
                if (_commentList == null)
                {
                    bool hasVotes = false;
                    int curVote;
                    _commentList = new List<int>();
                    foreach (StatDayRecord dayRec in this)
                    {
                        curVote = dayRec.O;
                        if (curVote > 0)
                            hasVotes = true;
                        _commentList.Add(curVote);
                    }

                    _hasComments = hasVotes;
                }


                return _commentList;
            }
        }

        public bool HasOpens
        {
            get 
            {
                if (_hasOpens == null)
                {
                    List<int> theOpens = Opens;
                }
                return (bool)_hasOpens;
            }
        }

        public bool HasViews
        {
            get
            {
                if (_hasViews == null)
                {
                    List<int> views = Impressions;
                }
                return (bool)_hasViews;
            }
        }

        public bool HasComments
        {
            get
            {
                if (_hasComments == null)
                {
                    List<int> theComments = Comments;
                }
                return (bool)_hasComments;
            }
        }
    }

  

    public class UserPollVote
    {
        public int W {get; set;}

        public UserPollVote()
        {
            W = -1;
        }
    }

    public class UserPredictionVote
    {
        public string D {get; set;}
        public string Z {get; set;}
    }

    [DataContract]
    public class Blah
    {
        [DataMember]
        public long A { get; set; }

        [DataMember]        
        public string F { get; set; }

        [DataMember]
        public long G { get; set; }

        [DataMember]
        public int O { get; set; }

        [DataMember]
        public double S { get; set; }

        [DataMember]
        public string T { get; set; }

        [DataMember]
        public int V { get; set; }

        [DataMember]
        public long Y { get; set; }

        [DataMember]
        public long _id { get; set; }

        [DataMember]
		public DateTime cdate { get; set; }

        [DataMember]
        public List<BadgeRecord> B { get; set; }

        [DataMember]
        public List<MediaRecordObject> M { get; set; }

        [DataMember]
        public PollItemList I { get; set; }

        [DataMember]
        public List<int> J { get; set; }

		[DataMember]
		public DateTime E { get; set; }


        [DataMember(Name = "1")]
        public int _1 { get; set; }

        [DataMember(Name = "2")]
        public int _2 { get; set; }

        [DataMember(Name = "3")]
        public int _3 { get; set; }

        [DataMember(Name = "4")]
        public int _4 { get; set; }

        [DataMember(Name = "5")]
        public int _5 { get; set; }

        [DataMember(Name = "6")]
        public int _6 { get; set; }

        [DataMember]
        public bool XX { get; set; }

         [DataMember]
        public bool XXX { get; set; }

        [DataMember]
        public Stats L { get; set; }

        [DataMember]
        public int uv { get; set; }

        [DataMember]
        public int P { get; set; }

        [DataMember]
        public int D { get; set; }

        [DataMember]
        public int C { get; set; }


        public bool IsPollInited = false;
        public bool IsPredictInited = false;
        public UserDescription Description {get; set;}
        public CommentList Comments { get; set; }
        private PollItemList _predictionItems;
        private PollItemList _expPredictionItems;
        private BadgeList _badgeList = null;



        public Blah()
        {
            uv = 0;
            O = 0;
            V = 0;
            C = 0;
            D = 0;
            P = 0;
            XXX = false;
            _1 = 0;
            _2 = 0;
            _3 = 0;
            _4 = 0;
            _5 = 0;
            _6 = 0;
            L = null;
        }

        // 

        public string URL
        {
            get
            {
                return "http://app.goheard.com/?blahId=" + _id;

            }
        }

        public bool IsPredictionExpired
        {
            get
            {
				return E < DateTime.Now;
            }
        }





        public string ConversionString
        {
            get
            {
                double theRate = 0;

                if (V > 0)
                    theRate = (double)O / (double)V;

                if (theRate > 1)
                    theRate = 1;
                theRate *= 100;
                if (theRate > 1)
                {
                    int newRate = (int)Math.Round(theRate);
                    if (newRate > 100)
                        newRate = 100;
                    return newRate.ToString() + "%";
                }
                else
                    return "< 1%";
                    
            }
        }

        public string StrengthString
        {
            get
            {
                int theStr = (int)(S * 100);
                if (theStr < 0)
                    theStr = 0;
                else if (theStr > 100)
                    theStr = 100;

                return theStr.ToString();

            }
        }

#if WP8
        public System.Windows.Media.Brush TypeBrush
        {
            get
            {
                System.Windows.Media.Brush newBrush = null;

                switch (this.TypeName)
                {
                    case "leaks":
                        newBrush = (System.Windows.Media.Brush)BlahguaMobile.Winphone.App.Current.Resources["HighlightBrushLeaks"];
                        break;
                    case "polls":
                        newBrush = (System.Windows.Media.Brush)BlahguaMobile.Winphone.App.Current.Resources["HighlightBrushPolls"];
                        break;
                    case "asks":
                        newBrush = (System.Windows.Media.Brush)BlahguaMobile.Winphone.App.Current.Resources["HighlightBrushAsks"];
                        break;
                    case "predicts":
                        newBrush = (System.Windows.Media.Brush)BlahguaMobile.Winphone.App.Current.Resources["HighlightBrushPredicts"];
                        break;
                    default:
                        newBrush = (System.Windows.Media.Brush)BlahguaMobile.Winphone.App.Current.Resources["HighlightBrushSays"];
                        break;
                }

                return newBrush;
            }
        }
#endif

        public string ImpressionString
        {
            get
            {
                string theStr = "opened " + O + " time";
                if (O != 1)
                    theStr += "s";
                theStr += " out of " + V + " impression";
                if (V != 1)
                    theStr += "s";
                return theStr;
            }
        }

        public PollItemList PredictionItems
        {
            get
            {
                return _predictionItems;
            }
        }

        public PollItemList ExpPredictionItems
        {
            get
            {
                return _expPredictionItems;
            }
        }

        public void UpdateUserPredictionVote(UserPredictionVote theVote)
        {
            int totalExpVotes = _1 + _2 + _3;
            int maxExpVote = Math.Max(Math.Max(_1, _2), _3);
            int totalVotes = _4 + _5 + _6;
            int maxVote = Math.Max(Math.Max(_4, _5), _6);
            _predictionItems = new PollItemList();
            _predictionItems.Add(new PollItem("I agree", _4, maxVote, totalVotes, false, "y"));
            _predictionItems.Add(new PollItem("I disagree", _5, maxVote, totalVotes, false, "n"));
            _predictionItems.Add(new PollItem("It is unclear", _6, maxVote, totalVotes, false, "u"));

            _expPredictionItems = new PollItemList();
            _expPredictionItems.Add(new PollItem("The prediction was right", _1, maxExpVote, totalExpVotes, false, "y"));
            _expPredictionItems.Add(new PollItem("The prediction was wrong", _2, maxExpVote, totalExpVotes, false, "n"));
            _expPredictionItems.Add(new PollItem("It is unclear", _3, maxExpVote, totalExpVotes, false, "u"));


            if (theVote == null)
            {
                // user is not signed in
            }
            else
            {
                switch (theVote.D)
                {
                    case "y":
                        _predictionItems[0].IsUserVote = true;
                        break;
                    case "n":
                        _predictionItems[1].IsUserVote = true;
                        break;
                    case "u":
                        _predictionItems[2].IsUserVote = true;
                        break;
                }

                switch (theVote.Z)
                {
                    case "y":
                        _expPredictionItems[0].IsUserVote = true;
                        break;
                    case "n":
                        _expPredictionItems[1].IsUserVote = true;
                        break;
                    case "u":
                        _expPredictionItems[2].IsUserVote = true;
                        break;
                }
            }

            IsPredictInited = true;
        }

        public void UpdateUserPollVote(UserPollVote theVote)
        {
            int maxVote = 0;
            int userVote = -1;
            int totalVotes = 0;

            if (theVote != null)
                userVote = theVote.W;

            foreach (int curVote in J)
            {
                totalVotes += curVote;
                if (curVote > maxVote)
                    maxVote = curVote;
            }
            PollItem curPollItem = null;

            for (int i = 0; i < I.Count; i++)
            {
                curPollItem = I[i];
                curPollItem.MaxVotes = maxVote;
                curPollItem.Votes = J[i];
                curPollItem.IsUserVote = (userVote == i);
                curPollItem.TotalVotes = totalVotes;
            }

            IsPollInited = true;
        }
  

        public string ElapsedTimeString
        {
            get
            {
				return Utilities.ElapsedDateString(cdate);
            }
        }

        
  
        public string UserName
        {
            get
            {
                if ((!XX) && (Description != null) && (Description.K != null))
                    return Description.K;
                else
                    return "Someone";
            }
        }

        public string DescriptionString
        {
            get
            {
                if ((!XX) && (Description != null) && (Description.d != null))
                    return Description.d;
                else
                    return "An unidentified person";
            }
        }

        public string UserImage
        {
            get
            {
				if ((!XX) && (Description != null) && (Description.m != null) && (Description.m.Count > 0) && !string.IsNullOrEmpty(Description.m[0].url))
                    return BlahguaAPIObject.Current.GetImageURL(Description.m[0].url, "A");
                else
                    return "https://s3-us-west-2.amazonaws.com/app.goheard.com/images/unknown-user.png";
            }
        }

       

        public string ImageURL
        {
            get
            {
                if ((M != null) && (M.Count > 0)) 
                {
                    string imageName = M[0].url;
					if (!string.IsNullOrEmpty (imageName))
						return BlahguaAPIObject.Current.GetImageURL (imageName, "D");
					else
						return null;
                }
                else
                    return null;
            }

        }

        



        public string TypeName
        {
            get
            {
                string typeName = "unknown";
                try
                {
                    typeName = BlahguaAPIObject.Current.CurrentBlahTypes.GetTypeName(Y);
                }
                catch (Exception exp)
                {
                    // to do - we currently remove ads.
                }
                return typeName;
            }
        }

        public string ChannelName
        {
            get
            {
                string channelName = BlahguaAPIObject.Current.CurrentChannelList.ChannelName(G);
                return channelName;
            }
        }

        
    }

    public class BlahList : ObservableCollection<Blah>
    {

    }


}
