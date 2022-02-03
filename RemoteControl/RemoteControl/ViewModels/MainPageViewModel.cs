using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using Xamarin.Forms;

namespace RemoteControl
{
    class MainPageViewModel : INotifyPropertyChanged
    {
        public MainPageViewModel()
        {
            NextPage1 = new Command(async () =>
            {
                await Application.Current.MainPage.Navigation.PushAsync(new CMTPage());
            });
            NextPage2 = new Command(async () =>
            {
                await Application.Current.MainPage.Navigation.PushAsync(new TreatmentPage());
            });
            NextPage3 = new Command(async () =>
            {
                await Application.Current.MainPage.Navigation.PushAsync(new COWIDManagementPage());
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
        public Command NextPage1 { get; }
        public Command NextPage2 { get;}
        public Command NextPage3 { get; }
    }
}
