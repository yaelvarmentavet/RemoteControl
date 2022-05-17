using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RemoteControl.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class KinematicPage : ContentPage
    {
        private bool _zoomUp = true;
        private int _camRow;
        private int _camCol;

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

        //protected override void OnDisappearing()
        //{
        //    base.OnDisappearing();
        //    grkin.children.remove(cam1);
        //    grkin.children.remove(cam2);
        //    GrKin.Children.Remove(Cam2);
        //}

        //protected override void OnAppearing()
        //{
        //    base.OnAppearing();
        //    //CamRefresh(Cam1);
        //    //CamRefresh(Cam2);
        //    //CamRefresh(Cam3);
        //    //App.Current.MainPage.Navigation.PushAsync(new KinematicPage());
        //}

        //private void CamRefresh(View cameraView)
        //    {
        //        CamRow = Grid.GetRow(cameraView);
        //        CamCol = Grid.GetColumn(cameraView);
        //        GrKin.Children.Remove(cameraView);
        //        GrKin.Children.Add(cameraView, CamCol, CamRow);
        //        Grid.SetRowSpan(cameraView, 8);
        //        Grid.SetColumnSpan(cameraView, 8);
        //        //double scale = cameraView.Scale;
        //        //cameraView.Scale = 1;
        //        //cameraView.Scale = scale;
        //    }

        public void Zoom(View cameraView)
        {
            if (_zoomUp)
            {
                _camRow = Grid.GetRow(cameraView);
                _camCol = Grid.GetColumn(cameraView);

                GrKin.Children.Remove(cameraView);
                GrKin.Children.Add(cameraView, 0, 0);
                Grid.SetRowSpan(cameraView, 18);
                Grid.SetColumnSpan(cameraView, 24);
                _zoomUp = false;
            }
            else
            {
                GrKin.Children.Remove(cameraView);
                GrKin.Children.Add(cameraView, _camCol, _camRow);
                Grid.SetRowSpan(cameraView, 8);
                Grid.SetColumnSpan(cameraView, 8);
                _zoomUp = true;
            }
        }
    }
}