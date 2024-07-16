using DesktopApp.ApiAccess;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
            PairCardDialog dialog = new PairCardDialog();
            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                string serial = dialog.GetCardSerial();
                Card newCard = new Card(serial);
                var response = await UserAccess.PairCard(user, newCard);
                if (response == Response.Fail)
                {
                    ErrorDialog error = new ErrorDialog("Failed to pair card.");
                    await error.ShowAsync();
                }
                else
                {
                    PairedTip.IsOpen = true;
                }
            }
        }

        private async void UpdateInfo()
        {
            UpdateUserDialog dialog = new UpdateUserDialog(user);
            var result = await dialog.ShowAsync();
        }

        private async void UpdatePassword()
        {
            bool isCurrentUser = false;
            if (AppPage.myAccount.IdNumber == user.IdNumber)
            {
                isCurrentUser = true;
            }
            ResetPasswordDialog dialog = new ResetPasswordDialog(isCurrentUser);

            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                string newPass = dialog.NewPassword;

                var response = await UserAccess.UpdatePassword(user, newPass);
                if (response == Response.Fail)
                {
                    ErrorDialog error = new ErrorDialog("Failed updating password.");
                    await error.ShowAsync();
                }
                else
                {
                    PassChangedTip.IsOpen = true;
                }
            }
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Load(e.Parameter);
            base.OnNavigatedTo(e);
        }

        private void Refresh()
        {
            userAttendance.Refresh(user.IdNumber);
        }

        private async void Load(object parameter)
        {
            if (parameter is User)
            {
                user = parameter as User;

                FullnameText.Text = user.FullName;
                UsernameText.Text = $"Username: {user.Username}";
                IdNumberText.Text = $"ID Number: {user.IdNumber.ToString()}";
                AccountTypeText.Text = $"Account Type: {user.AccountType}";

                userAttendance = new AttendanceList(user.IdNumber);
                userAttendance.Refresh(user.IdNumber);
            }
            else if (parameter is int)
            {
                user = new User((int)parameter);
                await user.Load();
                FullnameText.Text = user.FullName;
                UsernameText.Text = $"Username: {user.Username}";
                IdNumberText.Text = $"ID Number: {user.IdNumber.ToString()}";
                AccountTypeText.Text = $"Account Type: {user.AccountType}";

                userAttendance = new AttendanceList(user.IdNumber);
                userAttendance.Refresh(user.IdNumber);
            }
        }
    }
}
