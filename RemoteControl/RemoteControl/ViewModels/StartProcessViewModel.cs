using System.ComponentModel;
using Xamarin.Forms;

namespace RemoteControl.ViewModels
{
    class StartProcessViewModel : INotifyPropertyChanged
    {
        public StartProcessViewModel()
        {
            StopProcess = new Command(async () =>
            {
                await App.DataModel.StopProcess();
            });
            PauseResumeProcess = new Command(async () =>
            {
                await App.DataModel.PauseResumeProcess();
            });
        }
        public event PropertyChangedEventHandler PropertyChanged;

    public Command StopProcess { get; }
    public Command PauseResumeProcess { get; }

    }
}
