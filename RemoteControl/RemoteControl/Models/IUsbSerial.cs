using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemoteControl
{
    public interface IUsbSerial
    {
        //Task<bool> Connect();
        //Task Disconnect();
        IEnumerable<string> GetPorts();
        Task<int> Read(string portName, byte[] buffer);
        Task<int> Write(string portName, byte[] buffer);
        //void Event(EventHandler eventRemoved, EventHandler eventAdded);
    }
}
