using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Xamarin.Forms;

namespace RemoteControl.Models
{
    public class Aptx : Packet, INotifyPropertyChanged
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        unsafe struct PacketStatus
        {
            [MarshalAs(UnmanagedType.U2)]
            //public byte STX;
            //public byte APT_SERIAL_NUMBER;
            //public uint AM_number;
            ////public byte AM_number_2;
            ////public byte AM_number_3;
            ////public byte AM_number_lsb;
            //public uint Max_number;
            ////public byte Max_number_2;
            ////public byte Max_number_3;
            ////public byte Max_number_lsb;
            //public uint Current_number;
            ////public byte Current_number_2;
            ////public byte Current_number_3;
            ////public byte Current_number_lsb;
            //public uint Apt_number;
            ////public byte Apt_number_2;
            ////public byte Apt_number_3;
            ////public byte Apt_number_lsb;
            //public uint Apt_pulses_current;
            //public byte Pressure_flag;
            //public byte Battery_flag;
            //public byte motor_is_running;
            //public byte Apt_pulses_flag;
            //public byte errors;
            //public byte motor_temperature;
            //public byte motor_voltage;
            //public byte speed_of_bullet;
            //public ushort Cow_id;
            ////public byte Cow_id_lsb;
            //public uint Sum_pulses;
            ////public byte Sum_pulses_2;
            ////public byte Sum_pulses_3;
            ////public byte Sum_pulses_lsb;
            //public byte ETX;
            //public ushort Check_sum;
            ////public byte Check_sum_lsb;

            public byte stx;
            public byte id;
            public uint snum;
            //public byte AM_number_2;
            //public byte AM_number_3;
            //public byte AM_number_lsb;
            public uint maxi;
            //public byte Max_number_2;
            //public byte Max_number_3;
            //public byte Max_number_lsb;
            //public uint Current_number;
            public uint pulse;
            //public byte Current_number_2;
            //public byte Current_number_3;
            //public byte Current_number_lsb;
            public uint apt_id;
            //public byte Apt_number_2;
            //public byte Apt_number_3;
            //public byte Apt_number_lsb;
            public uint apt_pulse;
            public byte pressure;
            public byte battery;
            public byte operation;
            //public byte Apt_pulses_flag;
            //public byte operation;
            public byte temperature;
            public byte voltage;
            public byte speed;
            //public ushort Cow_id;
            //public byte Cow_id_lsb;
            //public uint pulse;
            //public byte Sum_pulses_2;
            //public byte Sum_pulses_3;
            //public byte Sum_pulses_lsb;
            public ushort error;
            public byte etx;
            public ushort checksum;
            //public byte Check_sum_lsb;
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
                RemainingOK = remaining > Maxi * 0.1 ? true : false;
                RemainingLow = remaining <= Maxi * 0.1 ? true : false;
            }
        }

        //private uint processPulses = UERROR;
        //public uint ProcessPulses
        //{
        //    get => processPulses;
        //    set
        //    {
        //        processPulses = value;
        //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ProcessPulses)));
        //    }
        //}

        private uint operationPulse = UERROR;
        public uint OperationPulse
        {
            get => operationPulse;
            set
            {
                operationPulse = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(OperationPulse)));
            }
        }

        private uint[] aptxid = new uint[3] { UERROR, UERROR, UERROR };
        public uint AptxId
        {
            get => aptxid[0];
            set
            {
                aptxid[0] = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AptxId)));
            }
        }
        //public uint[] aptxId
        //{
        //    get => aptxid;
        //    set
        //    {
        //        aptxid = value;
        //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(aptxId)));
        //    }
        //}
        //
        ////public string AptxId
        //public uint AptxId
        //{
        //    //get => aptxid.Aggregate("", (r, m) => r += m.ToString("X") + "   ");
        //    //get => aptxid[0].ToString();
        //    get => aptxid[0];
        //    set
        //    {
        //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AptxId)));
        //    }
        //}

        //private uint aptPulsesCurrent = UERROR;
        //public uint AptPulsesCurrent
        //{
        //    get => aptPulsesCurrent;
        //    set
        //    {
        //        aptPulsesCurrent = value;
        //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AptPulsesCurrent)));
        //    }
        //}

        private uint aptPulse = UERROR;
        public uint AptPulse
        {
            get => aptPulse;
            set
            {
                aptPulse = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AptPulse)));
            }
        }

        private uint aptRemaining = UERROR;
        public uint AptRemaining
        {
            get => aptRemaining;
            set
            {
                aptRemaining = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AptRemaining)));
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
                PressureOK = pressure == 1 ? true : false;
                PressureLow = pressure == 0 ? true : false;
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
                //BatteryOK = battery >= BATTERY_LIMIT ? true : false;
                //BatteryLow = battery < BATTERY_LIMIT ? true : false;
                Battery100Per = false;
                Battery75Per = false;
                Battery50Per = false;
                Battery25Per = false;
                Battery15Per = false;
                if ((battery > 75) && (battery <= 100))
                    Battery100Per = true;
                if ((battery > 50) && (battery <= 75))
                    Battery75Per = true;
                if ((battery > 25) && (battery <= 50))
                    Battery50Per = true;
                if ((battery > 15) && (battery <= 25))
                    Battery25Per = true;
                if (battery <= 15)
                    Battery15Per = true;
            }
        }

        //private uint motorisrunning = UERROR;
        //public uint MotorIsRunning
        //{
        //    get => motorisrunning;
        //    set
        //    {
        //        motorisrunning = value;
        //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MotorIsRunning)));
        //    }
        //}

        //private uint motor = UERROR;
        //public uint Motor
        //{
        //    get => motor;
        //    set
        //    {
        //        motor = value;
        //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Motor)));
        //    }
        //}

        private uint operation = UERROR;
        public uint Operation
        {
            get => operation;
            set
            {
                operation = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Operation)));
                OperationRun = operation == 1 ? true : false;
                OperationStop = operation == 0 ? true : false;
            }
        }

        //private uint aptpulses = UERROR;
        //public uint AptPulses
        //{
        //    get => aptpulses;
        //    set
        //    {
        //        aptpulses = value;
        //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AptPulses)));
        //        AptPulsesOK = aptpulses == 1 ? true : false;
        //        AptPulsesLow = aptpulses == 0 ? true : false;
        //    }
        //}

        //private uint motortemperature = UERROR;
        //public uint MotorTemperature
        //{
        //    get => motortemperature;
        //    set
        //    {
        //        motortemperature = value;
        //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MotorTemperature)));
        //    }
        //}

        private uint temperature = UERROR;
        public uint Temperature
        {
            get => temperature;
            set
            {
                temperature = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Temperature)));
            }
        }

        //private uint motorvoltage = UERROR;
        //public uint MotorVoltage
        //{
        //    get => motorvoltage;
        //    set
        //    {
        //        motorvoltage = value;
        //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MotorVoltage)));
        //    }
        //}

        private uint voltage = UERROR;
        public uint Voltage
        {
            get => voltage;
            set
            {
                voltage = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Voltage)));
            }
        }

        //private uint speedofbullet = UERROR;
        //public uint SpeedOfBullet
        //{
        //    get => speedofbullet;
        //    set
        //    {
        //        speedofbullet = value;
        //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SpeedOfBullet)));
        //    }
        //}
        
        private uint speed = UERROR;
        public uint Speed
        {
            get => speed;
            set
            {
                speed = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Speed)));
            }
        }

        private uint error = UERROR;
        public uint Error
        {
            get => error;
            set
            {
                error = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Error)));
                Errors = string.Empty;
                if ((error & A4000) != 0)
                    Errors += "A4000 Contact Service\n";
                if ((error & E6999) != 0)
                    Errors += "E6999 Contact Service Replace AM\n";
                if ((error & A7000) != 0)
                    Errors += "7000 Pulses Remaining Contact Service Press Reset\n";
                if ((error & A1000) != 0)
                    Errors += "1000 Pulses Remaining Contact Service Press Reset\n";
                if ((error & E0) != 0)
                    Errors += "0 Pulses Remaining Contact Service Replace AM\n";
                if ((error & A504) != 0)
                    Errors += "Maintenance Required Contact Service\n";
                if ((error & E503) != 0)
                    Errors += "Maintenance is due, Efficiency and safety comprimised Contact Service\n";
                if ((error & ABattery25) != 0)
                    Errors += "Please charge Press Reset\n";
                if ((error & ABattery20) != 0)
                    Errors += "Please charge Press Reset\n";
                if ((error & EBattery15) != 0)
                    Errors += "Please charge Press Reset\n";
                if ((error & EBattery0) != 0)
                    Errors += "Battery Depleted Charge to continue\n";
                if ((error & E200) != 0)
                    Errors += "Low ambient temperature\n";
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

        //private uint currentPulses = UERROR;
        //public uint CurrentPulses
        //{
        //    get => currentPulses;
        //    set
        //    {
        //        currentPulses = value;
        //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentPulses)));
        //    }
        //}

        private uint pulse = UERROR;
        public uint Pulse
        {
            get => pulse;
            set
            {
                pulse = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Pulse)));
            }
        }

        private bool pressureOK = false;
        public bool PressureOK
        {
            //get => Pressure == 1;
            get => pressureOK;
            set
            {
                pressureOK = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PressureOK)));
            }
        }

        private bool pressureLow = true;
        public bool PressureLow
        {
            //get => Pressure != 1;
            get => pressureLow;
            set
            {
                pressureLow = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PressureLow)));
            }
        }

        private bool remainingOK = false;
        public bool RemainingOK
        {
            //get => Remaining > Maxi * 0.1;
            get => remainingOK;
            set
            {
                remainingOK = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RemainingOK)));
            }
        }

        private bool remainingLow = true;
        public bool RemainingLow
        {
            //get => Remaining <= Maxi * 0.1;
            get => remainingLow;
            set
            {
                remainingLow = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RemainingLow)));
            }
        }

        //private bool batteryOK = false;
        //public bool BatteryOK
        //{
        //    //get => Battery == 1;
        //    get => batteryOK;
        //    set
        //    {
        //        batteryOK = value;
        //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BatteryOK)));
        //    }
        //}
        //
        //private bool batteryLow = true;
        //public bool BatteryLow
        //{
        //    //get => Battery != 1;
        //    get => batteryLow;
        //    set
        //    {
        //        batteryLow = value;
        //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BatteryLow)));
        //    }
        //}

        private bool battery100Per = false;
        public bool Battery100Per
        {
            //get => Battery == 1;
            get => battery100Per;
            set
            {
                battery100Per = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Battery100Per)));
            }
        }

        private bool battery75Per = false;
        public bool Battery75Per
        {
            //get => Battery == 1;
            get => battery75Per;
            set
            {
                battery75Per = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Battery75Per)));
            }
        }

        private bool battery50Per = false;
        public bool Battery50Per
        {
            //get => Battery == 1;
            get => battery50Per;
            set
            {
                battery50Per = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Battery50Per)));
            }
        }

        private bool battery25Per = false;
        public bool Battery25Per
        {
            //get => Battery == 1;
            get => battery25Per;
            set
            {
                battery25Per = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Battery25Per)));
            }
        }

        private bool battery15Per = true;
        public bool Battery15Per
        {
            //get => Battery == 1;
            get => battery15Per;
            set
            {
                battery15Per = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Battery15Per)));
            }
        }
 
        //private bool aptPulsesOK = false;
        //public bool AptPulsesOK
        //{
        //    //get => AptPulses == 1;
        //    get => aptPulsesOK;
        //    set
        //    {
        //        aptPulsesOK = value;
        //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AptPulsesOK)));
        //    }
        //}
        //private bool aptPulsesLow = true;
        //public bool AptPulsesLow
        //{
        //    //get => AptPulses != 1;
        //    get => aptPulsesLow;
        //    set
        //    {
        //        aptPulsesLow = value;
        //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AptPulsesLow)));
        //    }
        //}

