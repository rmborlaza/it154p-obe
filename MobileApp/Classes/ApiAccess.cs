using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Org.Apache.Http.Client.Params;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MobileApp.ApiAccess
{
    internal static class UserAccess
    {
        public async static Task<User> GetUser(int idNumber)
        {
            string url = $"IUserAccount/ListUserAccount.php?user_id={idNumber}";

            var result = await Connection.Get(url);

            if (result == "ERROR")
                return null;
            else if (result == null)
                return null;

            using var doc = JsonDocument.Parse(result);
            JsonElement accounts = doc.RootElement;

            accounts.GetProperty("user_id").TryGetInt32(out int userId);
            string firstName = accounts.GetProperty("first_name").GetString();
            string lastName = accounts.GetProperty("last_name").GetString();
            string uname = accounts.GetProperty("username").GetString();
            string registrationStr = accounts.GetProperty("registration").GetString();
            string typeStr = accounts.GetProperty("account_type").GetString();

            DateTime registration = ConvertToDateTime(registrationStr);

            AccountType type;
            if (typeStr == "system")
                type = AccountType.System;
            else
                type = AccountType.User;

            User user = new User(userId, firstName, lastName, uname, registration, type);

            return user;
        }
        public async static Task<Response> UpdatePassword(User user, string oldPassword, string newPassword)
        {
            return Response.Fail;
        }
        public async static Task<User> Authenticate(string username, string password)
        {
            string url = "IUserAccount/AuthenticateUserAccount.php";

            Dictionary<string, string> postData = new Dictionary<string, string>
            {
                {"username",username },
                {"password",password }
            };

            var result = await Connection.Post(url, postData);

            if (result == "ERROR")
                return null;
            else if (result == null)
                return null;

            using var doc = JsonDocument.Parse(result);
            JsonElement accounts = doc.RootElement;

            accounts.GetProperty("user_id").TryGetInt32(out int userId);
            string firstName = accounts.GetProperty("first_name").GetString();
            string lastName = accounts.GetProperty("last_name").GetString();
            string uname = accounts.GetProperty("username").GetString();
            string registrationStr = accounts.GetProperty("registration").GetString();
            string typeStr = accounts.GetProperty("account_type").GetString();

            DateTime registration = ConvertToDateTime(registrationStr);

            AccountType type;
            if (typeStr == "system")
                type = AccountType.System;
            else
                type = AccountType.User;

            User user = new User(userId, firstName, lastName, uname, registration, type);

            return user;
        }

        private static DateTime ConvertToDateTime(string dateTimeStr)
        {
            var split = dateTimeStr.Split(" ");
            var date = split[0];
            var time = split[1];

            var dateSplit = date.Split("-");
            var timeSplit = time.Split(":");

            var year = int.Parse(dateSplit[0]);
            var month = int.Parse(dateSplit[1]);
            var day = int.Parse(dateSplit[2]);

            var hour = int.Parse(timeSplit[0]);
            var minute = int.Parse(timeSplit[1]);
            var second = int.Parse(timeSplit[2]);

            DateTime formatted = new DateTime(year, month, day, hour, minute, second);
            return formatted;
        }
    }
    internal static class AttendanceAccess
    {
        public async static Task<List<Attendance>> GetUserAttendance(int idNumber)
        {
            return null;
        }
        private static DateTime ConvertToDateTime(string dateTimeStr)
        {
            var split = dateTimeStr.Split(" ");
            var date = split[0];
            var time = split[1];

            var dateSplit = date.Split("-");
            var timeSplit = time.Split(":");

            var year = int.Parse(dateSplit[0]);
            var month = int.Parse(dateSplit[1]);
            var day = int.Parse(dateSplit[2]);

            var hour = int.Parse(timeSplit[0]);
            var minute = int.Parse(timeSplit[1]);
            var second = int.Parse(timeSplit[2]);

            DateTime formatted = new DateTime(year, month, day, hour, minute, second);
            return formatted;
        }
    }

    internal static class Connection
    {
        static string host = "192.168.2.50/attendance";
        internal async static Task<string> Get(string url)
        {
            string result = null;

            HttpClient httpClient = new HttpClient();

            try
            {
                using (httpClient)
                {
                    Uri uri = new Uri($"http://{host}/{url}");
                    httpClient.Timeout = new TimeSpan(0, 0, 30);
                    var httpResponse = await httpClient.GetAsync(uri);
                    httpResponse.EnsureSuccessStatusCode();
                    result = await httpResponse.Content.ReadAsStringAsync();
                }
                return result;
            }
            catch
            {
                return null;
            }
            finally
            {
                httpClient.Dispose();
            }
        }
        
        internal async static Task<string> Post(string url, Dictionary<string, string> postData)
        {
            if (postData == null)
                return null;

            string result = null;
            HttpClient httpClient = new HttpClient();

            try
            {
                using (httpClient)
                {
                    Uri uri = new Uri($"http://{host}/{url}");
                    httpClient.Timeout = new TimeSpan(0, 0, 30);
                    var content = new FormUrlEncodedContent(postData);
                    var httpResponse = await httpClient.PostAsync(uri, content);
                    result = await httpResponse.Content.ReadAsStringAsync();
                }
                return result;
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                System.Diagnostics.Debug.WriteLine(error);
                return null;
            }
            finally
            {
                httpClient.Dispose();
            }
        }
    }

    public enum Response
    {
        Success,
        Fail
    }
}