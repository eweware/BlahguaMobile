using System;
using MonoTouch.Foundation;

namespace ServiceStackTextSampleiOS
{
	// Needed attribute to avoid linker remove code called dynamically
	// http://docs.xamarin.com/guides/ios/advanced_topics/linker/
	[Preserve (AllMembers = true)]
	public class Forecast
	{
		public string Code { get; set; }
		public string Date { get; set; }
		public string Day { get; set; }
		public string High { get; set; }
		public string Low { get; set; }
		public string Text { get; set; }
	}
}

