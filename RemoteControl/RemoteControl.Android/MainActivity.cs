using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Android.Hardware.Usb;
using Android.Content;
using System.Reflection;
using System.Collections;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//[assembly: Xamarin.Forms.Dependency(typeof(DeviceInfo))]
namespace RemoteControl.Droid
{
    [Activity(Label = "Armenta - Remote Control Application", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        public static UsbManager Manager = null;
        //public static PendingIntent mPermissionIntent = null;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            Xamarin.Forms.DependencyService.Register<RemoteControlUsbDevice>();

            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
           
            Manager = GetSystemService(Context.UsbService) as UsbManager;
            //PendingIntent mPermissionIntent = PendingIntent.GetBroadcast(this, 0, new Intent("android.permission.USB_PERMISSION"), 0);
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}