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
	[Register ("BGRightMenuViewController")]
	partial class BGRightMenuViewController
	{
		[Outlet]
		MonoTouch.UIKit.UIButton m_btnBadges { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton m_btnDemographics { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton m_btnHistory { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton m_btnLogout { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton m_btnProfile { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton m_btnStats { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView m_imgAvatar { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel m_lblUserName { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel m_menuHeader { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (m_menuHeader != null) {
				m_menuHeader.Dispose ();
				m_menuHeader = null;
			}

			if (m_btnBadges != null) {
				m_btnBadges.Dispose ();
				m_btnBadges = null;
			}

			if (m_btnDemographics != null) {
				m_btnDemographics.Dispose ();
				m_btnDemographics = null;
			}

			if (m_btnHistory != null) {
				m_btnHistory.Dispose ();
				m_btnHistory = null;
			}

			if (m_btnLogout != null) {
				m_btnLogout.Dispose ();
				m_btnLogout = null;
			}

			if (m_btnProfile != null) {
				m_btnProfile.Dispose ();
				m_btnProfile = null;
			}

			if (m_btnStats != null) {
				m_btnStats.Dispose ();
				m_btnStats = null;
			}

			if (m_imgAvatar != null) {
				m_imgAvatar.Dispose ();
				m_imgAvatar = null;
			}

			if (m_lblUserName != null) {
				m_lblUserName.Dispose ();
				m_lblUserName = null;
			}
		}
	}
}
