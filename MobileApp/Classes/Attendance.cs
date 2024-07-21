using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MobileApp.ApiAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileApp
{
    public struct Attendance
    {
        public int IdNumber { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string FullName
        {
            get
            {
                return $"{FirstName} {LastName}";
            }
        }
        public DateTime DateTime { get; private set; }

        public Attendance(int idNumber, string firstName, string lastName, DateTime dateTime)
        {
            IdNumber = idNumber;
            FirstName = firstName;
            LastName = lastName;
            DateTime = dateTime;
        }
    }
    public class AttendanceList : List<Attendance>
    {
        private int IdNumber;

        private AttendanceList()
        {

        }

        public AttendanceList(int idNumber)
        {
            IdNumber = idNumber;
        }

        public new void Add(Attendance attendance)
        {
            base.Add(attendance);
        }

        public async Task<Response> TryRefreshAsync()
        {
            var attendances = await AttendanceAccess.GetUserAttendance(IdNumber);
            if (attendances == null)
                return Response.Fail;

            Clear();
            foreach(Attendance attendance in attendances)
            {
                Add(attendance);
            }
            return Response.Success;
        }
    }
}