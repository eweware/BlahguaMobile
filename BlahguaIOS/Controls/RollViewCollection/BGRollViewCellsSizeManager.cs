using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using MonoTouch.Foundation;

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
        AD
	}

	public class BGRollViewCellsSizeManager 
	{
		#region Fields

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

		private Dictionary<BlahRowType, int> cellsPerRowType = new Dictionary<BlahRowType, int>() { 
			{BlahRowType.A, 3},
			{BlahRowType.B, 3},
			{BlahRowType.C, 3},
			{BlahRowType.D, 2},
			{BlahRowType.E, 2},
			{BlahRowType.F, 1},
            {BlahRowType.AD, 1}
		};

        private Dictionary<BlahRowType, float> rowHeightPerType = new Dictionary<BlahRowType, float>();

        private List<int> adLocs = new List<int>();

		#endregion

		#region Properties


		#endregion

		#region Constructors

		public BGRollViewCellsSizeManager()
		{
            InitializeRowHeights();
		}

        private void InitializeRowHeights()
        {
            rowHeightPerType.Add(BlahRowType.A, BGBlahCellSizesConstants.TinyCellSize.Height);
            rowHeightPerType.Add(BlahRowType.B, BGBlahCellSizesConstants.MediumCellSize.Height);
            rowHeightPerType.Add(BlahRowType.C, BGBlahCellSizesConstants.MediumCellSize.Height);
            rowHeightPerType.Add(BlahRowType.D, BGBlahCellSizesConstants.TinyCellSize.Height);
            rowHeightPerType.Add(BlahRowType.E, BGBlahCellSizesConstants.TinyCellSize.Height);
            rowHeightPerType.Add(BlahRowType.F, BGBlahCellSizesConstants.MediumCellSize.Height);
            rowHeightPerType.Add(BlahRowType.AD, BGBlahCellSizesConstants.TinyCellSize.Height);
           
        }

		#endregion

		#region Public Methods

		public string GetCellSize(NSIndexPath indexPath)
		{
			int itemsCount = 0;
			int index = 0;
			var rowType = GetRowType (indexPath, out itemsCount, out index);
			string sizeName = GetSizes (rowType)[indexPath.Item - itemsCount];
			return sizeName;
		}

		public string GetCellSize(int cellIndex, BlahRowType rowType)
		{
			return GetSizes (rowType) [cellIndex];
		}

        public void AddAd(int adLoc)
        {
            adLocs.Add(adLoc);
        }
		public SizeF GetCellSizeF(string sizeName)
		{
			return GetSizeFFromString(sizeName);
		}

		public SizeF GetSizeFFromString(string name)
		{
			if(name == BGBlahCellSizesConstants.TinyReusableId)
			{
				return BGBlahCellSizesConstants.TinyCellSize;
			} 
			else if(name == BGBlahCellSizesConstants.SmallReusableId)
			{
				return BGBlahCellSizesConstants.SmallCellSize;
			} else if(name == BGBlahCellSizesConstants.MediumReusableId)
			{
				return BGBlahCellSizesConstants.MediumCellSize;
			}
			return BGBlahCellSizesConstants.LargeCellSize;
		}

		public RectangleF GetFrameForItem(NSIndexPath path)
		{
			int index = 0;
			int itemsCount = 0;
			BlahRowType rowType = GetRowType (path, out itemsCount, out index);
			float yRowStart = 0;
			for (int i = 0; i < index; i++) 
			{
				yRowStart += rowHeightPerType [rowTypes[i % rowTypes.Length]];
				yRowStart += BGBlahCellSizesConstants.BlahSpacing; // spacing between rows
			}
			int cellIndex = path.Item - itemsCount;
			float cellXCoord = GetXCoordForCellInRow (rowType, cellIndex);
			float cellYCoord = GetYCoorForCellInRow (rowType, cellIndex, yRowStart);
			string sizeName = GetCellSize (cellIndex, rowType);

			return new RectangleF (new PointF (cellXCoord, cellYCoord), GetCellSizeF (sizeName));
		}

		public SizeF GetContentViewSize(int elementsCount)
		{
			int rowIndex = 0;
			float height = 0;
			for(int i = 0; i < elementsCount;)
			{
				BlahRowType row = rowTypes [rowIndex % 40];
				i += cellsPerRowType [row];
				height += rowHeightPerType [row];
                height += BGBlahCellSizesConstants.BlahSpacing;
				rowIndex++;
			}
			return new SizeF (320.0f, height);
		}

		public NSIndexPath[] GetIndexPathForRect(RectangleF rect)
		{
			List<NSIndexPath> paths = new List<NSIndexPath> ();
			float y = rect.Y < 0.0f ? 0.0f : rect.Y;
			int rowIndex = 0;
			int itemIndex = 0;
			for(float i = 0; i < y + rect.Height; )
			{
				int index = rowIndex % rowTypes.Length;
				string[] sizeForRow = GetSizes (rowTypes [index]);
				if(i >= y - 213.0f)
				{
					foreach(var cell in sizeForRow)
					{

						paths.Add (NSIndexPath.FromItemSection (itemIndex++, 0));
					}
				}
				else
				{
					itemIndex += sizeForRow.Length;
				}
					
                i = i + rowHeightPerType [rowTypes [index]] + BGBlahCellSizesConstants.BlahSpacing;
				rowIndex++;
			}

			return paths.ToArray();
		}

		#endregion

		#region Private Methods

		private BlahRowType GetRowType(NSIndexPath path, out int itemsCount, out int index)
		{
			int itemIndex = path.Item + 1, adCount = 0;
			bool isFound = false;
			index = 0;
			itemsCount = 0;
			BlahRowType rowType = BlahRowType.A;
			while(!isFound)
			{
				if(itemIndex >= itemsCount)
				{
                    
					rowType = rowTypes [index % rowTypes.Length];
					int cellsInRow = cellsPerRowType [rowType];
					if(itemIndex <= itemsCount + cellsInRow)
					{
						isFound = true;
					}
					else
					{
						itemsCount += cellsInRow;
						index += 1;
					}
				}
			}
			return rowType;
		}

		private float GetYCoorForCellInRow(BlahRowType type, int cellIndex, float rowStartYCoord)
		{
			switch(type)
			{
			case BlahRowType.B:
				{
					if(cellIndex == 0)
					{
						return rowStartYCoord;
					} else if(cellIndex == 1)
					{
                            return BGBlahCellSizesConstants.TinyCellSize.Height + BGBlahCellSizesConstants.BlahSpacing + rowStartYCoord;
					}
					else
					{
						return rowStartYCoord;
					}
				}
			case BlahRowType.C:
				{
					if(cellIndex == 0)
					{
						return rowStartYCoord;
					} else if(cellIndex == 1)
					{
						return rowStartYCoord;
					}
					else
					{
                            return BGBlahCellSizesConstants.TinyCellSize.Height + BGBlahCellSizesConstants.BlahSpacing + rowStartYCoord;
					}
				}
			case BlahRowType.A:
			case BlahRowType.D:
			case BlahRowType.E:
			case BlahRowType.F:
			default:
				{
					return rowStartYCoord;
				}
			}
		}

		private float GetXCoordForCellInRow(BlahRowType type, int cellIndex)
		{
            float newLeft = BGBlahCellSizesConstants.BlahGutter;
            float colTwo = newLeft + (BGBlahCellSizesConstants.TinyCellSize.Width + BGBlahCellSizesConstants.BlahSpacing);
            float colThree = newLeft + (BGBlahCellSizesConstants.MediumCellSize.Width + BGBlahCellSizesConstants.BlahSpacing);

			switch(type)
			{
			case BlahRowType.A:
				{
					if(cellIndex == 0)
					{
                            return newLeft;
					} else if(cellIndex == 1)
					{
                            return colTwo;
					}
					else
					{
                            return colThree;
					}
				}
			case BlahRowType.B:
				{
					if(cellIndex == 0)
					{
                            return newLeft;
					} else if(cellIndex == 1)
					{
                            return newLeft;
					}
					else
					{
                            return colTwo;
					}
				}
			case BlahRowType.C:
				{
					if(cellIndex == 0)
					{
                            return newLeft;
					} else if(cellIndex == 1)
					{
                            return colThree;
					}
					else
					{
                            return colThree;
					}
				}
			case BlahRowType.D:
				{
					if(cellIndex == 0)
					{
                            return newLeft;
					} 
					else
					{
                            return colTwo;
					}
				}
			case BlahRowType.E:
				{
					if(cellIndex == 0)
					{
                            return newLeft;
					} 
					else
					{
                            return colThree;
					}
				}
			case BlahRowType.F:
			default:
                    return newLeft;
			}
		}

		private string[] GetSizes(BlahRowType type)
		{
			switch(type)
			{
                case BlahRowType.AD:
                    {
                        return new string[] {
                            BGBlahCellSizesConstants.AdReusableId
                        };
                    }
                case BlahRowType.A:
                    {
                        return new string[] {
                            BGBlahCellSizesConstants.TinyReusableId,
                            BGBlahCellSizesConstants.TinyReusableId,
                            BGBlahCellSizesConstants.TinyReusableId
                        };
                    }
			case BlahRowType.B:
				{
					return new string[] {
						BGBlahCellSizesConstants.TinyReusableId,
						BGBlahCellSizesConstants.TinyReusableId,
						BGBlahCellSizesConstants.MediumReusableId
					};
				}
			case BlahRowType.C:
				{
					return new string[] {
						BGBlahCellSizesConstants.MediumReusableId,
						BGBlahCellSizesConstants.TinyReusableId,
						BGBlahCellSizesConstants.TinyReusableId
					};
				}
			case BlahRowType.D:
				{
					return new string[] {
						BGBlahCellSizesConstants.TinyReusableId,
						BGBlahCellSizesConstants.SmallReusableId
					};
				}
			case BlahRowType.E:
				{
					return new string[] {
						BGBlahCellSizesConstants.SmallReusableId,
						BGBlahCellSizesConstants.TinyReusableId
					};
				}
			case BlahRowType.F:
			default:
				return new string[] {
					BGBlahCellSizesConstants.LargeReusableId
				};
			}
		}
			


		#endregion
	}
}

