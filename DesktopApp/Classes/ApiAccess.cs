using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Web.Http;
using uwp = Windows.Storage.Streams;

namespace DesktopApp
{
    namespace ApiAccess
    {
        internal static class UserAccess
        {
            public async static Task<List<User>> GetAllUsers()
            {
                List<User> users = new List<User>();

                string url = "IUserAccount/ListUserAccount.php";

                string response = await Connection.Get(url);
                JsonArray.TryParse(response, out JsonArray array);
                
                foreach (var element in array)
                {
                    var entry = element.GetObject();
                    int.TryParse(entry["user_id"].GetNumber().ToString(), out int id);
                    string firstName = entry["first_name"].GetString();
                    string lastName = entry["last_name"].GetString();
                    string uname = entry["username"].GetString();
                    DateTime registration = ConvertToDateTime(entry["registration"].GetString());
                    AccountType type;
                    if (entry["account_type"].GetString() == "system")
                        type = AccountType.System;
                    else
                        type = AccountType.User;
                    User user = new User(id, firstName, lastName, uname, registration, type);
                    users.Add(user);
                }
                
                return users;
            }

            public async static Task<User> GetUser(int idNumber)
            {
                string url = $"IUserAccount/ListUserAccount.php?user_id={idNumber}";

                string response = await Connection.Get(url);
                JsonObject.TryParse(response, out JsonObject entry);
                
                int.TryParse(entry["user_id"].GetNumber().ToString(), out int id);
                string firstName = entry["first_name"].GetString();
                string lastName = entry["last_name"].GetString();
                string uname = entry["username"].GetString();
                DateTime registration = ConvertToDateTime(entry["registration"].GetString());
                AccountType type;
                if (entry["account_type"].GetString() == "system")
                    type = AccountType.System;
                else
                    type = AccountType.User;
                
                User user = new User(id, firstName, lastName, uname, registration, type);
                return user;
            }

            public async static Task<Response> AddUser(User user, string password)
            {
                string url = "IUserAccount/CreateUserAccount.php";

                JsonObject data = new JsonObject();
                data.SetNamedValue("username", JsonValue.CreateStringValue(user.Username));
                data.SetNamedValue("password", JsonValue.CreateStringValue(password));
                data.SetNamedValue("first_name", JsonValue.CreateStringValue(user.FirstName));
                data.SetNamedValue("last_name", JsonValue.CreateStringValue(user.LastName));
                data.SetNamedValue("account_type", JsonValue.CreateStringValue(user.AccountType.ToString().ToLower()));
                
                string response = await Connection.Post(url, data);
                if (response == "OK")
                    return Response.Success;
                else
                    return Response.Fail;
            }

            public async static Task<Response> UpdateUser(User user)
            {
                string url = "IUserAccount/UpdateUserAccount.php";
                JsonObject data = new JsonObject();
                data.SetNamedValue("user_id", JsonValue.CreateNumberValue(user.IdNumber));
                data.SetNamedValue("username", JsonValue.CreateStringValue(user.Username));
                data.SetNamedValue("first_name", JsonValue.CreateStringValue(user.FirstName));
                data.SetNamedValue("last_name", JsonValue.CreateStringValue(user.LastName));
                data.SetNamedValue("account_type", JsonValue.CreateStringValue(user.AccountType.ToString().ToLower()));
                string response = await Connection.Post(url, data);

                if (response == "OK")
                    return Response.Success;
                else
                    return Response.Fail;
            }

            public async static Task<Response> UpdatePassword(User user, string newPassword)
            {
                string url = "IUserAccount/UpdatePassword.php";
                JsonObject data = new JsonObject();
                data.SetNamedValue("user_id", JsonValue.CreateNumberValue(user.IdNumber));
                data.SetNamedValue("new_password", JsonValue.CreateStringValue(newPassword));
                string response = await Connection.Post(url, data);

                if (response == "OK")
                    return Response.Success;
                else
                    return Response.Fail;
            }

            public async static Task<Response> UpdatePassword(User user, string oldPassword, string newPassword)
            {
                string url = "IUserAccount/UpdatePassword.php";
                JsonObject data = new JsonObject();
                data.SetNamedValue("user_id", JsonValue.CreateNumberValue(user.IdNumber));
                data.SetNamedValue("current_password", JsonValue.CreateStringValue(oldPassword));
                data.SetNamedValue("new_password", JsonValue.CreateStringValue(newPassword));
                string response = await Connection.Post(url, data);

                if (response == "OK")
                    return Response.Success;
                else
                    return Response.Fail;
            }

