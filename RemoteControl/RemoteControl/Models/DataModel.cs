using RemoteControl.Views;
using System;
using System.Collections.Concurrent;
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
using Windows.Media.Capture;
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
        public delegate ushort DCheck(byte[] buffer);
        public delegate bool DAssign(byte[] buffer);
        public DCheck Dcheck;
        public DAssign Dassign;

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

        //public bool PacketGet(byte[] buffer, ref byte[] res, int size)
        public bool PacketGet(byte[] buffer, ref byte[] res, ref bool sopeop)
        {
            //res = buffer.SkipWhile(b => b != Sop).ToArray();
            //while (res.Length >= size)
            //{
            //    //buffer = buffer.SkipWhile(b => b != Sop).ToArray();
            //    res = res.SkipWhile(b => b != Sop).ToArray();
            //    if (res.Length >= size)
            //    {
            //        if (res[size - 1 - 2] == Eop)
            //            return true;
            //        res = res.Skip(1).ToArray();
            //    }
            //}
            //return false;

            //res = buffer.SkipWhile(b => b != Sop).TakeWhile(b => b != Eop).Take(3).ToArray();
            int idx = -1;
            int count = 0;
            bool found = false;
            ushort check = 0;
            res = buffer.SkipWhile(b => b != Sop).Where((b, i) =>
            {
                if (b == Eop)
                {
                    idx = i;
                    found = true;
                }
                if (idx > 0)
                    count++;
                if (count == 2)
                    check = (ushort)(b << 8);
                if (count == 3)
                    check += b;
                if (count > 3)
                    return false;
                return true;
            }).ToArray();
            sopeop = found;
            if (sopeop)
            {
                if (check == Dcheck(res))
                    return true;
            }
            return false;
        }

        public uint PacketParse(ref byte[] buffer, ref bool found)
        {
            byte[] res = new byte[0];
            byte[] buf = buffer;
            uint count = 0;
            bool sopeop = true;

            found = false;

            while (sopeop)
            {
                if (PacketGet(buf, ref res, ref sopeop))
                {
                    if (Dassign(res))
                        found = true;
                    count++;
                }
                buf = buf.Skip(res.Length).ToArray();
            }

            if (count > 0)
                buffer = buf;
            
            return count;
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

        private byte[] epc = new byte[12].Select(e => e = BERROR).ToArray();
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

            //epc = new byte[12].Select(e => e = BERROR).ToArray();

            Dcheck = CrcCalc;
            Dassign = PacketAssign;
        }

        public unsafe bool PacketAssign(byte[] buffer)
        {
            //unsafe //we'll now pin unmanaged struct over managed byte array
            //{
            if (buffer.Length == sizeof(ReadTypeCUIIResponse))
            {
                //if (PacketGet(buffer, ref res, sizeof(ReadTypeCUIIResponse)))
                fixed (byte* pbuffer = buffer)
                {
                    ReadTypeCUIIResponse* readTypeCUIIResponse = (ReadTypeCUIIResponse*)pbuffer;
                    if (readTypeCUIIResponse->MsgType == MSG_TYPE_RESPONSE)
                    {
                        if (readTypeCUIIResponse->Code == CODE_READ_TYPE_C_UII)
                        {
                            //if (ArrayToUshort((byte*)&readTypeCUIIResponse->CRC_16MSB) == CrcCalc(res.Skip(1).Take(sizeof(ReadTypeCUIIResponse) - 3).ToArray()))
                            //{
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
                            //buffer = res.Skip(sizeof(ReadTypeCUIIResponse)).ToArray();
                            return true;
                            //}
                        }
                    }
                }
            }
            //buffer = res.Skip(sizeof(ReadTypeCUIIResponse)).ToArray();
            //}
            return false;
        }

        public byte[] PacketBuild()
        {
            return new byte[] { PREEMBLE }.Concat(
                CrcAppend(new byte[] { MSG_TYPE_READ_TYPE_C_UII, CODE_READ_TYPE_C_UII, 0, 0, END_MARK })).ToArray();
        }

        public ushort CrcCalc(byte[] buffer)
        {
            ushort crc = 0xFFFF;
            buffer = buffer.Skip(1).Take(buffer.Length - 3).ToArray();
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
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Remaining)));
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
            //get => aptxid.Aggregate("", (r, m) => r += m.ToString("X") + "   ");
            get => aptxid[0].ToString();
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

        private bool pressureOK;
        public bool PressureOK
        {
            get => Pressure == 1;
            set
            {
                pressureOK = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PressureOK)));
            }
        }

        private bool pressureLow;
        public bool PressureLow
        {
            get => Pressure != 1;
            set
            {
                pressureLow = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PressureLow)));
            }
        }

        private bool remainingOK;
        public bool RemainingOK
        {
            get => Remaining > Maxi * 0.1;
            set
            {
                remainingOK = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RemainingOK)));
            }
        }

        private bool remainingLow;
        public bool RemainingLow
        {
            get => Remaining <= Maxi * 0.1;
            set
            {
                remainingLow = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RemainingLow)));
            }
        }

        private bool batteryOK;
        public bool BatteryOK
        {
            get => Battery == 1;
            set
            {
                batteryOK = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BatteryOK)));
            }
        }

        private bool batteryLow;
        public bool BatteryLow
        {
            get => Battery != 1;
            set
            {
                batteryLow = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BatteryLow)));
            }
        }

        private bool aptPulsesOK;
        public bool AptPulsesOK
        {
            get => AptPulses == 1;
            set
            {
                aptPulsesOK = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AptPulsesOK)));
            }
        }

        private bool aptPulsesLow;
        public bool AptPulsesLow
        {
            get => AptPulses != 1;
            set
            {
                aptPulsesLow = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AptPulsesLow)));
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

        public float Progress
        {
            get => (ProcessPulses - PulsesPrev) / ECOMILK_PROCESS_PULSES;
            set
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Progress)));
            }
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

        private const int ECOMILK_PROCESS_PULSES = 100;

        public static readonly byte[] APTXIDs = new byte[] { 0x00, 0x01, 0x02, 0x03 };

        public uint PulsesPrev = UERROR;

        public event PropertyChangedEventHandler PropertyChanged;

        public Aptx()
        {
            Sop = STX;
            Eop = ETX;

            Dcheck = ChecksumCalc;
            Dassign = PacketAssign;
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

        public unsafe bool PacketAssign(byte[] buffer)
        {
            unsafe //we'll now pin unmanaged struct over managed byte array
            {
                if (buffer.Length == sizeof(PacketStatus))
                {
                    //if (PacketGet(buffer, ref res, sizeof(PacketStatus)))
                    fixed (byte* pbuffer = buffer)
                    {
                        PacketStatus* packetStatus = (PacketStatus*)pbuffer;
                        //if (ArrayToUshort((byte*)&packetStatus->Check_sum_msb) == ChecksumCalc(res.Take(sizeof(PacketStatus) - 2).ToArray()))
                        //{
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
                        //buffer = res.Skip(sizeof(PacketStatus)).ToArray();
                        return true;
                        //}
                    }
                }
                //buffer = res.Skip(sizeof(PacketStatus)).ToArray();
            }
            return false;
        }

        public byte[] PacketBuild(byte count, ushort pulses, byte process)
        {
            byte[] bpulses = UshortToArray(pulses);
            return ChecksumAppend(new byte[] { STX, (byte)Id,
                    count, bpulses[0], bpulses[1], process,
                    RESERVED, RESERVED, RESERVED,
                    ETX}).ToArray();
        }

        public ushort ChecksumCalc(byte[] buffer)
        {
            buffer = buffer.Take(buffer.Length - 2).ToArray();
            return (ushort)buffer.Sum(b => b);
        }

        private byte[] ChecksumAppend(byte[] buffer)
        {
            //UInt16 checkSum = 0;
            //foreach (byte b in buffer)
            //    checkSum += b;
            //return buffer.Concat(UshortToArray((ushort)buffer.Sum(b => b))).ToArray();
            return buffer.Concat(UshortToArray(ChecksumCalc(buffer.Concat(new byte[2]).ToArray()))).ToArray();
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

    public class PortEventArgs : EventArgs
    {
        public string Port;
    }

    //public class StreamEventArgs : EventArgs
    //{
    //    public Stream Stream;
    //}

    //public class Commands
    //{
    //    public bool asdf = false;
    //}

    public class DataModel : INotifyPropertyChanged
    {
        //public class Reply
        //{
        //    public bool Found = false;
        //    public byte[] RxBuffer;
        //}

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
            get => cmtFL == "N" || cmtFL == "T" ? Color.Green : Color.Red;
        }

        public Color CmtRLColor
        {
            get => cmtRL == "N" || cmtRL == "T" ? Color.Green : Color.Red;
        }

        public Color CmtFRColor
        {
            get => cmtFR == "N" || cmtFR == "T" ? Color.Green : Color.Red;
        }

        public Color CmtRRColor
        {
            get => cmtRR == "N" || cmtRR == "T" ? Color.Green : Color.Red;
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

        private bool cowIdOk = false;
        public bool CowIdOk
        {
            get => cowIdOk;
            set
            {
                cowIdOk = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CowIdOk)));
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

        //public float Progress0
        //{
        //    get => (Aptxs[0].ProcessPulses - Aptxs[0].PulsesPrev) / ECOMILK_PROCESS_PULSES;
        //    set
        //    {
        //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Progress0)));
        //    }
        //}

        //public float Progress1
        //{
        //    get => (Aptxs[1].ProcessPulses - Aptxs[1].PulsesPrev) / ECOMILK_PROCESS_PULSES;
        //    set
        //    {
        //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Progress1)));
        //    }
        //}

        //public float Progress2
        //{
        //    get => (Aptxs[2].ProcessPulses - Aptxs[2].PulsesPrev) / ECOMILK_PROCESS_PULSES;
        //    set
        //    {
        //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Progress2)));
        //    }
        //}

        //public float Progress3
        //{
        //    get => (Aptxs[3].ProcessPulses - Aptxs[3].PulsesPrev) / ECOMILK_PROCESS_PULSES;
        //    set
        //    {
        //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Progress3)));
        //    }
        //}

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

        private bool autoTransition = false;
        public bool AutoTransition
        {
            get => autoTransition;
            set
            {
                autoTransition = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AutoTransition)));
                if (value == true)
                {
                    if (CmtFRColor == Color.Red)
                    {
                        fr = true;
                        FR = FR;
                    }
                    if (CmtRRColor == Color.Red)
                    {
                        rr = true;
                        RR = RR;
                    }
                    if (CmtRLColor == Color.Red)
                    {
                        rl = true;
                        RL = RL;
                    }
                    if (CmtFLColor == Color.Red)
                    {
                        fl = true;
                        FL = FL;
                    }
                }
            }
        }

        private string[] usbPorts = new string[0];
        //public string UsbPorts
        //{
        //    get => usbPorts.Aggregate("", (r, v) => r += v + " ");
        //    set
        //    {
        //        //usbPorts = value.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UsbPorts)));
        //    }
        //}

        //private uint packetCounter = 0;
        //public uint PacketCounter
        //{
        //    get => packetCounter;
        //    set
        //    {
        //        packetCounter = value;
        //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PacketCounter)));
        //    }
        //}

        private Dictionary<string, uint> packetCounters = new Dictionary<string, uint>();
        public string PacketCounters
        {
            get => usbPorts.Aggregate("", (r, v) => r += v + " ") + "\n" + packetCounters.Aggregate("", (r, v) => r += v.Key + DELIMITER + v.Value + "\n");
            //get => packetCounters.Aggregate("", (r, v) => r += v + DELIMITER);
            set
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PacketCounters)));
            }
        }

        private readonly string version = VERSION + " " + DateTime.Now;
        public string Version
        {
            get => version;
        }

        private enum PacketType
        {
            EMPTY,
            REMOTE_STATUS_0,
            REMOTE_STATUS_1,
            REMOTE_STATUS_2,
            REMOTE_STATUS_3,
            REMOTE_START,
            REMOTE_STOP,
            RFID_TAG,
            APTX1_ID,
            APTX1_SNUM,
            APTX1_CURRENT,
            APTX1_APTXID,
            ECOMILK_ID,
            ECOMILK_RCW_START,
            ECOMILK_RCW_STOP,
            ECOMILK_RCCW_START,
            ECOMILK_RCCW_STOP,
            ECOMILK_AF_START,
            ECOMILK_AB_START,
            ECOMILK_TCW_START,
            ECOMILK_XF_START,
            ECOMILK_MZD_STOP,
            ECOMILK_MZD_START,
            ECOMILK_MZU_STOP,
            ECOMILK_MZU_START,
            ECOMILK_AB_STOP,
            ECOMILK_AF_STOP,
        }

        private enum ProcedureType
        {
            APTX2,
            ECOMILK,
        }

        private class TxPacket
        {
            public static TxPacket Empty = new TxPacket() { device = string.Empty, packetType = PacketType.EMPTY };
            public string device;
            public PacketType packetType;
            public byte[] packet;
        }

        public const string VERSION = "Armenta - Remote Control Application V1.3";
        public const uint UERROR = 0xFFFFFFFF;
        private const int OK = 0;
        private const int ERROR = -1;

        private const string APTX1 = "APTX1";
        private const string REMOTE = "REMOTE";
        private const string ECOMILK = "ECOMILK";
        private const string RFID = "RFID";

        //private const uint APTX_COUNT = 4;

        //private readonly uint[] STATEs; // = new uint[APTX_COUNT];
        //private const int QUARTERS_NUMBER = 4;
        //private const int CONNECT_TIMEOUT = 3000;
        //private const int REQUEST_TIMEOUT = 1000; // in msec
        //private const int REQUEST_RETRIES = 3;
        //private const int REPLY_TIMEOUT = 1000; // in msec
        //private const int REPLY_RETRIES = 3;
        //private const int TXQUE_TIMEOUT = 1;
        private const int TXDEQUE_TIMEOUT = 100;
        //private const int TXRX_RETRIES = 1;
        //private const int RXBUFFER_SIZE = 1024;

        private const string LOGFILE_COWS = "LOGFILE_COWS";
        private const int MAX_RXBUFFER_LENGTH = 1024;
        private const string DELIMITER = "/";

        //public Command AddCow { get; }
        public Command TappedFL { get; }
        public Command TappedRL { get; }
        public Command TappedFR { get; }
        public Command TappedRR { get; }

        public Command NextPageCMT { get; }
        public Command NextPageTreatment { get; }

        //public string[] Devices;

        //public Aptx[] Aptxs = new Aptx[Aptx.APTXIDs.Length].Select((a, i) => { a = new Aptx(); a.Id = Aptx.APTXIDs[i]; return a; }).ToArray();
        public Aptx[] Aptxs = new Aptx[Aptx.APTXIDs.Length].Select((a, i) => { a = new Aptx(); a.Id = (ushort)i; return a; }).ToArray();
        private Semaphore SemaphoreAptxs = new Semaphore(1, 1);
        public Aptx Aptx = new Aptx();

        //private Dictionary<uint, string> Cows = new Dictionary<uint, string>();

        public event PropertyChangedEventHandler PropertyChanged;

        private IUsbSerial UsbSerial;

        //private ConcurrentDictionary<string, string> Ports = new ConcurrentDictionary<string, string>();
        private Dictionary<string, string> Ports = new Dictionary<string, string>();
        private Semaphore SemaphorePorts = new Semaphore(1, 1);

        //ManualResetEvent WaitHandleEcomilk = new ManualResetEvent(false);
        //ManualResetEvent WaitHandleRemote = new ManualResetEvent(false);
        //ManualResetEvent WaitHandleRfid = new ManualResetEvent(false);
        //ManualResetEvent WaitHandleAptx1 = new ManualResetEvent(false);

        //System.Collections.Concurrent.ConcurrentDictionary<> conc;
        //private bool Connected = false;
        private byte PauseResume = Aptx.STOP;

        //private byte[] RxBufferRfid = new byte[0];// RXBUFFER_SIZE];
        //private byte[] RxBufferAptx1 = new byte[0];// RXBUFFER_SIZE];
        //private byte[] RxBufferEcomilk = new byte[0];// RXBUFFER_SIZE];
        //private byte[] RxBufferRemote = new byte[0];// RXBUFFER_SIZE];
        private Dictionary<string, byte[]> RxBuffers = new Dictionary<string, byte[]>();
        private Semaphore SemaphoreRxBuffers = new Semaphore(1, 1);

        private List<TxPacket> TxQue;
        private Semaphore SemaphoreTxQue = new Semaphore(1, 1);
        //ManualResetEvent WaitHandleTxQue = new ManualResetEvent(false);

        private Timer TxDequeTimer;
        
        private int Count = 0;
        private uint PulsesPrev = 0;

        private ProcedureType Procedure = ProcedureType.APTX2;

        private Semaphore SemaphorePacketCounters = new Semaphore(1, 1);

        public DataModel(IUsbSerial usbSerial)
        {
            //Aptxs = new Aptx[Aptx.APTXIDs.Length].Select((a, i) => { a = new Aptx(); a.Id = Aptx.APTXIDs[i]; return a; }).ToArray();
            if (Device.RuntimePlatform == Device.Android)
            {
                //Devices = new string[] { REMOTE };
                //Devices = new string[] { REMOTE, APTX1 };
                TxQue = new List<TxPacket>() {
                    //new TxPacket() { device = ECOMILK, packetType = PacketType.ECOMILK_ID, packet = Encoding.UTF8.GetBytes("ecomilkid\r")},
                    //new TxPacket() { device = REMOTE, packetType = PacketType.REMOTE_STATUS_3, packet = Aptxs[3].PacketBuild() },
                    new TxPacket() { device = APTX1, packetType = PacketType.APTX1_ID, packet = Encoding.UTF8.GetBytes("getid,3#")},
                    new TxPacket() { device = RFID, packetType = PacketType.RFID_TAG, packet = new RfId().PacketBuild()},
                };
            }
            else if (Device.RuntimePlatform == Device.UWP)
            {
                //Devices = new string[] { ECOMILK, REMOTE, RFID, APTX1 };
                //Devices = new string[] { APTX1 };
                //Devices = new string[] { REMOTE };
                TxQue = new List<TxPacket>() {
                    new TxPacket() { device = ECOMILK, packetType = PacketType.ECOMILK_ID, packet = Encoding.UTF8.GetBytes("ecomilkid\r")},

                    new TxPacket() { device = RFID, packetType = PacketType.RFID_TAG, packet = new RfId().PacketBuild()},

                    new TxPacket() { device = REMOTE, packetType = PacketType.REMOTE_STATUS_0, packet = Aptxs[0].PacketBuild() },
                    new TxPacket() { device = REMOTE, packetType = PacketType.REMOTE_STATUS_1, packet = Aptxs[1].PacketBuild() },
                    new TxPacket() { device = REMOTE, packetType = PacketType.REMOTE_STATUS_2, packet = Aptxs[2].PacketBuild() },
                    new TxPacket() { device = REMOTE, packetType = PacketType.REMOTE_STATUS_3, packet = Aptxs[3].PacketBuild() },

                    new TxPacket() { device = APTX1, packetType = PacketType.APTX1_ID, packet = Encoding.UTF8.GetBytes("getid,3#\r")},
                };
                Procedure = ProcedureType.ECOMILK;
            }

            UsbSerial = usbSerial;
            UsbSerial.Event((sender, args) =>
            {
                SemaphorePorts.WaitOne();
                if (args is PortEventArgs)
                {
                    if (Ports.ContainsValue((args as PortEventArgs).Port))
                    {
                        //string key = Ports.Select((kv) => kv.Value == (args as PortEventArgs).Port ? kv : new KeyValuePair<string, string>()).First().Key;
                        string key = Ports.First((kv) => kv.Value == (args as PortEventArgs).Port).Key;
                        Ports.Remove(key);
                        CowId = UERROR;
                        Aptx.SNum = UERROR;
                        Aptx.CurrentPulses = UERROR;
                        Aptx.Remaining = UERROR;
                        Aptx.aptxId[0] = UERROR;
                        Aptx.aptxId[1] = UERROR;
                        Aptx.aptxId[2] = UERROR;
                    }
                }
                SemaphorePorts.Release();
            }, null);

            NextPageCMT = new Command(async () =>
            {
                await Application.Current.MainPage.Navigation.PushAsync(new CMTPage());
            });
            NextPageTreatment = new Command(async () =>
            {
                await Application.Current.MainPage.Navigation.PushAsync(new ProcessPage());
            });
            //AddCow = new Command(() =>
            //{
            //    if (!Cows.ContainsKey(CowId))
            //        Cows.Add(CowId, TagId);
            //});

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

            //foreach (string dev in Devices)
            //    new Thread((device) => { Rx(device); })
            //    { Name = dev }.Start(dev);

            //new Thread(() => { Tx(); })
            //{ Name = "Tx" }.Start();
            
            //WaitHandleTxQue.Set();

            //new Thread(() => { TxDeque(); })
            //{ Name = "Tx" }.Start();
            //ThreadPool.QueueUserWorkItem( (o) => { TxDeque(); });
            //new Thread(() => { TxDeque(); }){ Priority = ThreadPriority.BelowNormal }.Start();

            //Task.Run(() => { TxDeque(); });

            TxDequeTimer = new Timer((a) => { TxDeque(); }, null, 0, TXDEQUE_TIMEOUT);
        }

        private async Task TxRx(TxPacket txPacket, string device)
        {
            try
            {
                int ret = -1;
                //for (int i = 0; i < TXRX_RETRIES; i++)
                //{
                if (Ports.TryGetValue(device, out string port))
                {
                    ret = await PortRequest(txPacket, port);
                    await PortReply(device, port);
                }
                else
                {
                    string[] ports = UsbSerial.GetPorts().ToArray();
                    usbPorts = ports;
                    //UsbPorts = UsbPorts;
                    PacketCounters = PacketCounters;
                    foreach (string prt in ports)
                    {
                        if (!Ports.Values.Contains(prt))
                        {
                            ret = await PortRequest(txPacket, prt);
                        }
                    }
                    foreach (string prt in ports)
                    {
                        if (!Ports.Values.Contains(prt))
                        {
                            if (await PortReply(device, prt))
                            {
                                SemaphorePorts.WaitOne();
                                if (!Ports.ContainsKey(device))
                                {
                                    Ports.Add(device, prt);
                                }
                                SemaphorePorts.Release();
                            }
                        }
                    }
                }
                //}
            }
            catch
            {
            }
        }

        private async Task TxDeque()
        {
            //while (true)
            //{
            try
            {
                if (TxQue.Any())
                {
                    TxPacket txPacket = TxQue.First();
                    string device = txPacket.device;

                    //Task.Run(async () => { await TxRx(txPacket, device); }).Wait(TXRX_TIMEOUT);
                    Task.Run(async () => { await TxRx(txPacket, device); });

                    SemaphoreTxQue.WaitOne();
                    TxQue.Remove(txPacket);
                    if (txPacket.packetType != PacketType.EMPTY)
                        TxQue.Add(txPacket);
                    SemaphoreTxQue.Release();

                }
                //else
                //    Thread.Sleep(TXRX_TIMEOUT);
            }
            catch
            {
                //Thread.Sleep(TXRX_TIMEOUT);
            }
            //}
        }

        //private async Task<bool> PortReply(string device, string port, Dictionary<string, byte[]> RxBuffers)
        private async Task<bool> PortReply(string device, string port)
        {
            SemaphoreRxBuffers.WaitOne();
            if (!RxBuffers.TryGetValue(port, out byte[] rxBuffer))
            {
                //rxBuffers.Add(port, (rxBuffer = new byte[0]));
                rxBuffer = new byte[0];
                RxBuffers.Add(port, rxBuffer);
            }
            if(rxBuffer.Length > MAX_RXBUFFER_LENGTH)
                rxBuffer = new byte[0];
            SemaphoreRxBuffers.Release();

            byte[] buffer = new byte[1024];
            //data += await UsbSerial.Read(port, buffer);
            int length = 0;
            bool found = false;
            string data;
            if ((length = await UsbSerial.Read(port, buffer)) > 0)
            {
                rxBuffer = rxBuffer.Concat(buffer.Take(length)).ToArray();
            }

            if (rxBuffer.Length > 0)
            {
                switch (device)
                {
                    case ECOMILK:
                        data = Encoding.UTF8.GetString(rxBuffer);
                        found = data.Contains(device);
                        if (found)
                        {
                            rxBuffer = new byte[0];
                            //PacketCounter++;
                            SemaphorePacketCounters.WaitOne();
                            if (packetCounters.ContainsKey(port + DELIMITER + device))
                                packetCounters[port + DELIMITER + device]++;
                            else
                                packetCounters.Add(port + DELIMITER + device, 0);
                            PacketCounters = PacketCounters;
                            SemaphorePacketCounters.Release();
                        }
                        break;
                    case RFID:
                        //byte[] RxBuffer = new byte[20];
                        //RxBuffer = RxBuffer.Select((b, i) => b = (byte)i).ToArray();
                        //RxBuffer[10] = RfId.PREEMBLE;
                        //RxBuffer[10 + 19] = RfId.END_MARK;
                        RfId rfId = new RfId();
                        //RxBuffer[10 + 20] = rfId.UshortToArray(rfId.CrcCalc(RxBuffer.Skip(11).Take(19).ToArray()))[0];
                        //RxBuffer[10 + 21] = rfId.UshortToArray(rfId.CrcCalc(RxBuffer.Skip(11).Take(19).ToArray()))[1];
                        uint pcktCnt = rfId.PacketParse(ref rxBuffer, ref found);
                        if (pcktCnt > 0)
                        {
                            SemaphorePacketCounters.WaitOne();
                            if (packetCounters.ContainsKey(port + DELIMITER + device))
                                packetCounters[port + DELIMITER + device] += pcktCnt;
                            else
                                packetCounters.Add(port + DELIMITER + device, 0);
                            PacketCounters = PacketCounters;
                            //PacketCounters.Contains(port + DELIMITER + device);
                            SemaphorePacketCounters.Release();
                        }
                        if (found)
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
                        //byte[] RxBuffer = new byte[30];
                        //RxBuffer = RxBuffer.Select((b, i) => b = (byte)i).ToArray();
                        //RxBuffer[10] = Aptx.STX;
                        //RxBuffer[11] = Aptx.APTXIDs[0];
                        //RxBuffer[10 + 32] = Aptx.ETX;
                        //Aptx aptx = new Aptx();
                        //RxBuffer[10 + 33] = aptx.UshortToArray(aptx.ChecksumCalc(RxBuffer.Skip(10).Take(33).ToArray()))[0];
                        //RxBuffer[10 + 34] = aptx.UshortToArray(aptx.ChecksumCalc(RxBuffer.Skip(10).Take(33).ToArray()))[1];
                        pcktCnt = Aptx.PacketParse(ref rxBuffer, ref found);
                        if (pcktCnt > 0)
                        {
                            SemaphorePacketCounters.WaitOne();
                            if (packetCounters.ContainsKey(port + DELIMITER + device))
                                packetCounters[port + DELIMITER + device] += pcktCnt;
                            else
                                packetCounters.Add(port + DELIMITER + device, 0);
                            PacketCounters = PacketCounters;
                            SemaphorePacketCounters.Release();
                        }
                        if (found)
                        {
                            //if (Aptx.APTXIDs.Contains((byte)Aptx.Id))
                            SemaphoreAptxs.WaitOne();
                            if (Aptxs.Any(a => a.Id == Aptx.Id))
                            {
                                AptxUpdate();
                                Aptxs[Aptx.Id] = Aptx;
                            }
                            SemaphoreAptxs.Release();
                        }
                        break;
                    case APTX1:
                        {
                            //data = Encoding.UTF8.GetString(rxBuffer.Where(b => b != 0x00).ToArray());
                            data = Encoding.UTF8.GetString(rxBuffer);
                            if (!Ports.ContainsKey(device))
                            {
                                found = data.Contains("1F-85-01");
                                if (!found)
                                    found = data.Contains("0x1f-0x85-0x01");
                                if (found)
                                {
                                    rxBuffer = new byte[0];
                                    //PacketCounter++;
                                    SemaphorePacketCounters.WaitOne();
                                    if (packetCounters.ContainsKey(port + DELIMITER + device))
                                        packetCounters[port + DELIMITER + device]++;
                                    else
                                        packetCounters.Add(port + DELIMITER + device, 0);
                                    PacketCounters = PacketCounters;
                                    SemaphorePacketCounters.Release();
                                }
                            }
                            else
                            {
                                if (data.Contains("SNUM "))
                                {
                                    uint[] snum = DataParse(data, "SNUM ", NumberStyles.Number);
                                    if (snum[0] == 123)
                                    {
                                        Aptx.SNum++;
                                        rxBuffer = new byte[0];
                                        //PacketCounter++;
                                        SemaphorePacketCounters.WaitOne();
                                        if (packetCounters.ContainsKey(port + DELIMITER + device))
                                            packetCounters[port + DELIMITER + device]++;
                                        else
                                            packetCounters.Add(port + DELIMITER + device, 0);
                                        PacketCounters = PacketCounters;
                                        SemaphorePacketCounters.Release();
                                    }
                                }
                                //if (Aptx.SNum == UERROR)
                                //{
                                //    if (data.Contains("SNUM "))
                                //    {
                                //        uint[] snum = DataParse(data, "SNUM ", NumberStyles.Number);
                                //        Aptx.SNum = snum[0];
                                //        rxBuffer = new byte[0];
                                //    }
                                //}
                                //if ((Aptx.aptxId[0] == UERROR) ||
                                //    (Aptx.aptxId[1] == UERROR) ||
                                //    (Aptx.aptxId[2] == UERROR))
                                //{
                                //    if (data.Contains("readid Device_id"))
                                //    {
                                //        uint[] aptid = DataParse(data, "readid Device_id", NumberStyles.HexNumber);
                                //        Aptx.aptxId[0] = aptid[0];
                                //        Aptx.aptxId[1] = aptid[1];
                                //        Aptx.aptxId[2] = aptid[2];
                                //        Aptx.AptxId = Aptx.AptxId;
                                //        rxBuffer = new byte[0];
                                //    }
                                //}
                                //if ((Aptx.CurrentPulses == UERROR) || (Aptx.Maxi == UERROR))
                                //{
                                //    if (data.Contains("MAXI "))
                                //    {
                                //        uint[] maxi = DataParse(data, "MAXI ", NumberStyles.Number);
                                //        Aptx.Maxi = maxi[0];
                                //        rxBuffer = new byte[0];
                                //    }
                                //    if (data.Contains("Found: ") || data.Contains("pulses written "))
                                //    {
                                //        uint[] current = DataParse(data, "Found: ", NumberStyles.Number);
                                //        if (current[0] == 0)
                                //            current = DataParse(data, "pulses written ", NumberStyles.Number);
                                //        Aptx.CurrentPulses = current[0];
                                //        rxBuffer = new byte[0];
                                //    }
                                //    if ((Aptx.Maxi != UERROR) && (Aptx.CurrentPulses != UERROR))
                                //        Aptx.Remaining = Aptx.Maxi - Aptx.CurrentPulses;
                                //}
                            }
                        }
                        break;
                }
            }

            SemaphoreRxBuffers.WaitOne();
            if (RxBuffers.ContainsKey(port))
            {
                RxBuffers.Remove(port);
            }
            RxBuffers.Add(port, rxBuffer);
            SemaphoreRxBuffers.Release();
            return found;
        }

        //        data = Encoding.UTF8.GetString(rxBuffer);
        //        if (!(found = data.Contains(ECOMILK)))
        //        {
        //            RfId rfId = new RfId();
        //            if (found = rfId.PacketParse(ref rxBuffer))
        //            {
        //                unsafe
        //                {
        //                    fixed (byte* pepc = rfId.EPC)
        //                    {
        //                        CowId = rfId.ArrayToUint(pepc);
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                if (found = Aptx.PacketParse(ref rxBuffer))
        //                {
        //                    if (Aptx.APTXIDs.Contains((byte)Aptx.Id))
        //                    {
        //                        AptxUpdate();
        //                        Aptxs[Aptx.Id - 1] = Aptx;
        //                    }
        //                }
        //                else
        //                {
        //                    data = Encoding.UTF8.GetString(rxBuffer.Where(b => b != 0x00).ToArray());
        //                    if (!Ports.ContainsKey(device))
        //                    {
        //                        found = data.Contains("1F-85-01");
        //                        if (!found)
        //                            found = data.Contains("0x1f-0x85-0x01");
        //                        if (found)
        //                            rxBuffer = new byte[0];
        //                    }
        //                    else
        //                    {
        //                        if (data.Contains("SNUM "))
        //                        {
        //                            uint[] snum = DataParse(data, "SNUM ", NumberStyles.Number);
        //                            if (snum[0] == 123)
        //                            {
        //                                Aptx.SNum++;
        //                                rxBuffer = new byte[0];
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    SemaphoreRxBuffers.WaitOne();
        //    if (RxBuffers.ContainsKey(port))
        //    {
        //        RxBuffers.Remove(port);
        //        RxBuffers.Add(port, rxBuffer);
        //    }
        //    SemaphoreRxBuffers.Release();
        //    return found;
        //}
        
        //private async Task<int> PortRequest(string device, string port, uint state)
        private async Task<int> PortRequest(TxPacket txPacket, string port)
        {
            int ret = ERROR;
            switch (txPacket.device)
            {
                case ECOMILK:
                    if (txPacket.packetType != PacketType.ECOMILK_ID)
                        txPacket.packetType = PacketType.EMPTY;
                    break;
                case REMOTE:
                    if (txPacket.packetType != PacketType.REMOTE_STATUS_0 &&
                        txPacket.packetType != PacketType.REMOTE_STATUS_1 &&
                        txPacket.packetType != PacketType.REMOTE_STATUS_2 &&
                        txPacket.packetType != PacketType.REMOTE_STATUS_3)
                        txPacket.packetType = PacketType.EMPTY;
                    break;
                case APTX1:
                    //switch (txPacket.packetType)
                    //{
                    //    case PacketType.APTX1_ID:
                    //        //if (Ports.ContainsKey(txPacket.device))
                    //        //{
                    //        txPacket.packetType = PacketType.APTX1_SNUM;
                    //        txPacket.packet = Encoding.UTF8.GetBytes("testread,3#");
                    //        //}
                    //        break;
                    //    case PacketType.APTX1_SNUM:
                    //        //if (Aptx.SNum != UERROR)
                    //        //{
                    //        txPacket.packetType = PacketType.APTX1_CURRENT;
                    //        txPacket.packet = Encoding.UTF8.GetBytes("find,3#");
                    //        //}
                    //        break;
                    //    case PacketType.APTX1_CURRENT:
                    //        //if (Aptx.CurrentPulses != UERROR)
                    //        //{
                    //        //txPacket.packetType = PacketType.APTX1_APTXID;
                    //        //txPacket.packet = Encoding.UTF8.GetBytes("readid#");
                    //        txPacket.packetType = PacketType.APTX1_APTXID;
                    //        txPacket.packet = Encoding.UTF8.GetBytes("readid#");
                    //        //}
                    //        break;
                    //    case PacketType.APTX1_APTXID:
                    //        //if ((Aptx.aptxId[0] != UERROR) ||
                    //        //    (Aptx.aptxId[1] != UERROR) ||
                    //        //    (Aptx.aptxId[2] != UERROR))
                    //        //{
                    //        //    txPacket.packetType = PacketType.EMPTY;
                    //        //}
                    //        txPacket.packetType = PacketType.APTX1_ID;
                    //        txPacket.packet = Encoding.UTF8.GetBytes("getid,3#");
                    //        break;
                    //}
                    if (!Ports.ContainsKey(txPacket.device))
                    {
                        txPacket.packetType = PacketType.APTX1_ID;
                        txPacket.packet = Encoding.UTF8.GetBytes("#getid,3#");
                    }
                    else
                    {
                        txPacket.packetType = PacketType.APTX1_SNUM;
                        txPacket.packet = Encoding.UTF8.GetBytes("#testread,3#");
                        //if (Aptx.SNum == UERROR)
                        //{
                        //    txPacket.packetType = PacketType.APTX1_SNUM;
                        //    txPacket.packet = Encoding.UTF8.GetBytes("testread,3#");
                        //}
                        //else if (Aptx.CurrentPulses == UERROR)
                        //{
                        //    txPacket.packetType = PacketType.APTX1_CURRENT;
                        //    txPacket.packet = Encoding.UTF8.GetBytes("find,3#");
                        //}
                        //else if ((Aptx.aptxId[0] == UERROR) ||
                        //    (Aptx.aptxId[1] == UERROR) ||
                        //    (Aptx.aptxId[2] == UERROR))
                        //{
                        //    txPacket.packetType = PacketType.APTX1_ID;
                        //    txPacket.packet = Encoding.UTF8.GetBytes("getid,3#");
                        //}
                    }
                    break;
            }

            //if((!CowIdOk) || (port == "COM14"))
            ret = await UsbSerial.Write(port, txPacket.packet);

            return ret;
        }

        private void AptxUpdate()
        {
            Aptx.AptxId = Aptx.AptxId;
            Aptx.Remaining = Aptx.Maxi - Aptx.CurrentPulses;
            Aptx.PressureOK = Aptx.PressureOK;
            Aptx.PressureLow = Aptx.PressureLow;
            Aptx.BatteryOK = Aptx.BatteryOK;
            Aptx.BatteryLow = Aptx.BatteryLow;
            Aptx.RemainingOK = Aptx.RemainingOK;
            Aptx.RemainingLow = Aptx.RemainingLow;
            Aptx.AptPulsesOK = Aptx.AptPulsesOK;
            Aptx.AptPulsesLow = Aptx.AptPulsesLow;

            switch (Procedure)
            {
                case ProcedureType.APTX2:
                    if ((Aptx.ProcessPulses == 200) && (PulsesPrev > 190) && (PulsesPrev < 200))
                    {
                        Count++;
                        if (Count == 2)
                        {
                            if (AutoTransition)
                            {
                                if (fr)
                                {
                                    fr = false;
                                    FR = FR;
                                    Count = 0;
                                    //CmtSave(string.Format("CmtFR: {0}", CmtFR));
                                }
                                else
                                {
                                    if (rr)
                                    {
                                        rr = false;
                                        RR = RR;
                                        Count = 0;
                                        //CmtSave(string.Format("CmtRR: {0}", CmtRR));
                                    }
                                    else
                                    {
                                        if (rl)
                                        {
                                            rl = false;
                                            RL = RL;
                                            Count = 0;
                                            //CmtSave(string.Format("CmtRL: {0}", CmtRL));
                                        }
                                        else
                                        {
                                            if (fl)
                                            {
                                                fl = false;
                                                FL = FL;
                                                Count = 0;
                                                //CmtSave(string.Format("CmtFL: {0}", CmtFL));
                                            }
                                        }
                                    }
                                }
                            }
                            if (!fl && !rl && !fr && !rr)
                                CmtSave(string.Format("CmtFL: {0} CmtRL: {1} CmtFR: {2} CmtRR: {3}", CmtFL, CmtRL, CmtFR, CmtRR));
                        }
                    }
                    //if ((Aptx.ProcessPulses > 10) && (PulsesPrev < 10) && (Count >= 2))
                    //    Count = 0;
                    PulsesPrev = Aptx.ProcessPulses;
                    break;
                case ProcedureType.ECOMILK:
                    CmtSave(string.Format("CmtFL: {0} CmtRL: {1} CmtFR: {2} CmtRR: {3}", CmtFL, CmtRL, CmtFR, CmtRR));
                    break;
            }
        }

        private uint[] DataParse(string data, string pattern, NumberStyles numberStyles)
        {
            uint[] num = new uint[6];
            string snum = data.Contains(pattern) ?
                new string(data.Substring(data.IndexOf(pattern) + pattern.Length)?.TakeWhile(c => ((c != '\r') && (c != 'p') && (c != ' ')))?.ToArray())
                : string.Empty;
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

        public void CmtSave(string cmtData)
        {
            //if (CowId != UERROR)
            //{
            string LOGFILE_COWS = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "LogFileCows.txt");
            //File.AppendAllText(LOGFILE_COWS, string.Format("CowId: {0} CmtFL: {1} CmtRL: {2} CmtFR: {3} CmtRR: {4} Date: {5}\n",
            //    CowId, CmtFL, CmtRL, CmtFR, CmtRR, DateTime.Now));
            File.AppendAllText(LOGFILE_COWS, string.Format("CowId: {0} {1} Date: {2}\n",
                CowId, cmtData, DateTime.Now));
            //}
        }

        public void CmtRead()
        {
            string LOGFILE_COWS = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "LogFileCows.txt");
            if (File.Exists(LOGFILE_COWS))
            {
                string data = File.ReadAllText(LOGFILE_COWS);
                if (data.Contains("CowId"))
                {
                    //string line = data.Split(new char[] { '\n' }).Where(l => l.Contains("CowId")).Last();
                    //uint[] cowId = DataParse(line, "CowId: ", NumberStyles.Number);
                    //if (cowId[0] == CowId)
                    IEnumerable<string> lines = data.Split(new char[] { '\n' }).Where(l =>
                    {
                        uint[] cowId = DataParse(l, "CowId: ", NumberStyles.Number);
                        if (cowId[0] == CowId)
                            return true;
                        else return false;
                    });
                    if(lines.Any())
                    {
                        string line = lines.Last();
                        CmtFL = CmtParse(line, "CmtFL: ");
                        CmtRL = CmtParse(line, "CmtRL: ");
                        CmtFR = CmtParse(line, "CmtFR: ");
                        CmtRR = CmtParse(line, "CmtRR: ");
                    }
                }
            }
        }

        private string CmtParse(string line, string pattern)
        {
            return new string(line.Substring(line.IndexOf(pattern) + pattern.Length).Take(1).ToArray());
        }

        public async Task<int> ProcessStart()
        {
            Aptx[] aptxs = Aptxs;
            foreach (Aptx aptx in aptxs)
            {
                //if (await Process(aptx, Aptx.START) == ERROR)
                //return ERROR;
                aptx.PulsesPrev = aptx.ProcessPulses;
                await Process(aptx, Aptx.START);
            }
            return OK;
        }

        public async Task<int> ProcessStop()
        {
            Aptx[] aptxs = Aptxs;
            foreach (Aptx aptx in aptxs)
            { 
                //if (await Process(aptx, Aptx.STOP) == ERROR)
                //return ERROR;
                await Process(aptx, Aptx.STOP);
            }
            return OK;
        }

        public async Task<int> ProcessPauseResume()
        {
            PauseResume = PauseResume == Aptx.STOP ? PauseResume = Aptx.START : PauseResume = Aptx.STOP;
            Aptx[] aptxs = Aptxs;
            foreach (Aptx aptx in aptxs)
            {
                //if (await Process(aptx, PauseResume, (ushort)(Aptx.PULSES100 - (aptx.ProcessPulses - aptx.PulsesPrev))) == ERROR)
                //return ERROR;
                await Process(aptx, PauseResume, (ushort)(Aptx.PULSES100 - (aptx.ProcessPulses - aptx.PulsesPrev)));
            }
            return OK;
        }

        //public async Task<int> Process(Aptx aptx, byte process, ushort pulses = Aptx.PULSES100)
        public async Task Process(Aptx aptx, byte process, ushort pulses = Aptx.PULSES100)
        {
            //int response = await UsbSerial.Write(Ports.TryGetValue(REMOTE, out string val) ? val : string.Empty,
            //    aptx.PacketBuild(process, pulses));

            await TxQueEnque(REMOTE, process == Aptx.START ? PacketType.REMOTE_START : PacketType.REMOTE_STOP, aptx.PacketBuild(process, pulses));

            //string LOGFILE_COWS = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "LogFileCows.txt");
            //File.AppendAllText(LOGFILE_COWS, string.Format("Date: {0} Process: {1} Pulses: {2} Cow Id: {3} Current Pulses: {4}\n",
            //    //DateTime.Now, response == OK ? "OK" : "FAULT", process == Aptx.START ? "START" : "STOP",
            //    DateTime.Now, process == Aptx.START ? "START" : "STOP",
            //    pulses, CowId, aptx.ProcessPulses));
            //return response;
        }

        private async Task TxQueEnque(string device, PacketType packetType, byte[] packet)
        {
            SemaphoreTxQue.WaitOne();
            TxQue.Add(new TxPacket()
            {
                device = device,
                packetType = packetType,
                packet = packet,
            });
            SemaphoreTxQue.Release();
        }

        public async Task RCWStart()
        {
            await TxQueEnque(ECOMILK, PacketType.ECOMILK_RCW_START, Encoding.UTF8.GetBytes("rcw 1\r"));
            //await UsbSerial.Write(Ports.TryGetValue(ECOMILK, out var val) ? val : string.Empty, Encoding.UTF8.GetBytes("rcw 1\r"));
        }

        public async Task RCWStop()
        {
            await TxQueEnque(ECOMILK, PacketType.ECOMILK_RCW_STOP, Encoding.UTF8.GetBytes("rcw 0\r"));
            //await UsbSerial.Write(Ports.TryGetValue(ECOMILK, out var val) ? val : string.Empty, Encoding.UTF8.GetBytes("rcw 0\r"));
        }

        public async Task RCCWStart()
        {
            await TxQueEnque(ECOMILK, PacketType.ECOMILK_RCCW_START, Encoding.UTF8.GetBytes("rccw 1\r"));
            //await UsbSerial.Write(Ports.TryGetValue(ECOMILK, out var val) ? val : string.Empty, Encoding.UTF8.GetBytes("rccw 1\r"));
        }

        public async Task RCCWStop()
        {
            await TxQueEnque(ECOMILK, PacketType.ECOMILK_RCCW_STOP, Encoding.UTF8.GetBytes("rccw 0\r"));
            //await UsbSerial.Write(Ports.TryGetValue(ECOMILK, out var val) ? val : string.Empty, Encoding.UTF8.GetBytes("rccw 0\r"));
        }




        public async Task AFStart()
        {
            await TxQueEnque(ECOMILK, PacketType.ECOMILK_AF_START, Encoding.UTF8.GetBytes("af 1\r"));
            //await UsbSerial.Write(Ports.TryGetValue(ECOMILK, out var val) ? val : string.Empty, Encoding.UTF8.GetBytes("af 1\r"));
        }

        public async Task AFStop()
        {
            await TxQueEnque(ECOMILK, PacketType.ECOMILK_AF_STOP, Encoding.UTF8.GetBytes("af 0\r"));
            //await UsbSerial.Write(Ports.TryGetValue(ECOMILK, out var val) ? val : string.Empty, Encoding.UTF8.GetBytes("af 0\r"));
        }

        public async Task ABStart()
        {
            await TxQueEnque(ECOMILK, PacketType.ECOMILK_AB_START, Encoding.UTF8.GetBytes("ab 1\r"));
            //await UsbSerial.Write(Ports.TryGetValue(ECOMILK, out var val) ? val : string.Empty, Encoding.UTF8.GetBytes("ab 1\r"));
        }

        public async Task ABStop()
        {
            await TxQueEnque(ECOMILK, PacketType.ECOMILK_AB_STOP, Encoding.UTF8.GetBytes("ab 0\r"));
            //await UsbSerial.Write(Ports.TryGetValue(ECOMILK, out var val) ? val : string.Empty, Encoding.UTF8.GetBytes("ab 0\r"));
        }




        public async Task MZUStart()
        {
            await TxQueEnque(ECOMILK, PacketType.ECOMILK_MZU_START, Encoding.UTF8.GetBytes("mzu 1\r"));
            //await UsbSerial.Write(Ports.TryGetValue(ECOMILK, out var val) ? val : string.Empty, Encoding.UTF8.GetBytes("mzu 1\r"));
        }

        public async Task MZUStop()
        {
            await TxQueEnque(ECOMILK, PacketType.ECOMILK_MZU_STOP, Encoding.UTF8.GetBytes("mzu 0\r"));
            //await UsbSerial.Write(Ports.TryGetValue(ECOMILK, out var val) ? val : string.Empty, Encoding.UTF8.GetBytes("mzu 0\r"));
        }

        public async Task MZDStart()
        {
            await TxQueEnque(ECOMILK, PacketType.ECOMILK_MZD_START, Encoding.UTF8.GetBytes("mzd 1\r"));
            //await UsbSerial.Write(Ports.TryGetValue(ECOMILK, out var val) ? val : string.Empty, Encoding.UTF8.GetBytes("mzd 1\r"));
        }

        public async Task MZDStop()
        {
            await TxQueEnque(ECOMILK, PacketType.ECOMILK_MZD_STOP, Encoding.UTF8.GetBytes("mzd 0\r"));
            //await UsbSerial.Write(Ports.TryGetValue(ECOMILK, out var val) ? val : string.Empty, Encoding.UTF8.GetBytes("mzd 0\r"));
        }

        public async Task TCWStart()
        {
            await TxQueEnque(ECOMILK, PacketType.ECOMILK_TCW_START, Encoding.UTF8.GetBytes("tcw 1\r"));
            //await UsbSerial.Write(Ports.TryGetValue(ECOMILK, out var val) ? val : string.Empty, Encoding.UTF8.GetBytes("tcw 1\r"));
        }

        public async Task XFStart()
        {
            await TxQueEnque(ECOMILK, PacketType.ECOMILK_XF_START, Encoding.UTF8.GetBytes("xf 1\r"));
            //await UsbSerial.Write(Ports.TryGetValue(ECOMILK, out var val) ? val : string.Empty, Encoding.UTF8.GetBytes("xf 1\r"));
        }

        //private async Task Rx(object odevice)
        //{
        //    Dictionary<string, byte[]> rxBuffers = new Dictionary<string, byte[]>();
        //    if (odevice is string)
        //    {
        //        string device = odevice as string;
        //        string data = string.Empty;
        //        while (true)
        //        {
        //            try
        //            {
        //                switch (device)
        //                {
        //                    case ECOMILK:
        //                        WaitHandleEcomilk.WaitOne();
        //                        break;
        //                    case REMOTE:
        //                        WaitHandleRemote.WaitOne();
        //                        break;
        //                    case RFID:
        //                        WaitHandleRfid.WaitOne();
        //                        break;
        //                    case APTX1:
        //                        WaitHandleAptx1.WaitOne();
        //                        break;
        //                }
        //                if (Ports.TryGetValue(device, out string port))
        //                {
        //                    await PortReply(device, port, rxBuffers);
        //                }
        //                else
        //                {
        //                    string[] ports = UsbSerial.GetPorts().ToArray();
        //                    usbPorts = ports;
        //                    UsbPorts = UsbPorts;
        //                    foreach (string prt in ports)
        //                    {
        //                        if (!Ports.Values.Contains(prt))
        //                        {
        //                            if (await PortReply(device, prt, rxBuffers))
        //                            {
        //                                SemaphorePorts.WaitOne();
        //                                if (!Ports.ContainsKey(device))
        //                                    Ports.Add(device, prt);
        //                                SemaphorePorts.Release();
        //                            }
        //                        }
        //                    }
        //                }
        //                if (usbPorts.Length == 0)
        //                    Thread.Sleep(CONNECT_TIMEOUT);
        //            }
        //            catch
        //            {
        //            }
        //        }
        //    }
        //}

        //private async Task Tx()
        //{
        //    //string data = string.Empty;
        //    //string device = string.Empty;
        //    //Stopwatch stopWatch = new Stopwatch();
        //    //stopWatch.Start();
        //    //uint state = STATEs.First();
        //    while (true)
        //    {
        //        try
        //        {
        //            //foreach (string dev in Devices)
        //            //{
        //            //    device = dev;

        //            if (TxQue.Any())
        //            {
        //                TxPacket txPacket = TxQue.First();
        //                string device = txPacket.device;
        //                int ret = -1;
        //                switch (device)
        //                {
        //                    case ECOMILK:
        //                        WaitHandleAptx1.Reset();
        //                        WaitHandleRfid.Reset();
        //                        WaitHandleRemote.Reset();
        //                        WaitHandleEcomilk.Set();
        //                        break;
        //                    case REMOTE:
        //                        WaitHandleEcomilk.Reset();
        //                        WaitHandleAptx1.Reset();
        //                        WaitHandleRfid.Reset();
        //                        WaitHandleRemote.Set();
        //                        break;
        //                    case RFID:
        //                        WaitHandleEcomilk.Reset();
        //                        WaitHandleRemote.Reset();
        //                        WaitHandleAptx1.Reset();
        //                        WaitHandleRfid.Set();
        //                        break;
        //                    case APTX1:
        //                        WaitHandleEcomilk.Reset();
        //                        WaitHandleRemote.Reset();
        //                        WaitHandleRfid.Reset();
        //                        WaitHandleAptx1.Set();
        //                        break;
        //                }
        //                //ThreadPool.QueueUserWorkItem(async (o) => { await Rx(device); });
        //                for (int i = 0; i < REQUEST_RETRIES; i++)
        //                {
        //                    if (Ports.TryGetValue(device, out string port))
        //                    {
        //                        //if (await PortRequest(device, port, state) < 0)
        //                        ret = await PortRequest(txPacket, port);
        //                    }
        //                    else
        //                    {
        //                        string[] ports = UsbSerial.GetPorts().ToArray();
        //                        usbPorts = ports;
        //                        UsbPorts = UsbPorts;
        //                        foreach (string prt in ports)
        //                        {
        //                            if (!Ports.Values.Contains(prt))
        //                                //await PortRequest(device, prt, 0);
        //                                ret = await PortRequest(txPacket, prt);
        //                        }
        //                    }
        //                    Thread.Sleep(REQUEST_TIMEOUT);
        //                }
        //                //if (ret > 0)
        //                //{
        //                SemaphoreTxQue.WaitOne();
        //                TxQue.Remove(txPacket);
        //                if (txPacket.packetType != PacketType.EMPTY)
        //                    TxQue.Add(txPacket);
        //                SemaphoreTxQue.Release();
        //                //}
        //            }
        //        }
        //        catch
        //        {
        //        }
        //    }
        //}

        //TxType packetType = txPacket.packet;
        //switch (device)
        //{
        //case ECOMILK:
        //    switch (packetType)
        //    {
        //        case TxType.ECOMILK_ID:
        //            //ret = await UsbSerial.Write(port, Encoding.UTF8.GetBytes("ecomilkid\r"));
        //            //TxQUpdate(txPacket);
        //            packet = Encoding.UTF8.GetBytes("ecomilkid\r");
        //            break;
        //    }
        //    break;
        //case RFID:
        //    switch (packetType)
        //    {
        //        case TxType.RFID_TAG:
        //            //ret = await UsbSerial.Write(port, new RfId().PacketBuild());
        //            //TxQUpdate(txPacket);
        //            packet = new RfId().PacketBuild();
        //            break;
        //    }
        //    break;
        //case REMOTE:
        //    switch (packetType)
        //    {
        //        case TxType.REMOTE_START:
        //            //ret = await UsbSerial.Write(port, Aptxs[0].PacketBuild());
        //            //TxQUpdate(txPacket);
        //            packet = Aptxs[0].PacketBuild();
        //            break;
        //        case TxType.REMOTE_STATUS_0:
        //            //ret = await UsbSerial.Write(port, Aptxs[0].PacketBuild());
        //            //TxQUpdate(txPacket);
        //            packet = Aptxs[0].PacketBuild();
        //            break;
        //        case TxType.REMOTE_STATUS_1:
        //            //ret = await UsbSerial.Write(port, Aptxs[1].PacketBuild());
        //            //TxQUpdate(txPacket);
        //            packet = Aptxs[1].PacketBuild();
        //            break;
        //        case TxType.REMOTE_STATUS_2:
        //            //ret = await UsbSerial.Write(port, Aptxs[2].PacketBuild());
        //            //TxQUpdate(txPacket);
        //            packet = Aptxs[2].PacketBuild();
        //            break;
        //        case TxType.REMOTE_STATUS_3:
        //            //ret = await UsbSerial.Write(port, Aptxs[3].PacketBuild());
        //            //TxQUpdate(txPacket);
        //            packet = Aptxs[3].PacketBuild();
        //            break;
        //    }
        //    break;
        //if (device == APTX1)
        //{
        //    //ret = await UsbSerial.Write(port, Encoding.UTF8.GetBytes("getid,3#"));
        //    if (Ports.ContainsKey(device))
        //    {
        //        if (Aptx.SNum == UERROR)
        //            //ret = await UsbSerial.Write(port, Encoding.UTF8.GetBytes("testread,3#"));
        //            packet = Encoding.UTF8.GetBytes("testread,3#");
        //        else if (Aptx.CurrentPulses == UERROR)
        //            //ret = await UsbSerial.Write(port, Encoding.UTF8.GetBytes("find,3#"));
        //            packet = Encoding.UTF8.GetBytes("find,3#");
        //        else if ((Aptx.aptxId[0] == UERROR) ||
        //                  (Aptx.aptxId[1] == UERROR) ||
        //                  (Aptx.aptxId[2] == UERROR))
        //        {
        //            //ret = await UsbSerial.Write(port, Encoding.UTF8.GetBytes("readid#"));
        //            packet = Encoding.UTF8.GetBytes("readid#");
        //            txPacket = TxPacket.Empty;
        //        }
        //    }
        //}
        //if(packet.Length > 0)
        //    ret = await UsbSerial.Write(port, packet);
        //if (txPacket != TxPacket.Empty)
        //{
        //    //TxQueUpdate(txPacket);
        //    SemaphoreTxQ.WaitOne();
        //    TxQue.Remove(txPacket);
        //    TxQue.Add(txPacket);
        //    SemaphoreTxQ.Release();
        //}
        //    return ret;
        //}

        //private void TxQueUpdate(TxPacket txPacket)
        //{
        //    SemaphoreTxQ.WaitOne();
        //    TxQue.Remove(txPacket);
        //    TxQue.Add(txPacket);
        //    SemaphoreTxQ.Release();
        //}
    }
}
