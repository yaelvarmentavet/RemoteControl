using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RemoteControl.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class KinematicPage : ContentPage
    {
        private bool ZoomDown = true;

        public KinematicPage()
        {
            InitializeComponent();
        }

        private void RCWStart(object sender, EventArgs e)
        {
            Task.Run(async () =>
            {
                await App.DataModel.RCWStart();
            });
        }

        private void RCWStop(object sender, EventArgs e)
        {
            Task.Run(async () =>
            {
                await App.DataModel.RCWStop();
            });
        }

        private void RCCWStart(object sender, EventArgs e)
        {
            Task.Run(async () =>
            {
                await App.DataModel.RCCWStart();
            });
        }

        private void RCCWStop(object sender, EventArgs e)
        {
            Task.Run(async () =>
            {
                await App.DataModel.RCCWStop();
            });
        }

        private void AFStart(object sender, EventArgs e)
        {
            Task.Run(async () =>
            {
                await App.DataModel.AFStart();
            });
        }

        private void AFStop(object sender, EventArgs e)
        {
            Task.Run(async () =>
            {
                await App.DataModel.AFStop();
            });
        }

        private void ABStart(object sender, EventArgs e)
        {
            Task.Run(async () =>
            {
                await App.DataModel.AFStart();
            });
        }

        private void ABStop(object sender, EventArgs e)
        {
            Task.Run(async () =>
            {
                await App.DataModel.AFStop();
            });
        }

        private void MZUStart(object sender, EventArgs e)
        {
            Task.Run(async () =>
            {
                await App.DataModel.AFStart();
            });
        }

        private void MZUStop(object sender, EventArgs e)
        {
            Task.Run(async () =>
            {
                await App.DataModel.AFStop();
            });
        }

        private void MZDStart(object sender, EventArgs e)
        {
            Task.Run(async () =>
            {
                await App.DataModel.AFStart();
            });
        }

        private void MZDStop(object sender, EventArgs e)
        {
            Task.Run(async () =>
            {
                await App.DataModel.AFStop();
            });
        }

        private void Zoom(object sender, EventArgs e)
        {
            if (ZoomDown)
            {
                GrKin.Children.Remove(Cam1);
                GrKin.Children.Add(Cam1, 0, 0);
                Grid.SetRowSpan(Cam1, 18);
                Grid.SetColumnSpan(Cam1, 24);
                ZoomDown = false;
            }
            else
            {
                GrKin.Children.Remove(Cam1);
                GrKin.Children.Add(Cam1, 0, 0);
                Grid.SetRowSpan(Cam1, 8);
                Grid.SetColumnSpan(Cam1, 8);
                ZoomDown = true;
            }
        }
    }
}