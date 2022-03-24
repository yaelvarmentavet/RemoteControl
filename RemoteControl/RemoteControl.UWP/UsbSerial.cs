using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using Windows.Devices.Usb;
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
            //SemaphoreConnect.WaitOne();
            if (!Connected)
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
                        serialDevice.ReadTimeout = TimeSpan.FromMilliseconds(1);
                        serialDevice.WriteTimeout = TimeSpan.FromMilliseconds(10);
                        SerialPorts.Add(serialDevice.PortName, serialDevice);
                        Connected = true;
                    }
                }
                //while (true)
                //{
                //    DeviceInformationCollection myDevices = await DeviceInformation.FindAllAsync(SerialDevice.GetDeviceSelector());

                //    if (myDevices.Count > 0)
                //    {
                //        //get the device object 
                //        SerialDevice device = await SerialDevice.FromIdAsync(myDevices[0].Id);
                //        while (device != null)
                //        {
                //            DataWriter DataWriterObject = new DataWriter(device.OutputStream);
                //            DataWriterObject.WriteString("getid,3#");

                //            DataReader DataReaderObject = new DataReader(device.InputStream);
                //            DataReaderObject.InputStreamOptions = InputStreamOptions.Partial;
                //            var loadAsyncTask = DataReaderObject.LoadAsync(1024).AsTask();

                //            UInt32 bytesRead = await loadAsyncTask;

                //            if (bytesRead > 0)
                //            {
                //                String temp = DataReaderObject.ReadString(bytesRead);
                //            }
                //        }
                //    }
                //}
                //SemaphoreConnect.Release();
            }
            return Connected;
        }

        public async Task Disconnect()
        {
            //SemaphoreConnect.WaitOne();
            Connected = false;
            //SemaphoreConnect.Release();
        }

        public IEnumerable<string> GetPorts()
        {
            return SerialPorts.Keys;
            //return SerialPorts.Keys ?? Enumerable.Empty<string>();
        }

        public async Task<int> Read(string portName, byte[] buffer)
        {
            if (SerialPorts.TryGetValue(portName, out SerialDevice port))
            {
                Buffer ibuffer = new Buffer(1024);

                IBuffer response = await port.InputStream.ReadAsync(ibuffer, ibuffer.Capacity, InputStreamOptions.Partial);
                CryptographicBuffer.CopyToByteArray(ibuffer, out byte[] buf);
                buf?.CopyTo(buffer, 0);
                return (int)ibuffer.Length;

                //DataReader DataReaderObject = new DataReader(port.InputStream);
                //DataReaderObject.InputStreamOptions = InputStreamOptions.Partial;
                //var loadAsyncTask = DataReaderObject.LoadAsync(1).AsTask();

                //UInt32 bytesRead = await loadAsyncTask;

                //if (bytesRead > 0)
                //{
                //    byte[] buf = new byte[1024];
                //    DataReaderObject.ReadBytes(buf);
                //    buf?.CopyTo(buffer, 0);
                //    return (int)bytesRead;
                //}
            }
            return ERROR;
        }

        public async Task<int> Write(string portName, byte[] buffer)
        {
            if (SerialPorts.TryGetValue(portName, out SerialDevice port))
            {
                foreach (byte b in buffer)
                {
                    Thread.Sleep(100);
                    await port.OutputStream.WriteAsync(CryptographicBuffer.CreateFromByteArray(new byte[] { b }));
                }

                //DataWriter DataWriterObject = new DataWriter(port.OutputStream);
                //DataWriterObject.WriteBuffer(CryptographicBuffer.CreateFromByteArray(buffer));

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
