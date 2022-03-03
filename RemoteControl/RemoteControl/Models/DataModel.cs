using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;

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

        uint[] aptid = new uint[3] { Error, Error, Error };

        public uint[] aptId
        {
            get => aptid;
            set
            {
                aptid = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(aptId)));
            }
        }

        public string AptId
        {
            get => aptid.Aggregate("", (r, m) => r += m.ToString("X") + "   ");
            set
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AptId)));
            }
        }

        public IUsbInterface UsbDevice;

        public event PropertyChangedEventHandler PropertyChanged;

        public DataModel()
        { }
        public DataModel(IUsbInterface usbDevice)
        {
            UsbDevice = usbDevice;

            new Thread(async () =>
            {
                string data = string.Empty;
                while (true)
                {
                    if (UsbDevice.GetPorts().Count() > 0)
                    {
                        try
                        {
                            byte[] buffer = new byte[1024];
                            data += await App.DataModel.UsbDevice.Read(UsbDevice.GetPorts().First(), buffer);
                            string data1 = Encoding.UTF8.GetString(buffer);
                            if (App.DataModel.SNum == DataModel.Error)
                            {
                                //if (data.Contains("str,serial_num:"))
                                //{
                                //    uint snum = DataParse(data, "str,serial_num:", NumberStyles.Number);
                                //    SNum = snum;
                                //}
                                if (data.Contains("SNUM"))
                                {
                                    uint[] snum = DataParse(data, "SNUM", NumberStyles.Number);
                                    App.DataModel.SNum = snum[0];
                                }
                            }
                            if ((App.DataModel.aptId[0] == DataModel.Error) ||
                                (App.DataModel.aptId[1] == DataModel.Error) ||
                                (App.DataModel.aptId[2] == DataModel.Error))
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
                                    App.DataModel.aptId[0] = aptid[0];
                                    App.DataModel.aptId[1] = aptid[1];
                                    App.DataModel.aptId[2] = aptid[2];
                                    App.DataModel.AptId = App.DataModel.AptId;
                                }
                            }
                            if ((App.DataModel.Current == DataModel.Error) || (App.DataModel.Maxi == DataModel.Error))
                            {
                                //if (data.Contains("pulses written"))
                                //{
                                //    uint remaining = DataParse(data, "pulses written", NumberStyles.Number);
                                //    Remaining = remaining;
                                //}
                                if (data.Contains("MAXI"))
                                {
                                    uint[] maxi = DataParse(data, "MAXI", NumberStyles.Number);
                                    App.DataModel.Maxi = maxi[0];
                                }
                                if (data.Contains("Found:"))
                                {
                                    uint[] current = DataParse(data, "Found:", NumberStyles.Number);
                                    App.DataModel.Current = current[0];
                                }
                                App.DataModel.Remaining = App.DataModel.Maxi - App.DataModel.Current;
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
                        }
                        catch { }
                    }
                }
            })
            { Name = "UsbRx" }.Start();

            new Thread(() =>
            {
                while (true)
                {
                    if (UsbDevice.GetPorts().Count()>0)
                    {
                        Thread.Sleep(1000);

                        if (App.DataModel.SNum == DataModel.Error)
                            App.DataModel.UsbDevice.Write(UsbDevice.GetPorts().First(), Encoding.UTF8.GetBytes("testread,3#"));
                        else if (App.DataModel.Current == DataModel.Error)
                            //App.DataModel.UsbDevice.Send("find,3#");
                            App.DataModel.UsbDevice.Write(UsbDevice.GetPorts().First(), Encoding.UTF8.GetBytes("find, 3#"));
                        else if ((App.DataModel.aptId[0] == DataModel.Error) ||
                                 (App.DataModel.aptId[1] == DataModel.Error) ||
                                 (App.DataModel.aptId[2] == DataModel.Error))
                            //App.DataModel.UsbDevice.Send("readid#");
                            App.DataModel.UsbDevice.Write(UsbDevice.GetPorts().First(), Encoding.UTF8.GetBytes("readid#"));
                    }
                }
            })
            { Name = "UsbTx" }.Start();
        }

        private uint[] DataParse(string data, string pattern, NumberStyles numberStyles)
        {
            uint[] num = new uint[6];
            string snum = new string(data?.Substring(data.IndexOf(pattern) + pattern.Length)?.TakeWhile(c => ((c != '\r') && (c != 'p')))?.ToArray());
            //if (((numberStyles == NumberStyles.HexNumber) && (snum.Length <= 8)) ||
            //    ((numberStyles == NumberStyles.Number) && (snum.Length <= 10)))
            //{
            //    uint.TryParse(snum,
            //                 NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | numberStyles,
            //                 CultureInfo.InvariantCulture,
            //                 out num[0]);
            //}
            //else
            //{
            for (int i = 0; i < snum.Length / 8; i++)
            {
                uint.TryParse(new string(snum.Skip(i * 8).Take(8).ToArray()),
                             NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | numberStyles,
                             CultureInfo.InvariantCulture,
                             out num[i]);
            }
            //}
            return num;
        }
    }
}
