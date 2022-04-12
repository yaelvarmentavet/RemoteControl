using RemoteControl.Models;
using RemoteControl.Views;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace RemoteControl.ViewModels
{
    class MainViewModelAndroid : INotifyPropertyChanged
    {
        public MainViewModelAndroid()
        {
            //Task.Run(async () =>
            //{
            //    await App.DataModel.PortConnectRequest("RFID", "");
            //    await App.DataModel.PortConnectReply("RFID", "");
            //    await App.DataModel.PortConnectReply("REMOTE", "");
            //});

            NextPageSettings = new Command(async () =>
            {
                await Application.Current.MainPage.Navigation.PushAsync(new SettingsPage());
            });
            NextPageCowId = new Command(async () =>
            {
                await Application.Current.MainPage.Navigation.PushAsync(new CowIdPage());
                //await Application.Current.MainPage.Navigation.PushAsync(new CowIdPage());
            });

            //DataModel dataModel = ServiceLocator.Current.GetInstance<DataModel>();
            //DependencyService.Get<IUsbDevice>().Event(async (sender, eventArgs) =>
            //App.DataModel.UsbDevice.Event(async (sender, eventArgs) =>
            //{
            //    //string data = DependencyService.Get<IUsbDevice>().GetData();
            //    string data = App.DataModel.UsbDevice.GetData();

            //    if (App.DataModel.SNum == DataModel.Error)
            //    {
            //        //if (data.Contains("str,serial_num:"))
            //        //{
            //        //    uint snum = DataParse(data, "str,serial_num:", NumberStyles.Number);
            //        //    SNum = snum;
            //        //}
            //        if (data.Contains("SNUM"))
            //        {
            //            uint[] snum = DataParse(data, "SNUM", NumberStyles.Number);
            //            App.DataModel.SNum = snum[0];
            //        }
            //    }
            //    if ((App.DataModel.aptxId[0] == DataModel.Error) || 
            //        (App.DataModel.aptxId[1] == DataModel.Error) || 
            //        (App.DataModel.aptxId[2] == DataModel.Error))
            //    {
            //        //if (data.Contains("CS 3 ADR: 1000 W:"))
            //        //{
            //        //    uint aptid = DataParse(data, "CS 3 ADR: 1000 W:", NumberStyles.HexNumber);
            //        //    aptId[0] = aptid;
            //        //    aptId = aptId;
            //        //    AptId = AptId;
            //        //}
            //        //if (data.Contains("CS 3 ADR: 1004 W:"))
            //        //{
            //        //    uint aptid = DataParse(data, "CS 3 ADR: 1004 W:", NumberStyles.HexNumber);
            //        //    aptId[1] = aptid;
            //        //    aptId = aptId;
            //        //    AptId = AptId;
            //        //}
            //        //if (data.Contains("CS 3 ADR: 1008 W:"))
            //        //{
            //        //    uint aptid = DataParse(data, "CS 3 ADR: 1008 W:", NumberStyles.HexNumber);
            //        //    aptId[2] = aptid;
            //        //    aptId = aptId;
            //        //    AptId = AptId;
            //        //}
            //        if (data.Contains("readid Device_id"))
            //        {
            //            uint[] aptid = DataParse(data, "readid Device_id", NumberStyles.HexNumber);
            //            App.DataModel.aptxId[0] = aptid[0];
            //            App.DataModel.aptxId[1] = aptid[1];
            //            App.DataModel.aptxId[2] = aptid[2];
            //            App.DataModel.AptxId = App.DataModel.AptxId;
            //        }
            //    }
            //    if ((App.DataModel.Current == DataModel.Error) || (App.DataModel.Maxi == DataModel.Error))
            //    {
            //        //if (data.Contains("pulses written"))
            //        //{
            //        //    uint remaining = DataParse(data, "pulses written", NumberStyles.Number);
            //        //    Remaining = remaining;
            //        //}
            //        if (data.Contains("MAXI"))
            //        {
            //            uint[] maxi = DataParse(data, "MAXI", NumberStyles.Number);
            //            App.DataModel.Maxi = maxi[0];
            //        }
            //        if (data.Contains("Found:"))
            //        {
            //            uint[] current = DataParse(data, "Found:", NumberStyles.Number);
            //            App.DataModel.Current = current[0];
            //        }
            //        App.DataModel.Remaining = App.DataModel.Maxi - App.DataModel.Current;
            //    }
            //    if (data.Contains("Done Init"))
            //    {
            //        DoneInit = true;
            //    }

            //    //if ((BindingContext != null) &&
            //    //    (BindingContext as CowIdViewModel)?.IsPageOpened == true)
            //    //    return;
            //    //
            //    //BindingContext = new CowIdViewModel()
            //    //{
            //    //    Id = DependencyService.Get<IRemoteControlUsbDevice>().GetData(),
            //    //};
            //    //await Current.MainPage.Navigation.PushAsync(new CowIdPage());
            //});

            //new Thread(async () =>
            //{
            //    Thread.Sleep(10000);
            //    //while (!DoneInit)
            //    //{
            //    //}
            //    while (true)
            //    {
            //        Thread.Sleep(1000);
            //        if (App.DataModel.SNum == DataModel.Error)
            //            //DependencyService.Get<IUsbDevice>().Send("testread,3#");
            //            App.DataModel.UsbDevice.Send("testread,3#");
            //        else if (App.DataModel.Current == DataModel.Error)
            //            //DependencyService.Get<IUsbDevice>().Send("find,3#");
            //            App.DataModel.UsbDevice.Send("find,3#");
            //        else if ((App.DataModel.aptxId[0] == DataModel.Error) || 
            //                 (App.DataModel.aptxId[1] == DataModel.Error) || 
            //                 (App.DataModel.aptxId[2] == DataModel.Error))
            //            //DependencyService.Get<IUsbDevice>().Send("readid#");
            //            App.DataModel.UsbDevice.Send("readid#");
            //    }
            //})
            //{ Name = "UsbTx" }.Start();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<string> AllNotes { get; set; }

        string theNote;

        public string TheNote
        {
            get => theNote;
            set
            {
                theNote = value;
                var args = new PropertyChangedEventArgs(nameof(theNote));
                PropertyChanged?.Invoke(this, args);
            }
        }

        public Command NextPageCowId { get; }
        public Command NextPageSettings { get; }

        //private uint[] DataParse(string data, string pattern, NumberStyles numberStyles)
        //{
        //    uint[] num = new uint[6];
        //    string snum = new string(data?.Substring(data.IndexOf(pattern) + pattern.Length)?.TakeWhile(c => ((c != '\r') && (c != 'p')))?.ToArray());
        //    if (((numberStyles == NumberStyles.HexNumber) && (snum.Length <= 8)) || 
        //        ((numberStyles == NumberStyles.Number) && (snum.Length <= 10)))
        //    {
        //        uint.TryParse(snum,
        //                     NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | numberStyles,
        //                     CultureInfo.InvariantCulture,
        //                     out num[0]);
        //    }
        //    else
        //    {
        //        for (int i = 0; i < snum.Length / 8; i++)
        //        {
        //            uint.TryParse(new string(snum.Skip(i * 8).Take(8).ToArray()),
        //                         NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | numberStyles,
        //                         CultureInfo.InvariantCulture,
        //                         out num[i]);
        //        }
        //    }
        //    return num;
        //}
    }
}
