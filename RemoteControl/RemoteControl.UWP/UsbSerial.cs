using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using Windows.Security.Cryptography;
using Windows.Storage.Streams;
using Buffer = Windows.Storage.Streams.Buffer;

namespace RemoteControl.UWP
{
    //class UsbEventArgs : EventArgs
    //{
    //    public string Id;
    //}
    public class UsbSerial : IUsbSerial
    {
        private const int ERROR = -1;

        private Dictionary<string, SerialDevice> SerialPorts = new Dictionary<string, SerialDevice>();
        private EventHandler EventAdded;
        private EventHandler EventRemoved;
        private bool Connected = false;
        private Semaphore SemaphoreConnect = new Semaphore(1, 1);

        public UsbSerial()
        {
            DeviceWatcher deviceWatcher = DeviceInformation.CreateWatcher(DeviceClass.All);
            deviceWatcher.Added += DeviceAdded;
            deviceWatcher.Removed += DeviceRemoved;
            //DeviceWatcherTrigger deviceWatcherTrigger = deviceWatcher.GetBackgroundTrigger(new List<DeviceWatcherEventKind>() { DeviceWatcherEventKind.Add, DeviceWatcherEventKind.Remove });
            deviceWatcher.Start();
        }

        private void DeviceRemoved(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            //EventRemoved.Invoke(this, new UsbEventArgs() { Id = args.Id});
        }

        private void DeviceAdded(DeviceWatcher sender, DeviceInformation args)
        {
            //EventAdded.Invoke(this, new UsbEventArgs() { Id = args.Id});
        }

        public async Task<bool> Connect()
        {
            SemaphoreConnect.WaitOne();
            if (!Connected)
            {
                DeviceInformationCollection serialDeviceInfos = await DeviceInformation.FindAllAsync(SerialDevice.GetDeviceSelector());

                foreach (DeviceInformation serialDeviceInfo in serialDeviceInfos)
                {
                    //SerialDevice serialDevice = await SerialDevice.FromIdAsync(serialDeviceInfo.Id);
                    SerialDevice serialDevice = await SerialDevice.FromIdAsync(serialDeviceInfo.Id);

                    if (serialDevice != null)
                    {
                        serialDevice.BaudRate = 115200;
                        serialDevice.DataBits = 8;
                        serialDevice.Parity = SerialParity.None;
                        serialDevice.StopBits = SerialStopBitCount.One;
                        serialDevice.ReadTimeout = TimeSpan.FromMilliseconds(10);
                        serialDevice.WriteTimeout = TimeSpan.FromMilliseconds(10);
                        SerialPorts.Add(serialDevice.PortName, serialDevice);

                        //DataReader dataReader = new DataReader(serialDevice.InputStream);
                        //dataReader.InputStreamOptions = InputStreamOptions.Partial;
                        //var bytesAvailable = await dataReader.LoadAsync(1024);
                        //var byteArray = new byte[bytesAvailable];
                        //dataReader.ReadBytes(byteArray);
                        //string data = dataReader.ReadString(bytesAvailable);
                        //byte readByte = dataReader.ReadByte();
                        Connected = true;
                    }
                }
                SemaphoreConnect.Release();
            }
            return Connected;
        }

        public IEnumerable<string> GetPorts()
        {
            return SerialPorts.Keys;
            //return SerialPorts.Keys ?? Enumerable.Empty<string>();
        }

        public async Task<int> Read(string portName, byte[] buffer)
        {
            //IBuffer rsp = await SerialPorts.GetValueOrDefault(portName)?.InputStream.ReadAsync(ibuffer, ibuffer.Capacity, InputStreamOptions.Partial);
            if (SerialPorts.TryGetValue(portName, out SerialDevice port))
            {
                Buffer ibuffer = new Buffer(1024);
                IBuffer responce = await port.InputStream.ReadAsync(ibuffer, ibuffer.Capacity, InputStreamOptions.Partial);
                //DataReader dataReader = DataReader.FromBuffer(ibuffer);
                //string data = dataReader?.ReadString(ibuffer.Length);
                //buffer = Encoding.UTF8.GetBytes(data);
                CryptographicBuffer.CopyToByteArray(ibuffer, out buffer);
                //string data = Encoding.UTF8.GetString(buffer, 0, (int)ibuffer.Length);
                return (int)ibuffer.Length;
            }
            return ERROR;
        }

        public async Task<int> Write(string portName, byte[] buffer)
        {
            //DataWriter dataWriter = new DataWriter(SerialPorts.GetValueOrDefault(portName)?.OutputStream);
            //uint? resp = dataWriter?.WriteString(Encoding.UTF8.GetString(buffer));

            //var resp = await SerialPorts.GetValueOrDefault(portName)?.OutputStream.WriteAsync(buffer?.AsBuffer());

            //var resp = await SerialPorts.GetValueOrDefault(portName)?.OutputStream.WriteAsync(CryptographicBuffer.CreateFromByteArray(buffer));
            if (SerialPorts.TryGetValue(portName, out SerialDevice port))
            {
                foreach (byte b in buffer)
                {
                    Thread.Sleep(100);
                    //await SerialPorts.GetValueOrDefault(portName)?.OutputStream.WriteAsync(CryptographicBuffer.CreateFromByteArray(new byte[] { b }));
                    await port.OutputStream.WriteAsync(CryptographicBuffer.CreateFromByteArray(new byte[] { b }));
                }
                return buffer.Length;
            }
            return ERROR;
        }

        public void Event(EventHandler eventRemoved, EventHandler eventAdded)
        {
            EventRemoved = eventRemoved;
            EventAdded = eventAdded;
        }
    }
}
