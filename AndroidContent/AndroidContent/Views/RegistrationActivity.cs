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
    [Activity(Label = "RegistrationActivity", Theme = "@style/Theme.DesignDemo")]
    public class RegistrationActivity : Activity
    {
        const string pattern = @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" + @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$";
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.RegistrationLayout);
            var enter = FindViewById<Button>(Resource.Id.Continue);
            enter.Click += EnterClick;
        }
        void EnterClick(object sender, EventArgs e)
        {
            ProgressDialog mProgressDialog = new ProgressDialog(this);
            mProgressDialog.SetMessage("��������");
            mProgressDialog.SetProgressStyle(ProgressDialogStyle.Spinner);
            mProgressDialog.Show();

            string log = FindViewById<EditText>(Resource.Id.txtLogin).Text;
            string passw = FindViewById<EditText>(Resource.Id.txtPassword).Text;
            string email = FindViewById<EditText>(Resource.Id.txtemail).Text;

            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            AlertDialog error_dialog = builder.Create();
            if (log.Length <= 4)
            {
                error_dialog.SetTitle("������� �������� �����");
                error_dialog.SetMessage("� ���� ��� ������ ������ ���� ����� ������� ��������");
                error_dialog.Show();
                mProgressDialog.Hide();
            }
            else if (log.Length <= 4)
            {
                error_dialog.SetTitle("������� �������� ������");
                error_dialog.SetMessage("� ���� ��� ������ ������ ���� ����� ������� ��������");
                error_dialog.Show();
                mProgressDialog.Hide();
            }
            else if (!Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase))
            {
                error_dialog.SetTitle("������������ E-mail");
                error_dialog.SetMessage("������: hahaha@lol.kek");
                error_dialog.Show();
                mProgressDialog.Hide();
            }
            else
            {
                if (!User.Registration(log, passw, email))
                {
                    error_dialog.SetTitle("����� �����");
                    error_dialog.SetMessage("������������ � ����� ������� ��� ���������������");
                    error_dialog.Show();
                    mProgressDialog.Hide();
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