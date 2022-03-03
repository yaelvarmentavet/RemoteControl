using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using Windows.Devices.Usb;
using Windows.Storage.Streams;
using Buffer = Windows.Storage.Streams.Buffer;

namespace RemoteControl.UWP
{
    public class UsbInterface : IUsbInterface
    {
        private Dictionary<string, SerialDevice> SerialPorts = new Dictionary<string, SerialDevice>();

        public UsbInterface()
        {
            new Thread(async () =>
            {
                DeviceInformationCollection serialDeviceInfos = await DeviceInformation.FindAllAsync(SerialDevice.GetDeviceSelector());

                foreach (DeviceInformation serialDeviceInfo in serialDeviceInfos)
                {
                    SerialDevice serialDevice = await SerialDevice.FromIdAsync(serialDeviceInfo.Id);

                    if (serialDevice != null)
                    {
                        serialDevice.BaudRate = 115200;
                        serialDevice.DataBits = 8;
                        serialDevice.Parity = SerialParity.None;
                        serialDevice.StopBits = SerialStopBitCount.One;
                        serialDevice.ReadTimeout = TimeSpan.FromMilliseconds(1000);
                        SerialPorts.Add(serialDevice.PortName, serialDevice);

                        //DataReader dataReader = new DataReader(serialDevice.InputStream);
                        //dataReader.InputStreamOptions = InputStreamOptions.Partial;
                        //var bytesAvailable = await dataReader.LoadAsync(1024);
                        //var byteArray = new byte[bytesAvailable];
                        //dataReader.ReadBytes(byteArray);
                        //string data = dataReader.ReadString(bytesAvailable);
                        //byte readByte = dataReader.ReadByte();
                    }
                }
            }).Start();
        }

        public IEnumerable<string> GetPorts()
        {
            //return SerialPorts.Keys;
            return SerialPorts.Keys ?? Enumerable.Empty<string>();
        }

        public async Task<string> Read(string portName, byte[] buffer)
        {
            Buffer ibuffer = new Buffer(1024);
            IBuffer rsp = await SerialPorts.GetValueOrDefault(portName)?.InputStream.ReadAsync(ibuffer, ibuffer.Capacity, InputStreamOptions.None);
            DataReader dataReader = DataReader.FromBuffer(ibuffer);
            string data = dataReader?.ReadString(ibuffer.Length);
            buffer = Encoding.UTF8.GetBytes(data);
            return data;
        }

        public void Write(string portName, byte[] buffer)
        {
            DataWriter dataWriter = new DataWriter(SerialPorts.GetValueOrDefault(portName)?.OutputStream);
            uint? resp = dataWriter?.WriteString(Encoding.UTF8.GetString(buffer));
        }

        public string GetData()
        {
            return null;
        }
        public void Event(EventHandler eventHandler)
        { }
        public async Task<int> Send(string data)
        {
            return -1;
        }
    }
}