private bool operationRun = false;
        public bool OperationRun
        {
            //get => AptPulses == 1;
            get => operationRun;
            set
            {
                operationRun = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(OperationRun)));
            }
        }

        private bool operationStop = true;
        public bool OperationStop
        {
            //get => AptPulses != 1;
            get => operationStop;
            set
            {
                operationStop = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(OperationStop)));
            }
        }

        private string errors = string.Empty;
        public string Errors
        {
            get => errors;
            set
            {
                errors = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Errors)));
            }
        }

        public string StatusMessage
        {
            get
            {
                string sts = "Quarter " + (Id + 1) + "\n" + "\n";
                //if (Pressure == 1)
                //    sts += "Pressure OK\n";
                //else
                //    sts += "Pressure fault\n";
                sts += "Pressure " + Pressure + "\n";
                if (Battery == 1)
                    sts += "Battery OK\n";
                else
                    sts += "Battery fault\n";
                //if (MotorTemperature == 1)
                //    sts += "Motor temperature OK\n";
                //else
                //    sts += "Motor temperature fault\n";
                //if (MotorVoltage == 1)
                //    sts += "Motor voltage OK\n";
                //else
                //    sts += "Motor voltage fault\n";
                //if (SpeedOfBullet == 1)
                //    sts += "Speed of bullet OK\n";
                //else
                //    sts += "Speed of bullet fault\n";
                return sts;
            }
            set
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StatusMessage)));
            }
        }

        public Color StatusColor
        {
            //get => ((Pressure == 1) && (Battery == 1) && (MotorTemperature == 1) &&
            //        (MotorVoltage == 1) && (SpeedOfBullet == 1)) ? Color.LightGreen : Color.Red;
            get => ((Battery == 1) && (Pressure >= 24)) ? Color.LimeGreen : Color.Red;
            set
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StatusColor)));
            }
        }

        //public float progress = UERROR;
        public float Progress
        {
            //get => (ProcessPulses - PulsesPrev) / ECOMILK_PROCESS_PULSES;
            //get => (float)ProcessPulses / (float)ECOMILK_PROCESS_PULSES;
            get => (float)OperationPulse / (float)ECOMILK_PROCESS_PULSES;
            set
            {
                //progress = value;
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
        public const byte RESUME = 0x03;
        public const byte RESERVED = 0x00;

        public const byte BATTERY_LIMIT = 15;

        public const int ECOMILK_PROCESS_PULSES = 100;

        public static readonly byte[] APTXIDs = new byte[] { 0x00, 0x01, 0x02, 0x03 };

        //public uint PulsesPrev = UERROR;

        public event PropertyChangedEventHandler PropertyChanged;

        public Aptx()
        {
            Sop = STX;
            Eop = ETX;

            Dcheck = ChecksumCalc;
            Dassign = PacketAssign;
        }

        const uint A4000 = 1;
        const uint E4001 = 2;
        const uint E6999 = 4;
        const uint A7000 = 8;
        const uint A1000 = 16;
        const uint E0 = 32;
        const uint A504 = 64;
        const uint E503 = 128;
        const uint ABattery25 = 256;
        const uint ABattery20 = 512;
        const uint EBattery15 = 1024;
        const uint EBattery0 = 2048;
        const uint E200 = 4096;

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

                        //Id = packetStatus->APT_SERIAL_NUMBER;
                        //SNum = ArrayToUint((byte*)&packetStatus->AM_number);
                        //Maxi = ArrayToUint((byte*)&packetStatus->Max_number);
                        //ProcessPulses = ArrayToUint((byte*)&packetStatus->Current_number);
                        ////aptxId[0] = ArrayToUint((byte*)&packetStatus->Apt_number_msb);
                        //AptxId = ArrayToUint((byte*)&packetStatus->Apt_number);
                        //AptPulsesCurrent = ArrayToUint((byte*)&packetStatus->Apt_pulses_current);
                        //AptRemaining = 1000000 - AptPulsesCurrent;
                        //Pressure = packetStatus->Pressure_flag;
                        //Battery = packetStatus->Battery_flag;
                        //MotorIsRunning = packetStatus->motor_is_running;
                        //AptPulses = packetStatus->Apt_pulses_flag;
                        //MotorTemperature = packetStatus->motor_temperature;
                        //MotorVoltage = packetStatus->motor_voltage;
                        //SpeedOfBullet = packetStatus->speed_of_bullet;
                        //CowId = ArrayToUshort((byte*)&packetStatus->Cow_id);
                        //CurrentPulses = ArrayToUint((byte*)&packetStatus->Sum_pulses);
                        //Remaining = Maxi - CurrentPulses;

                        Id = packetStatus->id;
                        SNum = ArrayToUint((byte*)&packetStatus->snum);
                        Maxi = ArrayToUint((byte*)&packetStatus->maxi);
                        Pulse = ArrayToUint((byte*)&packetStatus->pulse);
                        Remaining = Maxi - Pulse;
                        //ProcessPulses = ArrayToUint((byte*)&packetStatus->Current_number);
                        //aptxId[0] = ArrayToUint((byte*)&packetStatus->Apt_number_msb);
                        AptxId = ArrayToUint((byte*)&packetStatus->apt_id);
                        AptPulse = ArrayToUint((byte*)&packetStatus->apt_pulse);
                        AptRemaining = 1000000 - AptPulse;
                        Pressure = packetStatus->pressure;
                        Battery = packetStatus->battery;
                        Operation = packetStatus->operation;
                        //AptPulses = packetStatus->Apt_pulses_flag;
                        Temperature = packetStatus->temperature;
                        Voltage = packetStatus->voltage;
                        Speed = packetStatus->speed;
                        Error = ArrayToUshort((byte*)&packetStatus->error);

                        //CowId = ArrayToUshort((byte*)&packetStatus->Cow_id);
                        //CurrentPulses = ArrayToUint((byte*)&packetStatus->Sum_pulses);
                        //Pulse = ArrayToUint((byte*)&packetStatus->Sum_pulses);
                        //Remaining = Maxi - Pulse;
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
