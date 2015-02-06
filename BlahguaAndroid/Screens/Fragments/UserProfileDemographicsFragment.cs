using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
//using Android.Support.V4.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using BlahguaMobile.BlahguaCore;
using BlahguaMobile.AndroidClient.Adapters;
using Android.Animation;
using Android.Graphics;
using BlahguaMobile.AndroidClient.HelpingClasses;

namespace BlahguaMobile.AndroidClient.Screens
{
    class UserProfileDemographicsFragment : Fragment
    {
        public static UserProfileDemographicsFragment NewInstance()
        {
            return new UserProfileDemographicsFragment { Arguments = new Bundle() };
        }

        private EditText dob, city, state, post_code;
        private Spinner spinnerGender, spinnerEthnicity, spinnerCountry;
        private CheckBox check_public_gender, check_public_dob, check_public_ethnicity, check_public_city, check_public_state, check_public_post_code, check_public_country;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
			HomeActivity.analytics.PostPageView("/self/demographics");
            View fragment = inflater.Inflate(Resource.Layout.fragment_userprofile_demographics, null);
            //if (container == null)
            //{
            //    Log.Debug(TAG, "Dialer Fragment is in a view without container");
            //    return null;
            //}
            spinnerGender = fragment.FindViewById<Spinner>(Resource.Id.gender);
            //spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner_ItemSelected);
            var adapterGender = ArrayAdapter.CreateFromResource(
                    Activity, Resource.Array.gender_array, Android.Resource.Layout.SimpleSpinnerItem);
            adapterGender.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinnerGender.Adapter = adapterGender;
            spinnerGender.ItemSelected += spinnerGender_ItemSelected;

            spinnerEthnicity = fragment.FindViewById<Spinner>(Resource.Id.ethnicity);
            //spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner_ItemSelected);
            var adapterEthnicity = ArrayAdapter.CreateFromResource(
                    Activity, Resource.Array.ethnicity_array, Android.Resource.Layout.SimpleSpinnerItem);
            adapterEthnicity.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinnerEthnicity.Adapter = adapterEthnicity;
            spinnerEthnicity.ItemSelected += spinnerEthnicity_ItemSelected;

            spinnerCountry = fragment.FindViewById<Spinner>(Resource.Id.country);
            //spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner_ItemSelected);
            var adapterCountry = ArrayAdapter.CreateFromResource(
                    Activity, Resource.Array.country_array, Android.Resource.Layout.SimpleSpinnerItem);
            adapterCountry.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinnerCountry.Adapter = adapterCountry;
            spinnerCountry.ItemSelected += spinnerCountry_ItemSelected;

            dob = fragment.FindViewById<EditText>(Resource.Id.dob);
            city = fragment.FindViewById<EditText>(Resource.Id.city);
            state = fragment.FindViewById<EditText>(Resource.Id.state);
            post_code = fragment.FindViewById<EditText>(Resource.Id.post_code);
            city.TextChanged += city_TextChanged;
            state.TextChanged += state_TextChanged;
            post_code.TextChanged += post_code_TextChanged;

            UiHelper.SetGothamTypeface(TypefaceStyle.Normal, dob, city, state, post_code);

            //// CHECKMARKS
            check_public_gender = fragment.FindViewById<CheckBox>(Resource.Id.check_public_gender);
            check_public_dob = fragment.FindViewById<CheckBox>(Resource.Id.check_public_dob);
            check_public_ethnicity = fragment.FindViewById<CheckBox>(Resource.Id.check_public_ethnicity);
            check_public_city = fragment.FindViewById<CheckBox>(Resource.Id.check_public_city);
            check_public_state = fragment.FindViewById<CheckBox>(Resource.Id.check_public_state);
            check_public_post_code = fragment.FindViewById<CheckBox>(Resource.Id.check_public_post_code);
            check_public_country = fragment.FindViewById<CheckBox>(Resource.Id.check_public_country);
            check_public_gender.CheckedChange += check_public_gender_CheckedChange;
            check_public_dob.CheckedChange += check_public_dob_CheckedChange;
            check_public_ethnicity.CheckedChange += check_public_ethnicity_CheckedChange;
            check_public_city.CheckedChange += check_public_city_CheckedChange;
            check_public_state.CheckedChange += check_public_state_CheckedChange;
            check_public_post_code.CheckedChange += check_public_post_code_CheckedChange;
            check_public_country.CheckedChange += check_public_country_CheckedChange;

