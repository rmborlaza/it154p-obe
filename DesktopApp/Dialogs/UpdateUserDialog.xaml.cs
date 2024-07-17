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
using DesktopApp.ApiAccess;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DesktopApp
{
    public sealed partial class UpdateUserDialog : ContentDialog
    {
        User user;
        
        public UpdateUserDialog()
        {
            this.InitializeComponent();
        }
        internal UpdateUserDialog(User user)
        {
            this.InitializeComponent();
            if (user != null)
            {
                this.user = user;
                FirstNameBox.Text = user.FirstName;
                LastNameBox.Text = user.LastName;
                UsernameBox.Text = user.Username;

                if (user.AccountType == AccountType.System)
                {
                    OptionSystem.IsChecked = true;
                    OptionUser.IsChecked = false;
                }
                else
                {
                    OptionSystem.IsChecked = false;
                    OptionUser.IsChecked = true;
                }
            }
        }

        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var deferral = args.GetDeferral();
            try
            {
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

                if (username.Contains(" "))
                {
                    MessageDialog message = new MessageDialog("Username cannot have space.", "Error");
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

                var response = await user.Update(firstName, lastName, username, type);

                if (response == Response.Fail)
                {
                    args.Cancel = true;
                }
                else
                {
                    args.Cancel = false;
                }
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
