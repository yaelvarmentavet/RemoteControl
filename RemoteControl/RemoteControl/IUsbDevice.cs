using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RemoteControl
{
    public interface IUsbDevice
    {
        void Open();
        void Read(string portName, byte[] buffer);
        void Write(string portName, byte[] buffer);
        string GetData();
        void Event(EventHandler eventHandler);
        Task<int> Send(string data);
    }
}
