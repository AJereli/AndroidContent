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
using Android.Preferences;

namespace AndroidContent.Views
{
    [Activity(Label = "Main Page TEST"/*, MainLauncher = true*/, Icon = "@drawable/icon", Theme = "@style/Theme.DesignDemo")]
    public class MainActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {

        private NavigationView navigationView;
        private DrawerLayout mDrawerLayout;
        protected override void OnCreate(Bundle bundle)
        {
            #region initialisation
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.MainLayout);
            SupportFragmentManager.BeginTransaction().AddToBackStack("news").Add(Resource.Id.fragmentZone, new NewsListFragment()).Commit();

            Android.Support.V7.Widget.Toolbar toolBar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolBar);
            SetSupportActionBar(toolBar);

            Android.Support.V7.App.ActionBar ab = SupportActionBar;
            ab.SetHomeAsUpIndicator(Resource.Drawable.ic_menu);
            ab.SetDisplayHomeAsUpEnabled(true);

            mDrawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);

            navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);

           


            mDrawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            View navHeader = navigationView.GetHeaderView(0);
            navHeader.FindViewById<TextView>(Resource.Id.userName).Text = User.Name;
            #endregion
            navigationView.SetNavigationItemSelectedListener(this);



        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater inflater = MenuInflater;
            inflater.Inflate(Resource.Menu.drawer_view, menu);
            return true;
        }
        public bool OnNavigationItemSelected(IMenuItem menuItem)
        {
            menuItem.SetCheckable(false);

            switch (menuItem.ItemId)
            {
                case Resource.Id.news:

                    SupportFragmentManager.BeginTransaction().AddToBackStack("news")
                   .Replace(Resource.Id.fragmentZone, new NewsListFragment()).Commit();

                    break;
                case Resource.Id.content_sources:


                    SupportFragmentManager.BeginTransaction().AddToBackStack("sources")
               .Replace(Resource.Id.fragmentZone, new SourcesFragment()).Commit();


                    break;
                case Resource.Id.settings:
                    SupportFragmentManager.BeginTransaction().AddToBackStack("settings")
                      .Replace(Resource.Id.fragmentZone, new SettingsFragment()).Commit();
                    break;
                case Resource.Id.info:

                    break;
                case Resource.Id.exit:
                    ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this); ;
                    ISharedPreferencesEditor editor = prefs.Edit();
                    editor.PutBoolean("check", false);
                    editor.Apply();
                    Intent authoriz = new Intent(this, typeof(AuthorizationActivity));
                    StartActivity(authoriz);
                    Finish();
                    break;
                default:
                    return base.OnOptionsItemSelected(menuItem);
            }
            mDrawerLayout.CloseDrawer(navigationView);

            return true;
        }



        //private void SetUpDrawerContent(NavigationView navigationView)
        //{
        //    navigationView.NavigationItemSelected += (object sender, NavigationView.NavigationItemSelectedEventArgs e) =>
        //    {
        //        e.MenuItem.SetChecked(true);
        //        mDrawerLayout.CloseDrawers();
        //    };
        //}

        public override void OnBackPressed()
        {
            if (mDrawerLayout.IsDrawerOpen(navigationView))
            {
                mDrawerLayout.CloseDrawer(navigationView);
                return;
            }
            SupportFragmentManager.PopBackStack();
            base.OnBackPressed();
        }

    }
}