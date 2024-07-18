using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using AndroidX.AppCompat.App;
using MobileApp.ApiAccess;

namespace MobileApp
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        Button LoginBtn;
        EditText UsernameText, PasswordText;

        Button TestButton;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            UsernameText = FindViewById<EditText>(Resource.Id.LoginUsernameBox);
            PasswordText = FindViewById<EditText>(Resource.Id.LoginPasswordBox);

            LoginBtn = FindViewById<Button>(Resource.Id.LoginBtn);
            LoginBtn.Click += LoginBtn_Click;
            // Test Button
            TestButton = FindViewById<Button>(Resource.Id.TestBtn);
            TestButton.Click += TestButton_Click;
        }

        private async void LoginBtn_Click(object sender, System.EventArgs e)
        {
            string username = UsernameText.Text;
            string password = PasswordText.Text;

            try
            {
                var result = await UserAccess.Authenticate(username, password);

                if (result != null)
                {
                    Intent i = new Intent(this, typeof(HomeActivity));
                    i.PutExtra("UserId", result.IdNumber);
                    StartActivity(i);
                }
                else
                {
                    Toast.MakeText(this, "Error logging in", ToastLength.Short).Show();
                }
            }
            catch
            {
                Toast.MakeText(this, "Error logging in", ToastLength.Short).Show();
            }
        }

        // Test function
        private async void TestButton_Click(object sender, System.EventArgs e)
        {
            var result = await AttendanceAccess.GetUserAttendance(789);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }


    }
}