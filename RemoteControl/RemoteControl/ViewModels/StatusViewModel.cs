using RemoteControl.Views;
using Xamarin.Forms;

namespace RemoteControl.ViewModels
{
    class StatusViewModel
    {
        public StatusViewModel()
        {
            NextPageKinematic = new Command(() =>
             {
                 App.Current.MainPage.Navigation.PushAsync(new KinematicPage());
             });
        }
        public Command NextPageKinematic { get; }
    }
}
