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
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using DesktopApp.ApiAccess;
using Windows.UI.Popups;
using System.Diagnostics;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace DesktopApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        User myAccount;

        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                myAccount = await UserAccess.Authenticate(UsernameBox.Text, PasswordBox.Password);

                if (myAccount == null)
                {
                    ErrorDialog error = new ErrorDialog("Invalid username and password");
                    await error.ShowAsync();
                    return;
                }

                if (myAccount.AccountType == AccountType.System)
                {
                    Frame.Navigate(typeof(AppPage), myAccount, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromLeft });
                }
                else
                {
                    ErrorDialog error = new ErrorDialog("This account is not a system account.");
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

        private async void CreateAccount_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CreateUserDialog dialog = new CreateUserDialog(true);
                var result = await dialog.ShowAsync();
                if (result == ContentDialogResult.Primary)
                {
                    UserAddTip.IsOpen = true;
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
    }
}
