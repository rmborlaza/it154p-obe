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
using DesktopApp.ApiAccess;
using System.Diagnostics;
using Windows.UI.Popups;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DesktopApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RegistryPage : Page
    {
        UserList users = new UserList();
        User rightClickedUser = null;

        public RegistryPage()
        {
            this.InitializeComponent();
            Refresh();
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            var obj = sender as AppBarButton;
            var tag = obj.Tag as string;

            switch (tag)
            {
                case "Add":
                    AddUser();
                    break;
                case "Refresh":
                    Refresh();
                    break;
            }
        }

        private async void AddUser()
        {
            try
            {
                CreateUserDialog createAccountDialog = new CreateUserDialog();
                var result = await createAccountDialog.ShowAsync();
                if (result == ContentDialogResult.Primary)
                {
                    UserAddTip.IsOpen = true;
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

        private async void UsersList_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            try
            {
                ListView usersListView = sender as ListView;
                rightClickedUser = (User)((FrameworkElement)e.OriginalSource).DataContext;
                if (rightClickedUser == null)
                {
                    return;
                }
                RegistryFlyout.ShowAt(usersListView, e.GetPosition(usersListView));
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

        private async void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var menu = (MenuFlyoutItem)e.OriginalSource;
                var tag = menu.Tag as string;

                switch (tag)
                {
                    case "View":
                        Frame.Navigate(typeof(UserAccountPage), rightClickedUser);
                        break;
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

        private async void Refresh()
        {
            try
            {
                var userList = await UserAccess.GetAllUsers();

                if (userList == null)
                {
                    ErrorDialog error = new ErrorDialog("Failed to refresh data.");
                    await error.ShowAsync();
                    return;
                }

                users.Clear();
                foreach (User user in userList)
                {
                    users.Add(user);
                }
                Bindings.Update();
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
