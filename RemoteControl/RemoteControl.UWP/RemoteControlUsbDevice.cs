using System;
using System.Threading.Tasks;

namespace RemoteControl.UWP
{
    public class RemoteControlUsbDevice : IRemoteControlUsbDevice
    {
        public string GetData()
        {
            return null;
        }
        public void Event(EventHandler eventHandler)
        { }
        public async Task<int> Send(string data)
        {
            return -1;
        }
    }
}
