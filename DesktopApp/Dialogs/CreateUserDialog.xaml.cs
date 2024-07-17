using DesktopApp.ApiAccess;
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

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DesktopApp
{
    public sealed partial class CreateUserDialog : ContentDialog
    {
        public string Password
        {
            get
            {
                return PasswordBox.Password;
            }
        }

        public CreateUserDialog()
        {
            this.InitializeComponent();
            OptionUser.IsChecked = true;
        }
        public CreateUserDialog(bool isSystem)
        {
            this.InitializeComponent();
            if (isSystem)
            {
                OptionSystem.IsChecked = true;
                OptionSystem.IsEnabled = false;
                OptionUser.IsEnabled = false;
            }
            else
            {
                OptionUser.IsChecked = true;
            }
        }

        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var deferral = args.GetDeferral();
            try
            {
                string password = PasswordBox.Password;
                string confirmPass = ConfirmPasswordBox.Password;
                string firstName = FirstNameBox.Text;
                string lastName = LastNameBox.Text;
                string username = UsernameBox.Text;

                if (string.IsNullOrEmpty(firstName))
                {
                    MessageDialog message = new MessageDialog("Fields cannot be blank.", "Error");
                    await message.ShowAsync();
                    args.Cancel = true;
                    return;
                }
                if (string.IsNullOrEmpty(lastName))
                {
                    MessageDialog message = new MessageDialog("Fields cannot be blank.", "Error");
                    await message.ShowAsync();
                    args.Cancel = true;
                    return;
                }
                if (string.IsNullOrEmpty(username))
                {
                    MessageDialog message = new MessageDialog("Fields cannot be blank.", "Error");
                    await message.ShowAsync();
                    args.Cancel = true;
                    return;
                }
                if (string.IsNullOrEmpty(PasswordBox.Password))
                {
                    MessageDialog message = new MessageDialog("Fields cannot be blank.", "Error");
                    await message.ShowAsync();
                    args.Cancel = true;
                    return;
                }
                if (string.IsNullOrEmpty(ConfirmPasswordBox.Password))
                {
                    MessageDialog message = new MessageDialog("Fields cannot be blank.", "Error");
                    await message.ShowAsync();
                    args.Cancel = true;
                    return;
                }

                if (username.Contains(" "))
                {
                    MessageDialog message = new MessageDialog("Username cannot have space.", "Error");
                    await message.ShowAsync();
                    args.Cancel = true;
                    return;
                }

                if (password != confirmPass)
                {
                    args.Cancel = true;
                    MessageDialog message = new MessageDialog("Passwords don't match.", "Error");
                    await message.ShowAsync();
                    args.Cancel = true;
                    return;
                }

                AccountType type;
                if ((bool)OptionSystem.IsChecked)
                {
                    type = AccountType.System;
                }
                else
                {
                    type = AccountType.User;
                }

                User user = new User(0, firstName, lastName, username, DateTime.Now, type);
                var response = await UserAccess.AddUser(user, PasswordBox.Password);

                if (response == Response.Fail)
                {
                    MessageDialog error = new MessageDialog("Error creating user account.", "Error");
                    await error.ShowAsync();
                    args.Cancel = true;
                    return;
                }
                args.Cancel = false;
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine(ex.Message);
#endif
                args.Cancel = true;
                MessageDialog error = new MessageDialog(ex.Message, "Runtime Error");
                await error.ShowAsync();
                
            }
            finally
            {
                deferral.Complete();
            }
        }
    }
}
