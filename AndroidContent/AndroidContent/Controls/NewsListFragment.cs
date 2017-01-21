using System;
using System.Collections.Generic;
using System.Linq;
using AllContent_Client;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V4.App;
using Square.Picasso;
using Android.Util;

namespace AndroidContent
{
    public class NewsListFragment : Android.Support.V4.App.ListFragment
    {

        private  List<ContentUnit> list_cu; // Вот так инкапсуляция идет нахуй
      
        Tests.ContentLoadTest CLT;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            list_cu = new List<ContentUnit>();
          
            FavoritList.Favorits.AddEvent += () =>
            {
                foreach (var fav in FavoritList.Favorits)
                {
                    list_cu.AddRange(fav.content);
                    Log.Info("List_cu cnt: ", list_cu.Count.ToString());
                }

            };
            CLT = new Tests.ContentLoadTest();
            
            ContentUnitAdapter adapter = new ContentUnitAdapter(Activity, list_cu);
            ListAdapter = adapter;
        }
        
        public override void OnListItemClick(ListView l, View v, int position, long id)
        {
            ContentUnit cu = ((ContentUnitAdapter)ListAdapter).GetItem(position);
            Intent intent = new Intent(Activity, Java.Lang.Class.FromType(typeof(WebActivity)));
            intent.SetData(Android.Net.Uri.Parse(cu.URL));
            StartActivity(intent);
        }
        
    }

 
    class ContentUnitAdapter : ArrayAdapter<ContentUnit>
    {
        Activity activity;
        public ContentUnitAdapter(Activity _activity, List<ContentUnit> content_list) : base(_activity, 0, content_list)
        {
            activity = _activity;
            
        }
      
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            
            ContentUnit cu = GetItem(position);
            int r_id_header = 0, r_id_description = 0, r_id_date = 0;
            convertView = activity.LayoutInflater.Inflate(Resource.Layout.item, null);

            if (cu.imgUrl == null || cu.imgUrl.Length == 0)
            {
                convertView = activity.LayoutInflater.Inflate(Resource.Layout.item_without_img, null);
                r_id_header = Resource.Id.content_HeaderTextViewWI;
                r_id_description = Resource.Id.content_DescriptionTextViewWI;
                r_id_date = Resource.Id.content_DateTextViewWI;
            }
            else {
                convertView = activity.LayoutInflater.Inflate(Resource.Layout.item, null);
                r_id_header = Resource.Id.content_HeaderTextView;
                r_id_description = Resource.Id.content_DescriptionTextView;
                r_id_date = Resource.Id.content_DateTextView;
                ImageView iv = convertView.FindViewById<ImageView>(Resource.Id.content_imgImageView);
                Picasso.With(activity).Load(cu.imgUrl).Into(iv);
            }


            TextView header = convertView.FindViewById<TextView>(r_id_header);
            TextView description = convertView.FindViewById<TextView>(r_id_description);
            TextView date = convertView.FindViewById<TextView>(r_id_date);

            header.Text = cu.header;
            description.Text = cu.description;
            date.Text = cu.date;

            if (position == Count - 1)
            {
                FavoritList.Favorits.LoadNextNews();
                Log.Info("COUNT INFO", "Pos: " + position + " CNT: " + Count);
            }

            return convertView;
        }
    }
}