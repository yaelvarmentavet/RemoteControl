using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Threading.Tasks;

namespace RemoteControl.UWP
{
    public class UsbDevice : IUsbDevice
    {
        private string[] SerialPortNames;
        private Dictionary<string, SerialPort> SerialPorts;

        public UsbDevice()
        {
            SerialPortNames = SerialPort.GetPortNames();
        }

        public void Open()
        {
            foreach (string portName in SerialPortNames)
            {
                SerialPort serialPort = new SerialPort(portName);
                serialPort.BaudRate = 115200;
                serialPort.DataBits = 8;
                serialPort.Parity = Parity.None;
                serialPort.StopBits = StopBits.One;
                //serialPort.Handshake = Handshake.None;
                serialPort.ReadTimeout = 1000;
                serialPort.WriteTimeout = 1000;
                serialPort.ReadBufferSize = 1024 * 16;
                serialPort.PortName = portName;

                try
                {
                    serialPort.Open();

                    SerialPorts.Add(portName, serialPort);
                }
                catch { }
            }
        }

        public void Read(string portName, byte[] buffer)
        {
            SerialPort serialPort = SerialPorts.GetValueOrDefault(portName);
            serialPort?.Read(buffer, 0, buffer.Length);
        }

        public void Write(string portName, byte[] buffer)
        {
            SerialPort serialPort = SerialPorts.GetValueOrDefault(portName);
            serialPort?.Write(buffer, 0, buffer.Length);
        }

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
