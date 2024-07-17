using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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
using System.Diagnostics;
using Windows.UI.Popups;
using DesktopApp.ApiAccess;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DesktopApp
{
    public sealed partial class PairCardDialog : ContentDialog
    {
        ArduinoDevice arduino;
        List<DeviceInstance> devices;
        string CardSerial = "";
        User user;

        public PairCardDialog()
        {
            this.InitializeComponent();
            Load();
        }
        public PairCardDialog(User user)
        {
            this.InitializeComponent();
            Load();
            this.user = user;
        }

        private async void Load()
        {
            try
            {
                devices = new List<DeviceInstance>();
                var getDevices = await ArduinoDevice.GetDevicesAsync();
                foreach (DeviceInstance device in getDevices)
                {
                    devices.Add(device);
                }

                IsPrimaryButtonEnabled = false;
            }
            catch (ArduinoDeviceNotFoundException ex)
            {
#if DEBUG
                Debug.WriteLine(ex.Message);
#endif
                MessageDialog error = new MessageDialog(ex.Message, "Arduino Error");
                await error.ShowAsync();
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

        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var deferral = args.GetDeferral();
            try
            {
                if (arduino != null)
                {
                    arduino.Dispose();
                }
                Card card = new Card(CardSerial);
                var response = await user.PairCard(card.SerialNo);
                if (response == Response.Fail)
                {
                    args.Cancel = true;
                    MessageDialog error = new MessageDialog("Failed pairing card.", "Error");
                    await error.ShowAsync();
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

        private async void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            try
            {
                if (arduino != null)
                {
                    arduino.Dispose();
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

        private async void Pair_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (arduino != null)
                {
                    arduino.Dispose();
                    arduino = null;
                }
                if (DeviceList.SelectedItem == null)
                {
                    return;
                }
                var selectedDevice = (DeviceInstance)DeviceList.SelectedItem;

                arduino = new ArduinoDevice(selectedDevice);
                arduino.DataReceived += Arduino_DataReceived;

                SerialLabel.Text = "Tap the ID card on the sensor...";
            }
            catch (ArduinoDeviceNotFoundException ex)
            {
#if DEBUG
                Debug.WriteLine(ex.Message);
#endif
                MessageDialog error = new MessageDialog(ex.Message, "Arduino Error");
                await error.ShowAsync();
            }
            catch (ArduinoDeviceIOException ex)
            {
#if DEBUG
                Debug.WriteLine(ex.Message);
#endif
                MessageDialog error = new MessageDialog(ex.Message, "Arduino Error");
                await error.ShowAsync();
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

        private async void Arduino_DataReceived(object sender, string e)
        {
            try
            {
                CardSerial = e;
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    SerialLabel.Text = $"Card Serial No.: {e}";
                    IsPrimaryButtonEnabled = true;
                });
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

        public string GetCardSerial()
        {
            return CardSerial;
        }
    }
}
