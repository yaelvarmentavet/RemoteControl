using System;
using System.Collections.Generic;
using System.Linq;
//using System.Management;
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
            DeviceWatcher watch = DeviceInformation.CreateWatcher(DeviceClass.All);
            watch.Added += DeviceAdded;
            watch.Removed += DeviceRemoved;
            watch.EnumerationCompleted += DeviceEnumerationCompleted;
            watch.Updated += DeviceUpdated;
            watch.Stopped += DeviceStopped;
            //DeviceWatcherTrigger deviceWatcherTrigger = deviceWatcher.GetBackgroundTrigger(new List<DeviceWatcherEventKind>() { DeviceWatcherEventKind.Add, DeviceWatcherEventKind.Remove });
            watch.Start();


            //Program p = new Program();
            //ManagementScope scope = new ManagementScope("root\\CIMV2");

            //scope.Options.EnablePrivileges = true;
            //try
            //{
            //    WqlEventQuery query = new WqlEventQuery();
            //    query.EventClassName = "__InstanceOperationEvent";
            //    query.WithinInterval = new TimeSpan(0, 0, 1);
            //    query.Condition = @"TargetInstance ISA 'Win32_USBControllerdevice'";

            //    //ManagementEventWatcher watcher = new ManagementEventWatcher(scope, query);
            //    ManagementEventWatcher watcher = new ManagementEventWatcher(query);
            //    watcher.EventArrived += new EventArrivedEventHandler(WaitForUSBChangeEvent);
            //    watcher.Start();
            //}
            //catch(Exception ex)
            //{
            //}
        }

        private void DeviceStopped(DeviceWatcher sender, object args)
        {
        }

        private void DeviceUpdated(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            Task.Run(async () =>
            {
                SemaphoreConnect.WaitOne();
                //await Connect();
                await Connect(args.Id);
                SemaphoreConnect.Release();
            });
        }

        //private void WaitForUSBChangeEvent(object sender, EventArrivedEventArgs e)
        //{
        //}


        private void DeviceEnumerationCompleted(DeviceWatcher sender, object args)
        {
            Task.Run(async () =>
            {
                SemaphoreConnect.WaitOne();
                await Connect();
                SemaphoreConnect.Release();
            });
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
            if (!Connected)
            {
                DeviceInformationCollection serialDeviceInfos = await DeviceInformation.FindAllAsync(SerialDevice.GetDeviceSelector());

                foreach (DeviceInformation serialDeviceInfo in serialDeviceInfos)
                {
                    await Connect(serialDeviceInfo.Id);
                    Connected = true;
                }
            }
            return Connected;
        }

        private async Task Connect(string id)
        {
            try
            {
                SerialDevice serialDevice = await SerialDevice.FromIdAsync(id);

                if (serialDevice != null)
                {
                    serialDevice.BaudRate = 115200;
                    serialDevice.DataBits = 8;
                    serialDevice.Parity = SerialParity.None;
                    serialDevice.StopBits = SerialStopBitCount.One;
                    serialDevice.ReadTimeout = TimeSpan.FromMilliseconds(1);
                    serialDevice.WriteTimeout = TimeSpan.FromMilliseconds(1);
                    SerialPorts.Add(id, serialDevice);
                }
            }
            catch
            {
                if (SerialPorts.ContainsKey(id))
                    SerialPorts.Remove(id);
            }
        }

        public async Task Disconnect()
        {
            //SemaphoreConnect.WaitOne();
            Connected = false;
            //SemaphoreConnect.Release();
        }

        public IEnumerable<string> GetPorts()
        {
            //return SerialPorts.Keys;
            return SerialPorts.Values.Select(v => v?.PortName);
            //return SerialPorts.Keys ?? Enumerable.Empty<string>();
        }

        public async Task<int> Read(string portName, byte[] buffer)
        {
            //if (SerialPorts.TryGetValue(portName, out SerialDevice port))
            SerialDevice port = SerialPorts.Values.FirstOrDefault(v => v?.PortName == portName);
            if (port != null)
            {
                Buffer ibuffer = new Buffer(1024);

                IBuffer response = await port.InputStream.ReadAsync(ibuffer, ibuffer.Capacity, InputStreamOptions.Partial);
                CryptographicBuffer.CopyToByteArray(ibuffer, out byte[] buf);
                buf?.CopyTo(buffer, 0);
                return (int)ibuffer.Length;
            }
            return ERROR;
        }

        public async Task<int> Write(string portName, byte[] buffer)
        {
            //if (SerialPorts.TryGetValue(portName, out SerialDevice port))
            SerialDevice port = SerialPorts.Values.FirstOrDefault(v => v?.PortName == portName);
            if (port != null)
            {
                foreach (byte b in buffer)
                {
                    Thread.Sleep(100);
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
