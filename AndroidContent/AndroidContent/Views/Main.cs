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
using Android.Support.V4.Widget;

namespace AndroidContent.Views
{
    [Activity(Label = "Main Page TEST"/*, MainLauncher = true*/, Icon = "@drawable/icon")]
    public class MainActivity : Activity /*, SwipeRefreshLayout.IOnRefreshListener */
    {
        private List<ContentUnit> list_cu;

        bool isLoading = false;
        private RecyclerView mRecyclerView;
        private ItemAdapter mAdapter;
        private LinearLayoutManager mLayoutManager;
        private ItemScrollListener scrollListener;
        SwipeRefreshLayout refresher;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.MainLayout);
            list_cu = new List<ContentUnit>();

            refresher = FindViewById<SwipeRefreshLayout>(Resource.Id.refresher);
            refresher.Refresh += Refresher_Refresh;
        

            mLayoutManager = new LinearLayoutManager(this);
            scrollListener = new ItemScrollListener(mLayoutManager);

            scrollListener.LoadMoreEvent += ScrollListener_LoadMoreEvent;

            FavoritList.Favorits.ReloadAllhEvent += NewContentEvent;
            FavoritList.Favorits.AddEvent += NewContentEvent;
            User.MainUser.LoadFavoritSources();

            mAdapter = new ItemAdapter(list_cu, this);

            mRecyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);

            mRecyclerView.SetLayoutManager(mLayoutManager);
            mRecyclerView.AddOnScrollListener(scrollListener);
            mRecyclerView.SetAdapter(mAdapter);

        }

        private void NewContentEvent(Favorit fav)
        {
            if (fav.content.Count == 0)
                return;
            list_cu.AddRange(fav.content);
            isLoading = false;
            Log.Info("List_cu cnt: ", list_cu.Count.ToString());
        }

        async private void Refresher_Refresh(object sender, EventArgs e)
        {
            await FavoritList.Favorits.ReloadAll(mAdapter);
            mAdapter.NotifyDataSetChanged();
            refresher.Refreshing = false;

        }

        private void ScrollListener_LoadMoreEvent(object sender, EventArgs e)
        {
            if (!isLoading)
            {
                isLoading = true;
                FavoritList.Favorits.LoadNextNews(mAdapter);
            }
        }

       
    }



}