// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace BlahguaMobile.IOS
{
	[Register ("BGFullScreenViewController")]
	partial class BGFullScreenViewController
	{
		[Outlet]
		MonoTouch.UIKit.UIImageView m_imageView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (m_imageView != null) {
				m_imageView.Dispose ();
				m_imageView = null;
			}
		}
	}
}
