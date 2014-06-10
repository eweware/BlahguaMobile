using System;

namespace BlahguaMobile.IOS
{
	public enum BlahRowType
	{
		A,
		B,
		C,
		D,
		E,
		F,
	}

	public class BGRollViewCellsSizeManager 
	{
		#region Fields

		private int currentRowNumber;
		private BlahRowType currentRowType;
		private BlahRowType[] rowTypes = new BlahRowType[] { 
			BlahRowType.A,
			BlahRowType.B,
			BlahRowType.E,
			BlahRowType.A,
			BlahRowType.F,
			BlahRowType.A,
			BlahRowType.D,
			BlahRowType.C,
			BlahRowType.A,
			BlahRowType.D,
			BlahRowType.E,
			BlahRowType.A,
			BlahRowType.C,
			BlahRowType.D,
			BlahRowType.A,
			BlahRowType.F,
			BlahRowType.A,
			BlahRowType.E,
			BlahRowType.B,
			BlahRowType.A,
			BlahRowType.D,
			BlahRowType.A,
			BlahRowType.D,
			BlahRowType.C,
			BlahRowType.A,
			BlahRowType.F,
			BlahRowType.A,
			BlahRowType.B,
			BlahRowType.E,
			BlahRowType.A,
			BlahRowType.E,
			BlahRowType.B,
			BlahRowType.A,
			BlahRowType.F,
			BlahRowType.A,
			BlahRowType.C,
			BlahRowType.D,
			BlahRowType.A,
			BlahRowType.E,
			BlahRowType.A 
		};

		#endregion

		#region Properties

		public BlahRowType CurrentItemType
		{
			get
			{
				return currentRowType;
			}
		}

		#endregion

		#region Methods

		public string[] GetNextRowSizes()
		{
			currentRowNumber++;
			int rowIndex = currentRowNumber % rowTypes.Length;
			currentRowType = rowTypes [rowIndex];
			switch(currentRowType)
			{
			case BlahRowType.A:
				{
					return new string[] {
						BGBlahCellSizesConstants.TinyReusableId,
						BGBlahCellSizesConstants.TinyReusableId,
						BGBlahCellSizesConstants.TinyReusableId
					};
					break;
				}
			case BlahRowType.B:
				{
					return new string[] {
						BGBlahCellSizesConstants.TinyReusableId,
						BGBlahCellSizesConstants.TinyReusableId,
						BGBlahCellSizesConstants.MediumReusableId
					};
					break;
				}
			case BlahRowType.C:
				{
					return new string[] {
						BGBlahCellSizesConstants.MediumReusableId,
						BGBlahCellSizesConstants.TinyReusableId,
						BGBlahCellSizesConstants.TinyReusableId
					};
					break;
				}
			case BlahRowType.D:
				{
					return new string[] {
						BGBlahCellSizesConstants.TinyReusableId,
						BGBlahCellSizesConstants.SmallReusableId
					};
					break;
				}
			case BlahRowType.E:
				{
					return new string[] {
						BGBlahCellSizesConstants.SmallReusableId,
						BGBlahCellSizesConstants.TinyReusableId
					};
					break;
				}
			case BlahRowType.F:
				{
					return new string[] {
						BGBlahCellSizesConstants.LargeReusableId
					};
					break;
				}
			}
		}

		#endregion
	}
}

