using RemoteControl.Views;
using Xamarin.Forms;

namespace RemoteControl.ViewModels
{
    class StatusViewModel
    {
        public StatusViewModel()
        {
            NextPage = new Command(() =>
             {
                 App.Current.MainPage.Navigation.PushAsync(new KinematicPage());
             });
        }
        public Command NextPage { get; }
    }
}
