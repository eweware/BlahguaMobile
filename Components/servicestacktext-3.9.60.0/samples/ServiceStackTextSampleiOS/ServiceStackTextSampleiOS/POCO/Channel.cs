using System;
using MonoTouch.Foundation;

namespace ServiceStackTextSampleiOS
{
	// Needed attribute to avoid linker remove code called dynamically
	// http://docs.xamarin.com/guides/ios/advanced_topics/linker/
	[Preserve (AllMembers = true)]
	public class Channel
	{
		public Item Item { get; set; }
	}
}

