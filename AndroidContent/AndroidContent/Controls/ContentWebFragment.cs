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
using Android.Webkit;
using AllContent_Client;

namespace AndroidContent
{
    public class ContentWebFragment : Android.Support.V4.App.Fragment
    {


        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View v = inflater.Inflate(Resource.Layout.WebLayout, container, false);
            
            WebView webView = v.FindViewById<WebView>(Resource.Id.WebView);
            webView.Settings.JavaScriptEnabled = true;

            webView.SetWebViewClient(new MyWebViewClient());
            string url = Activity.Intent.Data.ToString();
            webView.LoadUrl(url);
            return v;
        }

    }

  

    class MyWebViewClient : WebViewClient
    {
        public MyWebViewClient()
        {

        }

        [Obsolete]
        public override bool ShouldOverrideUrlLoading(WebView view, string url)
        {
            return false;
        }
    }
}