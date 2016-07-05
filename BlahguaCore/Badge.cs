using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace BlahguaMobile.BlahguaCore
{

    public class BadgeRecord
    {
        public string A { get; set; } // authority ID - aka endpoint
        public string D { get; set; } // authority display name
        public string I { get; set; } // authorty badge id
        public string N { get; set; } // badge display name
        public long U { get; set; } // user id
        public DateTime X { get; set; } // expiration date (jscript ticks)
        public string Y { get; set; }  // badge type
		public string V {get; set; } // badge value, if any
		public string URL {get; set;} // badge URL, if any
        public long _id { get; set; } // badge ID

        public BadgeRecord()
        {
        }



    }
		

	public class BadgeList : ObservableCollection<BadgeRecord>
    {
        
    }
    public class BadgeAuthority
    {
        public long _id { get; set; }
		public string c { get; set; }
        public string N { get; set; } // the name
        public string T { get; set; } // types of badges
        public string D { get; set; }  // description
        public string E { get; set; }  // home page
        public string R { get; set; }  // access point
    }

    public class BadgeAuthorityList : List<BadgeAuthority>
    {
        
    }
}
