using System;

namespace RemoteControl.UWP
{
    public class RemoteControlUsbDevice : IRemoteControlUsbDevice
    {
        public string GetId()
        {
            return null;
        }
        public void Event(EventHandler eventHandler)
        { }
    }
}
