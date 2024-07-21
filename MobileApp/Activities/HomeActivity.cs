using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Java.Lang;
using MobileApp.ApiAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobileApp
{
    [Activity(Label = "Attendance")]
    public class HomeActivity : Activity
    {
        public static User MyUserAccount;
        private static int MyUserAccountId;
        AttendanceList attendance;

        RecyclerView AttendanceRecyclerView;
        RecyclerView.LayoutManager AttendanceViewLayoutManager;
        AttendanceViewListAdapter adapter;

        Button ViewButton;
        Button RefreshBtn;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.home);

            string fullName = Intent.GetStringExtra("FullName");
            string username = Intent.GetStringExtra("Username");
            string idNumber = Intent.GetStringExtra("IdNumber");
            
            var greetingTextView = FindViewById<TextView>(Resource.Id.textViewGreeting);
            greetingTextView.Text = $"Hello, {fullName}";

            ViewButton = FindViewById<Button>(Resource.Id.buttonViewAccount);
            ViewButton.Click += (sender, e) =>
            {
                Intent intent = new Intent(this, typeof(MyAccountActivity));
                intent.PutExtra("FullName", fullName);
                intent.PutExtra("Username", username);
                intent.PutExtra("IdNumber", idNumber);
                StartActivity(intent);
            };
            
            RefreshBtn = FindViewById<Button>(Resource.Id.buttonViewRefresh);
            RefreshBtn.Click += RefreshBtn_Click;

            MyUserAccountId = int.Parse(idNumber);
            attendance = new AttendanceList(MyUserAccountId);

            AttendanceRecyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerViewAttendance);
            AttendanceViewLayoutManager = new LinearLayoutManager(this);
            AttendanceRecyclerView.SetLayoutManager(AttendanceViewLayoutManager);

            Refresh();
        }

        private void RefreshBtn_Click(object sender, EventArgs e)
        {
            Refresh();
        }

        private async void Refresh()
        {
            var response = await attendance.TryRefreshAsync();

            if (response == Response.Success)
            {
                adapter = new AttendanceViewListAdapter(attendance);
                AttendanceRecyclerView.SetAdapter(adapter);
            }
            else
            {
                Toast.MakeText(this, "Failed loading user attendance", ToastLength.Long).Show();
            }
        }
    }

    public class AttendanceViewHolder : RecyclerView.ViewHolder
    {
        public TextView AttendanceDateTime { get; private set; }

        public AttendanceViewHolder(View itemView) : base(itemView)
        {
            // Locate and cache view references:
            AttendanceDateTime = itemView.FindViewById<TextView>(Resource.Id.textViewCardDateTime);
        }
    }
    public class AttendanceViewListAdapter : RecyclerView.Adapter
    {
        public AttendanceList mAttendanceList;
        public AttendanceViewListAdapter(AttendanceList attendanceList)
        {
            mAttendanceList = attendanceList;
        }
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            // Inflate the CardView
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.card_attendance, parent, false);

            // Create a ViewHolder to hold view references inside the CardView:
            AttendanceViewHolder vh = new AttendanceViewHolder(itemView);
            return vh;
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            AttendanceViewHolder vh = holder as AttendanceViewHolder;

            // Load the date time to card view.
            vh.AttendanceDateTime.Text = mAttendanceList[position].DateTime.ToString();
        }
        public override int ItemCount
        {
            get { return mAttendanceList.Count; }
        }
    }
}