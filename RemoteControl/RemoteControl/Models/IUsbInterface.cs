using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemoteControl
{
    public interface IUsbInterface
    {
        bool GetInitDone();
        IEnumerable<string> GetPorts();
        Task<string> Read(string portName, byte[] buffer);
        Task Write(string portName, byte[] buffer);
        string GetData();
        void Event(EventHandler eventHandler);
        Task<int> Send(string data);
    }
}
