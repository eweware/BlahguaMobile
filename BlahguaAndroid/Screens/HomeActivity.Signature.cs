using System;
using System.Timers;
using System.ComponentModel;

using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using Android.OS;

using SlidingMenuSharp;
using SlidingMenuSharp.App;

using BlahguaMobile.BlahguaCore;
using BlahguaMobile.AndroidClient.ThirdParty.UrlImageViewHelper;
using Android.Preferences;
using BlahguaMobile.AndroidClient.Adapters;
using System.IO.IsolatedStorage;
using Android.Graphics;
using Android.Animation;
using System.Collections.Generic;
using Java.Util;
using Android.Graphics.Drawables;

namespace BlahguaMobile.AndroidClient.Screens
{
    public partial class HomeActivity
    {

        private PopupWindow signaturePopup;
        /*
         * Function to set up the pop-up window which acts as drop-down list
         * */
        private void initiateSignaturePopUp()
        {
            LayoutInflater inflater = (LayoutInflater)GetSystemService(Context.LayoutInflaterService);

            //get the pop-up window i.e.  drop-down layout
            LinearLayout layout = (LinearLayout)inflater.Inflate(Resource.Layout.popup_signature, (ViewGroup)FindViewById(Resource.Id.popUpView));

            //get the view to which drop-down layout is to be anchored
            Button layout1 = (Button)FindViewById(Resource.Id.btn_signature);

            signaturePopup = new PopupWindow(layout, (int)(Resources.DisplayMetrics.Density * 200), ViewGroup.LayoutParams.WrapContent, true);

            //Pop-up window background cannot be null if we want the pop-up to listen touch events outside its window
            signaturePopup.SetBackgroundDrawable(new BitmapDrawable());
            signaturePopup.Touchable = true;

            //let pop-up be informed about touch events outside its window. This  should be done before setting the content of pop-up
            signaturePopup.OutsideTouchable = true;
            signaturePopup.Height = ViewGroup.LayoutParams.WrapContent;

            //dismiss the pop-up i.e. drop-down when touched anywhere outside the pop-up
            //pw.setTouchInterceptor(new OnTouchListener() {

            //    public bool onTouch(View v, MotionEvent ev) {
            //        // TODO Auto-generated method stub
            //        if (ev.Action == MotionEventActions.Outside) {
            //            pw.Dismiss();
            //            return true;    				
            //        }
            //        return false;
            //    }
            //});

            //provide the source layout for drop-down
            signaturePopup.ContentView = layout;

            //anchor the drop-down to bottom-left corner of 'layout1'
            signaturePopup.ShowAsDropDown(layout1);

            //populate the drop-down list
            ListView list = (ListView)layout.FindViewById(Resource.Id.dropDownList);
            CreateBlahSignatureAdapter adapter = new CreateBlahSignatureAdapter(this, BlahguaAPIObject.Current.CurrentUser.Badges);
            list.Adapter = adapter;
        }
    }
}


