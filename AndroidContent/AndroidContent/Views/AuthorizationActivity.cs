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
using AllContent_Client;
using Android.Support.Design.Widget;

namespace AndroidContent.Views
{
    [Activity(Label = "AndroidContent", MainLauncher = true, Icon = "@drawable/icon", Theme = "@style/Theme.DesignDemo")]
    public class AuthorizationActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Authorizationlayout);
            var enter = FindViewById<Button>(Resource.Id.Enter);
            enter.Click += EnterClick;
            
            var reg = FindViewById<TextView>(Resource.Id.TVRegistration);
            reg.Click += RegClick;
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
            mProgressDialog.SetMessage("Загрузка");
            mProgressDialog.SetProgressStyle(ProgressDialogStyle.Spinner);
            mProgressDialog.Show();
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            AlertDialog error_dialog = builder.Create();
            string log = FindViewById<EditText>(Resource.Id.txtLogin).Text;
            string passw = FindViewById<EditText>(Resource.Id.txtPassword).Text;
            User user = new User();
            TextInputLayout passwordWrapper = FindViewById<TextInputLayout>(Resource.Id.txtInputLayoutPassword);
            try
            {
                if (user.Authorization(log, passw))
                {
                    //MainView.user = this.user;
                    var main = new Intent(this, typeof(MainActivity));
                    StartActivity(main);
                    Finish();
                }
                else
                {
                    error_dialog.SetTitle("Ошибка авторизации");
                    error_dialog.SetMessage("Неправильный логин или пароль");
                    error_dialog.Show();
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException exp)
            {
                if (exp.Number == 1024)
                {
                    error_dialog.SetTitle("Проблема подключением");
                    error_dialog.SetMessage("Сервер не отвечает");
                    error_dialog.Show();
                }                    
            }
        }
    }
}