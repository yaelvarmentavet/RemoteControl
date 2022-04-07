using System;
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

        }

    }
}