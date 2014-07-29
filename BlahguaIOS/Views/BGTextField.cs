using System;
using System.Drawing;
using System.CodeDom.Compiler;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace BlahguaMobile.IOS
{
	partial class BGTextField : UITextField
	{

		public override RectangleF EditingRect(RectangleF forBounds)
		{
			return new RectangleF(13, 0, forBounds.Width, forBounds.Height);
		}

		public override RectangleF TextRect(RectangleF forBounds)
		{
			return new RectangleF(13, 0, forBounds.Width, forBounds.Height);
		}


		public BGTextField(IntPtr handle) : base(handle) 
		{
			KeyboardAppearance = UIKeyboardAppearance.Light;
		}

		public BGTextField (RectangleF rect) : base (rect)
		{
			KeyboardAppearance = UIKeyboardAppearance.Light;
		}

		public BGTextField() 
		{
			KeyboardAppearance = UIKeyboardAppearance.Light;
		}
	}
}
