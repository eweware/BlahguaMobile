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
using BlahguaMobile.AndroidClient.Screens;
using System.Collections;
using BlahguaMobile.AndroidClient.HelpingClasses;
using Android.Graphics;

namespace BlahguaMobile.AndroidClient.Adapters
{
    class GothamFontArrayAdapter : ArrayAdapter
    {
    private Context mContext;
    private int id;
    private IList items;

    public GothamFontArrayAdapter(Context context, int textViewResourceId, IList list) 
        : base(context, textViewResourceId, list)
    {
        mContext = context;
        id = textViewResourceId;
        items = list;
    }

    public override View GetView(int position, View v, ViewGroup parent)
    {
        View mView = v ;
        if(mView == null) {
            LayoutInflater vi = (LayoutInflater)mContext.GetSystemService(Context.LayoutInflaterService);
            mView = vi.Inflate(id, null);
        }

        TextView text = (TextView) mView.FindViewById(Android.Resource.Id.Text1);

        if(items[position] != null )
        {
            UiHelper.SetGothamTypeface(TypefaceStyle.Normal, text);
            text.Text = items[position].ToString();

        }

        return mView;
    }
    }
}