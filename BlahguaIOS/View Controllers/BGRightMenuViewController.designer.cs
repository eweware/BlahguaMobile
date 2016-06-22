// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace BlahguaMobile.IOS
{
    [Register ("BGRightMenuViewController")]
    partial class BGRightMenuViewController
    {
        [Outlet]
        UIKit.UIButton m_btnBadges { get; set; }


        [Outlet]
        UIKit.UIButton m_btnDemographics { get; set; }


        [Outlet]
        UIKit.UIButton m_btnHistory { get; set; }


        [Outlet]
        UIKit.UIButton m_btnLogout { get; set; }


        [Outlet]
        UIKit.UIButton m_btnProfile { get; set; }


        [Outlet]
        UIKit.UIButton m_btnStats { get; set; }


        [Outlet]
        UIKit.UIImageView m_imgAvatar { get; set; }


        [Outlet]
        UIKit.UILabel m_lblUserName { get; set; }


        [Outlet]
        UIKit.UILabel m_menuHeader { get; set; }

        void ReleaseDesignerOutlets ()
        {
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