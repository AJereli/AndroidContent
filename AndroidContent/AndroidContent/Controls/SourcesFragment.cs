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
        private List<string> listOfAllFavorits;
        private List<string> currentFavorits;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            listOfAllFavorits = new List<string>();
            currentFavorits = User.MainUser.favoritSources;
            using (DBClient client = new DBClient())
            {

                List<string> sources = client.SelectQuery("SELECT favorites_source FROM users WHERE login = @login", new MySqlParameter("login", "$sources"));
                if (sources != null && sources.Count != 0)
                    foreach (var str in sources[0].Split(';'))
                        if (str != "")
                            listOfAllFavorits.Add(str);
            }

        }
        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            ListView.Adapter = new SourceListAdapter(Activity, listOfAllFavorits);
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View v = inflater.Inflate(Resource.Layout.FavoritsListLayout, container, false);
            v.FindViewById<Button>(Resource.Id.RefreshFavoritsButton).Click += SourcesFragment_Click;
            return v;
        }

        private void SourcesFragment_Click(object sender, EventArgs e)
        {
            User.MainUser.UpdateFavorits();
        }
    }


    class SourceListAdapter : BaseAdapter<string>
    {
        private Activity context;
        private List<string> allFavoritsList;
        private List<string> currentFavorits;

        public SourceListAdapter(Activity _context, List<string> _allFavoritsList)
        : base()
        {
            context = _context;
            allFavoritsList = _allFavoritsList;
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
                currentFavorits.Add(cb.Text);
            }
            else
                currentFavorits.Remove(cb.Text);
            User.MainUser.UpdateFavorits();
        }

        public override int Count
        {
            get { return allFavoritsList.Count; }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override string this[int index]
        {
            get { return allFavoritsList[index]; }
        }

    }
}