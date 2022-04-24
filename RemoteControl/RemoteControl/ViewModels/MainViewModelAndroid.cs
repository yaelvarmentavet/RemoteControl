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
            NextPageSettings = new Command(async () =>
            {
                await Application.Current.MainPage.Navigation.PushAsync(new SettingsPage());
            });
            NextPageCowId = new Command(async () =>
            {
                await Application.Current.MainPage.Navigation.PushAsync(new CowIdPage());
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        //public ObservableCollection<string> AllNotes { get; set; }

        //string theNote;

        //public string TheNote
        //{
        //    get => theNote;
        //    set
        //    {
        //        theNote = value;
        //        var args = new PropertyChangedEventArgs(nameof(theNote));
        //        PropertyChanged?.Invoke(this, args);
        //    }
        //}

        public Command NextPageCowId { get; }
        public Command NextPageSettings { get; }
    }
}
