using System;
using CoreGraphics;
using System.CodeDom.Compiler;

using Foundation;
using UIKit;

namespace BlahguaMobile.IOS
{
	partial class BGTextField : UITextField
	{

		public override CGRect EditingRect(CGRect forBounds)
		{
			return new CGRect(13, 0, forBounds.Width, forBounds.Height);
		}

		public override CGRect TextRect(CGRect forBounds)
		{
			return new CGRect(13, 0, forBounds.Width, forBounds.Height);
		}


		public BGTextField(IntPtr handle) : base(handle) 
		{
			KeyboardAppearance = UIKeyboardAppearance.Light;
		}

		public BGTextField (CGRect rect) : base (rect)
		{
			KeyboardAppearance = UIKeyboardAppearance.Light;
		}

		public BGTextField() 
		{
			KeyboardAppearance = UIKeyboardAppearance.Light;
		}
	}
}
