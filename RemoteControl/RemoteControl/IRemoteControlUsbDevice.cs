using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RemoteControl
{
    public interface IRemoteControlUsbDevice
    {
        string GetData();
        void Event(EventHandler eventHandler);
        Task<int> Send(string data);
    }
}
