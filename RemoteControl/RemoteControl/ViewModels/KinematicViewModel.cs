using RemoteControl.Views;
using System.ComponentModel;
using Xamarin.Forms;

namespace RemoteControl.ViewModels
{
    class KinematicViewModel : INotifyPropertyChanged
    {
        public KinematicViewModel()
        {
            ZRCWStart = new Command(async () =>
            {
                App.DataModel.ZRCWStart();
            });
            ZRCWStop = new Command(async () =>
            {
                App.DataModel.ZRCWStop();
            });
            ZRCCWStart = new Command(async () =>
            {
                App.DataModel.ZRCCWStart();
            });
            ZRCCWStop = new Command(async () =>
            {
                App.DataModel.ZRCCWStop();
            });

            AYFStart = new Command(async () =>
            {
                App.DataModel.AYFStart();
            });
            AYFStop = new Command(async () =>
            {
                App.DataModel.AYFStop();
            });
            AYBStart = new Command(async () =>
            {
                App.DataModel.AYBStart();
            });
            AYBStop = new Command(async () =>
            {
                App.DataModel.AYBStop();
            });

            MZUStart = new Command(async () =>
            {
                App.DataModel.MZUStart();
            });
            MZUStop = new Command(async () =>
            {
                App.DataModel.MZUStop();
            });
            MZDStart = new Command(async () =>
            {
                App.DataModel.MZDStart();
            });
            MZDStop = new Command(async () =>
            {
                App.DataModel.MZDStop();
            });

            StartTreatmentPage = new Command(async () =>
            {
                await Application.Current.MainPage.Navigation.PushAsync(new StartTreatmentPage());
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Command ZRCWStart { get; }
        public Command ZRCWStop { get; }
        public Command ZRCCWStart { get; }
        public Command ZRCCWStop { get; }
        public Command AYFStart { get; }
        public Command AYFStop { get; }
        public Command AYBStart { get; }
        public Command AYBStop { get; }
        public Command MZUStart { get; }
        public Command MZUStop { get; }
        public Command MZDStart { get; }
        public Command MZDStop { get; }

        public Command EmptyBath { get; }
        public Command ManualEnable { get; }
        public Command BrakesOnOff { get; }
        public Command StartTreatmentPage { get; }
    }
}
