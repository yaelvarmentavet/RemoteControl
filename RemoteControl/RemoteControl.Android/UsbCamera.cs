using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using RemoteControl.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace RemoteControl.Droid
{
    class UsbCamera : IUsbCamera
    {
        public void Event(EventHandler eventSource)
        {
        }

        public void File(string outputFile)
        {
        }

        public void ImageSet(Image image)
        {
        }
    }
}