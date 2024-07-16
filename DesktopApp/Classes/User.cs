using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using DesktopApp.ApiAccess;

namespace DesktopApp
{
    public class User
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
        public string Username { get; private set; }
        public DateTime Registration { get; private set; }
        public AccountType AccountType { get; private set; }
        public AttendanceList attendances { get; private set; }
        public Card Card { get; private set; }

        public User()
        {

        }

        public User(int idNumber)
        {
            IdNumber = idNumber;
        }

        public User(int idNumber, string firstName, string lastName, string username, DateTime registration, AccountType accountType)
        {
            IdNumber = idNumber;
            FirstName = firstName;
            LastName = lastName;
            Username = username;
            Registration = registration;
            AccountType = accountType;
        }

        public async Task Load()
        {
            User user = await UserAccess.GetUser(IdNumber);
            FirstName = user.FirstName;
            LastName = user.LastName;
            Username = user.Username;
            Registration = user.Registration;
            AccountType = user.AccountType;
            user = null;
        }

        public async Task Load(int idNumber)
        {
            User user = await UserAccess.GetUser(idNumber);
            IdNumber = user.IdNumber;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Username = user.Username;
            Registration = user.Registration;
            AccountType = user.AccountType;
            user = null;
        }

        public async void Update(string firstName, string  lastName, string username, AccountType accountType)
        {
            User user = new User(IdNumber, firstName, lastName, username, Registration, accountType);

            var response = await UserAccess.UpdateUser(user);
            if (response == Response.Fail)
            {
                ErrorDialog error = new ErrorDialog("Failed to update user information");
                await error.ShowAsync();
            }
            else
            {
                FirstName = user.FirstName;
                LastName = user.LastName;
                Username = user.Username;
                AccountType = accountType;
            }
        }

        public async void UpdatePassword(string oldPassword, string newPassword)
        {
            var response = await UserAccess.UpdatePassword(this, oldPassword, newPassword);
        }

        public async void PairCard(string serialNo)
        {
            Card newCard = new Card(serialNo);
            Card = newCard;
            var response = await UserAccess.PairCard(this, Card);
        }
    }

    public class UserList : ObservableCollection<User>
    {
        public new void Add(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (Contains(user))
            {
                throw new ArgumentException("This user is already in this collection", "user");
            }
            base.Add(user);
        }
        public new void Remove(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (!Contains(user))
            {
                throw new ArgumentException("This user is not in this collection", "user");
            }
            base.Remove(user);
        }
    }

    public enum AccountType
    {
        User,
        System
    }
}
