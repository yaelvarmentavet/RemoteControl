using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RemoteControl.Models
{
    public class DataModel : INotifyPropertyChanged
    {
        public const uint Error = 0xffffffff;

        uint cowid = Error;
        public uint CowId
        {
            get => cowid;
            set
            {
                cowid = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CowId)));
            }
        }

        uint snum = Error;
        public uint SNum
        {
            get => snum;
            set
            {
                snum = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SNum)));
            }
        }

        uint current = Error;
        public uint Current { get => current; set => current = value; }

        uint maxi = Error;
        public uint Maxi { get => maxi; set => maxi = value; }
        
        uint remaining = Error;
        public uint Remaining
        {
            get => remaining;
            set
            {
                remaining = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Remaining)));
            }
        }

        uint[] aptxid = new uint[3] { Error, Error, Error };

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

        public event PropertyChangedEventHandler PropertyChanged;

        //public DataModel() { }

        public IUsbInterface UsbDevice;
        public string APTXPort = string.Empty;
        public string EcomilkPort = string.Empty;
        bool UsbConnected = false;

        public DataModel(IUsbInterface usbDevice)
        {
            UsbDevice = usbDevice;

            UsbDevice.Event(
                new EventHandler((sender, args) =>
                {
                    APTXPort = string.Empty;
                    EcomilkPort = string.Empty;
                    UsbConnected = false;
                }),
                new EventHandler((sender, args) =>
                {
                    UsbConnected = true;
                    new Thread(async () =>
                    {
                        string data = string.Empty;
                        while (UsbConnected)
                        {
                            while (!(await UsbDevice.Connect())) ;

                            foreach (string port in UsbDevice.GetPorts())
                            {
                                while (string.IsNullOrEmpty(APTXPort))
                                {
                                    byte[] buffer = new byte[1024];
                                    try { data += await UsbDevice.Read(port, buffer); } catch { }
                                    string data1 = Encoding.UTF8.GetString(buffer);

                                    //if (string.IsNullOrEmpty(APTXPort) || string.IsNullOrEmpty(EcomilkPort))
                                    if (data.Contains("1F-85-01"))
                                    {
                                        APTXPort = port;
                                    }
                                    if (data.Contains("ECOMILK"))
                                    {
                                        EcomilkPort = port;
                                    }
                                }
                            }

                            data = string.Empty;
                            while (UsbConnected)
                            {
                                byte[] buffer = new byte[1024];
                                try { data += await UsbDevice.Read(APTXPort, buffer); } catch { }
                                string data1 = Encoding.UTF8.GetString(buffer);
                                if (SNum == Error)
                                {
                                    //if (data.Contains("str,serial_num:"))
                                    //{
                                    //    uint snum = DataParse(data, "str,serial_num:", NumberStyles.Number);
                                    //    SNum = snum;
                                    //}
                                    if (data.Contains("SNUM"))
                                    {
                                        uint[] snum = DataParse(data, "SNUM", NumberStyles.Number);
                                        SNum = snum[0];
                                    }
                                }
                                if ((aptxId[0] == Error) ||
                                    (aptxId[1] == Error) ||
                                    (aptxId[2] == Error))
                                {
                                    //if (data.Contains("CS 3 ADR: 1000 W:"))
                                    //{
                                    //    uint aptid = DataParse(data, "CS 3 ADR: 1000 W:", NumberStyles.HexNumber);
                                    //    aptId[0] = aptid;
                                    //    aptId = aptId;
                                    //    AptId = AptId;
                                    //}
                                    //if (data.Contains("CS 3 ADR: 1004 W:"))
                                    //{
                                    //    uint aptid = DataParse(data, "CS 3 ADR: 1004 W:", NumberStyles.HexNumber);
                                    //    aptId[1] = aptid;
                                    //    aptId = aptId;
                                    //    AptId = AptId;
                                    //}
                                    //if (data.Contains("CS 3 ADR: 1008 W:"))
                                    //{
                                    //    uint aptid = DataParse(data, "CS 3 ADR: 1008 W:", NumberStyles.HexNumber);
                                    //    aptId[2] = aptid;
                                    //    aptId = aptId;
                                    //    AptId = AptId;
                                    //}
                                    if (data.Contains("readid Device_id"))
                                    {
                                        uint[] aptid = DataParse(data, "readid Device_id", NumberStyles.HexNumber);
                                        aptxId[0] = aptid[0];
                                        aptxId[1] = aptid[1];
                                        aptxId[2] = aptid[2];
                                        AptxId = AptxId;
                                    }
                                }
                                if ((Current == Error) || (Maxi == Error))
                                {
                                    //if (data.Contains("pulses written"))
                                    //{
                                    //    uint remaining = DataParse(data, "pulses written", NumberStyles.Number);
                                    //    Remaining = remaining;
                                    //}
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
                                    if ((Current != Error) && (Maxi != Error))
                                        Remaining = Maxi - Current;
                                }
                                //if (data.Contains("Done Init"))
                                //{
                                //    DoneInit = true;
                                //}

                                //if ((BindingContext != null) &&
                                //    (BindingContext as CowIdViewModel)?.IsPageOpened == true)
                                //    return;
                                //
                                //BindingContext = new CowIdViewModel()
                                //{
                                //    Id = DependencyService.Get<IRemoteControlUsbDevice>().GetData(),
                                //};
                                //await Current.MainPage.Navigation.PushAsync(new CowIdPage());
                                //}
                                //catch { }
                            }
                        }
                    })
                    { Name = "UsbRx" }.Start();

                    new Thread(async () =>
                    {
                        while (UsbConnected)
                        {
                            while (!(await UsbDevice.Connect())) ;

                            while (string.IsNullOrEmpty(APTXPort))
                            {
                                Thread.Sleep(1000);

                                //if (string.IsNullOrEmpty(APTXPort) || string.IsNullOrEmpty(EcomilkPort))
                                foreach (string port in UsbDevice.GetPorts())
                                    await UsbDevice.Write(port, Encoding.UTF8.GetBytes("getid,3#"));
                            }

                            while (UsbConnected)
                            {
                                Thread.Sleep(1000);

                                if (SNum == Error)
                                    await UsbDevice.Write(APTXPort, Encoding.UTF8.GetBytes("testread,3#"));
                                else if (Current == Error)
                                    //App.DataModel.UsbDevice.Send("find,3#");
                                    await UsbDevice.Write(APTXPort, Encoding.UTF8.GetBytes("find,3#"));
                                else if ((aptxId[0] == Error) ||
                                         (aptxId[1] == Error) ||
                                         (aptxId[2] == Error))
                                    //App.DataModel.UsbDevice.Send("readid#");
                                    await UsbDevice.Write(APTXPort, Encoding.UTF8.GetBytes("readid#"));
                            }
                        }
                    })
                    { Name = "UsbTx" }.Start();
                }));
        }

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

        public async Task ZRCWStart()
        {
            await UsbDevice.Write(EcomilkPort, Encoding.UTF8.GetBytes("rcw 1"));
        }

        public async Task ZRCWStop()
        {
            await UsbDevice.Write(EcomilkPort, Encoding.UTF8.GetBytes("rcw 0"));
        }

        public async Task ZRCCWStart()
        {
            await UsbDevice.Write(EcomilkPort, Encoding.UTF8.GetBytes("rccw 1"));
        }

        public async Task ZRCCWStop()
        {
            await UsbDevice.Write(EcomilkPort, Encoding.UTF8.GetBytes("rccw 0"));
        }




        public async Task AYFStart()
        {
            await UsbDevice.Write(EcomilkPort, Encoding.UTF8.GetBytes("ayf 1"));
        }

        public async Task AYFStop()
        {
            await UsbDevice.Write(EcomilkPort, Encoding.UTF8.GetBytes("ayf 0"));
        }

        public async Task AYBStart()
        {
            await UsbDevice.Write(EcomilkPort, Encoding.UTF8.GetBytes("ayb 1"));
        }

        public async Task AYBStop()
        {
            await UsbDevice.Write(EcomilkPort, Encoding.UTF8.GetBytes("ayb 0"));
        }




        public async Task MZUStart()
        {
            await UsbDevice.Write(EcomilkPort, Encoding.UTF8.GetBytes("mzu 1"));
        }

        public async Task MZUStop()
        {
            await UsbDevice.Write(EcomilkPort, Encoding.UTF8.GetBytes("mzu 0"));
        }

        public async Task MZDStart()
        {
            await UsbDevice.Write(EcomilkPort, Encoding.UTF8.GetBytes("mzd 1"));
        }

        public async Task MZDStop()
        {
            await UsbDevice.Write(EcomilkPort, Encoding.UTF8.GetBytes("mzd 0"));
        }
    }
}
