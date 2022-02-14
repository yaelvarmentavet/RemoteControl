using RemoteControl.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Xamarin.Forms;

namespace RemoteControl.ViewModels
{
    class SettingsViewModel : INotifyPropertyChanged
    {
        public SettingsViewModel()
        {
            NextPageHome = new Command(async () =>
            {
                await Application.Current.MainPage.Navigation.PopToRootAsync();
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Command NextPageHome { get; }
    }
}
