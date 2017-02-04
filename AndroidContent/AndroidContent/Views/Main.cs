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
using Android.Preferences;

namespace AndroidContent.Views
{
    [Activity(Label = "Main Page TEST"/*, MainLauncher = true*/, Icon = "@drawable/icon", Theme = "@style/Theme.DesignDemo")]
    public class MainActivity : AppCompatActivity
    {
        private List<ContentUnit> list_cu;
        private DrawerLayout mDrawerLayout;
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

            Android.Support.V7.Widget.Toolbar toolBar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolBar);
            SetSupportActionBar(toolBar);

            Android.Support.V7.App.ActionBar ab = SupportActionBar;
            ab.SetHomeAsUpIndicator(Resource.Drawable.ic_menu);
            ab.SetDisplayHomeAsUpEnabled(true);

            mDrawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);

            Android.Support.Design.Widget.NavigationView navigationView = FindViewById<Android.Support.Design.Widget.NavigationView>(Resource.Id.nav_view);
            navigationView.NavigationItemSelected += NavigationView_NavigationItemSelected;
            if (navigationView != null)
            {
                SetUpDrawerContent(navigationView);
            }       

            refresher = FindViewById<SwipeRefreshLayout>(Resource.Id.refresher);
            mRecyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);

            refresher.Refresh += Refresher_Refresh;        

            mLayoutManager = new LinearLayoutManager(this);
            scrollListener = new ItemScrollListener(mLayoutManager);

            scrollListener.LoadMoreEvent += (s, e) =>
            {
                if (!isLoading)
                {
                    isLoading = true;
                    FavoritList.Favorits.LoadNextNews(mAdapter);
                }
            };

            FavoritList.Favorits.ReloadAllhEvent += Favorits_ReloadAllhEvent; ;
            FavoritList.Favorits.AddEvent += NewContentEvent;

            User.MainUser.LoadFavoritSources();

            mAdapter = new ItemAdapter(list_cu, this);

            mRecyclerView.SetLayoutManager(mLayoutManager);
            mRecyclerView.AddOnScrollListener(scrollListener);
            mRecyclerView.SetAdapter(mAdapter);

            var mUsername = FindViewById<TextView>(Resource.Id.username);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    mDrawerLayout.OpenDrawer((int)GravityFlags.Left);
                    return true;

                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        void NavigationView_NavigationItemSelected(object sender, Android.Support.Design.Widget.NavigationView.NavigationItemSelectedEventArgs e)
        {
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this); ;
            ISharedPreferencesEditor editor = prefs.Edit();
            switch (e.MenuItem.ItemId)
            {
                case (Resource.Id.exit):
                    editor.PutBoolean("check", false);
                    editor.Apply();
                    Intent authoriz = new Intent(this, typeof(AuthorizationActivity));                    
                    StartActivity(authoriz);
                    Finish();
                    break;
            }
        }

        private void SetUpDrawerContent(Android.Support.Design.Widget.NavigationView navigationView)
        {
            navigationView.NavigationItemSelected += (object sender, Android.Support.Design.Widget.NavigationView.NavigationItemSelectedEventArgs e) =>
            {
                e.MenuItem.SetChecked(true);
                mDrawerLayout.CloseDrawers();
            };
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
                return;
            list_cu.AddRange(fav.content);
            isLoading = false;
           // Log.Info("List_cu cnt: ", list_cu.Count.ToString());
            }

        async private void Refresher_Refresh(object sender, EventArgs e)
        {
            await FavoritList.Favorits.ReloadAll(mAdapter);
            mAdapter.NotifyDataSetChanged();
            refresher.Refreshing = false;

        }
        public override bool OnKeyDown(Keycode keyCode, KeyEvent e)
        {
            Finish();
            return base.OnKeyDown(keyCode, e);
        }
       
    }
}