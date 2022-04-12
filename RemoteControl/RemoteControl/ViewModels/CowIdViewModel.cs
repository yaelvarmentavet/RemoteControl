using RemoteControl.Models;
using RemoteControl.Views;
using System.ComponentModel;
using Xamarin.Forms;

namespace RemoteControl.ViewModels
{
    public class CowIdViewModel : INotifyPropertyChanged
    {
        public CowIdViewModel()
        {
            AddCow = new Command(async () =>
            {
                await Application.Current.MainPage.Navigation.PopToRootAsync();
                if (App.DataModel.CowId != DataModel.UERROR)
                {
                    App.DataModel.CmtRead();
                    App.DataModel.CowIdOk = true;
                }
                else
                {
                    App.DataModel.CowIdOk = false;
                }
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

        public bool IsPageOpened = false;

        public Command AddCow { get; }
        public Command NextPageHome { get; }
        public Command NextPageSettings { get; }
    }
}