            UiHelper.SetGothamTypeface(TypefaceStyle.Normal, check_public_gender, check_public_dob, check_public_ethnicity, check_public_city, check_public_state, check_public_post_code, check_public_country);

            UiHelper.SetGothamTypeface(TypefaceStyle.Normal,
                fragment.FindViewById<TextView>(Resource.Id.t1),
                fragment.FindViewById<TextView>(Resource.Id.t2),
                fragment.FindViewById<TextView>(Resource.Id.t3),
                fragment.FindViewById<TextView>(Resource.Id.t4),
                fragment.FindViewById<TextView>(Resource.Id.t5),
                fragment.FindViewById<TextView>(Resource.Id.t6),
                fragment.FindViewById<TextView>(Resource.Id.t7));

            UpdateDemographics();

            return fragment;
        }

        #region TextChanged delegates
        private void city_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            if (isDemoPopulated && (BlahguaAPIObject.Current.CurrentUser.Profile != null))
            {
                string newVal = city.Text;
                if (newVal != BlahguaAPIObject.Current.CurrentUser.Profile.City)
                {
                    BlahguaAPIObject.Current.CurrentUser.Profile.City = newVal;
                    UpdateProfile();
                }
            }
        }
        private void state_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            if (isDemoPopulated && (BlahguaAPIObject.Current.CurrentUser.Profile != null))
            {
                string newVal = state.Text;
                if (newVal != BlahguaAPIObject.Current.CurrentUser.Profile.State)
                {
                    BlahguaAPIObject.Current.CurrentUser.Profile.State = newVal;
                    UpdateProfile();
                }
            }
        }
        private void post_code_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            if (isDemoPopulated && (BlahguaAPIObject.Current.CurrentUser.Profile != null))
            {
                string newVal = post_code.Text;
                if (newVal != BlahguaAPIObject.Current.CurrentUser.Profile.Zipcode)
                {
                    BlahguaAPIObject.Current.CurrentUser.Profile.Zipcode = newVal;
                    UpdateProfile();
                }
            }
        }
        #endregion

        #region CheckedChange delegates
        private void check_public_gender_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            if (isDemoPopulated && (BlahguaAPIObject.Current.CurrentUser.Profile != null))
            {
                bool newVal = (bool)e.IsChecked;

                if (newVal != BlahguaAPIObject.Current.CurrentUser.Profile.GenderPerm)
                {
                    BlahguaAPIObject.Current.CurrentUser.Profile.GenderPerm = newVal;
                    UpdateProfile();
                }
            }
        }
        private void check_public_dob_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            if (isDemoPopulated && (BlahguaAPIObject.Current.CurrentUser.Profile != null))
            {
                bool newVal = (bool)e.IsChecked;

                if (newVal != BlahguaAPIObject.Current.CurrentUser.Profile.DOBPerm)
                {
                    BlahguaAPIObject.Current.CurrentUser.Profile.DOBPerm = newVal;
                    UpdateProfile();
                }
            }
        }
        private void check_public_ethnicity_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            if (isDemoPopulated && (BlahguaAPIObject.Current.CurrentUser.Profile != null))
            {
                bool newVal = (bool)e.IsChecked;

                if (newVal != BlahguaAPIObject.Current.CurrentUser.Profile.RacePerm)
                {
                    BlahguaAPIObject.Current.CurrentUser.Profile.RacePerm = newVal;
                    UpdateProfile();
                }
            }
        }
        private void check_public_city_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            if (isDemoPopulated && (BlahguaAPIObject.Current.CurrentUser.Profile != null))
            {
                bool newVal = (bool)e.IsChecked;

                if (newVal != BlahguaAPIObject.Current.CurrentUser.Profile.CityPerm)
                {
                    BlahguaAPIObject.Current.CurrentUser.Profile.CityPerm = newVal;
                    UpdateProfile();
                }
            }
        }
        private void check_public_state_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            if (isDemoPopulated && (BlahguaAPIObject.Current.CurrentUser.Profile != null))
            {
                bool newVal = (bool)e.IsChecked;

                if (newVal != BlahguaAPIObject.Current.CurrentUser.Profile.StatePerm)
                {
                    BlahguaAPIObject.Current.CurrentUser.Profile.StatePerm = newVal;
                    UpdateProfile();
                }
            }
        }
        private void check_public_post_code_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            if (isDemoPopulated && (BlahguaAPIObject.Current.CurrentUser.Profile != null))
            {
                bool newVal = (bool)e.IsChecked;

                if (newVal != BlahguaAPIObject.Current.CurrentUser.Profile.ZipcodePerm)
                {
                    BlahguaAPIObject.Current.CurrentUser.Profile.ZipcodePerm = newVal;
                    UpdateProfile();
                }
            }
        }
        private void check_public_country_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            if (isDemoPopulated && (BlahguaAPIObject.Current.CurrentUser.Profile != null))
            {
                bool newVal = (bool)e.IsChecked;

                if (newVal != BlahguaAPIObject.Current.CurrentUser.Profile.CountryPerm)
                {
                    BlahguaAPIObject.Current.CurrentUser.Profile.CountryPerm = newVal;
                    UpdateProfile();
                }
            }
        }
        #endregion

        #region SelectionChange delegates
        private void spinnerGender_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            if (isDemoPopulated && (BlahguaAPIObject.Current.CurrentUser.Profile != null))
            {
                string newVal = spinnerGender.SelectedItem.ToString();
                if (newVal != BlahguaAPIObject.Current.CurrentUser.Profile.Gender)
                {
                    BlahguaAPIObject.Current.CurrentUser.Profile.Gender = newVal;
                    UpdateProfile();
                }
            }
        }
        private void spinnerEthnicity_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            if (isDemoPopulated && (BlahguaAPIObject.Current.CurrentUser.Profile != null))
            {
                string newVal = spinnerEthnicity.SelectedItem.ToString();
                if (newVal != BlahguaAPIObject.Current.CurrentUser.Profile.Race)
                {
                    BlahguaAPIObject.Current.CurrentUser.Profile.Race = newVal;
                    UpdateProfile();
                }
            }
        }
        private void spinnerCountry_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            if (isDemoPopulated && (BlahguaAPIObject.Current.CurrentUser.Profile != null))
            {
                string newVal = spinnerCountry.SelectedItem.ToString();
                if (newVal != BlahguaAPIObject.Current.CurrentUser.Profile.Country)
                {
                    BlahguaAPIObject.Current.CurrentUser.Profile.Country = newVal;
                    UpdateProfile();
                }
            }
        }
        #endregion

        bool isDemoPopulated = false;
        private void SetSpinner(Spinner sp, String[] array, String value)
        {
            sp.SetSelection(0);
            for (int i = 0; i < array.Length; ++i)
            {
                if (array[i].Equals(value))
                {
                    sp.SetSelection(i);
                    break;
                }
            }
        }
        private void UpdateDemographics()
        {
            UserProfile theProfile = BlahguaAPIObject.Current.CurrentUser.Profile;

            if (theProfile != null)
            {
                isDemoPopulated = false;
                String[] genders = Activity.Resources.GetStringArray(Resource.Array.gender_array);
                String[] ethnicities = Activity.Resources.GetStringArray(Resource.Array.ethnicity_array);
                String[] countries = Activity.Resources.GetStringArray(Resource.Array.country_array);
                
                SetSpinner(spinnerGender, genders, theProfile.Gender);
                SetSpinner(spinnerEthnicity, ethnicities, theProfile.Race);
                SetSpinner(spinnerCountry, countries, theProfile.Country);
                //IncomeList.SelectedItem = theProfile.Income;
                if (theProfile.DOB != null)
                {
                    dob.Text = theProfile.DOB.Value.ToString("MM/dd/yyyy");
                }
                city.Text = theProfile.City;
                state.Text = theProfile.State;
                post_code.Text = theProfile.Zipcode;

                check_public_gender.Checked = theProfile.GenderPerm;
                check_public_dob.Checked = theProfile.DOBPerm;
                check_public_ethnicity.Checked = theProfile.RacePerm;
                check_public_city.Checked = theProfile.CityPerm;
                check_public_state.Checked = theProfile.StatePerm;
                check_public_post_code.Checked = theProfile.ZipcodePerm;
                check_public_country.Checked = theProfile.CountryPerm;
                //IncomePerm.IsChecked = theProfile.IncomePerm;

                isDemoPopulated = true;
            }
        }

        //////////////////////////
        void UpdateProfile()
        {
            // the profile has changed, save and reload the description...
            BlahguaAPIObject.Current.UpdateUserProfile((theString) =>
                {
                    BlahguaAPIObject.Current.GetUserDescription((theDesc) =>
                        {
                            // to do - see if we need to rebind or...
                        }
                    );
                }
            );
        }
    }
}