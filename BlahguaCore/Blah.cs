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


    public class PollItem
    {
        public string G {get; set;}
		public string F { get; set; }
		public long _id { get; set; }
		public long blahId { get; set; }
		public int count { get; set;}


        //[DataMember]
        //public string T {get; set;}

        private int _maxVotes = 0;
        private int _totalVotes = 0;
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
            count = numVotes;
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
                votes = 360.0 * ((double)count / (double)_maxVotes);
                return Math.Max(5, votes); 
            }
        }

        public string VotePercent
        {
            get 
            {
                int percent = 0;

                if (_totalVotes > 0)
                    percent = (int)(((double)count / (double)_totalVotes) * 100);

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
			get { return (int)count; }
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
        public int views { get; set; }
        public int opens { get; set; }
        public int comments { get; set; }
		public int upvotes { get; set;}
		public int downvotes { get; set;}
        public int userViews { get; set; }
        public int userOpens { get; set; }
        public int userComments { get; set; }
        public int userUpvotes { get; set; }
        public int userDownvotes { get; set; }
        public int userCreates { get; set; }

        public DateTime date { get; set;}


        public StatDayRecord()
        {
            views = opens = comments = 0;
        }

		public StatDayRecord(DateTime someDate)
		{
			views = opens = comments = 0;
            upvotes = downvotes = userUpvotes = userDownvotes = 0;
            userViews = userOpens = userComments = userCreates = 0;
			date = someDate;
		}
    }


	public class StatsList : ObservableCollection<StatDayRecord> { }


  

    public class UserPredictionVote
    {
        public string D {get; set;}
        public string Z { get; set; }
    }

	public class PredictionVoteStatsObj
	{
		public int yesCount { get; set; }
		public int unknownCount { get; set; }
		public int noCount { get; set; }

		public int expYesCount { get; set; }
		public int expNoCount { get; set; }
		public int expUnknownCount { get; set; }
	}

    public class Blah
    {
        public long A { get; set; }
		public string F { get; set; }
		public long G { get; set; }
		public int O { get; set; }
		public double S { get; set; }
		public string T { get; set; }
		public int V { get; set; }
		public long Y { get; set; }
		public long _id { get; set; }
		public DateTime cdate { get; set; }
		public PredictionVoteStatsObj PS { get; set;}
		public List<BadgeRecord> B { get; set; }
		public List<MediaRecordObject> M { get; set; }
		public PollItemList I { get; set; }
		public DateTime E { get; set; }
		public bool XX { get; set; }
		public bool XXX { get; set; }
		public int uv { get; set; }
		public int P { get; set; }
		public int D { get; set; }
		public int C { get; set; }


		// private
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
			int totalExpVotes = PS.expNoCount + PS.expYesCount + PS.expUnknownCount;
            int maxExpVote = Math.Max(Math.Max(PS.expNoCount, PS.expYesCount), PS.expUnknownCount);
			int totalVotes = PS.noCount + PS.yesCount + PS.unknownCount;
            int maxVote = Math.Max(Math.Max(PS.noCount, PS.yesCount), PS.unknownCount);
            _predictionItems = new PollItemList();
            _predictionItems.Add(new PollItem("I agree", PS.yesCount, maxVote, totalVotes, false, "y"));
            _predictionItems.Add(new PollItem("I disagree", PS.noCount, maxVote, totalVotes, false, "n"));
            _predictionItems.Add(new PollItem("It is unclear", PS.unknownCount, maxVote, totalVotes, false, "u"));

            _expPredictionItems = new PollItemList();
            _expPredictionItems.Add(new PollItem("The prediction was right", PS.expYesCount, maxExpVote, totalExpVotes, false, "y"));
            _expPredictionItems.Add(new PollItem("The prediction was wrong", PS.expNoCount, maxExpVote, totalExpVotes, false, "n"));
            _expPredictionItems.Add(new PollItem("It is unclear", PS.expUnknownCount, maxExpVote, totalExpVotes, false, "u"));


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

        public void UpdateUserPollVote(int userVote)
        {
            int maxVote = 0;
            int totalVotes = 0;


            foreach (PollItem curVote in I)
            {
                totalVotes += curVote.count;
                if (curVote.count > maxVote)
                    maxVote = curVote.count;
            }
            PollItem curPollItem = null;

            for (int i = 0; i < I.Count; i++)
            {
                curPollItem = I[i];
                curPollItem.MaxVotes = maxVote;
                curPollItem.IsUserVote = (userVote == i+1);
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
