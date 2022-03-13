using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemoteControl
{
    public interface IUsbSerial
    {
        Task<bool> Connect();
        IEnumerable<string> GetPorts();
        Task<string> Read(string portName, byte[] buffer);
        Task Write(string portName, byte[] buffer);
        void Event(EventHandler eventRemoved, EventHandler eventAdded);
    }
}
