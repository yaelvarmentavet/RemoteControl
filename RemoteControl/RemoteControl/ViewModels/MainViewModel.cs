using RemoteControl.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
        }

        public ObservableCollection<string> AllNotes { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

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
    }
}
