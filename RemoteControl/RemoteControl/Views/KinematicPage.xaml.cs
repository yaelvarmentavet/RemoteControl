using System;
using System.Collections.Generic;
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
    }
}