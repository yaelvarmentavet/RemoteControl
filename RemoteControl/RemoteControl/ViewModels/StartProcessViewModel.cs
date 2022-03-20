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
                await App.DataModel.ProcessStop();
            });
            PauseResumeProcess = new Command(async () =>
            {
                await App.DataModel.ProcessPauseResume();
            });
        }
        public event PropertyChangedEventHandler PropertyChanged;

    public Command StopProcess { get; }
    public Command PauseResumeProcess { get; }

    }
}
