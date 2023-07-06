using RemoteControl.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Xamarin.Forms;

namespace RemoteControl.ViewModels
{
    class MainViewModelEcomilk : INotifyPropertyChanged
    {
        public MainViewModelEcomilk()
        {
            NextPageCowId = new Command(async () =>
            {
                await App.Current.MainPage.Navigation.PushAsync(new ManualCowIdPage());
                //await App.DataModel.PortConnectRequest("RFID", "");
                //await App.DataModel.PortConnectReply("RFID", "");
                //await App.DataModel.PortConnectReply("REMOTE", "");
            });
            NextPageStatus = new Command(async () =>
            {
                await App.Current.MainPage.Navigation.PushAsync(new StatusPage());
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Command NextPageCowId{ get; }
        public Command NextPageStatus { get; }
    }
}