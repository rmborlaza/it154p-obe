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
using muxc = Microsoft.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DesktopApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AppPage : Page
    {
        public static User myAccount;
        public AppPage()
        {
            this.InitializeComponent();
            AppNav.SelectedItem = NavAttendance;
            AppNav.IsBackEnabled = false;
        }

        private void AppNav_SelectionChanged(muxc.NavigationView sender, muxc.NavigationViewSelectionChangedEventArgs args)
        {
            var tag = args.SelectedItemContainer.Tag as string;

            switch (tag)
            {
                case "Home":
                    ContentFrame.Navigate(typeof(HomePage));
                    break;
                case "Attendance":
                    ContentFrame.Navigate(typeof(AttendancePage));
                    break;
                case "Registry":
                    ContentFrame.Navigate(typeof(RegistryPage));
                    break;
                case "MyAccount":
                    ContentFrame.Navigate(typeof(UserAccountPage), myAccount);
                    break;
                case "Logout":
                    Frame.GoBack();
                    return;
            }
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is User)
            {
                myAccount = (User)e.Parameter;
            }
            base.OnNavigatedTo(e);
        }
    }
}