            public async static Task<Response> PairCard(User user, Card card)
            {
                string url = "IUserAccount/PairCard.php";
                JsonObject data = new JsonObject();
                data.SetNamedValue("user_id", JsonValue.CreateNumberValue(user.IdNumber));
                data.SetNamedValue("serial_no", JsonValue.CreateStringValue(card.SerialNo));
                string response = await Connection.Post(url, data);

                if (response == "OK")
                    return Response.Success;
                else
                    return Response.Fail;
            }

            public async static Task<User> Authenticate(string username, string password)
            {
                string url = "IUserAccount/AuthenticateUserAccount.php";

                JsonObject data = new JsonObject();
                data.SetNamedValue("username", JsonValue.CreateStringValue(username));
                data.SetNamedValue("password", JsonValue.CreateStringValue(password));
                
                string response = await Connection.Post(url, data);

                if (response == "ERROR")
                    return null;
                else if (response == null)
                    return null;

                var rootObject = JsonObject.Parse(response);
                int.TryParse(rootObject["user_id"].GetNumber().ToString(), out int id);
                string firstName = rootObject["first_name"].GetString();
                string lastName = rootObject["last_name"].GetString();
                string uname = rootObject["username"].GetString();
                DateTime registration = ConvertToDateTime(rootObject["registration"].GetString());
                AccountType type;
                if (rootObject["account_type"].GetString() == "system")
                    type = AccountType.System;
                else
                    type = AccountType.User;

                User user = new User(id, firstName, lastName, uname, registration, type);
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
                List<Attendance> attendances = new List<Attendance>();

                string url = $"IAttendanceLog/ListLog.php?user_id={idNumber}";

                string response = await Connection.Get(url);
                JsonArray.TryParse(response, out JsonArray array);

                foreach (var element in array)
                {
                    var entry = element.GetObject();
                    int.TryParse(entry["user_id"].GetNumber().ToString(), out int id);
                    string firstName = entry["first_name"].GetString();
                    string lastName = entry["last_name"].GetString();
                    DateTime log = ConvertToDateTime(entry["date_time"].GetString());
                    
                    Attendance attendance = new Attendance(id, firstName, lastName, log);
                    attendances.Add(attendance);
                }
                
                return attendances;
            }

            public async static Task<List<Attendance>> GetAllAttendance()
            {
                List<Attendance> attendances = new List<Attendance>();

                string url = $"IAttendanceLog/ListLog.php";

                string response = await Connection.Get(url);
                JsonArray.TryParse(response, out JsonArray array);

                foreach (var element in array)
                {
                    var entry = element.GetObject();
                    int.TryParse(entry["user_id"].GetNumber().ToString(), out int id);
                    string firstName = entry["first_name"].GetString();
                    string lastName = entry["last_name"].GetString();
                    DateTime log = ConvertToDateTime(entry["date_time"].GetString());

                    Attendance attendance = new Attendance(id, firstName, lastName, log);
                    attendances.Add(attendance);
                }

                return attendances;
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
                HttpClient httpClient = new HttpClient();
                string responseBody = null;

                try
                {
                    Uri requestUri = new Uri($"http://{host}/{url}");
                    HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(requestUri);
                    httpResponseMessage.EnsureSuccessStatusCode();
                    responseBody = await httpResponseMessage.Content.ReadAsStringAsync();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
                finally
                {
                    httpClient.Dispose();
                }
                return responseBody;
            }
            internal async static Task<string> Post(string url, JsonObject postData)
            {
                return await Connection.Post(url, new HttpStringContent(postData.Stringify(), uwp.UnicodeEncoding.Utf8, "application/json"));
            }
            internal async static Task<string> Post(string url, HttpStringContent postData)
            {
                HttpClient httpClient = new HttpClient();
                string httpResponse = null;
                try
                {
                    Uri uri = new Uri($"http://{host}/{url}");
                    HttpResponseMessage httpResponseMessage = await httpClient.PostAsync(uri, postData);
                    httpResponseMessage.EnsureSuccessStatusCode();
                    httpResponse = await httpResponseMessage.Content.ReadAsStringAsync();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
                finally
                {
                    httpClient.Dispose();
                }
                return httpResponse;
            }
        }

        internal enum Response
        {
            Success,
            Fail
        }
    }
}
