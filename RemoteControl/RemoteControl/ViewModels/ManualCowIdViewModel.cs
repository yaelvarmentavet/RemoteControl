using RemoteControl.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Xamarin.Forms;

namespace RemoteControl.ViewModels
{
    class ManualCowIdViewModel : INotifyPropertyChanged
    {
        public ManualCowIdViewModel()
        {
            Approve = new Command(async () =>
            {
                await App.Current.MainPage.Navigation.PushAsync(new ProcessPage());
            });
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public Command Approve { get; }
    }
}
