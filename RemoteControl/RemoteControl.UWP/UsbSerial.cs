using RemoteControl.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using Windows.Security.Cryptography;
using Windows.Storage.Streams;
using Buffer = Windows.Storage.Streams.Buffer;

namespace RemoteControl.UWP
{
    public class UsbSerial : IUsbSerial
    {
        private const int ERROR = -1;

        private Dictionary<string, SerialDevice> SerialPorts = new Dictionary<string, SerialDevice>();
        private EventHandler EventAdded;
        private EventHandler EventRemoved;
        //private bool Connected = false;
        private Semaphore SemaphoreConnect = new Semaphore(1, 1);
        private DeviceWatcher Watcher;

        public UsbSerial()
        {
            Watcher = DeviceInformation.CreateWatcher(DeviceClass.All);
            Watcher.Added += DeviceAdded;
            Watcher.Removed += DeviceRemoved;
            Watcher.EnumerationCompleted += DeviceEnumerationCompleted;
            Watcher.Updated += DeviceUpdated;
            Watcher.Stopped += DeviceStopped;
            //DeviceWatcherTrigger deviceWatcherTrigger = deviceWatcher.GetBackgroundTrigger(new List<DeviceWatcherEventKind>() { DeviceWatcherEventKind.Add, DeviceWatcherEventKind.Remove });
            Watcher.Start();
        }

        private void DeviceStopped(DeviceWatcher sender, object args)
        {
        }

        private void DeviceUpdated(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            new Thread(async () =>
            {
                SemaphoreConnect.WaitOne();
                //await Connect();
                await Connect(args.Id);
                SemaphoreConnect.Release();
            }).Start();
        }

        private void DeviceEnumerationCompleted(DeviceWatcher sender, object args)
        {
            new Thread(async () =>
            {
                SemaphoreConnect.WaitOne();
                await Connect();
                SemaphoreConnect.Release();
            }).Start();
        }

        private void DeviceRemoved(DeviceWatcher sender, DeviceInformationUpdate args)
        {
        }

        private void DeviceAdded(DeviceWatcher sender, DeviceInformation args)
        {
        }

        private async Task<bool> Connect()
        {
            //if (!Connected)
            //{
            DeviceInformationCollection serialDeviceInfos = await DeviceInformation.FindAllAsync(SerialDevice.GetDeviceSelector());

            foreach (DeviceInformation serialDeviceInfo in serialDeviceInfos)
            {
                await Connect(serialDeviceInfo.Id);
                //Connected = true;
            }
            //}
            //return Connected;
            return true;
        }

        private async Task Connect(string id)
        {
            try
            {
                //SemaphoreConnect.WaitOne();
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
                {
                    EventRemoved?.Invoke(this, new PortEventArgs() { Port = SerialPorts.TryGetValue(id, out SerialDevice device) ? device.PortName : string.Empty });
                    SerialPorts.Remove(id);
                }
            }
            finally
            {
                //SemaphoreConnect.Release();
            }
        }

        //public async Task Disconnect()
        //{
        //    //SemaphoreConnect.WaitOne();
        //    //Connected = false;
        //    //SemaphoreConnect.Release();
        //}

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
                //foreach (byte b in buffer)
                //{
                //    Thread.Sleep(100);
                //    await port.OutputStream.WriteAsync(CryptographicBuffer.CreateFromByteArray(new byte[] { b }));
                //}
                await port.OutputStream.WriteAsync(CryptographicBuffer.CreateFromByteArray(buffer));
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
