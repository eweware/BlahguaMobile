using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Reflection;

namespace BlahguaMobile.BlahguaCore
{
	public class ProfileSchemaValueObj {
		public long _id  { get; set; }
		public string schemaId  { get; set; }
		public string name  { get; set; }
	}



	public class ProfileSchema {
		public long _id { get; set; }
		public int type { get; set; }    // the type of this schema.  1 = free text, 2 = integer with range, 3 = integer no range, 4 = enum
		public string name { get; set; }     // the name of this schema
		public List<ProfileSchemaValueObj> values { get; set; } // a list of possible values
		public string defaultVal { get; set; }   // default value for this
		public bool isPublic { get; set; }  // whether this is part of the public schema for all users
		public string currentValue { get; set; } // when we have actual values
	}
}
