﻿using System;
using Android.Hardware.Usb;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Android.Runtime;
using Android.App;
using Android.Content;
using System.Threading.Tasks;

//[assembly: Xamarin.Forms.Dependency(typeof(DeviceInfo))]
namespace RemoteControl.Droid
{
    class HashMapp : Java.Util.HashMap
    {

        IntPtr class_ref;

        public HashMapp(IntPtr handle, JniHandleOwnership transfer)
        {
            IntPtr lref = JNIEnv.GetObjectClass(handle);
            class_ref = JNIEnv.NewGlobalRef(lref);
            JNIEnv.DeleteLocalRef(lref);
        }
    }

    public class SerialDevice
    {
        public UsbDevice UsbDevice;
        public UsbEndpoint EndpointRx;
        public UsbEndpoint EndpointTx;
        public UsbDeviceConnection Connection;
    }

    public class BroadcastReceiverSystem : BroadcastReceiver
    {
        public BroadcastReceiverSystem() { }
        public event EventHandler usbAttached;
        public override void OnReceive(Context c, Intent i)
        {
            usbAttached(this, EventArgs.Empty);
        }
    }

    public class UsbInterface : IUsbInterface
    {
        private Dictionary<string, SerialDevice> SerialPorts = new Dictionary<string, SerialDevice>();
        private EventHandler EventAdded;
        private EventHandler EventRemoved;

        public UsbInterface()
        {
            //PendingIntent mAttachedIntent = PendingIntent.GetBroadcast(Application.Context, 0, new Intent(UsbManager.ActionUsbDeviceAttached), 0);
            BroadcastReceiverSystem usbReciever = new BroadcastReceiverSystem();
            IntentFilter filter = new IntentFilter(UsbManager.ActionUsbDeviceAttached);
            //RegisterReceiver(usbReciever, filter);
            usbReciever.usbAttached += usbAttached;
        }

        private void usbAttached(object sender, EventArgs e)
        {
            EventAdded.Invoke(this, EventArgs.Empty);
        }

        public async Task<bool> Connect()
        {

            //new Thread(async () =>
            //    {
            //        while (true)
            //        {
            //UsbManager manager = GetSystemService(Context.UsbService) as UsbManager;

            //Java.Util.HashMap devices = (Java.Util.HashMap)(manager?.DeviceList);
            var devices = MainActivity.Manager?.DeviceList;

            foreach (UsbDevice device in (devices as IDictionary<string, UsbDevice>)?.Values)
            {
                PendingIntent mPermissionIntent = PendingIntent.GetBroadcast(Application.Context, 0, new Intent("android.permission.USB_PERMISSION"), 0);
                bool? hasPermision = false;
                while (hasPermision != true)
                {
                    MainActivity.Manager?.RequestPermission(device, mPermissionIntent);
                    hasPermision = MainActivity.Manager?.HasPermission(device);
                }

                Android.Hardware.Usb.UsbInterface intf = device?.GetInterface(0);
                UsbEndpoint endpointRx = intf?.GetEndpoint(0);
                UsbEndpoint EndpointTx = intf?.GetEndpoint(1);
                UsbDeviceConnection Connection = MainActivity.Manager?.OpenDevice(device);
                if (Connection != null)
                {
                    Connection?.ClaimInterface(intf, true);
                    int? resp = -1;

                    resp = Connection?.ControlTransfer((UsbAddressing)64, 0, 0, 0, null, 0, 0);// reset  mConnection.controlTransfer(0×40, 0, 1, 0, null, 0, 0);//clear Rx
                    resp = Connection?.ControlTransfer((UsbAddressing)64, 0, 1, 0, null, 0, 0);// clear Rx
                    resp = Connection?.ControlTransfer((UsbAddressing)64, 0, 2, 0, null, 0, 0);// clear Tx
                    resp = Connection?.ControlTransfer((UsbAddressing)64, 3, 26, 0, null, 0, 0);// baudrate  57600 115200-0x001A-26, 9600-0x4138-16696, 19200-0x809C-32924, 230040-0x000D-13
                    resp = Connection?.ControlTransfer((UsbAddressing)64, 2, 0, 0, null, 0, 0);// flow  control none                                                            
                    resp = Connection?.ControlTransfer((UsbAddressing)64, 4, 8, 0, null, 0, 0);// data bit  8, parity  none,  stop bit 1, tx off
                }

                SerialPorts.Add(device.DeviceName, new SerialDevice() { UsbDevice = device, Connection = Connection, EndpointRx = endpointRx, EndpointTx = EndpointTx });
            }
            if (SerialPorts.Any())
                return true;
            return false;
        }
        public IEnumerable<string> GetPorts()
        {
            //return SerialPorts.Keys;
            return SerialPorts.Keys ?? Enumerable.Empty<string>();
        }

