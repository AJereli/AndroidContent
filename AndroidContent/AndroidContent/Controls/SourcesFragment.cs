using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AllContent_Client;
using MySql.Data.MySqlClient;

namespace AndroidContent
{
    public class SourcesFragment : Android.Support.V4.App.ListFragment
    {

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

        }
        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            ListView.Adapter = new SourceListAdapter(Activity);
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View v = inflater.Inflate(Resource.Layout.FavoritsListLayout, container, false);

            return v;
        }

    }


    class SourceListAdapter : BaseAdapter<string>
    {
        private Activity context;
        private List<string> currentFavorits;

        public SourceListAdapter(Activity _context)
        : base()
        {
            context = _context;
            currentFavorits = User.MainUser.favoritSources;

        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView;

            if (view == null)
                view = context.LayoutInflater.Inflate(Resource.Layout.Source, parent, false);

            string item = this[position];
            CheckBox cb = view.FindViewById<CheckBox>(Resource.Id.CheckBoxSource);
            cb.Text = item;
            cb.CheckedChange += Cb_CheckedChange;

            if (User.MainUser.favoritSources.Contains(item))
            {

                cb.Checked = true;
                if (!currentFavorits.Contains(item))
                    currentFavorits.Add(item);

            }
            else
                cb.Checked = false;

            return view;
        }

        private void Cb_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            if (e.IsChecked)
            {
                if (!currentFavorits.Contains(cb.Text))
                    currentFavorits.Add(cb.Text);
                else return;
            }
            else
                currentFavorits.Remove(cb.Text);
            User.MainUser.UpdateFavorits();
        }

        public override int Count
        {
            get { return FavoritList.Favorits.ALL_SOURCE.Count; }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override string this[int index]
        {
            get { return FavoritList.Favorits.ALL_SOURCE[index]; }
        }

    }
}