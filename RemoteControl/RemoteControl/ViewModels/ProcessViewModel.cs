using RemoteControl.Views;
using System.ComponentModel;
using Xamarin.Forms;

namespace RemoteControl.ViewModels
{
    class ProcessViewModel : INotifyPropertyChanged
    {
        public ProcessViewModel()
        {
            
            Save = new Command(async () =>
            {
                await Application.Current.MainPage.Navigation.PopToRootAsync();
                App.DataModel.CmtSave();
            });
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

        public Command Save { get; }
        public Command NextPageHome { get; }
        public Command NextPageSettings { get; }
    }
}
