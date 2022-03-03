using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.SerialCommunication;

namespace RemoteControl
{
    public interface IUsbInterface
    {
        IEnumerable<string> GetPorts();
        Task<string> Read(string portName, byte[] buffer);
        void Write(string portName, byte[] buffer);
        string GetData();
        void Event(EventHandler eventHandler);
        Task<int> Send(string data);
    }
}
