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
using Android.Support.Design.Widget;

using SupportFragment = Android.Support.V4.App.Fragment;

namespace AndroidContent
{
    class NewsListFragment : SupportFragment
    {

        private List<ContentUnit> list_cu;

        private bool isLoading = false;
        private bool IsRecyclerViewInited = false;

        private RecyclerView mRecyclerView;
        private ItemAdapter mAdapter;
        private LinearLayoutManager mLayoutManager;
        private ItemScrollListener scrollListener;


        private SwipeRefreshLayout refresher;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            list_cu = new List<ContentUnit>();
            mLayoutManager = new LinearLayoutManager(Activity);
            scrollListener = new ItemScrollListener(mLayoutManager);
            mAdapter = new ItemAdapter(list_cu, Activity);

            FavoritList.Favorits.ReloadAllhEvent += Favorits_ReloadAllhEvent; ;
            FavoritList.Favorits.AddEvent += NewContentEvent;
            scrollListener.LoadMoreEvent += (s, e) =>
            {
                if (!isLoading)
                {
                    isLoading = true;
                    FavoritList.Favorits.LoadNextNews(mAdapter);
                }
            };


        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View v = inflater.Inflate(Resource.Layout.NewsListLayout, container, false);

            refresher = v.FindViewById<SwipeRefreshLayout>(Resource.Id.refresher);
            mRecyclerView = v.FindViewById<RecyclerView>(Resource.Id.recyclerView);
            refresher.Refresh += Refresher_Refresh;
            if (!IsRecyclerViewInited)
            {
                mRecyclerView.SetLayoutManager(mLayoutManager);
                mRecyclerView.AddOnScrollListener(scrollListener);
                mRecyclerView.SetAdapter(mAdapter);
                IsRecyclerViewInited = true;
            }
            User.MainUser.LoadFavoritSources();

            return v;

        }

       

        private void Favorits_ReloadAllhEvent(Favorit obj)
        {
            if (obj.content.Count == 0)
                return;
            list_cu.RemoveAll(i => i.source == obj.Source);
            list_cu.AddRange(obj.content);

        }

        private void NewContentEvent(Favorit fav)
        {
            if (fav.content.Count == 0)
            {
                isLoading = false;
                return;
            }
            list_cu.AddRange(fav.content);
            isLoading = false;
        }

        async private void Refresher_Refresh(object sender, EventArgs e)
        {
            await FavoritList.Favorits.ReloadAll(mAdapter);
            mAdapter.NotifyDataSetChanged();
            refresher.Refreshing = false;

        }

    }

    
}