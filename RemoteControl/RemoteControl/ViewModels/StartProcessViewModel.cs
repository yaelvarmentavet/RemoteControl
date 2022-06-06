using System.ComponentModel;
using Xamarin.Forms;

namespace RemoteControl.ViewModels
{
    class StartProcessViewModel : INotifyPropertyChanged
    {
        public StartProcessViewModel()
        {
            StopProcess = new Command(() =>
            {
                App.DataModel.ProcessStop();
            });
            PauseResumeProcess = new Command(() =>
            {
                App.DataModel.ProcessPauseResume();
            });
        }
        public event PropertyChangedEventHandler PropertyChanged;

    public Command StopProcess { get; }
    public Command PauseResumeProcess { get; }

    }
}
