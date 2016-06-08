using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlahguaMobile.BlahguaCore
{
	public class ChannelPermissions
	{
		public bool post {get; set;}
		public bool join { get; set; }
		public bool comment {get; set;}
		public bool admin {get; set;}
		public bool moderate { get; set; }
	}

    public class Channel
    {
        public int B { get; set; }
        public string D { get; set; }
        public int F { get; set; }
        public string G { get; set; }
        public int L { get; set; }
        public string M { get; set; }
        public string N { get; set; }
        public int R { get; set; }
        public string S { get; set; }
        public int U { get; set; }
        public int V { get; set; }
        public string X { get; set; }
        public long Y { get; set; }
        public long _id { get; set; }
		public string GHI { get; set; }
        public bool SAD { get; set; } // show author descriptions
        public bool SSA { get; set; } // show speech acts
        public bool SBL { get; set; } // show badge list?
        public List<long> PQ { get; set; }  // list of profile question IDs


        public Channel()
        {
            SSA = true;
            SAD = true;
            PQ = null;
            GHI = null;
        }

		public string HeaderImage {
			get { return GHI; }
		}

        public string ChannelName
        {
            get { return N; }
        }

        public long ChannelId
        {
            get { return _id; }
        }

        public long ChannelTypeId
        {
            get { return Y; }
        }

        public string ChannelTypeName
        {
            get
            {
                if (Y != null)
                    return BlahguaAPIObject.Current.CurrentChannelTypeList.ChannelTypeName(Y);
                else
                    return null;
            }
        }

        

    }

    public class ChannelList : List<Channel>
    {
        public string ChannelName(long channelId)
        {
            return this.Where(i => i._id == channelId).FirstOrDefault().N;
        }

        public Channel ChannelFromName(string channelName)
        {
            return this.Where(i => i.N == channelName).FirstOrDefault();
        }

    }

    public class ChannelType
    {
        public long _id { get; set; }
        public string N { get; set; }

    }

    public class ChannelTypeList : List<ChannelType>
    {
        public string ChannelTypeName(long channelId)
        {
            return this.Where(i => i._id == channelId).FirstOrDefault().N;
        }
    }

}
