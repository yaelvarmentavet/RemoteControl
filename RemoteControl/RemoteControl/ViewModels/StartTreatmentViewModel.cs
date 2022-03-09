using System.ComponentModel;
using Xamarin.Forms;

namespace RemoteControl.ViewModels
{
    class StartTreatmentViewModel : INotifyPropertyChanged
    {
        public StartTreatmentViewModel()
        {
            Start = new Command(async () =>
            {
                
            });
        }
    public event PropertyChangedEventHandler PropertyChanged;

    public Command Start { get; }

    }
}
