using CommonServiceLocator;
using RemoteControl.Views;
using System.ComponentModel;
using Xamarin.Forms;
using RemoteControl.Models;

namespace RemoteControl.ViewModels
{
    class TreatmentViewModel : INotifyPropertyChanged
    {
        public TreatmentViewModel()
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

        public Command NextPageHome { get; }
        public Command NextPageSettings { get; }
    }
}
