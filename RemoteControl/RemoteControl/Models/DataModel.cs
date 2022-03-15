using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace RemoteControl.Models
{
    public struct Aptx2Rx
    {
        byte STX;
        byte APT_SERIAL_NUMBER;
        byte AM_number_msb;
        byte AM_number_2;
        byte AM_number_3;
        byte AM_number_lsb;
        byte Max_number_msb;
        byte Max_number_2;
        byte Max_number_3;
        byte Max_number_lsb;
        byte Current_number_msb;
        byte Current_number_2;
        byte Current_number_3;
        byte Current_number_lsb;
        byte Apt_number_msb;
        byte Apt_number_2;
        byte Apt_number_3;
        byte Apt_number_lsb;
        byte Pressure_flag;
        byte Battery_flag;
        byte motor_is_running;
        byte Apt_pulses_flag;
        byte errors;
        byte motor_temperature;
        byte motor_voltage;
        byte speed_of_bullet;
        byte Cow_id_msb;
        byte Cow_id_lsb;
        byte reserved;
        byte reserved1;
        byte reserved2;
        byte reserved3;
        byte ETX;
        byte Check_sum_msb;
        byte Check_sum_lsb;
    }

    public class DataModel : INotifyPropertyChanged
    {
        private const uint UERROR = 0xffffffff;
        private const int OK = 0;
        private const int ERROR = -1;
        private const long TIMEOUT = 3000;

        private const string APTX1 = "APTX1";
        private const string REMOTE = "REMOTE";
        private const string ECOMILK = "ECOMILK";
        private const byte STX = 0xBB;
        private const byte ETX = 0x7E;
        private const byte APTXID1 = 0x01;
        private const byte APTXID2 = 0x02;
        private const byte APTXID3 = 0x03;
        private const byte APTXID4 = 0x04;
        private const byte COUNTDOWN = 0x01;
        private const byte COUNTUP = 0x02;
        private readonly byte[] PULSES100 = new byte[] { 0x00, 0x64}; // 100
        private readonly byte[] PULSES400 = new byte[] { 0x01, 0x90 }; // 400
        private const byte START = 0x01;
        private const byte STOP = 0x02;
        private const byte RESERVED = 0x00;
        private readonly byte[] APTXIDs = new byte[] { APTXID1, APTXID2, APTXID3, APTXID4 };

        private string cowid = string.Empty;
        public string CowId
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

        public Dictionary<string, string> Cows = new Dictionary<string, string>();

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

        public DataModel(IUsbSerial usbSerial)
        {
            UsbSerial = usbSerial;

            AddCow = new Command(() =>
            {
                Cows.Add(CowId, TagId);
                CowId = string.Empty;
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
                        Stopwatch stopwatch = new Stopwatch();
                        stopwatch.Start();
                        while (!Ports.TryGetValue(device, out string prt) || (stopwatch.ElapsedMilliseconds > TIMEOUT))
                        {
                            foreach (string port in UsbSerial.GetPorts())
                            {
                                await PortPingRequest(device, port);

                                //byte[] buffer = new byte[1024];
                                //data += await UsbSerial.Read(device, buffer);
                                //await UsbSerial.Read(port, buffer);
                                if (await PortPingReply(device, port))
                                    Ports.Add(device, port);
                            }
                        }
                        //data = string.Empty;
                        while (Ports.TryGetValue(device, out string port))
                        {
                            PortDataRequest(device);

                            //byte[] buffer = new byte[1024];
                            //data += await UsbSerial.Read(port, buffer);
                            //if (await UsbSerial.Read(port, buffer) > 0)
                            //{
                            if (!await PortDataReply(device, port))
                                Ports.Remove(REMOTE);
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
                        break;
                    case APTX1:
                        string data = Encoding.UTF8.GetString(buffer);
                        if (SNum == UERROR)
                        {
                            if (data.Contains("SNUM"))
                            {
                                uint[] snum = DataParse(data, "SNUM", NumberStyles.Number);
                                SNum = snum[0];
                            }
                        }
                        if ((aptxId[0] == UERROR) ||
                            (aptxId[1] == UERROR) ||
                            (aptxId[2] == UERROR))
                        {
                            if (data.Contains("readid Device_id"))
                            {
                                uint[] aptid = DataParse(data, "readid Device_id", NumberStyles.HexNumber);
                                aptxId[0] = aptid[0];
                                aptxId[1] = aptid[1];
                                aptxId[2] = aptid[2];
                                AptxId = AptxId;
                            }
                        }
                        if ((Current == UERROR) || (Maxi == UERROR))
                        {
                            if (data.Contains("MAXI"))
                            {
                                uint[] maxi = DataParse(data, "MAXI", NumberStyles.Number);
                                Maxi = maxi[0];
                            }
                            if (data.Contains("Found:"))
                            {
                                uint[] current = DataParse(data, "Found:", NumberStyles.Number);
                                Current = current[0];
                            }
                            if ((Current != UERROR) && (Maxi != UERROR))
                                Remaining = Maxi - Current;
                        }
                        break;
                    default:
                        return false;
                }
                return true;
            }
            return false;
        }

        private async Task PortDataRequest(string device)
        {
            switch (device)
            {
                case ECOMILK:
                    break;
                case REMOTE:
                    break;
                case APTX1:
                    if (SNum == UERROR)
                        await UsbSerial.Write(Ports.TryGetValue(device, out var val) ? val : string.Empty, Encoding.UTF8.GetBytes("testread,3#"));
                    else if (Current == UERROR)
                        await UsbSerial.Write(Ports.TryGetValue(device, out var val) ? val : string.Empty, Encoding.UTF8.GetBytes("find,3#"));
                    else if ((aptxId[0] == UERROR) ||
                              (aptxId[1] == UERROR) ||
                              (aptxId[2] == UERROR))
                        await UsbSerial.Write(Ports.TryGetValue(device, out var val) ? val : string.Empty, Encoding.UTF8.GetBytes("readid#"));
                    break;
            }
        }

        private async Task PortPingRequest(string device, string port)
        {
            switch (device)
            {
                case ECOMILK:
                case APTX1:
                    await UsbSerial.Write(port, Encoding.UTF8.GetBytes("getid,3#"));
                    break;
                case REMOTE:
                    break;
            }
        }

        private async Task<bool> PortPingReply(string device, string port)
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
                            found = buffer.Contains(STX);
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
            foreach (byte aptxid in APTXIDs)
            {
                if (await Command(aptxid, COUNTDOWN, PULSES100, START) == ERROR)
                    return ERROR;
            }
            return OK;
        }

        public async Task<int> StopProcess()
        {
            foreach (byte aptxid in APTXIDs)
            {
                if (await Command(aptxid, COUNTDOWN, PULSES100, STOP) == ERROR)
                    return ERROR;
            }
            return OK;
        }

        public async Task<int> PauseResumeProcess()
        {
            foreach (byte aptxid in APTXIDs)
            {
                if (await Command(aptxid, COUNTDOWN, PULSES100, STOP) == ERROR)
                    return ERROR;
            }
            return OK;
        }

        private async Task<int> Command(byte aptxid, byte count, byte[] pulses, byte action)
        {
            return await UsbSerial.Write(Ports.TryGetValue(REMOTE, out string val) ? val : string.Empty,
                 Checksum(new byte[] { STX, aptxid,
                    count, pulses[0], pulses[1], action,
                    RESERVED, RESERVED, RESERVED,
                    ETX}).ToArray());
        }

        private byte[] Checksum(byte[] buffer)
        {
            UInt16 sum = 0;
            foreach(byte b in buffer)
                sum += b;
            return buffer.Concat(new byte[] {(byte)(sum & 0xFF00 >> 8), (byte)(sum & 0x00FF) }).ToArray();
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