        public async Task<string> Read(string portName, byte[] buffer)
        {
            //int size = 1024;
            //int sizeRx = 0;
            //byte[]? bytesRx = null;
            //byte[]? bytesTx = null;
            //string strRx = null;
            //string strTx = null;
            //ByteBuffer bytesBufferRx = null;
            //ByteBuffer bytesBufferRx = ByteBuffer.Allocate(size);
            //ByteBuffer bytesBufferTx = null;

            //while (true)
            //{
            // Tx
            //bytesTx = Encoding.UTF8.GetBytes("testread,3#");
            //resp = Connection?.BulkTransfer(EndpointTx, bytesTx, bytesTx.Length, 0); //do in another thread

            // Rx
            //bytesRx = new byte[size];
            SerialPorts.GetValueOrDefault(portName)?.Connection.BulkTransfer(SerialPorts.GetValueOrDefault(portName)?.EndpointRx, buffer, buffer.Length, 1000);

            //strRx = Encoding.UTF8.GetString(bytesRx);

            // Tx
            //bytesBufferTx = ByteBuffer.Allocate(size * 2);
            //bytesTx = Encoding.UTF8.GetBytes("testread,3#");
            //bytesBufferTx.Put(bytesTx, 0, bytesTx.Length);
            //
            //UsbRequest requestTx = new UsbRequest();
            //requestTx.Initialize(connection, endpointTx);
            //requestTx.Queue(bytesBufferTx, bytesTx.Length);
            //UsbRequest? responseTx = await connection?.RequestWaitAsync();
            //
            //bytesTx = new byte[size];
            //bytesBufferTx.Get(bytesTx, 0, bytesTx.Length);
            //bytesBufferTx.Position(0);
            //strTx = Encoding.UTF8.GetString(bytesTx);

            // Rx
            //UsbRequest requestRx = new UsbRequest();
            //requestRx.Initialize(Connection, endpointRx);
            //
            //bytesBufferRx.Clear();
            //requestRx.Queue(bytesBufferRx);
            //
            //UsbRequest? responseRx = await Connection?.RequestWaitAsync();
            //
            //bytesRx = new byte[size];
            //bytesBufferRx.Flip();
            //bytesBufferRx.Get(bytesRx, 0, bytesBufferRx.Limit());

            buffer = buffer.Where(b => ((b != 0x00) && (b != 0x01) && (b != 0x60))).ToArray();
            string data = Encoding.UTF8.GetString(buffer, 0, buffer.Length);
            return data;

            //if ((bytesRx != null) && (bytesRx.Length > 0))
            //{
            //    strRx += Encoding.UTF8.GetString(bytesRx, 0, bytesRx.Length);
            //
            //    Xamarin.Forms.Device.BeginInvokeOnMainThread((Action)(() =>
            //    {
            //        Data = strRx;
            //        UsbInterface.EventHendle?.Invoke((object)this, (EventArgs)new EventArgs());
            //    }));
            //}
            //}
        }

        //static private EventHandler EventHendle = null;
        //static private string Data = null;
        //static private UsbDeviceConnection Connection = null;
        //static private UsbEndpoint EndpointTx = null;

        //public string GetData()
        //{
        //    return Data;
        //}
        //
        public void Event(EventHandler eventRemoved, EventHandler eventAdded)
        {
            EventRemoved = eventRemoved;
            EventAdded = eventAdded;
        }

        //public async Task<int> Send(string data)
        public async Task Write(string portName, byte[] buffer)
        {
            //if ((Connection != null) && (EndpointTx != null))
            //{
            //int size = 64;
            //byte[]? bytesTx = null;
            //string strTx = null;
            //ByteBuffer bytesBufferTx = null;
            //int? resp = -1;

            //bytesTx = Encoding.UTF8.GetBytes(data);
            //resp = Connection?.BulkTransfer(EndpointTx, bytesTx, bytesTx.Length, 0); //do in another thread
            foreach (byte b in buffer)
            {
                Thread.Sleep(100);
                SerialPorts.GetValueOrDefault(portName)?.Connection.BulkTransfer(SerialPorts.GetValueOrDefault(portName)?.EndpointTx, new byte[] { b }, 1, 0);
            }

            // Tx
            //UsbRequest requestTx = new UsbRequest();
            //requestTx.Initialize(singletone.Connection, singletone.EndpointTx);
            //
            //bytesTx = Encoding.UTF8.GetBytes("testread,3#");
            //bytesBufferTx = ByteBuffer.Wrap(bytesTx);
            //
            //requestTx.Queue(bytesBufferTx);
            //UsbRequest? responseTx = await singletone.Connection?.RequestWaitAsync();
            //
            //bytesTx = new byte[size];
            //bytesBufferTx.Flip();
            //bytesBufferTx.Get(bytesTx, 0, bytesBufferTx.Limit());

            //strTx = Encoding.UTF8.GetString(bytesTx, 0, bytesBufferTx.Limit());

            //strTx = Encoding.UTF8.GetString(bytesTx, 0, bytesTx.Length);
            //
            //return 0;
            //}
            //return -1;
        }

    }
}