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

        private async void AppBarButton_Click(object sender, RoutedEventArgs e)
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
            CreateUserDialog createAccountDialog = new CreateUserDialog();
            await createAccountDialog.ShowAsync();
        }

        private void UsersList_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            ListView usersListView = sender as ListView;
            rightClickedUser = (User)((FrameworkElement)e.OriginalSource).DataContext;
            if (rightClickedUser == null)
            {
                return;
            }
            RegistryFlyout.ShowAt(usersListView, e.GetPosition(usersListView));
        }

        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
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

        private async void Refresh()
        {
            var userList = await UserAccess.GetAllUsers();

            if (userList == null)
                return;

            users.Clear();
            foreach (User user in userList)
            {
                users.Add(user);
            }
        }
    }
}
