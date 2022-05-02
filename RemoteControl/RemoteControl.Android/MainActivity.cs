
using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Android.Hardware.Usb;
using Android.Content;
using Autofac;
using RemoteControl.Models;
using Autofac.Extras.CommonServiceLocator;
using CommonServiceLocator;
using System;

//[assembly: Xamarin.Forms.Dependency(typeof(DeviceInfo))]
namespace RemoteControl.Droid
{
    //[Activity(Label = "Armenta - Remote Control Application", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    [Activity(Label = "Armenta - Remote Control Application", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        public static UsbManager Manager;
        public static BroadcastReceiverSystem UsbReciever = new BroadcastReceiverSystem();
        public static EventHandler UsbEvent;
        
        //public static PendingIntent mPermissionIntent = null;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            //PendingIntent mAttachedIntent = PendingIntent.GetBroadcast(Application.Context, 0, new Intent(UsbManager.ActionUsbDeviceAttached), 0);
            IntentFilter filterA = new IntentFilter(UsbManager.ActionUsbDeviceAttached);
            IntentFilter filterD = new IntentFilter(UsbManager.ActionUsbDeviceDetached);
            RegisterReceiver(UsbReciever, filterA);
            RegisterReceiver(UsbReciever, filterD);

            //Xamarin.Forms.DependencyService.Register<UsbDevice>();

            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            //ContainerBuilder builder = new ContainerBuilder();
            //builder.RegisterType<Data>().AsSelf();
            //builder.RegisterType<UsbDevice>().As<IUsbDevice>();

            //IContainer container = builder.Build();

            //AutofacServiceLocator asl = new AutofacServiceLocator(container);
            //ServiceLocator.SetLocatorProvider(() => asl);
            Bootstrap.Initialize();
            
            LoadApplication(new App());
            
            Manager = GetSystemService(Context.UsbService) as UsbManager;
            UsbEvent.Invoke(this, EventArgs.Empty);
            //PendingIntent mPermissionIntent = PendingIntent.GetBroadcast(this, 0, new Intent("android.permission.USB_PERMISSION"), 0);
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}