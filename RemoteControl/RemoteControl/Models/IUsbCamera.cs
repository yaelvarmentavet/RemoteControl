using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace RemoteControl.Models
{
    public interface IUsbCamera
    {
        void Image(object image);
        void Event(EventHandler eventSource);
    }
}
