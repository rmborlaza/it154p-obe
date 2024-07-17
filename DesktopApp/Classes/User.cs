using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using DesktopApp.ApiAccess;
using System.Diagnostics;

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

        public async Task<Response> Load()
        {
            try
            {
                User user = await UserAccess.GetUser(IdNumber);
                if (user == null)
                {
                    return Response.Fail;
                }
                else
                {
                    FirstName = user.FirstName;
                    LastName = user.LastName;
                    Username = user.Username;
                    Registration = user.Registration;
                    AccountType = user.AccountType;
                    user = null;
                    return Response.Success;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return Response.Fail;
            }
        }

        public async Task<Response> Load(int idNumber)
        {
            try
            {
                User user = await UserAccess.GetUser(idNumber);
                IdNumber = user.IdNumber;
                FirstName = user.FirstName;
                LastName = user.LastName;
                Username = user.Username;
                Registration = user.Registration;
                AccountType = user.AccountType;
                user = null;
                return Response.Success;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return Response.Fail;
            }
        }

        public async Task<Response> Update(string firstName, string lastName, string username, AccountType accountType)
        {
            User user = new User(IdNumber, firstName, lastName, username, Registration, accountType);

            var response = await UserAccess.UpdateUser(user);
            if (response == Response.Fail)
            {
                return Response.Fail;
            }
            else
            {
                FirstName = firstName;
                LastName = lastName;
                Username = username;
                AccountType = accountType;
                return Response.Success;
            }
        }

        public async Task<Response> UpdatePassword(string newPassword, string oldPassword)
        {
            var response = await UserAccess.UpdatePassword(this, oldPassword, newPassword);
            return response;
        }
        public async Task<Response> UpdatePassword(string newPassword)
        {
            var response = await UserAccess.UpdatePassword(this, newPassword);
            return response;
        }

        public async Task<Response> PairCard(string serialNo)
        {
            Card newCard = new Card(serialNo);
            Card = newCard;
            var response = await UserAccess.PairCard(this, Card);
            return response;
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
