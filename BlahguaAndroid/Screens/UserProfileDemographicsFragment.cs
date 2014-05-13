using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using BlahguaMobile.BlahguaCore;
using BlahguaMobile.AndroidClient.Adapters;
using Android.Animation;
using Android.Graphics;

namespace BlahguaMobile.AndroidClient.Screens
{
    class UserProfileDemographicsFragment : Fragment
    {
        public static UserProfileDemographicsFragment NewInstance()
        {
            return new UserProfileDemographicsFragment { Arguments = new Bundle() };
        }

        private readonly string TAG = "UserProfileDemographicsFragment";

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View fragment = inflater.Inflate(Resource.Layout.fragment_userprofile_demographics, null);
            //if (container == null)
            //{
            //    Log.Debug(TAG, "Dialer Fragment is in a view without container");
            //    return null;
            //}
            Spinner spinnerGender = fragment.FindViewById<Spinner>(Resource.Id.gender);
            //spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner_ItemSelected);
            var adapterGender = ArrayAdapter.CreateFromResource(
                    Activity, Resource.Array.gender_array, Android.Resource.Layout.SimpleSpinnerItem);
            adapterGender.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinnerGender.Adapter = adapterGender;
            Spinner spinnerEthnicity = fragment.FindViewById<Spinner>(Resource.Id.ethnicity);
            //spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner_ItemSelected);
            var adapterEthnicity = ArrayAdapter.CreateFromResource(
                    Activity, Resource.Array.ethnicity_array, Android.Resource.Layout.SimpleSpinnerItem);
            adapterEthnicity.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinnerEthnicity.Adapter = adapterEthnicity;

            return fragment;
        }

    }
}