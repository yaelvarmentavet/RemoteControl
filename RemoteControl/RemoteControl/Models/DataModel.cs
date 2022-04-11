﻿using RemoteControl.Views;
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
            while (buffer.Length >= size)
            {
                buffer = buffer.SkipWhile(b => b != Sop).ToArray();
                if (buffer.Length >= size)
                {
                    if (buffer[size - 1 - 2] == Eop)
                        return true;
                    buffer = buffer.Skip(1).ToArray();
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
                    buffer = buffer.Skip(1).ToArray();
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
                    buffer = buffer.Skip(1).ToArray();
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

    public class PortEventArgs : EventArgs
    {
        public string Port;
    }

    public class StreamEventArgs : EventArgs
    {
        public Stream Stream;
    }

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
            get => (Aptx.ProcessPulses - Aptx.PulsesPrev) / Aptx.PulsesPrev;
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

        private bool twice200 = false;
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

        //public Command AddCow { get; }
        public Command TappedFL { get; }
        public Command TappedRL { get; }
        public Command TappedFR { get; }
        public Command TappedRR { get; }

        private string[] usbPorts;
        public string UsbPorts
        {
            get => usbPorts.Aggregate("", (r, v) => r += v + " "); 
            set
            {
                usbPorts = value.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UsbPorts)));
            }
        }

        enum PacketType
        {
            EMPTY,
            REMOTE_STATUS_0,
            REMOTE_STATUS_1,
            REMOTE_STATUS_2,
            REMOTE_STATUS_3,
            REMOTE_START,
            REMOTE_STOP,
            ECOMILK_ID,
            RFID_TAG,
            APTX1_ID,
            APTX1_SNUM,
            APTX1_CURRENT,
            APTX1_APTXID,
        }

        class TxPacket
        {
            public static TxPacket Empty = new TxPacket() { device = string.Empty, packetType = PacketType.EMPTY};
            public string device;
            public PacketType packetType;
            public byte[] packet;
        }
        
        public const uint UERROR = 0xFFFFFFFF;
        private const int OK = 0;
        private const int ERROR = -1;

        private const string APTX1 = "APTX1";
        private const string REMOTE = "REMOTE";
        private const string ECOMILK = "ECOMILK";
        private const string RFID = "RFID";

        //private const uint APTX_COUNT = 4;

        //private readonly uint[] STATEs; // = new uint[APTX_COUNT];
        private const int QUARTERS_NUMBER = 4;
        private const int CONNECT_TIMEOUT = 3000;
        private const int STATE_TIMEOUT = 3000; // in msec
        private const int REQUEST_TIMEOUT = 1000; // in msec
        private const int REQUEST_RETRIES = 3;

        private const int RXBUFFER_SIZE = 1024;

        private const string LOGFILE_COWS = "LOGFILE_COWS";

        public string[] Devices;

        public Aptx[] Aptxs; // = new Aptx[APTX_COUNT];
        public Aptx Aptx = new Aptx();

        private Dictionary<uint, string> Cows = new Dictionary<uint, string>();

        public event PropertyChangedEventHandler PropertyChanged;

        private IUsbSerial UsbSerial;
        
        //private ConcurrentDictionary<string, string> Ports = new ConcurrentDictionary<string, string>();
        private Dictionary<string, string> Ports = new Dictionary<string, string>();
        private Semaphore SemaphorePorts = new Semaphore(1, 1);
        
        //ManualResetEvent WaitHandleRemote = new ManualResetEvent(false);
        //ManualResetEvent WaitHandleRfid = new ManualResetEvent(false);
        //ManualResetEvent WaitHandleAptx1 = new ManualResetEvent(false);
        //System.Collections.Concurrent.ConcurrentDictionary<> conc;
        //private bool Connected = false;
        private byte PauseResume = Aptx.STOP;

        //private byte[] RxBufferRfid = new byte[1];// RXBUFFER_SIZE];
        //private byte[] RxBufferAptx1 = new byte[1];// RXBUFFER_SIZE];
        //private byte[] RxBufferEcomilk = new byte[1];// RXBUFFER_SIZE];
        //private byte[] RxBufferRemote = new byte[1];// RXBUFFER_SIZE];

        List<TxPacket> TxQue;
        private Semaphore SemaphoreTxQue = new Semaphore(1, 1);

        public DataModel(IUsbSerial usbSerial)
        {
            Aptxs = new Aptx[Aptx.APTXIDs.Length].Select((a, i) => { a = new Aptx(); a.Id = Aptx.APTXIDs[i]; return a; }).ToArray();
            if (Device.RuntimePlatform == Device.Android)
            {
                //Devices = new string[] { REMOTE, APTX1 };
                Devices = new string[] { REMOTE };
                TxQue = new List<TxPacket>() { 
                    new TxPacket() { device = REMOTE, packetType = PacketType.REMOTE_STATUS_0, packet = Aptxs[0].PacketBuild() },
                };
            }
            else if (Device.RuntimePlatform == Device.UWP)
            {
                Devices = new string[] { ECOMILK, REMOTE, RFID, APTX1 };
                TxQue = new List<TxPacket>() {
                    new TxPacket() { device = REMOTE, packetType = PacketType.REMOTE_STATUS_0, packet = Aptxs[0].PacketBuild() },
                    //TxQue = new List<TxPacket>() {
                    //    new TxPacket() { device = ECOMILK, packetType = PacketType.ECOMILK_ID, packet = Encoding.UTF8.GetBytes("ecomilkid\r")},
                    //    new TxPacket() { device = RFID, packetType = PacketType.RFID_TAG, packet = new RfId().PacketBuild()},
                    //    new TxPacket() { device = REMOTE, packetType = PacketType.REMOTE_STATUS_0, packet = Aptxs[0].PacketBuild() },
                    //    new TxPacket() { device = REMOTE, packetType = PacketType.REMOTE_STATUS_1, packet = Aptxs[1].PacketBuild() },
                    //    new TxPacket() { device = REMOTE, packetType = PacketType.REMOTE_STATUS_2, packet = Aptxs[2].PacketBuild() },
                    //    new TxPacket() { device = REMOTE, packetType = PacketType.REMOTE_STATUS_3, packet = Aptxs[3].PacketBuild() },
                    //    new TxPacket() { device = APTX1, packetType = PacketType.APTX1_ID, packet = Encoding.UTF8.GetBytes("getid,3#")},
                };
            }
            //STATEs = new uint[Aptx.APTXIDs.Length].Select((s, i) => s = (uint)i).ToArray();

            UsbSerial = usbSerial;
            UsbSerial.Event((obj, args) => 
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

            CmtRead();

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

            //UsbSerial.Event(
            //    new EventHandler((sender, args) =>
            //    {
            //        Ports = new Dictionary<string, string>();
            //        Connected = false;
            //    }),
            //    new EventHandler((sender, args) =>
            //    {
            //Connected = true;

            foreach (string dev in Devices)
                new Thread((device) => { Rx(device); })
                { Name = dev }.Start(dev);

            //if (Device.RuntimePlatform == Device.UWP)
            //    new Thread((device) => { Rx(device); })
            //    { Name = "RFID" }.Start(RFID);

            //if (Device.RuntimePlatform == Device.UWP)
            //    new Thread((device) => { Rx(device); })
            //    { Name = "APTX1" }.Start(APTX1);

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
                while (true)
                {
                    try
                    {
                        //switch(device)
                        //{
                        //    case REMOTE:
                        //        WaitHandleRemote.WaitOne();
                        //        break;
                        //    case RFID:
                        //        WaitHandleRfid.WaitOne();
                        //        break;
                        //    case APTX1:
                        //        WaitHandleAptx1.WaitOne();
                        //        break;
                        //}
                        if (Ports.TryGetValue(device, out string port))
                        {
                            await PortReply(device, port, rxBuffers);
                        }
                        else
                        {
                            string[] ports = UsbSerial.GetPorts().ToArray();
                            usbPorts = ports;
                            UsbPorts = UsbPorts;
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
            //string data = string.Empty;
            //string device = string.Empty;
            //Stopwatch stopWatch = new Stopwatch();
            //stopWatch.Start();
            //uint state = STATEs.First();
            while (true)
            {
                try
                {
                    //foreach (string dev in Devices)
                    //{
                    //    device = dev;
                    //switch (device)
                    //{
                    //    case REMOTE:
                    //        WaitHandleAptx1.Reset();
                    //        WaitHandleRfid.Reset();
                    //        WaitHandleRemote.Set();
                    //        //if (stopWatch.ElapsedMilliseconds % STATE_TIMEOUT == 0)
                    //        //    state = state == STATEs.Last() ? STATEs.First() : state++;
                    //        break;
                    //    case RFID:
                    //        WaitHandleRemote.Reset();
                    //        WaitHandleAptx1.Reset();
                    //        WaitHandleRfid.Set();
                    //        break;
                    //    case APTX1:
                    //        WaitHandleRemote.Reset();
                    //        WaitHandleRfid.Reset();
                    //        WaitHandleAptx1.Set();
                    //        break;
                    //}

                    if (TxQue.Any())
                    {
                        TxPacket txPacket = TxQue.First();
                        string device = txPacket.device;
                        int ret = -1;
                        for (int i = 0; i < REQUEST_RETRIES; i++)
                        {
                            if (Ports.TryGetValue(device, out string port))
                            {
                                //if (await PortRequest(device, port, state) < 0)
                                //if (await PortRequest(txPacket, port) < 0)
                                //{
                                //SemaphorePorts.WaitOne();
                                //Ports.Remove(device);
                                //SemaphorePorts.Release();
                                //UsbSerial.Disconnect();
                                //}
                                ret = await PortRequest(txPacket, port);
                            }
                            else
                            {
                                //UsbSerial.Connect();
                                string[] ports = UsbSerial.GetPorts().ToArray();
                                usbPorts = ports;
                                UsbPorts = UsbPorts;
                                foreach (string prt in ports)
                                {
                                    if (!Ports.Values.Contains(prt))
                                        //await PortRequest(device, prt, 0);
                                        ret = await PortRequest(txPacket, prt);
                                }
                            }
                            Thread.Sleep(REQUEST_TIMEOUT);
                        }
                        if (ret > 0)
                        {
                            SemaphoreTxQue.WaitOne();
                            TxQue.Remove(txPacket);
                            if (txPacket.packetType != PacketType.EMPTY)
                                TxQue.Add(txPacket);
                            SemaphoreTxQue.Release();
                        }
                    }
                }
                catch
                {
                    //if (Ports.Keys.Contains(device))
                    //{
                    //SemaphorePorts.WaitOne();
                    //Ports.TryRemove(device, out string val);
                    //SemaphorePorts.Release();
                    //}
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
                        //Aptx aptx = new Aptx();
                        //RxBuffer[10 + 33] = aptx.UshortToArray(aptx.ChecksumCalc(RxBuffer.Skip(10).Take(33).ToArray()))[0];
                        //RxBuffer[10 + 34] = aptx.UshortToArray(aptx.ChecksumCalc(RxBuffer.Skip(10).Take(33).ToArray()))[1];
                        if (found = Aptx.PacketParse(ref rxBuffer))
                        {
                            if (Aptx.APTXIDs.Contains((byte)Aptx.Id))
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

                                if (AutoTransition)
                                {
                                    if (Aptx.ProcessPulses >= 180)
                                    {
                                        if (twice200)
                                        {
                                            if (fr)
                                            {
                                                fr = false;
                                                FR = FR;
                                            }
                                            else
                                            {
                                                if (rr)
                                                {
                                                    rr = false;
                                                    RR = RR;
                                                }
                                                else
                                                {
                                                    if (rl)
                                                    {
                                                        rl = false;
                                                        RL = RL;
                                                    }
                                                    else
                                                    {
                                                        if (fl)
                                                        {
                                                            fl = false;
                                                            FL = FL;
                                                        }
                                                    }
                                                }
                                            }
                                            twice200 = false;
                                        }
                                        else
                                        {
                                            twice200 = true;
                                        }
                                    }
                                }
                                Aptxs[Aptx.Id - 1] = Aptx;
                            }
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
                                if (Aptx.SNum == UERROR)
                                {
                                    if (data.Contains("SNUM"))
                                    {
                                        uint[] snum = DataParse(data, "SNUM", NumberStyles.Number);
                                        Aptx.SNum = snum[0];
                                        rxBuffer = new byte[1];
                                    }
                                }
                                if ((Aptx.aptxId[0] == UERROR) ||
                                    (Aptx.aptxId[1] == UERROR) ||
                                    (Aptx.aptxId[2] == UERROR))
                                {
                                    if (data.Contains("readid Device_id"))
                                    {
                                        uint[] aptid = DataParse(data, "readid Device_id", NumberStyles.HexNumber);
                                        Aptx.aptxId[0] = aptid[0];
                                        Aptx.aptxId[1] = aptid[1];
                                        Aptx.aptxId[2] = aptid[2];
                                        Aptx.AptxId = Aptx.AptxId;
                                        rxBuffer = new byte[1];
                                    }
                                }
                                if ((Aptx.CurrentPulses == UERROR) || (Aptx.Maxi == UERROR))
                                {
                                    if (data.Contains("MAXI"))
                                    {
                                        uint[] maxi = DataParse(data, "MAXI", NumberStyles.Number);
                                        Aptx.Maxi = maxi[0];
                                        rxBuffer = new byte[1];
                                    }
                                    if (data.Contains("Found:") || data.Contains("pulses written"))
                                    {
                                        uint[] current = DataParse(data, "Found:", NumberStyles.Number);
                                        if (current[0] == 0)
                                            current = DataParse(data, "pulses written", NumberStyles.Number);
                                        Aptx.CurrentPulses = current[0];
                                        rxBuffer = new byte[1];
                                    }
                                    if ((Aptx.Maxi != UERROR) && (Aptx.CurrentPulses != UERROR))
                                        Aptx.Remaining = Aptx.Maxi - Aptx.CurrentPulses;
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

        //private async Task<int> PortRequest(string device, string port, uint state)
        private async Task<int> PortRequest(TxPacket txPacket, string port)
        {
            int ret = ERROR;
            switch (txPacket.device)
            {
                case REMOTE:
                    switch (txPacket.packetType)
                    {
                        case PacketType.REMOTE_START:
                        case PacketType.REMOTE_STOP:
                            txPacket.packetType = PacketType.EMPTY;
                            break;
                    }
                    break;
                case APTX1:
                    switch (txPacket.packetType)
                    {
                        case PacketType.APTX1_ID:
                            if (Ports.ContainsKey(txPacket.device))
                            {
                                txPacket.packetType = PacketType.APTX1_SNUM;
                                txPacket.packet = Encoding.UTF8.GetBytes("testread,3#");
                            }
                            break;
                        case PacketType.APTX1_SNUM:
                            if (Aptx.SNum != UERROR)
                            {
                                txPacket.packetType = PacketType.APTX1_CURRENT;
                                txPacket.packet = Encoding.UTF8.GetBytes("find,3#");
                            }
                            break;
                        case PacketType.APTX1_CURRENT:
                            if (Aptx.CurrentPulses != UERROR)
                            {
                                txPacket.packetType = PacketType.APTX1_APTXID;
                                txPacket.packet = Encoding.UTF8.GetBytes("readid#");
                            }
                            break;
                        case PacketType.APTX1_APTXID:
                            if ((Aptx.aptxId[0] != UERROR) ||
                                (Aptx.aptxId[1] != UERROR) ||
                                (Aptx.aptxId[2] != UERROR))
                            {
                                txPacket.packetType = PacketType.EMPTY;
                            }
                            break;
                    }
                    break;
            }
            ret = await UsbSerial.Write(port, txPacket.packet);

            return ret;
        }
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

        private uint[] DataParse(string data, string pattern, NumberStyles numberStyles)
        {
            uint[] num = new uint[6];
            string snum = data.Contains(pattern) ? 
                new string(data.Substring(data.IndexOf(pattern) + pattern.Length)?.TakeWhile(c => ((c != '\r') && (c != 'p')))?.ToArray()) 
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

        public void CmtSave()
        {
            if (CowId != UERROR)
            {
                string LOGFILE_COWS = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "LogFileCows.txt");
                File.AppendAllText(LOGFILE_COWS, string.Format("CowId: {0} CmtFL: {1} CmtRL: {2} CmtFR: {3} CmtRR: {4} Date: {5}\n",
                    CowId, CmtFL, CmtRL, CmtFR, CmtRR, DateTime.Now));
            }
        }

        public void CmtRead()
        {
            string LOGFILE_COWS = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "LogFileCows.txt");
            string data = File.ReadAllText(LOGFILE_COWS);
            if (data.Contains("CowId"))
            {
                string line = data.Split(new char[] { '\n' }).Where(l => l.Contains("CowId")).Last();
                uint[] id = DataParse(line, "CowId", NumberStyles.Number);
                CmtFL = CmtParse(line, "CmtFL: ");
                CmtRL = CmtParse(line, "CmtRL: ");
                CmtFR = CmtParse(line, "CmtFR: ");
                CmtRR = CmtParse(line, "CmtRR: ");
            }
        }

        private string CmtParse(string line, string pattern)
        {
            return new string(line.Substring(line.IndexOf(pattern) + pattern.Length).Take(1).ToArray());
        }

        public async Task<int> ProcessStart()
        {
            foreach (Aptx aptx in Aptxs)
            {
                //if (await Process(aptx, Aptx.START) == ERROR)
                //return ERROR;
                await Process(aptx, Aptx.START);
            }
            return OK;
        }

        public async Task<int> ProcessStop()
        {
            foreach (Aptx aptx in Aptxs)
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
            foreach (Aptx aptx in Aptxs)
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
            
            SemaphoreTxQue.WaitOne();
            PacketType packetType = process == Aptx.START ? PacketType.REMOTE_START : PacketType.REMOTE_STOP;
            if (!TxQue.Where(t => t.packetType == packetType).Any())
                TxQue.Add(new TxPacket()
                {
                    device = REMOTE,
                    packetType = packetType,
                    packet = aptx.PacketBuild(process, pulses)
                });
            SemaphoreTxQue.Release();

            //string LOGFILE_COWS = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "LogFileCows.txt");
            //File.AppendAllText(LOGFILE_COWS, string.Format("Date: {0} Process: {1} Pulses: {2} Cow Id: {3} Current Pulses: {4}\n",
            //    //DateTime.Now, response == OK ? "OK" : "FAULT", process == Aptx.START ? "START" : "STOP",
            //    DateTime.Now, process == Aptx.START ? "START" : "STOP",
            //    pulses, CowId, aptx.ProcessPulses));
            //return response;
        }

        public async Task RCWStart()
        {
            await UsbSerial.Write(Ports.TryGetValue(ECOMILK, out var val) ? val : string.Empty, Encoding.UTF8.GetBytes("rcw 1\r"));
        }

        public async Task RCWStop()
        {
            await UsbSerial.Write(Ports.TryGetValue(ECOMILK, out var val) ? val : string.Empty, Encoding.UTF8.GetBytes("rcw 0\r"));
        }

        public async Task RCCWStart()
        {
            await UsbSerial.Write(Ports.TryGetValue(ECOMILK, out var val) ? val : string.Empty, Encoding.UTF8.GetBytes("rccw 1\r"));
        }

        public async Task RCCWStop()
        {
            await UsbSerial.Write(Ports.TryGetValue(ECOMILK, out var val) ? val : string.Empty, Encoding.UTF8.GetBytes("rccw 0\r"));
        }




        public async Task AFStart()
        {
            await UsbSerial.Write(Ports.TryGetValue(ECOMILK, out var val) ? val : string.Empty, Encoding.UTF8.GetBytes("af 1\r"));
        }

        public async Task AFStop()
        {
            await UsbSerial.Write(Ports.TryGetValue(ECOMILK, out var val) ? val : string.Empty, Encoding.UTF8.GetBytes("af 0\r"));
        }

        public async Task ABStart()
        {
            await UsbSerial.Write(Ports.TryGetValue(ECOMILK, out var val) ? val : string.Empty, Encoding.UTF8.GetBytes("ab 1\r"));
        }

        public async Task ABStop()
        {
            await UsbSerial.Write(Ports.TryGetValue(ECOMILK, out var val) ? val : string.Empty, Encoding.UTF8.GetBytes("ab 0\r"));
        }




        public async Task MZUStart()
        {
            await UsbSerial.Write(Ports.TryGetValue(ECOMILK, out var val) ? val : string.Empty, Encoding.UTF8.GetBytes("mzu 1\r"));
        }

        public async Task MZUStop()
        {
            await UsbSerial.Write(Ports.TryGetValue(ECOMILK, out var val) ? val : string.Empty, Encoding.UTF8.GetBytes("mzu 0\r"));
        }

        public async Task MZDStart()
        {
            await UsbSerial.Write(Ports.TryGetValue(ECOMILK, out var val) ? val : string.Empty, Encoding.UTF8.GetBytes("mzd 1\r"));
        }

        public async Task MZDStop()
        {
            await UsbSerial.Write(Ports.TryGetValue(ECOMILK, out var val) ? val : string.Empty, Encoding.UTF8.GetBytes("mzd 0\r"));
        }

        public async Task TCWStart()
        {
            await UsbSerial.Write(Ports.TryGetValue(ECOMILK, out var val) ? val : string.Empty, Encoding.UTF8.GetBytes("tcw 1\r"));
        }

        public async Task XFStart()
        {
            await UsbSerial.Write(Ports.TryGetValue(ECOMILK, out var val) ? val : string.Empty, Encoding.UTF8.GetBytes("xf 1\r"));
        }
    }
}
