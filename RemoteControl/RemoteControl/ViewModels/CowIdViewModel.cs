using RemoteControl.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Xamarin.Forms;

namespace RemoteControl.ViewModels
{
    public class CowIdViewModel : INotifyPropertyChanged
    {
        public CowIdViewModel()
        {
            NextPageHome = new Command(async () =>
            {
                await Application.Current.MainPage.Navigation.PopToRootAsync();
            });
            NextPageSettings = new Command(async () =>
            {
                await Application.Current.MainPage.Navigation.PushAsync(new SettingsPage());
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsPageOpened = false;

        string id;
        public string Id
        {
            get { return id; }
            set
            {
                id = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(id)));
            }
        }

        public Command NextPageHome { get; }
        public Command NextPageSettings { get; }
    }
}
