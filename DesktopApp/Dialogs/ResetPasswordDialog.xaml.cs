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
    public sealed partial class ResetPasswordDialog : ContentDialog
    {
        User user;
        bool isCurrentUser;

        public ResetPasswordDialog()
        {
            this.InitializeComponent();
        }
        public ResetPasswordDialog(User user, bool isCurrentUser)
        {
            this.InitializeComponent();
            CurrentPassbox.IsEnabled = isCurrentUser;
            this.user = user;
            this.isCurrentUser = isCurrentUser;
        }

        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var deferral = args.GetDeferral();
            string oldPassword = CurrentPassbox.Password;
            string newPassword = NewPassbox.Password;
            string confirmPassword = ConfirmPassbox.Password;

            try
            {
                if (newPassword != confirmPassword)
                {
                    MessageDialog error = new MessageDialog("Passwords don't match", "Error");
                    await error.ShowAsync();
                    args.Cancel = true;
                }
                else
                {
                    Response response;

                    if (isCurrentUser)
                    {
                        response = await user.UpdatePassword(oldPassword, newPassword);
                    }
                    else
                    {
                        response = await user.UpdatePassword(newPassword);
                    }

                    if (response == Response.Fail)
                    {
                        MessageDialog error = new MessageDialog("Failed updating user account password.", "Error");
                        await error.ShowAsync();
                        args.Cancel = true;
                    }
                    else
                    {
                        args.Cancel = false;
                    }
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
