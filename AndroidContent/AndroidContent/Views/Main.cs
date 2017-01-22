using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Views;
using Android.Widget;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V7.Widget;
using Android.Support.V7.App;
using AllContent_Client;
using Android.Util;


namespace AndroidContent.Views
{
    [Activity(Label = "Main Page TEST"/*, MainLauncher = true*/, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private List<ContentUnit> list_cu;
        Tests.ContentLoadTest CLT;

        private RecyclerView mRecyclerView;
        private ItemAdapter mAdapter;
        private RecyclerView.LayoutManager mLayoutManager;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.MainLayout);

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

            mAdapter = new ItemAdapter(list_cu, this);
            mRecyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);
            mLayoutManager = new LinearLayoutManager(this);
            //mAdapter.ItemClick += MAdapter_ItemClick;

            mRecyclerView.SetLayoutManager(mLayoutManager);
            mRecyclerView.SetAdapter(mAdapter);

        }

       
    }

   

}