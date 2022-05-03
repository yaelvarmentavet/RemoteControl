using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Media.Capture;
using Windows.System.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace RemoteControl.UWP
{
    public sealed partial class MainPage
    {

        public MainPage()
        {
            //Xamarin.Forms.DependencyService.Register<UsbDevice>();

            this.InitializeComponent();

            Bootstrap.Initialize();
            App.UsbCamera = new UsbCamera();

            LoadApplication(new RemoteControl.App());
        }
    }
}
