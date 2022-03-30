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
                    buffer = buffer.Skip(i).ToArray();
                    if (buffer.Length >= size)
                    {
                        if (buffer[size - 1 - 2] == Eop)
                            return true;
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
        private const byte MSG_TYPE_RESPONSE = 0x01;
        private const byte CODE_READ_TYPE_C_UII = 0x22;
        private const byte CODE_ERROR = 0xFF;

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
                    if (readTypeCUIIResponse->MsgType == MSG_TYPE_RESPONSE)
                    {
                        if (readTypeCUIIResponse->Code == CODE_READ_TYPE_C_UII)
                        {
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
                }
            }
            //}
            buffer = buffer.Skip(1).ToArray();
            return false;
        }
        
        public byte[] PacketBuild()
        {
            return new byte[] { PREEMBLE}.Concat(
                CrcAppend(new byte[] { MSG_TYPE_READ_TYPE_C_UII, CODE_READ_TYPE_C_UII, 0, 0, END_MARK})).ToArray();
        }

        public ushort CrcCalc(byte[] buffer)
        {
            ushort crc = 0xFFFF;
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
            public byte Sum_pulses_msb;
            public byte Sum_pulses_2;
            public byte Sum_pulses_3;
            public byte Sum_pulses_lsb;
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

        //private uint current1 = UERROR;
        //public uint Current1 { get => current1; set => current1 = value; }

        private uint maxi = UERROR;
        public uint Maxi 
        { 
            get => maxi;
            set
            {
                maxi = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Maxi)));
            }
        }

        private uint remaining = UERROR;
        public uint Remaining
        {
            get => remaining;
            set
            {
                remaining = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(Remaining)));
            }
        }

        private uint processPulses = UERROR;
        public uint ProcessPulses
        {
            get => processPulses;
            set
            {
                processPulses = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ProcessPulses)));
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

        private uint currentPulses = UERROR;
        public uint CurrentPulses
        {
            get => currentPulses;
            set
            {
                currentPulses = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentPulses)));
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
                        ProcessPulses = ArrayToUint((byte*)&packetStatus->Current_number_msb);
                        aptxId[0] = ArrayToUint((byte*)&packetStatus->Apt_number_msb);
                        Pressure = packetStatus->Pressure_flag;
                        Battery = packetStatus->Battery_flag;
                        MotorIsRunning = packetStatus->motor_is_running;
                        AptPulses = packetStatus->Apt_pulses_flag;
                        MotorTemperature = packetStatus->motor_temperature;
                        MotorVoltage = packetStatus->motor_voltage;
                        SpeedOfBullet = packetStatus->speed_of_bullet;
                        CowId = ArrayToUshort((byte*)&packetStatus->Cow_id_msb);
                        CurrentPulses = ArrayToUint((byte*)&packetStatus->Sum_pulses_msb);
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
        public class Reply
        {
            public bool Found = false;
            public byte[] RxBuffer;
        }

        public bool PressureOK { get => Aptxs[0].Pressure == 1; }
        public bool PressureLow { get => Aptxs[0].Pressure != 1; }
        public bool SpeedOfBulletOK { get => Aptxs[0].SpeedOfBullet == 1; }
        public bool SpeedOfBulletLow { get => Aptxs[0].SpeedOfBullet != 1; }
        public bool BatteryOK { get => Aptxs[0].Battery == 1; }
        public bool BatteryLow { get => Aptxs[0].Battery != 1; }
        public bool PulsesYes { get => Aptxs[0].ProcessPulses == 1; }
        public bool PulsesNo { get => Aptxs[0].ProcessPulses != 1; }

        private string cmtFL;
        public string CmtFL
        {
            get => cmtFL;
            set
            {
                //if (value != null)
                //{
                cmtFL = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CmtFL)));
                //}
            }
        }

        private string cmtRL;
        public string CmtRL
        {
            get => cmtRL;
            set
            {
                //if (value != null)
                //{
                cmtRL = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CmtRL)));
                //}
            }
        }

        private string cmtFR;
        public string CmtFR
        {
            get => cmtFR;
            set
            {
                //if (value != null)
                //{
                cmtFR = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CmtFR)));
                //}
            }
        }

        private string cmtRR;
        public string CmtRR
        {
            get => cmtRR;
            set
            {
                //if (value != null)
                //{
                cmtRR = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CmtRR)));
                //}
            }
        }

        public Color CmtFLColor
        {
            get => cmtFL == "1" || cmtFL == "2" || cmtFL == "3" ? Color.Green : Color.Red;
        }

        public Color CmtRLColor
        {
            get => cmtRL == "1" || cmtRL == "2" || cmtRL == "3" ? Color.Green : Color.Red;
        }

        public Color CmtFRColor
        {
            get => cmtFR == "1" || cmtFR == "2" || cmtFR == "3" ? Color.Green : Color.Red;
        }

        public Color CmtRRColor
        {
            get => cmtRR == "1" || cmtRR == "2" || cmtRR == "3" ? Color.Green : Color.Red;
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
            get => (Aptxs[0].ProcessPulses - Aptxs[0].PulsesPrev) / Aptxs[0].PulsesPrev;
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
        private const int QUARTERS_NUMBER = 4;
        private const int CONNECT_TIMEOUT = 3000;
        private const int STATE_TIMEOUT = 3000; // in msec
        private const int REQUEST_TIMEOUT = 1000; // in msec

        private const int RXBUFFER_SIZE = 1024;

        private const string LOGFILE_COWS = "LOGFILE_COWS";

        public string[] Devices;

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
        private Semaphore SemaphorePorts = new Semaphore(1, 1);
        private bool Connected = false;
        private byte PauseResume = Aptx.STOP;
        private byte[] RxBufferRfid = new byte[1];// RXBUFFER_SIZE];
        private byte[] RxBufferAptx1 = new byte[1];// RXBUFFER_SIZE];
        private byte[] RxBufferEcomilk = new byte[1];// RXBUFFER_SIZE];
        private byte[] RxBufferRemote = new byte[1];// RXBUFFER_SIZE];

        public DataModel(IUsbSerial usbSerial)
        {

            //new Thread(() =>
            //{
            //    while (true)
            //    {
            //        Thread.Sleep(1000);

            //        UInt32 ftdiDeviceCount = 0;
            //        FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OK;

            //        // Create new instance of the FTDI device class
            //        FTDI myFtdiDevice = new FTDI();

            //        // Determine the number of FTDI devices connected to the machine
            //        ftStatus = myFtdiDevice.GetNumberOfDevices(ref ftdiDeviceCount);
            //        // Check status
            //        if (ftStatus == FTDI.FT_STATUS.FT_OK)
            //        {
            //            Console.WriteLine("Number of FTDI devices: " + ftdiDeviceCount.ToString());
            //            Console.WriteLine("");
            //        }
            //        else
            //        {
            //            // Wait for a key press
            //            Console.WriteLine("Failed to get number of devices (error " + ftStatus.ToString() + ")");
            //            //Console.ReadKey();
            //            //return;
            //        }

            //        // If no devices available, return
            //        if (ftdiDeviceCount == 0)
            //        {
            //            // Wait for a key press
            //            Console.WriteLine("Failed to get number of devices (error " + ftStatus.ToString() + ")");
            //            //Console.ReadKey();
            //            //return;
            //        }

            //        // Allocate storage for device info list
            //        FTDI.FT_DEVICE_INFO_NODE[] ftdiDeviceList = new FTDI.FT_DEVICE_INFO_NODE[ftdiDeviceCount];

            //        // Populate our device list
            //        ftStatus = myFtdiDevice.GetDeviceList(ftdiDeviceList);

            //        if (ftStatus == FTDI.FT_STATUS.FT_OK)
            //        {
            //            for (UInt32 i = 0; i < ftdiDeviceCount; i++)
            //            {
            //                Console.WriteLine("Device Index: " + i.ToString());
            //                Console.WriteLine("Flags: " + String.Format("{0:x}", ftdiDeviceList[i].Flags));
            //                Console.WriteLine("Type: " + ftdiDeviceList[i].Type.ToString());
            //                Console.WriteLine("ID: " + String.Format("{0:x}", ftdiDeviceList[i].ID));
            //                Console.WriteLine("Location ID: " + String.Format("{0:x}", ftdiDeviceList[i].LocId));
            //                Console.WriteLine("Serial Number: " + ftdiDeviceList[i].SerialNumber.ToString());
            //                Console.WriteLine("Description: " + ftdiDeviceList[i].Description.ToString());
            //                Console.WriteLine("");
            //            }
            //        }


            //        // Open first device in our list by serial number
            //        ftStatus = myFtdiDevice.OpenBySerialNumber(ftdiDeviceList[0].SerialNumber);
            //        if (ftStatus != FTDI.FT_STATUS.FT_OK)
            //        {
            //            // Wait for a key press
            //            Console.WriteLine("Failed to open device (error " + ftStatus.ToString() + ")");
            //            //Console.ReadKey();
            //            //return;
            //        }

            //        // Set up device data parameters
            //        // Set Baud rate to 9600
            //        ftStatus = myFtdiDevice.SetBaudRate(115200);
            //        if (ftStatus != FTDI.FT_STATUS.FT_OK)
            //        {
            //            // Wait for a key press
            //            Console.WriteLine("Failed to set Baud rate (error " + ftStatus.ToString() + ")");
            //            //Console.ReadKey();
            //            //return;
            //        }

            //        // Set data characteristics - Data bits, Stop bits, Parity
            //        ftStatus = myFtdiDevice.SetDataCharacteristics(FTDI.FT_DATA_BITS.FT_BITS_8, FTDI.FT_STOP_BITS.FT_STOP_BITS_1, FTDI.FT_PARITY.FT_PARITY_NONE);
            //        if (ftStatus != FTDI.FT_STATUS.FT_OK)
            //        {
            //            // Wait for a key press
            //            Console.WriteLine("Failed to set data characteristics (error " + ftStatus.ToString() + ")");
            //            //Console.ReadKey();
            //            //return;
            //        }

            //        // Set flow control - set RTS/CTS flow control
            //        //ftStatus = myFtdiDevice.SetFlowControl(FTDI.FT_FLOW_CONTROL.FT_FLOW_RTS_CTS, 0x11, 0x13);
            //        ftStatus = myFtdiDevice.SetFlowControl(FTDI.FT_FLOW_CONTROL.FT_FLOW_NONE, 0, 0);
            //        if (ftStatus != FTDI.FT_STATUS.FT_OK)
            //        {
            //            // Wait for a key press
            //            Console.WriteLine("Failed to set flow control (error " + ftStatus.ToString() + ")");
            //            //Console.ReadKey();
            //            //return;
            //        }

            //        // Set read timeout to 5 seconds, write timeout to infinite
            //        ftStatus = myFtdiDevice.SetTimeouts(5000, 0);
            //        if (ftStatus != FTDI.FT_STATUS.FT_OK)
            //        {
            //            // Wait for a key press
            //            Console.WriteLine("Failed to set timeouts (error " + ftStatus.ToString() + ")");
            //            //Console.ReadKey();
            //            //return;
            //        }

            //        // Perform loop back - make sure loop back connector is fitted to the device
            //        // Write string data to the device
            //        string dataToWrite = "getid,3#";
            //        UInt32 numBytesWritten = 0;
            //        // Note that the Write method is overloaded, so can write string or byte array data
            //        ftStatus = myFtdiDevice.Write(dataToWrite, dataToWrite.Length, ref numBytesWritten);
            //        if (ftStatus != FTDI.FT_STATUS.FT_OK)
            //        {
            //            // Wait for a key press
            //            Console.WriteLine("Failed to write to device (error " + ftStatus.ToString() + ")");
            //            //Console.ReadKey();
            //            //return;
            //        }


            //        // Check the amount of data available to read
            //        // In this case we know how much data we are expecting, 
            //        // so wait until we have all of the bytes we have sent.
            //        UInt32 numBytesAvailable = 0;
            //        //do
            //        //{
            //        //    ftStatus = myFtdiDevice.GetRxBytesAvailable(ref numBytesAvailable);
            //        //    if (ftStatus != FTDI.FT_STATUS.FT_OK)
            //        //    {
            //        //        // Wait for a key press
            //        //        Console.WriteLine("Failed to get number of bytes available to read (error " + ftStatus.ToString() + ")");
            //        //        //Console.ReadKey();
            //        //        //return;
            //        //    }
            //        //    Thread.Sleep(10);
            //        //} while (numBytesAvailable < dataToWrite.Length);

            //        // Now that we have the amount of data we want available, read it
            //        string readData;
            //        UInt32 numBytesRead = 0;
            //        // Note that the Read method is overloaded, so can read string or byte array data
            //        ftStatus = myFtdiDevice.Read(out readData, numBytesAvailable, ref numBytesRead);
            //        if (ftStatus != FTDI.FT_STATUS.FT_OK)
            //        {
            //            // Wait for a key press
            //            Console.WriteLine("Failed to read data (error " + ftStatus.ToString() + ")");
            //            //Console.ReadKey();
            //            //return;
            //        }
            //        Console.WriteLine(readData);

            //        // Close our device
            //        ftStatus = myFtdiDevice.Close();

            //        // Wait for a key press
            //        Console.WriteLine("Press any key to continue.");
            //        //Console.ReadKey();
            //        //return;
            //    }
            ////}).Start();


            Devices = new string[] { ECOMILK, REMOTE, RFID, APTX1 };
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

            new Thread((device) => { Rx(device); })
            { Name = "RFID" }.Start(RFID);

            new Thread((device) => { Rx(device); })
            { Name = "APTX1" }.Start(APTX1);

            new Thread(() => { Tx(); })
            { Name = "Tx" }.Start();
        }

        private async Task Rx(object odevice)
        {
            Dictionary<string, byte[]> rxBuffers = new Dictionary<string, byte[]>();
            if (odevice is string)
            {
                string device = odevice as string;
                string data = string.Empty;
                while (Connected)
                {
                    try
                    {
                        if (Ports.TryGetValue(device, out string port))
                        {
                            await PortReply(device, port, rxBuffers);
                        }
                        else
                        {
                            string[] ports = UsbSerial.GetPorts().ToArray();
                            foreach (string prt in ports)
                            {
                                if (await PortReply(device, prt, rxBuffers))
                                {
                                    SemaphorePorts.WaitOne();
                                    Ports.Add(device, prt);
                                    SemaphorePorts.Release();
                                }
                            }
                        }
                    }
                    catch
                    {
                    }
                }
            }
        }

        private async Task Tx()
        {
            string data = string.Empty;
            string device = string.Empty;
            while (Connected)
            {
                try
                {
                    Stopwatch stopWatch = new Stopwatch();
                    stopWatch.Start();
                    uint state = STATEs.First();
                    foreach (string dev in Devices)
                    {
                        device = dev;
                        Thread.Sleep(REQUEST_TIMEOUT);
                        if (Ports.TryGetValue(device, out string port))
                        {
                            if (stopWatch.ElapsedMilliseconds % STATE_TIMEOUT == 0)
                                state = state == STATEs.Last() ? STATEs.First() : state++;
                            if (await PortRequest(device, port, state) < 0)
                            {
                                //SemaphorePorts.WaitOne();
                                //Ports.Remove(device);
                                //SemaphorePorts.Release();
                                //UsbSerial.Disconnect();
                            }
                        }
                        else
                        {
                            //UsbSerial.Connect();
                            string[] ports = UsbSerial.GetPorts().ToArray();
                            foreach (string prt in ports)
                            {
                                await PortRequest(device, prt, 0);
                            }
                        }
                    }
                }
                catch
                {
                    if (Ports.Keys.Contains(device))
                    {
                        SemaphorePorts.WaitOne();
                        Ports.Remove(device);
                        SemaphorePorts.Release();
                    }
                    //UsbSerial.Disconnect();
                    //UsbSerial.Connect();
                }
            }
        }

        private async Task<bool> PortReply(string device, string port, Dictionary<string, byte[]> rxBuffers)
        {
            if (!rxBuffers.TryGetValue(port, out byte[] rxBuffer))
                rxBuffers.Add(port, (rxBuffer = new byte[1]));
            byte[] buffer = new byte[1024];
            //data += await UsbSerial.Read(port, buffer);
            int length = 0;
            bool found = false;
            if ((length = await UsbSerial.Read(port, buffer)) > 0)
            {
                rxBuffer = rxBuffer.Concat(buffer.Take(length)).ToArray();
                switch (device)
                {
                    case ECOMILK:
                        if (!Ports.ContainsKey(device))
                        {
                            string data = Encoding.UTF8.GetString(rxBuffer);
                            found = data.Contains(device);
                        }
                        break;
                    case RFID:
                        //RxBuffer = RxBuffer.Select((b, i) => b = (byte)i).ToArray();
                        //RxBuffer[10] = RfId.PREEMBLE;
                        //RxBuffer[10 + 19] = RfId.END_MARK;
                        RfId rfId = new RfId();
                        //RxBuffer[10 + 20] = rfId.UshortToArray(rfId.CrcCalc(RxBuffer.Skip(11).Take(19).ToArray()))[0];
                        //RxBuffer[10 + 21] = rfId.UshortToArray(rfId.CrcCalc(RxBuffer.Skip(11).Take(19).ToArray()))[1];
                        if (found = rfId.PacketParse(ref rxBuffer))
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
                        //RxBuffer = RxBuffer.Select((b, i) => b = (byte)i).ToArray();
                        //RxBuffer[10] = Aptx.STX;
                        //RxBuffer[11] = Aptx.APTXIDs[0];
                        //RxBuffer[10 + 32] = Aptx.ETX;
                        Aptx aptx = new Aptx();
                        //RxBuffer[10 + 33] = aptx.UshortToArray(aptx.ChecksumCalc(RxBuffer.Skip(10).Take(33).ToArray()))[0];
                        //RxBuffer[10 + 34] = aptx.UshortToArray(aptx.ChecksumCalc(RxBuffer.Skip(10).Take(33).ToArray()))[1];
                        if (found = aptx.PacketParse(ref rxBuffer))
                        {
                            if (Aptx.APTXIDs.Contains((byte)aptx.Id))
                                Aptxs[aptx.Id - 1] = aptx;
                        }
                        break;
                    case APTX1:
                        {
                            string data = Encoding.UTF8.GetString(rxBuffer.Where(b => b != 0x00).ToArray());
                            if (!Ports.ContainsKey(device))
                            {
                                found = data.Contains("1F-85-01");
                                if (!found)
                                    found = data.Contains("0x1f-0x85-0x01");
                                if (found)
                                    rxBuffer = new byte[1];
                            }
                            else
                            {
                                if (Aptxs[0].SNum == UERROR)
                                {
                                    if (data.Contains("SNUM"))
                                    {
                                        uint[] snum = DataParse(data, "SNUM", NumberStyles.Number);
                                        Aptxs[0].SNum = snum[0];
                                        rxBuffer = new byte[1];
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
                                        rxBuffer = new byte[1];
                                    }
                                }
                                if ((Aptxs[0].CurrentPulses == UERROR) || (Aptxs[0].Maxi == UERROR))
                                {
                                    if (data.Contains("MAXI"))
                                    {
                                        uint[] maxi = DataParse(data, "MAXI", NumberStyles.Number);
                                        Aptxs[0].Maxi = maxi[0];
                                        rxBuffer = new byte[1];
                                    }
                                    if (data.Contains("Found:") || data.Contains("pulses written"))
                                    {
                                        uint[] current = DataParse(data, "Found:", NumberStyles.Number);
                                        if (current[0] == 0)
                                            current = DataParse(data, "pulses written", NumberStyles.Number);
                                        Aptxs[0].CurrentPulses = current[0];
                                        rxBuffer = new byte[1];
                                    }
                                    if ((Aptxs[0].Maxi != UERROR) && (Aptxs[0].CurrentPulses != UERROR))
                                        Aptxs[0].Remaining = Aptxs[0].Maxi - Aptxs[0].CurrentPulses;
                                }
                            }
                        }
                        break;
                }
            }
            rxBuffers.Remove(port);
            rxBuffers.Add(port, rxBuffer);
            return found;
        }

        private async Task<int> PortRequest(string device, string port, uint state)
        {
            int ret = ERROR;
            switch (device)
            {
                case ECOMILK:
                    ret = await UsbSerial.Write(port, Encoding.UTF8.GetBytes("id#"));
                    break;
                case RFID:
                    ret = await UsbSerial.Write(port, new RfId().PacketBuild());
                    break;
                case REMOTE:
                    ret = await UsbSerial.Write(port, Aptxs[state].PacketBuild());
                    break;
                case APTX1:
                    if (!Ports.ContainsKey(device))
                    {
                        ret = await UsbSerial.Write(port, Encoding.UTF8.GetBytes("getid,3#"));
                    }
                    else
                    {
                        if (Aptxs[0].SNum == UERROR)
                            ret = await UsbSerial.Write(port, Encoding.UTF8.GetBytes("testread,3#"));
                        else if (Aptxs[0].CurrentPulses == UERROR)
                            ret = await UsbSerial.Write(port, Encoding.UTF8.GetBytes("find,3#"));
                        else if ((Aptxs[0].aptxId[0] == UERROR) ||
                                  (Aptxs[0].aptxId[1] == UERROR) ||
                                  (Aptxs[0].aptxId[2] == UERROR))
                            ret = await UsbSerial.Write(port, Encoding.UTF8.GetBytes("readid#"));
                    }
                    break;
            }
            return ret;
        }

        //public async Task PortConnectRequest(string device, string port)
        //{
        //    switch (device)
        //    {
        //        case ECOMILK:
        //        case APTX1:
        //            await UsbSerial.Write(port, Encoding.UTF8.GetBytes("getid,3#"));
        //            break;
        //        case RFID:
        //            await UsbSerial.Write(port, new RfId().PacketBuild());
        //            break;
        //        case REMOTE:
        //            await UsbSerial.Write(port, Aptxs[0].PacketBuild());
        //            break;
        //    }
        //}

        //public async Task<Reply> PortConnectReply(string device, string port, byte[] rxBuffer)
        //{
        //    byte[] buffer = new byte[1024];
        //    //data += await UsbSerial.Read(device, buffer);
        //    int length = await UsbSerial.Read(port, buffer);
        //    bool found = false;
        //    if (length > 0)
        //    {
        //        //RxBufferRfid = RxBufferRfid.Concat(buffer.Take(length)).ToArray();
        //        rxBuffer = rxBuffer.Concat(buffer.Take(length)).ToArray();

        //        if (!Ports.ContainsKey(device))
        //        {
        //            switch (device)
        //            {
        //                case ECOMILK:
        //                    {
        //                        string data = Encoding.UTF8.GetString(rxBuffer);
        //                        found = data.Contains(device);
        //                        break;
        //                    }
        //                case RFID:
        //                    //RxBuffer = RxBuffer.Select((b, i) => b = (byte)i).ToArray();
        //                    //RxBuffer[10] = RfId.PREEMBLE;
        //                    //RxBuffer[10 + 19] = RfId.END_MARK;
        //                    RfId rfId = new RfId();
        //                    //RxBuffer[10 + 20] = rfId.UshortToArray(rfId.CrcCalc(RxBuffer.Skip(11).Take(19).ToArray()))[0];
        //                    //RxBuffer[10 + 21] = rfId.UshortToArray(rfId.CrcCalc(RxBuffer.Skip(11).Take(19).ToArray()))[1];
        //                    if (found = rfId.PacketParse(ref rxBuffer))
        //                    {
        //                        unsafe
        //                        {
        //                            fixed (byte* pepc = rfId.EPC)
        //                            {
        //                                CowId = rfId.ArrayToUint(pepc);
        //                            }
        //                        }
        //                    }
        //                    break;
        //                case REMOTE:
        //                    //RxBuffer = RxBuffer.Select((b, i) => b = (byte)i).ToArray();
        //                    //RxBuffer[10] = Aptx.STX;
        //                    //RxBuffer[11] = Aptx.APTXIDs[0];
        //                    //RxBuffer[10 + 32] = Aptx.ETX;
        //                    Aptx aptx = new Aptx();
        //                    //RxBuffer[10 + 33] = aptx.UshortToArray(aptx.ChecksumCalc(RxBuffer.Skip(10).Take(33).ToArray()))[0];
        //                    //RxBuffer[10 + 34] = aptx.UshortToArray(aptx.ChecksumCalc(RxBuffer.Skip(10).Take(33).ToArray()))[1];
        //                    if (found = aptx.PacketParse(ref rxBuffer))
        //                    {
        //                        if (Aptx.APTXIDs.Contains((byte)aptx.Id))
        //                            Aptxs[aptx.Id - 1] = aptx;
        //                    }
        //                    break;
        //                case APTX1:
        //                    {
        //                        string data = Encoding.UTF8.GetString(rxBuffer);
        //                        //found = data.Contains("1F-85-01");
        //                        found = data.Contains("0x1f-0x85-0x01");
        //                    }
        //                    break;
        //                default:
        //                    return new Reply() { Found = found, RxBuffer = rxBuffer };
        //            }
        //        }
        //    }
        //    return new Reply() { Found = found, RxBuffer = rxBuffer };
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
                if (await Process(aptx, PauseResume, (ushort)(Aptx.PULSES100 - (aptx.ProcessPulses - aptx.PulsesPrev))) == ERROR)
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
                pulses, CowId, aptx.ProcessPulses));
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
