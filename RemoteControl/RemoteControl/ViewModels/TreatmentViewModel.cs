using RemoteControl.Views;
using System.ComponentModel;
using Xamarin.Forms;

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

            TappedFL = new Command(() =>
            {
                App.DataModel.FL = true;
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Command NextPageHome { get; }
        public Command NextPageSettings { get; }

        public Command TappedFL { get; }
    }
}
