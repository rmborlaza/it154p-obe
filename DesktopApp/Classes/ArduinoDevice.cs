using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using Windows.Storage.Streams;
using Windows.System.Threading;
using Windows.UI.Popups;

namespace DesktopApp
{
    public class ArduinoDevice : IDisposable
    {
        SerialDevice serialDevice;
        DataReader dataReader;
        DataWriter dataWriter;
        private const uint BufferSize = 1;

        public event EventHandler<string> DataReceived;
        StringBuilder dataText;

        [Obsolete]
        public ArduinoDevice(string portName)
        {
            Initialize(portName);
            dataText = new StringBuilder();
        }

        [Obsolete]
        public ArduinoDevice(string portName, uint baudRate)
        {
            Initialize(portName, baudRate);
            dataText = new StringBuilder();
        }
        
        public ArduinoDevice(DeviceInstance device)
        {
            Initialize(device);
            dataText = new StringBuilder();
        }
        private async void Initialize(string portName)
        {
            var selector = SerialDevice.GetDeviceSelector(portName);
            var devices = await DeviceInformation.FindAllAsync(selector);
            if (devices.Count == 0)
            {
                throw new ArduinoDeviceNotFoundException();
            }
            if (devices.Any())
            {
                var deviceInfo = devices.First();
                serialDevice = await SerialDevice.FromIdAsync(deviceInfo.Id);
                if (serialDevice == null)
                {
                    throw new ArduinoDeviceNotFoundException();
                }

                serialDevice.BaudRate = 9600;
                serialDevice.DataBits = 8;
                serialDevice.Parity = SerialParity.None;
                serialDevice.StopBits = SerialStopBitCount.One;

                dataReader = new DataReader(serialDevice.InputStream);
                dataWriter = new DataWriter(serialDevice.OutputStream);

                Task.Run(ReadDataAsync);
            }
        }
        
        private async void Initialize(string portName, uint baudRate)
        {
            var selector = SerialDevice.GetDeviceSelector(portName);
            var devices = await DeviceInformation.FindAllAsync(selector);
            if (devices.Count == 0)
            {
                throw new ArduinoDeviceNotFoundException();
            }
            if (devices.Any())
            {
                var deviceInfo = devices.First();
                serialDevice = await SerialDevice.FromIdAsync(deviceInfo.Id);
                if (serialDevice == null)
                {
                    throw new ArduinoDeviceNotFoundException();
                }

                serialDevice.BaudRate = 9600;
                serialDevice.DataBits = 8;
                serialDevice.Parity = SerialParity.None;
                serialDevice.StopBits = SerialStopBitCount.One;

                dataReader = new DataReader(serialDevice.InputStream);
                dataWriter = new DataWriter(serialDevice.OutputStream);

                Task.Run(ReadDataAsync);
            }
        }
        
        private async void Initialize(DeviceInstance device)
        {
            try
            {
                serialDevice = await SerialDevice.FromIdAsync(device.Id);
                if (serialDevice == null)
                {
                    throw new ArduinoDeviceNotFoundException();
                }
                serialDevice.BaudRate = 9600;
                serialDevice.DataBits = 8;
                serialDevice.Parity = SerialParity.None;
                serialDevice.StopBits = SerialStopBitCount.One;

                dataReader = new DataReader(serialDevice.InputStream);
                dataWriter = new DataWriter(serialDevice.OutputStream);

                Task.Run(ReadDataAsync);
            }
            catch (Exception ex)
            {
                MessageDialog dialog = new MessageDialog(ex.Message, "Error");
                await dialog.ShowAsync();
            }
        }
        private async Task ReadDataAsync()
        {
            while (true)
            {
                uint bytesRead = await dataReader.LoadAsync(BufferSize);
                if (bytesRead > 0)
                {
                    string data = dataReader.ReadString(bytesRead);
                    Append(data);
                }
            }
        }

        private void Append(string character)
        {
            if (character == "\r")
            {
                return;
            }
            if (character == "\n")
            {
                DataReceived?.Invoke(this, dataText.ToString());
                dataText.Clear();
                return;
            }
            dataText.Append(character);
        }

        public static async Task<DeviceInstance[]> GetDevicesAsync()
        {
            var selector = SerialDevice.GetDeviceSelector();
            var devices = await DeviceInformation.FindAllAsync(selector);
            var count = devices.Count;
            DeviceInstance[] portNames = new DeviceInstance[count];

            for (int i = 0; i < count; i++)
            {
                portNames[i] = new DeviceInstance(devices[i].Name, devices[i].Id);
            }
            return portNames;
        }

        public async Task WriteLineAsync(string text)
        {
            dataWriter.WriteString(text);
            await dataWriter.StoreAsync();
        }
        public void Dispose()
        {
            dataReader?.Dispose();
            dataWriter?.Dispose();
            serialDevice?.Dispose();
            DataReceived = null;
        }
    }
    public class ArduinoDeviceNotFoundException : Exception
    {
        public override string Message
        {
            get
            {
                return "Arduino device not found.";
            }
        }
    }
    public class ArduinoDeviceIOException : Exception
    {
        public override string Message
        {
            get
            {
                return "Arduino device I/O error.";
            }
        }
    }
    public struct DeviceInstance
    {
        public string Name { get; private set; }
        public string Id { get; private set; }
        public DeviceInstance(string Name, string Id)
        {
            this.Name = Name;
            this.Id = Id;
        }
        public override string ToString()
        {
            return Name;
        }
    }
}