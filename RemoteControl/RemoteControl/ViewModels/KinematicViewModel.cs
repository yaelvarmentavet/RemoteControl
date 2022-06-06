using RemoteControl.Views;
using System;
using System.ComponentModel;
using System.Linq;
using Xamarin.Forms;
using static RemoteControl.Views.CustomButton;

namespace RemoteControl.ViewModels
{
    class KinematicViewModel : INotifyPropertyChanged
    {
        private bool ZoomDown = true;
        public KinematicViewModel()
        {
            //RCWStart = new Command(() =>
            //{
            //    App.DataModel.RCWStart();
            //});
            //RCWStop = new Command(() =>
            //{
            //    App.DataModel.RCWStop();
            //});
            RCWStart = new DBindableEvent(() =>
            {
                App.DataModel.RCWStart();
            });
            RCWStop = new DBindableEvent(() =>
            {
                App.DataModel.RCWStop();
            });
            RCCWStart = new DBindableEvent(() =>
            {
                App.DataModel.RCCWStart();
            });
            RCCWStop = new DBindableEvent(() =>
            {
                App.DataModel.RCCWStop();
            });

            AFStart = new DBindableEvent(() =>
            {
                App.DataModel.AFStart();
            });
            AFStop = new DBindableEvent(() =>
            {
                App.DataModel.AFStop();
            });
            ABStart = new DBindableEvent(() =>
            {
                App.DataModel.ABStart();
            });
            ABStop = new DBindableEvent(() =>
            {
                App.DataModel.ABStop();
            });

            MZUStart = new DBindableEvent(() =>
            {
                App.DataModel.MZUStart();
            });
            MZUStop = new DBindableEvent(() =>
            {
                App.DataModel.MZUStop();
            });
            MZDStart = new DBindableEvent(() =>
            {
                App.DataModel.MZDStart();
            });
            MZDStop = new DBindableEvent(() =>
            {
                App.DataModel.MZDStop();
            });
            TCWStart = new DBindableEvent(() =>
            {
                App.DataModel.TCWStart();
            });
            TCWStop = new DBindableEvent(() =>
            {
                App.DataModel.TCWStop();
            });

            TCCWStart = new DBindableEvent(() =>
            {
                App.DataModel.TCCWStart();
            });

            TCCWStop = new DBindableEvent(() =>
            {
                App.DataModel.TCCWStop();
            });

            XFStart = new Command(() =>
            {
                App.DataModel.XFStart();
            });

            StartProcessPage = new Command(async () =>
            {
                await Application.Current.MainPage.Navigation.PushAsync(new StartProcessPage());
                App.DataModel.ProcessStart();
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
        public DBindableEvent RCWStart { get; }
        public DBindableEvent RCWStop { get; }
        public DBindableEvent RCCWStart { get; }
        public DBindableEvent RCCWStop { get; }
        public DBindableEvent AFStart { get; }
        public DBindableEvent AFStop { get; }
        public DBindableEvent ABStart { get; }
        public DBindableEvent ABStop { get; }
        public DBindableEvent MZUStart { get; }
        public DBindableEvent MZUStop { get; }
        public DBindableEvent MZDStart { get; }
        public DBindableEvent MZDStop { get; }
        public DBindableEvent TCWStart { get; }
        public DBindableEvent TCWStop { get; }
        public DBindableEvent TCCWStart { get; }
        public DBindableEvent TCCWStop { get; }
        public Command XFStart { get; }

        public Command StartProcessPage { get; }
        public Command Zoom { get; }
    }
}
