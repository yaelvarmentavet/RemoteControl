using RemoteControl.Views;
using System.ComponentModel;
using Xamarin.Forms;

namespace RemoteControl.ViewModels
{
    class ManualCowIdViewModel : INotifyPropertyChanged
    {
        public ManualCowIdViewModel()
        {
            NextPageStatus = new Command(async () =>
            {
                await App.Current.MainPage.Navigation.PushAsync(new StatusPage());
            });
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public Command NextPageStatus { get; }
    }
}
