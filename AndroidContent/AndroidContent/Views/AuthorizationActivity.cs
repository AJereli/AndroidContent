using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Preferences;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Accounts;
using AllContent_Client;

namespace AndroidContent.Views
{
    [Activity(Label = "AndroidContent", MainLauncher = true, Icon = "@drawable/icon", Theme = "@style/Theme.DesignDemo")]
    public class AuthorizationActivity : Activity
    {
        ISharedPreferences prefs;
        ISharedPreferencesEditor editor;
        Intent main;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            bool k = prefs.GetBoolean("check", false);
            if (k)
            {
                main = new Intent(this, typeof(MainActivity));
                User.Name = prefs.GetString("username", "");
                StartActivity(main);
                Finish();
            }
            SetContentView(Resource.Layout.Authorizationlayout);
            var enter = FindViewById<Button>(Resource.Id.Enter);
            enter.Click += EnterClick;

            var reg = FindViewById<TextView>(Resource.Id.TVRegistration);
            reg.Click += RegClick;
            #region test
            FindViewById<EditText>(Resource.Id.txtLogin).Text = "test";
            FindViewById<EditText>(Resource.Id.txtPassword).Text = "test";
            #endregion
        }
        void RegClick(object sender, EventArgs e)
        {
            var reg = new Intent(this, typeof(RegistrationActivity));
            StartActivity(reg);
            Finish();
        }
        void EnterClick(object sender, EventArgs e)
        {
            ProgressDialog mProgressDialog = new ProgressDialog(this);
            mProgressDialog.SetMessage("Загрузка...");
            mProgressDialog.SetProgressStyle(ProgressDialogStyle.Spinner);
            mProgressDialog.Show();
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            AlertDialog error_dialog = builder.Create();
            string log = FindViewById<EditText>(Resource.Id.txtLogin).Text;
            string passw = FindViewById<EditText>(Resource.Id.txtPassword).Text;
            try
            {
                if (User.MainUser.Authorization(log, passw))
                {
                    prefs = PreferenceManager.GetDefaultSharedPreferences(this);
                    editor = prefs.Edit();
                    editor.PutString("username", log);
                    editor.PutBoolean("check", true);
                    editor.Apply();
                    main = new Intent(this, typeof(MainActivity));                    
                    StartActivity(main);
                    Finish();
                }
                else
                {
                    error_dialog.SetTitle("Ошибка авторизации");
                    error_dialog.SetMessage("Неправильный логин или пароль");
                    error_dialog.Show();
                    mProgressDialog.Hide();
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException exp)
            {
                if (exp.Number == 1024)
                {
                    error_dialog.SetTitle("Проблема подключением");
                    error_dialog.SetMessage("Сервер не отвечает");
                    error_dialog.Show();
                    mProgressDialog.Hide();
                }
            }
        }
    }
}