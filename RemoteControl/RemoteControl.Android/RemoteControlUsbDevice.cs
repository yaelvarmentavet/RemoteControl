using System;
using Android.Hardware.Usb;
using System.Linq;
using System.Collections.Generic;
using Java.Nio;
using System.Text;
using System.Threading;
using Android.Runtime;
using Android.App;
using Android.Content;
using System.ComponentModel;

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

    public class RemoteControlUsbDevice : IRemoteControlUsbDevice
    {
        private RemoteControlUsbDevice singletone = null;
        public RemoteControlUsbDevice()
        {
            if (singletone != null)
                return;
            
            singletone = this;

            new Thread(async () =>
            {
                while (true)
                {
                    //UsbManager manager = GetSystemService(Context.UsbService) as UsbManager;

                    //Java.Util.HashMap devices = (Java.Util.HashMap)(manager?.DeviceList);
                    var devices = MainActivity.manager?.DeviceList;
                    UsbDevice device = (devices as IDictionary<string, UsbDevice>)?.Values.FirstOrDefault();

                    if (device != null)
                    {
                        PendingIntent mPermissionIntent = PendingIntent.GetBroadcast(Application.Context, 0, new Intent("android.permission.USB_PERMISSION"), 0);
                        MainActivity.manager?.RequestPermission(device, mPermissionIntent);
                        bool? hasPermision = MainActivity.manager?.HasPermission(device);
                        if (hasPermision == true)
                        {
                            UsbInterface intf = device?.GetInterface(0);
                            UsbEndpoint endpointRx = intf?.GetEndpoint(0);
                            UsbEndpoint endpointTx = intf?.GetEndpoint(1);
                            UsbDeviceConnection connection = MainActivity.manager?.OpenDevice(device);
                            if (connection != null)
                            {
                                connection?.ClaimInterface(intf, true);
                                int? resp = -1;

                                resp = connection?.ControlTransfer((UsbAddressing)64, 0, 0, 0, null, 0, 0);// reset  mConnection.controlTransfer(0×40, 0, 1, 0, null, 0, 0);//clear Rx
                                resp = connection?.ControlTransfer((UsbAddressing)64, 0, 1, 0, null, 0, 0);// clear Rx
                                resp = connection?.ControlTransfer((UsbAddressing)64, 0, 2, 0, null, 0, 0);// clear Tx
                                resp = connection?.ControlTransfer((UsbAddressing)64, 3, 26, 0, null, 0, 0);// baudrate  57600 115200-0x001A-26, 9600-0x4138-16696, 19200-0x809C-32924, 230040-0x000D-13
                                resp = connection?.ControlTransfer((UsbAddressing)64, 2, 0, 0, null, 0, 0);// flow  control none                                                            
                                resp = connection?.ControlTransfer((UsbAddressing)64, 4, 8, 0, null, 0, 0);// data bit  8, parity  none,  stop bit 1, tx off

                                //}
                                //    }
                                //}

                                //public async Task Read(byte[] bytesRx, int size)
                                //{
                                int size = 64;
                                byte[]? bytesRx = null;
                                byte[]? bytesTx = null;
                                string strRx = null;
                                string strTx = null;
                                ByteBuffer bytesBufferTx = null;
                                ByteBuffer bytesBufferRx = null;

                                while (true)
                                {
                                    //    bytesTx = Encoding.ASCII.GetBytes("testread,3#");
                                    //    resp = Connection?.BulkTransfer(EndpointTx, bytesTx, bytesTx.Length, 0); //do in another thread
                                    //    bytesRx = new byte[size];
                                    //    resp = Connection?.BulkTransfer(EndpointRx, bytesRx, bytesRx.Length, 300); //do in another thread
                                    //    strRx = Encoding.ASCII.GetString(bytesRx);
                                    //}

                                    //while (true)
                                    //{

                                    // Tx
                                    bytesBufferTx = ByteBuffer.Allocate(size * 2);
                                    bytesTx = Encoding.ASCII.GetBytes("testread,3#");
                                    bytesBufferTx.Put(bytesTx, 0, bytesTx.Length);

                                    UsbRequest requestTx = new UsbRequest();
                                    requestTx.Initialize(connection, endpointTx);
                                    requestTx.Queue(bytesBufferTx, bytesTx.Length);
                                    UsbRequest? responseTx = await connection?.RequestWaitAsync();

                                    bytesTx = new byte[size];
                                    bytesBufferTx.Get(bytesTx, 0, bytesTx.Length);
                                    bytesBufferTx.Position(0);
                                    strTx = Encoding.ASCII.GetString(bytesTx);

                                    // Rx
                                    bytesBufferRx = ByteBuffer.Allocate(size * 2);

                                    UsbRequest requestRx = new UsbRequest();
                                    requestRx.Initialize(connection, endpointRx);
                                    requestRx.Queue(bytesBufferRx, size);

                                    UsbRequest? responseRx = await connection?.RequestWaitAsync();

                                    bytesRx = new byte[size];
                                    bytesBufferRx.Position(0);
                                    bytesBufferRx.Get(bytesRx, 0, bytesRx.Length);
                                    strRx = Encoding.ASCII.GetString(bytesRx);

                                    //Activity.RunOnUiThread
                                    Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                                    {
                                        Id = strRx;
                                        eventHandler?.Invoke(this, new EventArgs());
                                    });
                                }
                            }
                        }
                    }
                }
            }).Start();
        }

        private EventHandler eventHandler = null;
        private string Id;

        public string GetId()
        {
            return singletone.Id;
        }

        public void Event(EventHandler eventHandler)
        {
            singletone.eventHandler = eventHandler;
        }
    }
}