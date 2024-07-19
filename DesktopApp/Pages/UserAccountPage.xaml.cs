using DesktopApp.ApiAccess;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DesktopApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class UserAccountPage : Page
    {
        User user;
        AttendanceList userAttendance;

        public UserAccountPage()
        {
            this.InitializeComponent();
            userAttendance = new AttendanceList();
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            var obj = sender as AppBarButton;
            var tag = obj.Tag as string;

            switch (tag)
            {
                case "Refresh":
                    Refresh();
                    break;
                case "Update":
                    UpdateInfo();
                    break;
                case "Reset":
                    UpdatePassword();
                    break;
                case "Pair":
                    UpdateCard();
                    break;
            }
        }

        private async void UpdateCard()
        {
            try
            {
                PairCardDialog dialog = new PairCardDialog(user);
                var result = await dialog.ShowAsync();

                if (result == ContentDialogResult.Primary)
                {
                    PairedTip.IsOpen = true;
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine(ex.Message);
#endif
                MessageDialog error = new MessageDialog(ex.Message, "Runtime Error");
                await error.ShowAsync();
            }
        }

        private async void UpdateInfo()
        {
            try
            {
                UpdateUserDialog dialog = new UpdateUserDialog(user);
                var result = await dialog.ShowAsync();
                if (result == ContentDialogResult.Primary)
                {
                    Refresh();
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine(ex.Message);
#endif
                MessageDialog error = new MessageDialog(ex.Message, "Runtime Error");
                await error.ShowAsync();
            }
        }

        private async void UpdatePassword()
        {
            try
            {
                bool isCurrentUser = false;
                if (AppPage.MyAccount.IdNumber == user.IdNumber)
                {
                    isCurrentUser = true;
                }
                ResetPasswordDialog dialog = new ResetPasswordDialog(user, isCurrentUser);
                var result = await dialog.ShowAsync();

                if (result == ContentDialogResult.Primary)
                {
                    PassChangedTip.IsOpen = true;
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine(ex.Message);
#endif
                MessageDialog error = new MessageDialog(ex.Message, "Runtime Error");
                await error.ShowAsync();
            }
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Load(e.Parameter);
            base.OnNavigatedTo(e);
        }

        private async void Refresh()
        {
            try
            {
                Response userResponse = Response.Success;
                userResponse = await user.Load();
                if (userResponse == Response.Fail)
                {
                    ErrorDialog error = new ErrorDialog("Failed loading user data.");
                    await error.ShowAsync();
                }

                if (userResponse == Response.Success)
                {
                    FullnameText.Text = user.FullName;
                    UsernameText.Text = $"Username: {user.Username}";
                    IdNumberText.Text = $"ID Number: {user.IdNumber.ToString()}";
                    AccountTypeText.Text = $"Account Type: {user.AccountType}";

                    userAttendance = new AttendanceList(user.IdNumber);
                }
                
                var result = await userAttendance.TryRefreshAsync(user.IdNumber);
                if (result == Response.Fail)
                {
                    ErrorDialog error = new ErrorDialog("Failed to refresh data.");
                    await error.ShowAsync();
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine(ex.Message);
#endif
                MessageDialog error = new MessageDialog(ex.Message, "Runtime Error");
                await error.ShowAsync();
            }
        }

        private async void Load(object parameter)
        {
            try
            {
                if (parameter is User)
                {
                    user = parameter as User;
                }
                else if (parameter is int)
                {
                    user = new User((int)parameter);
                }
                Refresh();
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine(ex.Message);
#endif
                MessageDialog error = new MessageDialog(ex.Message, "Runtime Error");
                await error.ShowAsync();
            }
        }
    }
}
