﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlahguaMobile.BlahguaCore
{
	public class PresenceMessage
	{
		public string action { get; set; }
		public int timestamp { get; set; }
		public string uuid { get; set; }
		public int occupancy { get; set; }
	}

	public class PublishAction
	{
		public string action { get; set; }
		public long blahid { get; set; }
		public long commentid { get; set; }
		public long userid {get; set;}
        public string data { get; set; }
	}
}
