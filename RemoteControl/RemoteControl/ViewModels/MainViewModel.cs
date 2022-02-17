using RemoteControl.Models;
using RemoteControl.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
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

                if (data.Contains("str,serial_num:"))
                {
                    int snum = DataParse(data, "str,serial_num:", NumberStyles.Number);
                    SNum = snum;
                }
                if (data.Contains("CS 3 ADR: 1000 W:"))
                {
                    int aptid = DataParse(data, "CS 3 ADR: 1000 W:", NumberStyles.HexNumber);
                    aptId[0] = aptid;
                    aptId = aptId;
                    AptId = AptId;
                }
                if (data.Contains("CS 3 ADR: 1004 W:"))
                {
                    int aptid = DataParse(data, "CS 3 ADR: 1004 W:", NumberStyles.HexNumber);
                    aptId[1] = aptid;
                    aptId = aptId;
                    AptId = AptId;
                }
                if (data.Contains("CS 3 ADR: 1008 W:"))
                {
                    int aptid = DataParse(data, "CS 3 ADR: 1008 W:", NumberStyles.HexNumber);
                    aptId[2] = aptid;
                    aptId = aptId;
                    AptId = AptId;
                }

                //if (!data.Contains("testread"))
                //{
                //    await DependencyService.Get<IRemoteControlUsbDevice>().Send("testread,3#");
                //    return;
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
            });

            //new Thread (async () =>
            //{
            //    int resp = -1;
            //    while (resp < 0)
            //    {
            //        resp = await DependencyService.Get<IRemoteControlUsbDevice>().Send("testread,3#");
            //    }
            //}).Start();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        int snum;
        public int SNum
        {
            get => snum;
            set
            {
                snum = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SNum)));
            }
        }

        int[] aptid = new int[3];
        
        public int[] aptId
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

        private int DataParse(string data, string pattern, NumberStyles numberStyles)
        {
            int num;
            int.TryParse(new string(data?.Substring(data.IndexOf(pattern) + pattern.Length)?.TakeWhile(c => c != '\r')?.ToArray()),
                         NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | numberStyles,
                         CultureInfo.InvariantCulture,
                         out num);
            return num;
        }
    }
}
