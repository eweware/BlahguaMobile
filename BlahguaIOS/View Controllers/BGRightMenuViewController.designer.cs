// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.CodeDom.Compiler;

namespace BlahguaMobile.IOS
{
	[Register ("BGRightMenuViewController")]
	partial class BGRightMenuViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton m_btnBadges { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton m_btnDemographics { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton m_btnHistory { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton m_btnLogout { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton m_btnProfile { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton m_btnStats { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIImageView m_imgAvatar { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel m_lblUserName { get; set; }

		void ReleaseDesignerOutlets ()
		{
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
