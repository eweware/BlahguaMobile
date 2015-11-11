using System;
using CoreGraphics;

namespace BlahguaMobile.IOS
{
	public static class BGBlahCellSizesConstants
	{
        public const  string TinyReusableId   = "TinyCell";
        public const  string SmallReusableId  = "SmallCell";
        public const  string MediumReusableId = "MediumCell";
        public const  string LargeReusableId  = "LargeCell";
        public const  string AdReusableId  = "AdCell";

        public static  CGSize TinyCellSize   = new CGSize(106.0f, 106.0f);
        public static  CGSize SmallCellSize  = new CGSize(213.0f, 106.0f);
        public static  CGSize MediumCellSize = new CGSize(213.0f, 213.0f);
        public static  CGSize LargeCellSize  = new CGSize(320.0f, 212.0f);
        public static  CGSize AdCellSize  = new CGSize(320.0f, 106.0f);
        public static float BlahGutter = 8.0f;
		public static float BlahSpacing = 4.0f;
	}
}

