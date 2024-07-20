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

                if (response == null)
                    return null;

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

                if (response == null)
                    return null;

                bool parseSuccess = JsonObject.TryParse(response, out JsonObject entry);
                if (!parseSuccess)
                    return null;

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
                
                HttpForm form = new HttpForm();
                form.Add("username", user.Username);
                form.Add("password", password);
                form.Add("first_name", user.FirstName);
                form.Add("last_name", user.LastName);
                form.Add("account_type", user.AccountType.ToString().ToLower());

                string response = await Connection.Post(url, form);

                if (response == "OK")
                    return Response.Success;
                else
                    return Response.Fail;
            }

            public async static Task<Response> UpdateUser(User user)
            {
                string url = "IUserAccount/UpdateUserAccount.php";

                HttpForm form = new HttpForm();
                form.Add("user_id", user.IdNumber.ToString());
                form.Add("username", user.Username);
                form.Add("first_name", user.FirstName);
                form.Add("last_name", user.LastName);
                form.Add("account_type", user.AccountType.ToString().ToLower());

                string response = await Connection.Post(url, form);

                if (response == "OK")
                    return Response.Success;
                else
                    return Response.Fail;
            }

            public async static Task<Response> UpdatePassword(User user, string newPassword)
            {
                string url = "IUserAccount/UpdatePassword.php";

                HttpForm form = new HttpForm();
                form.Add("user_id", user.IdNumber.ToString());
                form.Add("new_password", newPassword);

                string response = await Connection.Post(url, form);

                if (response == "OK")
                    return Response.Success;
                else
                    return Response.Fail;
            }

            public async static Task<Response> UpdatePassword(User user, string oldPassword, string newPassword)
            {
                string url = "IUserAccount/UpdatePassword.php";

                HttpForm form = new HttpForm();
                form.Add("user_id", user.IdNumber.ToString());
                form.Add("current_password", oldPassword);
                form.Add("new_password", newPassword);

                string response = await Connection.Post(url, form);

                if (response == "OK")
                    return Response.Success;
                else
                    return Response.Fail;
            }

            public async static Task<Response> PairCard(User user, Card card)
            {
                string url = "IUserAccount/PairCard.php";

                HttpForm form = new HttpForm();
                form.Add("user_id", user.IdNumber.ToString());
                form.Add("serial_no", card.SerialNo);

                string response = await Connection.Post(url, form);

                if (response == "OK")
                    return Response.Success;
                else
                    return Response.Fail;
            }

            public async static Task<User> Authenticate(string username, string password)
            {
                string url = "IUserAccount/AuthenticateUserAccount.php";

                HttpForm form = new HttpForm();
                form.Add("username", username);
                form.Add("password", password);
                
                string response = await Connection.Post(url, form);

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

                if (response == null)
                    return null;

                bool parseSucess = JsonArray.TryParse(response, out JsonArray array);
                if (!parseSucess)
                    return null;

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

                if (response == null)
                    return null;

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
            static string host = "mcl-ccis.site";

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
            [Obsolete]
            internal async static Task<string> Post(string url, JsonObject postData)
            {
                return await Connection.Post(url, new HttpStringContent(postData.Stringify(), uwp.UnicodeEncoding.Utf8, "application/json"));
            }
            [Obsolete]
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
            internal async static Task<string> Post(string url, HttpForm postData)
            {
                string httpResponse = null;
                HttpClient httpClient = new HttpClient();

                try
                {
                    Uri uri = new Uri($"http://{host}/{url}");
                    HttpFormUrlEncodedContent form = new HttpFormUrlEncodedContent(postData);

                    HttpResponseMessage httpResponseMessage = await httpClient.PostAsync(uri, form);
                    httpResponse = await httpResponseMessage.Content.ReadAsStringAsync();
                    httpResponseMessage.EnsureSuccessStatusCode();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    return null;
                }
                finally
                {
                    httpClient.Dispose();
                }

                return httpResponse;
            }
        }
        internal class HttpForm : List<KeyValuePair<string, string>>
        {
            public HttpForm()
            {

            }
            public void Add(string key, string value)
            {
                base.Add(new KeyValuePair<string, string>(key, value));
            }
            private new void Add(KeyValuePair<string, string> pair)
            {
                base.Add(pair);
            }
        }

        public enum Response
        {
            Success,
            Fail
        }
    }
}
