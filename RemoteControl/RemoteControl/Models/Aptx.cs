using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using Xamarin.Forms;

namespace RemoteControl.Models
{
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
}
