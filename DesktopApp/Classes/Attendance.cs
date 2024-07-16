using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DesktopApp.ApiAccess;

namespace DesktopApp
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

    public class AttendanceList : ObservableCollection<Attendance>
    {
        private int IdNumber;
        public AttendanceList()
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

        public async void Refresh()
        {
            var attendances = await AttendanceAccess.GetAllAttendance();
            if (attendances == null)
                return;

            Clear();
            foreach (Attendance attendance in attendances)
            {
                Add(attendance);
            }
        }

        public async void Refresh(int idNumber)
        {
            var attendances = await AttendanceAccess.GetUserAttendance(idNumber);
            if (attendances == null)
                return;

            Clear();
            foreach (Attendance attendance in attendances)
            {
                Add(attendance);
            }
        }
    }
}
