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
using MobileApp.ApiAccess;

namespace MobileApp
{
    [Activity(Label = "Change Password")]
    public class ChangePasswordActivity : Activity
    {
        EditText currentPasswordBox;
        EditText newPasswordBox;
        EditText confirmPasswordBox;
        Button changePassBtn;
        Button cancelBtn;
        string username;
        string idNumber;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.change_password);

            username = Intent.GetStringExtra("Username");
            idNumber = Intent.GetStringExtra("IdNumber");

            currentPasswordBox = FindViewById<EditText>(Resource.Id.CurrentPasswordBox);
            newPasswordBox = FindViewById<EditText>(Resource.Id.NewPasswordBox);
            confirmPasswordBox = FindViewById<EditText>(Resource.Id.ConfirmPasswordBox);
            changePassBtn = FindViewById<Button>(Resource.Id.ChangePassBtn);
            changePassBtn.Click += ChangePassBtn_Click;

            cancelBtn = FindViewById<Button>(Resource.Id.CancelBtn);
            cancelBtn.Click += CancelBtn_Click;
        }
        
        private async void ChangePassBtn_Click(object sender, EventArgs e)
        {
            string currentPassword = currentPasswordBox.Text;
            string newPassword = newPasswordBox.Text;
            string confirmPassword = confirmPasswordBox.Text;

            if (string.IsNullOrWhiteSpace(currentPassword) || string.IsNullOrWhiteSpace(newPassword) || string.IsNullOrWhiteSpace(confirmPassword))
            {
                Toast.MakeText(this, "Please fill in all fields.", ToastLength.Short).Show();
                return;
            }

            if (newPassword.Length < 6)
            {
                Toast.MakeText(this, "New password must be at least 6 characters long.", ToastLength.Short).Show();
                return;
            }

            if (newPassword != confirmPassword)
            {
                Toast.MakeText(this, "New passwords do not match.", ToastLength.Short).Show();
                return;
            }

            User user = new User(int.Parse(idNumber));
            await user.Load();

            var response = await UserAccess.UpdatePassword(user, currentPassword, newPassword);

            if (response == Response.Success)
            {
                Toast.MakeText(this, "Password changed successfully.", ToastLength.Short).Show();
                Finish();
            }
            else
            {
                Toast.MakeText(this, "Failed to change password. Please try again.", ToastLength.Short).Show();
            }
        }

        private void CancelBtn_Click(object sender, EventArgs e)
        {
            Finish();
        }
    }
}