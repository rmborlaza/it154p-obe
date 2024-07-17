using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MobileApp.ApiAccess;

namespace MobileApp
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
                System.Diagnostics.Debug.WriteLine(ex.Message);
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
                //Debug.WriteLine(ex.Message);
                return Response.Fail;
            }
        }

        public async Task<Response> UpdatePassword(string newPassword, string oldPassword)
        {
            var response = await UserAccess.UpdatePassword(this, oldPassword, newPassword);
            return response;
        }
    }
    public enum AccountType
    {
        User,
        System
    }
}