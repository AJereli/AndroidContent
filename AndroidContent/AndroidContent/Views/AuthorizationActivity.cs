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

namespace AndroidContent.Views
{
    [Activity(Label = "AuthorizationActivity", MainLauncher = true)]
    public class AuthorizationActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Authorizationlayout);
            var enter = FindViewById<Button>(Resource.Id.Enter);
            enter.Click += EnterClick;
            var reg = FindViewById<TextView>(Resource.Id.Registration);
            reg.Click += RegClick;
        }
        void RegClick(object sender, EventArgs e)
        {
            var reg = new Intent(this, typeof(RegistrationActivity));
            StartActivity(reg);
        }
        void EnterClick(object sender, EventArgs e)
        {
            string log = FindViewById<EditText>(Resource.Id.Login).Text;
            string passw = FindViewById<EditText>(Resource.Id.Password).Text;
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            AlertDialog error_dialog = builder.Create();
            User user = new User();
            try
            {
                if (user.Authorization(log, passw))
                {
                    //MainView.user = this.user;
                    var main = new Intent(this, typeof(MainActivity));
                    StartActivity(main);
                }
                else
                {
                    error_dialog.SetTitle("Не правильный логин или пароль");
                    error_dialog.SetMessage("Проверьте данные и повторите попытку");
                    error_dialog.Show();
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException exp)
            {
                if (exp.Number == 1024)
                {
                    error_dialog.SetTitle("Ошибка подключения");
                    error_dialog.SetMessage("Сервер временно недоступен");
                    error_dialog.Show();
                }                    
            }
        }
    }
}