using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RemoteControl.Models;
using System.IO.Ports;

namespace RemoteControl.iOS
{
    public class SerialDevice
    {
    }

    public class UsbSerial : IUsbSerial
    {
        private const int ERROR = -1;

        private Dictionary<string, SerialDevice> SerialPorts = new Dictionary<string, SerialDevice>();

        public UsbSerial()
        {
            SerialPort port = new SerialPort("/dev/cu.usbserial", 115200);
            port.Open();
            port.Write("Hello, world!");
            port.Close();
        }

        public IEnumerable<string> GetPorts()
        {
            return SerialPorts.Keys ?? Enumerable.Empty<string>();
        }

        public async Task<int> Read(string portName, byte[] buffer)
        {
            return ERROR;
        }

        public async Task<int> Write(string portName, byte[] buffer)
        {
            return ERROR;
        }
    }
}