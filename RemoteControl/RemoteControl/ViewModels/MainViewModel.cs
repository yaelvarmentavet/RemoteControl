using RemoteControl.Models;
using RemoteControl.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using Xamarin.Forms;

namespace RemoteControl.ViewModels
{
    class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel()
        {
            NextPageSettings = new Command(async () =>
            {
                await Application.Current.MainPage.Navigation.PushAsync(new SettingsPage());
            });
            NextPageCMT = new Command(async () =>
            {
                await Application.Current.MainPage.Navigation.PushAsync(new CMTPage());
            });
            NextPageTreatment = new Command(async () =>
            {
                await Application.Current.MainPage.Navigation.PushAsync(new TreatmentPage());
            });
            NextPageCowId = new Command(async () =>
            {
                await Application.Current.MainPage.Navigation.PushAsync(new COWIDManagementPage());
                //await Application.Current.MainPage.Navigation.PushAsync(new CowIdPage());
            });

            DependencyService.Get<IRemoteControlUsbDevice>().Event(async (sender, eventArgs) =>
            {
                string data = DependencyService.Get<IRemoteControlUsbDevice>().GetData();

                if (SNum == 0)
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
                if ((aptId[0] == 0) || (aptId[1] == 0) || (aptId[2] == 0))
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
                        aptId[0] = aptid[0];
                        aptId[1] = aptid[1];
                        aptId[2] = aptid[2];
                        AptId = AptId;
                    }
                }
                if (Remaining == 0)
                {
                    //if (data.Contains("pulses written"))
                    //{
                    //    uint remaining = DataParse(data, "pulses written", NumberStyles.Number);
                    //    Remaining = remaining;
                    //}
                    if (data.Contains("Found:"))
                    {
                        uint[] remaining = DataParse(data, "Found:", NumberStyles.Number);
                        Remaining = remaining[0];
                    }
                }
                if (data.Contains("Done Init"))
                {
                    DoneInit = true;
                }

                //if ((BindingContext != null) &&
                //    (BindingContext as CowIdViewModel)?.IsPageOpened == true)
                //    return;
                //
                //BindingContext = new CowIdViewModel()
                //{
                //    Id = DependencyService.Get<IRemoteControlUsbDevice>().GetData(),
                //};
                //await Current.MainPage.Navigation.PushAsync(new CowIdPage());
            });

            new Thread(async () =>
            {
                Thread.Sleep(10000);
                //while (!DoneInit)
                //{
                //}
                while (true)
                {
                    Thread.Sleep(1000);
                    if (SNum == 0)
                        DependencyService.Get<IRemoteControlUsbDevice>().Send("testread,3#");
                    else if (Remaining == 0)
                        DependencyService.Get<IRemoteControlUsbDevice>().Send("find,3#");
                    else if ((aptId[0] == 0) || (aptId[1] == 0) || (aptId[2] == 0))
                        DependencyService.Get<IRemoteControlUsbDevice>().Send("readid#");
                }
            })
            { Name = "UsbTx" }.Start();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        uint snum;
        public uint SNum
        {
            get => snum;
            set
            {
                snum = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SNum)));
            }
        }

        uint[] aptid = new uint[3];
        
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

        uint remaining;
        public uint Remaining
        {
            get => remaining;
            set
            {
                remaining = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Remaining)));
            }
        }
        
        private bool DoneInit;

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

        public Command NextPageSettings { get; }
        public Command NextPageCMT { get; }
        public Command NextPageTreatment { get; }
        public Command NextPageCowId { get; }

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
    }
}
