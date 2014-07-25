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

        public static  SizeF TinyCellSize   = new SizeF(106.0f, 106.0f);
        public static  SizeF SmallCellSize  = new SizeF(213.0f, 106.0f);
        public static  SizeF MediumCellSize = new SizeF(213.0f, 213.0f);
        public static  SizeF LargeCellSize  = new SizeF(320.0f, 212.0f);
        public static float BlahGutter = 8.0f;
		public static float BlahSpacing = 4.0f;
	}
}

