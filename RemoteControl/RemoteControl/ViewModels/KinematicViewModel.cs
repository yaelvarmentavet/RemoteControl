using RemoteControl.Views;
using System;
using System.ComponentModel;
using System.Linq;
using Xamarin.Forms;

namespace RemoteControl.ViewModels
{
    class KinematicViewModel : INotifyPropertyChanged
    {
        private bool ZoomDown = true;
        public KinematicViewModel()
        {
            //RCWStart = new Command(async () =>
            //{
            //    await App.DataModel.RCWStart();
            //});
            //RCWStop = new Command(async () =>
            //{
            //    App.DataModel.RCWStop();
            //});
            //RCCWStart = new Command(async () =>
            //{
            //    App.DataModel.RCCWStart();
            //});
            //RCCWStop = new Command(async () =>
            //{
            //    App.DataModel.RCCWStop();
            //});

            //AFStart = new Command(async () =>
            //{
            //    App.DataModel.AFStart();
            //});
            //AFStop = new Command(async () =>
            //{
            //    App.DataModel.AFStop();
            //});
            //ABStart = new Command(async () =>
            //{
            //    App.DataModel.ABStart();
            //});
            //ABStop = new Command(async () =>
            //{
            //    App.DataModel.ABStop();
            //});

            //MZUStart = new Command(async () =>
            //{
            //    App.DataModel.MZUStart();
            //});
            //MZUStop = new Command(async () =>
            //{
            //    App.DataModel.MZUStop();
            //});
            //MZDStart = new Command(async () =>
            //{
            //    App.DataModel.MZDStart();
            //});
            //MZDStop = new Command(async () =>
            //{
            //    App.DataModel.MZDStop();
            //});
            //TCWStart = new Command(async () =>
            //{
            //    App.DataModel.TCWStart();
            //});
            //XFStart = new Command(async () =>
            //{
            //    App.DataModel.XFStart();
            //});

            StartProcessPage = new Command(async () =>
            {
                await Application.Current.MainPage.Navigation.PushAsync(new StartProcessPage());
                await App.DataModel.ProcessStart();
            });

            Zoom = new Command((cameraView) =>
            {
                //Application.Current.MainPage.Navigation.NavigationStack.Where(n =>
                foreach (Page n in Application.Current.MainPage.Navigation.NavigationStack)
                {
                    if (n is KinematicPage)
                    {
                        (n as KinematicPage).Zoom(cameraView as View);
                        //return true;
                    }
                    //return false;
                    //});
                }
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        //public Command RCWStart { get; }
        //public Command RCWStop { get; }
        //public Command RCCWStart { get; }
        //public Command RCCWStop { get; }
        //public Command AFStart { get; }
        //public Command AFStop { get; }
        //public Command ABStart { get; }
        //public Command ABStop { get; }
        //public Command MZUStart { get; }
        //public Command MZUStop { get; }
        //public Command MZDStart { get; }
        //public Command MZDStop { get; }
        //public Command TCWStart { get; }
        //public Command XFStart { get; }

        public Command StartProcessPage { get; }
        public Command Zoom { get; }
    }
}
