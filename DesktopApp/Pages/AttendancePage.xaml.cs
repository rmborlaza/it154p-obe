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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DesktopApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AttendancePage : Page
    {
        AttendanceList attendanceList;
        Attendance rightClickedAttendance;

        public AttendancePage()
        {
            this.InitializeComponent();
            attendanceList = new AttendanceList();
            Refresh();
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
            }
        }
        private async void Refresh()
        {
            try
            {
                var result = await attendanceList.TryRefreshAsync();
                if (result == Response.Fail)
                {
                    ErrorDialog error = new ErrorDialog("Failed to refresh data.");
                    await error.ShowAsync();
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

        private async void AttendanceList_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            try
            {
                ListView attendanceListView = sender as ListView;
                var data = ((FrameworkElement)e.OriginalSource).DataContext;
                if (data == null)
                {
                    return;
                }
                rightClickedAttendance = (Attendance)data;
                RegistryFlyout.ShowAt(attendanceListView, e.GetPosition(attendanceListView));
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
                        Frame.Navigate(typeof(UserAccountPage), rightClickedAttendance.IdNumber);
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
    }
}
