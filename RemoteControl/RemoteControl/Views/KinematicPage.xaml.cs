using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RemoteControl.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class KinematicPage : ContentPage
    {
        public KinematicPage()
        {
            InitializeComponent();

            //ImgCam1.BindingContext = App.DataModel;
            //App.DataModel.UsbCamera.ImageSet(ImgCam1);

            //App.DataModel.UsbCamera.ImageSet(new List<Image>() { ImgCam1, ImgCam2, ImgCam3 });

        }

        private void RCWStart(object sender, EventArgs e)
        {
            App.DataModel.RCWStart();
        }

        private void RCWStop(object sender, EventArgs e)
        {
            App.DataModel.RCWStop();
        }

        private void RCCWStart(object sender, EventArgs e)
        {
            App.DataModel.RCCWStart();
        }

        private void RCCWStop(object sender, EventArgs e)
        {
            App.DataModel.RCCWStop();
        }

        private void AFStart(object sender, EventArgs e)
        {
            App.DataModel.AFStart();
        }

        private void AFStop(object sender, EventArgs e)
        {
            App.DataModel.AFStop();
        }

        private void ABStart(object sender, EventArgs e)
        {
            App.DataModel.AFStart();
        }

        private void ABStop(object sender, EventArgs e)
        {
            App.DataModel.AFStop();
        }

        private void MZUStart(object sender, EventArgs e)
        {
            App.DataModel.AFStart();
        }

        private void MZUStop(object sender, EventArgs e)
        {
            App.DataModel.AFStop();
        }

        private void MZDStart(object sender, EventArgs e)
        {
            App.DataModel.AFStart();
        }

        private void MZDStop(object sender, EventArgs e)
        {
            App.DataModel.AFStop();
        }
    }
}