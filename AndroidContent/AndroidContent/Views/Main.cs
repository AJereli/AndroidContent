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

namespace AndroidContent.Views
{
    [Activity(Label = "Main Page TEST", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : FragmentActivity
    {

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.MainLayout);
            Android.Support.V4.App.FragmentManager fm = SupportFragmentManager;
            Android.Support.V4.App.Fragment fragment = fm.FindFragmentById(Resource.Id.fragmentContainer);

            if (fragment == null)
            {
                fragment = new NewsListFragment();
                fm.BeginTransaction().Add(Resource.Id.fragmentContainer, fragment).Commit();
            }

        }

    }
}