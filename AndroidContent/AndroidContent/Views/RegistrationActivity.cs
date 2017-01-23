using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AllContent_Client;

namespace AndroidContent.Views
{
    [Activity(Label = "RegistrationActivity")]
    public class RegistrationActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.RegistrationLayout);
            var enter = FindViewById<Button>(Resource.Id.Reg);
            enter.Click += EnterClick;
        }
        void EnterClick(object sender, EventArgs e)
        {
            string pattern = @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" + @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$";
            string log = FindViewById<EditText>(Resource.Id.Login).Text;
            string passw = FindViewById<EditText>(Resource.Id.Password).Text;
            string email = FindViewById<EditText>(Resource.Id.Email).Text;
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            AlertDialog error_dialog = builder.Create();
            if (log.Length <= 4)
            {
                error_dialog.SetTitle("Слишком короткий логин");
                error_dialog.SetMessage("В поле для логина должно быть более четырех символов");
                error_dialog.Show();
            }
            else if (log.Length <= 4)
            {
                error_dialog.SetTitle("Слишком короткий пароль");
                error_dialog.SetMessage("В поле для пароля должно быть более четырех символов");
                error_dialog.Show();
            }
            else if (!Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase))
            {
                error_dialog.SetTitle("Некорректный E-mail");
                error_dialog.SetMessage("Пример: hahaha@lol.kek");
                error_dialog.Show();
            }
            else
            {
                if (!User.Registration(log, passw, email))
                {
                    error_dialog.SetTitle("Логин занят");
                    error_dialog.SetMessage("Пользователь с таким логином уже зарегистрирован");
                    error_dialog.Show();
                }
                else
                {
                    var back = new Intent(this, typeof(AuthorizationActivity));
                    StartActivity(back);
                }
            }
        }
    }
}