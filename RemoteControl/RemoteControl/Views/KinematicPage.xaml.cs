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
        private bool ZoomUp = true;
        private int CamRow;
        private int CamCol;

        public KinematicPage()
        {
            InitializeComponent();
        }

        //private void RCWStart(object sender, EventArgs e)
        //{
        //    App.DataModel.RCWStart();
        //}

        //private void RCWStop(object sender, EventArgs e)
        //{
        //    App.DataModel.RCWStop();
        //}

        //private void RCCWStart(object sender, EventArgs e)
        //{
        //    App.DataModel.RCCWStart();
        //}

        //private void RCCWStop(object sender, EventArgs e)
        //{
        //    App.DataModel.RCCWStop();
        //}

        //private void AFStart(object sender, EventArgs e)
        //{
        //    App.DataModel.AFStart();
        //}

        //private void AFStop(object sender, EventArgs e)
        //{
        //    App.DataModel.AFStop();
        //}

        //private void ABStart(object sender, EventArgs e)
        //{
        //    App.DataModel.ABStart();
        //}

        //private void ABStop(object sender, EventArgs e)
        //{
        //    App.DataModel.ABStop();
        //}

        //private void MZUStart(object sender, EventArgs e)
        //{
        //    App.DataModel.MZUStart();
        //}

        //private void MZUStop(object sender, EventArgs e)
        //{
        //    App.DataModel.MZUStop();
        //}

        //private void MZDStart(object sender, EventArgs e)
        //{
        //    App.DataModel.MZDStart();
        //}

        //private void MZDStop(object sender, EventArgs e)
        //{
        //    App.DataModel.MZDStop();
        //}

        //private void TCWStart(object sender, EventArgs e)
        //{
        //    App.DataModel.TCWStart();
        //}

        //private void TCWStop(object sender, EventArgs e)
        //{
        //    App.DataModel.TCWStop();
        //}

        //private void TCCWStart(object sender, EventArgs e)
        //{
        //    App.DataModel.TCCWStart();
        //}

        //private void TCCWStop(object sender, EventArgs e)
        //{
        //    App.DataModel.TCCWStop();
        //}

        public void Zoom(View cameraView)
        {
            if (ZoomUp)
            {
                CamRow = Grid.GetRow(cameraView);
                CamCol = Grid.GetColumn(cameraView);

                GrKin.Children.Remove(cameraView);
                GrKin.Children.Add(cameraView, 0, 0);
                Grid.SetRowSpan(cameraView, 18);
                Grid.SetColumnSpan(cameraView, 24);
                ZoomUp = false;
            }
            else
            {
                GrKin.Children.Remove(cameraView);
                GrKin.Children.Add(cameraView, CamCol, CamRow);
                Grid.SetRowSpan(cameraView, 8);
                Grid.SetColumnSpan(cameraView, 8);
                ZoomUp = true;
            }
        }
    }
}