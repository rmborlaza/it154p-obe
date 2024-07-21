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
        EditText Username;
        EditText Password;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
            
            LoginBtn = FindViewById<Button>(Resource.Id.LoginBtn);
            LoginBtn.Click += LoginBtn_Click;

            Username = FindViewById<EditText>(Resource.Id.LoginUsernameBox);
            Password = FindViewById<EditText>(Resource.Id.LoginPasswordBox);
        }

        private async void LoginBtn_Click(object sender, System.EventArgs e)
        {
            string username = Username.Text;
            string password = Password.Text;

            try
            {
                var user = await UserAccess.Authenticate(username, password);

                if (user != null)
                {
                    Intent intent = new Intent(this, typeof(HomeActivity));
                    intent.PutExtra("FullName", $"{user.FirstName} {user.LastName}");
                    intent.PutExtra("Username", user.Username);
                    intent.PutExtra("IdNumber", user.IdNumber.ToString());
                    StartActivity(intent);
                }
                else
                {
                    Toast.MakeText(this, "Invalid username or password", ToastLength.Short).Show();
                }
            }
            catch
            {
                Toast.MakeText(this, "Runtime error", ToastLength.Short).Show();
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}