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

        public Command AddCow { get; }
        public Command NextPageHome { get; }
        public Command NextPageSettings { get; }
    }
}
