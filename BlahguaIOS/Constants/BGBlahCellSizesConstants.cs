using System;
using System.Drawing;

namespace BlahguaMobile.IOS
{
	public static class BGBlahCellSizesConstants
	{
		public static readonly string TinyReusableId   = "TinyCell";
		public static readonly string SmallReusableId  = "SmallCell";
		public static readonly string MediumReusableId = "MediumCell";
		public static readonly string LargeReusableId  = "LargeCell";

		public static readonly SizeF TinyCellSize   = new SizeF(106.0f, 106.0f);
		public static readonly SizeF SmallCellSize  = new SizeF(213.0f, 106.0f);
		public static readonly SizeF MediumCellSize = new SizeF(213.0f, 213.0f);
		public static readonly SizeF LargeCellSize  = new SizeF(320.0f, 212.0f);

		public static readonly float Spacing = 1.0f;
	}
}

