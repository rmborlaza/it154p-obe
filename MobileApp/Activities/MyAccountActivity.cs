using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobileApp
{
    [Activity(Label = "My Account")]
    public class MyAccountActivity : Activity
    {
        User user;

        Button ChangeButton;
        TextView FullNameTextView;
        TextView UsernameTextView;
        TextView IdNumberTextView;
        
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.my_account);

            string fullName = Intent.GetStringExtra("FullName");
            string username = Intent.GetStringExtra("Username");
            string idNumber = Intent.GetStringExtra("IdNumber");
            
            FullNameTextView = FindViewById<TextView>(Resource.Id.textViewFullName);
            FullNameTextView.Text = $"Full Name: {fullName}";

            UsernameTextView = FindViewById<TextView>(Resource.Id.textViewUsername);
            UsernameTextView.Text = $"Username: {username}";

            IdNumberTextView = FindViewById<TextView>(Resource.Id.textViewIdNumber);
            IdNumberTextView.Text = $"ID Number: {idNumber}";

            ChangeButton = FindViewById<Button>(Resource.Id.buttonChangePassword);
            ChangeButton.Click += (sender, e) =>
            {
                Intent intent = new Intent(this, typeof(ChangePasswordActivity));
                intent.PutExtra("Username", username);
                intent.PutExtra("IdNumber", idNumber);
                StartActivity(intent);
            };
        }
    }
}