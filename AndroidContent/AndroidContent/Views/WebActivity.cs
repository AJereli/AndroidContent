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
using Android.Support.V4.App;

namespace AndroidContent
{
    [Activity(Label = "WebActivity")]
    public class WebActivity : FragmentActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            SetContentView(Resource.Layout.WebLayout);
            Android.Support.V4.App.FragmentManager fm = SupportFragmentManager;
            Android.Support.V4.App.Fragment fragment = fm.FindFragmentById(Resource.Id.WebLayout);

            if (fragment == null)
            {
                fragment = new ContentWebFragment();
                
                fm.BeginTransaction().Add(Resource.Id.WebLayout, fragment).Commit();
            }
        }
        
    }
}