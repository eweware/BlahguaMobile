using System;
using System.Drawing;

namespace BlahguaMobile.IOS
{
	public static class BGBlahCellSizesConstants
	{
        public const  string TinyReusableId   = "TinyCell";
        public const  string SmallReusableId  = "SmallCell";
        public const  string MediumReusableId = "MediumCell";
        public const  string LargeReusableId  = "LargeCell";

        public static readonly  SizeF TinyCellSize   = new SizeF(106.0f, 106.0f);
        public static readonly  SizeF SmallCellSize  = new SizeF(213.0f, 106.0f);
        public static readonly  SizeF MediumCellSize = new SizeF(213.0f, 213.0f);
        public static readonly  SizeF LargeCellSize  = new SizeF(320.0f, 212.0f);

		public static readonly float Spacing = 1.0f;
	}
}

