using System;
using System.Collections.Generic;
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
    public sealed partial class ResetPasswordDialog : ContentDialog
    {
        public string CurrentPassword
        {
            get
            {
                return CurrentPassbox.Password;
            }
        }
        public string NewPassword
        {
            get
            {
                return NewPassbox.Password;
            }
        }
        public string ConfirmPassword
        {
            get
            {
                return ConfirmPassbox.Password;
            }
        }

        public ResetPasswordDialog()
        {
            this.InitializeComponent();
            CurrentPassbox.IsEnabled = false;
        }
        public ResetPasswordDialog(bool isCurrentUser)
        {
            this.InitializeComponent();
            CurrentPassbox.IsEnabled = isCurrentUser;
        }

        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var deferral = args.GetDeferral();

            if (NewPassword != ConfirmPassword)
            {
                MessageDialog error = new MessageDialog("Passwords don't match", "Error");
                await error.ShowAsync();
                args.Cancel = true;
            }
            else
            {
                args.Cancel = false;
            }

            deferral.Complete();
        }
    }
}
