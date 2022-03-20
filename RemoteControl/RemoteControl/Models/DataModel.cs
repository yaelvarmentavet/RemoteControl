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
using Xamarin.Essentials;
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

    public class Packet
    {
        public byte Sop;
        public byte Eop;

        public unsafe uint ArrayToUshort(byte* buffer)
        {
            return (uint)(*buffer << 8) + (uint)(*(buffer + 1));
        }

        public byte[] UshortToArray(ushort buffer)
        {
            return new byte[] { (byte)((buffer & 0xFF00) >> 8), (byte)(buffer & 0x00FF) };
        }

        public unsafe uint ArrayToUint(byte* buffer)
        {
            return (uint)(*(buffer) << 24) +
                (uint)(*(buffer + 1) << 16) +
                (uint)(*(buffer + 2) << 8) +
                (uint)(*(buffer + 3));
        }

        public bool PacketGet(ref byte[] buffer, int size)
        {
            if (buffer.Length >= size)
            {
                byte b;
                if ((b = buffer.FirstOrDefault(bf => bf == Sop)) != null)
                {
                    int i = buffer.ToList().IndexOf(b);
                    if ((buffer.Length - i) >= size)
                    {
                        if (buffer[i + size - 1 - 2] == Eop)
                        {
                            buffer = buffer.Skip(i).ToArray();
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }

    public class RfId : Packet, INotifyPropertyChanged
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        unsafe struct ReadTypeCUIIResponse
        {
            [MarshalAs(UnmanagedType.U2)]
            public byte Preamble;
            public byte MsgType;
            public byte Code;
            public byte PLMSB;
            public byte PLLSB;
            public byte PCMSB;
            public byte PCLSB;
            public byte EPCMSB;
            public byte EPC1;
            public byte EPC2;
            public byte EPC3;
            public byte EPC4;
            public byte EPC5;
            public byte EPC6;
            public byte EPC7;
            public byte EPC8;
            public byte EPC9;
            public byte EPC10;
            public byte EPCLSB;
            public byte End_Mark;
            public byte CRC_16MSB;
            public byte CRC_16LSB;
        }

        private byte[] epc;
        public byte[] EPC
        {
            get => epc;
            set
            {
                epc = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EPC)));
            }
        }

        private const uint UERROR = 0xFFFFFFFF;
        private const byte BERROR = 0xFF;

        public const byte PREEMBLE = 0xBB;
        public const byte END_MARK = 0x7E;
        private const byte MSG_TYPE_READ_TYPE_C_UII = 0x00;
        private const byte CODE_READ_TYPE_C_UII = 0x22;

        public event PropertyChangedEventHandler PropertyChanged;

        public RfId()
        {
            Sop = PREEMBLE;
            Eop = END_MARK;

            epc = new byte[12].Select(e => e = BERROR).ToArray();
        }

        public unsafe bool PacketParse(ref byte[] buffer)
        {
            //unsafe //we'll now pin unmanaged struct over managed byte array
            //{
            if (PacketGet(ref buffer, sizeof(ReadTypeCUIIResponse)))
            {
                fixed (byte* pbuffer = buffer)
                {
                    ReadTypeCUIIResponse* readTypeCUIIResponse = (ReadTypeCUIIResponse*)pbuffer;
                    if (ArrayToUshort((byte*)&readTypeCUIIResponse->CRC_16MSB) == CrcCalc(buffer.Skip(1).Take(sizeof(ReadTypeCUIIResponse) - 3).ToArray()))
                    {
                        EPC[0] = readTypeCUIIResponse->EPCMSB;
                        EPC[1] = readTypeCUIIResponse->EPC1;
                        EPC[2] = readTypeCUIIResponse->EPC2;
                        EPC[3] = readTypeCUIIResponse->EPC3;
                        EPC[4] = readTypeCUIIResponse->EPC4;
                        EPC[5] = readTypeCUIIResponse->EPC5;
                        EPC[6] = readTypeCUIIResponse->EPC6;
                        EPC[7] = readTypeCUIIResponse->EPC7;
                        EPC[8] = readTypeCUIIResponse->EPC8;
                        EPC[9] = readTypeCUIIResponse->EPC9;
                        EPC[10] = readTypeCUIIResponse->EPC10;
                        EPC[11] = readTypeCUIIResponse->EPCLSB;
                        buffer = buffer.Skip(sizeof(ReadTypeCUIIResponse)).ToArray();
                        return true;
                    }
                }
            }
            //}
            return false;
        }
        
        public byte[] PacketBuild()
        {
            return new byte[] { PREEMBLE}.Concat(
                CrcAppend(new byte[] { MSG_TYPE_READ_TYPE_C_UII, CODE_READ_TYPE_C_UII, 0, 0, END_MARK})).ToArray();
        }

        public ushort CrcCalc(byte[] buffer)
        {
            ushort crc = 0;
            for (int i = 0; i < buffer.Length; i++)
            {
                crc ^= (ushort)(buffer[i] << 8);
                for (int j = 0; j < 8; j++)
                {
                    if ((crc & 0x8000) != 0)
                        crc = (ushort)((crc << 1) ^ 0x1021);
                    else
                        crc <<= 1;
                }
            }
            return crc;
        }

        private byte[] CrcAppend(byte[] buffer)
        {
            //ushort crc = 0;
            //for (int i = 0; i < buffer.Length; i++)
            //{
            //    crc ^= (ushort)(buffer[i] << 8);
            //    for (int j = 0; j < 8; j++)
            //    {
            //        if ((crc & 0x8000) != 0)
            //            crc = (ushort)((crc << 1) ^ 0x1021);
            //        else
            //            crc <<= 1;
            //    }
            //}
            return buffer.Concat(UshortToArray(CrcCalc(buffer))).ToArray();
        }

    }

    public class Aptx : Packet, INotifyPropertyChanged
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
        public uint MotorIsRunning
        {
            get => motorisrunning;
            set
            {
                motorisrunning = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MotorIsRunning)));
            }
        }

        private uint aptpulses = UERROR;
        public uint AptPulses
        {
            get => aptpulses;
            set
            {
                aptpulses = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AptPulses)));
            }
        }

        private uint motortemperature = UERROR;
        public uint MotorTemperature
        {
            get => motortemperature;
            set
            {
                motortemperature = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MotorTemperature)));
            }
        }

        private uint motorvoltage = UERROR;
        public uint MotorVoltage
        {
            get => motorvoltage;
            set
            {
                motorvoltage = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MotorVoltage)));
            }
        }

        private uint speedofbullet = UERROR;
        public uint SpeedOfBullet
        {
            get => speedofbullet;
            set
            {
                speedofbullet = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SpeedOfBullet)));
            }
        }

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

        public string StatusMessage
        {
            get
            {
                string sts = string.Empty;
                if (Pressure == 1)
                    sts += "Pressure OK\n";
                else
                    sts += "Pressure fault\n";
                if (Battery == 1)
                    sts += "Battery OK\n";
                else
                    sts += "Battery fault\n";
                if (MotorTemperature == 1)
                    sts += "Motor temperature OK\n";
                else
                    sts += "Motor temperature fault\n";
                if (MotorVoltage == 1)
                    sts += "Motor voltage OK\n";
                else
                    sts += "Motor voltage fault\n";
                if (SpeedOfBullet == 1)
                    sts += "Speed of bullet OK\n";
                else
                    sts += "Speed of bullet fault\n";
                return sts;
            }
        }

        public Color StatusColor
        {
            get => ((Pressure == 1) && (Battery == 1) && (MotorTemperature == 1) &&
                    (MotorVoltage == 1) && (SpeedOfBullet == 1)) ? Color.Cyan : Color.Red;
        }

        private const uint UERROR = 0xFFFFFFFF;
        private const int ERROR = -1;
        private const int OK = 0;

        public const byte STX = 0xBB;
        public const byte ETX = 0x7E;
        public const byte COUNTDOWN = 0x01;
        public const byte COUNTUP = 0x02;
        public const ushort PULSES100 = 100;
        public const ushort PULSES400 = 400;
        public const byte STATUS = 0x00;
        public const byte START = 0x01;
        public const byte STOP = 0x02;
        public const byte RESERVED = 0x00;

        public static readonly byte[] APTXIDs = new byte[] { 0x01, 0x02, 0x03, 0x04 };

        public uint PulsesPrev = UERROR;

        public event PropertyChangedEventHandler PropertyChanged;

        public Aptx()
        {
            Sop = STX;
            Eop = ETX;
        }

        //public static uint PacketGetId(byte[] buffer)
        //{
        //    uint id = UERROR;
        //    unsafe //we'll now pin unmanaged struct over managed byte array
        //    {
        //        fixed (byte* pbuffer = buffer)
        //        {
        //            PacketStatus* packetStatus = (PacketStatus*)pbuffer;
        //            id = packetStatus->APT_SERIAL_NUMBER;
        //        }
        //    }
        //    return id;
        //}

        public unsafe bool PacketParse(ref byte[] buffer)
        {
            //unsafe //we'll now pin unmanaged struct over managed byte array
            //{
            if (PacketGet(ref buffer, sizeof(PacketStatus)))
            {
                fixed (byte* pbuffer = buffer)
                {
                    PacketStatus* packetStatus = (PacketStatus*)pbuffer;
                    if (ArrayToUshort((byte*)&packetStatus->Check_sum_msb) == ChecksumCalc(buffer.Take(sizeof(PacketStatus) - 2).ToArray()))
                    {
                        Id = packetStatus->APT_SERIAL_NUMBER;
                        SNum = ArrayToUint((byte*)&packetStatus->AM_number_msb);
                        Maxi = ArrayToUint((byte*)&packetStatus->Max_number_msb);
                        Pulses = ArrayToUint((byte*)&packetStatus->Current_number_msb);
                        aptxId[0] = ArrayToUint((byte*)&packetStatus->Apt_number_msb);
                        Pressure = packetStatus->Pressure_flag;
                        Battery = packetStatus->Battery_flag;
                        MotorIsRunning = packetStatus->motor_is_running;
                        AptPulses = packetStatus->Apt_pulses_flag;
                        MotorTemperature = packetStatus->motor_temperature;
                        MotorVoltage = packetStatus->motor_voltage;
                        SpeedOfBullet = packetStatus->speed_of_bullet;
                        CowId = ArrayToUshort((byte*)&packetStatus->Cow_id_msb);
                        buffer = buffer.Skip(sizeof(PacketStatus)).ToArray();
                        return true;
                    }
                }
            }
            //}
            return false;
        }

        public byte[] PacketBuild (byte count, ushort pulses, byte process)
        {
            byte[] bpulses = UshortToArray(pulses);
            return ChecksumAppend(new byte[] { STX, (byte)Id,
                    count, bpulses[0], bpulses[1], process,
                    RESERVED, RESERVED, RESERVED,
                    ETX}).ToArray();
        }

        public ushort ChecksumCalc(byte[] buffer)
        {
            return (ushort)buffer.Sum(b => b);
        }

        private byte[] ChecksumAppend(byte[] buffer)
        {
            //UInt16 checkSum = 0;
            //foreach (byte b in buffer)
            //    checkSum += b;
            //return buffer.Concat(UshortToArray((ushort)buffer.Sum(b => b))).ToArray();
            return buffer.Concat(UshortToArray(ChecksumCalc(buffer))).ToArray();
        }

        public byte[] PacketBuild(byte process, ushort pulses = PULSES100)
        {
            return PacketBuild(COUNTDOWN, pulses, process);
        }

        public byte[] PacketBuild()
        {
            return PacketBuild(RESERVED, RESERVED, STATUS);
        }
    }

    public class DataModel : INotifyPropertyChanged
    {
        private uint cmt = UERROR;
        public uint Cmt
        {
            get => cmt;
            set
            {
                cmt = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Cmt)));
            }
        }

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

        //private const uint APTX_COUNT = 4;

        private readonly uint[] STATEs; // = new uint[APTX_COUNT];
        
        private const long CONNECT_TIMEOUT = 3000;
        private const long STATE_TIMEOUT = 3000; // in msec
        private const long REQUEST_TIMEOUT = 1000; // in msec
        
        private const long RXBUFFER_SIZE = 1024;

        private const string LOGFILE_COWS = "LOGFILE_COWS";

        public Aptx[] Aptxs; // = new Aptx[APTX_COUNT];

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
        private byte[] RxBuffer = new byte[RXBUFFER_SIZE];

        public DataModel(IUsbSerial usbSerial)
        {
            Aptxs = new Aptx[Aptx.APTXIDs.Length].Select((a, i) => { a = new Aptx(); a.Id = Aptx.APTXIDs[i]; return a; }).ToArray();
            STATEs = new uint[Aptx.APTXIDs.Length].Select((s, i) => s = (uint)i).ToArray();

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
                //lbl.SetDynamicResource(Label.BackgroundColorProperty, "BackgroundCyan");
            });

            TappedRL = new Command(() =>
            {
                //FL = lbl.BackgroundColor == Color.Cyan ? Color.AliceBlue : Color.Cyan;
                rl = rl ? false : true;
                RL = RL;
                //lbl.SetDynamicResource(Label.BackgroundColorProperty, "BackgroundCyan");
            });

            TappedFR = new Command(() =>
            {
                //FL = lbl.BackgroundColor == Color.Cyan ? Color.AliceBlue : Color.Cyan;
                fr = fr ? false : true;
                FR = FR;
                //lbl.SetDynamicResource(Label.BackgroundColorProperty, "BackgroundCyan");
            });

            TappedRR = new Command(() =>
            {
                //FL = lbl.BackgroundColor == Color.Cyan ? Color.AliceBlue : Color.Cyan;
                rr = rr ? false : true;
                RR = RR;
                //lbl.SetDynamicResource(Label.BackgroundColorProperty, "BackgroundCyan");
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
            //new Thread((device) => { TxRx(device); })
            //{ Name = "EcomilkTxRx" }.Start(ECOMILK);
            //Task.Run(TxRxECOMILK);
            Task.Run(async () => { await TxRx(REMOTE); });

            //new Thread((device) => { TxRx(device); })
            //{ Name = "RemoteTxRx" }.Start(REMOTE);
            Task.Run(async () => { await TxRx(REMOTE); });

            //new Thread((device) => { TxRx(device); })
            //{ Name = "RfidTxRx" }.Start(RFID);
            Task.Run(async () => { await TxRx(RFID); });

            //new Thread((device) => { TxRx(device); })
            //{ Name = "Aptx1TxRx" }.Start(APTX1);
            Task.Run(async () => { await TxRx(APTX1); });
        }

        //private async Task TxRxECOMILK() { TxRx(ECOMILK); }
        private async Task TxRx(object odevice)
        {
            if (odevice is string)
            {
                string device = odevice as string;
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
            int length;
            if ((length = await UsbSerial.Read(port, buffer)) > 0)
            {
                RxBuffer.Concat(buffer.Take(length));
                switch (device)
                {
                    case ECOMILK:
                        break;
                    case RFID:
                        RfId rfId = new RfId();
                        rfId.PacketParse(ref RxBuffer);
                        break;
                    case REMOTE:
                        //Aptxs.Single(aptx => aptx.Id == Aptx.PacketGetId(buffer)).PacketParse(buffer);
                        Aptx aptx = new Aptx();
                        if (aptx.PacketParse(ref RxBuffer))
                            Aptxs[aptx.Id - 1] = aptx;
                        break;
                    case APTX1:
                        string data = Encoding.UTF8.GetString(RxBuffer);
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
                case RFID:
                    {
                        await UsbSerial.Write(Ports.TryGetValue(RFID, out string val) ? val : string.Empty,
                            new RfId().PacketBuild());
                    }
                    break;
                case REMOTE:
                    {
                        await UsbSerial.Write(Ports.TryGetValue(REMOTE, out string val) ? val : string.Empty,
                            Aptxs[state].PacketBuild());
                    }
                    break;
                case APTX1:
                    {
                        if (Aptxs[0].SNum == UERROR)
                            await UsbSerial.Write(Ports.TryGetValue(device, out var val) ? val : string.Empty, Encoding.UTF8.GetBytes("testread,3#"));
                        else if (Aptxs[0].Current == UERROR)
                            await UsbSerial.Write(Ports.TryGetValue(device, out var val) ? val : string.Empty, Encoding.UTF8.GetBytes("find,3#"));
                        else if ((Aptxs[0].aptxId[0] == UERROR) ||
                                  (Aptxs[0].aptxId[1] == UERROR) ||
                                  (Aptxs[0].aptxId[2] == UERROR))
                            await UsbSerial.Write(Ports.TryGetValue(device, out var val) ? val : string.Empty, Encoding.UTF8.GetBytes("readid#"));
                    }
                    break;
            }
        }

        public async Task PortConnectRequest(string device, string port)
        {
            switch (device)
            {
                case ECOMILK:
                case APTX1:
                    await UsbSerial.Write(port, Encoding.UTF8.GetBytes("getid,3#"));
                    break;
                case RFID:
                    await UsbSerial.Write(port, new RfId().PacketBuild());
                    break;
                case REMOTE:
                    await UsbSerial.Write(port, Aptxs[0].PacketBuild());
                    break;
            }
        }

        public async Task<bool> PortConnectReply(string device, string port)
        {
            byte[] buffer = new byte[1024];
            //data += await UsbSerial.Read(device, buffer);
            int length = await UsbSerial.Read(port, buffer);
            RxBuffer.Concat(buffer.Take(length));

            bool found = false;
            if (!Ports.ContainsKey(device))
            {
                switch (device)
                {
                    case ECOMILK:
                        {
                            string data = Encoding.UTF8.GetString(RxBuffer);
                            found = data.Contains(device);
                            break;
                        }
                    case RFID:
                        RxBuffer = RxBuffer.Select((b, i) => b = (byte)i).ToArray();
                        RxBuffer[10] = RfId.PREEMBLE;
                        RxBuffer[10 + 19] = RfId.END_MARK;
                        RfId rfId = new RfId();
                        RxBuffer[10 + 20] = rfId.UshortToArray(rfId.CrcCalc(RxBuffer.Skip(11).Take(19).ToArray()))[0];
                        RxBuffer[10 + 21] = rfId.UshortToArray(rfId.CrcCalc(RxBuffer.Skip(11).Take(19).ToArray()))[1];
                        if (found = rfId.PacketParse(ref RxBuffer))
                        {
                            unsafe
                            {
                                fixed (byte* pepc = rfId.EPC)
                                {
                                    CowId = rfId.ArrayToUint(pepc);
                                }
                            }
                        }
                        break;
                    case REMOTE:
                        RxBuffer = RxBuffer.Select((b, i) => b = (byte)i).ToArray();
                        RxBuffer[10] = Aptx.STX;
                        RxBuffer[11] = Aptx.APTXIDs[0];
                        RxBuffer[10 + 32] = Aptx.ETX;
                        Aptx aptx = new Aptx();
                        RxBuffer[10 + 33] = aptx.UshortToArray(aptx.ChecksumCalc(RxBuffer.Skip(10).Take(33).ToArray()))[0];
                        RxBuffer[10 + 34] = aptx.UshortToArray(aptx.ChecksumCalc(RxBuffer.Skip(10).Take(33).ToArray()))[1];
                        if (found = aptx.PacketParse(ref RxBuffer))
                        {
                            if (Aptx.APTXIDs.Contains((byte)aptx.Id))
                                Aptxs[aptx.Id - 1] = aptx;
                        }
                        break;
                    case APTX1:
                        {
                            string data = Encoding.UTF8.GetString(RxBuffer);
                            found = data.Contains("1F-85-01");
                        }
                        break;
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

        public async Task<int> ProcessStart()
        {
            foreach (Aptx aptx in Aptxs)
            {
                if (await Process(aptx, Aptx.START) == ERROR)
                    return ERROR;
            }
            return OK;
        }

        public async Task<int> ProcessStop()
        {
            foreach (Aptx aptx in Aptxs)
            {
                if (await Process(aptx, Aptx.STOP) == ERROR)
                    return ERROR;
            }
            return OK;
        }

        public async Task<int> ProcessPauseResume()
        {
            PauseResume = PauseResume == Aptx.STOP ? PauseResume = Aptx.START : PauseResume = Aptx.STOP;
            foreach (Aptx aptx in Aptxs)
            {
                if (await Process(aptx, PauseResume, (ushort)(Aptx.PULSES100 - (aptx.Pulses - aptx.PulsesPrev))) == ERROR)
                    return ERROR;
            }
            return OK;
        }

        public async Task<int> Process(Aptx aptx, byte process, ushort pulses = Aptx.PULSES100)
        {
            int response = await UsbSerial.Write(Ports.TryGetValue(REMOTE, out string val) ? val : string.Empty,
                aptx.PacketBuild(process, pulses));

            string LOGFILE_COWS = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "LogFileCows.txt");
            File.AppendAllText(LOGFILE_COWS, string.Format("Date: {0} {1} Process: {2} Pulses: {3} Cow Id: {4} Current Pulses: {5}\n",
                DateTime.Now, response == OK ? "OK" : "FAULT", process == Aptx.START ? "START" : "STOP",
                pulses, CowId, aptx.Pulses));
            return response;
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
