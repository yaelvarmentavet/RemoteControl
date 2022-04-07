using System;
using System.Collections.Generic;
using System.Text;
using Windows.Storage;
using Xamarin.Forms;

namespace RemoteControl.Models
{
    public interface IUsbCamera
    {
        void ImageSet(Xamarin.Forms.Image image);
        void File(string outputFile);
        void Event(EventHandler eventSource);
    }
}
