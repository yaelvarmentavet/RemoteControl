using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace RemoteControl.Models
{
    //public enum EError
    //{
    //    ERROR = -1,
    //    OK = 0,
    //}

    //public enum ECount
    //{
    //    //public static readonly byte COUNTDOWN = 0x01;
    //    //public static readonly byte COUNTUP = 0x02;
    //    DOWN = 1,
    //    UP = 2
    //}

    //public enum EPulses
    //{
    //    //public static readonly UInt16 PULSES100 = 100;
    //    //public static readonly UInt16 PULSES400 = 400;
    //    PULSES100 = 100,
    //    PULSES400 = 400
    //}

    //public enum EProcess
    //{
    //    //public static readonly byte STATUS = 0x00;
    //    //public static readonly byte START = 0x01;
    //    //public static readonly byte STOP = 0x02;
    //    STATUS = 0,
    //    START = 1,
    //    STOP = 2
    //}

    public class Aptx : INotifyPropertyChanged
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        unsafe struct PacketStatus
        {
            [MarshalAs(UnmanagedType.U2)]
            public byte STX;
            public byte APT_SERIAL_NUMBER;
            public byte AM_number_msb;
            public byte AM_number_2;
            public byte AM_number_3;
            public byte AM_number_lsb;
            public byte Max_number_msb;
            public byte Max_number_2;
            public byte Max_number_3;
            public byte Max_number_lsb;
            public byte Current_number_msb;
            public byte Current_number_2;
            public byte Current_number_3;
            public byte Current_number_lsb;
            public byte Apt_number_msb;
            public byte Apt_number_2;
            public byte Apt_number_3;
            public byte Apt_number_lsb;
            public byte Pressure_flag;
            public byte Battery_flag;
            public byte motor_is_running;
            public byte Apt_pulses_flag;
            public byte errors;
            public byte motor_temperature;
            public byte motor_voltage;
            public byte speed_of_bullet;
            public byte Cow_id_msb;
            public byte Cow_id_lsb;
            public byte reserved;
            public byte reserved1;
            public byte reserved2;
            public byte reserved3;
            public byte ETX;
            public byte Check_sum_msb;
            public byte Check_sum_lsb;
        }

        private uint id = UERROR;
        public uint Id
        {
            get => id;
            set
            {
                id = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Id)));
            }
        }

        private uint snum = UERROR;
        public uint SNum
        {
            get => snum;
            set
            {
                snum = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SNum)));
            }
        }

        private uint current = UERROR;
        public uint Current { get => current; set => current = value; }

        private uint maxi = UERROR;
        public uint Maxi { get => maxi; set => maxi = value; }

        private uint remaining = UERROR;
        public uint Remaining
        {
            get => remaining;
            set
            {
                remaining = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Remaining)));
            }
        }

        private uint pulses = UERROR;
        public uint Pulses
        {
            get => pulses;
            set
            {
                pulses = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Pulses)));
            }
        }

        private uint[] aptxid = new uint[3] { UERROR, UERROR, UERROR };
        public uint[] aptxId
        {
            get => aptxid;
            set
            {
                aptxid = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(aptxId)));
            }
        }

        public string AptxId
        {
            get => aptxid.Aggregate("", (r, m) => r += m.ToString("X") + "   ");
            set
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AptxId)));
            }
        }

        private uint pressure = UERROR;
        public uint Pressure
        {
            get => pressure;
            set
            {
                pressure = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Pressure)));
            }
        }

        private uint battery = UERROR;
        public uint Battery
        {
            get => battery;
            set
            {
                battery = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Battery)));
            }
        }

        private uint motorisrunning = UERROR;
        public uint Motorisrunning
        {
            get => motorisrunning;
            set
            {
                motorisrunning = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Motorisrunning)));
            }
        }

        private uint aptpulses = UERROR;
        public uint Aptpulses
        {
            get => aptpulses;
            set
            {
                aptpulses = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Aptpulses)));
            }
        }

        private uint motortemperature = UERROR;
        public uint Motortemperature
        {
            get => motortemperature;
            set
            {
                motortemperature = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Motortemperature)));
            }
        }

        private uint motorvoltage = UERROR;
        public uint Motorvoltage
        {
            get => motorvoltage;
            set
            {
                motorvoltage = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Motorvoltage)));
            }
        }

        private uint speedofbullet = UERROR;
        public uint Speedofbullet
        {
            get => speedofbullet;
            set
            {
                speedofbullet = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Speedofbullet)));
            }
        }

        private uint _cowid = UERROR;
        public uint _CowId
        {
            get => _cowid;
            set
            {
                _cowid = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(_CowId)));
            }
        }

        public string StatusMessage
        {
            get
            {
                string sts = string.Empty;
                if (Pressure != 1)
                    sts += "Pressure fault\n";
                if (Battery != 1)
                    sts += "Battery fault\n";
                if (Motortemperature != 1)
                    sts += "Motor temperature fault\n";
                if (Motorvoltage != 1)
                    sts += "Motor voltage fault\n";
                if (Speedofbullet != 1)
                    sts += "Speed of bullet fault\n";
                return sts;
            }
        }

        public Color StatusColor
        {
            get => ((Pressure == 1) && (Battery == 1) && (Motortemperature == 1) &&
                    (Motorvoltage == 1) && (Speedofbullet == 1)) ? Color.Cyan : Color.Red;
        }

        private const uint UERROR = 0xFFFFFFFF;
        private const int ERROR = -1;
        private const int OK = 0;
        public static readonly byte STX = 0xBB;
        public static readonly byte ETX = 0x7E;
        public static readonly byte COUNTDOWN = 0x01;
        public static readonly byte COUNTUP = 0x02;
        public static readonly UInt16 PULSES100 = 100;
        public static readonly UInt16 PULSES400 = 400;
        public static readonly byte STATUS = 0x00;
        public static readonly byte START = 0x01;
        public static readonly byte STOP = 0x02;
        public static readonly byte RESERVED = 0x00;

        public uint PulsesPrev = UERROR;
       
        public event PropertyChangedEventHandler PropertyChanged;

        public static uint PacketGetId(byte [] buffer)
        {
            uint id = UERROR;
            unsafe //we'll now pin unmanaged struct over managed byte array
            {
                fixed (byte* pbuffer = buffer)
                {
                    PacketStatus* packetStatus = (PacketStatus*)pbuffer;
                    id = packetStatus->APT_SERIAL_NUMBER;
                }
            }
            return id;
        }

        public void PacketParse(byte[] buffer)
        {
            unsafe //we'll now pin unmanaged struct over managed byte array
            {
                fixed (byte* pbuffer = buffer)
                {
                    PacketStatus* packetStatus = (PacketStatus*)pbuffer;
                    Id = packetStatus->APT_SERIAL_NUMBER;
                    SNum = ArrayToUint32((byte*)&packetStatus->AM_number_msb);
                    Maxi = ArrayToUint32((byte*)&packetStatus->Max_number_msb);
                    Pulses = ArrayToUint32((byte*)&packetStatus->Current_number_msb);
                    aptxId[0] = ArrayToUint32((byte*)&packetStatus->Apt_number_msb);
                    aptxId[1] = 0;
                    aptxId[2] = 0;
                    aptxId[3] = 0;
                    Pressure = packetStatus->Pressure_flag;
                    Battery = packetStatus->Battery_flag;
                    Motorisrunning = packetStatus->motor_is_running;
                    Aptpulses = packetStatus->Apt_pulses_flag;
                    Motortemperature = packetStatus->motor_temperature;
                    Motorvoltage = packetStatus->motor_voltage;
                    Speedofbullet = packetStatus->speed_of_bullet;
                    _CowId = ArrayToUint16((byte*)&packetStatus->Cow_id_msb);
                }
            }
        }

        private unsafe uint ArrayToUint16(byte* buffer)
        {
            return (uint)(*buffer << 8) + (uint)(*(buffer + 1));
        }

        private byte[] Uint16ToArray(UInt16 buffer)
        {
            return new byte[] { (byte)(buffer & 0xFF00 >> 8), (byte)(buffer & 0x00FF) };
        }

        private unsafe uint ArrayToUint32(byte* buffer)
        {
            return (uint)(*(buffer) << 24) +
                (uint)(*(buffer + 1) << 16) +
                (uint)(*(buffer + 2) << 8) +
                (uint)(*(buffer + 3));
        }

        //public async Task<int> Command(IUsbSerial usbSerial, string port, byte count, UInt16 pulses, byte process)
        //{
        //    int response = ERROR;
        //    byte[] bpulses = Uint16ToArray(pulses);
        //    response = await usbSerial.Write(port, Checksum(new byte[] { STX, (byte)Id,
        //            count, bpulses[0], bpulses[1], process,
        //            RESERVED, RESERVED, RESERVED,
        //            ETX}).ToArray());
        //    //File.AppendAllText(LOGFILE_COWS, string.Format("Date: {0} {1} Action: {2} {3} Cow Id: {4} Current Pulses: {5}\n",
        //    //    DateTime.Now, response == OK ? "OK" : "FAULT", action, pulses, CowId, Pulses));
        //    return response;
        //}

        public byte[] PacketBuild (byte count, UInt16 pulses, byte process)
        {
            byte[] bpulses = Uint16ToArray(pulses);
            return Checksum(new byte[] { STX, (byte)Id,
                    count, bpulses[0], bpulses[1], process,
                    RESERVED, RESERVED, RESERVED,
                    ETX}).ToArray();
        }

        private byte[] Checksum(byte[] buffer)
        {
            UInt16 sum = 0;
            foreach (byte b in buffer)
                sum += b;
            return buffer.Concat(new byte[] { (byte)(sum & 0xFF00 >> 8), (byte)(sum & 0x00FF) }).ToArray();
        }

        public byte[] PacketBuildPulses100(byte process)
        {
            return PacketBuild(COUNTDOWN, PULSES100, process);
        }

        public byte[] PacketBuildCountDown(UInt16 pulses, byte process)
        {
            return PacketBuild(COUNTDOWN, pulses, process);
        }

        public byte[] PacketBuildStatus()
        {
            return PacketBuild(RESERVED, RESERVED, STATUS);
        }
    }

    public class DataModel : INotifyPropertyChanged
    {
        private uint cowid = UERROR;
        public uint CowId
        {
            get => cowid;
            set
            {
                cowid = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CowId)));
            }
        }

        private string tagid = string.Empty;

        public string TagId
        {
            get => tagid;
            set
            {
                tagid = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TagId)));
            }
        }

        public float Progress
        {
            get => (Aptxs[0].Pulses - Aptxs[0].PulsesPrev) / Aptxs[0].PulsesPrev;
        }

        private bool fl = false;
        public Color FL
        {
            get => fl ? Color.Cyan : Color.AliceBlue;
            set
            {
                fl = value == Color.Cyan ? true : false;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FL)));
            }
        }

        private bool rl = false;
        public Color RL
        {
            get => rl ? Color.Cyan : Color.AliceBlue;
            set
            {
                rl = value == Color.Cyan ? true : false;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RL)));
            }
        }

        private bool fr = false;
        public Color FR
        {
            get => fr ? Color.Cyan : Color.AliceBlue;
            set
            {
                fr = value == Color.Cyan ? true : false;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FR)));
            }
        }

        private bool rr = false;
        public Color RR
        {
            get => rr ? Color.Cyan : Color.AliceBlue;
            set
            {
                rr = value == Color.Cyan ? true : false;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RR)));
            }
        }

        private const uint UERROR = 0xFFFFFFFF;
        private const int OK = 0;
        private const int ERROR = -1;

        private const string APTX1 = "APTX1";
        private const string REMOTE = "REMOTE";
        private const string ECOMILK = "ECOMILK";
        private const string RFID = "RFID";
        
        //private readonly byte[] APTXIDs = new byte[] { 0x01, 0x02, 0x03, 0x04 };
        private readonly uint[] STATEs = new uint[] { 0, 1, 2, 3 };
        
        private const long CONNECT_TIMEOUT = 3000;
        private readonly long STATE_TIMEOUT = 3000; // in msec
        private readonly long REQUEST_TIMEOUT = 1000; // in msec

        private readonly string LOGFILE_COWS = "LogFileCows";

        public Aptx[] Aptxs;

        private Dictionary<uint, string> Cows = new Dictionary<uint, string>();

        public event PropertyChangedEventHandler PropertyChanged;

        public Command AddCow { get; }
        public Command TappedFL { get; }
        public Command TappedRL { get; }
        public Command TappedFR { get; }
        public Command TappedRR { get; }

        private IUsbSerial UsbSerial;
        //public string APTXPort = string.Empty;
        //public string EcomilkPort = string.Empty;
        private Dictionary<string, string> Ports = new Dictionary<string, string>();
        private bool Connected = false;
        private byte PauseResume = Aptx.STOP;

        public DataModel(IUsbSerial usbSerial)
        {
            Aptxs = new Aptx[] { new Aptx(), new Aptx(), new Aptx(), new Aptx() };
            UsbSerial = usbSerial;

            AddCow = new Command(() =>
            {
                Cows.Add(CowId, TagId);
                CowId = UERROR;
                TagId = string.Empty;
            });

            //TappedFL = new Command<Label>((lbl) =>
            TappedFL = new Command(() =>
            {
                //FL = lbl.BackgroundColor == Color.Cyan ? Color.AliceBlue : Color.Cyan;
                fl = fl ? false : true;
                FL = FL;
                //lbl.SetDynamicResource(Label.BackgroundColorProperty, "BackgroundPressed");
            });

            TappedRL = new Command(() =>
            {
                //FL = lbl.BackgroundColor == Color.Cyan ? Color.AliceBlue : Color.Cyan;
                rl = rl ? false : true;
                RL = RL;
                //lbl.SetDynamicResource(Label.BackgroundColorProperty, "BackgroundPressed");
            });

            TappedFR = new Command(() =>
            {
                //FL = lbl.BackgroundColor == Color.Cyan ? Color.AliceBlue : Color.Cyan;
                fr = fr ? false : true;
                FR = FR;
                //lbl.SetDynamicResource(Label.BackgroundColorProperty, "BackgroundPressed");
            });

            TappedRR = new Command(() =>
            {
                //FL = lbl.BackgroundColor == Color.Cyan ? Color.AliceBlue : Color.Cyan;
                rr = rr ? false : true;
                RR = RR;
                //lbl.SetDynamicResource(Label.BackgroundColorProperty, "BackgroundPressed");
            });

            //UsbSerial.Event(
            //    new EventHandler((sender, args) =>
            //    {
            //        Ports = new Dictionary<string, string>();
            //        Connected = false;
            //    }),
            //    new EventHandler((sender, args) =>
            //    {
            Connected = true;
            new Thread(async (device) =>
                { await TxRx(device); })
            { Name = "EcomilkTxRx" }.Start(ECOMILK);

            new Thread(async (device) =>
            { await TxRx(device); })
            { Name = "RemoteTxRx" }.Start(REMOTE);

            new Thread(async (device) =>
            { await TxRx(device); })
            { Name = "RfidTxRx" }.Start(RFID);

            new Thread(async (device) =>
                { await TxRx(device); })
            { Name = "Aptx1TxRx" }.Start(APTX1);
        }

        private async Task TxRx(object odevice)
        {
            string device = odevice as string;
            if (device != null)
            {
                string data = string.Empty;
                while (Connected)
                {
                    while (await UsbSerial.Connect())
                    {
                        Stopwatch stopWatch = new Stopwatch();
                        stopWatch.Start();
                        while (!Ports.TryGetValue(device, out string prt) || (stopWatch.ElapsedMilliseconds > CONNECT_TIMEOUT))
                        {
                            foreach (string port in UsbSerial.GetPorts())
                            {
                                if (stopWatch.ElapsedMilliseconds % REQUEST_TIMEOUT == 0)
                                    await PortConnectRequest(device, port);

                                //byte[] buffer = new byte[1024];
                                //data += await UsbSerial.Read(device, buffer);
                                //await UsbSerial.Read(port, buffer);
                                if (await PortConnectReply(device, port))
                                    Ports.Add(device, port);
                            }
                        }
                        //data = string.Empty;
                        uint state = STATEs.First();
                        while (Ports.TryGetValue(device, out string port))
                        {
                            if (stopWatch.ElapsedMilliseconds % REQUEST_TIMEOUT == 0)
                            {
                                if (stopWatch.ElapsedMilliseconds % STATE_TIMEOUT == 0)
                                    state = state == STATEs.Last() ? STATEs.First() : state++;
                                await PortDataRequest(device, state);
                            }

                            //byte[] buffer = new byte[1024];
                            //data += await UsbSerial.Read(port, buffer);
                            //if (await UsbSerial.Read(port, buffer) > 0)
                            //{
                            if (!await PortDataReply(device, port))
                                Ports.Remove(device);
                        }
                    }
                }
            }
        }
        //{ Name = "UsbTxRx" }.Start();
        
        //    new Thread(async () =>
        //    {
        //        while (Connected)
        //        {
        //            while (await UsbSerial.Connect())
        //            {
        //                while (Ports.Count() < UsbSerial.GetPorts().Count())
        //                //while (string.IsNullOrEmpty(Ports.TryGetValue(REMOTE, out var val) ? val : string.Empty))
        //                {
        //                    Thread.Sleep(1000);

        //                    //if (string.IsNullOrEmpty(APTXPort) || string.IsNullOrEmpty(EcomilkPort))
        //                    foreach (string port in UsbSerial.GetPorts())
        //                        await UsbSerial.Write(port, Encoding.UTF8.GetBytes("getid,3#"));
        //                }

        //                while (Ports.Count() == UsbSerial.GetPorts().Count())
        //                //while (Connected)
        //                {
        //                    Thread.Sleep(1000);

        //                    if (SNum == UERROR)
        //                        await UsbSerial.Write(Ports.TryGetValue(REMOTE, out var val) ? val : string.Empty, Encoding.UTF8.GetBytes("testread,3#"));
        //                    else if (Current == UERROR)
        //                        //App.DataModel.UsbDevice.Send("find,3#");
        //                        await UsbSerial.Write(Ports.TryGetValue(REMOTE, out var val) ? val : string.Empty, Encoding.UTF8.GetBytes("find,3#"));
        //                    else if ((aptxId[0] == UERROR) ||
        //                              (aptxId[1] == UERROR) ||
        //                              (aptxId[2] == UERROR))
        //                        //App.DataModel.UsbDevice.Send("readid#");
        //                        await UsbSerial.Write(Ports.TryGetValue(REMOTE, out var val) ? val : string.Empty, Encoding.UTF8.GetBytes("readid#"));
        //                }
        //            }
        //        }
        //    })
        //    { Name = "UsbTx" }.Start();
        //    //}));
        //}

        private async Task<bool> PortDataReply(string device, string port)
        {
            byte[] buffer = new byte[1024];
            //data += await UsbSerial.Read(port, buffer);
            if (await UsbSerial.Read(port, buffer) > 0)
            {
                switch(device)
                {
                    case ECOMILK:
                        break;
                    case REMOTE:
                        Aptxs.Single(aptx => aptx.Id == Aptx.PacketGetId(buffer)).PacketParse(buffer);
                        break;
                    case APTX1:
                        string data = Encoding.UTF8.GetString(buffer);
                        if (Aptxs[0].SNum == UERROR)
                        {
                            if (data.Contains("SNUM"))
                            {
                                uint[] snum = DataParse(data, "SNUM", NumberStyles.Number);
                                Aptxs[0].SNum = snum[0];
                            }
                        }
                        if ((Aptxs[0].aptxId[0] == UERROR) ||
                            (Aptxs[0].aptxId[1] == UERROR) ||
                            (Aptxs[0].aptxId[2] == UERROR))
                        {
                            if (data.Contains("readid Device_id"))
                            {
                                uint[] aptid = DataParse(data, "readid Device_id", NumberStyles.HexNumber);
                                Aptxs[0].aptxId[0] = aptid[0];
                                Aptxs[0].aptxId[1] = aptid[1];
                                Aptxs[0].aptxId[2] = aptid[2];
                                Aptxs[0].AptxId = Aptxs[0].AptxId;
                            }
                        }
                        if ((Aptxs[0].Current == UERROR) || (Aptxs[0].Maxi == UERROR))
                        {
                            if (data.Contains("MAXI"))
                            {
                                uint[] maxi = DataParse(data, "MAXI", NumberStyles.Number);
                                Aptxs[0].Maxi = maxi[0];
                            }
                            if (data.Contains("Found:"))
                            {
                                uint[] current = DataParse(data, "Found:", NumberStyles.Number);
                                Aptxs[0].Current = current[0];
                            }
                            if ((Aptxs[0].Current != UERROR) && (Aptxs[0].Maxi != UERROR))
                                Aptxs[0].Remaining = Aptxs[0].Maxi - Aptxs[0].Current;
                        }
                        break;
                    default:
                        return false;
                }
                return true;
            }
            return false;
        }

        private async Task PortDataRequest(string device, uint state)
        {
            switch (device)
            {
                case ECOMILK:
                    break;
                case REMOTE:
                    {
                        await UsbSerial.Write(Ports.TryGetValue(REMOTE, out string val) ? val : string.Empty,
                            Aptxs[state].PacketBuildStatus());
                    }
                        break;
                case APTX1:
                    if (Aptxs[0].SNum == UERROR)
                        await UsbSerial.Write(Ports.TryGetValue(device, out var val) ? val : string.Empty, Encoding.UTF8.GetBytes("testread,3#"));
                    else if (Aptxs[0].Current == UERROR)
                        await UsbSerial.Write(Ports.TryGetValue(device, out var val) ? val : string.Empty, Encoding.UTF8.GetBytes("find,3#"));
                    else if ((Aptxs[0].aptxId[0] == UERROR) ||
                              (Aptxs[0].aptxId[1] == UERROR) ||
                              (Aptxs[0].aptxId[2] == UERROR))
                        await UsbSerial.Write(Ports.TryGetValue(device, out var val) ? val : string.Empty, Encoding.UTF8.GetBytes("readid#"));
                    break;
            }
        }

        private async Task PortConnectRequest(string device, string port)
        {
            switch (device)
            {
                case ECOMILK:
                case APTX1:
                    await UsbSerial.Write(port, Encoding.UTF8.GetBytes("getid,3#"));
                    break;
                case REMOTE:
                    await UsbSerial.Write(port, Aptxs[0].PacketBuildStatus());
                    break;
            }
        }

        private async Task<bool> PortConnectReply(string device, string port)
        {
            byte[] buffer = new byte[1024];
            //data += await UsbSerial.Read(device, buffer);
            await UsbSerial.Read(port, buffer);

            bool found = false;
            if (!Ports.ContainsKey(device))
            {
                switch (device)
                {
                    case ECOMILK:
                        {
                            string data = Encoding.UTF8.GetString(buffer);
                            found = data.Contains(device);
                            break;
                        }
                    case REMOTE:
                        {
                            found = buffer.Contains(Aptx.STX);
                            break;
                        }
                    case APTX1:
                        {
                            string data = Encoding.UTF8.GetString(buffer);
                            found = data.Contains("1F-85-01");
                            break;
                        }
                    default:
                        return false;
                }
            }
            return found;
        }

        //private void Aptx1DataParse(byte[] buffer)
        //{
        //    string data = Encoding.UTF8.GetString(buffer);
        //    if (SNum == UERROR)
        //    {
        //        if (data.Contains("SNUM"))
        //        {
        //            uint[] snum = DataParse(data, "SNUM", NumberStyles.Number);
        //            SNum = snum[0];
        //        }
        //    }
        //    if ((aptxId[0] == UERROR) ||
        //        (aptxId[1] == UERROR) ||
        //        (aptxId[2] == UERROR))
        //    {
        //        if (data.Contains("readid Device_id"))
        //        {
        //            uint[] aptid = DataParse(data, "readid Device_id", NumberStyles.HexNumber);
        //            aptxId[0] = aptid[0];
        //            aptxId[1] = aptid[1];
        //            aptxId[2] = aptid[2];
        //            AptxId = AptxId;
        //        }
        //    }
        //    if ((Current == UERROR) || (Maxi == UERROR))
        //    {
        //        if (data.Contains("MAXI"))
        //        {
        //            uint[] maxi = DataParse(data, "MAXI", NumberStyles.Number);
        //            Maxi = maxi[0];
        //        }
        //        if (data.Contains("Found:"))
        //        {
        //            uint[] current = DataParse(data, "Found:", NumberStyles.Number);
        //            Current = current[0];
        //        }
        //        if ((Current != UERROR) && (Maxi != UERROR))
        //            Remaining = Maxi - Current;
        //    }
        //}

        private uint[] DataParse(string data, string pattern, NumberStyles numberStyles)
        {
            uint[] num = new uint[6];
            string snum = new string(data?.Substring(data.IndexOf(pattern) + pattern.Length)?.TakeWhile(c => ((c != '\r') && (c != 'p')))?.ToArray());
            if (((numberStyles == NumberStyles.HexNumber) && (snum.Length <= 8)) ||
                ((numberStyles == NumberStyles.Number) && (snum.Length <= 10)))
            {
                uint.TryParse(snum,
                             NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | numberStyles,
                             CultureInfo.InvariantCulture,
                             out num[0]);
            }
            else
            {
                for (int i = 0; i < snum.Length / 8; i++)
                {
                    uint.TryParse(new string(snum.Skip(i * 8).Take(8).ToArray()),
                                 NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | numberStyles,
                                 CultureInfo.InvariantCulture,
                                 out num[i]);
                }
            }
            return num;
        }

        public async Task<int> StartProcess()
        {
            foreach (Aptx aptx in Aptxs)
            {
                if (await UsbSerial.Write(Ports.TryGetValue(REMOTE, out var val) ? val : string.Empty,
                    aptx.PacketBuildPulses100(Aptx.START)) == ERROR)
                    return ERROR;
            }
            return OK;
        }

        public async Task<int> StopProcess()
        {
            foreach (Aptx aptx in Aptxs)
            {
                if (await UsbSerial.Write(Ports.TryGetValue(REMOTE, out string val) ? val : string.Empty,
                    aptx.PacketBuildPulses100(Aptx.STOP)) == ERROR)
                    return ERROR;
            }
            return OK;
        }

        public async Task<int> PauseResumeProcess()
        {
            PauseResume = PauseResume == Aptx.STOP ? PauseResume = Aptx.START : PauseResume = Aptx.STOP;
            foreach (Aptx aptx in Aptxs)
            {
                if (await UsbSerial.Write(Ports.TryGetValue(REMOTE, out string val) ? val : string.Empty,
                    aptx.PacketBuildCountDown((UInt16)(Aptx.PULSES100 - (aptx.Pulses - aptx.PulsesPrev)), PauseResume)) == ERROR)
                    return ERROR;
            }
            return OK;
        }

        public async Task RCWStart()
        {
            await UsbSerial.Write(Ports.TryGetValue(ECOMILK, out var val) ? val : string.Empty, Encoding.UTF8.GetBytes("rcw 1"));
        }

        public async Task RCWStop()
        {
            await UsbSerial.Write(Ports.TryGetValue(ECOMILK, out var val) ? val : string.Empty, Encoding.UTF8.GetBytes("rcw 0"));
        }

        public async Task RCCWStart()
        {
            await UsbSerial.Write(Ports.TryGetValue(ECOMILK, out var val) ? val : string.Empty, Encoding.UTF8.GetBytes("rccw 1"));
        }

        public async Task RCCWStop()
        {
            await UsbSerial.Write(Ports.TryGetValue(ECOMILK, out var val) ? val : string.Empty, Encoding.UTF8.GetBytes("rccw 0"));
        }




        public async Task AFStart()
        {
            await UsbSerial.Write(Ports.TryGetValue(ECOMILK, out var val) ? val : string.Empty, Encoding.UTF8.GetBytes("af 1"));
        }

        public async Task AFStop()
        {
            await UsbSerial.Write(Ports.TryGetValue(ECOMILK, out var val) ? val : string.Empty, Encoding.UTF8.GetBytes("af 0"));
        }

        public async Task ABStart()
        {
            await UsbSerial.Write(Ports.TryGetValue(ECOMILK, out var val) ? val : string.Empty, Encoding.UTF8.GetBytes("ab 1"));
        }

        public async Task ABStop()
        {
            await UsbSerial.Write(Ports.TryGetValue(ECOMILK, out var val) ? val : string.Empty, Encoding.UTF8.GetBytes("ab 0"));
        }




        public async Task MZUStart()
        {
            await UsbSerial.Write(Ports.TryGetValue(ECOMILK, out var val) ? val : string.Empty, Encoding.UTF8.GetBytes("mzu 1"));
        }

        public async Task MZUStop()
        {
            await UsbSerial.Write(Ports.TryGetValue(ECOMILK, out var val) ? val : string.Empty, Encoding.UTF8.GetBytes("mzu 0"));
        }

        public async Task MZDStart()
        {
            await UsbSerial.Write(Ports.TryGetValue(ECOMILK, out var val) ? val : string.Empty, Encoding.UTF8.GetBytes("mzd 1"));
        }

        public async Task MZDStop()
        {
            await UsbSerial.Write(Ports.TryGetValue(ECOMILK, out var val) ? val : string.Empty, Encoding.UTF8.GetBytes("mzd 0"));
        }

        public async Task TCWStart()
        {
            await UsbSerial.Write(Ports.TryGetValue(ECOMILK, out var val) ? val : string.Empty, Encoding.UTF8.GetBytes("tcw 1"));
        }

        public async Task XFStart()
        {
            await UsbSerial.Write(Ports.TryGetValue(ECOMILK, out var val) ? val : string.Empty, Encoding.UTF8.GetBytes("xf 1"));
        }
    }
}
