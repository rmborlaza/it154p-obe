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

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DesktopApp
{
    public sealed partial class PairCardDialog : ContentDialog
    {
        ArduinoDevice arduino;
        List<DeviceInstance> devices;
        string CardSerial = "";

        public PairCardDialog()
        {
            this.InitializeComponent();
            Load();
        }
        private async void Load()
        {
            devices = new List<DeviceInstance>();
            var getDevices = await ArduinoDevice.GetDevicesAsync();
            foreach (DeviceInstance device in getDevices)
            {
                devices.Add(device);
            }

            IsPrimaryButtonEnabled = false;
        }
        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (arduino != null)
            {
                arduino.Dispose();
            }
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (arduino != null)
            {
                arduino.Dispose();
            }
        }

        private void Pair_Click(object sender, RoutedEventArgs e)
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
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private async void Arduino_DataReceived(object sender, string e)
        {
            CardSerial = e;
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                SerialLabel.Text = $"Card Serial No.: {e}";
                IsPrimaryButtonEnabled = true;
            });
        }

        public string GetCardSerial()
        {
            return CardSerial;
        }
    }
}
