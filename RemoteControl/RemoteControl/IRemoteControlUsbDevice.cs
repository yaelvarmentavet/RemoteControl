using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RemoteControl
{
    public interface IRemoteControlUsbDevice
    {
        string GetId();
        void Event(EventHandler eventHandler);
    }
}
